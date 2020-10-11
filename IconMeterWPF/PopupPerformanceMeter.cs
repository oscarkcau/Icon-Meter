﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

namespace IconMeterWPF
{
	class PopupPerformanceMeter : IDisposable, INotifyPropertyChanged
	{
		// inner class for storing process information
		public class ProcessPerformance
		{
			public string Name { get; set; }
			public ulong WorkingSet { get; set; }
			public long ProcessorTime { get; set; }

			public ProcessPerformance(string name, ulong workingSet, long processorTime)
			{
				this.Name = name;
				this.WorkingSet = workingSet;
				this.ProcessorTime = processorTime;

				if (ProcessorTime < 0) ProcessorTime = 0;
			}

			public string FormattedProcessorTime { get => $"{ProcessorTime}%"; }
			public string FormattedWorkingSet
			{
				get
				{
					return GetFormattedSize(WorkingSet);
				}
			}
		}

		// inner class for storing disk performance information
		public class DiskPerformance
		{
			public string Name { get; set; }
			public int ActiveTime { get; set; }
			public ulong ReadSpeed { get; set; }
			public ulong WriteSpeed { get; set; }

			public DiskPerformance(string name, int activeTime, ulong readSpeed, ulong writeSpeed)
			{
				this.Name = name;
				this.ActiveTime = activeTime;
				this.ReadSpeed = readSpeed;
				this.WriteSpeed = writeSpeed;

				if (ActiveTime < 0) ActiveTime = 0;
			}

			public string FormattedActiveTime { get => $"{ActiveTime}%"; }
			public string FormattedReadSpeed {
				get
				{
					return $"{GetFormattedSize_NoDecimalDigits(ReadSpeed)}/s";
				}
			}
			public string FormattedWriteSpeed {
				get
				{
					return $"{GetFormattedSize_NoDecimalDigits(WriteSpeed)}/s";
				}
			}
		}

		// inner class for computing moving
		public class MovingAverage
		{
			private readonly int MaxCount;
			private readonly Queue<uint> queue = new Queue<uint>();
			private long sum;

			public MovingAverage(int maxCount)
			{
				this.MaxCount = maxCount;
			}
			public void AddValue(uint value)
			{
				queue.Enqueue(value);
				sum += value;

				if (queue.Count > MaxCount)
				{
					sum -= queue.Dequeue();
				}
			}
			public float Average()
			{
				if (queue.Count == 0) return 0;
				return (float)(sum) / queue.Count;
			}
		}

		// readonly private fields
		//readonly DispatcherTimer timer = new DispatcherTimer();
		readonly Timer timer = null;
		readonly ManagementObjectSearcher processPerformanceData =
			new ManagementObjectSearcher("root\\CIMV2",
			"SELECT Name, WorkingSetPrivate, PercentProcessorTime FROM Win32_PerfFormattedData_PerfProc_Process");
		ManagementOperationObserver processPerformanceObserver = new ManagementOperationObserver();

		readonly ManagementClass systemPerformanceData = new ManagementClass("Win32_PerfFormattedData_PerfOS_System");
		readonly ManagementObjectSearcher memoryPerformanceData = 
			new ManagementObjectSearcher("root\\CIMV2",
				"Select TotalVisibleMemorySize, TotalVirtualMemorySize, FreePhysicalMemory, FreeVirtualMemory from Win32_OPeratingSystem");

		readonly Dictionary<string, (ulong, long)> processPerformanceDIct = new Dictionary<string, (ulong, long)>();
		readonly MovingAverage processorQueueLength1Min = new MovingAverage(60);
		readonly MovingAverage processorQueueLength5Min = new MovingAverage(300);
		readonly MovingAverage processorQueueLength15Min = new MovingAverage(900);

		// private fields for properties
		string _lastPrivilegedTime, _lastUserTime, _idleTime;
		int logicalProcessorCount;
		uint _processorQueueLength;
		IEnumerable<float> _lastCpuPerformance, _lastLoadAverage, _lastMemoryLoad;
		IEnumerable<System.Windows.Media.Color> _cpuPerformanceColors, _loadAverageColors, _memoryLoadColors;
		IEnumerable<ProcessPerformance> _performanceSortedByProcessorTime, _performanceSortedByWorkingSet;
		float _loadAverage1Min, _loadAverage5Min, _loadAverage15Min;
		int _upTimeMin, _upTimeHour, _upTimeDay;
		string _availableMemory, _memoryInUse, _availableCommittedMemory, _committedMemoryInUse, _availableVirtualMemory, _virtualMemoryInUse;
		float _lastVirtualMemoryLoad, _lastCommittedMemoryLoad;
		string _publicIP, _localIP, _downloadSpeed, _uploadSpeed;
		IEnumerable<DiskPerformance> _diskPerformance;

		// private fields
		PerformanceCounter privilegedTimeCounter, userTimeCounter;
		List<PerformanceCounter[]> diskCounters = new List<PerformanceCounter[]>();
		Boolean paused = true;
		int tick = 0;

		// public properties
		public string LastPrivilegedTime
		{
			get => _lastPrivilegedTime;
			set => SetField(ref _lastPrivilegedTime, value);
		}
		public string LaseUserTime
		{
			get => _lastUserTime;
			set => SetField(ref _lastUserTime, value);
		}
		public string IdleTime
		{
			get => _idleTime;
			set => SetField(ref _idleTime, value);
		}
		public IEnumerable<float> LastCpuPerformance
		{
			get => _lastCpuPerformance;
			set => SetField(ref _lastCpuPerformance, value);
		}
		public IEnumerable<ProcessPerformance> PerformanceSortedByProcessorTime
		{
			get => _performanceSortedByProcessorTime;
			set => SetField(ref _performanceSortedByProcessorTime, value);
		}
		public IEnumerable<ProcessPerformance> PerformanceSortedByWorkingSet
		{
			get => _performanceSortedByWorkingSet;
			set => SetField(ref _performanceSortedByWorkingSet, value);
		}
		public IEnumerable<System.Windows.Media.Color> CpuPerformanceColors
		{
			get => _cpuPerformanceColors;
			set => SetField(ref _cpuPerformanceColors, value);
		}
		public float LoadAverage1Min
		{
			get => _loadAverage1Min;
			set => SetField(ref _loadAverage1Min, value);
		}
		public float LoadAverage5Min
		{
			get => _loadAverage5Min;
			set => SetField(ref _loadAverage5Min, value);
		}
		public float LoadAverage15Min
		{
			get => _loadAverage15Min;
			set => SetField(ref _loadAverage15Min, value);
		}
		public uint ProcessorQueueLength
		{
			get => _processorQueueLength;
			set => SetField(ref _processorQueueLength, value);
		}
		public IEnumerable<float> LastLoadAverage
		{
			get => _lastLoadAverage;
			set => SetField(ref _lastLoadAverage, value);
		}
		public IEnumerable<System.Windows.Media.Color> LoadAverageColors
		{
			get => _loadAverageColors;
			set => SetField(ref _loadAverageColors, value);
		}
		public int UpTimeMin
		{
			get => _upTimeMin;
			set => SetField(ref _upTimeMin, value);
		}
		public int UpTimeHour
		{
			get => _upTimeHour;
			set => SetField(ref _upTimeHour, value);
		}
		public int UpTimeDay
		{
			get => _upTimeDay;
			set => SetField(ref _upTimeDay, value);
		}
		public string AvailableMemory
		{
			get => _availableMemory;
			set => SetField(ref _availableMemory, value);
		}
		public string MemoryInUse
		{
			get => _memoryInUse;
			set => SetField(ref _memoryInUse, value);
		}
		public string TotalCommittedMemory
		{
			get => _availableCommittedMemory;
			set => SetField(ref _availableCommittedMemory, value);
		}
		public string CommittedMemoryInUse
		{
			get => _committedMemoryInUse;
			set => SetField(ref _committedMemoryInUse, value);
		}
		public string TotalVirtualMemory
		{
			get => _availableVirtualMemory;
			set => SetField(ref _availableVirtualMemory, value);
		}
		public string VirtualMemoryInUse
		{
			get => _virtualMemoryInUse;
			set => SetField(ref _virtualMemoryInUse, value);
		}
		public IEnumerable<float> LastMemoryLoad
		{
			get => _lastMemoryLoad;
			set => SetField(ref _lastMemoryLoad, value);
		}
		public IEnumerable<System.Windows.Media.Color> MemoryLoadColors
		{
			get => _memoryLoadColors;
			set => SetField(ref _memoryLoadColors, value);
		}
		public float LastVirtualMemoryLoad
		{
			get => _lastVirtualMemoryLoad;
			set => SetField(ref _lastVirtualMemoryLoad, value);
		}
		public float LastCommittedMemoryLoad
		{
			get => _lastCommittedMemoryLoad;
			set => SetField(ref _lastCommittedMemoryLoad, value);
		}
		public string PublicIP
		{
			get => _publicIP;
			set => SetField(ref _publicIP, value);
		}
		public string LocalIP
		{
			get => _localIP;
			set => SetField(ref _localIP, value);
		}
		public string DownloadSpeed {
			get => _downloadSpeed;
			set => SetField(ref _downloadSpeed, value);
		}
		public string UploadSpeed {
			get => _uploadSpeed;
			set => SetField(ref _uploadSpeed, value);
		}
		public IEnumerable<DiskPerformance> AllDiskPerformance {
			get => _diskPerformance;
			set => SetField(ref _diskPerformance, value);
		}

		// constructor
		public PopupPerformanceMeter(PerformanceMeter mainMeter)
		{
			// initialize color values for chart controls
			CpuPerformanceColors = new System.Windows.Media.Color[] 
			{
				System.Windows.Media.Colors.DodgerBlue,
				System.Windows.Media.Colors.Red
			};
			LoadAverageColors = new System.Windows.Media.Color[]
			{
				System.Windows.Media.Colors.DodgerBlue,
				System.Windows.Media.Colors.Red,
				System.Windows.Media.Colors.DarkGray
			};
			MemoryLoadColors = new System.Windows.Media.Color[]
			{
				System.Windows.Media.Colors.DodgerBlue
			};

			// initialize all performance counters
			InitializePerformanceCounters();
			
			// register event for achieving network performance data
			mainMeter.PropertyChanged += MainMeter_PropertyChanged;

			// initialize callback for detecting disk change event
			ManagementEventWatcher watcher = new ManagementEventWatcher();
			WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 2 or EventType = 3");
			watcher.EventArrived += new EventArrivedEventHandler(VolumeChange_EventArrived);
			watcher.Query = query;
			watcher.Start();

			processPerformanceObserver.Completed += new CompletedEventHandler(this.ProcessPerformanceSearcher_Completed);
			processPerformanceObserver.ObjectReady += new ObjectReadyEventHandler(this.ProcessPerformanceSearcher_ObjectReady);

			timer = new Timer(new TimerCallback(Timer_Tick), null, 5000, 1000);
		}

		// public methods
		public void Pause()
		{
			paused = true;
		}
		public void Resume()
		{
			paused = false;
		}

		// event handler
		private void Timer_Tick(object state)
		{
			UpdateReadings();
		}
		private void MainMeter_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			PerformanceMeter m = sender as PerformanceMeter;

			switch (e.PropertyName)
			{
				case "LastNetworkReceive":
					DownloadSpeed = GetFormattedSize((ulong)m.LastNetworkReceive) + "/s";
					break;
				case "LastNetworkSend":
					UploadSpeed = GetFormattedSize((ulong)m.LastNetworkSend) + "/s";
					break;
			}
		}
		private void VolumeChange_EventArrived(object sender, EventArrivedEventArgs e)
		{
			InitializeDiskPerformanceCounters();
		}
		private void ProcessPerformanceSearcher_ObjectReady(object sender, ObjectReadyEventArgs obj)
		{
			//try
			{
				// get process name
				string name = obj.NewObject["Name"].ToString();
				name = Regex.Replace(name, "#[0-9]+", ""); // remove counter number for processes with same name

				// skip special processes
				if (name.Equals("_Total")) return;
				if (name.Equals("Idle")) return;
				if (name.Equals("Memory Compression")) return;

				// get memory and processor performance 
				ulong workingSet = Convert.ToUInt64(obj.NewObject["WorkingSetPrivate"]);
				long percentProcessorTime = Convert.ToInt64(obj.NewObject["PercentProcessorTime"]);

				// store data to dictionary
				if (processPerformanceDIct.ContainsKey(name))
				{
					workingSet += processPerformanceDIct[name].Item1;
					percentProcessorTime += processPerformanceDIct[name].Item2;
				}
				processPerformanceDIct[name] = (workingSet, percentProcessorTime);
			}
			//catch (Exception)
			{
			}
		}
		private void ProcessPerformanceSearcher_Completed(object sender, CompletedEventArgs obj)
		{
			// generate array of performance data of the first few processes with most processor usage
			PerformanceSortedByProcessorTime = processPerformanceDIct.OrderBy(p => p.Value.Item2).Reverse().Take(5)
				.Select(p => new ProcessPerformance(p.Key, p.Value.Item1, p.Value.Item2 / logicalProcessorCount)).ToArray();

			// generate array of performance data of the first few processes with most memory usage
			PerformanceSortedByWorkingSet = processPerformanceDIct.OrderBy(p => p.Value.Item1).Reverse().Take(5)
				.Select(p => new ProcessPerformance(p.Key, p.Value.Item1, p.Value.Item2 / logicalProcessorCount)).ToArray();

			processPerformanceDIct.Clear();
		}

		// private methods
		void InitializePerformanceCounters()
		{
			try
			{
				privilegedTimeCounter = new PerformanceCounter("Processor Information", "% Privileged Utility", "_Total");
			}
			catch (Exception)
			{
				privilegedTimeCounter = new PerformanceCounter("Processor", "% Privileged Time", "_Total");
			}

			try
			{
				userTimeCounter = new PerformanceCounter("Processor Information", "% User Utility", "_Total");
			}
			catch (Exception)
			{
				userTimeCounter = new PerformanceCounter("Processor", "% User Time", "_Total");
			}

			var processorCategory = new PerformanceCounterCategory("Processor Information");
			var logicalProcessorNames = processorCategory.GetInstanceNames()
				.Where(s => !s.Contains("Total"));
			logicalProcessorCount = logicalProcessorNames.Count();

			InitializeDiskPerformanceCounters();
		}
		void InitializeDiskPerformanceCounters()
		{
			// get performance counters for all physical disks
			List<PerformanceCounter[]> pcList = new List<PerformanceCounter[]>();
			PerformanceCounterCategory diskCategory = new PerformanceCounterCategory("PhysicalDisk");
			foreach (string driveName in diskCategory.GetInstanceNames())
			{
				// skip the _Total instance
				if (driveName == "_Total") continue;

				PerformanceCounter[] pcs = new PerformanceCounter[3]
				{
					new PerformanceCounter("PhysicalDisk", "% Idle Time", driveName),
					new PerformanceCounter("PhysicalDisk", "Disk Read Bytes/sec", driveName),
					new PerformanceCounter("PhysicalDisk", "Disk Write Bytes/sec", driveName)
				};
				pcList.Add(pcs);
			}

			// sort performance counters by name 
			pcList.Sort((pc1, pc2) => pc1[0].InstanceName.CompareTo(pc2[0].InstanceName));

			try
			{
				foreach (var pcs in diskCounters)
					foreach (var pc in pcs)
						pc?.Dispose();
				diskCounters.Clear();
			}
			catch (Exception) { }

			diskCounters = pcList;
		}
		void DisposePerformanceCounters()
		{
			// dispose all performance counters and reset fields
			privilegedTimeCounter?.Dispose();
			userTimeCounter?.Dispose();
			privilegedTimeCounter = null;
			userTimeCounter = null;

			foreach (var pcs in diskCounters)
				foreach (var pc in pcs)
					pc?.Dispose();
			diskCounters.Clear();
		}
		void UpdateReadings()
		{
			// increase time tick
			tick++;

			// update CPU, memory and system readings
			UpdateReadings_CPU();
			UpdateReadings_Memory();
			UpdateReadings_System();

			// update IPs per every 10 seconds
			if (tick % 10 == 1)
			{
				UpdateReadings_IPs();
			}

			// update processes information per every 2 or 10 seconds (which depends on popup window is shown or hidden)
			if ((paused && tick % 10 == 0) || (!paused && tick%2==0))
			{
				UpdateReadings_Processes();
			}

			// update disk performance per every 2 or 10 seconds (which depends on popup window is shown or hidden)
			if ((paused && tick % 10 == 5) || (!paused))
			{
				UpdateReadings_Disks();
			}
		}
		void UpdateReadings_CPU()
		{
			// get the next cpu performance readings from performance counters
			int t1 = (int)(privilegedTimeCounter.NextValue() + 0.5f);
			int t2 = (int)(userTimeCounter.NextValue() + 0.5f);
			int t3 = 100 - t1 - t2;
			if (t1 < 0) t1 = 0;
			if (t2 < 0) t2 = 0;
			if (t3 < 0) t3 = 0;
			LastPrivilegedTime = $"{t1}%";
			LaseUserTime = $"{t2}%";
			IdleTime = $"{t3}%";
			LastCpuPerformance = new float[] { t2, t1 };
		}
		void UpdateReadings_Memory()
		{
			// get memory performance readings
			try
			{
				foreach (ManagementObject obj in memoryPerformanceData.Get())
				{
					ulong freePhysical = Convert.ToUInt64(obj["FreePhysicalMemory"]);
					ulong totalPhysical = Convert.ToUInt64(obj["TotalVisibleMemorySize"]);
					AvailableMemory = GetFormattedSize(size: freePhysical, KB: true);
					MemoryInUse = GetFormattedSize(size: totalPhysical - freePhysical, KB: true);
					float inUsePercentage = 1.0f - (float)freePhysical / (float)totalPhysical;
					LastMemoryLoad = new float[] { inUsePercentage };

					ulong freeCommitted = Convert.ToUInt64(obj["FreeVirtualMemory"]);
					ulong totalComitted = Convert.ToUInt64(obj["TotalVirtualMemorySize"]);
					TotalCommittedMemory = GetFormattedSize(size: totalComitted, KB: true);
					CommittedMemoryInUse = GetFormattedSize(size: totalComitted - freeCommitted, KB: true);
					LastCommittedMemoryLoad = 1.0f - (float)freeCommitted / (float)totalComitted;

					ulong freeVirtual = freeCommitted - freePhysical;
					ulong totalVirtual = totalComitted - totalPhysical;
					TotalVirtualMemory = GetFormattedSize(size: totalVirtual, KB: true);
					VirtualMemoryInUse = GetFormattedSize(size: totalVirtual - freeVirtual, KB: true);
					LastVirtualMemoryLoad = 1.0f - (float)freeVirtual / (float)totalVirtual;
					break;
				}
			}
			catch (Exception) { }
		}
		void UpdateReadings_System()
		{
			// get the next processor queue length and system up timne readings from performance counters
			try
			{
				foreach (ManagementObject obj in systemPerformanceData.GetInstances())
				{
					ProcessorQueueLength = Convert.ToUInt32(obj["ProcessorQueueLength"]);
					ulong ut = Convert.ToUInt64(obj["SystemUpTime"]);
					UpTimeMin = (int)((ut / 60) % 60);
					UpTimeHour = (int)((ut / 3600) % 24);
					UpTimeDay = (int)(ut / 86400);
					break;
				}

				// compute moving averages for 1, 5 and 15 mins
				processorQueueLength1Min.AddValue(ProcessorQueueLength);
				processorQueueLength5Min.AddValue(ProcessorQueueLength);
				processorQueueLength15Min.AddValue(ProcessorQueueLength);
				LoadAverage1Min = processorQueueLength1Min.Average();
				LoadAverage5Min = processorQueueLength5Min.Average();
				LoadAverage15Min = processorQueueLength15Min.Average();
				LastLoadAverage = new float[] { LoadAverage1Min, LoadAverage5Min, LoadAverage15Min };
			}
			catch (Exception) { }
		}
		void UpdateReadings_IPs()
		{
			// try to get local ips
			try
			{
				string response = new WebClient().DownloadString("http://icanhazip.com");
				PublicIP = response.Trim();
			}
			catch (Exception) 
			{
				PublicIP = "unknown";
			}

			// try to obtain local IP
			bool found = false;
			var host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (var ip in host.AddressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork)
				{
					LocalIP = ip.ToString();
					found = true;
					break;
				}
			}
			if (!found)
			{
				LocalIP = "unknown";
			}
		}
		void UpdateReadings_Processes()
		{
			// clear process performance cache dictionary
			processPerformanceDIct.Clear();

			// retrieve and iterate process performance data asynchronously 
			try
			{
				processPerformanceData.Get(processPerformanceObserver);
			}
			catch (Exception) { }
		}
		void UpdateReadings_Disks()
		{
			try
			{
				AllDiskPerformance = diskCounters.Select(pc =>
				new DiskPerformance(
					pc[0].InstanceName,
					(int)Math.Round(100f - pc[0].NextValue()),
					(ulong)pc[1].NextValue(),
					(ulong)pc[2].NextValue()
					)
				);
			}
			catch (Exception) { }
		}

		// public static method
		public static string GetFormattedSize(ulong size, bool KB=false)
		{
			// convret size value to string with unit
			float s = size;
			if (!KB) s /= 1024;
			if (s < 1024) return string.Format("{0:N2} KB", s);
			s /= 1024;
			if (s < 1024) return string.Format("{0:N2} MB", s);
			s /= 1024;
			return string.Format("{0:N2} GB", s);
		}
		public static string GetFormattedSize_NoDecimalDigits(ulong size, bool KB = false)
		{
			// convret size value to string with unit
			float s = size;
			if (!KB) s /= 1024;
			if (s < 1024) return string.Format("{0:N1} KB", s);
			s /= 1024;
			if (s < 1024) return string.Format("{0:N1} MB", s);
			s /= 1024;
			return string.Format("{0:N1} GB", s);
		}

		// IDisposable implementation
		public void Dispose()
		{
			DisposePerformanceCounters();
		}

		// INotifyPropertyChanged implementation
		public event PropertyChangedEventHandler PropertyChanged;
		void SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(field, value)) return;
			field = value;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
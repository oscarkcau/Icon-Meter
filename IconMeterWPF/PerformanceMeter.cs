using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Management;

namespace IconMeterWPF
{
	sealed class PerformanceMeter : IDisposable, INotifyPropertyChanged
	{
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		extern static bool DestroyIcon(IntPtr handle);

        // private fields
        Properties.Settings settings;
		PerformanceCounter cpuCounter, memoryCounter, diskCounter;
		float lastCpuUsage = 0;
		float lastMemoryUsage = 0;
		float lastDiskUsage = 0;
		float _lastNetworkReceive = 0;
		float _lastNetworkSend = 0;
		float[] logicalProcessorUsage = null;
		string _mainTooltip, _logicalProcessorsTooltip;
		Icon _defaultTrayIcon, _mainTrayIcon, _logicalProcessorsTrayIcon;
		IEnumerable<float> _lastNetworkSpeed;

		// private readonly fields
		readonly DispatcherTimer timer = new DispatcherTimer();
		readonly float totalMemorySize;
		readonly List<PerformanceCounter> networkReceiveCounters = new List<PerformanceCounter>();
		readonly List<PerformanceCounter> networkSendCounters = new List<PerformanceCounter>();
		readonly List<PerformanceCounter> logicalProcessorsCounter = new List<PerformanceCounter>();
		readonly Queue<float> previousNetwordReceive = new Queue<float>();
		readonly Queue<float> previousNetwordSend = new Queue<float>();
		const string upArrow = "\u25b2";
		const string downArrow = "\u25bc";

		// public propertie
		public Icon DefaultTrayIcon {
			get => _defaultTrayIcon;
			set => SetField(ref _defaultTrayIcon, value);
		}
		public Icon MainTrayIcon {
			get => _mainTrayIcon;
			set => SetField(ref _mainTrayIcon, value);
		}
		public string MainTooltip {
			get => _mainTooltip;
			set => SetField(ref _mainTooltip, value);
		}
		public Icon LogicalProcessorsTrayIcon {
			get => _logicalProcessorsTrayIcon;
			set => SetField(ref _logicalProcessorsTrayIcon, value);
		}
		public string LogicalProcessorsTooltip {
			get => _logicalProcessorsTooltip;
			set => SetField(ref _logicalProcessorsTooltip, value);
		}
		public float LastNetworkReceive {
			get => _lastNetworkReceive;
			set => SetField(ref _lastNetworkReceive, value);
		}
		public float LastNetworkSend {
			get => _lastNetworkSend;
			set => SetField(ref _lastNetworkSend, value);
		}
		public IEnumerable<float> LastNetworkSpeed {
			get => _lastNetworkSpeed;
			set => SetField(ref _lastNetworkSpeed, value);
		}

		// constructor
		public PerformanceMeter()
		{
			totalMemorySize = (float)GetTotalMemorySize();

			// initialize queues and timer
			previousNetwordReceive.Enqueue(0);
			previousNetwordSend.Enqueue(0);
			timer.Tick += new EventHandler(Timer_Tick);
			timer.Interval = new TimeSpan(0, 0, 1);

			// initialize all performance counters
			ResetPerformanceMeter();
		}

		// public method
		public void ResetPerformanceMeter()
		{
			// dispose all current performance counters
			DisposePerformanceCounters();

            // update the setting field
            //this.settings = settings;
            this.settings = Properties.Settings.Default;

            // create all performance counters with new settings
            InitializePerformanceCounters();
		}
		public void Pause()
		{
			this.timer.Stop();
		}
		public void Resume()
		{
			this.timer.Start();
		}

		// event handler
		private void Timer_Tick(object sender, EventArgs e)
		{
			// first get new readings from performance counters
			UpdateReadings();

			// update icon image and tooltip of main tray icon
			if (MainTrayIcon != null && MainTrayIcon != DefaultTrayIcon)
				DestroyIcon(MainTrayIcon.Handle);
			MainTrayIcon = BuildMainNotifyIcon();
			MainTooltip = BuildMainTooltip();

			// update icon image and tooltip of logical processor tray icon if it is in used
			if (settings.ShowLogicalProcessorsUsage)
			{
				if (LogicalProcessorsTrayIcon != null && LogicalProcessorsTrayIcon != DefaultTrayIcon)
					DestroyIcon(LogicalProcessorsTrayIcon.Handle);
				LogicalProcessorsTrayIcon = BuildLogicalProcessorIcon();
				LogicalProcessorsTooltip = BuildLogicalProcessorTooltip();
			}
		}

		// private methods
		void InitializePerformanceCounters()
		{
			// initialize necessary performance counters depending on current setting

			// Processor PC
			// in virtual machine the "Processor Information" category may not found,
			// therefore use "Processor" category if exception occurs
			try
			{
				cpuCounter = new PerformanceCounter("Processor Information", "% Processor Utility", "_Total");
			}
			catch (Exception)
			{
				cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
			}

			// memory PC
			memoryCounter = new PerformanceCounter("Memory", "Available MBytes");
				//"Memory", "% Committed Bytes In Use");

			diskCounter = new PerformanceCounter("PhysicalDisk", "% Idle Time", "_Total");

			// network PC
			PerformanceCounterCategory networkCounterCategory
				= new PerformanceCounterCategory("Network Interface");
			foreach (string name in networkCounterCategory.GetInstanceNames())
			{
				networkReceiveCounters.Add(new PerformanceCounter("Network Interface", "Bytes Received/sec", name));
				networkSendCounters.Add(new PerformanceCounter("Network Interface", "Bytes Sent/sec", name));
			}

			// logical processor PCs
			var processorCategory = new PerformanceCounterCategory("Processor Information");
			var logicalProcessorNames = processorCategory.GetInstanceNames()
				.Where(s => !s.Contains("Total"))
				.OrderBy(s => s);
			int nLogicalProcessors = logicalProcessorNames.Count();
			foreach (string name in logicalProcessorNames)
			{
				try
				{
					logicalProcessorsCounter.Add(new PerformanceCounter("Processor Information", "% Processor Utility", name));
				}
				catch
				{
					logicalProcessorsCounter.Add(new PerformanceCounter("Processor Information", "% Processor Time", name));
				}
			}
			this.logicalProcessorUsage = new float[logicalProcessorsCounter.Count];

			timer.Start();
		}
		void DisposePerformanceCounters()
		{
			// dispose all performance counters and clean up

			// first stop timer
			timer.Stop();

			// dispose all performance counters and reset fields
			cpuCounter?.Dispose();
			memoryCounter?.Dispose();
			diskCounter?.Dispose();
			cpuCounter = null;
			memoryCounter = null;
			diskCounter = null;

			foreach (var pc in networkReceiveCounters) pc?.Dispose();
			foreach (var pc in networkSendCounters) pc?.Dispose();
			foreach (var pc in logicalProcessorsCounter) pc?.Dispose();
			networkReceiveCounters.Clear();
			networkSendCounters.Clear();
			logicalProcessorsCounter.Clear();
		}
		void UpdateReadings()
		{
			// get the next readings from performance counters

			// CPU, memory and disk
			if (settings.ShowCpuUsage) lastCpuUsage = cpuCounter.NextValue();
			if (settings.ShowMemoryUsage) lastMemoryUsage = 100 - (memoryCounter.NextValue() / totalMemorySize) * 100;
			if (settings.ShowDiskUsage) lastDiskUsage = 100 - diskCounter.NextValue();

			// network 
			//if (settings.ShowNetworkUsage)
			{
				float lastNetworkReceive = 0;
				foreach (var c in networkReceiveCounters)
					lastNetworkReceive += c.NextValue();

				previousNetwordReceive.Enqueue(lastNetworkReceive);
				if (previousNetwordReceive.Count > 60)
					previousNetwordReceive.Dequeue();

				float  lastNetworkSend = 0;
				foreach (var c in networkSendCounters)
					lastNetworkSend += c.NextValue();

				previousNetwordSend.Enqueue(lastNetworkSend);
				if (previousNetwordSend.Count > 60)
					previousNetwordSend.Dequeue();

				LastNetworkReceive = lastNetworkReceive;
				LastNetworkSend = lastNetworkSend;
				LastNetworkSpeed = new float[] { lastNetworkReceive, lastNetworkSend };
			}

			// logical processors
			if (settings.ShowLogicalProcessorsUsage)
			{
				for (int i = 0; i < logicalProcessorsCounter.Count; i++)
					logicalProcessorUsage[i] = logicalProcessorsCounter[i].NextValue();
			}
		}
		Icon BuildMainNotifyIcon()
		{
			// prepare list of display readings and brushes
			List<(float, Brush)> list = new List<(float, Brush)>();

			if (settings.ShowCpuUsage)
				list.Add((lastCpuUsage, new SolidBrush(settings.CpuColor)));

			if (settings.ShowMemoryUsage)
				list.Add((lastMemoryUsage, new SolidBrush(settings.MemoryColor)));

			if (settings.ShowDiskUsage)
				list.Add((lastDiskUsage, new SolidBrush(settings.DiskColor)));

			if (settings.ShowNetworkUsage)
			{
				// compute the moving maximum network flow value
				float maxNetworkReceive = previousNetwordReceive.Max();
				float maxNetworkSend = previousNetwordSend.Max();
				float maxNetword = Math.Max(maxNetworkReceive, maxNetworkSend);

				// compute relative flow
				float send = (_lastNetworkSend / maxNetword) * 100;
				float receive = (_lastNetworkReceive / maxNetword) * 100;

				list.Add((send, new SolidBrush(settings.NetworkReceiveColor)));
				list.Add((receive, new SolidBrush(settings.NetworkSendColor)));
			}

			// build the new icon
			Icon icon = IconBuilder.BuildIcon(list, useVerticalBar: settings.UseVerticalBars);
		
			// release resource used by brushes
			foreach (var (_, brush) in list) brush.Dispose();

			// return the icon
			return icon;
		}
		Icon BuildLogicalProcessorIcon()
		{
			// create brush for drawing
			Color color = settings.LogicalProcessorColor;
			Brush brush = new SolidBrush(color);

			// build the new icon from logical processor readings
			Icon icon = IconBuilder.BuildIcon(
				logicalProcessorUsage.Select(x => (x, brush)),
				useVerticalBar:settings.UseVerticalBars
				); ;

			// release resource used by brushes
			brush.Dispose();

			// return the icon
			return icon;
		}
		Icon BuildDiskActiveTimeIcon()
		{
			// create brush for drawing
			Color color = Color.Green; // settings.LogicalProcessorColor;
			Brush brush = new SolidBrush(color);

			// build the new icon from logical processor readings
			Icon icon = IconBuilder.BuildIcon(
				logicalProcessorUsage.Select(x => (x, brush)),
				useVerticalBar: settings.UseVerticalBars
				);

			// release resource used by brushes
			brush.Dispose();

			// return the icon
			return icon;
		}

		string BuildMainTooltip()
		{
			// build notify icon's tooltip text

			// detemine unit for network flow readings
			float nr = _lastNetworkReceive / 1024;
			float ns = _lastNetworkSend / 1024;
			string unit = "KBps";
			if (nr > 1024 || ns > 1024)
			{
				nr = nr / 1024;
				ns = ns / 1024;
				unit = "MBps";
			}

			// build the text
			StringBuilder sb = new StringBuilder();

			if (settings.ShowCpuUsage) sb.AppendLine($"{Properties.Resources.CPU} {Math.Round(lastCpuUsage)}%");
			if (settings.ShowMemoryUsage) sb.AppendLine($"{Properties.Resources.Memory} {Math.Round(lastMemoryUsage)}%");
			if (settings.ShowDiskUsage) sb.AppendLine($"{Properties.Resources.Disk} {Math.Round(lastDiskUsage)}%");
			if (settings.ShowNetworkUsage)
			{
				sb.Append($"{Properties.Resources.Network} {downArrow}:" + nr.ToString("0.0"));
				sb.Append($" {upArrow}:" + ns.ToString("0.0") + " " + unit);
			}

			// make sure the tooltip text has at most 128 characters
			if (sb.Length >= 128) sb.Remove(127, sb.Length - 127);

			// return the text value
			return sb.ToString().TrimEnd();
		}
		string BuildLogicalProcessorTooltip()
		{
			// build notify icon's tooltip text for logical processors

			// build the text
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < logicalProcessorUsage.Count(); i++)
			{
				sb.AppendLine($"{Properties.Resources.CPU} {i + 1}: {Math.Round(logicalProcessorUsage[i])}%");
			}

			// make sure the tooltip text has at most 128 characters
			if (sb.Length >= 128) sb.Remove(127, sb.Length - 127);

			// return the text value
			return sb.ToString().TrimEnd();
		}
		double GetTotalMemorySize()
		{
			ObjectQuery wql = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
			ManagementObjectSearcher searcher = new ManagementObjectSearcher(wql);
			ManagementObjectCollection results = searcher.Get();

			foreach (ManagementObject result in results)
			{
				double res = Convert.ToDouble(result["TotalVisibleMemorySize"]);
				return res / 1024;
			}
			return 0;
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

	public static class ConvertExtesions
	{
		public static System.Drawing.Color ToGDIColor(this System.Windows.Media.Color c)
		{
			return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
		}

	}


}

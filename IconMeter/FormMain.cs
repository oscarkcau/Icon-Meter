using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Win32;
using System.Reflection;

namespace IconMeter
{
	public partial class FormMain : Form
	{
		//GetSystemMetrics(SM_CXSMICON)

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		extern static bool DestroyIcon(IntPtr handle);

		// internal setting class
		[Serializable]
		public class Settings
		{
			[XmlElement(Type = typeof(XmlColor))] public Color CpuColor { get; set; }
			[XmlElement(Type = typeof(XmlColor))] public Color MemoryColor { get; set; }
			[XmlElement(Type = typeof(XmlColor))] public Color DiskColor { get; set; }
			[XmlElement(Type = typeof(XmlColor))] public Color NetworkReceiveColor { get; set; }
			[XmlElement(Type = typeof(XmlColor))] public Color NetworkSendColor { get; set; }
			[XmlElement(Type = typeof(XmlColor))] public Color LogicalProcessorColor { get; set; }
			public bool ShowCpuUsage { get; set; }
			public bool ShowMemoryUsage { get; set; }
			public bool ShowDiskUsage { get; set; }
			public bool ShowNetworkUsage { get; set; }
			public bool ShowLogicalProcessorsUsage { get; set; }
			public bool UseVerticalBars { get; set; }
			public bool RunAtStartup { get; set; }

			public Settings()
			{
				CpuColor = Color.OrangeRed;
				MemoryColor = Color.DodgerBlue;
				DiskColor = Color.LimeGreen;
				NetworkReceiveColor = Color.Yellow;
				NetworkSendColor = Color.Goldenrod;
				LogicalProcessorColor = Color.OrangeRed;
				ShowCpuUsage = true;
				ShowMemoryUsage = true;
				ShowDiskUsage = true;
				ShowNetworkUsage = true;
				ShowLogicalProcessorsUsage = false;
				UseVerticalBars = true;
				RunAtStartup = false;
			}
		}

		// private fields
		Settings settings = new Settings();
		readonly string settingsFilename = AppDomain.CurrentDomain.BaseDirectory + "settings.xml";
		readonly string upArrow = "\u25b2";
		readonly string downArrow = "\u25bc";
		PerformanceCounter cpuCounter, memoryCounter, diskCounter;
		float lastCpuUsage = 0;
		float lastMemoryUsage = 0;
		float lastDiskUsage = 0;
		float lastNetworkReceive = 0;
		float lastNetworkSend = 0;
		float[] logicalProcessorUsage = null;
		bool isClosing = false;
		bool allowShowForm = false;

		// private readonly fields
		readonly List<PerformanceCounter> networkReceiveCounters = new List<PerformanceCounter>();
		readonly List<PerformanceCounter> networkSendCounters = new List<PerformanceCounter>();
		readonly List<PerformanceCounter> logicalProcessorsCounter = new List<PerformanceCounter>();
		readonly Queue<float> previousNetwordReceive = new Queue<float>();
		readonly Queue<float> previousNetwordSend = new Queue<float>();

		// Constructor and event handlers
		public FormMain()
		{
			InitializeComponent();

			// load saved settings from file
			LoadSettings();

			// update auto start system setting (if necessary)
			UpdateAutoStartSetting();

			// initialize necessary performance counters
			InitializePerformanceCounters();

			// start collect readings
			timerMain.Enabled = true;
		}
		private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			// hide form when user close the form through UI
			if (e.CloseReason == CloseReason.UserClosing && isClosing == false)
			{
				e.Cancel = true;
				this.Hide();
			}
		}
		private void FormMain_Shown(object sender, EventArgs e)
		{
			// update form controls with the current settings
			UpdateControlsFromSettings();
		}
		private void buttonOK_Click(object sender, EventArgs e)
		{
			// make sure at least one meter in main icon is selected
			if (!(checkBoxCpu.Checked || checkBoxMemory.Checked || checkBoxDisk.Checked || checkBoxNetwork.Checked))
			{
				// otherwise shown error message
				MessageBox.Show(this,
					"At least one of the following meters should be selected: \nCPU / Memory / Disk / Network",
					"Icom Meter",
					MessageBoxButtons.OK,
					MessageBoxIcon.Information
					);
				// and does not hide the setting dialog
				return;
			}

			// update settings from controls' properties
			UpdateSettingsFromControls();

			// save settings to file
			SaveSettings();

			// update auto start system setting (if necessary)
			UpdateAutoStartSetting();

			// hide the form
			this.Hide();

			// dispose and reinitialize all performance counters
			ResetPerformanceCounters();
		}
		private void buttonCancel_Click(object sender, EventArgs e)
		{
			// hide the form
			this.Hide();
		}
		private void buttonColor_Click(object sender, EventArgs e)
		{
			// get the background color of the sender button
			Button b = sender as Button;
			this.colorDialogMain.Color = b.BackColor;

			// show color dialog to allow user to specify a new color
			DialogResult ret = colorDialogMain.ShowDialog(this);

			// update button background color if a new color is selected
			if (ret == DialogResult.OK)
			{
				b.BackColor = colorDialogMain.Color;
			}
		}
		private void notifyIconMain_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			// start Task Manager
			if (e.Button == MouseButtons.Left)
			{
				Process p = new Process();
				p.StartInfo.FileName = "taskmgr";
				p.Start();
			}
		}
		private void closeToolStripMenuItem_Click(object sender, EventArgs e)
		{ 
			// close this the application (thus close this main form)
			isClosing = true;
			Application.Exit();
		}
		private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// show this form
			allowShowForm = true; // this flag is used for override function SetVisibleCore()
			this.Visible = true;

			// update controls from current settings
			UpdateControlsFromSettings();
		}
		private void resetIconMeterToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// dispose and reinitialize all performance counters
			ResetPerformanceCounters();
		}
		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// show AboutBox dialog
			AboutBox a = new AboutBox();
			a.ShowDialog();
		}
		private void timerMain_Tick(object sender, EventArgs e)
		{
			// try to update readings from performance counters
			try
			{
				UpdateReadings();
			}
			catch (Exception) //when (ex is InvalidOperationException)
			{
				// dispose and reinitialize all performance counters if there is exception
				ResetPerformanceCounters();
			}

			// update main notify icons and tooltip text
			UpdateIcon();
			UpdateNotifyIconTooltipText();

			// update logical processor notify icon if it is in used
			if (settings.ShowLogicalProcessorsUsage)
			{
				UpdateLogicalProcessorIcon();
				this.notifyIconLogicalProcessor.Visible = true;
			}
			else // otherwise hide it
			{
				this.notifyIconLogicalProcessor.Visible = false;
			}
		}

		// override function for disable show window on startup
		protected override void SetVisibleCore(bool value)
		{
			base.SetVisibleCore(allowShowForm ? value : false);
		}

		// main procedures
		private void SaveIconToFile(string filename)
		{
			// save current notify icon to file
			// input: filename

			using (FileStream s = new FileStream(filename, FileMode.Create))
			{
				this.notifyIconMain.Icon.Save(s);
			}
		}
		private void SaveSettings()
		{
			// serialize current setting to xml file

			try
			{
				XmlSerializer serializer = new XmlSerializer(typeof(Settings));

				using (StreamWriter writer = new StreamWriter(this.settingsFilename))
				{
					serializer.Serialize(writer, this.settings);
					writer.Close();
				}
			}
			catch (Exception e) { Debug.Write(e.Message); }
		}
		private void LoadSettings()
		{
			// deserialize current setting from xml file

			try
			{
				if (File.Exists(this.settingsFilename) == false) return;

				XmlSerializer serializer = new XmlSerializer(typeof(Settings));
				using (StreamReader reader = new StreamReader(this.settingsFilename))
				{
					this.settings = (Settings)serializer.Deserialize(reader);
					reader.Close();
				}
			}
			catch (Exception e) { Debug.Write(e.Message); }
		}
		private void InitializePerformanceCounters()
		{
			// initialize necessary performance counters depending on current setting

			if (settings.ShowCpuUsage)
			{
				// in virtual machine the "Processor Information" category may not found,
				// therefore use "Processor" category if exception occurs
				try
				{
					cpuCounter = new PerformanceCounter("Processor Informance", "% Processor Utility", "_Total");
				}
				catch (Exception)
				{
					cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
				}
			}

			if (settings.ShowMemoryUsage)
			{
				memoryCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
			}

			if (settings.ShowNetworkUsage)
			{
				diskCounter = new PerformanceCounter("PhysicalDisk", "% Idle Time", "_Total");
			}

			if (settings.ShowNetworkUsage)
			{
				PerformanceCounterCategory networkCounterCategory
					= new PerformanceCounterCategory("Network Interface");
				foreach (string name in networkCounterCategory.GetInstanceNames())
				{
					networkReceiveCounters.Add(new PerformanceCounter("Network Interface", "Bytes Received/sec", name));
					networkSendCounters.Add(new PerformanceCounter("Network Interface", "Bytes Sent/sec", name));
				}
			}

			if (settings.ShowLogicalProcessorsUsage)
			{
				var processorCategory = new PerformanceCounterCategory("Processor Information");
				var logicalProcessorNames = processorCategory.GetInstanceNames()
					.Where(s => !s.Contains("Total"))
					.OrderBy(s => s);
				int nLogicalProcessors = logicalProcessorNames.Count();
				foreach (string name in logicalProcessorNames)
				{
					logicalProcessorsCounter.Add(new PerformanceCounter("Processor Information", "% Processor Utility", name));
				}
				this.logicalProcessorUsage = new float[nLogicalProcessors];
				this.notifyIconLogicalProcessor.Text = nLogicalProcessors + " Logical Processor(s)";
			}
		}		 
		private void ResetPerformanceCounters()
		{
			// dispose and reinitialize all performance counters

			timerMain.Stop();					// stop timer to avoid getting new readings
			DisposePerformanceCounters();		// dispose all PC
			System.Threading.Thread.Sleep(2000);// pause for a short while (2 sec)
			InitializePerformanceCounters();	// initialize PC
			timerMain.Start();					// start timer again for getting new readings
		}
		private void DisposePerformanceCounters()
		{
			// dispose all performance counters

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
		private void UpdateControlsFromSettings()
		{
			// update control properties with the current settings

			this.buttonCpuColor.BackColor = settings.CpuColor;
			this.buttonMemoryColor.BackColor = settings.MemoryColor;
			this.buttonDiskColor.BackColor = settings.DiskColor;
			this.buttonReceiveColor.BackColor = settings.NetworkReceiveColor;
			this.buttonSendColor.BackColor = settings.NetworkSendColor;
			this.buttonLogicalProcessorsColor.BackColor = settings.LogicalProcessorColor;

			this.checkBoxCpu.Checked = settings.ShowCpuUsage;
			this.checkBoxMemory.Checked = settings.ShowMemoryUsage;
			this.checkBoxDisk.Checked = settings.ShowDiskUsage;
			this.checkBoxNetwork.Checked = settings.ShowNetworkUsage;
			this.checkBoxLogicalProcessors.Checked = settings.ShowLogicalProcessorsUsage;
			this.checkBoxUseVerticalBar.Checked = settings.UseVerticalBars;
			this.checkBoxRunAtStartup.Checked = settings.RunAtStartup;
		}
		private void UpdateSettingsFromControls()
		{
			// update current settings from control properties

			settings.CpuColor = buttonCpuColor.BackColor;
			settings.MemoryColor = buttonMemoryColor.BackColor;
			settings.DiskColor = buttonDiskColor.BackColor;
			settings.NetworkReceiveColor = buttonReceiveColor.BackColor;
			settings.LogicalProcessorColor = buttonLogicalProcessorsColor.BackColor;
			settings.NetworkSendColor = buttonSendColor.BackColor;

			settings.ShowCpuUsage = checkBoxCpu.Checked;
			settings.ShowMemoryUsage = checkBoxMemory.Checked;
			settings.ShowDiskUsage = checkBoxDisk.Checked;
			settings.ShowNetworkUsage = checkBoxNetwork.Checked;
			settings.ShowLogicalProcessorsUsage = checkBoxLogicalProcessors.Checked;
			settings.UseVerticalBars = checkBoxUseVerticalBar.Checked;
			settings.RunAtStartup = checkBoxRunAtStartup.Checked;
		}
		private void UpdateAutoStartSetting()
		{
			// The path to the key where Windows looks for startup applications
			RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

			if (settings.RunAtStartup)
			{
				// Add the value in the registry so that the application runs at startup
				rkApp.SetValue("IconMeter", Application.ExecutablePath);
			}
			else
			{
				// Remove the value from the registry so that the application doesn't start
				rkApp.DeleteValue("IconMeter", false);
			}
		}
		private void UpdateReadings()
		{
			// get the next readings from performance counters

			if (settings.ShowCpuUsage) lastCpuUsage = cpuCounter.NextValue();
			if (settings.ShowMemoryUsage) lastMemoryUsage = memoryCounter.NextValue();
			if (settings.ShowDiskUsage) lastDiskUsage = 100 - diskCounter.NextValue();

			if (settings.ShowNetworkUsage)
			{
				lastNetworkReceive = 0;
				foreach (var c in networkReceiveCounters)
					lastNetworkReceive += c.NextValue();

				previousNetwordReceive.Enqueue(lastNetworkReceive);
				if (previousNetwordReceive.Count > 60)
					previousNetwordReceive.Dequeue();

				lastNetworkSend = 0;
				foreach (var c in networkSendCounters)
					lastNetworkSend += c.NextValue();

				previousNetwordSend.Enqueue(lastNetworkSend);
				if (previousNetwordSend.Count > 60)
					previousNetwordSend.Dequeue();
			}

			if (settings.ShowLogicalProcessorsUsage)
			{
				for (int i = 0; i < logicalProcessorsCounter.Count; i++)
					logicalProcessorUsage[i] = logicalProcessorsCounter[i].NextValue();
			}
		}
		private void UpdateIcon()
		{
			// Update main notify icon

			// create bitmap and corresponding graphics object
			Bitmap bitmapText = new Bitmap(16, 16);
			Graphics g = System.Drawing.Graphics.FromImage(bitmapText);

			// build some brushes
			Brush brush1 = new SolidBrush(settings.CpuColor);
			Brush brush2 = new SolidBrush(settings.MemoryColor);
			Brush brush3 = new SolidBrush(settings.DiskColor);
			Brush brush4 = new SolidBrush(settings.NetworkReceiveColor);
			Brush brush5 = new SolidBrush(settings.NetworkSendColor);
			
			// compute bar height
			int nReading = 0;
			if (settings.ShowCpuUsage) nReading++;
			if (settings.ShowMemoryUsage) nReading++;
			if (settings.ShowDiskUsage) nReading++;
			if (settings.ShowNetworkUsage) nReading += 2;
			int barHeight = 16 / nReading;

			// clear background and draw bounding box
			g.Clear(Color.Transparent);
			Pen pen = new Pen(Color.DarkGray);
			g.DrawLine(pen, 0, 0, 0, 15);
			g.DrawLine(pen, 0, 15, 15, 15);
			g.DrawLine(pen, 15, 15, 15, 0);
			g.DrawLine(pen, 15, 0, 0, 0);

			// render all bars
			int leftOrTop = 0;
			if (settings.ShowCpuUsage)
			{
				float h = (lastCpuUsage * 16 / 100.0f);
				if (settings.UseVerticalBars)
					g.FillRectangle(brush1, leftOrTop, 16 - h, barHeight, h);
				else
					g.FillRectangle(brush1, 0, leftOrTop, h, barHeight);
				leftOrTop += barHeight;
			}

			if (settings.ShowMemoryUsage)
			{
				float h = (lastMemoryUsage * 16 / 100.0f);
				if (settings.UseVerticalBars)
					g.FillRectangle(brush2, leftOrTop, 16 - h, barHeight, h);
				else
					g.FillRectangle(brush2, 0, leftOrTop, h, barHeight);
				leftOrTop += barHeight;
			}

			if (settings.ShowDiskUsage)
			{
				float h = (lastDiskUsage * 16 / 100.0f);
				if (settings.UseVerticalBars)
					g.FillRectangle(brush3, leftOrTop, 16 - h, barHeight, h);
				else
					g.FillRectangle(brush3, 0, leftOrTop, h, barHeight);
				leftOrTop += barHeight;
			}

			if (settings.ShowNetworkUsage)
			{
				// compute the moving maximum network flow value
				float maxNetworkReceive = previousNetwordReceive.Max();
				float maxNetworkSend = previousNetwordSend.Max();
				float maxNetword = Math.Max(maxNetworkReceive, maxNetworkSend);

				// compute relative flow
				float send = (lastNetworkSend * 16 / maxNetword);
				float receive = (lastNetworkReceive * 16 / maxNetword);

				// render the bars
				if (settings.UseVerticalBars)
				{
					g.FillRectangle(brush4, leftOrTop, 16 - receive, barHeight, receive);
					g.FillRectangle(brush5, leftOrTop + barHeight, 16 - send, barHeight, send);
				}
				else
				{
					g.FillRectangle(brush4, 0, leftOrTop, receive, barHeight);
					g.FillRectangle(brush5, 0, leftOrTop + barHeight, send, barHeight);
				}
			}
			
			// create icon from bitmap
			IntPtr hIcon = (bitmapText.GetHicon());
			Icon newIcon = Icon.FromHandle(hIcon);
			Icon oldIcon = notifyIconMain.Icon;
			notifyIconMain.Icon = newIcon;
			if (oldIcon != null) DestroyIcon(oldIcon.Handle); // remember to delete the old icon

			// remember to dispose brushes
			brush1.Dispose();
			brush2.Dispose();
			brush3.Dispose();
			brush4.Dispose();
			brush5.Dispose();
		}
		private void UpdateLogicalProcessorIcon()
		{
			// Update logical processor notify icon

			// create bitmap and corresponding graphics object
			Bitmap bitmapText = new Bitmap(16, 16);
			Graphics g = System.Drawing.Graphics.FromImage(bitmapText);

			// build some brushes and pens
			Brush barBrush = new SolidBrush(settings.LogicalProcessorColor);
			Pen shadowPen = new Pen(Color.FromArgb(128, Color.Black));

			// compute bar height
			int nReadings = this.logicalProcessorsCounter.Count;
			float barHeight = 16 / nReadings;

			// clear background and draw bounding box
			g.Clear(Color.Transparent);
			Pen pen = new Pen(Color.DarkGray);
			g.DrawLine(pen, 0, 0, 0, 15);
			g.DrawLine(pen, 0, 15, 15, 15);
			g.DrawLine(pen, 15, 15, 15, 0);
			g.DrawLine(pen, 15, 0, 0, 0);

			// render all bars
			if (settings.UseVerticalBars)
			{
				float left = 0;
				for (int i = 0; i < nReadings; i++)
				{
					float h = (logicalProcessorUsage[i] * 16 / 100);
					g.FillRectangle(barBrush, left, 16 - h, barHeight, h);
					left += barHeight;
					g.DrawLine(shadowPen, left - 1, 16 - h + 0.5f, left - 1, 16);
				}
			}
			else // use horizontal bars
			{
				float top = 0;
				for (int i = 0; i < nReadings; i++)
				{
					float h = (logicalProcessorUsage[i] * 16 / 100);
					g.FillRectangle(barBrush, 0, top, h, barHeight);
					top += barHeight;
					g.DrawLine(shadowPen, 0, top - 1, h - 0.5f, top - 1);
				}
			}

			// create icon from bitmap
			IntPtr hIcon = (bitmapText.GetHicon());
			Icon newIcon = Icon.FromHandle(hIcon);
			Icon oldIcon = notifyIconLogicalProcessor.Icon;
			notifyIconLogicalProcessor.Icon = newIcon;
			if (oldIcon != null) DestroyIcon(oldIcon.Handle); // remember to delete the old icon

			// remember to dispose objects
			barBrush.Dispose();
			shadowPen.Dispose();
		}
		private void UpdateNotifyIconTooltipText()
		{
			// update notify icon's tooltip text

			// detemine unit for network flow readings
			float nr = lastNetworkReceive / 1024;
			float ns = lastNetworkSend / 1024;
			string unit = "KBps";
			if (nr > 1024 || ns > 1024)
			{
				nr = lastNetworkReceive / 1024;
				ns = lastNetworkSend / 1024;
				unit = "MBps";
			}

			// build the text
			StringBuilder sb = new StringBuilder();
			if (settings.ShowCpuUsage) sb.AppendLine("CPU " + Math.Round(lastCpuUsage) + "%");
			if (settings.ShowMemoryUsage) sb.AppendLine("Memory " + Math.Round(lastMemoryUsage) + "%");
			if (settings.ShowDiskUsage) sb.AppendLine("Disk " + Math.Round(lastDiskUsage) + "%");
			if (settings.ShowNetworkUsage)
			{
				sb.Append($"Network {downArrow}:" + nr.ToString("0.0"));
				sb.Append($" {upArrow}:" + ns.ToString("0.0") + " " + unit);
			}

			// make sure the tooltip text has at most 128 characters
			if (sb.Length >= 128) sb.Remove(127, sb.Length - 127);

			// update the text value
			SetNotifyIconText(notifyIconMain, sb.ToString());
		}
		private void SetNotifyIconText(NotifyIcon ni, string text)
		{
			// set notify icon text with text up to 127 characters

			if (text.Length >= 128) throw new ArgumentOutOfRangeException("Text limited to 127 characters");
			Type t = typeof(NotifyIcon);
			BindingFlags hidden = BindingFlags.NonPublic | BindingFlags.Instance;
			t.GetField("text", hidden).SetValue(ni, text);
			if ((bool)t.GetField("added", hidden).GetValue(ni))
				t.GetMethod("UpdateIcon", hidden).Invoke(ni, new object[] { true });
		}

	}


}

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

namespace IconMeter
{
	public partial class FormMain : Form
	{
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		extern static bool DestroyIcon(IntPtr handle);

		[Serializable]
		public class Settings
		{
			[XmlElement(Type = typeof(XmlColor))] public Color CpuColor { get; set; }
			[XmlElement(Type = typeof(XmlColor))] public Color MemoryColor { get; set; }
			[XmlElement(Type = typeof(XmlColor))] public Color DiskColor { get; set; }
			[XmlElement(Type = typeof(XmlColor))] public Color NetworkReceiveColor { get; set; }
			[XmlElement(Type = typeof(XmlColor))] public Color NetworkSendColor { get; set; }
			public bool  ShowMemoryUsage { get; set; }
			public bool ShowDiskUsage { get; set; }
			public bool ShowNetworkUsage { get; set; }
			public bool UseVerticalBars { get; set; }
			public bool RunAtStartup { get; set; }

			public Settings()
			{
				CpuColor = Color.Red;
				MemoryColor = Color.DodgerBlue;
				DiskColor = Color.LimeGreen;
				NetworkReceiveColor = Color.Yellow;
				NetworkSendColor = Color.Goldenrod;
				ShowMemoryUsage = true;
				ShowDiskUsage = true;
				ShowNetworkUsage = true;
				UseVerticalBars = true;
				RunAtStartup = false;
			}
		}

		Settings settings = new Settings();
		PerformanceCounter cpuCounter, memoryCounter, diskCounter;
		List<PerformanceCounter> networkReceiveCounters = new List<PerformanceCounter>();
		List<PerformanceCounter> networkSendCounters = new List<PerformanceCounter>();
		float lastCpuUsage = 0;
		float lastMemoryUsage = 0;
		float lastDiskUsage = 0;
		float lastNetworkReceive = 0;
		float lastNetworkSend = 0;
		Queue<float> previousNetwordReceive = new Queue<float>();
		Queue<float> previousNetwordSend = new Queue<float>();
		bool isClosing = false;
		bool allowShowForm = false;

		// Constructor and event handlers
		public FormMain()
        {
            InitializeComponent();

			InitializePerformanceCounters();

			LoadSettings();

			UpdateAutoStartSetting();

			timerMain.Enabled = true;
		}
		private void FormMain_Load(object sender, EventArgs e)
		{

		}
		private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing && isClosing == false)
			{
				e.Cancel = true;
				this.Hide();
			}
		}
		private void FormMain_Shown(object sender, EventArgs e)
		{
			UpdateControlsFromSettings();
		}
		private void buttonOK_Click(object sender, EventArgs e)
		{
			UpdateSettingsFromControls();
			SaveSettings();
			UpdateAutoStartSetting();
			this.Hide();
		}
		private void buttonCancel_Click(object sender, EventArgs e)
		{
			this.Hide();
		}
		private void buttonColor_Click(object sender, EventArgs e)
		{
			Button b = sender as Button;
			this.colorDialogMain.Color = b.BackColor;

			DialogResult ret = colorDialogMain.ShowDialog(this);
			if (ret == DialogResult.OK)
			{
				b.BackColor = colorDialogMain.Color;
			}
		}
		private void notifyIconMain_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				Process p = new Process();
				p.StartInfo.FileName = "taskmgr";
				p.StartInfo.CreateNoWindow = true;
				p.Start();
			}
		}
		private void closeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			isClosing = true;
			Application.Exit();
		}
		private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			allowShowForm = true;
			this.Visible = true;
			UpdateControlsFromSettings();
		}
		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AboutBox a = new AboutBox();
			a.ShowDialog();
		}
		private void timerMain_Tick(object sender, EventArgs e)
		{
			UpdateReadings();

			UpdateIcon();
		}

		// override function for disable show window on startup
		protected override void SetVisibleCore(bool value)
		{
			base.SetVisibleCore(allowShowForm ? value : false);
		}

		// main procedures
		private void SaveIconToFile(string filename)
		{
			using (FileStream s = new FileStream(filename, FileMode.Create))
			{
				this.notifyIconMain.Icon.Save(s);
			}
		}
		private void SaveSettings()
		{
			try
			{
				XmlSerializer serializer = new XmlSerializer(typeof(Settings));
				string filename = AppDomain.CurrentDomain.BaseDirectory + "settings.xml";

				using (StreamWriter writer = new StreamWriter(filename))
				{
					serializer.Serialize(writer, this.settings);
					writer.Close();
				}
			}
			catch (Exception e) { Debug.Write(e.Message); }
		}
		private void LoadSettings()
		{
			try
			{
				string filename = AppDomain.CurrentDomain.BaseDirectory + "settings.xml";
				if (File.Exists(filename) == false) return;

				XmlSerializer serializer = new XmlSerializer(typeof(Settings));
				using (StreamReader reader = new StreamReader(filename))
				{
					this.settings = (Settings)serializer.Deserialize(reader);
					reader.Close();
				}
			}
			catch (Exception e) { Debug.Write(e.Message); }
		}
		private void InitializePerformanceCounters()
		{
			cpuCounter = new PerformanceCounter("Processor Information", "% Processor Utility", "_Total");
			memoryCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
			diskCounter = new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total");

			PerformanceCounterCategory networkCounterCategory
				= new PerformanceCounterCategory("Network Interface");

			foreach (string name in networkCounterCategory.GetInstanceNames())
			{
				networkReceiveCounters.Add(new PerformanceCounter("Network Interface", "Bytes Received/sec", name));
				networkSendCounters.Add(new PerformanceCounter("Network Interface", "Bytes Sent/sec", name));
			}
		}
		private void UpdateControlsFromSettings()
		{
			this.buttonCpuColor.BackColor = settings.CpuColor;
			this.buttonMemoryColor.BackColor = settings.MemoryColor;
			this.buttonDiskColor.BackColor = settings.DiskColor;
			this.buttonReceiveColor.BackColor = settings.NetworkReceiveColor;
			this.buttonSendColor.BackColor = settings.NetworkSendColor;
			this.checkBoxMemory.Checked = settings.ShowMemoryUsage;
			this.checkBoxDisk.Checked = settings.ShowDiskUsage;
			this.checkBoxNetwork.Checked = settings.ShowNetworkUsage;
			this.checkBoxUseVerticalBar.Checked = settings.UseVerticalBars;
			this.checkBoxRunAtStartup.Checked = settings.RunAtStartup;
		}
		private void UpdateSettingsFromControls()
		{
			settings.CpuColor = buttonCpuColor.BackColor;
			settings.MemoryColor = buttonMemoryColor.BackColor;
			settings.DiskColor = buttonDiskColor.BackColor;
			settings.NetworkReceiveColor = buttonReceiveColor.BackColor;
			settings.NetworkSendColor = buttonSendColor.BackColor;
			settings.ShowMemoryUsage = checkBoxMemory.Checked;
			settings.ShowDiskUsage = checkBoxDisk.Checked;
			settings.ShowNetworkUsage = checkBoxNetwork.Checked;
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
				rkApp.SetValue("MyApp", Application.ExecutablePath);
			}
			else
			{
				// Remove the value from the registry so that the application doesn't start
				rkApp.DeleteValue("MyApp", false);
			}
		}
		private void UpdateReadings()
		{
			lastCpuUsage = cpuCounter.NextValue();
			lastMemoryUsage = memoryCounter.NextValue();
			lastDiskUsage = diskCounter.NextValue();

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
		}
		private void UpdateIcon()
		{
			Bitmap bitmapText = new Bitmap(16, 16);
			Graphics g = System.Drawing.Graphics.FromImage(bitmapText);
			Brush brush1 = new SolidBrush(settings.CpuColor);
			Brush brush2 = new SolidBrush(settings.MemoryColor);
			Brush brush3 = new SolidBrush(settings.DiskColor);
			Brush brush4 = new SolidBrush(settings.NetworkReceiveColor);
			Brush brush5 = new SolidBrush(settings.NetworkSendColor);

			int nReading = 1;
			if (settings.ShowMemoryUsage) nReading++;
			if (settings.ShowDiskUsage) nReading++;
			if (settings.ShowNetworkUsage) nReading+=2;
			int barHeight = 16 / nReading;


			g.Clear(Color.Transparent);
			Pen pen = new Pen(Color.DarkGray);
			g.DrawLine(pen, 0, 0, 0, 15);
			g.DrawLine(pen, 0, 15, 15, 15);
			g.DrawLine(pen, 15, 15, 15, 0);
			g.DrawLine(pen, 15, 0, 0, 0);

			if (settings.UseVerticalBars)
			{
				int left = 0;
				float h = (lastCpuUsage * 16 / 100);
				g.FillRectangle(brush1, left, 16 - h, barHeight, h);
				left += barHeight;
				if (settings.ShowMemoryUsage)
				{
					h = (lastMemoryUsage * 16 / 100);
					g.FillRectangle(brush2, left, 16 - h, barHeight, h);
					left += barHeight;
				}
				if (settings.ShowDiskUsage)
				{
					h = (lastDiskUsage * 16 / 100);
					g.FillRectangle(brush3, left, 16 - h, barHeight, h);
					left += barHeight;
				}
				if (settings.ShowNetworkUsage)
				{
					float maxNetworkReceive = previousNetwordReceive.Max();
					float maxNetworkSend = previousNetwordSend.Max();
					float maxNetword = Math.Max(maxNetworkReceive, maxNetworkSend);

					h = (lastNetworkSend * 16 / maxNetword);
					g.FillRectangle(brush5, left, 16 - h, barHeight, h);
					left += barHeight;
					h = (lastNetworkReceive * 16 / maxNetword);
					g.FillRectangle(brush4, left, 16 - h, barHeight, h);
				}
			}
			else // use horizontal bars
			{
				int top = 0;
				g.FillRectangle(brush1, 0, top, (lastCpuUsage * 16 / 100), barHeight);
				top += barHeight;
				if (settings.ShowMemoryUsage)
				{
					g.FillRectangle(brush2, 0, top, (lastMemoryUsage * 16 / 100), barHeight);
					top += barHeight;
				}
				if (settings.ShowDiskUsage)
				{
					g.FillRectangle(brush3, 0, top, (lastDiskUsage * 16 / 100), barHeight);
					top += barHeight;
				}
				if (settings.ShowNetworkUsage)
				{
					float maxNetworkReceive = previousNetwordReceive.Max();
					float maxNetworkSend = previousNetwordSend.Max();
					float maxNetword = Math.Max(maxNetworkReceive, maxNetworkSend);

					g.FillRectangle(brush5, 0, top, (lastNetworkSend * 16 / maxNetword), barHeight);
					top += barHeight;
					g.FillRectangle(brush4, 0, top, (lastNetworkReceive * 16 / maxNetword), barHeight);
				}
			}

			// create icon from bitmap
			IntPtr hIcon = (bitmapText.GetHicon());
			Icon newIcon = Icon.FromHandle(hIcon);
			Icon oldIcon = notifyIconMain.Icon;
			notifyIconMain.Icon = newIcon;
			if (oldIcon != null) DestroyIcon(oldIcon.Handle); // remember to delete the old icon

			float nr = lastNetworkReceive / 1024;
			float ns = lastNetworkSend / 1024;
			string unit = "KBps";
			if (nr > 1024 || ns > 1024)
			{
				nr = lastNetworkReceive / 1024;
				ns = lastNetworkSend / 1024;
				unit = "MBps";
			}

			// build the icon's tooltip text
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("CPU " + Math.Round(lastCpuUsage) + "%");
			sb.AppendLine("Memory " + Math.Round(lastMemoryUsage) + "%");
			sb.AppendLine("Disk " + Math.Round(lastDiskUsage) + "%");
			if (settings.ShowNetworkUsage)
			{
				sb.Append("Network R:" + nr.ToString("0.0"));
				sb.Append(" S:" + ns.ToString("0.0") + " " + unit);
			}

			// make sure the tooltip text has at most 64 characters
			if (sb.Length > 64) sb.Remove(64, sb.Length - 64);

			notifyIconMain.Text = sb.ToString();
		}
	}

	// helper class for serializing color values
	public class XmlColor
	{
		private Color color_ = Color.Black;

		public XmlColor() { }
		public XmlColor(Color c) { color_ = c; }

		public Color ToColor()
		{
			return color_;
		}
		public void FromColor(Color c)
		{
			color_ = c;
		}
		public static implicit operator Color(XmlColor x)
		{
			return x.ToColor();
		}
		public static implicit operator XmlColor(Color c)
		{
			return new XmlColor(c);
		}

		[XmlAttribute]
		public string Web
		{
			get
			{
				var converter = new ColorConverter();
				return converter.ConvertToString(color_);
			}
			set
			{
				var converter = new ColorConverter();
				color_ = (Color) converter.ConvertFromString(value);
			}
		}
	}
}

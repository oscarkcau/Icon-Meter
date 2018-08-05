using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Serialization;

namespace IconMeterWPF
{
	[Serializable]
	public class Settings
	{
		// settings for Icon Meter

		public Color CpuColor { get; set; } = Colors.OrangeRed;
		public Color MemoryColor { get; set; } = Colors.DodgerBlue;
		public Color DiskColor { get; set; } = Colors.LimeGreen;
		public Color NetworkReceiveColor { get; set; } = Colors.Yellow;
		public Color NetworkSendColor { get; set; } = Colors.Goldenrod;
		public Color LogicalProcessorColor { get; set; } = Colors.OrangeRed;
		public bool ShowCpuUsage { get; set; } = true;
		public bool ShowMemoryUsage { get; set; } = true;
		public bool ShowDiskUsage { get; set; } = true;
		public bool ShowNetworkUsage { get; set; } = true;
		public bool ShowLogicalProcessorsUsage { get; set; } = false;
		public bool UseVerticalBars { get; set; } = true;
		public bool RunAtStartup { get; set; } = false;

		// public methods
		public Settings Clone()
		{
			// clone a new Settings instance

			//serualize to memory stream
			XmlSerializer serializer = new XmlSerializer(typeof(Settings));
			MemoryStream memStream = new MemoryStream();
			serializer.Serialize(memStream, this);

			// descerialize to new instance and return
			memStream.Position = 0;
			return ((Settings)serializer.Deserialize(memStream));
		}
		public void SaveToFile(string filename)
		{
			// serialize settings to xml file

			// serialize to file stream
			XmlSerializer serializer = new XmlSerializer(typeof(Settings));
			using (StreamWriter writer = new StreamWriter(filename))
			{
				serializer.Serialize(writer, this);
				writer.Close();
			}
		}
		public static Settings LoadFromFile(string filename)
		{
			// read and deserialize settings from file

			// ensure file exist before reading
			if (File.Exists(filename) == false) throw new FileNotFoundException();

			// read and deserialize from file stream
			XmlSerializer serializer = new XmlSerializer(typeof(Settings));
			using (StreamReader reader = new StreamReader(filename))
			{
				Settings s = (Settings)serializer.Deserialize(reader);
				reader.Close();
				return s;
			}
		}
	}
}

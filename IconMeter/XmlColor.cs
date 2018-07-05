using System.Drawing;
using System.Xml.Serialization;

namespace IconMeter
{
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
		public string Web {
			get
			{
				var converter = new ColorConverter();
				return converter.ConvertToString(color_);
			}
			set
			{
				var converter = new ColorConverter();
				color_ = (Color)converter.ConvertFromString(value);
			}
		}
	}
}
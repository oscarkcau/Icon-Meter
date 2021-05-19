using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;

namespace IconMeterWPF
{
	class IconBuilder
	{
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		static extern int GetSystemMetrics(int nIndex);
		const int SM_CXSMICON = 49;

		static readonly int iconSize;
		static readonly Font drawFont;

		static IconBuilder() 
		{
			// get the default tray icon size
			iconSize = GetSystemMetrics(SM_CXSMICON);

			// create drawing objects
			drawFont = new Font(SystemFonts.DefaultFont.FontFamily, iconSize/2, FontStyle.Bold, GraphicsUnit.Pixel);
		}

		public static Icon BuildIcon(IEnumerable<(float, Brush)> list, bool useVerticalBar = false, bool drawShadow = true, string label = null)
		{
			// draw new icon according the input reading values and brushes

			// create bitmap and corresponding graphics objects
			Bitmap bmp = new Bitmap(iconSize, iconSize);
			Graphics g = System.Drawing.Graphics.FromImage(bmp);
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;

			// clear background and draw bounding box
			Color bgColor = Properties.Settings.Default.TrayIconBackgroundColor;
			g.Clear(bgColor);
			int t = iconSize - 1;
			Pen linePen = Pens.DarkGray;
			g.DrawLine(linePen, 0, 0, 0, t);
			g.DrawLine(linePen, 0, t, t, t);
			g.DrawLine(linePen, t, t, t, 0);
			g.DrawLine(linePen, t, 0, 0, 0);

			// draw label if it is provided
			if (string.IsNullOrWhiteSpace(label) == false)
            {
				// pick light or dark font color based on background color
				float bgIntensity = bgColor.R * 0.2989f + bgColor.G * 0.5870f + bgColor.B * 0.1140f;
				Color fontColor = bgIntensity > 128 ? Color.DimGray : Color.DarkGray;

				// render label with selected font and color
				using (SolidBrush drawBrush = new SolidBrush(fontColor))
				{
					if (useVerticalBar)
					{
						g.DrawString(label, drawFont, drawBrush, 1, 1);
					}
					else
					{
						SizeF labelSize = g.MeasureString(label, drawFont);
						g.DrawString(label, drawFont, drawBrush, iconSize - labelSize.Width - 1, iconSize - labelSize.Height);
					}
				}
			}

			// compute bar height
			int nReadings = list.Count();
			float barHeight = iconSize / nReadings;

			// render all bars
			using (Pen shadowPen = new Pen(Color.FromArgb(128, Color.Black)))
			{
				if (useVerticalBar)
				{
					float left = 0;
					foreach (var (value, brush) in list)
					{
						float height = value * iconSize / 100.0f;
						g.FillRectangle(brush, left, iconSize - height, barHeight, height);
						left += barHeight;
						if (drawShadow)
							g.DrawLine(shadowPen, left - 1, iconSize - height + 0.5f, left - 1, iconSize);
					}
				}
				else // use horizontal bars
				{
					float top = 0;
					foreach (var (value, brush) in list)
					{
						float height = value * iconSize / 100.0f;
						g.FillRectangle(brush, 0, top, height, barHeight);
						top += barHeight;
						if (drawShadow)
							g.DrawLine(shadowPen, 0, top - 1, height - 0.5f, top - 1);
					}
				}
			}

			// create icon from bitmap
			return System.Drawing.Icon.FromHandle(bmp.GetHicon());
		}

	}
}

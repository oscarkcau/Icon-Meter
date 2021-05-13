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

		static readonly int systemTrayIconSize;
		static readonly Font drawFont;

		static IconBuilder() 
		{
			// get the default tray icon size
			systemTrayIconSize = GetSystemMetrics(SM_CXSMICON);

			// create drawing objects
			drawFont = new Font("Arial", 9, FontStyle.Bold, GraphicsUnit.Pixel);
		}

		public static Icon BuildIcon(IEnumerable<(float, Brush)> list, bool useVerticalBar = false, bool drawShadow = true, string label = null)
		{
			// draw new icon according the input reading values and brushes

			// return default icon if no reading to display
			int nReadings = list.Count();
			//if (nReadings == 0) return null;

			// determine icon size
			int iconSize =
				nReadings <= 4 ? 16 :
				32 < systemTrayIconSize ? 32 :
				systemTrayIconSize;

			// create bitmap and corresponding graphics objects
			Bitmap bmp = new Bitmap(iconSize, iconSize);
			Graphics g = System.Drawing.Graphics.FromImage(bmp);
			
			
			// clear background and draw bounding box
			g.Clear(Color.Transparent);
			int t = iconSize - 1;
			Pen linePen = Pens.DarkGray;
			g.DrawLine(linePen, 0, 0, 0, t);
			g.DrawLine(linePen, 0, t, t, t);
			g.DrawLine(linePen, t, t, t, 0);
			g.DrawLine(linePen, t, 0, 0, 0);

			// draw label if it is provided
			if (string.IsNullOrWhiteSpace(label) == false)
            {
				using (SolidBrush drawBrush = new SolidBrush(Color.FromArgb(128, Color.White)))
				{
					SizeF labelSize = g.MeasureString(label, drawFont);
					if (useVerticalBar)
						g.DrawString(label, drawFont, drawBrush, 0, 0);
					else
						g.DrawString(label, drawFont, drawBrush, iconSize - labelSize.Width, iconSize - labelSize.Height);
				}
			}

			// compute bar height
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

			return System.Drawing.Icon.FromHandle(bmp.GetHicon());
		}

	}
}

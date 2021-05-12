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
		static System.Drawing.Font drawFont;
		static System.Drawing.SolidBrush drawBrush;

		static IconBuilder() 
		{
			// get the default tray icon size
			systemTrayIconSize = GetSystemMetrics(SM_CXSMICON);

			drawFont = new System.Drawing.Font("Arial", 5, FontStyle.Bold);
			drawBrush = new System.Drawing.SolidBrush(Color.FromArgb(128, Color.White));
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

			// create bitmap and corresponding graphics object
			Bitmap bmp = new Bitmap(iconSize, iconSize);
			Graphics g = System.Drawing.Graphics.FromImage(bmp);
			Pen shadowPen = new Pen(Color.FromArgb(128, Color.Black));

			// clear background and draw bounding box
			g.Clear(Color.Transparent);
			Pen pen = new Pen(Color.DarkGray);
			int t = iconSize - 1;
			g.DrawLine(pen, 0, 0, 0, t);
			g.DrawLine(pen, 0, t, t, t);
			g.DrawLine(pen, t, t, t, 0);
			g.DrawLine(pen, t, 0, 0, 0);

			if (string.IsNullOrWhiteSpace(label) == false)
            {
				if (useVerticalBar)
					g.DrawString(label, drawFont, drawBrush, 0, 0);
				else
					g.DrawString(label, drawFont, drawBrush, iconSize-11, iconSize-13);
			}

			// compute bar height
			float barHeight = iconSize / nReadings;

			// render all bars
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

			// remember to dispose objects
			shadowPen.Dispose();

			return System.Drawing.Icon.FromHandle(bmp.GetHicon());
		}

	}
}

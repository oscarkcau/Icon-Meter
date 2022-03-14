using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IconMeterWPF
{
	/// <summary>
	/// Interaction logic for PopupWindow.xaml
	/// </summary>
	public partial class PopupWindow : UserControl
	{
		// private fields
		private bool isAnchorMouseDown = false;
		private Point mouseDownPosition;

		// constructor
		public PopupWindow()
		{
			InitializeComponent();
		}

		// event handlers
		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			MainViewModel vm = DataContext as MainViewModel;
			vm?.PopupMeter?.Resume();
		}
		private void UserControl_Unloaded(object sender, RoutedEventArgs e)
		{
			MainViewModel vm = DataContext as MainViewModel;
			vm?.PopupMeter?.Pause();
		}
		private void Image_MouseUp(object sender, MouseButtonEventArgs e)
		{
			Image img = sender as Image;
			ImagePressed(img);
		}
		private void Image_TouchUp(object sender, TouchEventArgs e)
		{
			Image img = sender as Image;
			ImagePressed(img);
		}
		private void ImageAnchor_MouseDown(object sender, MouseButtonEventArgs e)
		{
			// set flag
			isAnchorMouseDown = true;

			// record mouse down position for calculate relative movement
			mouseDownPosition = e.GetPosition(this);

			// force button to receive mouse events
			ImageAnchor.CaptureMouse();
		}
		private void ImageAnchor_MouseMove(object sender, MouseEventArgs e)
		{
			// if it is mouse drag
			if (isAnchorMouseDown)
			{
				// calcute relative mouse movement
				Point pos = e.GetPosition(this);
				double dx = pos.X - mouseDownPosition.X;
				double dy = pos.Y - mouseDownPosition.Y;

				// update popup window position
				Popup p = Parent as Popup;
				p.HorizontalOffset += dx;
				p.VerticalOffset += dy;
			}
		}
		private void ImageAnchor_MouseUp(object sender, MouseButtonEventArgs e)
		{
			// reset flag
			isAnchorMouseDown = false;

			// release mouse capture 
			ImageAnchor.ReleaseMouseCapture();

			// keep popup be shown even focus is lost
			Popup p = Parent as Popup;
			p.StaysOpen = true;
		}

		// private methods
		private void ImagePressed(Image sender)
		{
			if (sender == ImageClose)
			{
				// hide popup window
				Popup p = Parent as Popup;
				p.IsOpen = false;
			}

			if (sender == ImageConfig)
			{
				// pause update readings
				MainViewModel vm = DataContext as MainViewModel;
				vm?.PauseUpdate();

				// show setting window
				Window w = vm?.MainWindow;
				if (w != null)
				{
					w.Show();
					w.WindowState = WindowState.Normal;
					w.Visibility = Visibility.Visible;
					w.ShowInTaskbar = true;
				}
			}

			if (sender == ImageTask)
			{
				// start task manager
				System.Diagnostics.Process.Start("taskmgr");
			}

			if (sender == ImageControl)
			{
				// start control panel
				System.Diagnostics.Process.Start("control");
			}
			
			if (sender == ImageAbout)
			{
				// hide popup window
				Popup p = Parent as Popup;
				p.IsOpen = false;

				// show about dialog
				AboutBox aboutBox = new AboutBox();
				aboutBox.ShowDialog();
			}

			if (sender == ImageRefresh)
			{
				MainViewModel vm = DataContext as MainViewModel;
				vm?.PopupMeter.UpdateIPs();
			}
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace IconMeterWPF
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		[DllImport("user32.dll")]
		internal static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetCursorPos(ref Win32Point pt);

		[StructLayout(LayoutKind.Sequential)]
		internal struct Win32Point
		{
			public int X;
			public int Y;
		};

		// private field
		private Point taskbarIconMouseDownPosition;

		// constructor
		public MainWindow()
		{
			InitializeComponent();

			// add reference to MainViewModel
			MainViewModel vm = DataContext as MainViewModel;
			vm.MainWindow = this;
	
			// setup property changed listener for tray icon update
			vm.Meter.PropertyChanged += Meter_PropertyChanged;
			vm.PopupMeter.PropertyChanged += PopupMeter_PropertyChanged;

			// setup defailt icon for PerformanceMeter object
			var uri = new Uri(@"pack://application:,,,/icon.ico", UriKind.RelativeOrAbsolute);
			Stream iconStream = Application.GetResourceStream(uri).Stream;
			vm.Meter.DefaultTrayIcon = new System.Drawing.Icon(iconStream);

			// initial ComboBox for language selection
			ResourceSet resourceSet = 
				Properties.LanguageResource.ResourceManager.GetResourceSet(
					CultureInfo.CurrentUICulture, true, true
					);
			cmbLanguage.ItemsSource = resourceSet;
		}

		// event handlers
		private void Meter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			// update tray icons if needed
			MainViewModel vm = DataContext as MainViewModel;
			if (e.PropertyName == "MainTrayIcon")
			{
				MainTaskbarIcon.Icon = vm.Meter.MainTrayIcon;
			}
			if (e.PropertyName == "LogicalProcessorsTrayIcon")
			{
				LogicalProcessorsTaskbarIcon.Icon = vm.Meter.LogicalProcessorsTrayIcon;
			}
		}
		private void PopupMeter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "DiskActiveTimeTrayIcon")
			{
				Dispatcher.Invoke(() =>
			   {
				   MainViewModel vm = DataContext as MainViewModel;
				   IndividualDiskTaskbarIcon.Icon = vm.PopupMeter.DiskActiveTimeTrayIcon;
			   });
			}
		}
		private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
			// hide setting window
			Hide();

			// save settings and resume update readings
			MainViewModel vm = DataContext as MainViewModel;
			vm.SaveSettings();
			vm.ResumeUpdate();
		}
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
		{
			// hide setting window
			Hide();

			// discard new settings and resume update readings
			MainViewModel vm = DataContext as MainViewModel;
			vm.ReloadSettings();
			vm.ResumeUpdate();
		}
		private void MenuItemSettings_Click(object sender, RoutedEventArgs e)
		{
			// pause update readings
			MainViewModel vm = DataContext as MainViewModel;
			vm.PauseUpdate();

			// show setting window
			Show();
			WindowState = System.Windows.WindowState.Normal;
			Visibility = Visibility.Visible;
			ShowInTaskbar = true;
		}
		private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
		{
			AboutBox aboutBox = new AboutBox();
			aboutBox.ShowDialog();
		}
		private void MenuItemClose_Click(object sender, RoutedEventArgs e)
		{
			MainTaskbarIcon.Visibility = Visibility.Collapsed;
			LogicalProcessorsTaskbarIcon.Visibility = Visibility.Collapsed;
			Application.Current.Shutdown();
		}
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
			// cancel closing settings window
            e.Cancel = true;

			// hide settings window
            Hide();

			// discard new settings and resume update readings
			MainViewModel vm = DataContext as MainViewModel;
			vm.ReloadSettings();
			vm.ResumeUpdate();
		}
		private void Window_SourceInitialized(object sender, EventArgs e)
		{
			// hide this setting window after its size and position is initialized
			WindowState = System.Windows.WindowState.Minimized;
			Visibility = Visibility.Hidden;
		}
		private void MainTaskbarIcon_TrayLeftMouseDown(object sender, RoutedEventArgs e)
		{
			taskbarIconMouseDownPosition = GetMousePosition();
		}
		private void LogicalProcessorsTaskbarIcon_TrayLeftMouseDown(object sender, RoutedEventArgs e)
		{
			taskbarIconMouseDownPosition = GetMousePosition();
		}

		// private methods
		private Point GetMousePosition()
		{
			// get raw mouse position
			Win32Point w32Mouse = new Win32Point();
			GetCursorPos(ref w32Mouse);
			Point p = new Point(w32Mouse.X, w32Mouse.Y);

			// scale to current DPI setting
			Matrix transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
			return transform.Transform(p);
		}

		// public methods
		public void ShowPopup()
		{
			// set popup window position to mouse down position
			popup.Placement = System.Windows.Controls.Primitives.PlacementMode.AbsolutePoint;
			popup.HorizontalOffset = taskbarIconMouseDownPosition.X;
			popup.VerticalOffset = taskbarIconMouseDownPosition.Y;

			// show the popup
			popup.IsOpen = true;
			popup.StaysOpen = false;

			// set this window to be foreground window, which ensures the popup hides when user click somewhere else on the screen
			HwndSource source = (HwndSource)PresentationSource.FromVisual(this);
			IntPtr handle = source.Handle;
			SetForegroundWindow(handle);
		}
	}

	public class BoolToVisibility : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			bool b = (bool)value;
			return b ? Visibility.Visible : Visibility.Hidden;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			Visibility v = (Visibility)value;

			switch (v)
			{
				case Visibility.Visible: return true;
				case Visibility.Hidden: return false;
				case Visibility.Collapsed: return false;
			}

			return false;
		}
	}

    public class DrawingColorToWindowsMediaColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            System.Drawing.Color c = (System.Drawing.Color)value;
            return Color.FromArgb(c.A, c.R, c.G, c.B);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
			Color c = (System.Windows.Media.Color)value;
            return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
        }
    }

	public class ToUpperValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string str = value as string;
			return string.IsNullOrEmpty(str) ? string.Empty : str.ToUpper();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}

	public class ToLowerValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string str = value as string;
			return string.IsNullOrEmpty(str) ? string.Empty : str.ToLower();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}

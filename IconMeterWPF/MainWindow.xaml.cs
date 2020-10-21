using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
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
		// constructor
		public MainWindow()
		{
			InitializeComponent();

			// hide window before loading
			this.Visibility = Visibility.Hidden;

			
			var vm = this.DataContext as MainViewModel;

			vm.MainWindow = this;
	
			// setup property changed listener for tray icon update
			vm.Meter.PropertyChanged += Meter_PropertyChanged;

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

			// ensure popup window will be loaded
			MainTaskbarIcon.TrayPopupResolved.IsOpen = true;
			MainTaskbarIcon.TrayPopupResolved.IsOpen = false;

		}

		// event handlers
		private void Meter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)  
		{
			// update tray icons if needed
			var vm = this.DataContext as MainViewModel;
			if (e.PropertyName == "MainTrayIcon") MainTaskbarIcon.Icon = vm.Meter.MainTrayIcon;
			if (e.PropertyName == "LogicalProcessorsTrayIcon") LogicalProcessorsTaskbarIcon.Icon = vm.Meter.LogicalProcessorsTrayIcon;
		}
        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
			// hide setting window
			this.Hide();

			// save settings and resume update readings
			var vm = this.DataContext as MainViewModel;
			vm.SaveSettings();
			vm.ResumeUpdate();
		}
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
		{
			// hide setting window
			this.Hide();

			// discard new settings and resume update readings
			var vm = this.DataContext as MainViewModel;
			vm.ReloadSettings();
			vm.ResumeUpdate();
		}
		private void MenuItemSettings_Click(object sender, RoutedEventArgs e)
		{
			// pause update readings
			var vm = this.DataContext as MainViewModel;
			vm.PauseUpdate();

			// show setting window
			this.Show();
		}
		private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
		{
			AboutBox aboutBox = new AboutBox();
			aboutBox.ShowDialog();
		}
		private void MenuItemClose_Click(object sender, RoutedEventArgs e)
		{
			System.Windows.Application.Current.Shutdown();
		}
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
			// cancel closing settings window
            e.Cancel = true;

			// hide settings window
            this.Hide();

			// discard new settings and resume update readings
			var vm = this.DataContext as MainViewModel;
			vm.ReloadSettings();
			vm.ResumeUpdate();
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
            return System.Windows.Media.Color.FromArgb(c.A, c.R, c.G, c.B);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            System.Windows.Media.Color c = (System.Windows.Media.Color)value;
            return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
        }
    }

	public class ToUpperValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var str = value as string;
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
			var str = value as string;
			return string.IsNullOrEmpty(str) ? string.Empty : str.ToLower();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}

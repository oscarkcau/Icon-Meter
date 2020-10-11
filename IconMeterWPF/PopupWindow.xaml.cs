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
		public PopupWindow()
		{
			InitializeComponent();
		}

		private void Border_Unloaded(object sender, RoutedEventArgs e)
		{
			//var vm = this.DataContext as MainViewModel;
			//vm.PopupMeter.Pause();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			var vm = this.DataContext as MainViewModel;
			vm.PopupMeter.Resume();
		}

		private void UserControl_Unloaded(object sender, RoutedEventArgs e)
		{
			var vm = this.DataContext as MainViewModel;
			vm.PopupMeter.Pause();
		}

		private void ImageClose_MouseUp(object sender, MouseButtonEventArgs e)
		{
			var p = this.Parent as Popup;
			p.IsOpen = false;
		}

		private void ImageClose_TouchUp(object sender, TouchEventArgs e)
		{
			var p = this.Parent as Popup;
			p.IsOpen = false;
		}
	}
}

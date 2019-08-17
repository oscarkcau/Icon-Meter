using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace IconMeterWPF
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		Mutex myMutex;

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			// ensure single instance
			myMutex = new Mutex(true, "IconMeter-windlknwgcouhq", out bool aIsNewInstance);
			if (!aIsNewInstance)
			{
				App.Current.Shutdown();
			}
			else
			{
				System.Threading.Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
				//new System.Globalization.CultureInfo("ja");

				MainWindow window = new MainWindow();
			}
		}
	}
}

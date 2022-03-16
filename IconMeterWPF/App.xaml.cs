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
		// Mutex object for ensuring only single instance of application is allowed
		private Mutex myMutex;

		private ColorTheme colorTheme = ColorTheme.Light;

		public ColorTheme ColorTheme
		{
			get { return colorTheme; }
			set
			{
				if (value != colorTheme)
				{
					colorTheme = value;

					foreach (ResourceDictionary d in Resources.MergedDictionaries)
					{
						if (d is ThemeResourceDictionary td)
							td.UpdateSource();
					}
				}
			}
		}

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			// wait the previous instance to close when it is restarting
			if (IconMeterWPF.Properties.Settings.Default.IsRestarting)
			{
				// reset the restarting flag
				IconMeterWPF.Properties.Settings.Default.IsRestarting = false;
				IconMeterWPF.Properties.Settings.Default.Save();
				Thread.Sleep(2000);
			}

			// ensure single instance
			myMutex = new Mutex(true, "IconMeter-windlknwgcouhq", out bool aIsNewInstance);
			if (!aIsNewInstance)
			{
				Current.Shutdown();
				return;
			}

			Properties.Settings settings = IconMeterWPF.Properties.Settings.Default;

			// if no language is selected (i.e. default setting of first run)
			if (settings.Language == "")
			{
				// set default lang to English
				settings.Language = "en";

				// specify any matched and supported language
				string defaultLang = Thread.CurrentThread.CurrentCulture.Name;
				if (defaultLang.StartsWith("ja")) settings.Language = "ja-JP";
				if (defaultLang.StartsWith("zh")) settings.Language = "zh";
				if (defaultLang.StartsWith("zh-CHS")) settings.Language = "zh-CN";
				if (defaultLang.StartsWith("zh-CN")) settings.Language = "zh-CN";

				// save the new language setting
				settings.Save();
			}

			// set the language being used
			Thread.CurrentThread.CurrentUICulture = 
				new System.Globalization.CultureInfo(settings.Language);

			// apply color theme
			ColorTheme = settings.UseDarkMode ? ColorTheme.Dark : ColorTheme.Light;

			// create main window
			MainWindow w = new MainWindow();
			w.Show();
		}
	}
}

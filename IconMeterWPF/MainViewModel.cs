using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace IconMeterWPF
{
	class MainViewModel : INotifyPropertyChanged
	{
		// private fields
		string settingsFilename = "";
		Settings originalSettings;
		Settings _settings;
		PerformanceMeter _meter;
		bool _showLogicalProcessorsUsage;

		// properties
		public Settings Settings { get => _settings; private set => SetField(ref _settings, value); }
		public PerformanceMeter Meter { get => _meter; private set => SetField(ref _meter, value); }
		public bool ShowLogicalProcessorsUsage { get => _showLogicalProcessorsUsage; private set => SetField(ref _showLogicalProcessorsUsage, value); }
		public ICommand SaveAndReset { get; private set; }
		public ICommand LoadAndReset { get; private set; }
		public ICommand CancelSettingsUpdate { get; private set; }
		public ICommand StartTaskManager { get; private set; }

		// constructors
		public MainViewModel()
		{
			string loadlDataPath = 
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
				Path.DirectorySeparatorChar + "Icon_Meter";
			settingsFilename =
				loadlDataPath + 
				Path.DirectorySeparatorChar + "settings.xml";
			System.IO.Directory.CreateDirectory(loadlDataPath);

			// initial all public ICommand objects
			InitCommands();

			// load settings from file and reset meter
			_LoadAndReset();
		}

		// private methods
		void InitCommands()
		{
			//
			// init ICommand objects for binding
			//

			SaveAndReset = new RelayCommand(_SaveAndReset);
			LoadAndReset = new RelayCommand(_LoadAndReset);
			CancelSettingsUpdate = new RelayCommand(_CancelSettingsUpdate);
			StartTaskManager = new RelayCommand(_StartTaskManager);
		}
		void _SaveAndReset(object obj = null)
		{
			//
			// save settings and reset meter
			//

			// update underlying setting object and save settings to file
			originalSettings = Settings.Clone();
			originalSettings.SaveToFile(this.settingsFilename);

			// update exposed property 
			ShowLogicalProcessorsUsage = Settings.ShowLogicalProcessorsUsage;

			// update autostart setting
			UpdateAutoStartSetting();

			// update meter with new settings
			Meter.ResetPerformanceMeter(originalSettings);
		}
		void _LoadAndReset(object obj = null)
		{
			//
			// load settings from file and reset meter
			//

			// load settings and copy to exposed setting instance
			try
			{
				originalSettings = Settings.LoadFromFile(settingsFilename);
			}
			catch { originalSettings = new Settings(); }
			Settings = originalSettings.Clone();

			// update exposed property 
			ShowLogicalProcessorsUsage = Settings.ShowLogicalProcessorsUsage;

			// update autostart setting
			UpdateAutoStartSetting();

			// reset existing meter or create new meter 
			if (Meter == null)
				Meter = new PerformanceMeter(originalSettings);
			else
				Meter.ResetPerformanceMeter(originalSettings);
		}
		void _CancelSettingsUpdate(object obj = null)
		{
			//
			// Cancle setting update
			//

			// copy underlying settings to exposed instance
			Settings = originalSettings.Clone();
		}
		void _StartTaskManager(object obj = null)
		{
			// start Task Manager
			Process p = new Process();
			p.StartInfo.FileName = "taskmgr";
			p.Start();
		}
		void UpdateAutoStartSetting()
		{
			// The path to the key where Windows looks for startup applications
			RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

			if (Settings.RunAtStartup)
			{
				// Add the value in the registry so that the application runs at startup
				rkApp.SetValue("IconMeter", System.Reflection.Assembly.GetExecutingAssembly().Location);
			}
			else
			{
				// Remove the value from the registry so that the application doesn't start
				rkApp.DeleteValue("IconMeter", false);
			}
		}

		// INotifyPropertyChanged implementation
		public event PropertyChangedEventHandler PropertyChanged;
		protected void SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(field, value)) return;
			field = value;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}

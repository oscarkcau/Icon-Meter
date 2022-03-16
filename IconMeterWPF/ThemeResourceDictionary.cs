using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IconMeterWPF
{
    public enum ColorTheme { Light, Dark }

	public class ThemeResourceDictionary : ResourceDictionary
    {
        private Uri _lightThemeSource;
        private Uri _darkThemeSource;

        public Uri LightThemeSource
        {
            get { return _lightThemeSource; }
            set
            {
                _lightThemeSource = value;
                UpdateSource();
            }
        }
        public Uri DarkThemeSource
        {
            get { return _darkThemeSource; }
            set
            {
                _darkThemeSource = value;
                UpdateSource();
            }
        }

        public void UpdateSource()
        {
            var val = (Application.Current as App).ColorTheme == ColorTheme.Light ? LightThemeSource : DarkThemeSource;
            if (val != null && base.Source != val)
                base.Source = val;
        }
    }
}
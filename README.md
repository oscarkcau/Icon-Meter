# Icon-Meter
Small notifyicon system performance meter for MS Windows.

![](/IconMeter/capture.png)

I need a small system performance meter for my windows tablet thus make a small Winform application with c#. It display a small notifyicon which visualizes the current CPU, memory, disk and network loading in small bars.
Icon Meter has the following features:

* Customizable bar colors
* Optionally hide / display bars of memory, disk and network performance
* Use vertical or horizontal bars
* Autostart when Windows start up
* Quick launch for Task Manager
* Display numerical readings in popup bubble when mouse cursor hovers over the meter

The following coding features were implemented for the function

* Uses **Mutex** to allow only single running instance.
* Uses simple **XML serialization** for storing settings.
* Uses custom type converter to serialize System.Drawing.Color values.
* Overrides **SetVisibleCore** method for hidden main window during program startup
* Uses Registry key to enable autostart.
* Draws dynamic icon using GDI+ 

## Installation

Download and run the [installer](/IconMeterSetup/Release/IconMeterSetup.msi).

## Usage

* Hover the mouse over the meter to display current numerical performance readings. 
* Right click the meter to access the setup dialog (**setup** menu item), or to close the program (**Close** menu item).
* All settings could be found in the setup dialog.
* Double left click the meter to launch the system Task Manager.




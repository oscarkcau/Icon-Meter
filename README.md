# Icon-Meter
Small notifyicon system performance meter for MS Windows.

![](/IconMeter/images/capture.png)

I need a small and lightweight system performance meter for my windows tablet thus make a simple application with c#. It displays small notifyicons which visualize the current CPU, memory, disk and network loading in small bars.
Icon Meter has the following features:

* (Version 2.0) Reimplement using WPF for better support for Windows 10
* Customizable bar colors
* Optionally hide / display bars of memory, disk and network performance
* (Version 1.1) Visualize individual logical processor usage
* Use vertical or horizontal bars
* Autostart when Windows start up
* Quick launch for Task Manager
* Display numerical readings in popup tooltip message when mouse cursor hovers over the meter

The following coding features were implemented for the function

* Uses **Mutex** to allow only single running instance.
* Overrides **SetVisibleCore** method for hidden main window during program startup
* Uses Registry key to enable autostart.
* Draws dynamic icon using GDI+ 

## Installation

Download and run the lastest [installer](https://github.com/oscarkcau/Icon-Meter/releases/latest).

## Usage

* Hover the mouse over the meter to display current numerical performance readings. 
* Right click the meter to access the setup dialog (**setup** menu item), or to close the program (**Close** menu item).
* All settings could be found in the setup dialog.
* Double left click the meter to launch the system Task Manager.




# HEA:RT MobileApplication


The HEA:RT mobile application is developed using the cross platform Xamarin Forms framework. 

The project targets the following versions as of this time (upgrade as applicable when developing):

- Android: Version 9.0 Pie 
- UWP: The current UWP version is not compatible with the version  of the shared .NET class library (.NET 2.1).

- iOS: Not verified

## Get started 
Install latest version of Visual Studio toghether with the following workloads:
- **Mobile development with .NET** \-
containing nessesary SDK managers.

- **.NET desktop development** \- to build Windows Forms and console applications using C# with .NET core and .NET
- **Universal Windows Platform development, UWP** 
- **ASP.NET and web development** \- includes support for dockers and containers.

Also, install the latest version of Android  SDK using Tools > Andriod > Android  SDK Manager

## Launch 
 Open the solution file (.sln) with Visual Studio and run it.

 The project utilizes the following NuGets packages. They will be restored at build.

-	NETStandard.Library
-	Xamarin.Essentials
-	Xam.Plugins.Messaging
-	Rg.Plugins.PopUp
-	Microsoft.AspNetCore.SignalR.Client 
-	Newtonsoft.Json

## The Application

MedTechClient is for now a simple test application for the project **HEA:RT** at the Dependable Systems program at Mälardalens University. The application is created to show one way of visualizing sensor data and to test the communication with the BaseStation.

#### Xamarin
A Xamarin forms application is built in several abstraction layers. The *Content pages* are made up of 2 types of pages:
- The controller page ( .xaml.cs), where the functionality and navigation between pages is happening.
- The view page ( .xaml). where the UI elements are created.

Behind these pages the *view models* handle states and operations.


#### Settings
MedTechClient shows heart rate in real time using the open source SignalR library. The default url is hard-coded in ChangeServer.xaml.cs > ButtonPressed > url. 

To start the connection: 
- Start server side, (the SignalR hub). 
- Click the  *Change Server URL* button, select SAVE to use default url, or type in a new url. 
- Go back to main page to see your current heart rate.

## Future
For now, MedTechClient contains of only a few pages to show how the application could look like in different scenarios. In future updates, the user should get a correct evaluation of their heart condition when pressing *Cardiac Status*. And if their status is life-threating, the application should be able to automatically send an alarm to a pre-selected number.




## Useful documentation

Microsoft Docs Xamarins.Forms documentation: https://docs.microsoft.com/en-us/xamarin/xamarin-forms/ 

Installing Andriod emulator: https://docs.microsoft.com/sv-se/xamarin/android/get-started/installation/android-emulator/

To get started with Xamarin on iOS: 
https://docs.microsoft.com/sv-se/xamarin/ios/get-started/

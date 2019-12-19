# PROJECT HEA:RT
**Health Evaluation and Awereness using Reliable Technology**

![The System](img/logotype.png)

## About
This is a student project at [Mälardalens University](https://www.mdh.se/international) (Sweden). Eight students has worked on this during a semester within the course **Project Course in Dependable Systems**. The goal is to continue this project for the years to come. The first phase uploded in this repository contains the concept phase.

## Intro

This project is a multisensor platform for conducting realtime measurements of wireless sensors placed on a human body.
The measurements are analyzed with a set of algorithms to determine heartrate and arrhythmia. 
The platform has support for sending the data to a mobile application and in the future to a database.

## Getting Started

### Requirements

* Development: Windows or Linux (limited support for gRPC and Xamarin) 
* Runtime: Windows or Linux
* .NET Core (tested verison: 3.0)

### Sensors

The platform supports sensors from [Shimmer](http://shimmersensing.com/).
Documentation about the shimmerAPI can be found on their [Github repository](https://github.com/ShimmerEngineering/Shimmer-C-API).

#### Implemented Shimmer Sensors

* [Shimmer3 ECG Unit](http://shimmersensing.com/products/shimmer3-ecg-sensor)
* [Shimmer3 GSR+ Unit](http://shimmersensing.com/products/shimmer3-wireless-gsr-sensor)

### For Linux
[Bluethooth Linux Readme](Basestation/docs/BashScript.md.md)

### Windows and Linux

```bash
#Clone repo
git clone https://github.com/emiilbjorklund/hea_rt.git
cd hea_rt

#Build service executable
cd Basestation/Basestation.Service
dotnet build

#Launch development system
cd ../DevLauncher
dotnet run -- -y init_conf_dev.yml
```

### Note
The shimmerAPI for connecting to the sensors is not included in this repository but can easly be downloaded from their repository see link above.
The library for PPG heart rate is also available on shimmers repository.
For in-depth information about the project see docs.

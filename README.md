# PROJECT HEA:RT
**Health Evaluation and Awereness using Reliable Technology**

![The System](img/logotype.png)

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
git clone https://github.com/emiilbjorklund/hea_rt.git
cd Basestation
etc etc etc
```

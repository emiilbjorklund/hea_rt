# Implementing new sensors

**Basestation.DataAcqusition** is responsable for communication with sensors.

## Configuration

Navigate to **Basestation.Common** solution in the project. In the `Sensors.cs` file each sensors require to be defined.

```C#
public enum SensorType
{
    EcgTestData,
    PpgTestData,
    ShimmerEcg,
    ShimmerPpg
}
```

### Note

The new sensor will require a definition in the configuration file (.yml). See [System Configuration Readme](SystemConfigurationFile.md)

## Example implementation with Shimmer

Navigate to the **Basestation.DataAcqusition** solution in the project.
In the `SensorManager.cs` constructor each sensor are created based on the input of the constructor and stored in a list depending on sensor type.

```C#
switch (sensor.Type)
{
    case SensorType.ShimmerEcg:
    Console.WriteLine($"Adding ecg sensor: {typeof(ShimmerEcg)}: {sensor.Id}");
    EcgSensors.Add(new ShimmerEcg(sensor.Id));
    break;
...
}
```

### Initialization
All implementations of sensors need to inherit from the abstract base class `SensorStream`.
All sensors requires a uniqe ID and shall be included in the constructor.

```C#
//Class header for Shimmer ECG
public class ShimmerEcg : SensorStream<EcgData> {}
```



### Message Types
The sensor types have uniqe data messages classes for communication.

```C#
//ECG Message
public class EcgData : DataMessage
{
    public double Ll_Ra { get; set; }
    public double La_Ra { get; set; }
    public double Vx_Rl { get; set; }
    public double Ll_La => Ll_Ra - La_Ra;
}
```

```C#
// PPG Message
public class PpgData : DataMessage
{
    public double Ppg { get; set; }
}
```

The message types extends the baseclass for all messages.

```C#
public abstract class DataMessage
{
    //Timestamp for time at message generation (unix timestamp, milliseconds)
    public double Timestamp { get; set; }


    //Timestamp for original source (e.g for example heartrate data the timestamp will be the time at which the heartrate was calculated,
    //  and sourcetimestamp would be the timestamp for the original sensor data)
    public double SourceTimestamp { get; set; }


    public double SampleRate { get; set; }
}
```

### Send Data

The abstract class `DataStreamer` provides functionallity for sending data over gRPC.

```C#
// Send ECG data
private void _shimmer_NewEcgResponse(object sender, NewEcgResponseEventArgs e)
{
    WriteData(e.Ecg);
}
```

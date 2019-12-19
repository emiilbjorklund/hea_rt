using Basestation.Common.Abstractions;
using Basestation.Common.Data;
using Basestation.DataAcquisition.Shimmer;
using Basestation.DataAcquisition.TestData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basestation.DataAcquisition
{
    public class SensorManager : ISensorManager
    {
        public SensorManager(DataAcquisitionCapability capability)
        {
            foreach (var sensor in capability.Sensors)
            {
                switch (sensor.Type)
                {
                    case SensorType.EcgTestData:
                        Console.WriteLine($"Adding ecg sensor: {typeof(EcgTestData)}: {sensor.Id}");
                        EcgSensors.Add(new EcgTestData(sensor.Id));
                        break;
                    case SensorType.PpgTestData:
                        Console.WriteLine($"Adding ppg sensor: {typeof(PpgTestData)}: {sensor.Id}");
                        PpgSensors.Add(new PpgTestData(sensor.Id));
                        break;
                    case SensorType.ShimmerEcg:
                        Console.WriteLine($"Adding ecg sensor: {typeof(ShimmerEcg)}: {sensor.Id}");
                        EcgSensors.Add(new ShimmerEcg(sensor.Id));
                        break;
                    case SensorType.ShimmerPpg:
                        Console.WriteLine($"Adding ppg sensor: {typeof(ShimmerPpg)}: {sensor.Id}");
                        PpgSensors.Add(new ShimmerPpg(sensor.Id));
                        break;
                    default:
                        break;
                }
            }
        }

        public List<SensorStream<EcgData>> EcgSensors { get; } = new List<SensorStream<EcgData>>();

        public List<SensorStream<PpgData>> PpgSensors { get; } = new List<SensorStream<PpgData>>();
    }
}

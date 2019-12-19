using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace Basestation.Common.Abstractions
{
    public class DataAcquisitionCapability : Capability
    {
        public DataAcquisitionCapability()
        {
            Type = CapabilityType.DataAcquisition;
        }
        
        public List<Sensor> Sensors { get; private set; } = new List<Sensor>();


        public static DataAcquisitionCapability FromYaml(YamlMappingNode daNode)
        {
            var da = new DataAcquisitionCapability();

            var sensors = (YamlSequenceNode)daNode.Children[new YamlScalarNode("sensors")];
            foreach (var s in sensors.Children.Cast<YamlMappingNode>())
            {
                if (!Enum.TryParse<SensorType>(s.Children.First().Key.ToString(), true, out SensorType type))
                    throw new Exception($"Node type could not be parsed '{s.Children.First().Key.ToString()}' is not a valid sensor type");

                da.Sensors.Add(new Sensor(s.Children.First().Value.ToString(), type));
            }

            return da;
        }
    }
}
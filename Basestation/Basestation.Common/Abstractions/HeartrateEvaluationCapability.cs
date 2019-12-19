using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace Basestation.Common.Abstractions
{
    public class HeartrateEvaluationCapability : Capability
    {
        public HeartrateEvaluationCapability()
        {
            Type = CapabilityType.HeartrateEvaluation;
        }

        public List<string> EcgSources { get; } = new List<string>();
        public List<string> PpgSources { get; } = new List<string>();

        public static HeartrateEvaluationCapability FromYaml(YamlMappingNode hrNode)
        {
            var hr = new HeartrateEvaluationCapability();

            if (hrNode.Children.TryGetValue(new YamlScalarNode("ecg"), out YamlNode ecgsources))
                foreach (var s in ((YamlSequenceNode)ecgsources).Children.Cast<YamlScalarNode>())
                    hr.EcgSources.Add(s.ToString());



            var ppgsources = (YamlSequenceNode)hrNode.Children[new YamlScalarNode("ppg")];
            foreach (var s in ppgsources.Children.Cast<YamlScalarNode>())
                hr.PpgSources.Add(s.ToString());

            return hr;
        }

        public string GetAddress(string ecg)
        {
            var service = from serv in Service.System.Services
                          from DataAcquisitionCapability cap in serv.Capabilities.Where(c => c.Type == CapabilityType.DataAcquisition)
                          where cap.Sensors.Any(s => s.Id == ecg)
                          select serv;

            if (!service.Any())
                throw new Exception($"Could not locate the service hosting the sensor '{ecg}'");

            if (service.Count() > 2)
                throw new Exception($"Get address returned multiple services when looking for sensor '{ecg}'");

            //TODO use property in service class instead
            var address = $"http://{service.First().Hostname}:{service.First().Port}";

            return address;
        }
    }
}
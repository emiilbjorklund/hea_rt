using System;
using System.Collections.Generic;
using YamlDotNet.RepresentationModel;

namespace Basestation.Common.Abstractions
{
    public class Capability
    {
        public Capability()
        {

        }

        public CapabilityType Type { get; protected set; }

        public Service Service { get; set; }


        public static Capability FromYaml(KeyValuePair<YamlNode, YamlNode> capabilityNode, Service servRef)
        {
            var key = capabilityNode.Key;
            var val = (YamlMappingNode)capabilityNode.Value;

            if (!Enum.TryParse<CapabilityType>(key.ToString(), true, out CapabilityType type))
                throw new Exception($"Node type could not be parsed '{key.ToString()}' is not a valid node type");

            Capability cap = null;
            switch (type)
            {
                case CapabilityType.DataAcquisition:
                    cap = DataAcquisitionCapability.FromYaml(val);
                    break;
                case CapabilityType.HeartrateEvaluation:
                    cap = HeartrateEvaluationCapability.FromYaml(val);
                    break;
                case CapabilityType.MobileAppCommunication:
                    cap = MobileAppCommunicationCapability.FromYaml(val);
                    break;
                case CapabilityType.SystemHealthMonitor:
                    break;
                case CapabilityType.WarningsAndAlerts:
                    cap = WarningsAndAlertsCapability.FromYaml(val);
                    break;
                default:
                    throw new Exception($"Node could not be parsed to a capability {key.ToString()}");
            }

            if (cap != null)
                cap.Service = servRef;
            return cap;
        }
    }

    public enum CapabilityType
    {
        DataAcquisition,
        HeartrateEvaluation,
        MobileAppCommunication,
        SystemHealthMonitor,
        WarningsAndAlerts
    }
}
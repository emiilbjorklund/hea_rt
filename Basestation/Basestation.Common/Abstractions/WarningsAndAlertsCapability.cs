using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace Basestation.Common.Abstractions
{
    public class WarningsAndAlertsCapability : Capability
    {
        public WarningsAndAlertsCapability()
        {
            Type = CapabilityType.WarningsAndAlerts;
        }

        public static WarningsAndAlertsCapability FromYaml(YamlMappingNode wasNode)
        {
            var was = new WarningsAndAlertsCapability();
            // TODO: Any further implementation needed here???
            return was;
        }
    }
}

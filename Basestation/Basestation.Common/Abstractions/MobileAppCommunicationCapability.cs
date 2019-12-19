using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace Basestation.Common.Abstractions
{
    public class MobileAppCommunicationCapability : Capability
    {
        public MobileAppCommunicationCapability()
        {
            Type = CapabilityType.MobileAppCommunication;
        }

        //public List<Capability> Sources { get; private set; } = new List<Capability>();
        public List<Guid> HeartrateSourceIds { get; private set; } = new List<Guid>();
        public List<Guid> ArrythmiaReqIds { get; private set; } = new List<Guid>();

        public static MobileAppCommunicationCapability FromYaml(YamlMappingNode macNode)
        {
            var mac = new MobileAppCommunicationCapability();
            YamlSequenceNode sources = null;
            YamlSequenceNode reqtargets = null;

            if (macNode.Children.ContainsKey("datasources"))
            {
                sources = (YamlSequenceNode)macNode.Children[new YamlScalarNode("datasources")];
            }
            if (macNode.Children.ContainsKey("requesttargets"))
            {
                reqtargets = (YamlSequenceNode)macNode.Children[new YamlScalarNode("requesttargets")];
            }
                
            if (sources != null)
            {
                foreach (var srcType in sources.Children.Cast<YamlMappingNode>())
                {
                    var hrSrcs = (YamlSequenceNode)srcType.Children[new YamlScalarNode("heartrate")];
                    foreach (var id in hrSrcs.Children)
                    {
                        mac.HeartrateSourceIds.Add(Guid.Parse(id.ToString()));
                    }
                }
            }
            
            if (reqtargets != null)
            {
                foreach (var reqTrg in reqtargets.Children.Cast<YamlMappingNode>())
                {
                    if (reqTrg.Children.ContainsKey("arrythmiareq"))
                    {
                        var arytTarg = (YamlSequenceNode)reqTrg.Children[new YamlScalarNode("arrythmiareq")];
                        foreach (var id in arytTarg.Children)
                        {
                            mac.ArrythmiaReqIds.Add(Guid.Parse(id.ToString()));
                        }
                    }

                }
            }
            
            return mac;
        }

        public List<Capability> GetHeartrateSources()
        {
            List<Capability> srcList = new List<Capability>();
            foreach (var guid in HeartrateSourceIds)
            {
                var serv = Service.System.Services.FirstOrDefault(x => x.Id == guid);
                srcList.AddRange(serv.Capabilities);
            }
            return srcList;
        }

        public List<Capability> GetArrythmiaTargets()
        {
            List<Capability> trgList = new List<Capability>();
            foreach (var guid in ArrythmiaReqIds)
            {
                var serv = Service.System.Services.FirstOrDefault(x => x.Id == guid);
                trgList.AddRange(serv.Capabilities);
            }
            return trgList;
        }
    }
}
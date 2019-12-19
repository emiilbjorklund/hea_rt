using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace Basestation.Common.Abstractions
{
    public class SystemStructure
    {
        public SystemStructure(string ymlPath = "")
        {
            ParseYaml(ymlPath);
        }



        public List<Service> Services { get; } = new List<Service>();


        private void ParseYaml(string path)
        {
            using (var reader = new StreamReader(path))
            {
                var yaml = new YamlStream();
                yaml.Load(reader);

                var doc = yaml.Documents.FirstOrDefault();
                if (doc == null)
                    throw new Exception($"YmlParsingError: No document present in the config file '{Path.GetFullPath(path)}'");
                var root = (YamlMappingNode)doc.RootNode;
                if (root == null)
                    throw new Exception($"No root node present in the config file '{Path.GetFullPath(path)}'");

                var setup = (YamlMappingNode)root.Children[new YamlScalarNode("setup")];

                var services = (YamlMappingNode)setup.Children[new YamlScalarNode("services")];

                foreach (var s in services.Children.Values.Cast<YamlMappingNode>())
                    Services.Add(Service.FromYaml(s, this));
            }
        }

        public Capability GetCapability(Guid serviceId)
        {
            var service = Services.FirstOrDefault(s => s.Id == serviceId);
            if (service == null)
                throw new Exception($"The service {serviceId} could not be found in the structure");
            if (service.Capabilities.Count < 1 || service.Capabilities.Count > 1)
                throw new Exception($"Only one capability is currently allowed for a service, multiple were found in {serviceId}");
            return service.Capabilities.First();
        }

        /// <summary>
        /// Used to return multiple capabilities (not yet supported)
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        public List<Capability> GetCapabilities(Guid serviceId)
        {
            var service = Services.FirstOrDefault(s => s.Id == serviceId);
            return service.Capabilities;
        }
    }
}

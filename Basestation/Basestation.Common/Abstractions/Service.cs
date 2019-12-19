using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace Basestation.Common.Abstractions
{
    public class Service
    {
        public Service()
        {

        }

        public Service(Guid id, string hostname, int port, SystemStructure system)
        {
            Id = id;
            Hostname = hostname;
            Port = port;
            System = system;
        }

        public Guid Id { get; private set; }
        public string Hostname { get; private set; }
        public int Port { get; private set; }
        public string Address => $"http://{Hostname}:{Port}";

        public List<Capability> Capabilities { get; } = new List<Capability>();

        public SystemStructure System { get; set; }

        public static Service FromYaml(YamlMappingNode serviceNode, SystemStructure sysRef)
        {
            if (!serviceNode.Children.TryGetValue(new YamlScalarNode("id"), out YamlNode idNode))
                throw new Exception($"Id not specified in {serviceNode.NodeType.ToString()}");
            if (!Guid.TryParse(idNode.ToString(), out Guid idres))
                throw new Exception($"Could not parse id in {serviceNode.NodeType.ToString()}");
            var id = idres;


            if (!serviceNode.Children.TryGetValue(new YamlScalarNode("hostname"), out YamlNode hostNode))
                throw new Exception($"Hostname not specified in {serviceNode.NodeType.ToString()}");
            var hostname = hostNode.ToString();

            if (!serviceNode.Children.TryGetValue(new YamlScalarNode("port"), out YamlNode portNode))
                throw new Exception($"Port not specified in {serviceNode.NodeType.ToString()}");
            if (!int.TryParse(portNode.ToString(), out int portres))
                throw new Exception($"Could not parse port in {serviceNode.NodeType.ToString()}");
            var port = portres;

            var service = new Service(id, hostname, port, sysRef);

            var capNode = (YamlSequenceNode)serviceNode.Children[new YamlScalarNode("capabilities")];
            foreach (var c in capNode.Children.Cast<YamlMappingNode>())
                service.Capabilities.Add(Capability.FromYaml(c.Children.First(), service));

            return service;
        }
    }
}

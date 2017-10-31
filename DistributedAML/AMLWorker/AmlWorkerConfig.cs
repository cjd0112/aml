using System;
using System.Collections.Generic;
using System.Text;

namespace AMLWorker
{
    public class ServiceConfig
    {
        public ServiceConfig()
        {
            Properties = new Dictionary<string, object>();
        }
        public String Type { get; set; }
        public String Interface { get; set; }
        public int BucketStart { get; set; }
        public int BucketCount { get; set; }
        public Dictionary<String,Object> Properties { get; set; }

    }

    public class NodeConfig
    {
        public NodeConfig()
        {
            Services = new List<ServiceConfig>();
        }
        public String Name { get; set; }
        public List<ServiceConfig> Services { get; set; }
    }

    public class Nodes 
    {
        public Nodes()
        {
            NodeList = new List<NodeConfig>();
        }

        public List<NodeConfig> NodeList { get; set; }
    }

    public class MyNodes
    {
        public Nodes Nodes { get; set; }
    }

}

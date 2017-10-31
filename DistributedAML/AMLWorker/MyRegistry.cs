using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using AS.Logger;
using Comms;
using Logger;
using Microsoft.Extensions.Configuration;
using StructureMap;

namespace AMLWorker
{
    using System;

    namespace AS.Application
    {
        public class PubSubObj
        {
            public string hostname { get; set; }
            public int port { get; set; }

        }
        class PubSubServerConfig
        {
            public PubSubObj PubSub { get; set; }
        }
        public class MyRegistry : Registry
        {
            public MyRegistry(string nodeName)
            {
                Console.WriteLine($"Opening configuration file - appsettings.json with Node Name - {nodeName}");
                var builder = new ConfigurationBuilder()
                   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                var config = builder.Build();

                L.InitLogConsole(Console.Out, $"{config["ApplicationName"]}_{nodeName}");// config["TraceFilePath"], ls);

                L.Trace($"Opening log file");

                L.Trace("Initializing Dependencies");

                var nodes = config.Get<MyNodes>();

                var node = nodes.Nodes.NodeList.FirstOrDefault(x => x.Name == nodeName);
                if (node == null)
                    throw new Exception($"Could not find {nodeName} Node in Service List");

                For<NodeConfig>().Use(node);

                Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.SingleImplementationsOfInterface();
                    x.AddAllTypesOf<ICommsContract>();
                });

            }
        }
    }

}

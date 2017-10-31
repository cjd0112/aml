using System;
using AS.Logger;
using Comms;
using Logger;
using Microsoft.Extensions.Configuration;
using StructureMap;

namespace AmlClient
{
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
            public String DataDirectory { get; set; }
            public MyRegistry(string jsonFile)
            {
                Console.WriteLine($"Opening configuration file - {jsonFile}");
                var builder = new ConfigurationBuilder()
                   .AddJsonFile(jsonFile, optional: true, reloadOnChange: true);

                var config = builder.Build();

                LogSource ls;
                Enum.TryParse<LogSource>(config["ApplicationName"], out ls);

                L.InitLogConsole(Console.Out, config["ApplicationName"]);// config["TraceFilePath"], ls);

                L.Trace($"Opening log file");

                L.Trace("Initializing Dependencies");

                For<IServiceClient>().Add<ServiceClient>();

                For<IClientProxy>().Add<ClientFactory>();

                DataDirectory = config["DataDirectory"];

                Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.AddAllTypesOf<IServiceClient>();
                    x.Assembly("Comms");
                    x.SingleImplementationsOfInterface();
                    x.AddAllTypesOf<ICommsContract>();
                    x.AddAllTypesOf<IServiceClient>();
                });


            }
        }
    }

}

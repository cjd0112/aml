using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AS.Logger;
using Comms;
using Logger;
using Microsoft.Extensions.Configuration;
using StructureMap;

namespace AmlClient
{
    namespace AS.Application
    {
        public class MyRegistry : Registry
        {
            public String DataDirectory { get; set; }

            public List<Type> ClientTypes { get; set; }

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

                ClientTypes = new List<Type>();

                ClientTypes.AddRange(Assembly.GetAssembly(typeof(ICommsContract)).GetTypes().Where(x=>typeof(ICommsContract).IsAssignableFrom(x) && x.IsInterface && x != typeof(ICommsContract)));

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

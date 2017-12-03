using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using As.Logger;
using As.Comms;
using Google.Protobuf;
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

            public MyRegistry(string clientName)
            {
                if (String.IsNullOrEmpty(clientName))
                {
                    L.Trace("Setting client name to 'Default' as not passed on command line");
                    clientName = "Default";
                }
                L.Trace($"Opening configuration file - {Directory.GetCurrentDirectory()}/appsettings.json with client -{clientName} ");
                var builder = new ConfigurationBuilder()
                   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                var config = builder.Build();
                
                LogSource ls;
                Enum.TryParse<LogSource>(config["ApplicationName"], out ls);

                L.InitLogConsole(Console.Out, config["ApplicationName"]);// config["TraceFilePath"], ls);

                L.Trace($"Opening log file");

                L.Trace("Initializing Dependencies");

                For<IServiceClient>().Add<ServiceClient>();

                For<IClientProxy>().Add<ClientFactory>();

                var clients = config.Get<MyClients>();

                var client = clients.Clients.FirstOrDefault(x => x.Name.ToLower() == clientName.ToLower());
                if (client == null)
                    throw new Exception($"Could not find client config with name - {clientName}");


                DataDirectory = client.DataDirectory;

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

using System;
using System.Collections.Generic;
using System.Text;
using As.Client.AS.Application;
using As.Logger;
using As.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.Extensions.Options;
using StructureMap;

namespace As.Client
{
    public class InitializeClient
    {
        public static Container StartupAndRegisterClientServices(IConfiguration root,Container c)
        {
            try
            {
                
                var reg = new MyRegistry( root,Helper.GetPlatform().ToString());

                reg.For<MyRegistry>().Use(reg);
                c.Inject(typeof(Container), c);

                c.Configure(x=>
                {
                    x.AddRegistry(reg);
                 
                });


               
                
                var clientServicePartitionValidator= c.GetInstance<ClientServicePartitionValidator>();
                clientServicePartitionValidator.ValidateServicePartitions();

                c.Inject<ClientServicePartitionValidator>(clientServicePartitionValidator);

                return c;

            }
            catch (Exception e)
            {
                L.Trace(e.Message);
                if (e.InnerException != null)
                {
                    L.Trace($"Inner exception is {e.InnerException.Message}");
                    L.Trace($"{e.InnerException.StackTrace}");

                }
                L.Trace("Error on initialization ... quitting");
                throw e;
            }
        }
    }
}

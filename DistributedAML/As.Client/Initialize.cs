using System;
using System.Collections.Generic;
using System.Text;
using As.Client.AS.Application;
using As.Logger;
using As.Shared;
using StructureMap;

namespace As.Client
{
    public class Initialize
    {
        public static Container c;
        public static Container Startup(String clientName="")
        {
            try
            {
                var reg = new MyRegistry(clientName == ""? Helper.GetPlatform().ToString() : clientName);

                reg.For<MyRegistry>().Use(reg);
                c = new Container(reg);
                c.Inject(typeof(Container), c);

                var init = c.GetInstance<InitializeBuckets>();
                init.Run();

                c.Inject<InitializeBuckets>(init);

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

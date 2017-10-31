using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMLWorker.AS.Application;
using Logger;
using MajordomoProtocol;
using NetMQ;
using StructureMap;

namespace AMLWorker
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Container c = null;
                if (args.Length == 0)
                    throw new Exception("Invalid argument - expecting name of node from appsettings.json, e.g., 'Apple'");
                var reg = new MyRegistry(args[0]);
                c = new Container(reg);
                reg.For<IContainer>().Use(c);



                c.GetInstance<Node>().Run();

                /*

                var workers = c.GetInstance<WorkerActors>();

                for (byte i = 0; i < workers.NumNodes; i++)
                {                   
                    var z = new MyWorkerActor($"{workers.Service}_{i}",i,c.GetInstance<IWorker>());
                    z.Run();
                }
                */

                Console.ReadLine();
                L.CloseLog();


            }
            catch (Exception e)
            {
                L.Exception(e);
                Console.WriteLine(e.Message);
                Console.ReadLine();

            }
        }

    }
}

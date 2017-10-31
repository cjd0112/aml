using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using Comms;
using StructureMap;

namespace AMLWorker
{
    public class ServiceType
    {
        private ServiceConfig config;
        public List<Service> underlying = new List<Service>();

        public String Type => config.Type;

        private IContainer container;
        public ServiceType(IContainer container,ServiceConfig config)
        {
            this.config = config;
            this.container = container;

        }

        public String GetServiceName(int bucket)
        {
            return $"{config.Interface}_{bucket}";
        }

        public void Run()
        {
            for (int i = config.BucketStart; i < config.BucketStart + config.BucketCount; i++)
            {
                var q = new Service(this,config,i);
                underlying.Add(q as Service);

                // create logic component
                var logicType = System.Type.GetType(config.Type);
                if (logicType == null)
                    throw new Exception($"Could not create component of type - {config.Type} - are you using fully qualified name?");
                var logic = container.With(typeof(IServiceServer), q).GetInstance(logicType);

                underlying.Last().Run();

            }
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using Shared;
using StructureMap;

namespace AMLWorker
{
    public class Node
    {
        private NodeConfig config;
        private List<ServiceType> Services = new List<ServiceType>();
        private IContainer container;
        public Node(IContainer container,NodeConfig config)
        {
            this.config = config;
            this.container = container;
        }

        public void Run()
        {
            Services.AddRange(config.Services.Select(x => container.With(typeof(ServiceConfig), x)
                .GetInstance<ServiceType>()));
            Services.Do(x=>x.Run());
        }
    }
}
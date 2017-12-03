using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using As.Comms;
using MDPCommons;
using NetMQ;

namespace AMLWorker
{
    public class Service : IServiceServer
    {
        private ServiceType parent;
        private int bucketId;
        private ServiceConfig serviceConfig;
        public Service(ServiceType parent, ServiceConfig serviceConfig, int bucketId)
        {
            this.parent = parent;
            this.bucketId = bucketId;
            this.serviceConfig = serviceConfig;
        }

        public void Run()
        {
            var t = new Task(() =>
            {

                var g = new MDPWorker("tcp://localhost:5555", parent.GetServiceName(bucketId),
                    Encoding.ASCII.GetBytes(parent.GetServiceName(bucketId)));

                // logging info to be displayed on screen
                g.LogInfoReady += (s, e) => Console.WriteLine($"{e.Info}");

                // there is no initial reply
                NetMQMessage reply = null;

                bool exit = false;
                while (!exit)
                {
                    // send the reply and wait for a request
                    var request = g.Receive(reply);

                    Console.WriteLine($"Received a request");

                    // was the worker interrupted
                    if (ReferenceEquals(request, null))
                        break;
                    // echo the request
                    if (OnReceived != null)
                        reply = OnReceived(request);
                    else
                        throw new Exception("OnReceived handler is null - is handler set?");
                }
            });
            t.Start();
        }

        public object GetConfigProperty(string key)
        {
            if (serviceConfig.Properties.ContainsKey(key) == false)
                throw new Exception($"Service Config does not contain key - {key}");
            return serviceConfig.Properties[key];
        }

        public object GetConfigProperty(string key,int bucket)
        {
            var key2 = key + "_" + bucket;
            if (serviceConfig.Properties.ContainsKey(key2) == false)
                throw new Exception($"Service Config does not contain key - {key2}");
            return serviceConfig.Properties[key2];
        }


        public int BucketId => bucketId;

        public event Func<NetMQMessage, NetMQMessage> OnReceived;
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Comms;
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

                var g = new MajordomoProtocol.MDPWorker("tcp://localhost:5555", parent.GetServiceName(bucketId),
                                      new byte[] { (byte)'W', (byte)(bucketId+'A') });

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

        public int BucketId => bucketId;

        public event Func<NetMQMessage, NetMQMessage> OnReceived;
    }
}

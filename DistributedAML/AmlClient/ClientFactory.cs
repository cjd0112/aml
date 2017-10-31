using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Comms;
using Logger;
using MajordomoProtocol;
using NetMQ;
using StructureMap;

namespace AmlClient
{
    public class ClientFactory : IClientProxy
    {
        private MDPClientAsync mainClient;

        Dictionary<string,ServiceClient> serviceQueue = new Dictionary<string, ServiceClient>();

        ConcurrentQueue<Tuple<String,NetMQMessage>> myQueue = new ConcurrentQueue<Tuple<String,NetMQMessage>>();

        private Container container;
        private AutoResetEvent initialEvent = new AutoResetEvent(false);
        public ClientFactory(Container c)
        {
            mainClient = new MDPClientAsync("tcp://localhost:5555", new byte[] { (byte)'C', (byte)'1' });
            mainClient.ReplyReady += Z_ReplyReady;
            mainClient.LogInfoReady += Z_LogInfoReady;

            var pp = new NetMQMessage();
            pp.Append("list");

            // get a list of the services that we support
            mainClient.Send("mmi.service", pp);
            this.container = c;

            initialEvent.WaitOne();


        }

        public T GetClient<T>(int bucket) where T:ICommsContract
        {
            var str = typeof(T).Name + "_" + bucket;
            if (serviceQueue.ContainsKey(str) == false)
                throw new Exception($"Service with name - {str} not found");

            var serviceClient = serviceQueue[str];

            if (serviceClient.Underlying == null)
                throw new Exception($"Underlying for service - {str} is null ...");

            return (T) serviceClient.Underlying;
        }

        public IEnumerable<int> GetClientBuckets<T>() where T : ICommsContract
        {
            foreach (var c in serviceQueue.Keys)
            {
                if (c.StartsWith(typeof(T).Name))
                    yield return serviceQueue[c].Bucket;
            }
        }

        private void Z_LogInfoReady(object sender, MDPCommons.MDPLogEventArgs e)
        {
            Console.WriteLine(e.Info);
        }

        private void Z_ReplyReady(object sender, MDPCommons.MDPReplyEventArgs e)
        {
            var msg = e.Reply.Pop();
            var serviceName = msg.ConvertToString();
            Console.WriteLine("service name is: " + msg.ConvertToString());
            if (serviceName == "mmi.service")
            {
                CreateServiceClients(e.Reply.Pop().ConvertToString().Split(new char[] { ',' }));
                Run();
            }
            else
            {
                if (serviceQueue.ContainsKey(serviceName) == false)
                    throw new Exception($"Service response found with unexpected name - {serviceName}");
                serviceQueue[serviceName].OnResponse(e.Reply);

                Console.WriteLine("message is: " + msg.ConvertToString());
            }
        }

        void CreateServiceClients(IEnumerable<String> serviceNames)
        {
            foreach (var c in serviceNames)
            {
                try
                {
                    L.Trace($"Loading service- {c}");

                    if (c.IndexOf("_") == -1)
                        throw new Exception("Expected '_' in service name");

                    var interfaceName = c.Substring(0, c.IndexOf("_"));

                    var bucket = Convert.ToInt32(c.Substring(c.IndexOf("_")+1));

                    L.Trace($"Extracted interface name - {interfaceName}");

                    Type interfaceType = null;
                    foreach (var g in Assembly.GetAssembly(typeof(ICommsContract)).GetExportedTypes())
                    {
                        if (g.IsInterface && typeof(ICommsContract).IsAssignableFrom(g) && g.Name == interfaceName)
                        {
                            interfaceType = g;
                        }
                    }

                    if (interfaceType == null)
                        throw new Exception(
                            $"Couldn't find interface type inheriting from ICommsContract in Comms assembly with name - {interfaceName}");

                    var commsAssembly = Assembly.GetAssembly(typeof(ICommsContract));

                    var str = $"Comms.{interfaceName.Substring(1)}Client,{commsAssembly.FullName}";

                    L.Trace($"Trying to create client type - {str}");

                    var clientType = System.Type.GetType(str);

                    if (clientType == null)
                        throw new Exception($"Couldn't create client type with name - {str}");

                    var serviceClient = container.
                        With(typeof(string), c).
                        With(typeof(int),bucket).
                        With(typeof(IClientProxy), this).GetInstance<ServiceClient>();

                    if (serviceClient == null)
                        throw new Exception($"Couldn't create service client");

                    serviceQueue[c] = serviceClient;

                    // hook up the underlying

                    var clientObj = container.With(typeof(IServiceClient), serviceQueue[c]).GetInstance(clientType);

                    if (clientObj == null)
                        throw new Exception($"Couldn't create client obj");

                    L.Trace($"Created client obj with type - {clientType}");


                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception loading our client service");
                    Console.WriteLine(e);
                    Console.WriteLine("Continuing with other service initiation ...");
                }
            }

            
        }

        private bool finished = false;

        void Run()
        {
            var t = new Task(() =>
            {

                while (!finished)
                {
                    Tuple<string, NetMQMessage> msg = null;
                    if (myQueue.TryDequeue(out msg))
                    {
                        mainClient.Send(msg.Item1, msg.Item2);
                    }
                }
            });
            t.Start();

            initialEvent.Set();

        }


        public void SendMessage(String serviceName,NetMQMessage msg)
        {
            if (serviceQueue.ContainsKey(serviceName) == false)
            {
                throw new Exception($"cannot find serviceQueue for {serviceName}");
            }
            myQueue.Enqueue(new Tuple<string,NetMQMessage>(serviceName,msg));
        }
    }
}

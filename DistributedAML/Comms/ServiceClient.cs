using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Google.Protobuf;
using Logger;
using NetMQ;
using Shared;

namespace Comms
{
    public interface IClientProxy
    {
        void SendMessage(String serviceName, NetMQMessage msg);
    }
    public class ServiceClient : IServiceClient
    {
        private IClientProxy proxy;
        private string serviceName;
        private ConcurrentQueue<NetMQMessage> myQueue = new ConcurrentQueue<NetMQMessage>();

        public ICommsContract Underlying;

        public int Bucket { get; set; }

        public ServiceClient(int bucket,string serviceName,IClientProxy proxy)
        {
            this.proxy = proxy;
            this.serviceName = serviceName;
            this.Bucket = bucket;
        }

        public void SetUnderlying(ICommsContract contract)
        {
            Underlying = contract;
        }

        public void OnResponse(NetMQMessage msg)
        {
            myQueue.Enqueue(msg);
        }

        private NetMQMessage Send(NetMQMessage request)
        {
            L.Trace($"sending from bucket - {Bucket} thread - {Thread.CurrentThread.ManagedThreadId}");
            proxy.SendMessage(serviceName, request);
            NetMQMessage msg = null;
            while (msg == null)
            {
                myQueue.TryDequeue(out msg);
            }
            return msg;
        }

        public Int32 SendEnumerableIntResult<T>(string function, IEnumerable<T> lst, params Object[] param) where T:IMessage
        {
            int t = 0;
            foreach (var c in lst.Chunk(100000))
            {
                var msg = new NetMQMessage();
                msg.Append(function);
               
                msg.PackMessageList<T>(c);
                foreach (var z in param)
                {
                    msg.AddParameter(z);
                }
                var ret = Send(msg);
                if (ret.First.IsEmpty) throw new Exception(ret[1].ConvertToString());
                t += ret.First.ConvertToInt32();
            }
            return t;
        }

        public IEnumerable<Y> SendEnumerableListResult<T, Y>(string function, Func<Stream, Y> transform, IEnumerable<T> lst,
            params object[] param) where T : IMessage where Y : IMessage
        {
            List<Y> ret = new List<Y>();  // be careful changing this to 'yield return' because causes threading issues ... 
            L.Trace($"SendEnumerable - {Thread.CurrentThread.ManagedThreadId}");
            foreach (var c in lst.Chunk(100000))
            {
                var msg = new NetMQMessage();
                msg.Append(function);

                msg.PackMessageList<T>(c);
                foreach (var z in param)
                {
                    msg.AddParameter(z);
                }
                L.Trace($"About to actually send- {Thread.CurrentThread.ManagedThreadId}");
                var result = Send(msg);
                if (result.First.IsEmpty) throw new Exception(result[1].ConvertToString());
                foreach (var obj in result.UnpackMessageList(transform))
                    ret.Add(obj);
            }
            return ret;
        }

        public Y Send<T, Y>(string function, Func<Stream,Y> transform, T data, params object[] param) where T : IMessage where Y : IMessage
        {
            var msg = new NetMQMessage();
            msg.Append(function);
            msg.PackMessage(data);
            var result = Send(msg);
            if (result.First.IsEmpty) throw new Exception(result[1].ConvertToString());
            return result.UnpackMessage<Y>(transform);
        }
        public Y Send<Y>(string function, Func<Stream, Y> transform, params object[] param) where Y : IMessage
        {
            var msg = new NetMQMessage();
            msg.Append(function);
            var result = Send(msg);
            if (result.First.IsEmpty) throw new Exception(result[1].ConvertToString());
            return result.UnpackMessage<Y>(transform);
        }

    }
}

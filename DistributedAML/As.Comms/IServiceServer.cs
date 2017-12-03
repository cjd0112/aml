using System;
using NetMQ;

namespace As.Comms
{
    public interface IServiceServer
    {
        Object GetConfigProperty(string key);
        Object GetConfigProperty(string key, int bucketId);
        int BucketId { get; }
        event Func<NetMQMessage, NetMQMessage> OnReceived;
    }
}

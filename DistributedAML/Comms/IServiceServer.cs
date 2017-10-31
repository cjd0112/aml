using System;
using System.Collections.Generic;
using System.Text;
using NetMQ;

namespace Comms
{
    public interface IServiceServer
    {
        Object GetConfigProperty(string key);
        int BucketId { get; }
        event Func<NetMQMessage, NetMQMessage> OnReceived;
    }
}

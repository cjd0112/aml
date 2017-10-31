using System;
using System.Collections.Generic;
using System.Text;
using NetMQ;

namespace AmlClient
{
    public interface IClientProxy
    {
        void SendMessage(String serviceName, NetMQMessage msg);
    }
}

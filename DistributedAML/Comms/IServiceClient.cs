using System;
using System.Collections.Generic;
using System.Text;
using NetMQ;

namespace Comms
{
    public interface IServiceClient
    {
        void SetUnderlying(ICommsContract contract);
        NetMQMessage Send(NetMQMessage request);

    }
}

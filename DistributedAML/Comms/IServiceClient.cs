using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Google.Protobuf;
using NetMQ;

namespace Comms
{
    public interface IServiceClient
    {
        void SetUnderlying(ICommsContract contract);

        Int32 SendEnumerableIntResult<T>(string function, IEnumerable<T> lst, params Object[] param1)
            where T : IMessage;

        IEnumerable<Y> SendEnumerableListResult<T, Y>(string function, Func<Stream, Y> transform, IEnumerable<T> lst,
            params Object[] param) where T : IMessage where Y : IMessage;
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using As.Comms;

namespace As.Client
{
    public interface IClientFactory
    {
        T GetClient<T>(int bucket) where T : ICommsContract;

        IEnumerable<int> GetClientBuckets<T>() where T : ICommsContract;
    }
}

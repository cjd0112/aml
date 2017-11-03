using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf.Reflection;

namespace Comms
{
    public interface IRetailStore : ICommsContract
    {
        int StoreRetail(List<Retail> retails);
    }
}

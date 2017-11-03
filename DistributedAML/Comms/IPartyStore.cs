using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf.Reflection;

namespace Comms
{
    public interface IPartyStore : ICommsContract
    {
        int StoreParties(List<Party> parties);


    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf.Reflection;

namespace Comms
{
    public interface ITransactionStore : ICommsContract
    {
        int StoreTransactions(List<Transaction> transactions);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Comms
{
    public interface ITransactionMapper : ICommsContract
    {
        bool StoreTransactions(List<Transaction> transactions);
    }
}

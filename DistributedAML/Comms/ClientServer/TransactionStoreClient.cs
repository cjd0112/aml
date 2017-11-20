   
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NetMQ;
using Shared;

namespace Comms.ClientServer
{
    public class TransactionStoreClient : ITransactionStore
    {
        protected IServiceClient client;
        public TransactionStoreClient(IServiceClient client)
        {
            this.client = client;
            this.client.SetUnderlying(this);
        }

        

		public Int32 StoreTransactions(IEnumerable<Transaction> transactions)
		{
			return client.SendEnumerableIntResult<Transaction>("StoreTransactions",transactions);
		}

    }
}
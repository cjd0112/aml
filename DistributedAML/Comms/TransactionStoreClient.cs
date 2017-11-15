   
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NetMQ;
using Shared;

namespace Comms
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
			var msg = new NetMQMessage();
			msg.Append("StoreTransactions");
			Helpers.PackMessageList<Transaction>(msg,transactions);
			var ret = client.Send(msg);
			if (ret.First.IsEmpty) throw new Exception(ret[1].ConvertToString());
			return ret.First.ConvertToInt32();
		}
    }
}
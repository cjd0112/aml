   
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NetMQ;
using Logger;
using Shared;

namespace Comms.ClientServer
{
    public abstract class TransactionStoreServer : ITransactionStore
    {
        protected IServiceServer server;
        protected TransactionStoreServer(IServiceServer server)
        {
            this.server= server;
            this.server.OnReceived += OnReceived;
        }

        private NetMQMessage OnReceived(NetMQMessage request)
        {
            var ret = new NetMQMessage();
            var selector = request.Pop();

            try
            {
                switch (selector.ConvertToString())
                {
               case "StoreTransactions":
                {
                    
                        var transactions = Helpers.UnpackMessageList<Transaction>(request,Transaction.Parser.ParseDelimitedFrom);
					
                    var methodResult=StoreTransactions(transactions);
                    ret.Append(methodResult);
                    break;
                }
                    default:
                        throw new Exception($"Unexpected selector - {selector}");
                }
            }
            catch (Exception e)
            {
                L.Trace($"{selector} caused an exception");
                L.Exception(e);
                ret.AppendEmptyFrame();
                ret.Append($"{selector} caused an exception - '{e.Message}' check server logs for more details");
            }
            return ret;
        }

        
		public abstract Int32 StoreTransactions(IEnumerable<Transaction> transactions);

    }
}
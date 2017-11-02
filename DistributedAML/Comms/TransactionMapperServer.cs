   
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NetMQ;
using Shared;

namespace Comms
{
    public abstract class TransactionMapperServer : ITransactionMapper
    {
        protected IServiceServer server;
        protected TransactionMapperServer(IServiceServer server)
        {
            this.server= server;
            this.server.OnReceived += OnReceived;
        }

        private NetMQMessage OnReceived(NetMQMessage request)
        {
            var ret = new NetMQMessage();
            var selector = request.Pop();
            switch (selector.ConvertToString())
            {
               case "StoreTransactions":
                {
                    
                        var transactions = Helpers.UnpackMessageList<Transaction>(request,Transaction.Parser.ParseDelimitedFrom);					
                    var methodResult=StoreTransactions(transactions);
                    ret.Append(Convert.ToInt32(methodResult));
                    break;
                }
                default:
                    throw new Exception($"Unexpected selector - {selector}");
            }
            return ret;
        }

        
		public abstract Boolean StoreTransactions(List<Transaction> transactions);

    }
}
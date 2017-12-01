   
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NetMQ;
using Logger;
using Shared;

namespace Comms.ClientServer
{
    public abstract class AmlRepositoryServer : IAmlRepository
    {
        protected IServiceServer server;
        protected AmlRepositoryServer(IServiceServer server)
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
               case "StoreParties":
                {
                    
                        var parties = request.UnpackMessageList<Party>(Party.Parser.ParseDelimitedFrom);
					
                    var methodResult=StoreParties(parties);
                    ret.Append(methodResult);
                    break;
                }
               case "StoreAccounts":
                {
                    
                        var accounts = request.UnpackMessageList<Account>(Account.Parser.ParseDelimitedFrom);
					
                    var methodResult=StoreAccounts(accounts);
                    ret.Append(methodResult);
                    break;
                }
               case "StoreTransactions":
                {
                    
                        var transactions = request.UnpackMessageList<Transaction>(Transaction.Parser.ParseDelimitedFrom);
					
                    var methodResult=StoreTransactions(transactions);
                    ret.Append(methodResult);
                    break;
                }
               case "StoreLinkages":
                {
                    
                        var mappings = request.UnpackMessageList<AccountToParty>(AccountToParty.Parser.ParseDelimitedFrom);
					var direction = (LinkageDirection)Enum.Parse(typeof(LinkageDirection),request.Pop().ConvertToString());
					
                    var methodResult=StoreLinkages(mappings,direction);
                    ret.Append(methodResult);
                    break;
                }
               case "GetLinkages":
                {
                    
                        var source = request.UnpackMessageList<Identifier>(Identifier.Parser.ParseDelimitedFrom);
					var direction = (LinkageDirection)Enum.Parse(typeof(LinkageDirection),request.Pop().ConvertToString());
					
                    var methodResult=GetLinkages(source,direction);
                    ret.PackMessageList<AccountToParty>(methodResult);;
                    break;
                }
               case "AccountsExist":
                {
                    
                        var account = request.UnpackMessageList<Identifier>(Identifier.Parser.ParseDelimitedFrom);
					
                    var methodResult=AccountsExist(account);
                    ret.PackMessageList<YesNo>(methodResult);;
                    break;
                }
               case "RunQuery":
                {
                    var query = request.UnpackMessage<GraphQuery>(GraphQuery.Parser.ParseDelimitedFrom);					
                    var methodResult=RunQuery(query);
                    ret.PackMessage<GraphResponse>(methodResult);;
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

        
		public abstract Int32 StoreParties(IEnumerable<Party> parties);

		public abstract Int32 StoreAccounts(IEnumerable<Account> accounts);

		public abstract Int32 StoreTransactions(IEnumerable<Transaction> transactions);

		public abstract Int32 StoreLinkages(IEnumerable<AccountToParty> mappings,LinkageDirection direction);

		public abstract IEnumerable<AccountToParty> GetLinkages(IEnumerable<Identifier> source,LinkageDirection direction);

		public abstract IEnumerable<YesNo> AccountsExist(IEnumerable<Identifier> account);

		public abstract GraphResponse RunQuery(GraphQuery query);

    }
}
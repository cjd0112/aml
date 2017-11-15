   
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NetMQ;
using Logger;
using Shared;

namespace Comms
{
    public abstract class PartyStoreServer : IPartyStore
    {
        protected IServiceServer server;
        protected PartyStoreServer(IServiceServer server)
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
                    
                        var parties = Helpers.UnpackMessageList<Party>(request,Party.Parser.ParseDelimitedFrom);
					
                    var methodResult=StoreParties(parties);
                    ret.Append(methodResult);
                    break;
                }
               case "StoreAccounts":
                {
                    
                        var accounts = Helpers.UnpackMessageList<Account>(request,Account.Parser.ParseDelimitedFrom);
					
                    var methodResult=StoreAccounts(accounts);
                    ret.Append(methodResult);
                    break;
                }
               case "StoreLinkages":
                {
                    
                        var mappings = Helpers.UnpackMessageList<AccountToParty>(request,AccountToParty.Parser.ParseDelimitedFrom);
					var direction = (LinkageDirection)Enum.Parse(typeof(LinkageDirection),request.Pop().ConvertToString());
					
                    var methodResult=StoreLinkages(mappings,direction);
                    ret.Append(methodResult);
                    break;
                }
               case "GetLinkages":
                {
                                    
                    var source = Helpers.UnpackMessageListString(request);
					var direction = (LinkageDirection)Enum.Parse(typeof(LinkageDirection),request.Pop().ConvertToString());
					
                    var methodResult=GetLinkages(source,direction);
                    Helpers.PackMessageList<AccountToParty>(ret,methodResult);;
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

		public abstract Int32 StoreLinkages(IEnumerable<AccountToParty> mappings,LinkageDirection direction);

		public abstract IEnumerable<AccountToParty> GetLinkages(IEnumerable<String> source,LinkageDirection direction);

    }
}
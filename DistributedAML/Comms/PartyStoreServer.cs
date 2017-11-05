   
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NetMQ;
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
            switch (selector.ConvertToString())
            {
               case "StoreParties":
                {
                    
                        var parties = Helpers.UnpackMessageList<Party>(request,Party.Parser.ParseDelimitedFrom);					
                    var methodResult=StoreParties(parties);
                    ret.Append(methodResult);
                    break;
                }
                default:
                    throw new Exception($"Unexpected selector - {selector}");
            }
            return ret;
        }

        
		public abstract Int32 StoreParties(List<Party> parties);

    }
}
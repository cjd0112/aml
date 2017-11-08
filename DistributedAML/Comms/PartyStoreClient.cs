   
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NetMQ;
using Shared;

namespace Comms
{
    public class PartyStoreClient : IPartyStore
    {
        protected IServiceClient client;
        public PartyStoreClient(IServiceClient client)
        {
            this.client = client;
            this.client.SetUnderlying(this);
        }

        

		public Int32 StoreParties(List<Party> parties)
		{
			var msg = new NetMQMessage();
			msg.Append("StoreParties");
			Helpers.PackMessageList<Party>(msg,parties);
			var ret = client.Send(msg);
			return ret.First.ConvertToInt32();
		}

		public Int32 StoreAccounts(List<Account> accounts)
		{
			var msg = new NetMQMessage();
			msg.Append("StoreAccounts");
			Helpers.PackMessageList<Account>(msg,accounts);
			var ret = client.Send(msg);
			return ret.First.ConvertToInt32();
		}

		public Int32 StoreAccountToPartyMapping(List<AccountToPartyMapping> mappings)
		{
			var msg = new NetMQMessage();
			msg.Append("StoreAccountToPartyMapping");
			Helpers.PackMessageList<AccountToPartyMapping>(msg,mappings);
			var ret = client.Send(msg);
			return ret.First.ConvertToInt32();
		}
    }
}
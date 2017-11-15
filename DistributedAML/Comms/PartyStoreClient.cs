   
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

        

		public Int32 StoreParties(IEnumerable<Party> parties)
		{
			var msg = new NetMQMessage();
			msg.Append("StoreParties");
			Helpers.PackMessageList<Party>(msg,parties);
			var ret = client.Send(msg);
			if (ret.First.IsEmpty) throw new Exception(ret[1].ConvertToString());
			return ret.First.ConvertToInt32();
		}

		public Int32 StoreAccounts(IEnumerable<Account> accounts)
		{
			var msg = new NetMQMessage();
			msg.Append("StoreAccounts");
			Helpers.PackMessageList<Account>(msg,accounts);
			var ret = client.Send(msg);
			if (ret.First.IsEmpty) throw new Exception(ret[1].ConvertToString());
			return ret.First.ConvertToInt32();
		}

		public Int32 StoreLinkages(IEnumerable<AccountToParty> mappings,LinkageDirection direction)
		{
			var msg = new NetMQMessage();
			msg.Append("StoreLinkages");
			Helpers.PackMessageList<AccountToParty>(msg,mappings);
			msg.Append(direction.ToString());
			var ret = client.Send(msg);
			if (ret.First.IsEmpty) throw new Exception(ret[1].ConvertToString());
			return ret.First.ConvertToInt32();
		}

		public IEnumerable<AccountToParty> GetLinkages(IEnumerable<String> source,LinkageDirection direction)
		{
			var msg = new NetMQMessage();
			msg.Append("GetLinkages");
			Helpers.PackMessageListString(msg,source);
			msg.Append(direction.ToString());
			var ret = client.Send(msg);
			if (ret.First.IsEmpty) throw new Exception(ret[1].ConvertToString());
			return Helpers.UnpackMessageList(ret, AccountToParty.Parser.ParseDelimitedFrom);;
		}
    }
}
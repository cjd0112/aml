   
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
    }
}
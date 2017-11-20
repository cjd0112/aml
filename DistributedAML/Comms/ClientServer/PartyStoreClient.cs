   
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NetMQ;
using Shared;

namespace Comms.ClientServer
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
			return client.SendEnumerableIntResult<Party>("StoreParties",parties);
		}


		public Int32 StoreAccounts(IEnumerable<Account> accounts)
		{
			return client.SendEnumerableIntResult<Account>("StoreAccounts",accounts);
		}


		public Int32 StoreLinkages(IEnumerable<AccountToParty> mappings,LinkageDirection direction)
		{
			return client.SendEnumerableIntResult<AccountToParty>("StoreLinkages",mappings,direction);
		}


		public IEnumerable<AccountToParty> GetLinkages(IEnumerable<Identifier> source,LinkageDirection direction)
		{
			return client.SendEnumerableListResult<Identifier,AccountToParty>("GetLinkages",AccountToParty.Parser.ParseDelimitedFrom,source,direction);
		}

    }
}
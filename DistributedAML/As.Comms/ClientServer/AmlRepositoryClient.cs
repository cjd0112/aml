   
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NetMQ;
using As.Shared;

namespace As.Comms.ClientServer
{
    public class AmlRepositoryClient : IAmlRepository
    {
        protected IServiceClient client;
        public AmlRepositoryClient(IServiceClient client)
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


		public Int32 StoreTransactions(IEnumerable<Transaction> transactions)
		{
			return client.SendEnumerableIntResult<Transaction>("StoreTransactions",transactions);
		}


		public Int32 StoreLinkages(IEnumerable<AccountToParty> mappings,LinkageDirection direction)
		{
			return client.SendEnumerableIntResult<AccountToParty>("StoreLinkages",mappings,direction);
		}


		public IEnumerable<AccountToParty> GetLinkages(IEnumerable<Identifier> source,LinkageDirection direction)
		{
			return client.SendEnumerableListResult<Identifier,AccountToParty>("GetLinkages",AccountToParty.Parser.ParseDelimitedFrom,source,direction);
		}


		public IEnumerable<YesNo> AccountsExist(IEnumerable<Identifier> account)
		{
			return client.SendEnumerableListResult<Identifier,YesNo>("AccountsExist",YesNo.Parser.ParseDelimitedFrom,account);
		}


		public GraphResponse RunQuery(GraphQuery query)
		{
			return client.Send<GraphQuery,GraphResponse>("RunQuery",GraphResponse.Parser.ParseDelimitedFrom,query);
		}

    }
}
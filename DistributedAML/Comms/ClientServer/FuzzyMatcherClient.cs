   
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NetMQ;
using Shared;

namespace Comms.ClientServer
{
    public class FuzzyMatcherClient : IFuzzyMatcher
    {
        protected IServiceClient client;
        public FuzzyMatcherClient(IServiceClient client)
        {
            this.client = client;
            this.client.SetUnderlying(this);
        }

        

		public Int32 AddEntry(IEnumerable<FuzzyWordEntry> entries)
		{
			return client.SendEnumerableIntResult<FuzzyWordEntry>("AddEntry",entries);
		}


		public IEnumerable<FuzzyQueryResponse> FuzzyQuery(IEnumerable<FuzzyCheck> phrases)
		{
			return client.SendEnumerableListResult<FuzzyCheck,FuzzyQueryResponse>("FuzzyQuery",FuzzyQueryResponse.Parser.ParseDelimitedFrom,phrases);
		}

    }
}
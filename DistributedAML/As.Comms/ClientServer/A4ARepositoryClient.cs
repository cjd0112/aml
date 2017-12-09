   
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NetMQ;
using As.Shared;

namespace As.Comms.ClientServer
{
    public class A4ARepositoryClient : IA4ARepository
    {
        protected IServiceClient client;
        public A4ARepositoryClient(IServiceClient client)
        {
            this.client = client;
            this.client.SetUnderlying(this);
        }

        

		public GraphResponse RunQuery(GraphQuery query)
		{
			return client.Send<GraphQuery,GraphResponse>("RunQuery",GraphResponse.Parser.ParseFrom,query);
		}

    }
}
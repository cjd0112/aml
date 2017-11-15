   
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NetMQ;
using Shared;

namespace Comms
{
    public class FuzzyMatcherClient : IFuzzyMatcher
    {
        protected IServiceClient client;
        public FuzzyMatcherClient(IServiceClient client)
        {
            this.client = client;
            this.client.SetUnderlying(this);
        }

        

		public Boolean AddEntry(IEnumerable<FuzzyWordEntry> entries)
		{
			var msg = new NetMQMessage();
			msg.Append("AddEntry");
			Helpers.PackMessageList<FuzzyWordEntry>(msg,entries);
			var ret = client.Send(msg);
			if (ret.First.IsEmpty) throw new Exception(ret[1].ConvertToString());
			return ret.First.ConvertToInt32() >0 ? true:false;
		}

		public IEnumerable<FuzzyQueryResponse> FuzzyQuery(IEnumerable<String> phrases)
		{
			var msg = new NetMQMessage();
			msg.Append("FuzzyQuery");
			Helpers.PackMessageListString(msg,phrases);
			var ret = client.Send(msg);
			if (ret.First.IsEmpty) throw new Exception(ret[1].ConvertToString());
			return Helpers.UnpackMessageList(ret, FuzzyQueryResponse.Parser.ParseDelimitedFrom);;
		}
    }
}
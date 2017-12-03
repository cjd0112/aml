   
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NetMQ;
using As.Logger;
using As.Shared;

namespace As.Comms.ClientServer
{
    public abstract class FuzzyMatcherServer : IFuzzyMatcher
    {
        protected IServiceServer server;
        protected FuzzyMatcherServer(IServiceServer server)
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
               case "AddEntry":
                {
                    
                        var entries = request.UnpackMessageList<FuzzyWordEntry>(FuzzyWordEntry.Parser.ParseDelimitedFrom);
					
                    var methodResult=AddEntry(entries);
                    ret.Append(methodResult);
                    break;
                }
               case "FuzzyQuery":
                {
                    
                        var phrases = request.UnpackMessageList<FuzzyCheck>(FuzzyCheck.Parser.ParseDelimitedFrom);
					
                    var methodResult=FuzzyQuery(phrases);
                    ret.PackMessageList<FuzzyQueryResponse>(methodResult);;
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

        
		public abstract Int32 AddEntry(IEnumerable<FuzzyWordEntry> entries);

		public abstract IEnumerable<FuzzyQueryResponse> FuzzyQuery(IEnumerable<FuzzyCheck> phrases);

    }
}
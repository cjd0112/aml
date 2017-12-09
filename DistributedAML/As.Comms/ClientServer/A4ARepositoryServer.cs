   
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NetMQ;
using As.Logger;
using As.Shared;

namespace As.Comms.ClientServer
{
    public abstract class A4ARepositoryServer : IA4ARepository
    {
        protected IServiceServer server;
        protected A4ARepositoryServer(IServiceServer server)
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
               case "RunQuery":
                {
                    var query = request.UnpackMessage<GraphQuery>(GraphQuery.Parser.ParseFrom);					
                    var methodResult=RunQuery(query);
                    ret.PackMessage<GraphResponse>(methodResult);;
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

        
		public abstract GraphResponse RunQuery(GraphQuery query);

    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using AMLWeb.Models;
using As.Client;
using As.Comms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AMLWeb.Controllers
{
    [Produces("application/json")]
    [Route("GraphQl")]
    public class GraphQlController : Controller
    {
        private IClientFactory factory;
        private ILogger logger;
        public GraphQlController(IClientFactory factory,ILogger<GraphQlController> logger)
        {
            this.factory = factory;
            this.logger = logger;
        }

        [HttpPost]
        public IActionResult Post([FromBody]GraphQlQuery query)
        {
            var first = factory.GetClient<IAmlRepository>(0);

            var graphResponse = first.RunQuery(new GraphQuery {Query = query.Query});

            logger.LogDebug(graphResponse.Response);

            JObject foo = JObject.Parse(graphResponse.Response);

/*
            var output = new GraphQlDocument(query.Query)
                .Validate(typeof(Query))
                .Run(new Query())
                .GetOutput();
                */
           
            return Ok(foo);

        }
    }
}
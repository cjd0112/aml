using System;
using System.Collections.Generic;
using System.ComponentModel;
using AMLWeb.Models;
using As.Client;
using As.Comms;
using Microsoft.AspNetCore.Mvc;
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
        public GraphQlController(IClientFactory factory)
        {
            this.factory = factory;

        }

        [HttpPost]
        public IActionResult Post([FromBody]GraphQlQuery query)
        {
            var first = factory.GetClient<IAmlRepository>(0);

            var graphResponse = first.RunQuery(new GraphQuery {Query = query.Query});

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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using AMLWeb.Models;
using As.Client;
using As.GraphQL;
using GraphQL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AMLWeb.Controllers
{
    [Produces("application/json")]
    [Route("GraphQl")]
    public class GraphQlController : Controller
    {
        private IClientFactory factory;
        public GraphQlController(MyClients test)
        {
            var t = test;

        }

        [HttpPost]
        public IActionResult Post([FromBody]GraphQlQuery query)
        {
            var output = new GraphQlDocument(query.Query)
                .Validate(typeof(Query))
                .Run(new Query())
                .GetOutput();

           
            return Ok(output);

        }
    }
}
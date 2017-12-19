using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using App4Answers.Models;
using App4Answers.Models.A4Amodels;
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
        private ILogger logger;
        private A4AModel1 model;
        public GraphQlController(ILogger<GraphQlController> logger,  A4AModel1 model)
        {
            this.logger = logger;
            this.model = model;
        }

        [HttpPost]
        public IActionResult Post([FromBody]GraphQlQuery query)
        {
            var graphResponse = model.RunQuery(new GraphQuery { Query = query.Query, OperationName = query.OperationName ?? "", Variables = query.Variables ?? "" });

            logger.LogDebug(graphResponse.Response);

            JObject foo = JObject.Parse(graphResponse.Response);

            return Ok(foo);

        }
    }
}
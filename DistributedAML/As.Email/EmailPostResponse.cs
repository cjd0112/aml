using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace As.Email
{
    public class EmailPostResponse
    {
        public string To { get; set; }

        public string Subject { get; set; }

        public string From { get; set; }

        [JsonProperty("Message-Id")]
        public String MessageId { get; set; }

        public String Date { get; set; }

        [JsonProperty("body-plain")]
        public String BodyPlain { get; set; }

    }
}

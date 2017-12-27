using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RestSharp;

namespace As.Email
{


    public class EmailSender : IEmailSender
    {
        private RestClient client;
        public EmailSender()
        {
            client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator =
                new RestSharp.Authenticators.HttpBasicAuthenticator("api",
                    "key-b74d7782c1dc73f525a9c537e2a6e9b8");
        }
        public IEnumerable<A4AEmailRecord> SendMail(A4AMessage msg,A4AUser source, IEnumerable<A4AExpert> targets)
        {
            foreach (var c in targets)
            {
                RestRequest request = new RestRequest();
                request.AddParameter("domain", "mg.alphaaml.com", ParameterType.UrlSegment);
                request.Resource = "{domain}/messages";
                request.AddParameter("from", $"{source.UserName} <{source.Email}>");
                request.AddParameter("to", $"{c.ExpertName} <{c.Email}>");
                request.AddParameter("subject", msg.Subject);
                request.AddParameter("text", msg.Content);
                request.Method = Method.POST;
                dynamic ret = client.Execute(request);

                var record = new A4AEmailRecord
                {
                    MessageId = msg.MessageId,
                    EmailFrom = source.Email,
                    EmailTo = c.Email,
                    Status = EmailStatus.Created

                };
                yield return record;

            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.ServiceModel.Description;
using Newtonsoft.Json;
using RestSharp;

namespace As.Email
{


    public class EmailSender : IEmailSender
    {
        public EmailSender()
        {
           
        }
        public IEnumerable<A4AEmailRecord> SendMail(A4AEmailService service,A4AMessage msg,A4AUser source, IEnumerable<A4AExpert> targets)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(service.Uri);
            client.Authenticator =
                new RestSharp.Authenticators.HttpBasicAuthenticator(service.ApiUserName,
                    service.ApiPassword);
            foreach (var c in targets)
            {
                RestRequest request = new RestRequest();
                request.AddParameter("domain", service.Domain, ParameterType.UrlSegment);
                request.Resource = "{domain}/messages";
                //request.AddParameter("from", $"{source.UserName} <{source.Email}>");
                request.AddParameter("from", "Joe Bloggs <Joe.Bloggs@mg.alphaaml.com>");
                //                request.AddParameter("to", $"{c.ExpertName} <{c.Email}>");
                request.AddParameter("to", "Colin <colin.dick@alphastorm.co.uk>");
                request.AddParameter("subject", msg.Subject);
                request.AddParameter("text", msg.Content);
                request.Method = Method.POST;
                var result  = client.Execute(request);

                var foo = new {message = "", id = ""};

                var json = JsonConvert.DeserializeAnonymousType(result.Content, foo);

                var record = new A4AEmailRecord
                {
                    MessageId = msg.MessageId,
                    EmailFrom = source.Email,
                    EmailTo = c.Email,
                    Status = EmailStatus.Created,
                    ExternalMessageId = json.id,
                    ExternalStatus = json.message
                };
                yield return record;

            }
        }

       
        public EventsResponse GetNextMailEvents(A4AEmailService service)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");

            client.Authenticator =
                new RestSharp.Authenticators.HttpBasicAuthenticator("api",
                    "key-b74d7782c1dc73f525a9c537e2a6e9b8");

            RestRequest request = new RestRequest();
            request.AddParameter("domain", "mg.alphaaml.com", ParameterType.UrlSegment);
            request.Resource = "{domain}/events";

            // just look back one day for now. 
            if (service.LastPollTime == 0)
                service.LastPollTime = DateTime.Now.AddDays(-1).ToUniversalTime().ToBinary();

            request.AddParameter("begin", DateTime.FromBinary(service.LastPollTime).ToString("r"));
            request.AddParameter("end", DateTime.Now.ToUniversalTime().ToString("r"));
            request.AddParameter("event", "delivered");

            service.LastPollTime = DateTime.Now.ToUniversalTime().ToBinary();

            var result = client.Execute(request);

            var eventsResponse = JsonConvert.DeserializeObject<EventsResponse>(result.Content);

            return eventsResponse;
        }

        public EmailPostResponse EmailFromUrl(A4AEmailService service, string url)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(url);

            client.Authenticator =
                new RestSharp.Authenticators.HttpBasicAuthenticator(service.ApiUserName,
                    service.ApiPassword);
            RestRequest request = new RestRequest();

            var result = client.Execute(request);

            var emailResponse = JsonConvert.DeserializeObject<EmailPostResponse>(result.Content);

            return emailResponse;

        }
    }
}

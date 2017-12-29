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
                request.AddParameter("from", $"{source.UserName} <{source.UserName}@{service.Domain}>");
                request.AddParameter("to", $"{c.ExpertName} <{c.Email}>");
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
                    NameFrom = source.UserName,
                    EmailTo = c.Email,
                    NameTo =    c.ExpertName,
                    Status = EmailStatus.Created,
                    ExternalMessageId = json.id,
                    ExternalStatus = json.message,
                    Subject = msg.Subject
                };

                if (record.ExternalMessageId.StartsWith("<") && record.ExternalMessageId.EndsWith(">"))
                    record.ExternalMessageId =
                        record.ExternalMessageId.Substring(1, record.ExternalMessageId.Length - 2);
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
                service.LastPollTime = DateTime.Today.AddDays(-1).ToUniversalTime().ToBinary();

            var begin = DateTime.FromBinary(service.LastPollTime).AddMilliseconds(-service.LookbackMilliseconds).ToString("r");
            var end = DateTime.Now.AddMilliseconds(-DateTime.Now.Millisecond).ToUniversalTime().ToString("r");

            request.AddParameter("begin",begin );
            request.AddParameter("end", end);
            request.AddParameter("event", "delivered OR stored");

            service.LastPollTime = DateTime.Now.AddMilliseconds(-DateTime.Now.Millisecond).ToUniversalTime().ToBinary();

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

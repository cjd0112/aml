using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using As.Shared;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;

namespace As.Email
{
    public class DeliveryStatus
    {
        public string message { get; set; }

        [JsonProperty(PropertyName="retry-seconds")]
        public int retrySeconds { get; set; }

        public int code { get; set; }
    }

    public class Headers
    {
        public string to { get; set; }
        public string from { get; set; }
        public string subject { get; set; }
        [JsonProperty(PropertyName="message-id")]
        public string messageid { get; set; }
    }

    public class Storage
    {
        public string url { get; set; }
    }

    public class Message
    {
        public Headers headers { get; set; }
    }

    public enum EventTypes
    {
        accepted,
        rejected,
        delivered,
        failed,
        opened,
        clicked,
        unsubscribed,
        complained,
        stored
    }

    public class EmailEvent
    {
        public Message message { get; set; }
        public Storage storage { get; set; }

        [JsonProperty(PropertyName="delivery-status")]
        public DeliveryStatus deliveryStatus { get; set; }

        [JsonProperty(PropertyName="event")]
        public EventTypes eventType
        {
            get;
            set;
        }

        public String id { get; set; }

        public double timestamp { get; set; }

        /*  E.G., we hide the "real" user details in the "userPrefix" on OUR DOMAIN
              and the other side has the the "real" user details via the userPrefix+'@'+domain ... 
              so we can resolve to our internal ides -> email + name
         *   "to": "AAAAA BBBBBB <colin.dick1@btinternet.com>",
            "message-id": "20171230090512.1.C6B6208548CE2C88@mg.alphaaml.com",
            "from": "App4Answers <USER_0000@mg.alphaaml.com>",
            "subject": "A4A Question on '/Information Technology/Software Development/C#/Germany'"

         */

        public A4AEmailRecord ToRecord(Func<A4AUserNameResolver, string, string> emailNameResolver,string ourDomain)
        {
            var z = new A4AEmailRecord();
            z.RawFrom = message.headers.from;
            z.RawTo = message.headers.to;
            var from = message.headers.from.ParseLongEmailString();
            var to = message.headers.to.ParseLongEmailString();
            z.RecordType = from.domain == ourDomain ? A4AEmailRecordType.UserToExpert : A4AEmailRecordType.ExpertToUser;

            if (from.domain == ourDomain)
            {
                z.EmailFrom = emailNameResolver(A4AUserNameResolver.ResolveNameToEmail, from.userPrefix);
                z.NameFrom = from.userPrefix;
                z.EmailTo = to.userPrefix + "@" + to.domain;
                z.NameTo = emailNameResolver(A4AUserNameResolver.ResolveEmailToName, z.EmailTo);
            }
            else
            {
                z.EmailTo = emailNameResolver(A4AUserNameResolver.ResolveNameToEmail, to.userPrefix);
                z.NameTo = to.userPrefix;
                z.EmailFrom = from.userPrefix + "@" + from.domain;
                z.NameFrom = emailNameResolver(A4AUserNameResolver.ResolveEmailToName, z.EmailFrom);

            }

            z.ServiceMessageId = message.headers.messageid;
            z.Subject = message.headers.subject;
            var convertStatus = eventType.ToString();
            z.Status = (Char.ToUpper(convertStatus[0]) + convertStatus.Substring(1)).ToEnum<A4AEmailStatus>();

            z.Url = storage.url;

            z.Timestamp = new Timestamp
            {
                Seconds = (long)timestamp
            };

            if (z.Status == A4AEmailStatus.Failed)
            {
                z.StatusMessage = deliveryStatus.message;
            }

            return z;

        }
    }

    public class EmailEventsResponse
    {
        [JsonProperty(PropertyName="items")]
        public List<EmailEvent> EmailEvents { get; set; }
    }
}

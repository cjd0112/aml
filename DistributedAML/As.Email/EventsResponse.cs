using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;

namespace As.Email
{
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

    public class Item
    {
        public Message message { get; set; }
        public Storage storage { get; set; }

        [JsonProperty(PropertyName="event")]
        public EventTypes eventType
        {
            get;
            set;
        }
    }

    public class EventsResponse
    {
        public List<Item> items { get; set; }
    }
}

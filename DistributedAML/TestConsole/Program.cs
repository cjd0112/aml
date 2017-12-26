using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using As.Logger;
using Fasterflect;
using GraphQL;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace TestConsole
{
    class Program
    {

        public class myCat
        {
            public String cat { get; set; }
        }

        public static IRestResponse SendSimpleMessage()
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator =
            new RestSharp.Authenticators.HttpBasicAuthenticator("api",
                                      "key-b74d7782c1dc73f525a9c537e2a6e9b8");
            RestRequest request = new RestRequest();
            request.AddParameter("domain", "mg.alphaaml.com", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "Joe Bloggs <Joe.Bloggs@mg.alphaaml.com>");
            request.AddParameter("to", "Colin <colin.dick@alphastorm.co.uk>");
            request.AddParameter("subject", "Hello Colin from joe");
            request.AddParameter("text", "test from a dummy users @alphaaml,com");
            request.Method = Method.POST;
            return client.Execute(request);
        }


        static void Main(string[] args)
        {
            
            try
            {

                var z22 = SendSimpleMessage();
               

                var definition = new
                {
                    data = new
                    {
                        MYLIST = new List<myCat>()
                    }
                };
                
                StreamReader r = new StreamReader("file1.json");
                var z =  JsonConvert.DeserializeAnonymousType(r.ReadToEnd(),definition);
                var cat = z.data.MYLIST[0];

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.ReadLine();
        }
    }


    public static class Ext2
    {
        public static int GetFoo(this Account blah, String foo2)
        {
            return 100;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AmlClient.AS.Application;
using Comms;
using Logger;
using Shared;
using StructureMap;

namespace AmlClient.Commands
{
    public class Initialize
    {
        public class ClientWithBucket
        {
            [PrimaryKey]
            public string ClientName { get; set; }
            public int BucketCount { get; set; }
        }


        private MyRegistry reg;
        private Container c;
        private ClientFactory clientFactory;
        private string dbConnection;
        public Initialize(Container c,MyRegistry reg,ClientFactory factory)
        {
            this.reg = reg;
            this.c = c;
            this.clientFactory = factory;
            dbConnection = reg.DataDirectory + "\\db\\client.mdb";
        }

        public ClientFactory ClientFactory => clientFactory;

        public void Run()
        {
            L.Trace("Opening local data connection ... ");
            using (var db = new SQLiteConnection(dbConnection))
            {
                Console.WriteLine("Supporting the following interfaces ... ");

                foreach (var t in reg.ClientTypes)
                    Console.WriteLine(t.Name);

                String service = "";

                while (service == "")
                {
                    Console.WriteLine("Enter name of type to validate it's local state ... ");

                    service = Console.ReadLine();

                    if (reg.ClientTypes.All(x => x.Name != service))
                    {
                        Console.WriteLine($"type - {service} not found ...");
                        service = "";
                    }
                    else
                    {
                        var serviceType = reg.ClientTypes.First(x => x.Name == service);
                        Console.WriteLine(
                            $"Click 'y' to clear bucket-state (i.e., if bucket numbers have cleared down) for '{service}' or any other key to validate the service");

                        var action = Console.ReadLine();
                        if (action.ToLower() == "y")
                        {
                            db.CreateTable<Initialize.ClientWithBucket>();
                            db.Delete<Initialize.ClientWithBucket>(service);
                        }
                        else
                        {
                            ValidateServiceBucketsAreConsistent(serviceType);
                        }
                    }
                }
            }
        }

        public void ValidateServiceBucketsAreConsistent(Type serviceType)
        {
            if (reg.ClientTypes.All(x=>x.Name != serviceType.Name) )
                throw new Exception($"Unexpected service name - {serviceType.Name}");


            using (var db = new SQLiteConnection(dbConnection))
            {
                var bucketMax = clientFactory.GetClientBuckets(serviceType).Max();
                var bucketMin = clientFactory.GetClientBuckets(serviceType).Min();

                if (bucketMin != 0)
                    throw new Exception(
                        $"Minimum bucket is not zero it is - {bucketMin} - should be zero, for {serviceType.Name}");

                if (db.Find<Initialize.ClientWithBucket>(serviceType.Name) == null)
                {
                    db.Insert(new Initialize.ClientWithBucket {BucketCount = bucketMax, ClientName = serviceType.Name});
                }
                else
                {
                    var ppp = db.Find<Initialize.ClientWithBucket>(serviceType.Name);
                    if (ppp.BucketCount != bucketMax)
                    {
                        throw new Exception(
                            $"We have (dynamic) bucketMax for '{serviceType.Name}= {bucketMax} - but last recorded run bucketMax was - {ppp.BucketCount}... cannot continue - need to match bucketCount - or rebuild");
                    }
                }
            }
        }
    }
}

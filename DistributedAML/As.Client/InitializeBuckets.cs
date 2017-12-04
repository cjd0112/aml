using System;
using System.IO;
using System.Linq;
using As.Client.AS.Application;
using As.Logger;
using StructureMap;

namespace As.Client
{
    public class InitializeBuckets
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
        public InitializeBuckets(Container c,MyRegistry reg,ClientFactory factory)
        {
            this.reg = reg;
            this.c = c;
            this.clientFactory = factory;
            if (!Directory.Exists(reg.DataDirectory + "/db"))
                Directory.CreateDirectory(reg.DataDirectory + "/db");
            dbConnection = reg.DataDirectory + "/db/client.mdb";

            c.Inject(typeof(ClientFactory),factory);
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
                    Console.WriteLine("Enter name of type to validate it's local state or 'c' to continue... ");

                    service = Console.ReadLine();
                    if (service == "c")
                    {
                        continue;
                    }
                    else if (reg.ClientTypes.All(x => x.Name != service))
                    {
                        Console.WriteLine($"type - {service} not found ...");
                        service = "";
                    }
                    else
                    {
                        if (clientFactory.Initialized == false)
                        {
                            L.Trace("Initializing Client Factory ... ");
                            clientFactory.Initialize();
                        }


                        Console.WriteLine("hit here");

                        Console.WriteLine("hit here2");

                        Console.WriteLine(service);

                        foreach (var n in reg.ClientTypes)
                            Console.WriteLine(n.Name);
                        var serviceType = reg.ClientTypes.First(x => x.Name == service);


                        Console.WriteLine(
                            $"Click 'y' to clear bucket-state (i.e., if bucket numbers have cleared down) for '{service}' or any other key to validate the service");

                        var action = Console.ReadLine();
                        if (action.ToLower() == "y")
                        {
                            db.CreateTable<InitializeBuckets.ClientWithBucket>();
                            db.Delete<InitializeBuckets.ClientWithBucket>(service);
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
            L.Trace($"Validating service - {serviceType.Name}");
            if (reg.ClientTypes.All(x=>x.Name != serviceType.Name) )
                throw new Exception($"Unexpected service name - {serviceType.Name}");


            using (var db = new SQLiteConnection(dbConnection))
            {
                var buckets = clientFactory.GetClientBuckets(serviceType).ToArray();

                var bucketMax = buckets.Max();
                var bucketMin = buckets.Min();

                L.Trace($"BucketCount = {buckets.Count()}, BucketMin = {bucketMin}, BucketMax = {bucketMax} for {serviceType.Name}");

                if (bucketMin != 0)
                    throw new Exception(
                        $"Minimum bucket is not zero it is - {bucketMin} - should be zero, for {serviceType.Name}");

                if (bucketMax < 0)
                    throw new Exception(
                        $"Maximum bucket is less than zero it is - {bucketMax}  for {serviceType.Name}");

                db.CreateTable<InitializeBuckets.ClientWithBucket>();

                if (db.Find<InitializeBuckets.ClientWithBucket>(serviceType.Name) == null)
                {
                    L.Trace($"Adding new initial bucket entry for {serviceType.Name}");

                    db.Insert(new InitializeBuckets.ClientWithBucket {BucketCount = buckets.Count(), ClientName = serviceType.Name});
                }
                else
                {
                    var ppp = db.Find<InitializeBuckets.ClientWithBucket>(serviceType.Name);
                    if (ppp.BucketCount != buckets.Count())
                    {
                        throw new Exception(
                            $"We have (dynamic) Bucket Count for '{serviceType.Name}= {buckets.Count()} - but last recorded run bucketCount was - {ppp.BucketCount}... cannot continue - need to match bucketCount - or rebuild");
                    }
                    else
                    {
                        L.Trace($"Local bucket counts for {serviceType.Name} match");

                    }
                }
            }
        }
    }
}

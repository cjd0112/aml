using System;
using System.IO;
using System.Linq;
using As.Client.AS.Application;
using As.Logger;
using Microsoft.Extensions.Logging;
using StructureMap;

namespace As.Client
{
    public class Test
    {
        
    }
    public class ClientServicePartitionValidator
    {
        public class ClientWithPartition
        {
            [PrimaryKey]
            public string ClientName { get; set; }
            public int PartitionCount { get; set; }
        }

        private ILogger logger;
        private MyRegistry reg;
        private Container c;
        private ClientFactory clientFactory;
        private string dbConnection;

        public ClientFactory ClientFactory => clientFactory;

        public ClientServicePartitionValidator(ILogger<Test> logger,Container c,MyRegistry reg,ClientFactory factory)
        {
            this.logger = logger;
            this.reg = reg;
            this.c = c;
            this.clientFactory = factory;
            if (!Directory.Exists(reg.DataDirectory + "/db"))
                Directory.CreateDirectory(reg.DataDirectory + "/db");
            dbConnection = reg.DataDirectory + "/db/client.mdb";

            c.Inject(typeof(IClientFactory), factory);
        
        }

        public void ValidateServicePartitions()
        {
            logger.LogDebug("Opening local data connection ... ");
            using (var db = new SQLiteConnection(dbConnection))
            {
                logger.LogDebug("Supporting the following interfaces ... ");

                foreach (var t in reg.ClientTypes)
                    logger.LogDebug(t.Name);

                if (clientFactory.Initialized == false)
                {
                    logger.LogDebug("Initializing Client Factory ... ");
                    clientFactory.Initialize();
                }

                logger.LogDebug("Validating service buckets");

                foreach (var serviceType in reg.ClientTypes)
                {
                    ValidateServiceBucketIsConsistent(serviceType);
                }
            }
        }

        public void ResetServicePartition(String name)
        {
            if (reg.ClientTypes.Exists(x => x.Name == name))
            {
                using (var db = new SQLiteConnection(dbConnection))
                {
                    db.CreateTable<ClientServicePartitionValidator.ClientWithPartition>();
                    db.Delete<ClientServicePartitionValidator.ClientWithPartition>(name);
                }
            }
            else
            {
                throw new Exception($"Service - {name} is not found");
            }
        }

        public void ValidateServiceBucketIsConsistent(Type serviceType)
        {
            logger.LogDebug($"Validating service - {serviceType.Name}");
            if (reg.ClientTypes.All(x=>x.Name != serviceType.Name) )
                throw new Exception($"Unexpected service name - {serviceType.Name}");


            using (var db = new SQLiteConnection(dbConnection))
            {
                var buckets = clientFactory.GetClientBuckets(serviceType).ToArray();

                var bucketMax = buckets.Max();
                var bucketMin = buckets.Min();

                logger.LogDebug($"PartitionCount = {buckets.Count()}, BucketMin = {bucketMin}, BucketMax = {bucketMax} for {serviceType.Name}");

                if (bucketMin != 0)
                    throw new Exception(
                        $"Minimum bucket is not zero it is - {bucketMin} - should be zero, for {serviceType.Name}");

                if (bucketMax < 0)
                    throw new Exception(
                        $"Maximum bucket is less than zero it is - {bucketMax}  for {serviceType.Name}");

                db.CreateTable<ClientServicePartitionValidator.ClientWithPartition>();

                if (db.Find<ClientServicePartitionValidator.ClientWithPartition>(serviceType.Name) == null)
                {
                    logger.LogDebug($"Adding new initial bucket entry for {serviceType.Name}");

                    db.Insert(new ClientServicePartitionValidator.ClientWithPartition {PartitionCount = buckets.Count(), ClientName = serviceType.Name});
                }
                else
                {
                    var ppp = db.Find<ClientServicePartitionValidator.ClientWithPartition>(serviceType.Name);
                    if (ppp.PartitionCount != buckets.Count())
                    {
                        throw new Exception(
                            $"We have (dynamic) Bucket Count for '{serviceType.Name}= {buckets.Count()} - but last recorded run bucketCount was - {ppp.PartitionCount}... cannot continue - need to match bucketCount - or rebuild");
                    }
                    else
                    {
                        logger.LogDebug($"Local bucket counts for {serviceType.Name} match");

                    }
                }
            }
        }
    }
}

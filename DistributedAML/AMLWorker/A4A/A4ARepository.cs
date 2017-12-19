using As.Comms;
using As.Comms.ClientServer;
using As.A4ACore;
using As.GraphDB.Sql;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AMLWorker.A4A
{
    public class A4ARepository : A4ARepositoryServer
    {
        private As.A4ACore.A4ARepository underlying;
        public A4ARepository(IServiceServer server) : base(server)
        {
            var connectionString = SqlTableWithId.GetConnectionString(
                (string) server.GetConfigProperty("DataDirectory", server.BucketId),
                server.BucketId, "A4ARepository");
            underlying = new As.A4ACore.A4ARepository(connectionString);
        }

        public override GraphResponse RunQuery(GraphQuery query)
        {
            return underlying.RunQuery(query);
        }
    }
}
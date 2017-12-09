using System.Collections.Generic;

namespace As.Comms
{

    public interface IA4ARepository : ICommsContract
    {
        GraphResponse RunQuery(GraphQuery query);
    }
}

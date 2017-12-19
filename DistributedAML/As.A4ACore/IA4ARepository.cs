using System.Collections.Generic;
using As.GraphDB.Sql;

namespace As.A4ACore
{
    public interface IA4ARepository
    {
        GraphResponse RunQuery(GraphQuery query);

        A4AParty AddParty(A4AParty party);
        IEnumerable<A4AParty> QueryParties(string query,Range r,Sort s);
        A4AParty GetPartyById(string id);
        A4AParty SaveParty(A4AParty party);
        void DeleteParty(string id);


    }
}
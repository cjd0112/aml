using System;
using System.Collections.Generic;
using As.GraphDB.Sql;
using As.Shared;

namespace As.A4ACore
{
    public interface IA4ARepository
    {
        GraphResponse RunQuery(GraphQuery query);

        T AddObject<T>(T party);
        IEnumerable<T> QueryObjects<T>(string query,Range r,Sort s);
        T GetObjectByPrimaryKey<T>(string id);
        T SaveObject<T>(T party);
        void DeleteObject<T>(string id);

        IEnumerable<(ForeignKey foreignKey, IEnumerable<string> values)> GetPossibleForeignKeys<T>();

    }
}
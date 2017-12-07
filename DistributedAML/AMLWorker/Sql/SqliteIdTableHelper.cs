using System;
using System.Collections.Generic;
using System.Text;

namespace AMLWorker.Sql
{
    public class SqliteIdTableHelper<T>
    {
        private SqlliteIdTablePropertiesAndCommands<T> propertiesAndCommands;
        public SqliteIdTableHelper(SqlliteIdTablePropertiesAndCommands<T> propertiesAndCommands)
        {
            this.propertiesAndCommands = propertiesAndCommands;
        }


    }
}

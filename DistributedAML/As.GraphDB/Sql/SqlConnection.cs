using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Data.Sqlite;

namespace As.GraphDB.Sql
{
    public class SqlConnection
    {
        private string connectionString;
        public SqlConnection(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public String GetConnectionString(String dataDirectory, int bucket, String dbFile)
        {
            if (Directory.Exists(dataDirectory) == false)
                Directory.CreateDirectory(dataDirectory);
            return $"{dataDirectory}/{dbFile}_{bucket}.mdb";
        }
        public SqliteConnection Connection()
        {
            var c = new SqliteConnection(connectionString);
            c.Open();
            return c;
        }

        public SqliteConnection ConnectionFk()
        {
            var c = new SqliteConnection(connectionString);
            c.Open();
            using (var z = c.CreateCommand())
            {
                z.CommandText = "PRAGMA foreign_keys = ON;";
                z.ExecuteNonQuery();
            }
            return c;
        }

    }
}

using System;
using System.Reflection;
using As.GraphDB;
using As.GraphDB.Sql;
using As.Logger;

namespace As.A4ACore
{
    public class A4ARepository : IA4ARepository
    {
        private string connectionString;

     
        private SqlitePropertiesAndCommands<A4ACategory> categorySql = new SqlitePropertiesAndCommands<A4ACategory>("Categories");
        private SqlitePropertiesAndCommands<A4AMessage> messageSql = new SqlitePropertiesAndCommands<A4AMessage>("Messages");
        private SqlitePropertiesAndCommands<A4AParty> partySql = new SqlitePropertiesAndCommands<A4AParty>("Parties");
        
        public A4ARepository(String connectionString) 
        {
            this.connectionString = connectionString;

            L.Trace($"Initializing Sql - connectionString is {connectionString}");

            using (var connection = SqlTableHelper.NewConnection(connectionString))
            {
                if (!SqlTableHelper.TableExists(connection, partySql))
                {
                    SqlTableHelper.CreateTable(connection,partySql);
                }

                if (!SqlTableHelper.TableExists(connection, messageSql))
                {
                    SqlTableHelper.CreateTable(connection,messageSql);
                }


                if (!SqlTableHelper.TableExists(connection, categorySql))
                {
                    SqlTableHelper.CreateTable(connection,categorySql);
                }
            }
        }

        class GQ : IGraphQuery
        {
            public string OperationName { get; set; }
            public string Query { get; set; }
            public string Variables { get; set; }
        }

        public GraphResponse RunQuery(GraphQuery query)
        {
            using (var connection = SqlTableHelper.NewConnection(connectionString))
            {
                return new GraphResponse
                {
                    Response = new A4ARepositoryGraphDb(connection,partySql,categorySql, messageSql).Run(new GQ
                    {
                        OperationName = query.OperationName,
                        Query = query.Query,
                        Variables = query.Variables
                           
                    },new A4ACustomizeSchema()).ToString()
                };
            }
        }
    }
}
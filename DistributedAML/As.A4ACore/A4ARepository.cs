using System;
using System.Collections.Generic;
using System.Linq;
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
                else
                {
                    SqlTableHelper.UpdateTableStructure(connection,partySql);
                }

                if (!SqlTableHelper.TableExists(connection, messageSql))
                {
                    SqlTableHelper.CreateTable(connection,messageSql);
                }
                else
                {
                    SqlTableHelper.UpdateTableStructure(connection, messageSql);
                }


                if (!SqlTableHelper.TableExists(connection, categorySql))
                {
                    SqlTableHelper.CreateTable(connection,categorySql);
                }
                else
                {
                    SqlTableHelper.UpdateTableStructure(connection, categorySql);
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

        public A4AParty AddParty(A4AParty party)
        {
            using (var connection = SqlTableHelper.NewConnection(connectionString))
            {
                var id = SqlTableHelper.GetNextId(connection, partySql.tableName);
                party.Id = $"PARTY{id:0000000}";
                SqlTableHelper.InsertOrReplace(connection, partySql, new[] {party});

                return SqlTableHelper.SelectDataById(connection, partySql, party.Id);
            }
        }

        public A4AParty SaveParty(A4AParty party)
        {
            using (var connection = SqlTableHelper.NewConnection(connectionString))
            {
                SqlTableHelper.InsertOrReplace(connection, partySql, new[] { party });
                return SqlTableHelper.SelectDataById(connection, partySql, party.Id);
            }
        }

        public void DeleteParty(String id)
        {
            using (var connection = SqlTableHelper.NewConnection(connectionString))
            {
                SqlTableHelper.Delete(connection, partySql,id);
            }
        }


        public A4AParty GetPartyById(string id)
        {
            using (var connection = SqlTableHelper.NewConnection(connectionString))
            {
                return SqlTableHelper.SelectDataById(connection, partySql, id);
            }
        }


        public IEnumerable<A4AParty> QueryParties(String whereClause,Range range,Sort sort)
        {
            using (var connection = SqlTableHelper.NewConnection(connectionString))
            {
                return SqlTableHelper.SelectData(connection, partySql, whereClause, range, sort).Select(x => x.GetObject())
                    .ToArray();
            }
        }

    }
}
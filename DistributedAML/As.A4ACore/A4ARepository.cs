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

        private SqlTableWithId sqlTableWithId;

        private SqlTableComplexLinkages<A4AEntityType, A4ARelationType> sqlTableComplexLinkages;

        public A4ARepository(String connectionString) 
        {
            this.connectionString = connectionString;

            sqlTableWithId = new SqlTableWithId();

            sqlTableComplexLinkages = new SqlTableComplexLinkages<A4AEntityType,A4ARelationType>();

            L.Trace($"Initializing Sql - connectionString is {connectionString}");

            using (var connection = sqlTableWithId.NewConnection(connectionString))
            {
                if (!sqlTableWithId.TableExists(connection, partySql))
                {
                    sqlTableWithId.CreateTable(connection,partySql);
                }
                else
                {
                    sqlTableWithId.UpdateTableStructure(connection,partySql);
                }

                if (!sqlTableWithId.TableExists(connection, messageSql))
                {
                    sqlTableWithId.CreateTable(connection,messageSql);
                }
                else
                {
                    sqlTableWithId.UpdateTableStructure(connection, messageSql);
                }


                if (!sqlTableWithId.TableExists(connection, categorySql))
                {
                    sqlTableWithId.CreateTable(connection,categorySql);
                }
                else
                {
                    sqlTableWithId.UpdateTableStructure(connection, categorySql);
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
            using (var connection = sqlTableWithId.NewConnection(connectionString))
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
            using (var connection = sqlTableWithId.NewConnection(connectionString))
            {
                var id = sqlTableWithId.GetNextId(connection, partySql.tableName);
                party.Id = $"PARTY{id:0000000}";
                sqlTableWithId.InsertOrReplace(connection, partySql, new[] {party});

                return sqlTableWithId.SelectDataById(connection, partySql, party.Id);
            }
        }

        public A4AParty SaveParty(A4AParty party)
        {
            using (var connection = sqlTableWithId.NewConnection(connectionString))
            {
                sqlTableWithId.InsertOrReplace(connection, partySql, new[] { party });
                return sqlTableWithId.SelectDataById(connection, partySql, party.Id);
            }
        }

        public void DeleteParty(String id)
        {
            using (var connection = sqlTableWithId.NewConnection(connectionString))
            {
                sqlTableWithId.Delete(connection, partySql,id);
            }
        }


        public A4AParty GetPartyById(string id)
        {
            using (var connection = sqlTableWithId.NewConnection(connectionString))
            {
                return sqlTableWithId.SelectDataById(connection, partySql, id);
            }
        }


        public IEnumerable<A4AParty> QueryParties(String whereClause,Range range,Sort sort)
        {
            using (var connection = sqlTableWithId.NewConnection(connectionString))
            {
                return sqlTableWithId.SelectData(connection, partySql, whereClause, range, sort).Select(x => x.GetObject())
                    .ToArray();
            }
        }

    }
}
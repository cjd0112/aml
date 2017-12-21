﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using As.GraphDB;
using As.GraphDB.Sql;
using As.Logger;
using As.Shared;
using Google.Protobuf;

namespace As.A4ACore
{
    public class A4ARepository : IA4ARepository
    {
        private static IEnumerable<Type> _A4ATypes;

        private static IEnumerable<Type> A4ATypes
        {
            get
            {
                return _A4ATypes ?? (_A4ATypes = Assembly.GetAssembly(typeof(A4ACategory)).GetTypes()
                           .Where(predicate: x =>
                               x.Name.StartsWith("A4A") && x.IsClass && typeof(IMessage).IsAssignableFrom(x))
                           .Select(x => x).ToArray());
            }
        }

        Dictionary<Type, SqlTableWithPrimaryKey> tables = new Dictionary<Type, SqlTableWithPrimaryKey>();

        private SqlConnection conn;

        public A4ARepository(String connectionString)
        {
            L.Trace("Initializing types in assembly");

            foreach (var type in A4ATypes)
            {
                tables[type] =
                    new SqlTableWithPrimaryKey(new SqlitePropertiesAndCommands(TypeContainer.GetTypeContainer(type)));
            }
            conn = new SqlConnection(connectionString);

            L.Trace($"Initializing Sql - connectionString is {connectionString}");

            using (var connection = conn.Connection())
            {
                foreach (var c in tables.Keys)
                {
                    tables[c].PropertiesAndCommands
                        .VerifyForeignKeysFromOtherTables(tables.Values.Select(x => x.PropertiesAndCommands));

                    if (tables[c].TableExists(connection) == false)
                    {
                        tables[c].CreateTable(connection);
                    }
                    else
                    {
                        tables[c].UpdateTableStructure(connection);
                    }
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
            using (var connection = conn.Connection())
            {
                return new GraphResponse
                {
                    Response = new A4ARepositoryGraphDb(connection, tables[typeof(A4AMessage)]).Run(new GQ
                    {
                        OperationName = query.OperationName,
                        Query = query.Query,
                        Variables = query.Variables

                    }, new A4ACustomizeSchema()).ToString()
                };
            }
        }

        public T AddObject<T>(T obj)
        {
            using (var connection = conn.ConnectionFk())
            {
                var sqlTable = tables[typeof(T)];
                sqlTable.InsertOrReplace(connection, new[] {obj});

                return sqlTable.SelectDataByPrimaryKey<T>(connection, sqlTable.GetPrimaryKey(obj));
            }
        }

        public T SaveObject<T>(T obj)
        {
            using (var connection = conn.ConnectionFk())
            {
                var sqlTable = tables[typeof(T)];
                sqlTable.InsertOrReplace(connection, new[] {obj}, true);
                return sqlTable.SelectDataByPrimaryKey<T>(connection, sqlTable.GetPrimaryKey(obj));
            }
        }

        public void DeleteObject<T>(String primaryKey)
        {
            using (var connection = conn.ConnectionFk())
            {
                var sqlTable = tables[typeof(T)];
                sqlTable.Delete<T>(connection, primaryKey);
            }
        }


        public T GetObjectByPrimaryKey<T>(string primaryKey)
        {
            using (var connection = conn.Connection())
            {
                var sqlTable = tables[typeof(T)];
                return sqlTable.SelectDataByPrimaryKey<T>(connection, primaryKey);
            }
        }

        public IEnumerable<T> QueryObjects<T>(String whereClause, Range range, Sort sort)
        {
            using (var connection = conn.Connection())
            {
                var sqlTable = tables[typeof(T)];
                return sqlTable.SelectData<T>(connection, whereClause, range, sort).Select(x => x.GetObject())
                    .ToArray();
            }
        }

        public IEnumerable<(ForeignKey foreignKey, IEnumerable<string> values)> GetPossibleForeignKeys<T>()
        {
            using (var connection = conn.Connection())
            {
                var sqlTable = tables[typeof(T)];
                foreach (var c in sqlTable.PropertiesAndCommands.typeContainer.Properties
                    .Where(x => x.foreignKey != null)
                    .Select(x => x.foreignKey))
                {
                    var table = tables[A4ATypes.First(x => x.Name == c.TableName)];

                    yield return (c, table.SelectPrimaryKeyValues(connection).ToArray());
                }
            }
        }


    }
}
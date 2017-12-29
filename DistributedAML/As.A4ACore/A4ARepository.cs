using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

                if (type == typeof(A4AEmailRecord))
                    tables[type].ConvertEmptyForeignKeysToNull();
            }
            conn = new SqlConnection(connectionString);

            L.Trace($"Initializing Sql - connectionString is {connectionString}");

            using (var connection = conn.Connection())
            {
                foreach (var c in tables.Keys)
                {
                    if (c == typeof(A4AEmailRecord))
                    {

                    }
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

                var existing = sqlTable.SelectDataByPrimaryKey<T>(connection, primaryKey);

                if (existing == null)
                    throw new Exception($"Delete failed - Entry in table {sqlTable.TableName} with primaryKey: '{primaryKey}'  on column {sqlTable.PropertiesAndCommands.GetPrimaryKeyProperty().Name} is not found");

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
                    var table = tables[A4ATypes.First(x => x.Name == c.ParentTableName)];

                    yield return (c, table.SelectPrimaryKeyValues(connection).ToArray());
                }
            }
        }


        private (string profession, string category, string subcategory, string location) ParseTopic(string topic)
        {
            var z = topic.Split(new char[] {'/'},StringSplitOptions.RemoveEmptyEntries);
            if (z.Length != 4)
                throw new Exception(
                    $"Invalid topic expression - {topic} expecting /profession/category/subcategory/location");

            return (z[0], z[1], z[2], z[3]);
        }

        public (A4AUser user,IEnumerable<A4AExpert> experts) GetUserAndExpertsForMessage(A4AMessage msg)
        {
            A4AUser user = null;
            List<A4AExpert> experts = new List<A4AExpert>();
            using (var connection = conn.Connection())
            {
                using (var cmd = connection.CreateCommand())
                {
                    var foo = ParseTopic(msg.Topic);
                    cmd.CommandText =
                        $"select B.* from A4ASubscription as A inner join A4AExpert as B where A.Profession like '{foo.profession}' and B.ExpertName == A.ExpertName and A.Category like '{foo.category}' and A.SubCategory like '{foo.subcategory}' and A.Location like '{foo.location}';";

                    L.Trace($"selecting experts for message - {cmd.CommandText}");
                    using (var data = cmd.ExecuteReader())
                    {
                        var p = new DataRecordHelper<A4AExpert>(tables[typeof(A4AExpert)].PropertiesAndCommands, data);

                        while (data.Read())
                        {
                            experts.Add(p.GetObject());
                        }
                    }
                }

                if (experts.Count == 0)
                {
                    throw new Exception($"message Topic - {msg.Topic} - did not select any experts ... ");
                }

                var userTable = tables[typeof(A4AUser)];
                user = userTable.SelectOne<A4AUser>(connection, "Email", msg.EmailSender);
            }

            return (user,experts);
        }

        public int Count<T>()
        {
            using (var connection = conn.Connection())
            {
                var sqlTable = tables[typeof(T)];
                return sqlTable.GetCount(connection);
            }
        }

        public A4AEmailRecord UpdateEmailRecordStatus(string externalMessageId, string status)
        {
            using (var connection = conn.Connection())
            {
                var sqlTable = tables[typeof(A4AEmailRecord)];
                var z = sqlTable.SelectOne<A4AEmailRecord>(connection, "externalMessageId",externalMessageId);
                z.ExternalStatus = status;
                z.UpdatedTime = DateTime.Now.ToUniversalTime().ToString("r");

                sqlTable.InsertOrReplace(connection, new[] {z}, true);

                return z;
            }
        }

        public (A4AUser user, A4AExpert expert) GetUserAndExpertForReply(string userName, string expertEmail)
        {
            using (var connection = conn.Connection())
            {
                var userTable = tables[typeof(A4AUser)];

                var expertTable = tables[typeof(A4AExpert)];

                var user = userTable.SelectOne<A4AUser>(connection, "UserName", userName);

                var expert = expertTable.SelectOne<A4AExpert>(connection, "Email", expertEmail);

                return (user, expert);
            }
        }
    }
}
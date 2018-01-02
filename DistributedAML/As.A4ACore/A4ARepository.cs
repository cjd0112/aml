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
using Fasterflect;
using Google.Protobuf;
using Microsoft.Data.Sqlite;

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

        SqlTableWithPrimaryKey GetTable<T>()
        {
            return tables[typeof(T)];
        }

        SqlTableWithPrimaryKey GetTable(Type t)
        {
            return tables[t];
        }

        SqlitePropertiesAndCommands GetPropertiesAndCommands<T>()
        {
            if (typeof(T) == typeof(AggregateMessage))
                return aggregateMessage;
            return GetTable<T>().PropertiesAndCommands;
        }

        SqlitePropertiesAndCommands GetPropertiesAndCommands(Type t)
        {
            if (t == typeof(AggregateMessage))
                return aggregateMessage;
            return GetTable(t).PropertiesAndCommands;
        }

        private SqlitePropertiesAndCommands aggregateMessage;

        private SqlConnection conn;


        private SqlPrimaryKeyAndTypeManager primaryKeyAndTypeManager = new SqlPrimaryKeyAndTypeManager();

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

            primaryKeyAndTypeManager.AddPrimaryKeyPrefixAndEnum(typeof(A4AExpert), "EXPERT", A4APartyType.Expert)
                .AddPrimaryKeyPrefixAndEnum(typeof(A4AUser), "USER", A4APartyType.User)
                .AddPrimaryKeyPrefixAndEnum(typeof(A4AExpert), "ADMIN", A4APartyType.Admin);

            tables[typeof(A4AExpert)]
                .SetAutoPrimaryKey((i) => primaryKeyAndTypeManager.GenerateId(typeof(A4AExpert), i));

            tables[typeof(A4AUser)]
                .SetAutoPrimaryKey((i) => primaryKeyAndTypeManager.GenerateId(typeof(A4AUser), i));

            tables[typeof(A4AAdministrator)]
                .SetAutoPrimaryKey((i) => primaryKeyAndTypeManager.GenerateId(typeof(A4AAdministrator), i));


            aggregateMessage = new SqlitePropertiesAndCommands(TypeContainer.GetTypeContainer(typeof(AggregateMessage)));

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

        A4APartyType GetUserEnumTypeFromId(string id)
        {
            return primaryKeyAndTypeManager.GetEnumFromId<A4APartyType>(id);
        }

        Type GetUserTypeFromId(string id)
        {
            return primaryKeyAndTypeManager.GetTypeFromId(id);
        }

        SqlTableWithPrimaryKey GetTableFromId(string id)
        {
            return tables[GetUserTypeFromId(id)];
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

        public IEnumerable<T> QueryObjects<T>(String whereClause="", Range range=null, Sort sort=null)
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

        AggregateParty GetAggregateParty(SqliteConnection conn,string name,string email)
        {
            var ret = new AggregateParty
            {
                Name = name,
                Email = email,
                PartyType = GetUserEnumTypeFromId(name)
            };
            var t = GetTableFromId(name).SelectDataByPrimaryKey(conn, name);
            switch (t)
            {
                case A4AUser user:
                    ret.User = user;
                    break;
                case A4AExpert expert:
                    ret.Expert = expert;
                    break;
                case A4AAdministrator admin:
                    ret.Admin = admin;
                    break;
                case A4ACompany company:
                    ret.Company = company;
                    break;
                default:
                    throw new Exception($"Unexpected user type from name - {name}");
            }
            return ret;
        }

        AggregateParty GetAggregatePartyFromCompany(SqliteConnection conn, string name)
        {
            var ret = new AggregateParty { Name = name,PartyType = A4APartyType.Company };
            ret.Company= (A4ACompany)tables[typeof(A4ACompany)].SelectDataByPrimaryKey(conn,name);
            return ret;
        }


        (string nameToOrFrom, string externalStatus) GetSearchParameters(A4AMailboxType mailboxType,A4APartyType party)
        {
            if (mailboxType == A4AMailboxType.Inbox && party == A4APartyType.Expert)
            {
                return ("NameTo", "delivered");
            }
            else if (mailboxType == A4AMailboxType.Inbox && party == A4APartyType.User)
            {
                return ("NameTo", "stored");
            }
            else if (mailboxType == A4AMailboxType.Sent && party == A4APartyType.Expert)
            {
                return ("NameFrom", "stored");
            }
            else if (mailboxType == A4AMailboxType.Sent && party == A4APartyType.User)
            {
                return ("NameFrom", "delivered");
            }

            throw new Exception($"Unexpected mailboxtype and party - {mailboxType} - {party}");

        }

        SubscriptionNode FindParent(SubscriptionNode current, A4ASubscriptionType t,string value)
        {
            if (((int) t) <= ((int) current.Type))
            {
                return null;
            }
            else
            {
                if (current.Type == t-1)
                    return current;
                else
                {
                    foreach (var c in current.Children)
                    {
                        var parent = FindParent(c, t, value);
                        if (parent != null)
                            return parent;
                    }

                    return null;
                }
            }           
        }

        SubscriptionNode MatchingSiblings(SubscriptionNode parent, A4ASubscriptionType t, string value)
        {
            foreach (var c in parent.Children)
            {
                if (c.Name == value)
                    return c;
                ;
            }

            return null;
        }

        SubscriptionNode AddChild(SubscriptionNode parent, A4ASubscriptionType t, string value)
        {
            var q = new SubscriptionNode
            {
                Name = value,
                Type = t
            };
            parent.Children.Add(q);
            return q;
        }

        A4ASubscriptionType GetLowestLevel(A4ASubscription sub)
        {
            if (!String.IsNullOrEmpty(sub.Location))
                return A4ASubscriptionType.Location;

            if (!String.IsNullOrEmpty(sub.SubCategory))
                return A4ASubscriptionType.SubCategory;

            if (!String.IsNullOrEmpty(sub.Category))
                return A4ASubscriptionType.Category;

            if (!String.IsNullOrEmpty(sub.Profession))
                return A4ASubscriptionType.Profession;

            return A4ASubscriptionType.Empty;
        }

        private void AddEntryToTree(SqliteConnection connection,String expertName,SubscriptionNode root,A4ASubscriptionType type,string value,A4ASubscriptionType lowestLevel)
        {
            var parent = FindParent(root, type, value);
            if (parent != null)
            {
                var matchingSiblings = MatchingSiblings(parent, type, value);
                if (matchingSiblings != null)
                {                    
                    if (lowestLevel == type)
                        matchingSiblings.Experts.Add(expertName);
                }
                else
                {
                        var node = AddChild(parent, type, value);
                        if (lowestLevel == type)
                            node.Experts.Add( expertName);
                }

            }
        }

        public SubscriptionResponse GetSubscriptionInfo(SubscriptionRequest request)
        {
            var sr = new SubscriptionResponse();

            using (var connection = conn.Connection())
            {
                sr.Root = new SubscriptionNode
                {
                    Name="Subscriptions",
                    Type=A4ASubscriptionType.Empty
                };

                List<string> expertNames = new List<string>();

                foreach (var q in tables[typeof(A4ASubscription)].SelectData<A4ASubscription>(connection, "").Select(x => x.GetObject()))
                {
                    var lowestLevel = GetLowestLevel(q);

                    AddEntryToTree(connection,q.ExpertName,sr.Root,A4ASubscriptionType.Profession,q.Profession,lowestLevel);
                    if (!String.IsNullOrEmpty(q.Category))
                    {
                        AddEntryToTree(connection, q.ExpertName, sr.Root, A4ASubscriptionType.Category, q.Category,
                            lowestLevel);

                        if (!String.IsNullOrEmpty(q.SubCategory))
                        {
                            AddEntryToTree(connection, q.ExpertName, sr.Root, A4ASubscriptionType.SubCategory,
                                q.SubCategory, lowestLevel);

                            if (!String.IsNullOrEmpty(q.Location))
                            {
                                AddEntryToTree(connection, q.ExpertName, sr.Root, A4ASubscriptionType.Location, q.Location, lowestLevel);
                            }
                        }
                    }
                    if (expertNames.Contains(q.ExpertName) == false)
                        expertNames.Add(q.ExpertName);
                }

                foreach (var c in expertNames)
                {

                    sr.Parties.Add(GetTable<A4AExpert>()
                        .SelectDataByPrimaryKey<A4AExpert>(connection, c));
                }

                return sr;
            }
        }



        public MailboxInfoResponse GetMailboxInfo(MailboxInfoRequest request)
        {
            var mir = new MailboxInfoResponse();

            using (var connection = conn.Connection())
            {
                var owner = GetUserEnumTypeFromId(request.Owner);

             

                foreach (var c in new[] {A4AMailboxType.Inbox, A4AMailboxType.Sent})
                {
                    var search = GetSearchParameters(c, owner);

                    mir.MailboxInfos.Add(new MailboxInfo
                    {
                        Owner = request.Owner,
                        MailboxType = c,
                        Count = GetTable<A4AEmailRecord>().GetCount(connection,new SqlPredicate(search.nameToOrFrom,request.Owner),new SqlPredicate("ExternalStatus",search.externalStatus)),
                        Read = GetTable<A4AEmailRecord>().GetCount(connection, new SqlPredicate(search.nameToOrFrom, request.Owner), new SqlPredicate("ExternalStatus", search.externalStatus),new SqlPredicate("Read","True"))
                    });
                }

                mir.Parties.Add(GetAggregateParty(connection, request.Owner, ""));
                if (GetUserTypeFromId(request.Owner) == typeof(A4AExpert))
                {
                    var company = GetAggregatePartyFromCompany(connection, mir.Parties.Last().Expert.CompanyName);
                    mir.Parties.Add(company);
                }
            }

            return mir;
        }


        public MailboxView GetMailbox(MailboxRequest request)
        {
            var mb = new MailboxView();
            
            mb.Request = request;
            using (var connection = conn.Connection())
            {
                bool userProcessed(string n)
                {
                    return mb.Parties.Any(x => x.Name == n);
                }

                var emailRecords = tables[typeof(A4AEmailRecord)];

                (var nameToOrFrom, var externalStatus) = GetSearchParameters(request.MailboxType, request.UserType);
                mb.Count = GetTable<A4AEmailRecord>().GetCount(connection,new SqlPredicate(nameToOrFrom, request.Owner),
                    new SqlPredicate("ExternalStatus", externalStatus));

                foreach (var c in new SqlInnerJoin<AggregateMessage, A4AEmailRecord, A4AMessage>(GetPropertiesAndCommands)
                    .JoinT1Predicate(connection, "MessageId", new SqlPredicate(nameToOrFrom, request.Owner), new SqlPredicate("ExternalStatus", externalStatus)))
                {
                    mb.Messages.Add(c);

                    if (!userProcessed(c.NameFrom))
                    {
                        var party = GetAggregateParty(connection, c.NameFrom, c.EmailFrom);
                        mb.Parties.Add(party);

                        if (GetUserTypeFromId(c.NameFrom) == typeof(A4AExpert))
                        {
                            if (!userProcessed(party.Expert.CompanyName))
                            {
                                var company = GetAggregatePartyFromCompany(connection, party.Expert.CompanyName);
                                mb.Parties.Add(company);
                            }
                        }


                        party = GetAggregateParty(connection, c.NameTo, c.EmailTo);

                        mb.Parties.Add(party);

                        if (GetUserTypeFromId(c.NameTo) == typeof(A4AExpert))
                        {
                            if (!userProcessed(party.Expert.CompanyName))
                            {
                                var company = GetAggregatePartyFromCompany(connection, party.Expert.CompanyName);
                                mb.Parties.Add(company);
                            }
                        }                        
                    }
                }
            }
            return mb;
        }

    }
}
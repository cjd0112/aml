using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using AMLWorker.Sql;
using As.GraphQL;
using As.GraphQL.Interface;
using Fasterflect;
using Google.Protobuf;
using GraphQL;
using Microsoft.Data.Sqlite;

namespace AMLWorker
{
    public class AmlRepositoryQuery
    {
        public IEnumerable<Transaction> Transactions { get; set; }
        public IEnumerable<Account> Accounts { get; set; }
        public IEnumerable<Party> Parties { get; set; }

    }

    public class Range
    {
        public int Start { get; set; }
        public int End { get; set; }
    }

    public enum SortTypeEnum
    {
        None,
        Asc,
        Desc,
    }

    public class Sort
    {

        public String SortField { get; set; }
        public SortTypeEnum SortType { get; set; }

    }

    public class CustomizeSchema : GraphQlCustomiseSchema
    {
        public override bool IncludeProperty(PropertyInfo pi)
        {
            if (typeof(IMessage).IsAssignableFrom(pi.DeclaringType))
            {
                if (pi.PropertyType.Name.StartsWith("MessageParser") ||
                    pi.PropertyType.Name == "MessageDescriptor")
                    return false;
            }

            return true;
        }

        public override IEnumerable<(string fieldName, string description, Type fieldType)> AddAdditionalFields(Type type)
        {
            return base.AddAdditionalFields(type);
        }

        public override IEnumerable<(string inputName, Type inputType)> GetInputValues(string fieldName, PropertyInfo pi)
        {
            foreach (var c in base.GetInputValues(fieldName, pi))
                yield return c;

            if (pi.DeclaringType == typeof(AmlRepositoryQuery) &&   pi.PropertyType == typeof(IEnumerable<Account>))
            {
                yield return ("Range", typeof(Range));
                yield return ("Sort", typeof(Sort));

            }
        }
    }

    public class AmlRepositoryGraphDb : IGraphQlDatabase
    {
        private SqliteConnection conn;

        public AmlRepositoryGraphDb(SqliteConnection conn)
        {
            this.conn = conn;

        }

        public Object Run(String query)
        {
            return new GraphQlDocument(query)
                .CustomiseSchema(new CustomizeSchema())
                .Validate(typeof(AmlRepositoryQuery))
                .Run(new AmlRepositoryQuery(), this)
                .GetOutput();
        }

        public bool SupportField(object parentObject, string fieldName)
        {
            if (parentObject is AmlRepositoryQuery && fieldName == "Transactions" || fieldName == "Accounts" ||
                fieldName == "Parties")
                return true;
            return false;
        }

        public IEnumerable<object> ResolveFieldValue(object parentObject, string fieldName,
            Dictionary<string, object> argumentValues)
        {
            if (parentObject is AmlRepositoryQuery && fieldName == "Accounts")
            {
                int start = 0;
                int end = 200;
                if (argumentValues.ContainsKey("Range"))
                {
                    var z = argumentValues["Range"] as Range;
                    start = z.Start;
                    end = z.End;
                }

                string sortKey = "";
                SortTypeEnum sortType = SortTypeEnum.None;
                if (argumentValues.ContainsKey("Sort"))
                {
                    var s = argumentValues["Sort"] as Sort;
                    sortKey = s.SortField;
                    sortType = s.SortType;
                }
                if (sortType == SortTypeEnum.None)
                {
                    foreach (var c in SqlConnectionHelper.GetBlobs(conn, "Accounts", start, end))
                    {
                        yield return Account.Parser.ParseFrom(c.blob);
                    }
                }
                else
                {
                    if (String.IsNullOrEmpty(sortKey))
                        throw new Exception($"Invalid sort-key - received null");

                    if (SqlConnectionHelper.IndexExists(conn, "Accounts", sortKey))
                    {
                        foreach (var c in SqlConnectionHelper.GetBlobs(conn, "Accounts", start, end,sortKey))
                        {
                            var z = new Account
                            {
                                AccountNo = 
                            }
                            yield return Account.Parser.ParseFrom(c.blob);
                        }
                    }
                    else
                    {
                        var sortValueDelegate = typeof(Account).DelegateForGetPropertyValue(sortKey);
                        List<(string id,string sorter) > lst = new List<(string id, string sorter)>();
                        foreach (var c in SqlConnectionHelper.GetBlobs(conn, "Accounts", 0, -1))
                        {
                            var obj = Account.Parser.ParseFrom(c.blob);
                            var sortVal = sortValueDelegate(obj);
                            var val = sortVal;
                            lst.Add((c.id,(string)val));

                        }

                        lst.Sort((x,y)=>String.Compare(x.sorter, y.sorter, StringComparison.Ordinal));

                        foreach (var c in SqlConnectionHelper.QueryId2(conn, "Accounts",
                            lst.GetRange(start, end - start).Cast<Object>(), (x) =>
                            {
                                (string, string) obj = (ValueTuple<string, string>) x;
                                return obj.Item1;
                            }))
                        {
                            yield return Account.Parser.ParseFrom(c.Item2);
                        }
                    }
                    
                }
            }
        }
    }
}

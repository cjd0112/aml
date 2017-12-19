using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using As.GraphQL.Interface;
using As.Shared;
using Fasterflect;

namespace As.GraphDB.Sql
{
    public enum ColumnType
    {
        String,
        Numeric
    }

    public class SqlitePropertiesAndCommands<T>
    {
        public Type t;
        public String tableName;
        List<PropertyContainer> pis = new List<PropertyContainer>();

        private PropertyContainer id;

        private ConstructorInvoker ci;
        
        public SqlitePropertiesAndCommands(String tableName,Predicate<PropertyInfo> includeProperty = null)
        {
            this.t = typeof(T);
            this.tableName = tableName;
            foreach (var c in t.GetProperties().Where(x => x.PropertyType == typeof(String) || !typeof(IEnumerable).IsAssignableFrom(x.PropertyType)))
            {
                if (!c.IsWritable()) continue;
                if (includeProperty != null && includeProperty(c) == false)
                {
                    continue;
                }

                if (c.Name.ToLower() == "id")
                    id = new PropertyContainer(c);

                pis.Add(new PropertyContainer(c));
            }

            ci = this.t.DelegateForCreateInstance();
        }

        public T CreateInstance()
        {
            return (T) ci();

        }

        public T CreateInstance(ISupportGetValue sv)
        {
            var t = CreateInstance();
            foreach (var q in pis)
            {
                q.SetValue(t,sv.GetValue(q.pi.Name));
            }
            return (T) t;

        }


        public SqlitePropertiesAndCommands(Predicate<PropertyInfo> propertyFilter=null) : this(typeof(T).Name,propertyFilter)
        {
        }

        public String GetId(T obj)
        {
            return (string)id.GetValue(obj);
        }
        
      


        public IEnumerable<PropertyContainer> SqlFields()
        {
            return pis;
        }

     

        public string QueryIdCommand()
        {
            return $"select count(*) from { tableName} where id = ($id)";
        }

        public string AddColumnCommand(String columnName,ColumnType type)
        {
            string columnType = "";
            if (type == ColumnType.String)
                columnType = "text";
            else
                columnType = "numeric";
            return $"alter table {tableName} add column {columnName} {columnType};";
        }

        public string UpdateColumnValuesCommandStr(String columnName,String value)
        {
            return $"update {tableName} set {columnName}=\"{value}\"";
        }

        public string UpdateColumnValuesCommandForId(String columnName)
        {
            return $"update {tableName} set {columnName}=$value where id=$id";
        }

        static void TrimComma(StringBuilder b)
        {
            if (b[b.Length - 1] == ',')
                b.Remove(b.Length - 1, 1);
        }

        public String InsertOrReplaceCommand()
        {
            StringBuilder b = new StringBuilder();

            b.Append($"insert or replace into {tableName} ");

            StringBuilder values = new StringBuilder();

            values.Append(" values (");

            StringBuilder names = new StringBuilder();

            names.Append("(");

            foreach (var c in pis)
            {
                names.Append($"{c.pi.Name},");
                values.Append($"${c.pi.Name},");
            }
            TrimComma(names);
            TrimComma(values);

            names.Append(")");
            values.Append(")");

            return b.ToString() + " " + names.ToString() + " " + values.ToString();
        }

        public String DeleteCommand(string id)
        {
            return $"delete from {tableName} where Id like '{id}'";
        }

        public String SelectCommand()
        {
            StringBuilder b = new StringBuilder();

            b.Append($"select  ");

            foreach (var c in pis)
            {
                b.Append($"{c.pi.Name},");
            }

            TrimComma(b);

            b.Append($" from {tableName} ");

            return b.ToString();
        }

        public String RangeClause(Range range)
        {
            if (range == null)
                return $" (rowid >= 0 and rowid <= 200) ";
            if (range.End > range.Start)
                return $" (rowid >= {range.Start} and rowid <= {range.End}) ";
            else
                return $" (rowid >= {range.Start} ";
        }

        public String SortClause(Sort sort)
        {
            if (sort == null)
                return $"";

            if (sort.SortType == SortTypeEnum.None)
                return "";
            if (String.IsNullOrEmpty(sort.SortField))
                return "";
            return $" order by {sort.SortField} {sort.SortType} ";
        }

        public string CreateTableCommand()
        {
            var b = new StringBuilder();

            b.Append($"create table {tableName} (");
            foreach (var c in pis)
            {
                if (c.pi.Name.ToLower() == "id")
                    b.Append($"{c.pi.Name} {ConvertPropertyType(c.pi.PropertyType)} primary key,");
                else
                    b.Append($"{c.pi.Name} {ConvertPropertyType(c.pi.PropertyType)},");
            }

            if (b[b.Length - 1] == ',')
                b.Remove(b.Length - 1, 1);

            b.Append(");");

            return b.ToString();
        }

        static String ConvertPropertyType(Type t)
        {
            if (t == typeof(String))
                return "text";
            else if (t.IsEnum)
                return "text";
            else
                return "numeric";
        }

      
    }
}

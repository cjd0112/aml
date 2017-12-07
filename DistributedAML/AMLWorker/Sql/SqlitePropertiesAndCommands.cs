using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Fasterflect;

namespace AMLWorker.Sql
{
    public enum ColumnType
    {
        String,
        Numeric
    }

    public class PropertyContainer
    {
        public PropertyInfo pi;
        public MemberGetter getter;
    }

    public class SqlitePropertiesAndCommands<T>
    {
        public Type t;
        public String tableName;
        List<PropertyContainer> pis = new List<PropertyContainer>();
        public SqlitePropertiesAndCommands(String tableName,IEnumerable<string> ignoreFields)
        {
            this.t = typeof(T);
            this.tableName = tableName;
            foreach (var c in t.GetProperties().Where(x => ignoreFields.Contains(x.Name) == false))
            {
                pis.Add(new PropertyContainer
                {
                    pi = c,
                    getter = c.DelegateForGetPropertyValue()
                });
            }
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

        public string UpdateColumnValuesCommand(String columnName)
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

        public String SelectCommand()
        {
            StringBuilder b = new StringBuilder();

            b.Append($"select  ");

            foreach (var c in pis)
            {
                b.Append($"{c.pi.Name},");
            }

            TrimComma(b);

            b.Append($"from {tableName}; ");

            return b.ToString();
        }

        public String RangeClause(int start,int end)
        {
            if (end > start)
                return $" (rowid >= {start} and rowid <= {end}) ";
            else
                return $" (rowid >= {start} ";
        }

        public String SortClause(string sortKey,SortTypeEnum sortType)
        {
            return $" sort by {sortKey} {sortType} ";
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

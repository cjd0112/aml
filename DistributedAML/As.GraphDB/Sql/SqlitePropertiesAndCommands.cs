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

    // primary key is first field
    public class SqlitePropertiesAndCommands
    {
        public Type t => typeContainer.UnderlyingType;
        public String tableName => typeContainer.Name;

        public TypeContainer typeContainer;

   
        public SqlitePropertiesAndCommands(TypeContainer typeContainer)
        {
            this.typeContainer = typeContainer;
        }

        public void VerifyForeignKeysFromOtherTables(IEnumerable<SqlitePropertiesAndCommands> alltables)
        {
            foreach (var c in alltables.Where(x => x != this))
            {
                foreach (var g in typeContainer.Properties)
                {
                    if (c.GetPrimaryKeyProperty().Name == g.Name)
                    {
                        g.foreignKey = new ForeignKey {FieldName = g.Name, TableName = c.tableName};
                    }
                }
            }
        }

        public T CreateInstance<T>()
        {
            return (T) typeContainer.CreateInstance<T>();

        }

        public T CreateInstance<T>(ISupportGetValue sv)
        {
            var t = CreateInstance<T>();
            foreach (var q in typeContainer.Properties)
            {
                q.SetValue(t,sv.GetValue(q.pi.Name));
            }
            return (T) t;

        }

        public String GetPrimaryKey(Object obj)
        {
            return (string)GetPrimaryKeyProperty().GetValue(obj);
        }

        public PropertyContainer GetPrimaryKeyProperty()
        {
            return typeContainer.Properties.FirstOrDefault(x => x.IsPrimaryKey);
        }
        
        public IEnumerable<PropertyContainer> SqlFields()
        {
            return typeContainer.Properties;
        }

     

        public string QueryIdCommand()
        {
            return $"select count(*) from { tableName} where {GetPrimaryKeyProperty().Name} = ($primaryKey)";
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

        public string UpdateColumnValuesCommandNumeric(String columnName, Int32 value)
        {
            return $"update {tableName} set {columnName}={value}";
        }



        public string UpdateColumnValuesCommandForId(String columnName)
        {
            return $"update {tableName} set {columnName}=$value where {GetPrimaryKeyProperty().Name}=$primaryKey";
        }

        static void TrimComma(StringBuilder b)
        {
            if (b[b.Length - 1] == ',')
                b.Remove(b.Length - 1, 1);
        }

        public string SelectPrimaryKeyValues()
        {
            return $"select {GetPrimaryKeyProperty().Name} from {tableName};";
        }

        public String InsertOrReplaceCommand(bool includeReplace)
        {
            StringBuilder b = new StringBuilder();

            b.Append("insert ");

            if (includeReplace)
                b.Append(" or replace ");

            b.Append($"into {tableName} ");

            StringBuilder values = new StringBuilder();

            values.Append(" values (");

            StringBuilder names = new StringBuilder();

            names.Append("(");

            foreach (var c in typeContainer.Properties)
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
            return $"delete from {tableName} where {GetPrimaryKeyProperty()} like '{id}'";
        }

        public String SelectCommand()
        {
            StringBuilder b = new StringBuilder();

            b.Append($"select  ");

            foreach (var c in typeContainer.Properties)
            {
                b.Append($"{c.pi.Name},");
            }

            TrimComma(b);

            b.Append($" from {tableName} ");

            return b.ToString();
        }

        public String SelectCommandByPrimaryKey(string primaryKey)
        {
            return SelectCommand() + $" where {GetPrimaryKeyProperty().Name} = '{primaryKey}'; ";
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
            foreach (var c in typeContainer.Properties)
            {
                if (c.IsPrimaryKey)
                    b.Append($"{c.pi.Name} {ConvertPropertyType(c.pi.PropertyType)} primary key,");
                else
                {
                    if (c.foreignKey == null)
                        b.Append($"{c.pi.Name} {ConvertPropertyType(c.pi.PropertyType)},");
                    else
                        b.Append($"{c.pi.Name} {ConvertPropertyType(c.pi.PropertyType)} REFERENCES {c.foreignKey.TableName}({c.foreignKey.FieldName}),");
                }
            }

            if (b[b.Length - 1] == ',')
                b.Remove(b.Length - 1, 1);

            b.Append(");");

            return b.ToString();
        }

        public static String ConvertPropertyType(Type t)
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

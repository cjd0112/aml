using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GraphQLInterface.GraphQLType
{
    public static class TypeCheck
    {
        public static bool IsScalar(Type t)
        {
            return (IsNumeric(t) || IsString(t) || IsNumeric(t) || IsDateTime(t) || IsEnum(t));
        }

        public static bool IsNumeric(Type t)
        {
            return t == typeof(decimal) || t == typeof(float) || t == typeof(int) || t == typeof(ushort) ||
                   t == typeof(short) || t == typeof(uint) || t == typeof(long) || t == typeof(ulong) ||
                   t == typeof(double);
        }

        public static bool IsString(Type t)
        {
            return t == typeof(string);
        }

        public static bool IsClass(Type t)
        {
            return t.IsClass;
        }

        public static bool IsEnum(Type t)
        {
            return t.IsEnum;
        }

        public static bool IsValueType(Type t)
        {
            return t.IsValueType;
        }

        public static bool IsEnumerableType(Type t)
        {
            return typeof(IEnumerable).IsAssignableFrom(t);
        }

        public static bool IsDateTime(Type t)
        {
            return t == typeof(DateTime);
        }

        public static bool IsBoolean(Type t)
        {
            return t == typeof(Boolean);
        }
    }
}

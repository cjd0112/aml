using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared
{
    public static class EnumHelper
    {
        public static T Parse<T>(String s)
        {
            return (T)Enum.Parse(typeof(T), s);
        }

        public static String ListValues(Type t)
        {
            return Enum.GetNames(t).Aggregate("", (x, y) => x + "," + y);
        }

        public static T EnumPrompt<T>()
        {
            if (!typeof(T).IsEnum)
                throw new Exception("EnumPrompt requires an enum type");

            return EnumHelper.Parse<T>(
                Helper.Prompt($"Enter a {typeof(T).Name} - {EnumHelper.ListValues(typeof(T))}"));
        }
    }
}

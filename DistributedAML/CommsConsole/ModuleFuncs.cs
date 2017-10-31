using System;
using System.Collections.Generic;
using System.Text;

namespace CommsConsole
{
    public static class ModuleFuncs
    {
        public static string GetClassName(Type t)
        {
            return t.Name.StartsWith("I") ? t.Name.Substring(1) : t.Name;
        }
    }
}

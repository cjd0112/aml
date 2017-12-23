using System;
using System.Collections.Generic;
using System.Text;

namespace As.Shared
{
    public static class StringHelper
    {
        public static String ConvertCamelCase(this string foo)
        {
            StringBuilder b = new StringBuilder();
            int cnt = 0;
            foreach (var c in foo)
            {
                if (cnt > 0 && char.IsUpper(c) || char.IsDigit(c))
                    b.Append(' ');

                b.Append(c);
                cnt++;
            }
            return b.ToString();
        }
    }
}

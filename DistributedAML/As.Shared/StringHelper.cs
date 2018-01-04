using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace As.Shared
{
    public static class StringHelper
    {
        public static T ToEnum<T>(this string foo)
        {
            return (T) Enum.Parse(typeof(T), foo);
        }
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

        // Colin Dick <colin.dick@btinternet.com> = userName <userPrefix@domain>
        public static (string userName, string userPrefix, string domain) ParseLongEmailString(this string foo)
        {
            if (foo.Contains("<") == false)
                throw new Exception("Invalid email string");

            var userName = foo.Substring(0, foo.IndexOf("<")).TrimEnd();

            var email = foo.Substring(foo.IndexOf("<") + 1);

            email = email.Substring(0, email.Length - 1);

            var userPrefix = email.Substring(0, email.IndexOf("@"));
            var domain = email.Substring(email.IndexOf("@") + 1);

            return (userName, userPrefix,domain);

        }

        public static String ToJSonString(this object o)
        {
            return JsonConvert.SerializeObject(o);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GraphQL.Utilities
{
    public class GraphQlJson
    {
        static JsonConverter[] converters = new JsonConverter[] { new StringEnumConverter() };
        static JsonSerializerSettings settings = new JsonSerializerSettings() { Converters = converters, TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented, ReferenceLoopHandling = ReferenceLoopHandling.Serialize };
        static JsonSerializerSettings missingMemberSettings = new JsonSerializerSettings() { Converters = converters, TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented, ReferenceLoopHandling = ReferenceLoopHandling.Serialize, MissingMemberHandling = MissingMemberHandling.Error };

        public static Object FromJSonString(String s, Type t, bool throwErrorOnMissingMember = false)
        {
            if (throwErrorOnMissingMember)
                return JsonConvert.DeserializeObject(s, t, missingMemberSettings);
            else
                return JsonConvert.DeserializeObject(s, t, settings);
        }
    }
}

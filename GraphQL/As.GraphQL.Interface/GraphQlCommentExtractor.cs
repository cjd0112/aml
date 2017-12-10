using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.XPath;

namespace As.GraphQL.Interface
{
    public class GraphQlCommentExtractor
    {

        Dictionary<string,XPathNavigator> navigatorMap = new Dictionary<string, XPathNavigator>();

        XPathNavigator GetNavigator(Type t)
        {
            var xmlName = t.Assembly.GetName().Name + ".xml";
            if (navigatorMap.ContainsKey(xmlName))
                return navigatorMap[xmlName];
            else
            {
                var directory = Path.GetDirectoryName(t.Assembly.Location);
                var path = Path.Combine(directory, xmlName);
                if (File.Exists(path))
                {
                    XPathDocument doc = new XPathDocument(path);
                    navigatorMap[xmlName] = doc.CreateNavigator();
                }
                else
                {
                    navigatorMap[xmlName] = null;
                }
                return navigatorMap[xmlName];
            }

        }


        public String GetCommentFromXml(PropertyInfo pi)
        {
            var navigator = GetNavigator(pi.DeclaringType);
            if (navigator == null)
                return "";
            var xmlString = $"/doc/members/member[@name=\"P:{pi.DeclaringType}.{pi.Name}\"]/summary/text()";

            var node = navigator.SelectSingleNode(xmlString);

            if (node == null)
                return "";

            return node.Value;
        }

        public String GetCommentFromXml(MethodInfo mi)
        {
            var navigator = GetNavigator(mi.DeclaringType);
            if (navigator == null)
                return "";
            var xmlString = $"/doc/members/member[starts-with(@name,\"M:{mi.DeclaringType}.{mi.Name}\")]/summary/text()";

            var node = navigator.SelectSingleNode(xmlString);

            if (node == null)
                return "";

            return node.Value;
        }

    }
}

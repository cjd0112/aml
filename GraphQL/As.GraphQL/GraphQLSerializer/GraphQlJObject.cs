using System;
using System.Collections.Generic;
using As.GraphQL.Interface;
using Newtonsoft.Json.Linq;

namespace As.GraphQL.GraphQLSerializer
{
    public class GraphQlJObject : IGraphQlOutput
    {
        private JObject root;
        private JContainer current;

        public void AddScalarProperty(string field,object value)
        {
            if (root == null || current == null)
                throw new Exception($"Need to push object first ... processing - {field}");

            current.Add(new JProperty(field, ConvertValue(value)));
        }

        public void PushObject(String name)
        {
            if (root == null)
            {
                root = new JObject();
                current = root;
                
            }
            var obj = new JObject();
            var property = new JProperty(name,obj);
            current.Add(property);                
            current = obj;
        }

        public void PushObject()
        {
            var obj = new JObject();

            if (root == null)
            {
                root = obj;
                current = obj;
            }
            else
            {
                current.Add(obj);
                current = obj;
            }
        }



        public void PushArray(string name)
        {
            if (root == null || current == null)
                throw new Exception($"Need to push object first ... processing - PushArray");

            var obj = new JArray();
            var property = new JProperty(name,obj);
            current.Add(property);
            current = obj;
        }

        public void PushArray()
        {
            if (root == null || current == null)
                throw new Exception($"Need to push object first ... processing - PushArray");

            var obj = new JArray();
            current.Add(obj);
            current = obj;
        }

        public void Pop()
        {
            if (current?.Parent == null && current != root)
                throw new Exception($"error - parent is null on 'Pop' operation and current is not root");

            if (current != root)
            {
                current = current.Parent;
                if (current is JProperty)
                {
                    current = current.Parent;
                }
            }
        }

        public void AddScalarValue(object value)
        {
            if (current is JProperty)
            {
                if (current.HasValues)
                    throw new Exception($"Attempt to add object of type - {value} to {(JProperty)current} which already has a value - {current.First}");

                ((JProperty) current).Value = new JValue(ConvertValue(value));
            }
            else if (current is JArray)
            {
                ((JArray)current).Add(ConvertValue(value));
            }
        }

        public void AddException(IGraphQlException exception)
        {
            var e2 = exception as Exception;
            var obj = new List<Object>();
            obj.Add(new
            {
                message = e2.Message,
                line = exception.Line,
                column = exception.Column,
                stacktrace = e2.StackTrace
            });

            if (e2.InnerException != null)
            {
                obj.Add(new
                {
                    message = e2.InnerException.Message,
                    stacktrace = e2.InnerException.StackTrace
                });
            }

            root = JObject.FromObject(new
            {
                errors = obj
            });
        }


        object ConvertValue(Object value)
        {
            if (value != null && value.GetType().IsEnum)
                return value.ToString();
            return value;
        }

        public object GetRoot()
        {
            return root;
        }
    }
}

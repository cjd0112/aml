using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Comms;
using Google.Protobuf;
using NetMQ;
using Shared;

namespace CommsConsole
{

    /*
        using System;
        using System.Runtime.InteropServices;
        using NetMQ;

namespace Comms
{
     *  
     *  public class FuzzyMatcherClient: IFuzzyMatcher
    {
        private IServiceClient client;
        public FuzzyMatcherClient(IServiceClient client)
        {
            this.client = client;
            client.SetUnderlying(this);
        }
        public string Select(string foo)
        {
            var z = new NetMQMessage();
            z.Append("Select");
            z.Append(foo);
            var ret = client.Send(z);
            return ret.First.ConvertToString();
        }
   }
    */


    public class GenerateCommsClient : GeneratorBase
    {
String outer = @"   
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NetMQ;
using Shared;

namespace Comms
{
    public class _NAME_Client : I_NAME_
    {
        private IServiceClient client;
        public _NAME_Client(IServiceClient client)
        {
            this.client = client;
            this.client.SetUnderlying(this);
        }

        _FUNCTIONS_
    }
}";

        private Type type;

        public GenerateCommsClient(Type type, List<Type> googleTypes, string sourceDirectoryName) : base(
            sourceDirectoryName + "/" + ModuleFuncs.GetClassName(type) + "Client.cs")
        {
            this.type = type;
        }

        public void GenerateFile()
        {
            List<string> methods = new List<string>();
            foreach (var method in type.GetMethods())
            {
                methods.Add(GenerateMethod(method));

                
            }
            
            O(outer.Replace("_NAME_", ModuleFuncs.GetClassName(type)).
                Replace("_FUNCTIONS_", methods.Aggregate("", (x, y) => x + "\n\n" + y)));

            Close();
        }


        String GenerateMethod(MethodInfo method)
        {
            var m = GenerateSignature(method);
            m += "\t\t{\n";
            m += "\t\t\tvar msg = new NetMQMessage();\n";
            m += $"\t\t\tmsg.Append(\"{method.Name}\");\n";

            foreach (var c in method.GetParameters())
            {
                m += $"\t\t\t{GenerateParameter(c)};\n";
            }



            m += $"\t\t\tvar ret = client.Send(msg);\n";
            m += $"\t\t\treturn {GenerateReturn(method.ReturnType)};\n";
            m += "\t\t}";


            return m;
        }

        String GenerateSignature(MethodInfo method)
        {
            var access = "public";
            var ret = "";
            if (typeof(IList).IsAssignableFrom(method.ReturnType))
            {
                ret = $"List<{method.ReturnType.GenericTypeArguments[0].Name}>";
            }
            else
            {
                ret = method.ReturnType.Name;

            }
            var name = method.Name;
            var parameters = method.GetParameters().Select(GenerateSignatureParameter)
                .Select(x => x.Item1 + " " + x.Item2).Aggregate("", (x, y) => x + y + ",");

            if (parameters.Last() == ',')
                parameters = parameters.Substring(0, parameters.Length - 1);

            return $"\t\t{access} {ret} {name}({parameters})\n";
        }

        String GenerateParameter(ParameterInfo pi)
        {
            if (pi.ParameterType == typeof(String) || pi.ParameterType == typeof(Int32))
            {
                return $"msg.Append({pi.Name})";
            }
            else if (typeof(IList).IsAssignableFrom(pi.ParameterType))
            {
                if (pi.ParameterType.GenericTypeArguments[0] == typeof(string))
                {
                    return $"Helpers.PackMessageListString(msg,{pi.Name})";
                }
                else if (pi.ParameterType.GenericTypeArguments[0] == typeof(Int32))
                {
                    return $"Helpers.PackMessageListInt32(msg,{pi.Name})";
                }
                else if (pi.ParameterType.GenericTypeArguments[0] == typeof(Int64))
                {
                    return $"Helpers.PackMessageListInt64(msg,{pi.Name})";
                }
                else if (typeof(IMessage).IsAssignableFrom(pi.ParameterType.GenericTypeArguments[0]))
                {
                    return $"Helpers.PackMessageList<{pi.ParameterType.GenericTypeArguments[0].Name}>(msg,{pi.Name})";
                }


            }

            return "";
        }

        String GenerateReturn(Type returnType)
        {
            var z = new NetMQMessage();
            if (returnType == typeof(String))
            {
                return $"ret.First.ConvertToString()";
            }
            else if (returnType == typeof(Int32))
            {
                return $"ret.First.ConvertToInt32()";
            }
            else if (returnType == typeof(Boolean))
            {
                return $"ret.First.ConvertToInt32() >0 ? true:false";
            }
            else if (typeof(IList).IsAssignableFrom(returnType))
            {
                if (typeof(IMessage).IsAssignableFrom(returnType.GenericTypeArguments[0]))
                {
                    return
                        $"Helpers.UnpackMessageList(ret, {returnType.GenericTypeArguments[0].Name}.Parser.ParseDelimitedFrom);";
                }
                else if (returnType.GenericTypeArguments[0] == typeof(String))
                {
                    return $"Helpers.UnpackMessageListString(ret);";
                }
                else if (returnType.GenericTypeArguments[0] == typeof(Int32))
                {
                    return $"Helpers.UnpackMessageListInt32(ret);";

                }
                else if (returnType.GenericTypeArguments[0] == typeof(Int64))
                {
                    return $"Helpers.UnpackMessageListInt64(ret);";
                }
            }
            return "";
        }

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using As.Comms;
using Google.Protobuf;
using NetMQ;
using As.Shared;

namespace CommsConsole
{

    

    public class GenerateCommsClient : GeneratorBase
    {
String outer = @"   
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NetMQ;
using As.Shared;

namespace As.Comms.ClientServer
{
    public class _NAME_Client : I_NAME_
    {
        protected IServiceClient client;
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
            sourceDirectoryName + "/ClientServer/" + ModuleFuncs.GetClassName(type) + "Client.cs")
        {
            Console.WriteLine($"Generating - {type.Name} CommsClient");
            this.type = type;
        }

        public void GenerateFile()
        {
            List<string> methods = new List<string>();
            foreach (var method in type.GetMethods())
            {
                Console.WriteLine($"Generating - {method.Name}");

                methods.Add(GenerateMethod(method));

                
            }
            
            O(outer.Replace("_NAME_", ModuleFuncs.GetClassName(type)).
                Replace("_FUNCTIONS_", methods.Aggregate("", (x, y) => x + "\n\n" + y)));

            Close();
        }

        String GenerateCallingArguments(MethodInfo i)
        {
            return i.GetParameters().Aggregate(",", (x, y) => x + y.Name + ",").TrimEnd(',');
        }

        enum MethodType
        {
            SupportMultiplePackets,
            SupportSingleTransaction
            
        }

        MethodType GetMethodType(MethodInfo method)
        {
            if (method.GetParameters().Any() && IsObjectEnumerable(method.GetParameters().First().ParameterType))
                return MethodType.SupportMultiplePackets;
            else
                return MethodType.SupportSingleTransaction;
        }



        String GenerateMethod(MethodInfo method)
        {
            var m = GenerateSignature(method);
            m += "\t\t{\n";

            if (GetMethodType(method) == MethodType.SupportMultiplePackets)
            {
                if (method.ReturnType == typeof(Int32))
                {
                    m +=
                        $"\t\t\treturn client.SendEnumerableIntResult<{method.GetParameters()[0].ParameterType.GenericTypeArguments[0].Name}>(\"{method.Name}\"{GenerateCallingArguments(method)});\n";
                }
                else
                {
                    m +=
                        $"\t\t\treturn client.SendEnumerableListResult<{method.GetParameters()[0].ParameterType.GenericTypeArguments[0].Name},{method.ReturnType.GenericTypeArguments[0].Name}>(\"{method.Name}\",{method.ReturnType.GenericTypeArguments[0].Name}.Parser.ParseDelimitedFrom{GenerateCallingArguments(method)});\n";
                }
            }
            else
            {
                if (method.GetParameters().Any())
                {
                    m +=
                        $"\t\t\treturn client.Send<{method.GetParameters()[0].ParameterType.Name},{method.ReturnType.Name}>(\"{method.Name}\",{method.ReturnType.Name}.Parser.ParseDelimitedFrom{GenerateCallingArguments(method)});\n";
                }
                else
                {
                    m +=
                        $"\t\t\treturn client.Send<{method.ReturnType.Name}>(\"{method.Name}\",{method.ReturnType.Name}.Parser.ParseDelimitedFrom{GenerateCallingArguments(method)});\n";

                }

            }

            m += "\t\t}\n";

            return m;
        }

        String GenerateSignature(MethodInfo method)
        {
            var access = "public";
            var ret = "";
            if (GetMethodType(method) == MethodType.SupportMultiplePackets)
            {
                if (IsObjectEnumerable(method.ReturnType))
                {
                    if (!typeof(IMessage).IsAssignableFrom(method.ReturnType.GenericTypeArguments[0]))
                        throw new Exception(
                            $"Only support list of return values that are inherited from IMessage - received {method.ReturnType.GenericTypeArguments[0].Name} for {method.Name}");

                    ret = $"IEnumerable<{method.ReturnType.GenericTypeArguments[0].Name}>";
                }
                else
                {
                    if (method.ReturnType != typeof(Int32))
                        throw new Exception(
                            $"Only support Int32 as scalar return values on methods that are sent with multiple packets - (have IEnumerable<IMessage> as first parameter) - processing {method.Name}");

                    ret = method.ReturnType.Name;

                }
            }
            else
            {
                if (!IsSupportedObject(method.ReturnType))
                    throw new Exception(
                        $"Only support IMessage types as return type on methods that are sent in one packet - processing {method.Name}.");
                ret = method.ReturnType.Name;
            }
            var name = method.Name;
            var parameters = method.GetParameters().Select(GenerateSignatureParameter)
                .Select(x => x.Item1 + " " + x.Item2).Aggregate("", (x, y) => x + y + ",");

            if (parameters.Any() && parameters.Last() == ',')
                parameters = parameters.Substring(0, parameters.Length - 1);

            return $"\t\t{access} {ret} {name}({parameters})\n";
        }

    }
}

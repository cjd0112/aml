using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using Google.Protobuf;

namespace CommsConsole
{
    public class GeneratorBase
    {
        private StreamWriter sw;
        public GeneratorBase(String fileName)
        {
            sw = new StreamWriter(new FileStream(fileName, FileMode.Create));
        }

        public void O(String s)
        {
            sw.Write(s);
        }

        public void L(String s)
        {
            sw.WriteLine(s);
        }

        public void Close()
        {
            sw.Flush();
            sw.Dispose();
        }

        protected (string, string) GenerateSignatureParameter(ParameterInfo pi)
        {
            if (pi.ParameterType == typeof(String) || pi.ParameterType == typeof(Int32) || pi.ParameterType == typeof(Int64) || pi.ParameterType.IsEnum)
            {
                return (pi.ParameterType.Name, pi.Name);
            }
            else if (typeof(IEnumerable).IsAssignableFrom(pi.ParameterType))
            {
                if (!typeof(IMessage).IsAssignableFrom(pi.ParameterType.GenericTypeArguments[0]))
                    throw new Exception($"Only support list parameter types that support 'IMessage', received - {pi.ParameterType.GenericTypeArguments[0].Name}");

                return ("IEnumerable<" + pi.ParameterType.GenericTypeArguments[0].Name + ">", pi.Name);
            }
            else if (typeof(IMessage).IsAssignableFrom(pi.ParameterType))
            {
                return (pi.ParameterType.Name,pi.Name);
            }
            throw new Exception($"Unsupported parameter type -{pi.ParameterType.Name} for {pi.Name}");

        }
    }
}

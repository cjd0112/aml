using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Input;
using Comms;
using NetMQ;
using NetMQ.Sockets;
using Xunit;

namespace CommsConsole
{
    class Program
    {
        static void Main(string[] args2)
        {
            // update the google objects
            var z2 = Directory.GetCurrentDirectory();
            foreach (var c in Directory.EnumerateFiles("../Comms/","*.proto"))
            {
                var f = new FileInfo(c);
                var parameters = $"--csharp_out=../Comms --proto_path=../Comms {f.Name}";
                System.Diagnostics.Process.Start("protoc.exe",parameters);
            }

            // find the comms types
            List<Type> commsTypes = new List<Type>();
            var ass = Assembly.GetAssembly(typeof(ICommsContract));
            foreach (var type in ass.GetExportedTypes().Where(x=>typeof(ICommsContract).IsAssignableFrom(x) && x != typeof(ICommsContract) && x.IsInterface))
            {
                commsTypes.Add(type);
            }

            // find the google objects
            List<Type> googleList = new List<Type>();
            foreach (var type in ass.GetExportedTypes().Where(x=>typeof(Google.Protobuf.IMessage).IsAssignableFrom(x)))
            {
                googleList.Add(type);
            }

            foreach (var c in commsTypes)
            {
                var foo = new GenerateComms(c,googleList,"../Comms");
                foo.GenerateFiles();
            }



            Console.ReadLine();

        }


    }
}

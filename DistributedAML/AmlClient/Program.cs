using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AmlClient.AS.Application;
using AmlClient.Commands;
using Comms;
using CsvHelper;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;
using Logger;
using NetMQ;
using Newtonsoft.Json;
using Shared;
using StructureMap;

namespace AmlClient
{
    class Program
    {
        public class Query
        {
            public IEnumerable<Transaction> Transactions { get; set; }
            public IEnumerable<Transaction> Accounts{ get; set; }

        }

        static void Main(string[] args)
        {

            bool IncludeProperty(PropertyInfo i)
            {
                if (typeof(IMessage).IsAssignableFrom(i.DeclaringType))
                {
                    if (i.PropertyType.Name.StartsWith("MessageParser") ||
                        i.PropertyType.Name == "MessageDescriptor")
                        return false;
                }

                return true;
            }


            String userCommand = "";
            bool initialized  = false;
            Container c = null;
            while (userCommand != "q")
            {
                try
                {
                    if (!initialized)
                    {
                        try
                        {

                            var reg = new MyRegistry(args.Any() == false ? Helper.GetPlatform().ToString() : args[0]);

                            reg.For<MyRegistry>().Use(reg);
                            c = new Container(reg);
                            c.Inject(typeof(Container),c);

                            var init = c.GetInstance<Initialize>();
                            init.Run();

                            c.Inject<Initialize>(init);

                            initialized = true;

                        }
                        catch (Exception e)
                        {
                            L.Trace(e.Message);
                            if (e.InnerException != null)
                            {
                                L.Trace($"Inner exception is {e.InnerException.Message}");
                                L.Trace($"{e.InnerException.StackTrace}");

                            }
                            L.Trace("Error on initialization ... quitting");
                            userCommand = "q";
                            continue;
                        }
                    }

                    Console.WriteLine("Enter a command - (l) to list");

                    userCommand = Console.ReadLine();
                    if (userCommand == "q")
                        continue;
                    if (userCommand.ToLower() == "l")
                    {
                        Console.Write(AmlCommand.GetCommands().Aggregate("", (x, y) => x + y.Name + "\n"));
                    }
                    else
                    {
                        var commandType = AmlCommand.GetAmlCommand(userCommand);

                        if (commandType == null)
                            throw new Exception($"Command not found - {userCommand}");

                        AmlCommand.RunCommand(c, commandType);

                    }

                }
                catch (Exception e)
                {
                    L.Exception(e);
                }
            }
            L.CloseLog();
            Console.WriteLine("Any key to quit ...");
            Console.ReadLine();

        }
    }
}

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
using As.Comms;
using As.GraphQL;
using CsvHelper;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;
using GraphQL;
using As.Logger;
using As.Shared;
using NetMQ;
using Newtonsoft.Json;
using StructureMap;

namespace AmlClient
{
    class Program
    {
        static void Main(string[] args)
        {
            String userCommand = "";
            Container c = null;

            try
            {
                Console.WriteLine("Initializing Client ...");
                As.Client.Initialize.Startup();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                if (e.InnerException != null)
                    Console.WriteLine(e.InnerException);
                Console.ReadLine();
                return;
            }

            while (userCommand != "q")
            {
                try
                {

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

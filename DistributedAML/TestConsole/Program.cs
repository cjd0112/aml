using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using As.Logger;
using Fasterflect;
using GraphQL;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TestConsole
{
    class Program
    {

        public class myCat
        {
            public String cat { get; set; }
        }

        static void Main(string[] args)
        {
            
            try
            {
                var definition = new
                {
                    data = new
                    {
                        MYLIST = new List<myCat>()
                    }
                };
                
                StreamReader r = new StreamReader("file1.json");
                var z =  JsonConvert.DeserializeAnonymousType(r.ReadToEnd(),definition);
                var cat = z.data.MYLIST[0];

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.ReadLine();
        }
    }


    public static class Ext2
    {
        public static int GetFoo(this Account blah, String foo2)
        {
            return 100;
        }
    }
}

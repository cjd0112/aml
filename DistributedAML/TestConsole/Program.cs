using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Fasterflect;
using GraphQL;

namespace TestConsole
{
    class Program
    {
      

        static void Main(string[] args)
        {
            try
            {

                var x = new Account();

                int z = x.GetFoo("shag");

                var t = typeof(Account);

                foreach (MethodInfo c in typeof(Ext2).GetMethods())
                {
                    if (c.GetCustomAttribute<ExtensionAttribute>() != null)
                    {
                        var p = c.GetCustomAttribute<ExtensionAttribute>();
                        foreach (ParameterInfo pi in c.GetParameters())
                        {
                            var z22 = pi;
                        }


                        var obj = c.Invoke(null, new object[] {x, "test"});

                    }

                }




                Console.WriteLine("Hello World!");

                var query = new StreamReader("SchemaQuery.txt").ReadToEnd();
                /*
                var output = new GraphQlDocument(query)
                    .Validate(typeof(Query))
                    .Run(new Query())
                    .GetOutput();
                    */
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

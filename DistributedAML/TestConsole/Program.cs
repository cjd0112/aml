using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using GraphQL;

namespace TestConsole
{
    class Program
    {
      

        static void Main(string[] args)
        {
            try
            {
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
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using GraphQL;
using GraphQLInterface;

namespace TestConsole
{
    class Program
    {
        public class SubObject
        {
            public enum Blah
            {
                Blah1,
                Blah2,
            }

            public String TEst2 { get; set; }
            public Blah Foo { get; set; }
        }
        public class Query
        {
            public Query()
            {
                foo = new List<SubObject>();
                foo.Add(new SubObject {Foo = SubObject.Blah.Blah1, TEst2 = "hallo flldldl"});
                foo.Add(new SubObject { Foo = SubObject.Blah.Blah1, TEst2 = "hallo flldldl" });
                foo.Add(new SubObject { Foo = SubObject.Blah.Blah1, TEst2 = "hallo flldldl" });

            }
            public List<SubObject> foo { get; set; }


        }

        public class Range
        {
            public int Start { get; set; }
            public int PageSize { get; set; }
            public RangeType RangeType { get; set; }
        }

        public enum SortTypeEnum
        {
            None,
            Ascending,
            Descending
        }

        public class Sort
        {
            public String SortField { get; set; }
            public SortTypeEnum SortType { get; set; }

        }

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Hello World!");

                var query = new StreamReader("SchemaQuery.txt").ReadToEnd();

                SchemaLoader.InitializeSchema(typeof(Query));

                var schema = SchemaLoader.GetSchema(typeof(Query));

                var output = new GraphQlDocument(query)
                    .Validate(schema)
                    .Run(new Query())
                    .GetOutput();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.ReadLine();
        }
    }
}

﻿namespace As.GraphQL.GraphQLType
{
    public class __SchemaContainer 
    {
        public __SchemaContainer(__Schema s)
        {
            __schema = s;
        }
        public __Schema __schema { get; set; }

        internal __Type GetType(string name)
        {
            return __schema.GetType(name);
        }
    }
}

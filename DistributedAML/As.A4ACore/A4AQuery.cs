using As.GraphQL;

namespace As.A4ACore
{

    public class A4AQuery 
    {
        public A4AQuery(A4ARepositoryQuery query,A4AMutations mutations)
        {
            Query = query;
            Mutation = mutations;

        }
        [GraphQlTopLevelQuery]
        public A4ARepositoryQuery Query { get; set; }

        [GraphQlMutations]
        public A4AMutations Mutation { get; set; }
    }
}
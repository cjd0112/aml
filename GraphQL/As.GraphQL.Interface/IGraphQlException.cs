namespace As.GraphQL.Interface
{
    public interface IGraphQlException
    {
        int Line { get; }
        int Column { get; }

        
    }
}

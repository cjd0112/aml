using System.Collections.Generic;
using Antlr4.Runtime.Misc;

namespace GraphQL
{
    public class ModifiableSelectionSet
    {
        public List<GraphQLParser.SelectionContext> selections = new ArrayList<GraphQLParser.SelectionContext>();

        public ModifiableSelectionSet()
        {

        }

        public ModifiableSelectionSet(GraphQLParser.SelectionSetContext sc)
        {
            selections.AddRange(sc.selection());
        }

        public void AddSelection(GraphQLParser.SelectionContext sel)
        {
            selections.Add(sel);
        }
    }
}

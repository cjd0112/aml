using System;
using System.Collections.Generic;
using System.Text;

namespace Comms
{
    public interface IFuzzyMatcher : ICommsContract
    {
        Boolean AddEntry(IEnumerable<FuzzyWordEntry> entries);
        IEnumerable<FuzzyQueryResponse> FuzzyQuery(IEnumerable<String> phrases);
    }
}

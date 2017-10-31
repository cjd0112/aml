using System;
using System.Collections.Generic;
using System.Text;

namespace Comms
{
    public interface IFuzzyMatcher : ICommsContract
    {
        Boolean AddEntry(List<FuzzyWordEntry> entries);
        List<FuzzyQueryResponse> FuzzyQuery(List<String> phrases);
    }
}

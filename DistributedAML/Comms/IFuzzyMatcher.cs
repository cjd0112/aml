using System;
using System.Collections.Generic;
using System.Text;

namespace Comms
{
    public interface IFuzzyMatcher : ICommsContract
    {
        int AddEntry(IEnumerable<FuzzyWordEntry> entries);
        IEnumerable<FuzzyQueryResponse> FuzzyQuery(IEnumerable<FuzzyCheck> phrases);
    }
}

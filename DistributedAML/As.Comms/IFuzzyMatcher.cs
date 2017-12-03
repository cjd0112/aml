using System.Collections.Generic;

namespace As.Comms
{
    public interface IFuzzyMatcher : ICommsContract
    {
        int AddEntry(IEnumerable<FuzzyWordEntry> entries);
        IEnumerable<FuzzyQueryResponse> FuzzyQuery(IEnumerable<FuzzyCheck> phrases);
    }
}

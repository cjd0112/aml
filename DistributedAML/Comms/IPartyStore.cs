using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf.Reflection;

namespace Comms
{
    public enum LinkageDirection
    {
        AccountToParty,
        PartyToAccount
    }
    public interface IPartyStore : ICommsContract
    {
        int StoreParties(IEnumerable<Party> parties);
        int StoreAccounts(IEnumerable<Account> accounts);

        int StoreLinkages(IEnumerable<AccountToParty> mappings,LinkageDirection direction);

        IEnumerable<AccountToParty> GetLinkages(IEnumerable<string> source, LinkageDirection direction);

    }
}

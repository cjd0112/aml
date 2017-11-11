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
        int StoreParties(List<Party> parties);
        int StoreAccounts(List<Account> accounts);

        int StoreLinkages(List<AccountToParty> mappings,LinkageDirection direction);

    }
}

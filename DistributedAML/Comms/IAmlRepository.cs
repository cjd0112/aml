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

    public interface IAmlRepository : ICommsContract
    {
        int StoreParties(IEnumerable<Party> parties);
        int StoreAccounts(IEnumerable<Account> accounts);

        int StoreTransactions(IEnumerable<Transaction> transactions);

        int StoreLinkages(IEnumerable<AccountToParty> mappings,LinkageDirection direction);

        IEnumerable<AccountToParty> GetLinkages(IEnumerable<Identifier> source, LinkageDirection direction);

        IEnumerable<YesNo> AccountsExist(IEnumerable<Identifier> account);

        //GraphResponse GraphQuery(GraphQuery query);

    }
}

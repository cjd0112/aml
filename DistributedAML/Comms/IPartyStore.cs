using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf.Reflection;

namespace Comms
{
    public interface IPartyStore : ICommsContract
    {
        int StoreHomeParties(List<Party> parties);
        int StoreHomeAccounts(List<Account> accounts);

        int StoreHomeAccountToPartyMapping(List<AccountToPartyMapping> mappings);
        int StoreHomePartyToAccountMapping(List<AccountToPartyMapping> mappings);

        List<AccountToPartyMapping> GetPartiesForHomeAccounts();
        List<(Party, Account)> GetAccountsForHomeParties();

    }
}

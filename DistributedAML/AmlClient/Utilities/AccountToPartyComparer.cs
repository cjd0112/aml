using System;
using System.Collections.Generic;
using System.Text;
using As.Comms;

namespace AmlClient.Utilities
{
    public class AccountToPartyComparer : IComparer<AccountToParty>
    {
        private LinkageDirection dir;
        public AccountToPartyComparer(LinkageDirection dir)
        {
            this.dir = dir;

        }

        public int Compare(AccountToParty x, AccountToParty y)
        {
            if (dir == LinkageDirection.AccountToParty)
            {
                return (x.AccountId.CompareTo(y.AccountId));
            }
            else
            {
                return (x.PartyId.CompareTo(y.PartyId));
            }
        }
    }
}

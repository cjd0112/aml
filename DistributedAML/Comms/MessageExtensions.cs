using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf;

namespace Comms
{
    public static class MessageExtensions
    {
        public static String GetMatchKey(this IMessage msg)
        {
            String matchKey = "";
            switch (msg)
            {
                case Party foo:
                {
                    switch (foo.Type)
                    {
                        case Party.Types.PartyType.Retail:
                            matchKey = foo.Name + foo.Address1 + foo.PostCode;
                            break;
                        case Party.Types.PartyType.Corporate:
                            matchKey = foo.CompanyName + foo.Address1 + foo.PostCode;
                            break;
                        case Party.Types.PartyType.FinancialInstitution:
                            matchKey = foo.CompanyName + foo.Address1 + foo.PostCode;
                            break;
                        default:
                            throw new Exception($"Unexpected party type - {foo.Type}");
                    }
                }
                break;
                case Account foo:
                {
                    matchKey = foo.AccountNo + foo.SortCode + foo.IBAN;
                }
                break;
                default:
                    throw new Exception($"Unexpected message type - {msg.GetType().Name}");
            }
            if (matchKey == "")
                throw new Exception($"Blank matchkey found for object - {msg} of type {msg.GetType().Name}");
            return matchKey;
        }
    }
}

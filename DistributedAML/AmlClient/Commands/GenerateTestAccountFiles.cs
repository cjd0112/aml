using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AmlClient.AS.Application;
using Comms;
using CsvHelper;
using Logger;
using Shared;

namespace AmlClient.Commands
{
    public class GenerateTestAccountFiles : AmlCommand
    {
        private Party.Types.PartyType partyType;
        private int numAccountsPerCustomer;
        private int numCustomersPerAccount;
        private MyRegistry reg;
        public GenerateTestAccountFiles(MyRegistry reg)
        {
            Console.WriteLine("Enter type of accounts - (Retail,Co+rporate,FinancialInstitution)...");
            partyType = Enum.Parse<Party.Types.PartyType>(Console.ReadLine());
            Console.WriteLine("Enter number of accounts per customer: ");
            numAccountsPerCustomer = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter number of customers per account: ");
            numCustomersPerAccount = int.Parse(Console.ReadLine());
            this.reg = reg;
        }

        (string, string, string ) GetPartyFiles(Party.Types.PartyType type)
        {
            (string, string, string) ret = ("", "", "");
            switch (partyType)
            {
                case Party.Types.PartyType.Retail:
                    ret =($"{reg.DataDirectory}\\input\\Retail-Large.csv",
                        $"{reg.DataDirectory}\\input\\RetailAccounts.csv",
                        $"{reg.DataDirectory}\\input\\RetailAccountsLink.csv");
                    break;

                default:
                    throw new Exception($"Unexpected party type - {partyType}");
            }
            return ret;
        }


        String GetNextSortCode( int i)
        {
            String s = "";
            switch (i % 3)
            {
                case 0:
                    s = "80-45-02";
                    break;
                case 1:
                    s = "90-01-23";
                    break;
                case 2:
                    s = "60-67-88";
                    break;
            }
            return s;
        }
        String GetNextCurrency(int i)
        {
            String s = "";
            switch (i % 3)
            {
                case 0:
                    s = "USD";
                    break;
                case 1:
                    s = "GBP";
                    break;
                case 2:
                    s = "EUR";
                    break;
            }
            return s;
        }

        public override void Run()
        {
            L.Trace("Generate Test accounts started");

            (var dataFile, var outAccountsFile, var outAccountsLinkFile) = GetPartyFiles(partyType);

            L.Trace($"starting load of - {dataFile}");

            Random rnd = new Random(DateTime.Now.Millisecond);
            int cnt = 0;

            using (CsvReader rdr = new CsvReader(new StreamReader(dataFile)))
            { 
                rdr.Configuration.HeaderValidated = null;

                rdr.Configuration.MissingFieldFound = null;

                using (CsvWriter accWriter = new CsvWriter(new StreamWriter(outAccountsFile)))
                {
                    accWriter.WriteHeader<Account>();
                    accWriter.NextRecord();
                    using (CsvWriter accLinkage = new CsvWriter(new StreamWriter(outAccountsLinkFile)))
                    {
                        accLinkage.WriteHeader<AccountToParty>();
                        accLinkage.NextRecord();
                        var records = rdr.GetRecords<Party>();
                        records.Do(x =>
                        {
                            for (int i = 0; i < numAccountsPerCustomer; i++)
                            {
                                var Acc = new Account
                                {
                                    AccountNo = rnd.Next(10_000_000,99_999_999).ToString("D8").ToString(),
                                    SortCode = GetNextSortCode(cnt),
                                    Currency = GetNextCurrency(cnt)
                                };

                                Acc.Id = Acc.AccountNo + "-" + Acc.SortCode;

                                accWriter.WriteRecord<Account>(Acc);
                                accWriter.NextRecord();
                                var linkCustomerAccounts = new AccountToParty
                                {
                                    PartyId = x.Id,
                                    AccountId = Acc.Id
                                };

                                accLinkage.WriteRecord(linkCustomerAccounts);
                                accLinkage.NextRecord();
                            }
                            cnt++;
                            if (cnt % 10000 == 0)
                            {
                                L.Trace($"Processed {cnt} parties ");
                            }

                        });
                    }
                }
            }
            
        }
    }
}

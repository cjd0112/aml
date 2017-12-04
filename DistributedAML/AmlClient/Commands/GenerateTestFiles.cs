using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AmlClient.AS.Application;
using As.Comms;
using CsvHelper;
using As.Logger;
using As.Shared;

namespace AmlClient.Commands
{
    public class GenerateTestFiles : AmlCommand
    {
        private MyRegistry reg;

        private int TransactionsPerRetailCustomer;

        public GenerateTestFiles(MyRegistry reg)
        {
            this.reg = reg;
            TransactionsPerRetailCustomer = Int32.Parse(As.Shared.Helper.Prompt("Number of transactions per customer"));
        }

        string GetInputPathForParty(Party.Types.PartyType type)
        {
            return $"{reg.DataDirectory}/input/{type}{LoadFromCSV.DataType.Party}.csv";
        }

        (string, string) GetAccountsFiles()
        {
            return ($"{reg.DataDirectory}/input/Accounts.csv", $"{reg.DataDirectory}/input/AccountToParty.csv");
        }

        string GetTransactionsFile()
        {
            return $"{reg.DataDirectory}/input/Transactions.csv";
        }

        string GetTransactionRolesFile()
        {
            return $"{reg.DataDirectory}/input/TransactionRoles.csv";
        }

        String GetNextSortCode(Account.Types.AccountType tp)
        {
            String s = "";
            switch (tp)
            {
                case Account.Types.AccountType.Corporate:
                    s = "99-99-99";
                    break;
                case Account.Types.AccountType.Retail:
                    s = "88-88-88";
                    break;
                case Account.Types.AccountType.Vostro:
                    s = "77-77-77";
                    break;
            }
            return s;
        }

        public override void Run()
        {
            L.Trace("Generate Test accounts started");

            L.Trace($"starting load of financial institutions - vostros");

            var fis = GetInputPathForParty(Party.Types.PartyType.FinancialInstitution);

            Random rnd = new Random(DateTime.Now.Millisecond);
            int cnt = 0;

            List<Account> fiAccounts = new List<Account>();

            List<Account> retailAccounts = new List<Account>();

            List<Account> corporateAccounts = new List<Account>();

            using (CsvReader rdr = new CsvReader(new StreamReader(fis)))
            {
                rdr.Configuration.HeaderValidated = null;
                rdr.Configuration.MissingFieldFound = null;

                var records = rdr.GetRecords<Party>();
                records.Do(x =>
                {
                    var acc = new Account
                    {
                        AccountNo = rnd.Next(10_000_000, 99_999_999).ToString("D8"),
                        Currency = "USD",
                        SortCode = GetNextSortCode(Account.Types.AccountType.Vostro),
                        Type = Account.Types.AccountType.Vostro,
                        IBAN = "GB29NWBK60161331926819",
                    };
                    acc.Id = acc.SortCode + acc.AccountNo;
                    fiAccounts.Add(acc);

                    acc.Parties.Add(x);
                });
            }

            L.Trace($"Generating accounts files and account-party linkage files");

            (var outAccountsFile, var outAccountsLinkFile) = GetAccountsFiles();

            using (CsvWriter accWriter = new CsvWriter(new StreamWriter(outAccountsFile)))
            {
                accWriter.WriteHeader<Account>();

                accWriter.NextRecord();

                using (CsvWriter accLinkage = new CsvWriter(new StreamWriter(outAccountsLinkFile)))
                {
                    accLinkage.WriteHeader<AccountToParty>();
                    accLinkage.NextRecord();

                    foreach (var c in fiAccounts)
                    {
                        accWriter.WriteRecord<Account>(c);
                        accWriter.NextRecord();

                        accLinkage.WriteRecord<AccountToParty>(new AccountToParty
                        {
                            AccountId = c.Id,
                            PartyId = c.Parties[0].Id
                        });
                    }

                    // load retail customers
                    using (CsvReader rdr =
                        new CsvReader(new StreamReader(GetInputPathForParty(Party.Types.PartyType.Retail))))
                    {
                        rdr.Configuration.HeaderValidated = null;
                        rdr.Configuration.MissingFieldFound = null;

                        var records = rdr.GetRecords<Party>();

                        records.Do(x =>
                        {
                            var Acc = new Account
                            {
                                AccountNo = rnd.Next(10_000_000, 99_999_999).ToString("D8").ToString(),
                                SortCode = GetNextSortCode(Account.Types.AccountType.Retail),
                                Currency = "USD",
                                Type = Account.Types.AccountType.Retail,
                                Name=x.Name
                            };
                            Acc.Id = Acc.SortCode + Acc.AccountNo;

                            retailAccounts.Add(Acc);

                            accWriter.WriteRecord<Account>(Acc);
                            accWriter.NextRecord();
                            var linkCustomerAccounts = new AccountToParty
                            {
                                PartyId = x.Id,
                                AccountId = Acc.Id
                            };
                            accLinkage.WriteRecord(linkCustomerAccounts);
                            accLinkage.NextRecord();
                            cnt++;

                            if (cnt % 100_000 == 0)
                            {
                                L.Trace($"Processed {cnt} parties ");
                            }
                        });
                    }

                    // load corporate customers
                    using (CsvReader rdr =
                        new CsvReader(new StreamReader(GetInputPathForParty(Party.Types.PartyType.Corporate))))
                    {
                        rdr.Configuration.HeaderValidated = null;
                        rdr.Configuration.MissingFieldFound = null;

                        var records = rdr.GetRecords<Party>();

                        records.Do(x =>
                        {
                            var Acc = new Account
                            {
                                AccountNo = rnd.Next(10_000_000, 99_999_999).ToString("D8").ToString(),
                                SortCode = GetNextSortCode(Account.Types.AccountType.Corporate),
                                Currency = "USD",
                                Type = Account.Types.AccountType.Corporate,
                                Name=x.CompanyName
                            };
                            Acc.Id = Acc.SortCode + Acc.AccountNo;

                            corporateAccounts.Add(Acc);
                            accWriter.WriteRecord<Account>(Acc);
                            accWriter.NextRecord();
                            var linkCustomerAccounts = new AccountToParty
                            {
                                PartyId = x.Id,
                                AccountId = Acc.Id
                            };
                            accLinkage.WriteRecord(linkCustomerAccounts);
                            accLinkage.NextRecord();
                            cnt++;

                            if (cnt % 100_000 == 0)
                            {
                                L.Trace($"Processed {cnt} parties ");
                            }
                        });
                    }
                }
            }

            Account RandomFi()
            {
                return fiAccounts[rnd.Next(0, fiAccounts.Count - 1)];
            }

            Account RandomRetail()
            {
                return retailAccounts[rnd.Next(0, retailAccounts.Count - 1)];
            }

            TransactionRole NewRole(Account a,TransactionRole.Types.RoleType r2)
            {
                var r = new TransactionRole();
                r.Account = a.AccountNo;
                r.SortCode = a.SortCode;
                r.Type = r2;
                return r;
            }

            TransactionRole NewPseudoRole(Account a, String randomName, TransactionRole.Types.RoleType r2)
            {
                var r = new TransactionRole();
                r.Account = rnd.Next(10_000_000, 99_999_999).ToString("D8");
                r.SortCode = a.Parties[0].FiSortCode;
                r.Type = r2;
                r.Name = randomName + "_PSEUDO";
                return r;
            }


            using (CsvWriter txnWriter = new CsvWriter(new StreamWriter(GetTransactionsFile())))
            {
                txnWriter.WriteHeader<Transaction>();
                txnWriter.NextRecord();

                using (CsvWriter txnRoleWriter = new CsvWriter(new StreamWriter(GetTransactionRolesFile())))
                {
                    txnRoleWriter.WriteHeader<TransactionRole>();
                    txnRoleWriter.NextRecord();
                    foreach (var c in retailAccounts)
                    {
                        for (int i = 0; i < TransactionsPerRetailCustomer; i++)
                        {
                            var txn = new Transaction
                            {
                                Amount = 100,
                                Id = Guid.NewGuid().ToString(),
                                Type = Transaction.Types.TransactionType.Type1,
                            };
                            txn.Roles.Add(NewRole(c, TransactionRole.Types.RoleType.Beneficiary));
                            var fi = RandomFi();
                            txn.Roles.Add(NewRole(fi, TransactionRole.Types.RoleType.Intermediary1));
                            var re = RandomRetail();
                            txn.Roles.Add(NewPseudoRole(fi, re.Name, TransactionRole.Types.RoleType.Originator));
                            txnWriter.WriteRecord(txn);
                            txnWriter.NextRecord();

                            foreach (var g in txn.Roles)
                            {
                                g.TxnId = txn.Id;

                                txnRoleWriter.WriteRecord(g);
                                txnRoleWriter.NextRecord();
                            }
                        }
                    }
                }
            }
        }
    }
}

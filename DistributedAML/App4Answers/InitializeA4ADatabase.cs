using As.A4ACore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App4Answers
{
    public class InitializeA4ADatabase
    {
        A4ARepository repository;
        public InitializeA4ADatabase(A4ARepository repository)
        {
            this.repository = repository;
        }

        public bool IsInitialized()
        {
            return repository.Count<A4ACompany>() > 0;
        }

        public void Initialize()
        {

            foreach (var c in new A4AAuthenticationAccount[]{ new A4AAuthenticationAccount
                {
                    Email = "colin.dick1@btinternet.com",
                    Code1 = 1,
                    Code2 = 2,
                    Code3 = 3,
                    Code4 = 4,
                    UserType = A4AUserType.Expert,
                    Name="EXPERT_0000"
                },
                new A4AAuthenticationAccount
                {
                    Email = "colin.dick@alphastorm.co.uk",
                    Code1 = 1,
                    Code2 = 2,
                    Code3 = 3,
                    Code4 = 4,
                    UserType = A4AUserType.User,
                    Name="USER_0000"

                },
                new A4AAuthenticationAccount
                {
                    Email = "admin@a4a.com",
                    Code1 = 1,
                    Code2 = 2,
                    Code3 = 3,
                    Code4 = 4,
                    UserType = A4AUserType.Admin,
                    Name="ADMIN_0000"

                }
            })
            {
                repository.AddObject(c);
            }
            var company = repository.AddObject<A4ACompany>(new A4ACompany
            {
                CompanyName = "Test Company",
            });

            repository.AddObject<A4AExpert>(new A4AExpert
            {
                ExpertName = "EXPERT_0000",
                FirstName = "AAAAA",
                LastName = "BBBBBB",
                Email = "colin.dick1@btinternet.com",
                CompanyName = company.CompanyName

            });

            repository.AddObject<A4AAdministrator>(new A4AAdministrator
            {
                AdministratorName = "ADMIN_0000",
                Email = "admin@a4a.com",
                Level = A4AAdminLevel.Administrator
            });

            repository.AddObject<A4AUser>(new A4AUser
            {
                UserName = "USER_0000",
                Email = "colin.dick@alphastorm.co.uk",
                Status = A4AUserStatus.Active
            });

            repository.AddObject<A4AProfession>(new A4AProfession {Profession = "Information Technology"});
            repository.AddObject<A4ACategory>(new A4ACategory{ Profession = "Information Technology",Category="Software Development" });
            repository.AddObject<A4ASubCategory>(new A4ASubCategory { Profession = "Information Technology", Category = "Software Development",SubCategory = "C#"});
            repository.AddObject<A4ASubCategory>(new A4ASubCategory { Profession = "Information Technology", Category = "Software Development", SubCategory = "Java" });
            repository.AddObject<A4ASubCategory>(new A4ASubCategory { Profession = "Information Technology", Category = "Software Development", SubCategory = "PYthon" });

            repository.AddObject<A4ASubscription>(new A4ASubscription
            {
                Profession = "Information Technology",
                Category = "Software Development",
                SubCategory = "C#",
                ExpertName = "EXPERT_0000"
            });



        }
    }
}

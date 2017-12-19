using System;
using System.Collections.Generic;
using System.Linq;
using As.A4ACore;
using As.GraphDB.Sql;
using Microsoft.AspNetCore.Http;

namespace App4Answers.Models.A4Amodels
{
    public class A4AModel1
    {
        public A4ARepository Repository { get; private set; }

        public CategoriesAndVerbs.Category Category { get; set; }
        public CategoriesAndVerbs.Verb Verb { get; set; }

        public A4AModel1(A4ARepository repository)
        {
            this.Repository = repository;
        }
        public A4ACompanySummaryViewModel NewCompany()
        {
            var party = Repository.AddParty(new A4ACompanySummaryViewModel
            {
                CompanyName = "New Company"

            }.ModelClassFromViewModel<A4AParty>());
            return new A4ACompanySummaryViewModel(party,CategoriesAndVerbs.Verb.New);
        }

        public ViewModelListBase ListCompany()
        {
            return new ViewModelListBase(typeof(A4ACompanySummaryViewModel), Repository
                .QueryParties($"PartyType like '{A4APartyType.Company}'", new Range(), new Sort())
                .Select(x => new A4ACompanySummaryViewModel(x,CategoriesAndVerbs.Verb.List)),
                CategoriesAndVerbs.Category.Company,CategoriesAndVerbs.Verb.List);
        }

        public A4ACompanyDetailViewModel EditCompany(string id)
        {
            return new A4ACompanyDetailViewModel(Repository.GetPartyById(id),CategoriesAndVerbs.Verb.Edit);
        }

        public ViewModelListBase SaveCompany(IFormCollection form)
        {
            var party = Repository.SaveParty(new A4ACompanyDetailViewModel(form).ModelClassFromViewModel<A4AParty>());
            return ListCompany();
        }

        public void DeleteCompany(String id)
        {
            Repository.DeleteParty(id);
        }


        public ViewModelListBase ListExpert()
        {
            Repository.QueryParties($"PartyType like '{A4APartyType.Expert}'", new Range(), new Sort());
        }

        public GraphResponse RunQuery(GraphQuery query)
        {
            return Repository.RunQuery(query);
        }
    }
}
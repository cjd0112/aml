using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace App4Answers.Models.A4Amodels
{
    /*
     *  /*
     * string FirstName=2;
	string LastName=3;
	string UserName=4;
	string AliasEmail=5;
	string RealEmail=6;
	string Mobile=7;
	string CompanyName=9;
	string Address1 = 10;
	string Address2 = 11;
	string Country = 12;
	string Postcode = 13;
	string Telephone = 14;
	string Website=15;
	string MainColour=16;
	string SecondaryColour=17;
	string TertiaryColour=18;
	string Logo = 19;
    */
    public class A4ACompanyDetailViewModel : ViewModelBase
    {
        public A4ACompanyDetailViewModel() : base(CategoriesAndVerbs.Category.Company, CategoriesAndVerbs.Verb.None)
        {
            Id = "";

        }

        public A4ACompanyDetailViewModel(CategoriesAndVerbs.Verb verb = CategoriesAndVerbs.Verb.None) :base(CategoriesAndVerbs.Category.Company,verb)
        {
            Id = "";
            
        }

        public A4ACompanyDetailViewModel(Object modelSource,CategoriesAndVerbs.Verb verb = CategoriesAndVerbs.Verb.None) :base(modelSource,CategoriesAndVerbs.Category.Company,verb)
        {
            PartyType = A4APartyType.Company;
        }

        public A4ACompanyDetailViewModel(IFormCollection form, CategoriesAndVerbs.Verb verb = CategoriesAndVerbs.Verb.None) :base(form, CategoriesAndVerbs.Category.Company, verb)
        {
            PartyType = A4APartyType.Company;
        }

        public String Id { get; set; }
        public String CompanyName { get; set; }
        public String Address1 { get; set; }
        public String Address2 { get; set; }
        public String Postcode { get; set; }
        public String Telephone { get; set; }
        public String RealEmail { get; set; }
        public String AliasEmail { get; set; }
        public String Mobile { get; set; }
        public String Website { get; set; }
        public String MainColour { get; set; }
        public String SecondaryColour { get; set; }
        public String TertiaryColour { get; set; }
        public String Logo { get; set; }

        public A4APartyType PartyType { get; set; }


    }
}

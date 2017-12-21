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
    public class A4ACompanyDetailViewModel : ViewModelBase<A4ACompany>
    {
        public A4ACompanyDetailViewModel() : base(ObjectTypesAndVerbs.ObjectType.Company, ObjectTypesAndVerbs.Verb.None)
        {

        }

        public A4ACompanyDetailViewModel(ObjectTypesAndVerbs.Verb verb = ObjectTypesAndVerbs.Verb.None) :base(ObjectTypesAndVerbs.ObjectType.Company,verb)
        {
        }

        public A4ACompanyDetailViewModel(A4ACompany modelSource,ObjectTypesAndVerbs.Verb verb = ObjectTypesAndVerbs.Verb.None) :base(modelSource,ObjectTypesAndVerbs.ObjectType.Company,verb)
        {
        }

        public A4ACompanyDetailViewModel(IFormCollection form, ObjectTypesAndVerbs.Verb verb = ObjectTypesAndVerbs.Verb.None) :base(form, ObjectTypesAndVerbs.ObjectType.Company, verb)
        {
        }

        public String CompanyName { get; set; }
        public String Address1 { get; set; }
        public String Address2 { get; set; }
        public String Country { get; set; }
        public String Postcode { get; set; }
        public String Telephone { get; set; }
        public String Email { get; set; }
        public String Website { get; set; }
        public String MainColour { get; set; }
        public String SecondaryColour { get; set; }
        public String TertiaryColour { get; set; }
        public String Logo { get; set; }


    }
}

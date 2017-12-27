using System;
using App4Answers.Models.A4Amodels.Base;
using Microsoft.AspNetCore.Http;

namespace App4Answers.Models.A4Amodels.Administration
{
    /*
     *  /*
     * string FirstName=2;
	string LastName=3;
	string UserName=4;
	string AliasEmail=5;
	string RealEmail=6;
	string Mobile=7;
	string ProfessionName=9;
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
    public class A4AProfessionDetailViewModel : ViewModelBase<A4AProfession>
    {
        public A4AProfessionDetailViewModel() : base(ModelNames.AdministrationNames.Profession, ModelNames.Verb.None)
        {

        }

        public A4AProfessionDetailViewModel(ModelNames.Verb verb = ModelNames.Verb.None) :base(ModelNames.AdministrationNames.Profession,verb)
        {
        }

        public A4AProfessionDetailViewModel(A4AProfession modelSource,ModelNames.Verb verb = ModelNames.Verb.None) :base(modelSource,ModelNames.AdministrationNames.Profession,verb)
        {
        }

        public A4AProfessionDetailViewModel(IFormCollection form, ModelNames.Verb verb = ModelNames.Verb.None) :base(form, ModelNames.AdministrationNames.Profession, verb)
        {
        }

        public String Profession { get; set; }


    }
}

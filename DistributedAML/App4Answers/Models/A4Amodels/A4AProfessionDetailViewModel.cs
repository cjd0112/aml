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
        public A4AProfessionDetailViewModel() : base(ObjectTypesAndVerbs.ObjectType.Profession, ObjectTypesAndVerbs.Verb.None)
        {

        }

        public A4AProfessionDetailViewModel(ObjectTypesAndVerbs.Verb verb = ObjectTypesAndVerbs.Verb.None) :base(ObjectTypesAndVerbs.ObjectType.Profession,verb)
        {
        }

        public A4AProfessionDetailViewModel(A4AProfession modelSource,ObjectTypesAndVerbs.Verb verb = ObjectTypesAndVerbs.Verb.None) :base(modelSource,ObjectTypesAndVerbs.ObjectType.Profession,verb)
        {
        }

        public A4AProfessionDetailViewModel(IFormCollection form, ObjectTypesAndVerbs.Verb verb = ObjectTypesAndVerbs.Verb.None) :base(form, ObjectTypesAndVerbs.ObjectType.Profession, verb)
        {
        }

        public String Profession { get; set; }


    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace App4Answers.Models.A4Amodels
{

   
    public class A4ACompanySummaryViewModel : ViewModelBase<A4ACompany>
    {
        public A4ACompanySummaryViewModel() : base(ObjectTypesAndVerbs.ObjectType.Company, ObjectTypesAndVerbs.Verb.None)
        {

        }

        public A4ACompanySummaryViewModel(ObjectTypesAndVerbs.Verb verb = ObjectTypesAndVerbs.Verb.None) :base(ObjectTypesAndVerbs.ObjectType.Company,verb)
        {
            CompanyName = "Exxon";
            Telephone = "123-123-234-324";
        }

        public A4ACompanySummaryViewModel(A4ACompany modelSource, ObjectTypesAndVerbs.Verb verb) :base(modelSource,ObjectTypesAndVerbs.ObjectType.Company,verb)
        {
        }

        public A4ACompanySummaryViewModel(IFormCollection form,ObjectTypesAndVerbs.Verb verb) :base(form,ObjectTypesAndVerbs.ObjectType.Company,verb)
        {
        }


        public String CompanyName { get; set; }
        public String Email { get; set; }
        public String Telephone { get; set; }
        public String Website { get; set; }


    }
}

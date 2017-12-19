using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace App4Answers.Models.A4Amodels
{

   
    public class A4ACompanySummaryViewModel : ViewModelBase
    {
        public A4ACompanySummaryViewModel() : base(CategoriesAndVerbs.Category.Company, CategoriesAndVerbs.Verb.None)
        {
            Id = "";

        }

        public A4ACompanySummaryViewModel(CategoriesAndVerbs.Verb verb = CategoriesAndVerbs.Verb.None) :base(CategoriesAndVerbs.Category.Company,verb)
        {
            CompanyName = "Exxon";
            Telephone = "123-123-234-324";
        }

        public A4ACompanySummaryViewModel(Object modelSource, CategoriesAndVerbs.Verb verb) :base(modelSource,CategoriesAndVerbs.Category.Company,verb)
        {
        }

        public A4ACompanySummaryViewModel(IFormCollection form,CategoriesAndVerbs.Verb verb) :base(form,CategoriesAndVerbs.Category.Company,verb)
        {
        }

        [Display(AutoGenerateField = false)]
        public String Id { get; set; }

        public String CompanyName { get; set; }
        public String RealEmail { get; set; }
        public String Telephone { get; set; }


    }
}

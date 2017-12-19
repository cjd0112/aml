using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace App4Answers.Models.A4Amodels
{

   
    public class A4AExpertSummaryViewModel : ViewModelBase
    {
        public A4AExpertSummaryViewModel() : base(CategoriesAndVerbs.Category.Expert, CategoriesAndVerbs.Verb.None)
        {
            Id = "";

        }

        public A4AExpertSummaryViewModel(CategoriesAndVerbs.Verb verb = CategoriesAndVerbs.Verb.None) :base(CategoriesAndVerbs.Category.Expert,verb)
        {
        }

        public A4AExpertSummaryViewModel(Object modelSource, CategoriesAndVerbs.Verb verb) :base(modelSource,CategoriesAndVerbs.Category.Expert,verb)
        {
        }

        public A4AExpertSummaryViewModel(IFormCollection form,CategoriesAndVerbs.Verb verb) :base(form,CategoriesAndVerbs.Category.Expert,verb)
        {
        }

        [Display(AutoGenerateField = false)]
        public String Id { get; set; }

        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String RealEmail { get; set; }
        public String AliasEmail { get; set; }
        public String CompanyName { get; set; }
        public String Telephone { get; set; }


    }
}

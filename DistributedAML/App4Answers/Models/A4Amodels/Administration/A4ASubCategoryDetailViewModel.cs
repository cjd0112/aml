using System;
using App4Answers.Models.A4Amodels.Base;
using Microsoft.AspNetCore.Http;

namespace App4Answers.Models.A4Amodels.Administration
{
    public class A4ASubCategoryDetailViewModel : ViewModelBase<A4ASubCategory>
    {
        public A4ASubCategoryDetailViewModel() : base(ModelNames.AdministrationNames.SubCategory, ModelNames.Verb.None)
        {

        }

        public A4ASubCategoryDetailViewModel(ModelNames.Verb verb = ModelNames.Verb.None) :base(ModelNames.AdministrationNames.SubCategory,verb)
        {
        }

        public A4ASubCategoryDetailViewModel(A4ASubCategory modelSource,ModelNames.Verb verb = ModelNames.Verb.None) :base(modelSource,ModelNames.AdministrationNames.SubCategory,verb)
        {
        }

        public A4ASubCategoryDetailViewModel(IFormCollection form, ModelNames.Verb verb = ModelNames.Verb.None) :base(form, ModelNames.AdministrationNames.SubCategory, verb)
        {
        }

        public String Profession { get; set; }

        public String Category { get; set; }

        public String SubCategory { get; set; }


    }
}

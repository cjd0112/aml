using System;
using App4Answers.Models.A4Amodels.Base;
using Microsoft.AspNetCore.Http;

namespace App4Answers.Models.A4Amodels.Administration
{
    public class A4ASubCategoryDetailViewModel : ViewModelBase<A4ASubCategory>
    {
        public A4ASubCategoryDetailViewModel() 
        {

        }

        public A4ASubCategoryDetailViewModel(A4ASubCategory modelSource) :base(modelSource)
        {
        }

        public A4ASubCategoryDetailViewModel(IFormCollection form) :base(form)
        {
        }

        public String Profession { get; set; }

        public String Category { get; set; }

        public String SubCategory { get; set; }


    }
}

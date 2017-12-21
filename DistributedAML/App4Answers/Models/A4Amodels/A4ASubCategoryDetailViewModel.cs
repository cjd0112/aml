using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace App4Answers.Models.A4Amodels
{
    public class A4ASubCategoryDetailViewModel : ViewModelBase<A4ASubCategory>
    {
        public A4ASubCategoryDetailViewModel() : base(ObjectTypesAndVerbs.ObjectType.SubCategory, ObjectTypesAndVerbs.Verb.None)
        {

        }

        public A4ASubCategoryDetailViewModel(ObjectTypesAndVerbs.Verb verb = ObjectTypesAndVerbs.Verb.None) :base(ObjectTypesAndVerbs.ObjectType.SubCategory,verb)
        {
        }

        public A4ASubCategoryDetailViewModel(A4ASubCategory modelSource,ObjectTypesAndVerbs.Verb verb = ObjectTypesAndVerbs.Verb.None) :base(modelSource,ObjectTypesAndVerbs.ObjectType.SubCategory,verb)
        {
        }

        public A4ASubCategoryDetailViewModel(IFormCollection form, ObjectTypesAndVerbs.Verb verb = ObjectTypesAndVerbs.Verb.None) :base(form, ObjectTypesAndVerbs.ObjectType.SubCategory, verb)
        {
        }

        public String Profession { get; set; }

        public String Category { get; set; }

        public String SubCategory { get; set; }


    }
}

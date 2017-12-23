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
        public A4ASubCategoryDetailViewModel() : base(ObjectTypesAndVerbsAndRoles.ObjectType.SubCategory, ObjectTypesAndVerbsAndRoles.Verb.None)
        {

        }

        public A4ASubCategoryDetailViewModel(ObjectTypesAndVerbsAndRoles.Verb verb = ObjectTypesAndVerbsAndRoles.Verb.None) :base(ObjectTypesAndVerbsAndRoles.ObjectType.SubCategory,verb)
        {
        }

        public A4ASubCategoryDetailViewModel(A4ASubCategory modelSource,ObjectTypesAndVerbsAndRoles.Verb verb = ObjectTypesAndVerbsAndRoles.Verb.None) :base(modelSource,ObjectTypesAndVerbsAndRoles.ObjectType.SubCategory,verb)
        {
        }

        public A4ASubCategoryDetailViewModel(IFormCollection form, ObjectTypesAndVerbsAndRoles.Verb verb = ObjectTypesAndVerbsAndRoles.Verb.None) :base(form, ObjectTypesAndVerbsAndRoles.ObjectType.SubCategory, verb)
        {
        }

        public String Profession { get; set; }

        public String Category { get; set; }

        public String SubCategory { get; set; }


    }
}

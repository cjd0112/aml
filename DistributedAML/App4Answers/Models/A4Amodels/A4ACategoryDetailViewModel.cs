using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace App4Answers.Models.A4Amodels
{
    public class A4ACategoryDetailViewModel : ViewModelBase<A4ACategory>
    {
        public A4ACategoryDetailViewModel() : base(ObjectTypesAndVerbs.ObjectType.Category, ObjectTypesAndVerbs.Verb.None)
        {

        }

        public A4ACategoryDetailViewModel(ObjectTypesAndVerbs.Verb verb = ObjectTypesAndVerbs.Verb.None) :base(ObjectTypesAndVerbs.ObjectType.Category,verb)
        {
        }

        public A4ACategoryDetailViewModel(A4ACategory modelSource,ObjectTypesAndVerbs.Verb verb = ObjectTypesAndVerbs.Verb.None) :base(modelSource,ObjectTypesAndVerbs.ObjectType.Category,verb)
        {
        }

        public A4ACategoryDetailViewModel(IFormCollection form, ObjectTypesAndVerbs.Verb verb = ObjectTypesAndVerbs.Verb.None) :base(form, ObjectTypesAndVerbs.ObjectType.Category, verb)
        {
        }

        public String Profession { get; set; }
        public String Category { get; set; }


    }
}

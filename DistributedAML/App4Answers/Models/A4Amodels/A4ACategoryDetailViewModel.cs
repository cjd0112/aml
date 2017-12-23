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
        public A4ACategoryDetailViewModel() : base(ObjectTypesAndVerbsAndRoles.ObjectType.Category, ObjectTypesAndVerbsAndRoles.Verb.None)
        {

        }

        public A4ACategoryDetailViewModel(ObjectTypesAndVerbsAndRoles.Verb verb = ObjectTypesAndVerbsAndRoles.Verb.None) :base(ObjectTypesAndVerbsAndRoles.ObjectType.Category,verb)
        {
        }

        public A4ACategoryDetailViewModel(A4ACategory modelSource,ObjectTypesAndVerbsAndRoles.Verb verb = ObjectTypesAndVerbsAndRoles.Verb.None) :base(modelSource,ObjectTypesAndVerbsAndRoles.ObjectType.Category,verb)
        {
        }

        public A4ACategoryDetailViewModel(IFormCollection form, ObjectTypesAndVerbsAndRoles.Verb verb = ObjectTypesAndVerbsAndRoles.Verb.None) :base(form, ObjectTypesAndVerbsAndRoles.ObjectType.Category, verb)
        {
        }

        public String Profession { get; set; }
        public String Category { get; set; }


    }
}

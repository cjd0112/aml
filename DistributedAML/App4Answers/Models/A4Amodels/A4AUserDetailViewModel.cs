using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace App4Answers.Models.A4Amodels
{

   
    public class A4AUserDetailViewModel : ViewModelBase<A4AUser>
    {
        public A4AUserDetailViewModel() : base(ObjectTypesAndVerbs.ObjectType.User, ObjectTypesAndVerbs.Verb.None)
        {

        }

        public A4AUserDetailViewModel(ObjectTypesAndVerbs.Verb verb = ObjectTypesAndVerbs.Verb.None) :base(ObjectTypesAndVerbs.ObjectType.User,verb)
        {
        }

        public A4AUserDetailViewModel(A4AUser modelSource, ObjectTypesAndVerbs.Verb verb = ObjectTypesAndVerbs.Verb.None) :base(modelSource,ObjectTypesAndVerbs.ObjectType.User,verb)
        {
        }

        public A4AUserDetailViewModel(IFormCollection form,ObjectTypesAndVerbs.Verb verb = ObjectTypesAndVerbs.Verb.None) :base(form,ObjectTypesAndVerbs.ObjectType.User,verb)
        {
        }

        public String UserName { get; set; }
        public String Email { get; set; }

        public A4AUserStatus Status { get; set; }

    }
}

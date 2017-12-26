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
        public A4AUserDetailViewModel() : base(ObjectTypesAndVerbsAndRoles.ObjectType.User, ObjectTypesAndVerbsAndRoles.Verb.None)
        {

        }

        public A4AUserDetailViewModel(ObjectTypesAndVerbsAndRoles.Verb verb = ObjectTypesAndVerbsAndRoles.Verb.None) :base(ObjectTypesAndVerbsAndRoles.ObjectType.User,verb)
        {
        }

        public A4AUserDetailViewModel(A4AUser modelSource, ObjectTypesAndVerbsAndRoles.Verb verb = ObjectTypesAndVerbsAndRoles.Verb.None) :base(modelSource,ObjectTypesAndVerbsAndRoles.ObjectType.User,verb)
        {
        }

        public A4AUserDetailViewModel(IFormCollection form,ObjectTypesAndVerbsAndRoles.Verb verb = ObjectTypesAndVerbsAndRoles.Verb.None) :base(form,ObjectTypesAndVerbsAndRoles.ObjectType.User,verb)
        {
        }

        public String UserName { get; set; }
        public String Email { get; set; }

        public A4AUserStatus Status { get; set; }

    }
}

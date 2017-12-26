using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace App4Answers.Models.A4Amodels
{
    public class A4AAdministratorDetailViewModel : ViewModelBase<A4AAdministrator>
    {
        public A4AAdministratorDetailViewModel() : base(ObjectTypesAndVerbsAndRoles.ObjectType.Administrator, ObjectTypesAndVerbsAndRoles.Verb.None)
        {

        }

        public A4AAdministratorDetailViewModel(ObjectTypesAndVerbsAndRoles.Verb verb = ObjectTypesAndVerbsAndRoles.Verb.None) :base(ObjectTypesAndVerbsAndRoles.ObjectType.Administrator,verb)
        {
        }

        public A4AAdministratorDetailViewModel(A4AAdministrator modelSource,ObjectTypesAndVerbsAndRoles.Verb verb = ObjectTypesAndVerbsAndRoles.Verb.None) :base(modelSource,ObjectTypesAndVerbsAndRoles.ObjectType.Administrator,verb)
        {
        }

        public A4AAdministratorDetailViewModel(IFormCollection form, ObjectTypesAndVerbsAndRoles.Verb verb = ObjectTypesAndVerbsAndRoles.Verb.None) :base(form, ObjectTypesAndVerbsAndRoles.ObjectType.Administrator, verb)
        {
        }

        public String Email { get; set; }
        public String AdministratorName { get; set; }
        public A4AAdminLevel Level { get; set; }





    }
}

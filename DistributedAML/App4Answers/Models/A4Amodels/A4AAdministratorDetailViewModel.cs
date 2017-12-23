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
        public A4AAdministratorDetailViewModel() : base(ObjectTypesAndVerbs.ObjectType.Administrator, ObjectTypesAndVerbs.Verb.None)
        {

        }

        public A4AAdministratorDetailViewModel(ObjectTypesAndVerbs.Verb verb = ObjectTypesAndVerbs.Verb.None) :base(ObjectTypesAndVerbs.ObjectType.Administrator,verb)
        {
        }

        public A4AAdministratorDetailViewModel(A4AAdministrator modelSource,ObjectTypesAndVerbs.Verb verb = ObjectTypesAndVerbs.Verb.None) :base(modelSource,ObjectTypesAndVerbs.ObjectType.Administrator,verb)
        {
        }

        public A4AAdministratorDetailViewModel(IFormCollection form, ObjectTypesAndVerbs.Verb verb = ObjectTypesAndVerbs.Verb.None) :base(form, ObjectTypesAndVerbs.ObjectType.Administrator, verb)
        {
        }

        public String Email { get; set; }
        public String AdministratorName { get; set; }
        public A4AAdminLevel Level { get; set; }





    }
}

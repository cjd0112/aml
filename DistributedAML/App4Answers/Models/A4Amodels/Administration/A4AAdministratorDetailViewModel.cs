using System;
using App4Answers.Models.A4Amodels.Base;
using Microsoft.AspNetCore.Http;

namespace App4Answers.Models.A4Amodels.Administration
{
    public class A4AAdministratorDetailViewModel : ViewModelBase<A4AAdministrator>
    {
        public A4AAdministratorDetailViewModel() : base(ModelNames.AdministrationNames.Administrator, ModelNames.Verb.None)
        {

        }

        public A4AAdministratorDetailViewModel(ModelNames.Verb verb = ModelNames.Verb.None) :base(ModelNames.AdministrationNames.Administrator,verb)
        {
        }

        public A4AAdministratorDetailViewModel(A4AAdministrator modelSource,ModelNames.Verb verb = ModelNames.Verb.None) :base(modelSource,ModelNames.AdministrationNames.Administrator,verb)
        {
        }

        public A4AAdministratorDetailViewModel(IFormCollection form, ModelNames.Verb verb = ModelNames.Verb.None) :base(form, ModelNames.AdministrationNames.Administrator, verb)
        {
        }

        public String Email { get; set; }
        public String AdministratorName { get; set; }
        public A4AAdminLevel Level { get; set; }





    }
}

using System;
using App4Answers.Models.A4Amodels.Base;
using Microsoft.AspNetCore.Http;

namespace App4Answers.Models.A4Amodels.Administration
{
    public class A4AAdministratorDetailViewModel : ViewModelBase<A4AAdministrator>
    {
        public A4AAdministratorDetailViewModel() 
        {

        }

        public A4AAdministratorDetailViewModel(A4AAdministrator modelSource) :base(modelSource)
        {
        }

        public A4AAdministratorDetailViewModel(IFormCollection form) :base(form)
        {
        }

        public String Email { get; set; }
        public String AdministratorName { get; set; }
        public A4AAdminLevel Level { get; set; }





    }
}

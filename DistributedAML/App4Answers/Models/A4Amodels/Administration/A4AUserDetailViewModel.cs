using System;
using App4Answers.Models.A4Amodels.Base;
using Microsoft.AspNetCore.Http;

namespace App4Answers.Models.A4Amodels.Administration
{

   
    public class A4AUserDetailViewModel : ViewModelBase<A4AUser>
    {
        public A4AUserDetailViewModel() : base(ModelNames.AdministrationNames.User, ModelNames.Verb.None)
        {

        }

        public A4AUserDetailViewModel(ModelNames.Verb verb = ModelNames.Verb.None) :base(ModelNames.AdministrationNames.User,verb)
        {
        }

        public A4AUserDetailViewModel(A4AUser modelSource, ModelNames.Verb verb = ModelNames.Verb.None) :base(modelSource,ModelNames.AdministrationNames.User,verb)
        {
        }

        public A4AUserDetailViewModel(IFormCollection form,ModelNames.Verb verb = ModelNames.Verb.None) :base(form,ModelNames.AdministrationNames.User,verb)
        {
        }

        public String UserName { get; set; }
        public String Email { get; set; }

        public A4AUserStatus Status { get; set; }

    }
}

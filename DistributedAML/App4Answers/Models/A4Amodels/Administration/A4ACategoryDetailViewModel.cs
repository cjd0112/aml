using System;
using App4Answers.Models.A4Amodels.Base;
using Microsoft.AspNetCore.Http;

namespace App4Answers.Models.A4Amodels.Administration
{
    public class A4ACategoryDetailViewModel : ViewModelBase<A4ACategory>
    {
        public A4ACategoryDetailViewModel() : base(ModelNames.AdministrationNames.Category, ModelNames.Verb.None)
        {

        }

        public A4ACategoryDetailViewModel(ModelNames.Verb verb = ModelNames.Verb.None) :base(ModelNames.AdministrationNames.Category,verb)
        {
        }

        public A4ACategoryDetailViewModel(A4ACategory modelSource,ModelNames.Verb verb = ModelNames.Verb.None) :base(modelSource,ModelNames.AdministrationNames.Category,verb)
        {
        }

        public A4ACategoryDetailViewModel(IFormCollection form, ModelNames.Verb verb = ModelNames.Verb.None) :base(form, ModelNames.AdministrationNames.Category, verb)
        {
        }

        public String Profession { get; set; }
        public String Category { get; set; }


    }
}

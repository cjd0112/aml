using System;
using App4Answers.Models.A4Amodels.Base;
using Microsoft.AspNetCore.Http;

namespace App4Answers.Models.A4Amodels.Administration
{
    public class A4ALocationDetailViewModel : ViewModelBase<A4ALocation>
    {
        public A4ALocationDetailViewModel() : base(ModelNames.AdministrationNames.Location, ModelNames.Verb.None)
        {

        }

        public A4ALocationDetailViewModel(ModelNames.Verb verb = ModelNames.Verb.None) :base(ModelNames.AdministrationNames.Location,verb)
        {
        }

        public A4ALocationDetailViewModel(A4ALocation modelSource,ModelNames.Verb verb = ModelNames.Verb.None) :base(modelSource,ModelNames.AdministrationNames.Location,verb)
        {
        }

        public A4ALocationDetailViewModel(IFormCollection form, ModelNames.Verb verb = ModelNames.Verb.None) :base(form, ModelNames.AdministrationNames.Location, verb)
        {
        }

        public String Profession { get; set; }

        public String Category { get; set; }

        public String SubCategory { get; set; }


        public String Location { get; set; }


    }
}

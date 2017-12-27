using System;
using App4Answers.Models.A4Amodels.Base;
using Microsoft.AspNetCore.Http;

namespace App4Answers.Models.A4Amodels.Administration
{

   
    public class A4AExpertDetailViewModel : ViewModelBase<A4AExpert>
    {
        public A4AExpertDetailViewModel() : base(ModelNames.AdministrationNames.Expert, ModelNames.Verb.None)
        {

        }

        public A4AExpertDetailViewModel(ModelNames.Verb verb = ModelNames.Verb.None) :base(ModelNames.AdministrationNames.Expert,verb)
        {
        }

        public A4AExpertDetailViewModel(A4AExpert modelSource, ModelNames.Verb verb = ModelNames.Verb.None) :base(modelSource,ModelNames.AdministrationNames.Expert,verb)
        {
        }

        public A4AExpertDetailViewModel(IFormCollection form,ModelNames.Verb verb = ModelNames.Verb.None) :base(form,ModelNames.AdministrationNames.Expert,verb)
        {
        }

        public String ExpertName { get; set; }
        public String CompanyName { get; set; }
        public String Mobile { get; set; }
        public String Email { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }


    }
}

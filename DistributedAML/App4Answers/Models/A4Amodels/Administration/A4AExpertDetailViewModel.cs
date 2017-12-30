using System;
using App4Answers.Models.A4Amodels.Base;
using Microsoft.AspNetCore.Http;

namespace App4Answers.Models.A4Amodels.Administration
{

   
    public class A4AExpertDetailViewModel : ViewModelBase<A4AExpert>
    {
        public A4AExpertDetailViewModel() 
        {

        }

        public A4AExpertDetailViewModel(A4AExpert modelSource, ModelNames.Verb verb = ModelNames.Verb.None) :base(modelSource)
        {
        }

        public A4AExpertDetailViewModel(IFormCollection form,ModelNames.Verb verb = ModelNames.Verb.None) :base(form)
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

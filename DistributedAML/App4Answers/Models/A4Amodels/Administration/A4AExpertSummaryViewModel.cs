using System;
using App4Answers.Models.A4Amodels.Base;
using Microsoft.AspNetCore.Http;

namespace App4Answers.Models.A4Amodels.Administration
{

   
    public class A4AExpertSummaryViewModel : ViewModelBase<A4AExpert>
    {
        public A4AExpertSummaryViewModel() : base(ModelNames.AdministrationNames.Expert, ModelNames.Verb.None)
        {

        }

        public A4AExpertSummaryViewModel(ModelNames.Verb verb = ModelNames.Verb.None) :base(ModelNames.AdministrationNames.Expert,verb)
        {
        }

        public A4AExpertSummaryViewModel(A4AExpert modelSource, ModelNames.Verb verb) :base(modelSource,ModelNames.AdministrationNames.Expert,verb)
        {
        }

        public A4AExpertSummaryViewModel(IFormCollection form,ModelNames.Verb verb) :base(form,ModelNames.AdministrationNames.Expert,verb)
        {
        }

        public String ExpertName { get; set; }
        public String CompanyName { get; set; }
        public String Mobile { get; set; }
        public String Email { get; set; }

    }
}

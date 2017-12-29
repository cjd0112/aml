using System;
using App4Answers.Models.A4Amodels.Base;
using Microsoft.AspNetCore.Http;

namespace App4Answers.Models.A4Amodels.Administration
{

   
    public class A4ACompanySummaryViewModel : ViewModelBase<A4ACompany>
    {
        public A4ACompanySummaryViewModel() : base(ModelNames.AdministrationNames.Company, ModelNames.Verb.None)
        {

        }

        public A4ACompanySummaryViewModel(ModelNames.Verb verb = ModelNames.Verb.None) :base(ModelNames.AdministrationNames.Company,verb)
        {
            CompanyName = "Exxon";
            Telephone = "123-123-234-324";
        }

        public A4ACompanySummaryViewModel(A4ACompany modelSource, ModelNames.Verb verb) :base(modelSource,ModelNames.AdministrationNames.Company,verb)
        {
        }

        public A4ACompanySummaryViewModel(IFormCollection form,ModelNames.Verb verb) :base(form,ModelNames.AdministrationNames.Company,verb)
        {
        }


        public String CompanyName { get; set; }
        public String Telephone { get; set; }
        public String Website { get; set; }


    }
}

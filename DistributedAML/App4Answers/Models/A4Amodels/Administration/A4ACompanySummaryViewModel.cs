using System;
using App4Answers.Models.A4Amodels.Base;
using Microsoft.AspNetCore.Http;

namespace App4Answers.Models.A4Amodels.Administration
{

   
    public class A4ACompanySummaryViewModel : ViewModelBase<A4ACompany>
    {
        public A4ACompanySummaryViewModel()         {

        }


        public A4ACompanySummaryViewModel(A4ACompany modelSource) :base(modelSource)
        {
        }

        public A4ACompanySummaryViewModel(IFormCollection form) :base(form)
        {
        }


        public String CompanyName { get; set; }
        public String Telephone { get; set; }
        public String Website { get; set; }


    }
}

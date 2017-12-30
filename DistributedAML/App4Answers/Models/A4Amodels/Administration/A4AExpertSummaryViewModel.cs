using System;
using App4Answers.Models.A4Amodels.Base;
using Microsoft.AspNetCore.Http;

namespace App4Answers.Models.A4Amodels.Administration
{

   
    public class A4AExpertSummaryViewModel : ViewModelBase<A4AExpert>
    {
        public A4AExpertSummaryViewModel()
        {

        }

        public A4AExpertSummaryViewModel(A4AExpert modelSource) :base(modelSource)
        {
        }

        public A4AExpertSummaryViewModel(IFormCollection form) :base(form)
        {
        }

        public String ExpertName { get; set; }
        public String CompanyName { get; set; }
        public String Mobile { get; set; }
        public String Email { get; set; }

    }
}

using System;
using App4Answers.Models.A4Amodels.Base;
using Microsoft.AspNetCore.Http;

namespace App4Answers.Models.A4Amodels.Administration
{
    public class A4ACompanyDetailViewModel : ViewModelBase<A4ACompany>
    {
        public A4ACompanyDetailViewModel() 
        {

        }

      
        public A4ACompanyDetailViewModel(A4ACompany modelSource) :base(modelSource)
        {
        }

        public A4ACompanyDetailViewModel(IFormCollection form) :base(form)
        {
        }

        public String CompanyName { get; set; }
        public String Address1 { get; set; }
        public String Address2 { get; set; }
        public String Country { get; set; }
        public String Postcode { get; set; }
        public String Telephone { get; set; }
        public String Website { get; set; }
        public String MainColour { get; set; }
        public String SecondaryColour { get; set; }
        public String TertiaryColour { get; set; }
        public String Logo { get; set; }


    }
}

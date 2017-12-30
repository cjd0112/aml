using System;
using App4Answers.Models.A4Amodels.Base;
using Microsoft.AspNetCore.Http;

namespace App4Answers.Models.A4Amodels.Administration
{
    public class A4ALocationDetailViewModel : ViewModelBase<A4ALocation>
    {
        public A4ALocationDetailViewModel() 
        {

        }

        public A4ALocationDetailViewModel(A4ALocation modelSource) :base(modelSource)
        {
        }

        public A4ALocationDetailViewModel(IFormCollection form) :base(form)
        {
        }

        public String Profession { get; set; }

        public String Category { get; set; }

        public String SubCategory { get; set; }


        public String Location { get; set; }


    }
}

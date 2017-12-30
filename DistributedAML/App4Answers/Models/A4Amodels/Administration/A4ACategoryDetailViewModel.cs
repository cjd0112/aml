using System;
using App4Answers.Models.A4Amodels.Base;
using Microsoft.AspNetCore.Http;

namespace App4Answers.Models.A4Amodels.Administration
{
    public class A4ACategoryDetailViewModel : ViewModelBase<A4ACategory>
    {
        public A4ACategoryDetailViewModel() 
        {

        }

        public A4ACategoryDetailViewModel(A4ACategory modelSource) :base(modelSource)
        {
        }

        public A4ACategoryDetailViewModel(IFormCollection form) :base(form)
        {
        }

        public String Profession { get; set; }
        public String Category { get; set; }


    }
}

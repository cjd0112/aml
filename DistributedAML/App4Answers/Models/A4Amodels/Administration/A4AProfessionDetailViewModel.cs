using System;
using App4Answers.Models.A4Amodels.Base;
using Microsoft.AspNetCore.Http;

namespace App4Answers.Models.A4Amodels.Administration
{
   
    public class A4AProfessionDetailViewModel : ViewModelBase<A4AProfession>
    {
        public A4AProfessionDetailViewModel() 
        {

        }

        public A4AProfessionDetailViewModel(A4AProfession modelSource) :base(modelSource)
        {
        }

        public A4AProfessionDetailViewModel(IFormCollection form) :base(form)
        {
        }

        public String Profession { get; set; }


    }
}

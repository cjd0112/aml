using System;
using App4Answers.Models.A4Amodels.Base;
using Microsoft.AspNetCore.Http;

namespace App4Answers.Models.A4Amodels.Administration
{
    public class A4ASubscriptionDetailViewModel : ViewModelBase<A4ASubscription>
    {
        public A4ASubscriptionDetailViewModel() 
        {

        }

        public A4ASubscriptionDetailViewModel(A4ASubscription modelSource) :base(modelSource)
        {
        }

        public A4ASubscriptionDetailViewModel(IFormCollection form) :base(form)
        {
        }

        public String Subscription { get; set; }

        public String Profession { get; set; }

        public String Category { get; set; }

        public String SubCategory { get; set; }

        public String Location { get; set; }


        public String ExpertName { get; set; }


    }
}

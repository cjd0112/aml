using System;
using App4Answers.Models.A4Amodels.Base;
using Microsoft.AspNetCore.Http;

namespace App4Answers.Models.A4Amodels.Administration
{
    public class A4ASubscriptionDetailViewModel : ViewModelBase<A4ASubscription>
    {
        public A4ASubscriptionDetailViewModel() : base(ModelNames.AdministrationNames.Subscription, ModelNames.Verb.None)
        {

        }

        public A4ASubscriptionDetailViewModel(ModelNames.Verb verb = ModelNames.Verb.None) :base(ModelNames.AdministrationNames.Subscription,verb)
        {
        }

        public A4ASubscriptionDetailViewModel(A4ASubscription modelSource,ModelNames.Verb verb = ModelNames.Verb.None) :base(modelSource,ModelNames.AdministrationNames.Subscription,verb)
        {
        }

        public A4ASubscriptionDetailViewModel(IFormCollection form, ModelNames.Verb verb = ModelNames.Verb.None) :base(form, ModelNames.AdministrationNames.Subscription, verb)
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

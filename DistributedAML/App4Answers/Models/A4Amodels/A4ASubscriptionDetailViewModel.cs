using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace App4Answers.Models.A4Amodels
{
    public class A4ASubscriptionDetailViewModel : ViewModelBase<A4ASubscription>
    {
        public A4ASubscriptionDetailViewModel() : base(ObjectTypesAndVerbs.ObjectType.Subscription, ObjectTypesAndVerbs.Verb.None)
        {

        }

        public A4ASubscriptionDetailViewModel(ObjectTypesAndVerbs.Verb verb = ObjectTypesAndVerbs.Verb.None) :base(ObjectTypesAndVerbs.ObjectType.Subscription,verb)
        {
        }

        public A4ASubscriptionDetailViewModel(A4ASubscription modelSource,ObjectTypesAndVerbs.Verb verb = ObjectTypesAndVerbs.Verb.None) :base(modelSource,ObjectTypesAndVerbs.ObjectType.Subscription,verb)
        {
        }

        public A4ASubscriptionDetailViewModel(IFormCollection form, ObjectTypesAndVerbs.Verb verb = ObjectTypesAndVerbs.Verb.None) :base(form, ObjectTypesAndVerbs.ObjectType.Subscription, verb)
        {
        }

        public String Subscription { get; set; }

        public String Profession { get; set; }

        public String Category { get; set; }

        public String SubCategory { get; set; }


        public String ExpertName { get; set; }


    }
}

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
        public A4ASubscriptionDetailViewModel() : base(ObjectTypesAndVerbsAndRoles.ObjectType.Subscription, ObjectTypesAndVerbsAndRoles.Verb.None)
        {

        }

        public A4ASubscriptionDetailViewModel(ObjectTypesAndVerbsAndRoles.Verb verb = ObjectTypesAndVerbsAndRoles.Verb.None) :base(ObjectTypesAndVerbsAndRoles.ObjectType.Subscription,verb)
        {
        }

        public A4ASubscriptionDetailViewModel(A4ASubscription modelSource,ObjectTypesAndVerbsAndRoles.Verb verb = ObjectTypesAndVerbsAndRoles.Verb.None) :base(modelSource,ObjectTypesAndVerbsAndRoles.ObjectType.Subscription,verb)
        {
        }

        public A4ASubscriptionDetailViewModel(IFormCollection form, ObjectTypesAndVerbsAndRoles.Verb verb = ObjectTypesAndVerbsAndRoles.Verb.None) :base(form, ObjectTypesAndVerbsAndRoles.ObjectType.Subscription, verb)
        {
        }

        public String Subscription { get; set; }

        public String Profession { get; set; }

        public String Category { get; set; }

        public String SubCategory { get; set; }


        public String ExpertName { get; set; }


    }
}

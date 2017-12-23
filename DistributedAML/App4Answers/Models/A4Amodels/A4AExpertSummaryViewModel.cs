using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace App4Answers.Models.A4Amodels
{

   
    public class A4AExpertSummaryViewModel : ViewModelBase<A4AExpert>
    {
        public A4AExpertSummaryViewModel() : base(ObjectTypesAndVerbsAndRoles.ObjectType.Expert, ObjectTypesAndVerbsAndRoles.Verb.None)
        {

        }

        public A4AExpertSummaryViewModel(ObjectTypesAndVerbsAndRoles.Verb verb = ObjectTypesAndVerbsAndRoles.Verb.None) :base(ObjectTypesAndVerbsAndRoles.ObjectType.Expert,verb)
        {
        }

        public A4AExpertSummaryViewModel(A4AExpert modelSource, ObjectTypesAndVerbsAndRoles.Verb verb) :base(modelSource,ObjectTypesAndVerbsAndRoles.ObjectType.Expert,verb)
        {
        }

        public A4AExpertSummaryViewModel(IFormCollection form,ObjectTypesAndVerbsAndRoles.Verb verb) :base(form,ObjectTypesAndVerbsAndRoles.ObjectType.Expert,verb)
        {
        }

        public String ExpertName { get; set; }
        public String CompanyName { get; set; }
        public String Mobile { get; set; }
        public String RealEmail { get; set; }

    }
}

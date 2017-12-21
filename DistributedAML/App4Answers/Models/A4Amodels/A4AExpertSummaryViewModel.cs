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
        public A4AExpertSummaryViewModel() : base(ObjectTypesAndVerbs.ObjectType.Expert, ObjectTypesAndVerbs.Verb.None)
        {

        }

        public A4AExpertSummaryViewModel(ObjectTypesAndVerbs.Verb verb = ObjectTypesAndVerbs.Verb.None) :base(ObjectTypesAndVerbs.ObjectType.Expert,verb)
        {
        }

        public A4AExpertSummaryViewModel(A4AExpert modelSource, ObjectTypesAndVerbs.Verb verb) :base(modelSource,ObjectTypesAndVerbs.ObjectType.Expert,verb)
        {
        }

        public A4AExpertSummaryViewModel(IFormCollection form,ObjectTypesAndVerbs.Verb verb) :base(form,ObjectTypesAndVerbs.ObjectType.Expert,verb)
        {
        }

        public String ExpertName { get; set; }
        public String CompanyName { get; set; }
        public String Mobile { get; set; }
        public String RealEmail { get; set; }

    }
}

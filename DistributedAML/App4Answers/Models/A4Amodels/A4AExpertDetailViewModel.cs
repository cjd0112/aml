using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace App4Answers.Models.A4Amodels
{

   
    public class A4AExpertDetailViewModel : ViewModelBase<A4AExpert>
    {
        public A4AExpertDetailViewModel() : base(ObjectTypesAndVerbs.ObjectType.Expert, ObjectTypesAndVerbs.Verb.None)
        {

        }

        public A4AExpertDetailViewModel(ObjectTypesAndVerbs.Verb verb = ObjectTypesAndVerbs.Verb.None) :base(ObjectTypesAndVerbs.ObjectType.Expert,verb)
        {
        }

        public A4AExpertDetailViewModel(A4AExpert modelSource, ObjectTypesAndVerbs.Verb verb = ObjectTypesAndVerbs.Verb.None) :base(modelSource,ObjectTypesAndVerbs.ObjectType.Expert,verb)
        {
        }

        public A4AExpertDetailViewModel(IFormCollection form,ObjectTypesAndVerbs.Verb verb = ObjectTypesAndVerbs.Verb.None) :base(form,ObjectTypesAndVerbs.ObjectType.Expert,verb)
        {
        }

        public String ExpertName { get; set; }
        public String CompanyName { get; set; }
        public String Mobile { get; set; }
        public String RealEmail { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String AliasEmail { get; set; }


    }
}

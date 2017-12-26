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
        public A4AExpertDetailViewModel() : base(ObjectTypesAndVerbsAndRoles.ObjectType.Expert, ObjectTypesAndVerbsAndRoles.Verb.None)
        {

        }

        public A4AExpertDetailViewModel(ObjectTypesAndVerbsAndRoles.Verb verb = ObjectTypesAndVerbsAndRoles.Verb.None) :base(ObjectTypesAndVerbsAndRoles.ObjectType.Expert,verb)
        {
        }

        public A4AExpertDetailViewModel(A4AExpert modelSource, ObjectTypesAndVerbsAndRoles.Verb verb = ObjectTypesAndVerbsAndRoles.Verb.None) :base(modelSource,ObjectTypesAndVerbsAndRoles.ObjectType.Expert,verb)
        {
        }

        public A4AExpertDetailViewModel(IFormCollection form,ObjectTypesAndVerbsAndRoles.Verb verb = ObjectTypesAndVerbsAndRoles.Verb.None) :base(form,ObjectTypesAndVerbsAndRoles.ObjectType.Expert,verb)
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

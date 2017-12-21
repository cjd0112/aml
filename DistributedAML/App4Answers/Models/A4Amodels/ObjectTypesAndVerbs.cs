using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App4Answers.Models.A4Amodels
{
    public class ObjectTypesAndVerbs
    {
        public enum ObjectType
        {
            None,
            Company,
            Expert,
            Profession,
            Category,
            SubCategory,
            Subscription,
            User,
            Administrator
        }

        public enum Verb
        {
            None,
            New,
            List,
            Edit,
            Delete,
            Save
        }
    }
}

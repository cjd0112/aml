using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App4Answers.Models.A4Amodels
{
    public class CategoriesAndVerbs
    {
        public enum Category
        {
            None,
            Company,
            Expert
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

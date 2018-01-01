using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App4Answers.Models.A4Amodels
{
    public class ModelNames
    {
        public enum ActionNames
        {
            None,
            Administration,
            EmailManager
            
        }
        public enum ObjectTypes
        {
            None,
            Company,
            Expert,
            Profession,
            Category,
            SubCategory,
            Location,
            Subscription,
            User,
            Administrator,
            Message,
            EmailRecord
        }

        public enum Verb
        {
            None,
            New,
            List,
            Edit,
            Delete,
            Save,
        }
        public enum SessionStrings
        {
            UserEmail,
            UserType,
            UserName
        }

        public enum ItemStrings
        {
            EmailListType
        }
    }
}

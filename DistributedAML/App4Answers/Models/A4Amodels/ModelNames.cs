using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App4Answers.Models.A4Amodels
{
    public class ModelNames
    {
        public enum AdministrationNames
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

        public enum Role
        {
            Administrator,
            Expert,
            User
        }

        public enum EmailList
        {
            None,
            Inbox,
            Sent,
            Drafts,
            Trash,
            Logs
        }

        public enum SessionStrings
        {
            User,
            Role
        }

        public enum ItemStrings
        {
            EmailListType
        }
    }
}

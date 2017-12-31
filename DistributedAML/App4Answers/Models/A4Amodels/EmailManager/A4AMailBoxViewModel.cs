using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App4Answers.Models.A4Amodels.Base;

namespace App4Answers.Models.A4Amodels.EmailManager
{
    public class A4AMailBoxViewModel : IViewModel
    {
        public A4AMailBoxViewModel()
        {

        }
        public A4AMailBoxViewModel(MailboxView model)
        {
            ViewModel = model;
        }

        public MailboxView ViewModel { get; set; }

        public ModelNames.ObjectTypes ObjectTypes { get; set; }
        public ModelNames.Verb Verb { get; set; }
        public ModelNames.ActionNames ActionNames { get; set; }
    }
}

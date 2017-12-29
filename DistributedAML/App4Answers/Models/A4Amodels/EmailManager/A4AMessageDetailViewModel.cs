using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App4Answers.Models.A4Amodels.Base;
using Microsoft.AspNetCore.Http;

namespace App4Answers.Models.A4Amodels.EmailManager
{
    public class A4AMessageDetailViewModel : ViewModelBase<A4AMessage>
    {
        public A4AMessageDetailViewModel() : base(ModelNames.AdministrationNames.Message, ModelNames.Verb.List)
        {

        }
        public A4AMessageDetailViewModel(IFormCollection forms) : base(forms, ModelNames.AdministrationNames.Message,
            ModelNames.Verb.List)
        {

        }

        public A4AMessageDetailViewModel(A4AMessage msg) : base(msg, ModelNames.AdministrationNames.Message,
            ModelNames.Verb.List)
        {

        }

        public string MessageId { get; set; }

        public string EmailSender { get; set; }

        public string Subject { get; set; }

        public string Content { get; set; }

        public string Date { get; set; }

        public string Profession { get; set; }

        public string Category { get; set; }

        public string SubCategory { get; set; }

    }
}

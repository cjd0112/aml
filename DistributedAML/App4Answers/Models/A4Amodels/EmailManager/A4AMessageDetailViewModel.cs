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
        public A4AMessageDetailViewModel() 
        {

        }
        public A4AMessageDetailViewModel(IFormCollection forms) : base(forms)
        {

        }

        public A4AMessageDetailViewModel(A4AMessage msg) : base(msg)
        {

        }

        public string MessageId { get; set; }

        public string EmailSender { get; set; }

        public string Subject { get; set; }

        public string TextContent { get; set; }

        public string HtmlContent { get; set; }


        public string Date { get; set; }

        public string Topic { get; set; }

    }
}

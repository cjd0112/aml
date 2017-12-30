using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App4Answers.Models.A4Amodels.Base;

namespace App4Answers.Models.A4Amodels.EmailManager
{
    public class A4AMessageSummaryViewModel : ViewModelBase<A4AMessage>
    {
        public A4AMessageSummaryViewModel() 
        {

        }

        public A4AMessageSummaryViewModel(A4AMessage msg) : base(msg)
        {

        }

        public A4AMessageSummaryViewModel(Microsoft.AspNetCore.Http.IFormCollection form) : base(form)
        {

        }

        public string MessageId { get; set; }

        public string Subject { get; set; }

        public string Date { get; set; }

        public string Topic { get; set; }

    }
}

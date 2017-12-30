using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App4Answers.Models.A4Amodels.Base;
using Microsoft.AspNetCore.Http;

namespace App4Answers.Models.A4Amodels.EmailManager
{
    public class A4AEmailRecordDetailViewModel : ViewModelBase<A4AEmailRecord>
    {
        public A4AEmailRecordDetailViewModel() 
        {

        }
        public A4AEmailRecordDetailViewModel(IFormCollection forms) : base(forms)
        {

        }

        public A4AEmailRecordDetailViewModel(A4AEmailRecord msg) : base(msg)
        {

        }

        public string EmailRecordId { get; set; }

        public string Subject { get; set; }

        public string MessageId { get; set; }

        public string NameFrom { get; set; }

        public string Url { get; set; }

        public string NameTo { get; set; }

        public string Status { get; set; }

        public string ExternalMessageId { get; set; }

        public string ExternalStatus { get; set; }

        public string UpdatedTime { get; set; }

    }
}

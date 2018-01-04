using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App4Answers.Models.A4Amodels.Base;
using Google.Protobuf.WellKnownTypes;
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

        public Int64 EmailRecordId { get; set; }

        public string Subject { get; set; }

        public string MessageId { get; set; }

        public string NameFrom { get; set; }

        public string Url { get; set; }

        public string NameTo { get; set; }

        public A4AEmailStatus Status { get; set; }

        public string ServiceMessageId { get; set; }

        public string StatusMessage { get; set; }

        public Timestamp Timestamp { get; set; }

        public A4AEmailRecordType RecordType { get; set; }
    }
}

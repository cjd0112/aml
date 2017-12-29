using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App4Answers.Models.A4Amodels.Base;
using Microsoft.AspNetCore.Http;

namespace App4Answers.Models.A4Amodels.EmailManager
{
    public class A4AEmailRecordSummaryViewModel : ViewModelBase<A4AEmailRecord>
    {
        public A4AEmailRecordSummaryViewModel() : base(ModelNames.AdministrationNames.EmailRecord, ModelNames.Verb.List)
        {

        }
        public A4AEmailRecordSummaryViewModel(IFormCollection forms) : base(forms, ModelNames.AdministrationNames.EmailRecord,
            ModelNames.Verb.List)
        {

        }

        public A4AEmailRecordSummaryViewModel(A4AEmailRecord msg) : base(msg, ModelNames.AdministrationNames.EmailRecord,
            ModelNames.Verb.List)
        {

        }

        public string EmailRecordId { get; set; }

        public string Subject { get; set; }

        public string MessageId { get; set; }

        public string NameFrom { get; set; }

        public string NameTo { get; set; }

        public string Status { get; set; }

        public string ExternalMessageId { get; set; }

        public string ExternalStatus { get; set; }

        public string UpdatedTime { get; set; }

    }
}

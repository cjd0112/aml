using System;
using System.Collections.Generic;
using System.Text;

namespace As.Email
{
    public interface IEmailSender
    {
        IEnumerable<A4AEmailRecord> SendMail(A4AEmailService service,A4AMessage msg, A4AUser source, IEnumerable<A4AExpert> targets);
        EventsResponse GetNextMailEvents(A4AEmailService service);
        EmailPostResponse EmailFromUrl(A4AEmailService service, string url);
    }
}

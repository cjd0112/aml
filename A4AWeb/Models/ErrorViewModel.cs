using System;
using System.Text;


namespace A4AWeb.Models
{
    public class ErrorViewModel
    {
        public StringBuilder foo = new StringBuilder();
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App4Answers.Models.A4Amodels
{
    public class A4ALoginViewModel : ViewModelBase
    {
        public enum AuthenticationResult
        {
            None,
            Authenticated,
            PasswordInvalid,
            UserNotFound
        }

        public String Email { get; set; }

        public int Code1 { get; set; }

        public int Code2 { get; set; }

        public int Code3 { get; set; }
        public int Code4 { get; set; }

        public AuthenticationResult Authenticated { get; set; }


        public A4AAuthenticationAccount AuthenticationAccount { get; set; }

    }
}

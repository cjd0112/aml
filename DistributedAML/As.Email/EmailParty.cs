using System;
using System.Collections.Generic;
using System.Text;

namespace As.Email
{
    public class EmailParty
    {
        public EmailParty(string name, string email)
        {
            Name = name;
            Email = email;
        }
        public string Name { get; set; }

        public string Email { get; set; }
    }
}

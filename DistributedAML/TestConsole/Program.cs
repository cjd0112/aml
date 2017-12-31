using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using As.A4ACore;
using As.Email;
using As.Logger;
using Fasterflect;
using GraphQL;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace TestConsole
{
    class Program
    {

     


        static void Main(string[] args)
        {
            
            try
            {
                var z = new A4ARepository("DataSource=c:/as/client/db/A4ALogic.mdb");

                var result = z.GetMailbox(new MailboxRequest
                {
                    MailboxType = A4AMailboxType.Inbox,
                    PageSize = 10,
                    Start = 0,
                    Owner = "USER_0000",
                    UserType = A4AUserType.User
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.ReadLine();
        }
    }


}

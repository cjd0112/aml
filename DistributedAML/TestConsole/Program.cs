using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using As.A4ACore;
using As.Shared;
using Google.Protobuf;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            
            try
            {
                foreach (var tc in TypeContainer.Initialize(Assembly.GetAssembly(typeof(A4ACategory)).GetTypes()
                    .Where(predicate: x=> x.IsClass && typeof(IMessage).IsAssignableFrom(x))))
                {

                    tc
                        .FirstPropertyIsPrimaryKey()
                        .SetDelegateForCreateInstance()
                        .AddProperties(x =>
                            x.PropertyType == typeof(String) || !typeof(IEnumerable).IsAssignableFrom(x.PropertyType));
                }
                var z = new A4ARepository("DataSource=c:/as/client/db/A4ALogic.mdb");

                var result1 = z.GetMailbox(new MailboxRequest
                {
                    MailboxType = A4AMailboxType.Inbox,
                    PageSize = 10,
                    Start = 0,
                    Owner = "USER_0000",
                    UserType = A4APartyType.User
                });

                var result2 = z.GetMailbox(new MailboxRequest
                {
                    MailboxType = A4AMailboxType.Sent,
                    PageSize = 10,
                    Start = 0,
                    Owner = "USER_0000",
                    UserType = A4APartyType.User
                });

                Console.WriteLine("Inbox:");
                Console.WriteLine(result1.ToJSonString());

                Console.WriteLine("Sent:");
                Console.WriteLine(result2.ToJSonString());


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.ReadLine();
        }
    }


}

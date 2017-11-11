using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public static class Helper
    {
        public static String Prompt(String query)
        {
            Console.WriteLine(query);
            return Console.ReadLine();
        }
    }
}

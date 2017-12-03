using System;
using System.Runtime.InteropServices;

namespace As.Shared
{
    public static class Helper
    {
        public static String Prompt(String query)
        {
            Console.WriteLine(query);
            return Console.ReadLine();
        }

        public static OSPlatform GetPlatform()
        {
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return OSPlatform.Linux;
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return OSPlatform.Windows;
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return OSPlatform.OSX;

            return OSPlatform.Windows;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace As.Shared
{
    public static class DateTimeHelper
    {
        public static DateTime FromUnixTime(this DateTime foo, long unixTime)
        {
            DateTimeOffset dto = DateTimeOffset.FromUnixTimeSeconds(unixTime);
            return dto.DateTime;
        }

        public static long ToUnixTime(this DateTime foo)
        {
            DateTimeOffset dto = DateTimeOffset.FromFileTime(foo.ToFileTime());
            return dto.ToUnixTimeSeconds();
        }
    }
}

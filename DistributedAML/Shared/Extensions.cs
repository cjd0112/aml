using System;
using System.Collections;
using System.Collections.Generic;

namespace Shared
{
    public static class Extensions
    {
        public static void Do<T>(this IEnumerable<T> foo, Action<T> act)
        {
            foreach (var z in foo)
            {
                act(z);
            }
        }

    }
}

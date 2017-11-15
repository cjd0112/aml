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


        public static IEnumerable<T> BinarySearchMultiple<T>(this List<T> lst,T item,IComparer<T> foo)
        {
            int index = lst.BinarySearch(item, foo);
            if (index < 0)
                yield break;
            else
            {
                // lower_bound
                int lower_bound = index;
                while (index > 0 && foo.Compare(lst[index], item) == 0)
                {
                    lower_bound = index--;
                }
                int upper_bound = lower_bound;
                index = upper_bound;
                while (index < lst.Count && foo.Compare(lst[index], item) == 0)
                {
                    upper_bound = index++;

                }

                while (lower_bound <= upper_bound)
                {
                    yield return lst[lower_bound++];
                }
            }

        }

    }
}

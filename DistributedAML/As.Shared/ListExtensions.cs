using System;
using System.Collections.Generic;
using System.Linq;

namespace As.Shared
{
    public static class ListExtensions
    {
        public static void Do<T>(this IEnumerable<T> foo, Action<T> act)
        {
            foreach (var z in foo)
            {
                act(z);
            }
        }

        public static IEnumerable<IEnumerable<TValue>> Chunk<TValue>(
            this IEnumerable<TValue> values,
            Int32 chunkSize)
        {
            using (var enumerator = values.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    yield return GetChunk(enumerator, chunkSize).ToList();
                }
            }
        }
        private static IEnumerable<T> GetChunk<T>(
            IEnumerator<T> enumerator,
            int chunkSize)
        {
            do
            {
                yield return enumerator.Current;
            } while (--chunkSize > 0 && enumerator.MoveNext());
        }


        public static G Chunk<TValue,G>(
            this IEnumerable<TValue> values,
            Int32 chunkSize,Func<IEnumerable<TValue>,G> f,Func<G,G,G> agg)
        {
            G foo = default(G);
            using (var enumerator = values.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    agg(f(GetChunk(enumerator, chunkSize)),foo);
                }
            }
            return foo;
        }

        public static IEnumerable<G> Chunk<TValue, G>(
            this IEnumerable<TValue> values,
            Int32 chunkSize, Func<IEnumerable<TValue>, IEnumerable<G>> f) 
        {
            using (var enumerator = values.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    foreach (var c in f(GetChunk(enumerator, chunkSize)))
                    {
                        yield return c;
                    }
                }
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

        public static int IntAggregator(int x, int y)
        {
            return x + y;
        }

        public static IEnumerable<T> EnumerableAggregator<T>(IEnumerable<T> n, IEnumerable<T> z)
        {
            return n;
        }

    }
}

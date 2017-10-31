using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Comms;
using Google.Protobuf;

namespace AmlClient
{
    public class Multiplexer<T>
    {
        Dictionary<int,List<T>> objs = new Dictionary<int, List<T>>();
        private int Buckets;
        public Multiplexer(int buckets)
        {
            Buckets = buckets;

        }

        public void Add(String key, T o)
        {
            var bucket = Math.Abs(MurMurHash3.Hash(new MemoryStream(Encoding.UTF8.GetBytes(key)))) % Buckets;
            if (bucket != 0 && bucket != 1)
                throw new Exception("temp exception hit ...");
            List<T> vals = null;
            if (!objs.TryGetValue(bucket,out vals))
                objs[bucket] = vals = new List<T>();
            vals.Add(o);
        }

        public IEnumerable<Tuple<int, List<T>>> GetBuckets()
        {
            foreach (var c in objs.Keys)
            {
                yield return new Tuple<int, List<T>>(c,objs[c]);
            }            
        }


    }
}

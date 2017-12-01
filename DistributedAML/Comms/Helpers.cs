using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Google.Protobuf;
using LZ4;
using NetMQ;
using Shared;

namespace Comms
{
    public static class Helpers
    {
        public static NetMQMessage PackMessage<T>(this NetMQMessage msg, T foo) where T : IMessage
        {
            byte[] buff = new byte[foo.CalculateSize()];
            var writer = new MemoryStream(buff);
            foo.WriteDelimitedTo(writer);
            msg.Append(LZ4Codec.Wrap(buff));
            return msg;
        }

        public static T UnpackMessage<T>(this NetMQMessage msg, Func<Stream, T> parseObject) where T : IMessage
        {
            var buff = msg.Pop().ToByteArray();
            var rdr = new MemoryStream(LZ4Codec.Unwrap(buff));
            return parseObject(rdr);
        }

        public static NetMQMessage PackMessageList<T>(this NetMQMessage msg, IEnumerable<T> foo) where T : IMessage
        {
            msg.Append(foo.Count());
            int size = foo.Aggregate(0, (x, y) => x+ y.CalculateSize()+sizeof(int));
            byte[] buff = new byte[size];
            var writer = new MemoryStream(buff);
            foreach (var z in foo)
            {
                z.WriteDelimitedTo(writer);
            }
            msg.Append(LZ4Codec.Wrap(buff));
            return msg;
        }

        public static IEnumerable<T> UnpackMessageList<T>(this NetMQMessage msg,Func<Stream,T> parseObject) where T:IMessage
        {
            var cnt = (int) msg.Pop().ConvertToInt32();
            var buff = msg.Pop().ToByteArray();
            var rdr = new MemoryStream(LZ4Codec.Unwrap(buff));
            List<T> ret = new List<T>();
            while (cnt--> 0)
            {
                ret.Add(parseObject(rdr));
            }
            return ret;
        }

        public static NetMQMessage PackMessageListString( this NetMQMessage msg, IEnumerable<String> s)
        {
            msg.Append(s.Count());
            int size = s.Aggregate(0, (x, y) => x + y.Length + sizeof(int));
            byte[] buff = new byte[size];
            var writer = new BinaryWriter(new MemoryStream(buff));
            foreach (var z in s)
            {
                var bytes = Encoding.UTF8.GetBytes(z);
                writer.Write(bytes.Length);
                writer.Write(bytes,0,bytes.Length);
            }
            msg.Append(LZ4Codec.Wrap(buff));
            return msg;

        }

        public static IEnumerable<String> UnpackMessageListString(this NetMQMessage msg)
        {
            var cnt = (int) msg.Pop().ConvertToInt32();
            var buff = msg.Pop().ToByteArray();
            var rdr = new BinaryReader(new MemoryStream(LZ4Codec.Unwrap(buff)));        
            List<String> ret = new List<String>();
            while (cnt-- > 0)
            {
                var bytes = rdr.ReadInt32();
                ret.Add(Encoding.UTF8.GetString(rdr.ReadBytes(bytes)));
            }
            return ret;

        }

       

        public static void AddParameter(this NetMQMessage msg, Object o)
        {
            if (o is null)
                throw new Exception($"Null value passed to addParameter");

            if (o is String)
                msg.Append((String) o);
            else if (o is Int32)
                msg.Append((int) o);
            else if (o is Int64)
                msg.Append((Int64) o);
            else if (o.GetType().IsEnum)
                msg.Append( o.ToString());
            else
            {
                throw new Exception($"Unexpected parameters for message - {o.GetType()}");
            }


        }

        public static NetMQMessage PackMessageListInt32(this NetMQMessage msg, IEnumerable<Int32> s)
        {
            msg.Append(s.Count());
            int size = s.Count() * sizeof(int);
            byte[] buff = new byte[size];
            var writer = new BinaryWriter(new MemoryStream(buff));
            foreach (var z in s)
            {
                writer.Write(z);
            }
            msg.Append(LZ4Codec.Wrap(buff));
            return msg;
        }


        public static IEnumerable<Int32> UnpackMessageListInt32(this NetMQMessage msg)
        {
            var cnt = (int)msg.Pop().ConvertToInt32();
            var buff = msg.Pop().ToByteArray();
            var rdr = new BinaryReader(new MemoryStream(LZ4Codec.Unwrap(buff)));
            List<int> ret = new List<int>();
            while (cnt-- > 0)
            {
                ret.Add(rdr.ReadInt32());
            }
            return ret;

        }

        public static NetMQMessage PackMessageListInt64(this NetMQMessage msg, List<Int64> s)
        {
            msg.Append(s.Count());
            int size = s.Count() * sizeof(Int64);
            byte[] buff = new byte[size];
            var writer = new BinaryWriter(new MemoryStream(buff));
            foreach (var z in s)
            {
                writer.Write(z);
            }
            msg.Append(LZ4Codec.Wrap(buff));
            return msg;
        }

        public static List<Int64> UnpackMessageListInt64(this NetMQMessage msg)
        {
            var cnt = (int)msg.Pop().ConvertToInt32();
            var buff = msg.Pop().ToByteArray();
            var rdr = new BinaryReader(new MemoryStream(LZ4Codec.Unwrap(buff)));
            List<Int64> ret = new List<Int64>();
            while (cnt-- > 0)
            {
                ret.Add(rdr.ReadInt64());
            }
            return ret;

        }

    }
}

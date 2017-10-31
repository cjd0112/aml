using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Google.Protobuf;
using NetMQ;

namespace Comms
{
    public class Helpers
    {
        public static NetMQMessage PackMessageList<T>(NetMQMessage msg, List<T> foo) where T : IMessage
        {
            msg.Append(foo.Count);
            int size = foo.Aggregate(0, (x, y) => x+ y.CalculateSize()+sizeof(int));
            byte[] buff = new byte[size];
            var writer = new MemoryStream(buff);
            foreach (var z in foo)
            {
                z.WriteDelimitedTo(writer);
            }
            msg.Append(buff);
            return msg;
        }

        public static List<T> UnpackMessageList<T>(NetMQMessage msg,Func<Stream,T> parseObject) where T:IMessage
        {
            var cnt = (int) msg.Pop().ConvertToInt32();
            var buff = msg.Pop().ToByteArray();
            var rdr = new MemoryStream(buff);
            List<T> ret = new List<T>();
            while (cnt--> 0)
            {
                ret.Add(parseObject(rdr));
            }
            return ret;
        }

        public static NetMQMessage PackMessageListString( NetMQMessage msg, List<String> s)
        {
            foreach (var z in s)
            {
                msg.Append(z);
            }
            return msg;

        }

        public static List<String> UnpackMessageListString(NetMQMessage msg)
        {
            var cnt = msg.FrameCount;
            List<String> ret = new List<String>();
            while (cnt-- > 0)
            {
                ret.Add(msg.Pop().ConvertToString());
            }
            return ret;

        }

        public static NetMQMessage PackMessageListInt32(NetMQMessage msg, List<Int32> s)
        {
            foreach (var z in s)
            {
                msg.Append(z);
            }
            return msg;

        }


        public static List<Int32> UnpackMessageListInt32(NetMQMessage msg)
        {
            var cnt = msg.FrameCount;
            List<Int32> ret = new List<Int32>();
            while (cnt-- > 0)
            {
                ret.Add(msg.Pop().ConvertToInt32());
            }
            return ret;

        }

        public static NetMQMessage PackMessageListInt64( NetMQMessage msg, List<Int64> s)
        {
            foreach (var z in s)
            {
                msg.Append(z);
            }
            return msg;

        }

        public static List<Int64> UnpackMessageListInt64(NetMQMessage msg)
        {
            var cnt = msg.FrameCount;
            List<Int64> ret = new List<Int64>();
            while (cnt-- > 0)
            {
                ret.Add(msg.Pop().ConvertToInt64());
            }
            return ret;

        }

    }
}

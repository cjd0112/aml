using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQL.GraphQLSerializer
{
    public class UnderlyingList : UnderlyingJson
    {
        private List<Object> listValues = new List<Object>();

        public UnderlyingList(int pushCount) : base(pushCount)
        {

        }

        public override void AddValue(int pos, String name, Object o)
        {
            listValues.Add(o);
        }

        public override void OnClosed()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(" [");
            if (listValues.Count > 0)
                builder.AppendLine();
            for (int i = 0; i < listValues.Count; i++)
            {
                builder.Append(' ', PushCount);

                JSonSerialize(listValues[i], builder);

                if (i != listValues.Count - 1)
                {
                    builder.AppendLine(",");
                }
                else
                {
                    builder.AppendLine();
                }
            }
            if (listValues.Count > 0)
                builder.Append(' ', PushCount);
            builder.Append("]");
            listValues.Clear();
            StringRepresentation = builder.ToString();

        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace AMLWorker
{
    public class StringFunctions
    {
        public static String TripleQuery(String phrase)
        {
            //            ^ ale AND(esh OR hia OR iew OR a_T OR Tom OR mki OR iew)

            if (phrase.Length < 4)
                return phrase;

            StringBuilder b = new StringBuilder();
            b.Append("^");
            b.Append(phrase.Substring(0, 3));
            b.Append(" ");
            b.Append(" AND (");
            int tripleChars = 1;
            for (int cnt = 4; cnt < phrase.Length; cnt++, tripleChars++)
            {
                char z = phrase[cnt];
                if (z == ' ')
                    z = '_';
                b.Append(z);
                if (tripleChars % 3 == 0)
                {
                    b.Append(" OR ");
                    cnt -= 2;
                }

            }
            // strip off last " OR "
            if (b.ToString(b.Length-4,4) == " OR ")
                b.Remove(b.Length - 4,4);
            b.Append(")");
            return b.ToString();
        }

        public static String CreateTriple(String phrase)
        {
            StringBuilder b = new StringBuilder();
            int tripleChars = 1;
            for (int cnt = 0; cnt < phrase.Length; cnt++, tripleChars++)
            {
                char z = phrase[cnt];
                if (z == ' ')
                    z = '_';
                b.Append(z);
                if (tripleChars % 3 == 0)
                {
                    b.Append(' ');
                    cnt -= 2;
                }
            }
            return b.ToString();
        }


        public static double LevensteinDistance(string s, string t)
        {
            // degenerate cases
            if (s == t) return 0;
            if (s.Length == 0) return t.Length;
            if (t.Length == 0) return s.Length;

            // create two work vectors of integer distances
            int[] v0 = new int[t.Length + 1];
            int[] v1 = new int[t.Length + 1];

            // initialize v0 (the previous row of distances)
            // this row is A[0][i]: edit distance for an empty s
            // the distance is just the number of characters to delete from t
            for (int i = 0; i < v0.Length; i++)
                v0[i] = i;

            for (int i = 0; i < s.Length; i++)
            {
                // calculate v1 (current row distances) from the previous row v0

                // first element of v1 is A[i+1][0]
                //   edit distance is delete (i+1) chars from s to match empty t
                v1[0] = i + 1;

                // use formula to fill in the rest of the row
                for (int j = 0; j < t.Length; j++)
                {
                    var cost = (s[i] == t[j]) ? 0 : 1;
                    v1[j + 1] = Math.Min(Math.Min(v1[j] + 1, v0[j + 1] + 1), v0[j] + cost);
                }

                // copy v1 (current row) to v0 (previous row) for next iteration
                for (int j = 0; j < v0.Length; j++)
                    v0[j] = v1[j];
            }

            return v1[t.Length];
        }
        
    }
}

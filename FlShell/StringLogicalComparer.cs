using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlShell
{

    /// <summary>
    /// Compares string such that strings containing numeric values will, assuming that non-numeric leading portions
    /// are equal, will sort in numeric order. Specifically, the strings: "a1", "a101", "a3" will sort as:
    /// "a1", "a3", "a101"
    /// </summary>
    /// <remarks><list type="bullet">
    /// <item><description>Article, code, and forum additions are found at:</description></item>
    /// <item><description>http://www.codeproject.com/cs/algorithms/csnsort.asp</description></item>
    /// <item><description>Original C# code by Vasian Cepa</description></item>
    /// <item><description>Optimized C# code by Richard Deeming</description></item>
    /// <item><description>Translated to VB.Net by Mike Cattle</description></item>
    /// <item><description>Corrected version of CompareNumbers by Jim Parsells</description></item>
    /// </list>
    /// </remarks>
    public class StringLogicalComparer : IComparer
    {

        private static StringLogicalComparer _default; // = new StringLogicalComparer();

        private StringLogicalComparer()
        {
            _default = new StringLogicalComparer();
        } //New

        /// <summary>
        /// returns an Instance of StringLogicalComparer
        /// </summary>
        /// <returns>an Instance of StringLogicalComparer</returns>
        public static IComparer Default
        {
            get
            {
                return _default;
            }
        }

        /// <summary>
        /// Compares two Objects which must be Strings. Allows for and Compares properly if one or both Strings are Nothing. 
        /// <para>When given two initialized Strings, Compares them using 
        /// <see cref="ExpTreeLib.StringLogicalComparer.CompareStrings">the CompareStrings function of this Class</see></para>
        /// </summary>
        /// <param name="x">First String to Compare</param>
        /// <param name="y">Second String to Compare</param>
        /// <returns>Negative value if x less than y, 0 if x=y, or a positive value if x greater than y</returns>
        /// <remarks></remarks>
        public int Compare(object x, object y)
        {

            if (x == null && y == null)
                return 0;



            if (x == null)
                return -1;



            if (y == null)
                return 1;



            if (x.GetType().Equals(typeof(string)) && y.GetType().Equals(typeof(string)))
                return CompareStrings(Convert.ToString(x), Convert.ToString(y));

            return Comparer.Default.Compare(x, y);
        } //Compare

        /// <summary>
        /// Compares string such that strings containing numeric values will, assuming that non-numeric leading portions
        /// are equal, will sort in numeric order. Specifically, the strings: "a1", "a101", "a3" will sort as:
        /// "a1", "a3", "a101"
        /// </summary>
        /// <param name="s1">First String to Compare</param>
        /// <param name="s2">Second String to Compare</param>
        /// <returns>Negative value if s1 less than s2, 0 if s1=s2, positive value if s1 greater than s2</returns>
        /// <remarks>Note that negative return values may be other than -1 and that positive return values may be other than 1</remarks>
        public static int CompareStrings(string s1, string s2)
        {
            if (s1 == null || s1.Length == 0)
            {
                if (s2 == null || s2.Length == 0)
                    return 0;

                return -1;
            }
            else if (s2 == null || s2.Length == 0)
                return 1;


            int s1Length = s1.Length;
            int s2Length = s2.Length;

            bool sp1 = Char.IsLetterOrDigit(s1[0]);
            bool sp2 = Char.IsLetterOrDigit(s2[0]);

            if (sp1 && !sp2)
                return 1;

            if (!sp1 && sp2)
                return -1;

            char c1;
            char c2;
            int i1 = 0;
            int i2 = 0;
            int r = 0;
            bool letter1;
            bool letter2;

            while (true)
            {
                c1 = s1[i1];
                c2 = s2[i2];

                sp1 = Char.IsDigit(c1);
                sp2 = Char.IsDigit(c2);

                if (!sp1 && !sp2)
                {
                    if (c1 != c2)
                    {
                        letter1 = Char.IsLetter(c1);
                        letter2 = Char.IsLetter(c2);

                        if (letter1 && letter2)
                        {
                            c1 = Char.ToUpper(c1);
                            c2 = Char.ToUpper(c2);

                            r = (int)c1 - (int)c2;
                            if (0 != r)
                                return r;
                        }
                        else if (!letter1 && !letter2)
                        {
                            r = (int)c1 - (int)c2;
                            if (0 != r)
                                return r;
                        }
                        else if (letter1)
                            return 1;

                        else if (letter2)
                            return -1;

                    }
                }
                else if (sp1 && sp2)
                {
                    r = CompareNumbers(s1, s1Length, ref i1, s2, s2Length, ref i2);
                    if (0 != r)
                        return r;
                }
                else if (sp1)
                    return -1;
                else if (sp2)
                    return 1;

                i1 += 1;
                i2 += 1;


                if (i1 >= s1Length)
                {
                    if (i2 >= s2Length)
                        return 0;

                    return -1;
                }
                else if (i2 >= s2Length)
                    return 1;
            } //while
        }



        private static int CompareNumbers(string s1, int s1Length, ref int i1, string s2, int s2Length, ref int i2)
        {
            int nzStart1 = i1;
            int nzStart2 = i2;
            int end1 = i1;
            int end2 = i2;

            ScanNumber(s1, s1Length, i1, ref nzStart1, ref end1);
            ScanNumber(s2, s2Length, i2, ref nzStart2, ref end2);

            int start1 = i1;
            i1 = end1 - 1;
            int start2 = i2;
            i2 = end2 - 1;

            int length1 = end2 - nzStart2;
            int length2 = end1 - nzStart1;

            if (length1 == length2)
            {
                int r;
                int j1 = nzStart1;
                int j2 = nzStart2;
                do
                {
                    r = Convert.ToInt32(s1[j1]) - Convert.ToInt32(s2[j2]);
                    if (0 != r)
                        return r;

                    j1 += 1;
                    j2 += 1;
                }
                while (j1 <= i1);

                length1 = end1 - start1;
                length2 = end2 - start2;

                if (length1 == length2)
                    return 0;

            }

            if (length1 > length2)
                return -1;

            return 1;
        }

        private static void ScanNumber(string s, int length, int start, ref int nzStart, ref int end)
        {
            nzStart = start;
            end = start;

            bool countZeros = true;
            char c = s[end];

            while (true)
            {
                if (countZeros)
                {
                    if ((char)0 == c)
                        nzStart += 1;
                    else
                        countZeros = false;

                }

                end += 1;
                if (end >= length)
                    break;

                c = s[end];
                if (!char.IsDigit(c))
                    break;

            }
        } //ScanNumber

    } //StringLogicalComparer
}

using System;
using System.Collections.Generic;
using System.Windows.Forms;


namespace Karaboss.Pages.ABCnotation
{

    // This class handles sorting for a listview in details mode. The tag property for each column
    // should be set to one of the SortType enumeration. 
    //
    // TITLE sorting ignores A, AN, and THE as well as skipping any punctuation like quotes at the beginning.
    // PATH sorting handles the case where a file and a directory are at the same level in the tree
    // DEFAULT is just standard string sorting
    // DATE and INTEGER convert to the type and compare. Note that these are not horribly efficient as coded; they may do an additional Parse() step.
    public enum SortType { DEFAULT, TITLE, PATH, DATE, INTEGER };
    public class ColumnSorter : System.Collections.IComparer
    {
        public SortType SortType { get; set; } // Yay for new 2008 simple property syntax!
        public int CurrentCol { get; set; }
        protected List<String> _lstIgnore = new List<string>();
        protected static char[] PATH_SEPARATORS = @"/\".ToCharArray();
        protected static String CURRENT_DIR = @".\";


        public ColumnSorter()
        {//--------------------------------------------------------------------
            // Remove A, An, and The for purposes of comparing
            _lstIgnore.Add("a ");
            _lstIgnore.Add("an ");
            _lstIgnore.Add("the ");
            return;
        }


        int System.Collections.IComparer.Compare(object x, object y)
        {//====================================================================
            String strA = ((ListViewItem)x).SubItems[CurrentCol].Text;
            String strB = ((ListViewItem)y).SubItems[CurrentCol].Text;
            switch (SortType)
            {
                default: throw new Exception("Unknown sort type!");
                case SortType.DEFAULT: return String.Compare(strA.TrimStart(), strB.TrimStart());
                case SortType.TITLE: return SortTitle(strA, strB);
                case SortType.PATH: return SortPath(strA, strB);
                case SortType.DATE: return DateTime.Parse(strA) < DateTime.Parse(strB) ? -1 : DateTime.Parse(strA) == DateTime.Parse(strB) ? 0 : 1;
                case SortType.INTEGER:
                    if (strA == "") return 1;
                    else if (strB == "") return -1;
                    else
                    {
                        int a = 0; int b = 0;
                        try { a = int.Parse(strA); } catch { };
                        try { b = int.Parse(strB); } catch { };
                        return a < b ? -1 : a == b ? 0 : 1;
                    }
            }
        }

        private int SortTitle(String strA, String strB)
        {//--------------------------------------------------------------------
            // Remove quotes, spaces, dashes, etc.
            if (strA.Length > 0) { int i = 0; while (!Char.IsLetterOrDigit(strA[i])) i += 1; if (i > 0) strA = strA.Substring(i); }
            if (strB.Length > 0) { int j = 0; while (!Char.IsLetterOrDigit(strB[j])) j += 1; if (j > 0) strB = strB.Substring(j); }

            // Remove any prefixes we want to ignore
            // Note that this code will turn "A to Z Waltz" into "to Z Waltz" 
            // for comparison, putting it in the Ts. 
            //
            // This is not necessarily desirable, but is a PITA to fix.
            //
            // Also note that multiple articles are ignored and articles are 
            // searched in a set order. "The A Train" becomes "A Train" but 
            // "A The Train" is just "Train"  
            //
            // All sorts of weirdness in the edge cases
            foreach (String s in _lstIgnore)
            {
                if (strA.StartsWith(s, StringComparison.CurrentCultureIgnoreCase)) strA = strA.Substring(s.Length);
                if (strB.StartsWith(s, StringComparison.CurrentCultureIgnoreCase)) strB = strB.Substring(s.Length);
            }
            return String.Compare(strA, strB);
        }

        private int SortPath(String strA, String strB)
        {//--------------------------------------------------------------------
         // One or more has as subdir in it
         // Cases to consider:
         //   bbb.abc           stuff/aaa.abc > bbb is first
         //   stuff/bbb.abc     tmp/aaa.abc   > bbb is first
         //   stuff/zzz/bbb.abc tmp/aaa.abc   > bbb is first

            // We're going to make parallel-but-possibly-ragged arrays of
            // the subdir structure and compare parallel depths. We need to 
            // fake up a dir name for files in the root so we have parallelism: 
            // .\zzz.abc and stuff\bbb.abc have to compare at the same depths.
            // Without this we compare zzz.abc to stuff, which is wrong. The 
            // root should always sort to the top (simulating a depth-first 
            // recursion like dir /s)
            if (strA.IndexOfAny(PATH_SEPARATORS) == -1) strA = CURRENT_DIR + strA;
            if (strB.IndexOfAny(PATH_SEPARATORS) == -1) strB = CURRENT_DIR + strB;

            String[] aPartsA = strA.Split(PATH_SEPARATORS);
            String[] aPartsB = strB.Split(PATH_SEPARATORS);

            // Compare parallel elements of the arrays
            for (int i = 0; i < (aPartsA.Length < aPartsB.Length ? aPartsA.Length : aPartsB.Length); i += 1)
            {
                if (aPartsA[i] != aPartsB[i]) return String.Compare(aPartsA[i], aPartsB[i]);
            }

            // Okay, we walked off the end of one of the arrays, so return the lower 
            // depth of the two as first
            return aPartsA.Length - aPartsB.Length; // Will be negative if B is longer, making A first. And vice-versa.
        }


    }
}

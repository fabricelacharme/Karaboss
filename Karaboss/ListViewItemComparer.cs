#region License

/* Copyright (c) 2018 Fabrice Lacharme
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to 
 * deal in the Software without restriction, including without limitation the 
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
 * sell copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software. 
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 * THE SOFTWARE.
 */

#endregion

#region Contact

/*
 * Fabrice Lacharme
 * Email: fabrice.lacharme@gmail.com
 */

#endregion
using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace Karaboss
{
    class ListViewItemComparer : IComparer
    {
        private int col;
        private SortOrder order;
        //private CaseInsensitiveComparer ObjectCompare;
        private MyComparer ObjectCompare;


        private class MyComparer : CaseInsensitiveComparer
        {
            public new int Compare(object x, object y)
            {
                try
                {
                    string s1 = x.ToString();
                    string s2 = y.ToString();

                    // check for a numeric column
                    decimal n1, n2 = 0;
                    if (Decimal.TryParse(s1, out n1) && Decimal.TryParse(s2, out n2))
                        return base.Compare(n1, n2);
                    else
                    {
                        // check for a date column
                        DateTime d1, d2;
                        if (DateTime.TryParse(s1, out d1) && DateTime.TryParse(s2, out d2))
                            return base.Compare(d1, d2);
                    }
                }
                catch (ArgumentException) { }

                // just use base string compare
                return base.Compare(x, y);
            }
        }

        public ListViewItemComparer(int column, SortOrder order)
        {
            col = column;
            this.order = order;
            //CaseInsensitiveComparer ObjectCompare = new CaseInsensitiveComparer();
            ObjectCompare = new MyComparer();
        }

        public int Compare(object x, object y)
        {
            int compareResult;
            ListViewItem listviewX, listviewY;

            // Envoit les objets à comparer aux objets ListViewItem
            listviewX = (ListViewItem)x;
            listviewY = (ListViewItem)y;

            // Compare les deux éléments
            compareResult = ObjectCompare.Compare(listviewX.SubItems[col].Text, listviewY.SubItems[col].Text);

            if (compareResult == 0)
            {
                listviewX.SubItems[0].ForeColor = Color.Red;
                listviewY.SubItems[0].ForeColor = Color.Red;
            }

            // Calcule la valeur correcte d'après la comparaison d'objets
            if (order == SortOrder.Ascending)
            {
                // Le tri croissant est sélectionné, renvoie des résultats normaux de comparaison
                return compareResult;
            }
            else if (order == SortOrder.Descending)
            {
                // Le tri décroissant est sélectionné, renvoie des résultats négatifs de comparaison
                return (-compareResult);
            }
            else
            {
                // Renvoie '0' pour indiquer qu'ils sont égaux
                return 0;
            }
        }
    }

}

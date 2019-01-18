#region License

/* Copyright (c) 2016 Fabrice Lacharme
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
//
//https://www.ipentec.com/document/document.aspx?page=csharp-shell-namespace-create-explorer-list-view-control
//
// dragging with insert
// http://www.cyotek.com/blog/dragging-items-in-a-listview-control-with-visual-insertion-guides
//
#endregion
using FlShell;
using System.Collections;
using System.Windows.Forms;

/// <summary>
/// Cette classe est une implémentation de l'interface 'IComparer'.
/// https://support.microsoft.com/fr-fr/kb/319401
/// </summary>
public class ListViewColumnSorter : IComparer
{
    /// <summary>
    /// Spécifie la colonne à trier
    /// </summary>
    private int ColumnToSort;
    /// <summary>
    /// Spécifie l'ordre de tri (en d'autres termes 'Croissant').
    /// </summary>
    private SortOrder OrderOfSort;
    /// <summary>
    /// Objet de comparaison ne respectant pas les majuscules et minuscules
    /// </summary>
    private CaseInsensitiveComparer ObjectCompare;

    private SortBy SortColBy;


    /// <summary>
    /// Constructeur de classe.  Initializes various elements
    /// </summary>
    public ListViewColumnSorter()
    {
        // Initialise la colonne sur '0'
        ColumnToSort = 0;

        // Initialise l'ordre de tri sur 'aucun'
        OrderOfSort = SortOrder.None;
        

        // Initialise l'objet CaseInsensitiveComparer
        ObjectCompare = new CaseInsensitiveComparer();

        SortColBy = SortBy.Text;

    }

    /// <summary>
    /// Cette méthode est héritée de l'interface IComparer.  Il compare les deux objets passés en effectuant une comparaison 
    ///qui ne tient pas compte des majuscules et des minuscules.
    /// </summary>
    /// <param name="x">Premier objet à comparer</param>
    /// <param name="x">Deuxième objet à comparer</param>
    /// <returns>Le résultat de la comparaison. "0" si équivalent, négatif si 'x' est inférieur à 'y' 
    ///et positif si 'x' est supérieur à 'y'</returns>
    public int Compare(object x, object y)
    {
        int compareResult = 0;
        ListViewItem listviewX, listviewY;

        // Envoit les objets à comparer aux objets ListViewItem
        listviewX = (ListViewItem)x;
        listviewY = (ListViewItem)y;

        if (OrderOfSort == SortOrder.None)
            OrderOfSort = SortOrder.Ascending;

        // Folders always come first, and both folders and files are sorted alphabetically 
        // http://stackoverflow.com/questions/7549607/grouping-and-sorting-folders-and-files-in-a-listview
        // If both X and Y items are of the same type(Folder or File): return the "normal" result based on the item name sort.
        // And if not, return -1 if X is a folder and 1 if not.        
        if (((ShellItem)listviewX.Tag).IsFolder && !((ShellItem)listviewY.Tag).IsFolder)
            return -1;

        if (!((ShellItem)listviewX.Tag).IsFolder && ((ShellItem)listviewY.Tag).IsFolder)
            return 1;


        // Compare les deux éléments
        switch (SortColBy)
        {
            case SortBy.Text:
                compareResult = ObjectCompare.Compare(listviewX.SubItems[ColumnToSort].Text, listviewY.SubItems[ColumnToSort].Text);
                break;
            case SortBy.Tag:
                compareResult = ObjectCompare.Compare(listviewX.SubItems[ColumnToSort].Tag, listviewY.SubItems[ColumnToSort].Tag);
                break;
        }

        // Calcule la valeur correcte d'après la comparaison d'objets
        if (OrderOfSort == SortOrder.Ascending)
        {
            // Le tri croissant est sélectionné, renvoie des résultats normaux de comparaison
            return compareResult;
        }
        else if (OrderOfSort == SortOrder.Descending)
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

    /// <summary>
    /// Obtient ou définit le numéro de la colonne à laquelle appliquer l'opération de tri (par défaut sur '0').
    /// </summary>
    public int SortColumn
    {
        set
        {
            ColumnToSort = value;
        }
        get
        {
            return ColumnToSort;
        }
    }

    /// <summary>
    /// Obtient ou définit l'ordre de tri à appliquer (par exemple, 'croissant' ou 'décroissant').
    /// </summary>
    public SortOrder Order
    {
        set
        {
            OrderOfSort = value;
        }
        get
        {
            return OrderOfSort;
        }
    }

    public SortBy SortColumnBy {
        get {return SortColBy;}
        set {SortColBy = value;}
    }
   

    public enum SortBy
    {
        Text = 1,
        Tag,
    }

}

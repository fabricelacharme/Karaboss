using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace GradientApp
{
    public static class cboColor 
    {

        // Margins around owner drawn ComboBoxes.
        private const int MarginWidth = 3;
        private const int MarginHeight = 3;


        // Set up the ComboBox to display color samples and their names.
        public static void DisplayKnownColors(ComboBox cbo)
        {
            // Load knowcolors colors into the combo box
            var wcolors = Enum.GetValues(typeof(KnownColor))
              .Cast<KnownColor>()
              .Where(k => k >= KnownColor.Transparent && k < KnownColor.ButtonFace) // Exclure les couleurs système
              .Select(k => Color.FromKnownColor(k))
              .OrderBy(c => c.GetHue())
              .ThenBy(c => c.GetSaturation())
              .ThenBy(c => c.GetBrightness())              
              .ToList();

            // Make the ComboBox owner-drawn.
            cbo.DrawMode = DrawMode.OwnerDrawFixed;
            cbo.DropDownStyle = ComboBoxStyle.DropDownList;

            // Add the colors to the ComboBox's items.
            cbo.Items.Clear();            
            foreach (Color color in wcolors) cbo.Items.Add(color);

            // Subscribe to the DrawItem event.
            cbo.DrawItem += cboKnownColors_DrawItem;
        }
      

        private static void cboKnownColors_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            var cbo = sender as ComboBox;
            Color foreColor = e.ForeColor;

            Rectangle rect;

            // Clear the background appropriately.
            if (e.State.HasFlag(DrawItemState.Selected) && !(e.State.HasFlag(DrawItemState.ComboBoxEdit)))
            {
                // Draw the background color of the ComboBox item.
                e.DrawBackground();
                //e.DrawFocusRectangle(); // <= could be removed for a cleaner rendering
            }
            else
            {
                // Draw the background color of the ComboBox item.
                using (var brush = new SolidBrush(cbo.BackColor))
                {
                    rect = e.Bounds;
                    rect.Inflate(1, 1);
                    e.Graphics.FillRectangle(brush, rect);
                }
                foreColor = cbo.ForeColor;
            }

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw the color rectangle.
            int hgt = e.Bounds.Height - 2 * MarginHeight;
            rect = new Rectangle(
                e.Bounds.X + MarginWidth,
                e.Bounds.Y + MarginHeight,
                (int)(1.7 * hgt), hgt - 1);
            
            Color color = (Color)cbo.Items[e.Index];
            using (SolidBrush brush = new SolidBrush(color))
            {
                e.Graphics.FillRectangle(brush, rect);
            }

            // Outline the rectangle in black.
            e.Graphics.DrawRectangle(Pens.Black, rect);

            // Draw the text color's name to the right.
            TextRenderer.DrawText(e.Graphics, cbo.GetItemText(cbo.Items[e.Index]), e.Font,  new Point(2 * hgt + MarginWidth, 1 + e.Bounds.Y), foreColor);
            
        }




        // Images with different sizes can be displayed in a ComboBox.
        #region Images with Different Sizes

        // Set up the ComboBox to display images.
        public static void DisplayImages2(this ComboBox cbo, Image[] images)
        {
            // Make the ComboBox owner-drawn.
            cbo.DrawMode = DrawMode.OwnerDrawVariable;

            // Add the images to the ComboBox's items.
            cbo.Items.Clear();
            foreach (Image image in images) cbo.Items.Add(image);

            // Subscribe to the DrawItem event.
            cbo.MeasureItem += cboDrawImage_MeasureItem2;
            cbo.DrawItem += cboDrawImage_DrawItem2;
        }

        // Measure a ComboBox item that is displaying an image.
        private static void cboDrawImage_MeasureItem2(object sender, MeasureItemEventArgs e)
        {
            if (e.Index < 0) return;

            // Get this item's image.
            ComboBox cbo = sender as ComboBox;
            Image img = (Image)cbo.Items[e.Index];
            e.ItemHeight = img.Height + 2 * MarginHeight;
            e.ItemWidth = img.Width + 2 * MarginWidth;
        }

        // Draw a ComboBox item that is displaying an image.
        private static void cboDrawImage_DrawItem2(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            // Clear the background appropriately.
            e.DrawBackground();

            // Draw the image.
            ComboBox cbo = sender as ComboBox;
            Image img = (Image)cbo.Items[e.Index];
            float hgt = e.Bounds.Height - 2 * MarginHeight;
            float scale = hgt / img.Height;
            float wid = img.Width * scale;
            RectangleF rect = new RectangleF(
                e.Bounds.X + MarginWidth,
                e.Bounds.Y + MarginHeight,
                wid, hgt);
            e.Graphics.InterpolationMode =
                InterpolationMode.HighQualityBilinear;
            e.Graphics.DrawImage(img, rect);

            // Draw the focus rectangle if appropriate.
            e.DrawFocusRectangle();
        }

        #endregion Images with Different Sizes


    }
    // Ajoutez cette méthode d'extension pour ComboBox afin de corriger l'erreur CS1061.
    // Cette méthode suppose que les éléments du ComboBox sont de type Image ou Color.
    // Si l'élément est une Image, il la retourne, sinon retourne null.

    public static class ComboBoxExtensions
    {
        public static Image GetItemImage(this ComboBox cbo, int index)
        {
            if (index < 0 || index >= cbo.Items.Count)
                return null;

            var item = cbo.Items[index];
            return item as Image;
        }
    }
}

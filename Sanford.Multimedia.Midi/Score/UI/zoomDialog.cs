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

#endregion
using System;
using System.Linq;
using System.Windows.Forms;


namespace Sanford.Multimedia.Midi.Score.UI
{
    public partial class frmZoomDialog : Form
    {      

        SheetMusic sheetmusic;
        float newzoom = 1.0f;

        public decimal zoom
        {
            get
            {
                return this.updZoom.Value;
            }
        }
        
        public frmZoomDialog(SheetMusic fsheetmusic, float fzoom)
        {
            InitializeComponent();
            updZoom.Value = 100;
            updZoom.Maximum = 400;
            updZoom.Minimum = 50;
            updZoom.Increment = 10;
            
            sheetmusic = fsheetmusic;
            
            newzoom = fzoom;
            updZoom.Value = Convert.ToDecimal(fzoom * 100);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {

        }

        private void updZoom_ValueChanged(object sender, EventArgs e)
        {

            float nzoom = Convert.ToInt32(zoom) / 100f;
            sheetmusic.SetZoom(nzoom);

          

        }

        /// <summary>
        /// Locate form
        /// </summary>
        /// <typeparam name="TForm"></typeparam>
        /// <returns></returns>
        private TForm getForm<TForm>()
            where TForm : Form
        {
            return (TForm)Application.OpenForms.OfType<TForm>().FirstOrDefault();
        }

    }
}

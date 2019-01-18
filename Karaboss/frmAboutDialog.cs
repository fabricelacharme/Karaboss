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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Karaboss
{
    public partial class frmAboutDialog : Form
    {
        public frmAboutDialog()
        {
            InitializeComponent();
            
            string tx = "Karaoke Player" + "\r\n";
            tx += "Created by: Fabrice Lacharme" + "\r\n";
            tx += "fabrice.lacharme@gmail.com" + "\r\n";
            tx += "Version: " + Application.ProductVersion;

            lblAbout.Text = tx;

            try
            {
                Uri uri = new Uri(@Properties.Settings.Default.RemoteUrl);
                lblUrl.Text = "http://" + uri.Host;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                lblUrl.Text = "http://karaoke.lacharme.net/";
            }
            

        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void lblUrl_Click(object sender, EventArgs e)
        {
            if (Uri.IsWellFormedUriString(lblUrl.Text, UriKind.Absolute))
            {

                ProcessStartInfo sInfo = new ProcessStartInfo(new Uri(lblUrl.Text).AbsoluteUri);
                Process.Start(sInfo);
            }
        }

        private void lblUrl_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor.Current = Cursors.Hand;
        }

    }
}
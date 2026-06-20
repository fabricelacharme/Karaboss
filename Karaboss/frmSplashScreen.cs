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
using FlShell.Interop;
using System;
using System.Threading;
using System.Windows.Forms;

namespace Karaboss
{
    public partial class frmSplashScreen : Form
    {
        private SynchronizationContext context;

        private ContextMenu SplashContextMenu;

        public frmSplashScreen()
        {
            InitializeComponent();

            context = SynchronizationContext.Current;
            
            label2.Text = "Version: " + Application.ProductVersion ;
            LblMsg.Text = "";
        }

        public void Msg(string message)
        {            
            context.Post(delegate (object dummy)
            {
                LblMsg.Text = message;
            }, null);

        }

        private void frmSplashScreen_Load(object sender, EventArgs e)
        {

        }

        private void frmSplashScreen_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                SplashContextMenu = new ContextMenu();
                SplashContextMenu.MenuItems.Clear();

                // Close
                MenuItem mnuClose = new MenuItem("Close");
                mnuClose.Click += new System.EventHandler(this.mnuClose_Click);
                SplashContextMenu.MenuItems.Add(mnuClose);
            }
        }

        private void mnuClose_Click(object sender, EventArgs e)
        {
            // Stop application
            //Application.Exit();
            Application.ExitThread();

        }
    }
}

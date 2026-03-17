using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Karaboss.Kfn
{
    public partial class frmFullScreen : Form
    {

        [DllImport("user32.dll")]
        public static extern IntPtr CreateIconIndirect(ref IconInfo icon);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);



        public struct IconInfo
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }
        
        private Bitmap bmpScreenshot;
        private Graphics gfxScreenshot;




        public frmFullScreen()
        {
            InitializeComponent();            

            ShowInTaskbar = false;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            KeyPreview = true;

            //Bitmap bitmap = new Bitmap(32, 32);
            // Get bitmap from resources
            Bitmap bitmap = Karaboss.Properties.Resources.color_picker_black18;

            //Graphics g = Graphics.FromImage(bitmap);
            this.Cursor = CreateCursor(bitmap, 3, 3);

            bitmap.Dispose();

            //Cursor = Cursors.Hand;
        }

        public static Cursor CreateCursor(Bitmap bmp, int xHotSpot, int yHotSpot)
        {
            IconInfo tmp = new IconInfo();
            GetIconInfo(bmp.GetHicon(), ref tmp);
            tmp.xHotspot = xHotSpot;
            tmp.yHotspot = yHotSpot;
            tmp.fIcon = false;
            return new Cursor(CreateIconIndirect(ref tmp));
        }


        private void frmFullScreen_MouseDown(object sender, MouseEventArgs e)
        {
           
           
        }

      
        private void frmFullScreen_Load(object sender, EventArgs e)
        {
            CopyScreen();
        }

        private void CopyScreen()
        {
            Size s;
            s = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

            // Hide the form so that it does not appear in the screenshot
            this.Hide();
            
            Rectangle rect = new Rectangle(0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Bitmap bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(rect.Left, rect.Top, 0, 0, s, CopyPixelOperation.SourceCopy);
            picScreen.Image = bmp;

            // Show the form
            this.Show();
        }

        /// <summary>
        /// Pick the color under the mouse pointer and send it to frmKfnCreate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picScreen_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var c = GetColorAt(Cursor.Position.X, Cursor.Position.Y);
                if (c != null)
                {
                    if (Application.OpenForms.OfType<frmKfnCreate>().Count() > 0)
                    {
                        frmKfnCreate frmKfnCreate = Utilities.FormUtilities.GetForm<frmKfnCreate>();
                        frmKfnCreate.GetColorFromPicker(c);
                    }
                }
                Close();
            }
            else
            {
                Close();
            }
        }

        private Color GetColorAt(int x, int y)
        {
            Bitmap bmp = new Bitmap(1, 1);
            Rectangle bounds = new Rectangle(x, y, 1, 1);
            using (Graphics g = Graphics.FromImage(bmp))
                g.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);
            return bmp.GetPixel(0, 0);
        }

        private void frmFullScreen_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)  
                Close();  
        }
    }
}

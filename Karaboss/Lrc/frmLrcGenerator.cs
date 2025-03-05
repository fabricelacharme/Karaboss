using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Karaboss.Mp3;

namespace Karaboss.Mp3
{
    public partial class frmLrcGenerator: Form
    {
        public frmLrcGenerator()
        {
            InitializeComponent();
        }

        #region Form load close resize
        private void frmLrcGenerator_Load(object sender, EventArgs e)
        {
            Location = Properties.Settings.Default.frmLrcGeneratorLocation;
            // Verify if this windows is visible in extended screens
            Rectangle rect = new Rectangle(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);
            foreach (Screen screen in Screen.AllScreens)
                rect = Rectangle.Union(rect, screen.Bounds);

            if (Location.X > rect.Width)
                Location = new Point(0, Location.Y);
            if (Location.Y > rect.Height)
                Location = new Point(Location.X, 0);
        }

        private void frmLrcGenerator_Resize(object sender, EventArgs e)
        {

        }

        private void frmLrcGenerator_FormClosing(object sender, FormClosingEventArgs e)
        {
            // enregistre la taille et la position de la forme
            // Copy window location to app settings                
            if (WindowState != FormWindowState.Minimized)
            {
                Properties.Settings.Default.frmLrcGeneratorLocation = Location;
                // Save settings
                Properties.Settings.Default.Save();
            }

            // Active le formulaire frmExplorer
            if (Application.OpenForms.OfType<frmMp3Player>().Count() > 0)
            {
                // Restore form
                Application.OpenForms["frmMp3Player"].Restore();
                Application.OpenForms["frmMp3Player"].Activate();
            }

            Dispose();
        }

        #endregion Form load close resize
    }
}

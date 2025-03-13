using System.Drawing;
using System.Windows.Forms;

namespace Karaboss.Mp3
{
    public partial class frmMp3LyricsSimple : Form
    {
        public frmMp3LyricsSimple()
        {
            InitializeComponent();
        }

        public void DisplayText(string tx)
        {
            this.txtLyrics.Text = tx;
        }

        private void frmMp3LyricsSimple_FormClosing(object sender, FormClosingEventArgs e)
        {
            // enregistre la taille et la position de la forme
            // Copy window location to app settings                
            if (WindowState != FormWindowState.Minimized)
            {
                if (WindowState == FormWindowState.Maximized)
                {
                    Properties.Settings.Default.frmMp3LyricsSimpleLocation = RestoreBounds.Location;
                    Properties.Settings.Default.frmMp3LyricsSimpleMaximized = true;

                }
                else if (WindowState == FormWindowState.Normal)
                {
                    Properties.Settings.Default.frmMp3LyricsSimpleLocation = Location;
                    Properties.Settings.Default.frmMp3LyricsSimpleSize = Size;
                    Properties.Settings.Default.frmMp3LyricsSimpleMaximized = false;
                }

                // Save settings
                Properties.Settings.Default.Save();
            }
        }

        private void frmMp3LyricsSimple_Load(object sender, System.EventArgs e)
        {
            // Récupère la taille et position de la forme
            // Set window location
            if (Properties.Settings.Default.frmMp3LyricsSimpleMaximized)
            {
                Location = Properties.Settings.Default.frmMp3LyricsSimpleLocation;
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                Location = Properties.Settings.Default.frmMp3LyricsSimpleLocation;
                // Verify if this windows is visible in extended screens
                Rectangle rect = new Rectangle(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);
                foreach (Screen screen in Screen.AllScreens)
                    rect = Rectangle.Union(rect, screen.Bounds);

                if (Location.X > rect.Width)
                    Location = new Point(0, Location.Y);
                if (Location.Y > rect.Height)
                    Location = new Point(Location.X, 0);

                Size = Properties.Settings.Default.frmMp3LyricsSimpleSize;
            }
        }
    }
}

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

    }
}

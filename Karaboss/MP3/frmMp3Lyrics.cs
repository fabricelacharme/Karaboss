using System.Windows.Forms;

namespace Karaboss.Mp3
{
    public partial class frmMp3Lyrics : Form
    {
        public frmMp3Lyrics()
        {
            InitializeComponent();
        }

        public void DisplayText(string tx)
        {
            this.txtLyrics.Text = tx;
        }

    }
}

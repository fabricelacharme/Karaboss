using System;
using System.Windows.Forms;


namespace Karaboss
{
    public partial class frmLrcOptions : Form
    {
       
        public Karaclass.LrcFormats LrcFormat
        {
            get { 
                if (OptFormatLines.Checked) 
                    return Karaclass.LrcFormats.Lines;
                else return Karaclass.LrcFormats.Syllables;
            }
        }

        public bool bRemoveAccents
        {
            get { return chkRemoveAccents.Checked; }
        }

        public bool bUpperCase
        {
            get { return chkUpperCase.Checked; }
        }

        public frmLrcOptions()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void OptFormatLines_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void OptFormatSyllabes_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void chkRemoveAccents_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkUpperCase_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}

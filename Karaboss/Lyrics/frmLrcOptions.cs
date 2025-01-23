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

        public bool bRemoveNonAlphaNumeric
        {
            get { return chkAlphaNumeric.Checked; }
        }

        public bool bRemoveAccents
        {
            get { return chkRemoveAccents.Checked; }
        }

        public bool bUpperCase
        {
            get { return chkUpperCase.Checked; }
        }

        public frmLrcOptions(bool bRemoveAccents, bool bForceUpperCase, bool bRemoveNonAlphaNumeric)
        {
            InitializeComponent();

            chkUpperCase.Checked = bForceUpperCase;
            chkAlphaNumeric.Checked = bRemoveNonAlphaNumeric;
            chkRemoveAccents.Checked = bRemoveAccents;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

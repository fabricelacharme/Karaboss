using System;
using System.Windows.Forms;
using static Karaboss.Karaclass;


namespace Karaboss
{
    public partial class frmLrcOptions : Form
    {

        #region properties
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
        
        #endregion properties


        //public frmLrcOptions(Karaclass.LrcFormats LrcFormat, bool bRemoveAccents, bool bForceUpperCase, bool bRemoveNonAlphaNumeric)
        public frmLrcOptions()
        {
            InitializeComponent();

            // Load and apply options
            LoadOptions();            
        }

        /// <summary>
        /// Load and apply options
        /// </summary>
        private void LoadOptions()
        {
            // Remove accents
            chkRemoveAccents.Checked = Properties.Settings.Default.bLrcRemoveAccents;
            // Force Upper Case
            chkUpperCase.Checked = Properties.Settings.Default.bLrcForceUpperCase;
            // Remove all non-alphanumeric characters
            chkAlphaNumeric.Checked = Properties.Settings.Default.bLrcRemoveNonAlphaNumeric;
            
            // Export to lines or syllabes
            Karaclass.LrcFormats LrcFormat = Properties.Settings.Default.lrcFormatLinesSyllabes == 0 ? LrcFormats.Lines : LrcFormats.Syllables;
            OptFormatLines.Checked = LrcFormat == Karaclass.LrcFormats.Lines;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void frmLrcOptions_FormClosing(object sender, FormClosingEventArgs e)
        {

            // Save options
            Properties.Settings.Default.bLrcRemoveAccents = bRemoveAccents;
            Properties.Settings.Default.bLrcForceUpperCase = bUpperCase;
            Properties.Settings.Default.bLrcRemoveNonAlphaNumeric = bRemoveNonAlphaNumeric;
            Properties.Settings.Default.lrcFormatLinesSyllabes = (OptFormatLines.Checked ? 0 : 1);   

            // Save settings
            Properties.Settings.Default.Save();
        }
    }
}

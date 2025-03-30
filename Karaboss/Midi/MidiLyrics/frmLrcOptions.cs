#region License

/* Copyright (c) 2025 Fabrice Lacharme
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

using Karaboss.Utilities;
using System;
using System.Windows.Forms;
using static Karaboss.Karaclass;


namespace Karaboss
{
    public partial class frmLrcOptions : Form
    {        

        #region properties
        public LrcLinesSyllabesFormats LrcLinesSyllabesFormat
        {
            get { 
                if (OptFormatLines.Checked) 
                    return LrcLinesSyllabesFormats.Lines;
                else return LrcLinesSyllabesFormats.Syllabes;
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
        
        public bool bLowerCase
        {
            get { return chkLowerCase.Checked; }
        }

        /// <summary>
        /// Number of characters max per lines
        /// </summary>
        public int LrcCutLinesChars
        {
            get { return (int)UpdCutLines.Value; }
        }

        /// <summary>
        /// Cut lines over UpdCutLines.Value characters
        /// </summary>
        public bool bCutLines
        {
            get { return chkCutLines.Checked; }
        }

        private int _LrcMillisecondsDigits = 2;
        public int LrcMillisecondsDigits
        {
            get { return _LrcMillisecondsDigits; }
        }


        public bool bSaveMetadata
        {
            get { return chkMetadata.Checked; }
        }

        #endregion properties


        /// <summary>
        /// Constructor
        /// </summary>
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
            try
            {
                // Remove accents
                chkRemoveAccents.Checked = Properties.Settings.Default.bLrcRemoveAccents;
                // Force Upper Case
                chkUpperCase.Checked = Properties.Settings.Default.bLrcForceUpperCase;
                // Force Lower Case 
                chkLowerCase.Checked = Properties.Settings.Default.bLrcForceLowerCase;

                // Remove all non-alphanumeric characters
                chkAlphaNumeric.Checked = Properties.Settings.Default.bLrcRemoveNonAlphaNumeric;

                // Export to lines or syllabes
                LrcLinesSyllabesFormats LrcLinesSyllabesFormat = Properties.Settings.Default.lrcFormatLinesSyllabes == 0 ? LrcLinesSyllabesFormats.Lines : LrcLinesSyllabesFormats.Syllabes;
                OptFormatLines.Checked = LrcLinesSyllabesFormat == LrcLinesSyllabesFormats.Lines;

                chkCutLines.Checked = Properties.Settings.Default.bLrcCutLines;
                UpdCutLines.Value = Properties.Settings.Default.LrcCutLinesChars;

                // Export Metadata
                chkMetadata.Checked = Properties.Settings.Default.bExportMetadata;


                _LrcMillisecondsDigits = Properties.Settings.Default.LrcMillisecondsDigits;
                OptFormat2Digits.Checked = _LrcMillisecondsDigits == 2;
                OptFormat3Digits.Checked = _LrcMillisecondsDigits == 3;

                // Default value for OptFormatSyllabes is Checked => no event at loading form
                // So manage this use case
                // The event OptFormatLines.Checked is managed by OptFormatLines_CheckedChanged
                ManageDisplayOptions();
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }


        #region Form Load Close
      
        private void frmLrcOptions_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // Save options
                Properties.Settings.Default.bLrcRemoveAccents = bRemoveAccents;
                
                Properties.Settings.Default.bLrcForceUpperCase = bUpperCase;
                Properties.Settings.Default.bLrcForceLowerCase = bLowerCase;

                Properties.Settings.Default.bLrcRemoveNonAlphaNumeric = bRemoveNonAlphaNumeric;
                Properties.Settings.Default.lrcFormatLinesSyllabes = (OptFormatLines.Checked ? 0 : 1);

                Properties.Settings.Default.bLrcCutLines = chkCutLines.Checked;
                Properties.Settings.Default.LrcCutLinesChars = (int)UpdCutLines.Value;

                Properties.Settings.Default.LrcMillisecondsDigits = _LrcMillisecondsDigits;

                // Save settings
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        #endregion Form Load close


        #region manage number of chars to cut

        /// <summary>
        /// Checkbox chkCutlines has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkCutLines_CheckedChanged(object sender, EventArgs e)
        {
            ManageDisplayOptions();            
        }

        #endregion manage number of chars to cut


        #region manage Options syllabes/lines
        
        /// <summary>
        /// OptFormatSyllabes has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OptFormatSyllabes_CheckedChanged(object sender, EventArgs e)
        {
            ManageDisplayOptions();            
        }

        /// <summary>
        /// OptFormatLines has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OptFormatLines_CheckedChanged(object sender, EventArgs e)
        {
            ManageDisplayOptions();            
        }


        private void ManageDisplayOptions()
        {
            if (OptFormatSyllabes.Checked)
            {
                // Cut lines options not visible if syllabes
                chkCutLines.Visible = false;
                UpdCutLines.Visible = false;
                lblCutLines.Visible = false;
            }
            else if (OptFormatLines.Checked)
            {
                // Cut lines options visibility depends on checkbox chkCutLines
                chkCutLines.Visible = true;

                if (chkCutLines.Checked)
                {
                    UpdCutLines.Visible = true;
                    lblCutLines.Visible = true;
                }
                else
                {
                    UpdCutLines.Visible = false;
                    lblCutLines.Visible = false;
                }
            }
        }

        #endregion manage options syllabes/lines

        private void chkLowerCase_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLowerCase.Checked)
                chkUpperCase.Checked = false;
        }

        private void chkUpperCase_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUpperCase.Checked)
                chkLowerCase.Checked = false;
        }
       

        private void OptFormat2Digits_CheckedChanged(object sender, EventArgs e)
        {
            if (OptFormat2Digits.Checked)
                _LrcMillisecondsDigits = 2;
            else
                _LrcMillisecondsDigits = 3;
        }

        private void OptFormat3Digits_CheckedChanged(object sender, EventArgs e)
        {
            if (OptFormat3Digits.Checked)
                _LrcMillisecondsDigits = 3;
            else
                _LrcMillisecondsDigits = 2;
        }

        private void chkMetadata_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}

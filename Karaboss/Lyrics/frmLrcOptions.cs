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
                // Remove all non-alphanumeric characters
                chkAlphaNumeric.Checked = Properties.Settings.Default.bLrcRemoveNonAlphaNumeric;

                // Export to lines or syllabes
                Karaclass.LrcFormats LrcFormat = Properties.Settings.Default.lrcFormatLinesSyllabes == 0 ? LrcFormats.Lines : LrcFormats.Syllables;
                OptFormatLines.Checked = LrcFormat == Karaclass.LrcFormats.Lines;

                chkCutLines.Checked = Properties.Settings.Default.bLrcCutLines;
                UpdCutLines.Value = Properties.Settings.Default.LrcCutLinesChars;

                if (OptFormatSyllabes.Checked)
                {
                    chkCutLines.Visible = false;
                    UpdCutLines.Visible = false;
                    lblCutLines.Visible = false;
                }
                if (!chkCutLines.Checked) 
                { 
                    UpdCutLines.Visible = false;
                    lblCutLines.Visible = false;
                }

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

        private void frmLrcOptions_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // Save options
                Properties.Settings.Default.bLrcRemoveAccents = bRemoveAccents;
                Properties.Settings.Default.bLrcForceUpperCase = bUpperCase;
                Properties.Settings.Default.bLrcRemoveNonAlphaNumeric = bRemoveNonAlphaNumeric;
                Properties.Settings.Default.lrcFormatLinesSyllabes = (OptFormatLines.Checked ? 0 : 1);

                Properties.Settings.Default.bLrcCutLines = chkCutLines.Checked;
                Properties.Settings.Default.LrcCutLinesChars = (int)UpdCutLines.Value;

                // Save settings
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void chkCutLines_CheckedChanged(object sender, EventArgs e)
        {
            UpdCutLines.Visible = chkCutLines.Checked;
            lblCutLines.Visible = chkCutLines.Checked;
        }

        private void OptFormatSyllabes_CheckedChanged(object sender, EventArgs e)
        {
            chkCutLines.Visible = !OptFormatSyllabes.Checked;
            UpdCutLines.Visible = !OptFormatSyllabes.Checked;
            lblCutLines.Visible = !OptFormatSyllabes.Checked;
        }

        private void OptFormatLines_CheckedChanged(object sender, EventArgs e)
        {
            chkCutLines.Visible = OptFormatLines.Checked;           
        }

    }
}

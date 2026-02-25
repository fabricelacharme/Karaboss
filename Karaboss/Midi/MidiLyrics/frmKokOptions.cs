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
using System.Text;
using System.Windows.Forms;
using static Karaboss.Karaclass;


namespace Karaboss
{
    public partial class frmKokOptions : Form
    {        

        #region properties
                              
        private string _defaultencoding = "UTF8";
        public string DefaultEncoding { get { return _defaultencoding; } }

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

        #endregion properties


        /// <summary>
        /// Constructor
        /// </summary>
        public frmKokOptions()
        {
            InitializeComponent();

            // Initialize list of encoding
            initCbEncoding();

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


                // Encoding
                _defaultencoding = "UTF8";
                if (Properties.Settings.Default.DefaultEncoding != null)
                    _defaultencoding = Properties.Settings.Default.DefaultEncoding;
                
                switch (_defaultencoding)
                {
                    case "ANSI":
                        cbEncoding.SelectedIndex = 0;
                        break;
                    case "UTF8":
                        cbEncoding.SelectedIndex = 1;                         
                        break;                   
                    default:
                        cbEncoding.SelectedIndex = 1;
                        break;
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


        #region encoding

        private void initCbEncoding()
        {
            //UTF8: Encoding encoding = Encoding.UTF8;
            //ANSI: encoding = System.Text.Encoding.GetEncoding("iso-8859-1");
            cbEncoding.Items.Add("ANSI");
            cbEncoding.Items.Add("UTF8");            
            //cbEncoding.SelectedIndex = 1;
        }

        #endregion encoding



        #region Form Load Close

        private void frmKokOptions_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // Save options
                Properties.Settings.Default.bLrcRemoveAccents = bRemoveAccents;                
                Properties.Settings.Default.bLrcForceUpperCase = bUpperCase;
                Properties.Settings.Default.bLrcForceLowerCase = bLowerCase;
                Properties.Settings.Default.bLrcRemoveNonAlphaNumeric = bRemoveNonAlphaNumeric;               

                switch (_defaultencoding)
                {                    
                    case "ANSI":
                        Properties.Settings.Default.DefaultEncoding = "ANSI";
                        break;
                    case "UTF8":
                        Properties.Settings.Default.DefaultEncoding = "UTF8";
                        break;
                    default:
                        Properties.Settings.Default.DefaultEncoding = "UTF8";
                        break;
                }

                // Save settings
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        #endregion Form Load close
     

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
       

        private void chkMetadata_CheckedChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Handles the event when the selected encoding option in the combo box changes.
        /// </summary>
        /// <remarks>Updates the internal encoding setting based on the user's selection. Selecting the
        /// first option sets the encoding to UTF-8, while the second option sets it to ISO-8859-1.</remarks>
        /// <param name="sender">The source of the event, typically the encoding combo box control.</param>
        /// <param name="e">The event data associated with the selection change.</param>
        private void cbEncoding_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbEncoding.SelectedIndex)
            {
                case 0:
                    _defaultencoding = "ANSI";
                    break;
                case 1:
                    _defaultencoding = "UTF8";
                    break;                
            }

        }
    }
}

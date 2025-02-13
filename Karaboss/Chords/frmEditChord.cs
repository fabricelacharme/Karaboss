using ChordAnalyser.UI;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Karaboss
{
    public partial class frmEditChord : Form
    {
        #region declarations
        private frmChords frmChords;
        private readonly int Beat;
        Point P;
        string currentvalue;

        List<string> lstRootNotes2 = new List<string> { "#", "b" };
        List<string> lstRootNotes1 = new List<string> { "a", "b", "c", "d", "e", "f", "g", "A", "B", "C", "D", "E", "F", "G" };
        
        List<string> lstNotes = new List<string> { "C", "C#", "D", "D#", "Db", "E", "Eb", "F", "F#", "G", "G#", "Gb", "A", "A#", "Ab", "B", "Bb" };
        List<string> lstTypes = new List<string> { "maj", "m", "7", "maj7", "m7", "sus4", "5", "dim", "sus2", "7sus4", "9", "7#9", "m9", "maj9", "6", "m6", "m7b5", "aug" };
        List<string> lstBass = new List<string> { "A", "Ab", "B", "Bb", "C", "C#", "D", "Db", "E", "Eb", "F", "F#", "G"  };

        #endregion declarations

        public frmEditChord(string Note, int beat, int X, int Y)
        {
            InitializeComponent();

            this.TopMost = true;
                        
            P = new Point(X, Y);
            Beat = beat;

            //  form will receive key events before the event is passed to the control that has focus.
            this.KeyPreview = true;

            InitCombos();
            SetCombosValues(Note);
        }

        #region Combos
        private void InitCombos()
        {
            for (int i = 0; i < lstTypes.Count; i++)
            {
                cbType.Items.Add(lstTypes[i]);
            }
            for (int i = 0; i < lstBass.Count; i++)
            {
                cbBass.Items.Add(lstBass[i]);
            }
        }
         
        private void SetCombosValues(string note)
        {
            string rest = string.Empty;
            string type;
            string bass;
            
            if (note.Length == 0)
                return;

            if (lstNotes.Contains(note))
            {

            }            
            // Notes is not contained only in the first Combo
            else
            {
                // First letter wrong
                if (!lstNotes.Contains(note.Substring(0, 1)))
                    return;

                // now we know that first letter is ok

                // Try with 2 firsts letters
                if (note.Length >= 2)
                {
                    if (!lstNotes.Contains(note.Substring(0, 2)))
                    {
                        // if 2 firsts letters wrong => take first one
                        rest = note.Substring(1, note.Length - 1);
                        note = note.Substring(0, 1);              // first letter is ok as seen previouly                        
                    }
                    else
                    {
                        // Take 2 first letters
                        rest = note.Substring(2, note.Length - 2);
                        note = note.Substring(0, 2);                        
                    }
                   

                    if (lstTypes.Contains(rest))
                    {
                        cbType.Text = rest;
                    }
                    else if (lstBass.Contains(rest))
                    {
                        cbBass.Text = rest;
                    }
                    else if (rest.IndexOf("/") != -1)
                    {
                        type = rest.Substring(0, rest.IndexOf("/"));
                        bass = rest.Substring(rest.IndexOf("/") + 1);

                        if (lstTypes.Contains(type))
                            cbType.Text = type;
                        if (lstBass.Contains(bass))
                            cbBass.Text = bass;
                    }                    
                }                
            }

            #region note
            // ============================
            // cbNote
            // ============================
            switch (note)
            {
                case "C#":
                case "C":
                    cbNote.BeginUpdate();
                    cbNote.Items.Clear();
                    cbNote.Items.Add("C");
                    cbNote.Items.Add("C#");
                    cbNote.Text = note;
                    cbNote.EndUpdate();
                    break;
                case "Db":
                case "D#":
                case "D":
                    cbNote.BeginUpdate();
                    cbNote.Items.Clear();
                    cbNote.Items.Add("D");
                    cbNote.Items.Add("D#");
                    cbNote.Items.Add("Db");
                    cbNote.Text = note;
                    cbNote.EndUpdate();
                    break;
                case "Eb":
                case "E":
                    cbNote.BeginUpdate();
                    cbNote.Items.Clear();
                    cbNote.Items.Add("E");
                    cbNote.Items.Add("Eb");
                    cbNote.Text = note;
                    cbNote.EndUpdate();
                    break;
                case "F#":
                case "F":
                    cbNote.BeginUpdate();
                    cbNote.Items.Clear();
                    cbNote.Items.Add("F");
                    cbNote.Items.Add("F#");
                    cbNote.Text = note;
                    cbNote.EndUpdate();
                    break;
                case "G#":
                case "Gb":
                case "G":
                    cbNote.BeginUpdate();
                    cbNote.Items.Clear();
                    cbNote.Items.Add("G");
                    cbNote.Items.Add("G#");
                    cbNote.Items.Add("Gb");
                    cbNote.Text = note;
                    cbNote.EndUpdate();
                    break;
                case "A#":
                case "Ab":
                case "A":
                    cbNote.BeginUpdate();
                    cbNote.Items.Clear();
                    cbNote.Items.Add("A");
                    cbNote.Items.Add("A#");
                    cbNote.Items.Add("Ab");
                    cbNote.Text = note;
                    cbNote.EndUpdate();
                    break;
                case "Bb":
                case "B":
                    cbNote.BeginUpdate();
                    cbNote.Items.Clear();
                    cbNote.Items.Add("B");
                    cbNote.Items.Add("Bb");
                    cbNote.Text = note;
                    cbNote.EndUpdate();
                    break;
            }
            currentvalue = note;
            cbNote.SelectionStart = note.Length;

            #endregion note
        }

        #endregion Combos


        #region form load unload

        private void frmEditChord_Load(object sender, EventArgs e)
        {
            // Locate form 
            Location = Location = new Point(P.X, P.Y);

        }
                   

        private void frmEditChord_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        #endregion form load unload


        #region Button

        /// <summary>
        /// Button: valid note change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOk_Click(object sender, EventArgs e)
        {
            SaveChord();
        }

        private void SaveChord()
        {
            string ChordName = cbNote.Text;
            ChordName += cbType.Text;
            ChordName += cbBass.Text != "" ? "/" + cbBass.Text : "";

            #region Check ChordName
            if (cbNote.Text == "" && (cbType.Text != "" || cbBass.Text != ""))
            {
                MessageBox.Show("Invalid chord", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            #endregion Check ChordName



            if (Application.OpenForms.OfType<frmChords>().Count() > 0)
            {
                frmChords = Utilities.FormUtilities.GetForm<frmChords>();
                frmChords.UpdateChord(Beat, ChordName);
            }

            this.Close();

        }

        #endregion Button


        #region cbNote

        private void cbNote_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Prevent form closing !!!!!!!!!!!!!!!!
            // manage Closing variable ?

            //e.Cancel = !lstNotes.Contains(cbNote.Text);
        }

        private void cbNote_TextUpdate(object sender, EventArgs e)
        {
            if (currentvalue != cbNote.Text)
            {
                currentvalue = cbNote.Text;
                SetCombosValues(currentvalue);
            }
        }

        private void cbNote_KeyPress(object sender, KeyPressEventArgs e)
        {
            
            if (cbNote.Text.Length == 0)
            {
                // 1st char: Restric 1st combo to "A", "B", "C", "D", "E", "F", "G", "#", "b"                
                if (!lstRootNotes1.Contains(e.KeyChar.ToString()))
                    e.Handled = true;
                else
                {
                    e.KeyChar = Char.ToUpper(e.KeyChar);                    
                }
            }
            else if (cbNote.Text.Length == 1)
            {
                // If selection => ecrase
                if (cbNote.SelectionLength == 1) 
                {
                    // 1st char: Restric 1st combo to "A", "B", "C", "D", "E", "F", "G", "#", "b"                
                    if (!lstRootNotes1.Contains(e.KeyChar.ToString()))
                        e.Handled = true;
                    else
                    {
                        e.KeyChar = Char.ToUpper(e.KeyChar);
                    }
                    return;
                }

                if (!lstRootNotes2.Contains(e.KeyChar.ToString()))
                {
                    // 2nd char: restrict to "#", "b"
                    e.Handled = true;
                }
                else if (!lstNotes.Contains(cbNote.Text + e.KeyChar.ToString()))
                {
                    // Restric to "C", "C#", "D", "D#", "Db", "E", "Eb", "F", "F#", "G", "G#", "Gb", "A", "A#", "Ab", "B", "Bb"
                    e.Handled= true;
                }
                else
                {
                    cbNote.Text = cbNote.Text + e.KeyChar.ToString();
                    cbNote.SelectionStart = cbNote.Text.Length;
                    e.Handled = true;
                }
            }
            else if (cbNote.Text.Length == 2)
            {
                if (cbNote.SelectionLength != 2)
                    e.Handled = true;
                else
                    e.KeyChar = Char.ToUpper(e.KeyChar);
            }
            else
            {
                e.Handled = true;
            }
        }

        private void cbNote_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {              
                case Keys.Enter:
                    SaveChord();
                    break;
                
                case Keys.Back:
                    if (cbNote.Text.Length > 1)
                    {
                        cbNote.Text = cbNote.Text.Substring(0, 1);
                        cbNote.SelectionStart = 1;
                    }
                    else
                        cbNote.Text = "";
                    break;
                default:                    
                    break;
            }
        }

        #endregion cbNote

        private void cbType_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    SaveChord();
                    break;
               
                default:
                    break;
            }
        }

        private void cbBass_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    SaveChord();
                    break;

                default:
                    break;
            }
        }
    }
}

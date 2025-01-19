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
        frmChords fmrmChords;
        ChordsMapControl chordsMapControl;
        Point P;
        string currentvalue;

        List<string> lstNotes = new List<string> { "C", "C#", "D", "D#", "Db", "E", "Eb", "F", "F#", "G", "G#", "Gb", "A", "A#", "Ab", "B", "Bb" };
        List<string> lstTypes = new List<string> { "maj", "m", "7", "maj7", "m7", "sus4", "5", "dim", "sus2", "7sus4", "9", "7#9", "m9", "maj9", "6", "m6", "m7b5", "aug" };
        List<string> lstBass = new List<string> { "A", "Ab", "B", "Bb", "C", "C#", "D", "E", "Eb", "F", "F#", "G"  };

        public frmEditChord(string Note, ChordsMapControl cm, int X, int Y)
        {
            InitializeComponent();

            this.TopMost = true;

            
            chordsMapControl = cm;
            P = new Point(X, Y);
            
            
            
            //  form will receive key events before the event is passed to the control that has focus.
            this.KeyPreview = true;

            InitCombos();


            SetNotesValue(Note);
        }


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

        private void cbNote_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
           e.Cancel = !lstNotes.Contains(cbNote.Text);
                

        }
        private void cbNote_TextUpdate(object sender, EventArgs e)
        {
            if (currentvalue != cbNote.Text)
            {
                currentvalue = cbNote.Text;
                SetNotesValue(currentvalue);
            }
        }
        private void cbNote_TextChanged(object sender, EventArgs e)
        {
            
           // SetNotesValue(cbNote.Text);
            
        }

        private void SetNotesValue(string note)
        {
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
                    cbNote.Text= note;
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
        }

        private void frmEditChord_Load(object sender, EventArgs e)
        {
            // Locate form the the right of MidiSheet
            StartupLocation();
        }

        private void StartupLocation()
        {

            Form fParent = (Form)findFormParent(chordsMapControl);
            if (fParent != null)
            {                
            int LocX = P.X + fParent.Left;
            int LocY = P.Y + fParent.Top + 270;

            Location = Location = new Point(LocX, LocY); ;
                
            }
        }

        private void frmEditChord_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        /// <summary>
        /// Find recursively parent Form
        /// </summary>
        /// <param name="theControl"></param>
        /// <returns></returns>
        private Control findFormParent(Control theControl)
        {
            Control rControl = null;

            if (theControl.Parent != null)
            {
                if (theControl.Parent is Form)
                {
                    rControl = theControl.Parent;
                }
                else
                {
                    rControl = findFormParent(theControl.Parent);
                }
            }
            else
            {
                rControl = null;
            }
            return rControl;
        }


        #region Locate form
        /// <summary>
        /// Locate form
        /// </summary>
        /// <typeparam name="TForm"></typeparam>
        /// <returns></returns>
        private TForm GetForm<TForm>()
            where TForm : Form
        {
            return (TForm)Application.OpenForms.OfType<TForm>().FirstOrDefault();
        }



        #endregion Locate form

       
    }
}

#region License

/* Copyright (c) 2016 Fabrice Lacharme
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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Sanford.Multimedia.Midi.Score.UI
{
    public partial class frmNoteEdit : Form    
    {        
        SheetMusic sheetmusic;
        private int _tracknum;
        private List<MidiNote> _lstmidinotes;
        bool busy = false;


        public frmNoteEdit(SheetMusic SM)
        {
            InitializeComponent();
            sheetmusic = SM;

            sheetmusic.CurrentNoteChanged += new SheetMusic.CurrentNoteChangedEventHandler(sheetmusic_CurrentNoteChanged);
            sheetmusic.SelectionChanged += new SheetMusic.SelectionChangedEventHandler(sheetmusic_SelectionChanged);
            sheetmusic.CurrentTrackChanged += new SheetMusic.CurrentTrackChangedEventHandler(sheetmusic_TrackChanged);
        }

        private void sheetmusic_TrackChanged(int tracknum)
        {
            _tracknum = tracknum;            
        }

        private void sheetmusic_CurrentNoteChanged(MidiNote n)
        {
            if (busy)
                return;

            // Convert note number to letter
            string[] Scale = { "A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#" };
            string NoteName = Scale[(n.Number + 3) % 12];

            upDownNoteValue.Value = n.Number;
            lblNoteString.Text = NoteName;
            lblTrackNumber.Text = String.Format("Track #{0}", _tracknum);
            txtTime.Text = sheetmusic.GetTimeInMeasure(n.StartTime).ToString();
            txtTicks.Text = n.StartTime.ToString();
            txtDuration.Text = n.Duration.ToString();
            upDownNoteVelocity.Value = n.Velocity;
        }

        private void sheetmusic_SelectionChanged(List<MidiNote> lstMidiNotes)
        {
            if (busy)
                return;

            _lstmidinotes = null;
            lblSelection.Text = "0";

            if (lstMidiNotes == null)
                _lstmidinotes = null;
            else if (lstMidiNotes != null && lstMidiNotes.Count == 0)
                _lstmidinotes = null;
            else
            {
                
                _lstmidinotes = new List<MidiNote>();
                foreach (MidiNote n in lstMidiNotes)
                {
                    _lstmidinotes.Add(n);
                }
            }


            if (_lstmidinotes != null)
            {
               // _lstmidinotes = lstMidiNotes;
                lblSelection.Text = String.Format("Selection: {0}", lstMidiNotes.Count);

                // Invalidate all
                upDownNoteValue.Enabled = false;
                //txtTime.Enabled = false;
                txtTicks.Enabled = false;
                txtDuration.Enabled = false;

            }
            else
            {
                upDownNoteValue.Enabled = true;
                //txtTime.Enabled = true;
                txtTicks.Enabled = true;
                txtDuration.Enabled = true;
            }
        }


        #region startup location
        /// <summary>
        /// Locate form the the right of MidiSheet
        /// </summary>
        private void StartupLocation()
        {
            try
            {
                Form fParent = (Form)findFormParent(sheetmusic);                
                Point parentPoint = fParent.Location;

                //int LocX = parentPoint.X + fParent.ClientSize.Width - this.Width - System.Windows.Forms.SystemInformation.VerticalScrollBarWidth;
                int LocX = parentPoint.X + fParent.ClientSize.Width - this.Width - 10;
                int LocY = parentPoint.Y + 104;

                Location = new Point(LocX, LocY);

                // Verify if this windows is visible in extended screens
                Rectangle rect = new Rectangle(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);
                foreach (Screen screen in Screen.AllScreens)
                    rect = Rectangle.Union(rect, screen.Bounds);

                if (Location.X > rect.Width)
                    Location = new Point(0, Location.Y);
                if (Location.Y > rect.Height)
                    Location = new Point(Location.X, 0);

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

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

        #endregion


      


        private void upDownNoteValue_ValueChanged(object sender, EventArgs e)
        {
            /*
            if (upDownNoteValue.Value > sheetmusic.CurrentNote.midinote.Number)
            {
                sheetmusic.SheetMusic_UpDownCurrentNote("Up");
                sheetmusic.Refresh();
            }
            else if (upDownNoteValue.Value < sheetmusic.CurrentNote.midinote.Number)
            {
                sheetmusic.SheetMusic_UpDownCurrentNote("Down");
                sheetmusic.Refresh();
            }
            */
        }

        /// <summary>
        /// Modify current Note
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPnlNoteOk_Click(object sender, EventArgs e)
        {
            if (busy)
                return;

            if (_lstmidinotes == null)
            {
                MidiNote n = new MidiNote(Convert.ToInt32(txtTicks.Text), sheetmusic.CurrentNote.midinote.Channel, Convert.ToInt32(upDownNoteValue.Value), Convert.ToInt32(txtDuration.Text), Convert.ToInt32(upDownNoteVelocity.Value), true);
                sheetmusic.ModifyCurrentNote(n, false);
                sheetmusic.Refresh();
            }
            else
            {
                // Update only velocity
                busy = true;

                sheetmusic.ModifyVelocitySelectedNotes(Convert.ToInt32(upDownNoteVelocity.Value));

                sheetmusic.Refresh();
                busy = false;
            }
        }

        /// <summary>
        /// Make velocity default 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDefVlocity_Click(object sender, EventArgs e)
        {
            sheetmusic.DefaultVelocity = Convert.ToInt32(upDownNoteVelocity.Value);            
        }

        private void btnPnlNoteCancel_Click(object sender, EventArgs e)
        {
            Close();
        }


        #region form load close

        private void frmNoteEdit_Load(object sender, EventArgs e)
        {           
            // Locate form the the right of MidiSheet
            StartupLocation();
        }

        private void frmNoteEdit_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        #endregion

       
    }
}

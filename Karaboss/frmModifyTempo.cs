using Sanford.Multimedia.Midi.Score;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Karaboss
{
    public partial class frmModifyTempo : Form
    {

        private enum TempoChangesModes
        {
            CreateTempo,
            UpdateTempo,
            DeleteTempo
        }
        private TempoChangesModes ChangeMode;


        private bool TempoDoChange = true;
        private float _bpm;
        private float _tempo;
        private float _starttime;
        private decimal _division;

        frmPlayer FrmPlayer;

        private SheetMusic sheetmusic;
        TempoSymbol _tempoSymbol;
        Sequence sequence1;

        public int Division
        {
            get
            { return Convert.ToInt32(updDivision.Value); }
            set
            {
                _division = value;
                updDivision.Value = _division;
            }
        }
        public int Tempo
        {
            get
            { return Convert.ToInt32(_tempo); }
        }

        public int StartTime
        {
            get
            {
                return Convert.ToInt32(_starttime);
            }
        }

        public frmModifyTempo(SheetMusic sheetMusic, Sequence seq)
        {
            InitializeComponent();

            sheetmusic = sheetMusic;
            sequence1 = seq;
            int deftempo = sequence1.Tempo;


            // Check if a TempoSymbol is selected
            TempoSymbol tempoSymbol = sheetMusic.GetSelectedTempoSymbol();
            if (tempoSymbol != null)
            {
                _tempoSymbol = tempoSymbol;
                _starttime = _tempoSymbol.StartTime;
                _tempo = _tempoSymbol.Tempo;
            }
            else if (sheetmusic.CurrentNote != null)
            {
                // if a note is selected
                MidiNote n = sheetmusic.CurrentNote.midinote;
                _starttime = n.StartTime;
                _tempo = deftempo;
            }
            else
            {
                _starttime = 0;
                _tempo = deftempo;
            }

            Division = sequence1.Division;
            updDivision.Value = Convert.ToDecimal(Division);
            txtTempo.Text = _tempo.ToString();
            txtStartTime.Text = _starttime.ToString();
        }


        #region OK CANCEL
        
        /// <summary>
        /// Create or update a tempo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            switch (ChangeMode)
            {
                case TempoChangesModes.CreateTempo:
                    CreateTempo();
                    break;

                case TempoChangesModes.UpdateTempo:
                    UpdateTempo();
                    break;

            }
        }


        /// <summary>
        /// Delere current tempo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteTempo();

        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion OK CANCEL

       
        #region events

        private void txtTempo_TextChanged(object sender, EventArgs e)
        {
            if (TempoDoChange == false) return;

            if (IsNumeric(this.txtTempo.Text))
            {
                if (Convert.ToInt32(this.txtTempo.Text) > 0)
                {
                    const float kOneMinuteInMicroseconds = 60000000;
                    _tempo = float.Parse(this.txtTempo.Text);
                    _bpm = Convert.ToInt32(kOneMinuteInMicroseconds / _tempo);

                    TempoDoChange = false;
                    txtBpm.Text = _bpm.ToString();
                    TempoDoChange = true;

                }
            }
        }

        private void txtBpm_TextChanged(object sender, EventArgs e)
        {
            if (TempoDoChange == false) return;

            if (IsNumeric(this.txtBpm.Text))
            {
                if (Convert.ToInt32(this.txtBpm.Text) > 0)
                {
                    const float kOneMinuteInMicroseconds = 60000000;
                    float _bpm = float.Parse(this.txtBpm.Text);
                    _tempo = Convert.ToInt32(kOneMinuteInMicroseconds / _bpm);

                    TempoDoChange = false;
                    txtTempo.Text = _tempo.ToString();
                    TempoDoChange = true;
                }
            }
        }

        private void updDivision_ValueChanged(object sender, EventArgs e)
        {
            decimal division = 0;
            decimal val = updDivision.Value;
            division = val;
            if (val % PpqnClock.PpqnMinValue != 0)
            {
                val = (int)(Math.Round((double)val / PpqnClock.PpqnMinValue) * PpqnClock.PpqnMinValue);
                updDivision.Value = val;

                string msg = "Division must be a multiple of 24 \r\n";
                msg += string.Format("Division will be changed from {0} to {1}", division, val);
                MessageBox.Show(msg, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtStartTime_TextChanged(object sender, EventArgs e)
        {
            UpdateFields();

        }


        #endregion events


        #region verif
        private void txtTempo_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void txtDivision_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void txtBpm_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void txtStartTime_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }


        #endregion verif

                  
        #region prev next

        private void btnPrevTempo_Click(object sender, EventArgs e)
        {
            DisplayPreviousTempoChange();
        }

        private void DisplayPreviousTempoChange()
        {
            List<TempoSymbol> l = sheetmusic.GetAllTempoChanges();

            float ticks = -1 + float.Parse(txtStartTime.Text);
            for (int i = l.Count - 1; i >= 0; i--)
            {
                TempoSymbol tempo = l[i];
                if (tempo.StartTime < ticks)
                {
                    txtStartTime.Text = tempo.StartTime.ToString();
                    txtTempo.Text = tempo.Tempo.ToString();

                    if (Application.OpenForms.OfType<frmPlayer>().Count() > 0)
                    {
                        frmPlayer frmPlayer = getForm<frmPlayer>();
                        frmPlayer.ScrollTo(tempo.StartTime);
                    }
                    break;
                }
            }
        }

        private void btnNextTempo_Click(object sender, EventArgs e)
        {
            DisplayNextTempoChange();
        }

        private void DisplayNextTempoChange()
        {
            List<TempoSymbol> l = sheetmusic.GetAllTempoChanges();

            float ticks = 1 + float.Parse(txtStartTime.Text);

            for (int i = 0; i < l.Count; i++)
            {
                TempoSymbol tempo = l[i];
                if (tempo.StartTime >= ticks)
                {
                    txtStartTime.Text = tempo.StartTime.ToString();
                    txtTempo.Text = tempo.Tempo.ToString();

                    if (Application.OpenForms.OfType<frmPlayer>().Count() > 0)
                    {
                        frmPlayer frmPlayer = getForm<frmPlayer>();     
                        frmPlayer.ScrollTo(tempo.StartTime);
                    }                    
                    break;
                }
            }
        }



        #endregion prev next


        #region functions

        /// <summary>
        /// Update existing tempo
        /// </summary>
        private void UpdateTempo ()
        {
            CreateTempo();
        }
        
        /// <summary>
        /// Create a new tempo
        /// </summary>
        private void CreateTempo()
        {
            List<TempoSymbol> l = sheetmusic.GetAllTempoChanges();

            int ticks = Convert.ToInt32(txtStartTime.Text);
            int tempo = Convert.ToInt32(txtTempo.Text);
            TempoSymbol tmps = new TempoSymbol(ticks, tempo);

            if (IsContains(tmps, l))
            {
                string tx = "This tempo already exists at this location";
                MessageBox.Show(tx, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);    
                return;
            }

            // Remove all tempo events at location ticks
            sheetmusic.DeleteTempoChange(ticks);
            /*
            foreach (Track trk in sequence1.tracks)
            {
                trk.RemoveTempoEvent(ticks);
            }
            */
            sheetmusic.CreateTempoChange(ticks, tempo);
            //sequence1.tracks[0].insertTempo(tempo, ticks);

            Redraw();

            DisplayPreviousTempoChange();
            DisplayNextTempoChange();

        }

        /// <summary>
        /// Delete an existing tempo
        /// </summary>
        private void DeleteTempo()
        {
            List<TempoSymbol> l = sheetmusic.GetAllTempoChanges();
            TempoSymbol tmps = new TempoSymbol((int)_starttime, (int)_tempo);

            string msg;

            // The method l.Contains(tmps) does not work ??????                       
            if (!IsContains(tmps, l))
            {
                msg = "There is no tempo change at this location";
                MessageBox.Show(msg, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            sheetmusic.DeleteTempoChange((int)_starttime);

            Redraw();

            DisplayPreviousTempoChange();

        }


        private void Redraw()
        {
            if (Application.OpenForms.OfType<frmPlayer>().Count() > 0)
            {
                frmPlayer frmPlayer = getForm<frmPlayer>();
                frmPlayer.Redraw();
            }

        }


        /// <summary>
        /// Update fields according to the tempo displayed
        /// </summary>
        private void UpdateFields()
        {
            int index;
            List<TempoSymbol> l = sheetmusic.GetAllTempoChanges();

            try
            {
                if (txtStartTime.Text == "")
                    txtStartTime.Text = "0";

                _starttime = float.Parse(this.txtStartTime.Text);

                if (_starttime == 0)
                {
                    index = 1;
                    lblTempoNumber.Text = string.Format("Tempo {0} of {1}", index, l.Count);
                    btnDelete.Enabled = false;
                    //txtStartTime.Enabled = false;
                    ChangeMode = TempoChangesModes.UpdateTempo;
                    btnUpdate.Text = "Update";
                    return;
                }

                index = IsTempoExists((int)_starttime);
                if (index != -1)
                {
                    btnDelete.Enabled = true;
                    lblTempoNumber.Text = string.Format("Tempo {0} of {1}", index + 1, l.Count);
                    txtStartTime.Enabled = true;
                    ChangeMode= TempoChangesModes.UpdateTempo;
                    btnUpdate.Text = "Update";
                }
                else
                {
                    btnDelete.Enabled = false;
                    lblTempoNumber.Text = "New tempo";
                    txtStartTime.Enabled = true;
                    ChangeMode = TempoChangesModes.CreateTempo;
                    btnUpdate.Text = "Create";
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /// <summary>
        /// Replacement function for List.Contains
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="l"></param>
        /// <returns></returns>
        private bool IsContains(TempoSymbol symbol, List<TempoSymbol> l)
        {
            foreach (TempoSymbol t in l)
            {
                if (symbol.StartTime == t.StartTime && symbol.Tempo == t.Tempo)
                    return true;
            }
            return false;
        }
        

        /// <summary>
        /// Check existence of a tempo
        /// </summary>
        /// <param name="ticks"></param>
        /// <returns></returns>
        private int IsTempoExists(int ticks)
        {
            List<TempoSymbol> l = sheetmusic.GetAllTempoChanges();

            for (int i = 0; i < l.Count; i++)
            {
                TempoSymbol temposymbol = l[i];
                if (temposymbol.StartTime == ticks)
                {
                    return i;
                }
            }

            return -1;

        }


        /// <summary>
        /// Test if data is numeric
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private bool IsNumeric(string input)
        {
            int test;
            return int.TryParse(input, out test);
        }


        /// <summary>
        /// Locate form
        /// </summary>
        /// <typeparam name="TForm"></typeparam>
        /// <returns></returns>
        private T getForm<T>()
        where T : Form
        {
            return (T)Application.OpenForms.OfType<T>().FirstOrDefault();
        }

        #endregion functions

       
    }
}

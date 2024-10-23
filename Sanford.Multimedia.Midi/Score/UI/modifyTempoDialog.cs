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
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace Sanford.Multimedia.Midi.Score.UI
{
    public partial class modifyTempoDialog : Form
    {

        private bool TempoDoChange = true;
        private float _bpm;
        private float _tempo;
        private float _starttime;
        private decimal _division;

        private SheetMusic sheetmusic;
        TempoSymbol _tempoSymbol;
        Sequence sequence1;

        public int Division
        {
            get
            { return Convert.ToInt32(updDivision.Value); }
            set { 
                _division = value;
                updDivision.Value = _division; }
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


        public modifyTempoDialog(SheetMusic sheetMusic, Sequence seq, int deftempo)
        {
            InitializeComponent();

            sheetmusic = sheetMusic;
            sequence1 = seq;

            // Check if a TempoSymbol is selected
            TempoSymbol tempoSymbol = sheetMusic.GetSelectedTempoSymbol();
            if (tempoSymbol != null)
            {
                _tempoSymbol = tempoSymbol;
                _starttime = _tempoSymbol.StartTime;
                _tempo = _tempoSymbol.Tempo;
            }
            // if a note is selected
            else if (sheetMusic.SelectedNotes != null && sheetmusic.SelectedNotes.Count > 0)
            {
                MidiNote n = sheetMusic.SelectedNotes[0];
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

        private void btnOk_Click(object sender, EventArgs e)
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

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }

        private bool IsNumeric(string input)
        {
            int test;
            return int.TryParse(input, out test);
        }

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

        private void txtStartTime_TextChanged(object sender, EventArgs e)
        {
            UpdateFields();
           
        }

        private void UpdateFields()
        {
            try
            {
                if (txtStartTime.Text == "")
                    txtStartTime.Text = "0";

                _starttime = float.Parse(this.txtStartTime.Text);

                if (_starttime == 0)
                {
                    lblTempoNumber.Text = string.Format("Tempo {0}", 0);
                    btnDelete.Enabled = false;
                    txtStartTime.Enabled = false;
                    btnOk.Text = "Update";
                    return;
                }

                int index = IsTempoExists(_starttime);
                if (index != -1)
                {
                    btnDelete.Enabled = true;
                    lblTempoNumber.Text = string.Format("Tempo {0}", index);
                    txtStartTime.Enabled = true;
                    btnOk.Text = "Update";
                }
                else
                {
                    btnDelete.Enabled = false;
                    lblTempoNumber.Text = "New tempo";
                    txtStartTime.Enabled = true;
                    btnOk.Text = "Create";
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // Delere current tempo
        private void btnDelete_Click(object sender, EventArgs e)
        {
            List<TempoSymbol> l = sheetmusic.GetAllTempoChanges();                        
            TempoSymbol tmps = new TempoSymbol((int)_starttime, (int)_tempo);
            
            string msg;

            // The method l.Contains(tmps) does not work ??????                       
            if (!IsContains(tmps, l))
            {
                msg = "There is no tempo change at this location";    
                MessageBox.Show(msg, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error );
                return;
            }

            sheetmusic.DeleteTempoChange((int)_starttime);

            DisplayPreviousTempoChange();


        }

        private bool IsContains(TempoSymbol symbol, List<TempoSymbol> l) 
        { 
            foreach (TempoSymbol t in l)
            {
                if (symbol.StartTime == t.StartTime && symbol.Tempo == t.Tempo)
                    return true;
            }
            return false;
        }


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
                    break;
                }
            }
        }


        private int IsTempoExists(float ticks)
        {
            List<TempoSymbol> l = sheetmusic.GetAllTempoChanges();
            
            for (int i = 0; i < l.Count; i ++)            
            {
                TempoSymbol temposymbol = l[i];
                if (temposymbol.StartTime == ticks)
                {
                    return i;
                }
            }

            return -1;

        }



    }
}

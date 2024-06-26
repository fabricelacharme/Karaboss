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
using System.Windows.Forms;

namespace Sanford.Multimedia.Midi.Score.UI
{
    public partial class modifyTempoDialog : Form
    {

        private bool TempoDoChange = true;
        private float _bpm;
        private float _tempo;
        private float _starttime;


        public int Division
        {
            get
            { return Convert.ToInt32(updDivision.Value); }
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

        public modifyTempoDialog(int division, int tempo, int starttime)
        {
            InitializeComponent();
            
            updDivision.Value = Convert.ToDecimal(division);

            _tempo = tempo;
            txtTempo.Text = tempo.ToString();

            _starttime = starttime;
            txtStartTime.Text = starttime.ToString();
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
            _starttime = float.Parse(this.txtStartTime.Text);
        }
    }
}

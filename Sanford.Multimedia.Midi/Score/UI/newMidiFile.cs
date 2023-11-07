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

namespace Sanford.Multimedia.Midi.Score.UI
{
    public partial class frmNewMidiFile : Form
    {

        private List<decimal> NumeratorList = new List<decimal>() { 2, 3, 4, 6, 9, 12 };
        
        private List<decimal> DenominatorList = new List<decimal>() { 1, 2, 4, 8, 16, 32, 64 };
        private bool NumDoChange = true; // used for not looping in changeValue event
        private bool DenomDoChange = true; // used for not looping in changeValue event

        private decimal NumeratorValue = 4;
        private decimal DenominatorValue = 4;
        public int Numerator
        {
            get
            { return Convert.ToInt32(this.updNumerator.Value); }
        }
        public int Denominator
        {
            get
            { return Convert.ToInt32(this.updDenominator.Value); }
        }
        public int Division
        {
            get
            { return Convert.ToInt32(this.txtDivision.Text); }
        }


        private bool TempoDoChange = true;        
        private float _bpm;
        private float _tempo;
        public int Tempo
        {
            get
            { return Convert.ToInt32(_tempo); }
        }        

        public int Measures
        {
            get
            { return Convert.ToInt32(this.updMeasures.Value); }
        }
        
        public frmNewMidiFile(int inumerator, int idenominator, int division, int tempo, int measures)
        {
            InitializeComponent();

            updNumerator.Maximum = NumeratorList[NumeratorList.Count - 1];
            updNumerator.Minimum = NumeratorList[0];
            updDenominator.Maximum = DenominatorList[DenominatorList.Count - 1];
            updDenominator.Minimum = DenominatorList[0];

            updNumerator.Value = Convert.ToDecimal(inumerator);
            updDenominator.Value = Convert.ToDecimal(idenominator);
            txtDivision.Text = division.ToString();
            _tempo = tempo;
            txtTempo.Text = tempo.ToString();
            updMeasures.Value = Convert.ToDecimal(measures);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }

        private void updNumerator_ValueChanged(object sender, EventArgs e)
        {
            // if the event is calling from this event (when set .value) do nothing
            if (NumDoChange == false) return;

            decimal cv = updNumerator.Value;

            // if no change (possible?) do nothing
            if (cv == NumeratorValue) return;

            // if the value IS on array do nothing
            if (NumeratorList.Contains(cv))
            {
                NumeratorValue = cv;
                return;
            }

            // if precedent value is 8 and up arrow pressed
            // the current value is 9 so i search the index in array of
            // value -1 and i take next element
            if (cv > NumeratorValue) {                           //  up arrow
                int ix = NumeratorList.IndexOf(cv - 1) + 1;
                if (ix >= NumeratorList.Count) return;

                //ix = Array.IndexOf(NumeratorList, cv - 1) + 1;   //  get next element
                NumDoChange = false;                             //  stop ValueChanged event 
                updNumerator.Value = NumeratorList[ix];          //  here start a call to this event
                NumDoChange = true;                              //  reset ValueChange event on
            }

            // the same but precedent element
            if (cv < NumeratorValue) {                             // ' down arrow pressed
                int ix = NumeratorList.IndexOf(cv + 1) - 1;
                if (ix < 0) return;

                //ix = Array.IndexOf(NumeratorList, cv + 1) - 1;
                NumDoChange = false;
                updNumerator.Value = NumeratorList[ix];
                NumDoChange = true;
            }
            NumeratorValue = updNumerator.Value;
            
        }

        private void updDenominator_ValueChanged(object sender, EventArgs e)
        {
            // if the event is calling from this event (when set .value) do nothing
            if (DenomDoChange == false) return;

            decimal cv = updDenominator.Value;

            // if no change (possible?) do nothing
            if (cv == DenominatorValue) return;

            // if the value IS on array do nothing
            if (DenominatorList.Contains(cv))
            {
                DenominatorValue = cv;
                return;
            }

            // if precedent value is 8 and up arrow pressed
            // the current value is 9 so i search the index in array of
            // value -1 and i take next element
            if (cv > DenominatorValue)
            {                           //  up arrow
                int ix = DenominatorList.IndexOf(cv - 1) + 1;
                if (ix >= DenominatorList.Count) return;

                //ix = Array.IndexOf(NumeratorList, cv - 1) + 1;   //  get next element
                DenomDoChange = false;                             //  stop ValueChanged event 
                updDenominator.Value = DenominatorList[ix];          //  here start a call to this event
                DenomDoChange = true;                              //  reset ValueChange event on
            }

            // the same but precedent element
            if (cv < DenominatorValue)
            {                             // ' down arrow pressed
                int ix = DenominatorList.IndexOf(cv + 1) - 1;
                if (ix < 0) return;

                //ix = Array.IndexOf(NumeratorList, cv + 1) - 1;
                DenomDoChange = false;
                updDenominator.Value = DenominatorList[ix];
                DenomDoChange = true;
            }
            DenominatorValue = updDenominator.Value;

        }
   


        private bool IsNumeric(string input)
        {
            int test;
            return int.TryParse(input, out test);
        }
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

 

    }
}

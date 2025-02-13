using Sanford.Multimedia.Midi.Score;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Karaboss
{
    public partial class frmModifyTempo : Form
    {
        #region declarations
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

        //frmPlayer FrmPlayer;

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

        #endregion declarations

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sheetMusic"></param>
        /// <param name="seq"></param>
        public frmModifyTempo(SheetMusic sheetMusic, Sequence seq)
        {
            InitializeComponent();

            sheetmusic = sheetMusic;
            sequence1 = seq;
           
            // Check if a TempoSymbol was selected by doubleclick
            TempoSymbol tempoSymbol = sheetMusic.GetSelectedTempoSymbol();            

            if (tempoSymbol != null)
            {
                _tempoSymbol = tempoSymbol;
            }
            else
            {
                // No tempo was selected by double click
                _tempoSymbol = sheetmusic.lstTempoSymbols[0];
                sheetmusic.SelectTempoSymbol(_tempoSymbol);
                
            }            

            Division = sequence1.Division;
            updDivision.Value = Convert.ToDecimal(Division);
            txtTempo.Text = _tempoSymbol.Tempo.ToString();
            txtStartTime.Text = _tempoSymbol.StartTime.ToString();            
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
            decimal division; // = 0;
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
            List<TempoSymbol> l = sheetmusic.lstTempoSymbols;            
            float starttime = float.Parse(txtStartTime.Text);

            // Case of existing tempo displayed
            int index; // = -1;
            if (_tempoSymbol != null)
            {
                index = l.IndexOf(_tempoSymbol);
                if (index > 0)
                {
                    DisplayTempoSymbol(l[index - 1]);
                    return;
                }
            }
            
            // Current tempo symbol does not exist (deletion or update for egg)
            // Display previous existing tempo
            for (int i = l.Count - 1; i >= 0; i--)
            {
                if (l[i].StartTime < starttime)
                {
                    DisplayTempoSymbol(l[i]);
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
            List<TempoSymbol> l = sheetmusic.lstTempoSymbols;            
            float starttime = float.Parse(txtStartTime.Text);

            // Case of existing tempo displayed
            int index; // = -1;
            if (_tempoSymbol != null)
            {
                index = l.IndexOf(_tempoSymbol);
                if (index != -1 && index < l.Count - 1)
                {
                    DisplayTempoSymbol(l[index + 1]);
                    return;
                }
            }
           
            // Current tempo symbol does not exist (deletion or update for egg)
            // Display upper tempo, id exists
            for (int i = 0; i < l.Count; i++)
            {
                if (l[i].StartTime > starttime)
                {
                    DisplayTempoSymbol(l[i]);
                    break;
                }
            }           
            
        }

        private void DisplayTempoSymbol(TempoSymbol tempoSymbol)
        {
            _tempoSymbol = tempoSymbol;
            txtStartTime.Text = _tempoSymbol.StartTime.ToString();
            txtTempo.Text = _tempoSymbol.Tempo.ToString();

            UpdateFields();

            sheetmusic.SelectTempoSymbol(_tempoSymbol);

            if (Application.OpenForms.OfType<frmPlayer>().Count() > 0)
            {
                frmPlayer frmPlayer = Utilities.FormUtilities.GetForm<frmPlayer>();
                frmPlayer.ScrollTo(_tempoSymbol.StartTime);
            }
        }

        #endregion prev next


        #region functions

        /// <summary>
        /// Update existing tempo : same starttime, but different tempo value
        /// </summary>
        private void UpdateTempo()
        {
            // We must delete the previous tempo symbol
            List<TempoSymbol> l = sheetmusic.lstTempoSymbols;

            // old starttime
            int oldstarttime = Convert.ToInt32(txtStartTime.Text);
            // New tempo value
            int newtempo = Convert.ToInt32(txtTempo.Text); 
            // old tempo value
            int oldtempo = _tempoSymbol.Tempo;

            TempoSymbol tmps;

            // Search for an existing tempo symbol having same oldstarttime and newtempo to be created
            for (int i = 0; i < l.Count; i++)
            {
                tmps = l[i];
                if (tmps.StartTime == oldstarttime && tmps.Tempo == newtempo)
                {
                    //_tempoSymbol = l[i];
                    string tx = "This tempo already exists at this location";
                    MessageBox.Show(tx, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // Remove all tempo events at location oldstarttime and oldtempo
            sheetmusic.DeleteTempoChange(oldstarttime, oldtempo);
            // Create a tempo event at location oldstarttime and newtempo
            _tempoSymbol = sheetmusic.CreateTempoChange(oldstarttime, newtempo);
                        
            sheetmusic.SelectTempoSymbol(_tempoSymbol);

            // File modified
            UpdatefrmPlayer();

        }

        /// <summary>
        /// Create a new tempo: ie new starttime & new tempo value
        /// </summary>
        private void CreateTempo()
        {
            List<TempoSymbol> l = sheetmusic.lstTempoSymbols;

            int starttime = Convert.ToInt32(txtStartTime.Text);
            int tempo = Convert.ToInt32(txtTempo.Text);
            TempoSymbol tmps; 

            // Search for an existing tempo symbol having same starttime and tempo to be created
            for (int i = 0; i < l.Count; i++)
            {
                tmps = l[i];
                if (tmps.StartTime == starttime && tmps.Tempo == tempo)
                {                    
                    _tempoSymbol = l[i];
                    string tx = "This tempo already exists at this location";
                    MessageBox.Show(tx, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // Remove all tempo events at location starttime and tempo
            sheetmusic.DeleteTempoChange(starttime, tempo);
            _tempoSymbol = sheetmusic.CreateTempoChange(starttime, tempo);                        
            
            sheetmusic.SelectTempoSymbol(_tempoSymbol);

            // File modified
            UpdatefrmPlayer();
        }

        /// <summary>
        /// Delete an existing tempo
        /// </summary>
        private void DeleteTempo()
        {
            List<TempoSymbol> l = sheetmusic.lstTempoSymbols;
            TempoSymbol tmps;

            string msg;
            int starttime = Convert.ToInt32(txtStartTime.Text);
            int tempo = Convert.ToInt32(txtTempo.Text);
            bool bfound = false;            
            
            // Search for the existing tempo symbol having this starttime and tempo to be deleted
            for (int i = 0; i < l.Count; i++)
            {
                tmps = l[i];
                if (tmps.StartTime == starttime && tmps.Tempo == tempo)
                {                    
                    bfound = true;
                    _tempoSymbol = l[i];
                    break;
                }
            }

            if (!bfound)
            {
                msg = "There is no tempo change at this location";
                MessageBox.Show(msg, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            sheetmusic.DeleteTempoChange(starttime, tempo);

            // Check if a tempo symbol still exists having the same start time (case of several tempos at the same start time)
            // Refresh l
            l = sheetmusic.lstTempoSymbols;
            for (int i = l.Count - 1; i >= 0; i--)
            {
                if (l[i].StartTime == starttime)
                {
                    _tempoSymbol = l[i];
                    sheetmusic.SelectTempoSymbol(_tempoSymbol);
                    
                    UpdatefrmPlayer();
                    UpdateFields();

                    txtTempo.Text = _tempoSymbol.Tempo.ToString();

                    return;
                }
            } 

            DisplayPreviousTempoChange();
            UpdatefrmPlayer();
        }


        /// <summary>
        /// File was modified
        /// </summary>
        private void UpdatefrmPlayer()
        {
            if (Application.OpenForms.OfType<frmPlayer>().Count() > 0)
            {
                frmPlayer frmPlayer = Utilities.FormUtilities.GetForm<frmPlayer>();
                frmPlayer.UpdateTimes();
            }
        }

        /// <summary>
        /// Update fields according to the tempo displayed
        /// </summary>
        private void UpdateFields()
        {
            int index;
            List<TempoSymbol> l = sheetmusic.lstTempoSymbols;
            string tx;

            try
            {
                if (txtStartTime.Text == "")
                    txtStartTime.Text = "0";

                _starttime = float.Parse(this.txtStartTime.Text);

                if (_starttime == 0)
                {
                    index = 1;
                    lblTempoNumber.Text = string.Format("Tempo {0} of {1}", index, l.Count);

                    int nbStartTimes = 0;
                    for (int i =0; i < l.Count; i++)
                    {
                        if (l[i].StartTime == 0)
                            nbStartTimes++;
                    }

                    if (nbStartTimes == 1)
                        btnDelete.Enabled = false;
                    
                    ChangeMode = TempoChangesModes.UpdateTempo;
                    tx = Karaboss.Resources.Localization.Strings.Update;
                    btnUpdate.Text = tx;
                    
                    index = l.IndexOf(_tempoSymbol);
                    lblTempoNumber.Text = string.Format("Tempo {0} of {1}", index + 1, l.Count);

                    return;
                }

                
                if (_tempoSymbol != null && _tempoSymbol.StartTime == _starttime)
                {
                    index = l.IndexOf(_tempoSymbol);
                    btnDelete.Enabled = true;
                    lblTempoNumber.Text = string.Format("Tempo {0} of {1}", index + 1, l.Count);
                    txtStartTime.Enabled = true;
                    ChangeMode = TempoChangesModes.UpdateTempo;
                    tx = Karaboss.Resources.Localization.Strings.Update;
                    btnUpdate.Text = tx;

                }                
                else
                {
                    btnDelete.Enabled = false;
                    tx = Resources.Localization.Strings.NewTempo;
                    lblTempoNumber.Text = tx;
                    txtStartTime.Enabled = true;
                    ChangeMode = TempoChangesModes.CreateTempo;
                    tx = Karaboss.Resources.Localization.Strings.Create;
                    btnUpdate.Text = tx;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
      

        /// <summary>
        /// Test if data is numeric
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private bool IsNumeric(string input)
        {
            //int test;
            return int.TryParse(input, out int test);
        }        

        #endregion functions

       
    }
}

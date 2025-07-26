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
    public partial class frmModifyDivision : Form
    {

        /*
        Le temps dans le MIDI est déterminé par la division.
        La division, également appelée « résolution d'un fichier MIDI », indique la manière dont les ticks d'horloge doivent être traduits en temps.

        Elle représente le nombre de ticks d'horloge par noire.

        Une division de 480 signifie que la valeur de la noire sera de 480 ticks d'horloge, une croche 240, une double croche 120 etc.... 
        I faut donc s'assurer d'avoir une division suffisamment grande si on veut pouvoir gérer des triples ou quadruples croches.

        Les valeurs courantes de division sont 384, 480 et 960 tpq, car elles sont divisibles par 3, 4, 6 et 8, 
        qui sont les numérateurs et dénominateurs courants de la signature temporelle.

        Les delta-time, qui donnent l'instant d'un événement, s'expriment en ticks : 
        l'événement E survient après N ticks (à l'instant tick...). 

        Interviennent donc la finesse du tick et sa vitesse (période) : 
        si on divise le temps 2 fois plus finement en divisant par deux la durée du tick, 
        le même événement E apparaîtra après 2N ticks, mais à la même date absolue (résolution). 
        Indirectement (connaissant le tempo), la division indique la "durée" (cad. la finesse) du tick.

        
        ** Pulses per quarter note (PPQ)
        When the top bit of the time division bytes is 0, the time division is in ticks per beat. 
        The remaining 15 bits are the number of MIDI ticks per beat (per quarter note). 
        If, for example, these 15 bits compute to the number 60, then the time division is 60 ticks per beat and the length of one tick is
        1 tick = microseconds per beat / 60

        The variable "microseconds per beat" is specified by a set tempo meta message. 
        If it is not specified then it is 500,000 microseconds by default, which is equivalent to 120 beats per minute. 
        In the example above, if the MIDI time division is 60 ticks per beat and if the microseconds per beat is 500,000, 
        then 1 tick = 500,000 / 60 = 8333.33 microseconds.


        ** Frames per second
        When the top bit of the time division bytes is 1, the remaining 15 bits have to be broken in two pieces. 
        The top remaining 7 bits (the rest of the top byte) represent the number of frames per second and could be 24, 25, 29.97 
        (represented by 29), or 30. The low byte (the low 8 bits) describes the number of ticks per frame. 
        Thus, if, for example, there are 24 frames per second and there are 100 ticks per frame, 
        since there are 1,000,000 microseconds in a second, one tick is equal to
        1 tick = 1,000,000 / (24 * 100) = 416.66 microseconds

        Thus, when the time division top bit is 1, the length of a tick is strictly defined by the two time division bytes. 
        The first byte is the frames per second and the second byte is the number of ticks per frame, which is enough to specify the tick length exactly. 
        This is not so when the top bit of the time division bytes is 0 and the time division is in pulses per quarter note. 
        The time division in this case defines the ticks per beat, but nothing in the time division specifies the number of beats per second. 
        A MIDI message should be used to specify the number of beats per second (or the length of a beat), 
        or it should be left up to the MIDI device to set the tempo (120 beats per minute by default, as mentioned above).

        */


        Sequence sequence1;
        private decimal _division;
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

        public frmModifyDivision(Sequence seq)
        {
            InitializeComponent();

            sequence1 = seq;

            Division = sequence1.Division;
            updDivision.Value = Convert.ToDecimal(Division);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (updDivision.Value > 0)
            {
                //sequence1.Division = Convert.ToInt32(updDivision.Value);
                UpdatefrmPlayer();

                //DialogResult = DialogResult.OK;
                Close();

            }
            else
            {
                MessageBox.Show("The division must be greater than 0.", "Invalid Division", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /// <summary>
        /// File was modified
        /// </summary>
        private void UpdatefrmPlayer()
        {
            if (Application.OpenForms.OfType<frmPlayer>().Count() > 0)
            {
                frmPlayer frmPlayer = Utilities.FormUtilities.GetForm<frmPlayer>();
                frmPlayer.UpdateDivision(updDivision.Value);
            }
        }


        #region events
        private void updDivision_ValueChanged(object sender, EventArgs e)
        {
            decimal division;
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

        

        private void updDivision_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        #endregion events
    }
}

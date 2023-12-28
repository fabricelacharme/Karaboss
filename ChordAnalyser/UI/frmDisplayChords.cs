using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sanford.Multimedia.Midi;

namespace ChordAnalyser.UI
{
    public partial class frmDisplayChords : Form
    {
        
        private Sequence sequence1 = new Sequence();
        private ChordAnalyser.UI.ChordsControl chordAnalyserControl1;
        private Panel pnlDisplay;

        public frmDisplayChords(Sequence seq)
        {
            InitializeComponent();
            this.sequence1 = seq;
            DisplayChordControl();

        }

        private void DisplayChordControl()
        {
            // Panel Display
            pnlDisplay = new Panel();   
            pnlDisplay.SuspendLayout();
            pnlDisplay.BackColor = Color.FromArgb(70, 77, 95); // new Color(70; 77; 95);

            pnlDisplay.Left = 0;
            pnlDisplay.Top = 0;            
            pnlDisplay.Dock = DockStyle.Fill;
            this.Controls.Add(pnlDisplay);


            // ChordControl
            chordAnalyserControl1 = new ChordsControl();
            pnlDisplay.Controls.Add(chordAnalyserControl1);
            chordAnalyserControl1.Dock = DockStyle.Fill;


        }


    }
}

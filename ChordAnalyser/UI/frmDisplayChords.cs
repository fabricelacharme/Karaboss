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
            pnlDisplay.Location = new Point(0, 0);
            pnlDisplay.Size = new Size(40, 40);
            pnlDisplay.BackColor = Color.FromArgb(70, 77, 95);
            pnlDisplay.Dock = DockStyle.Fill;
            this.Controls.Add(pnlDisplay);


            // ChordControl
            chordAnalyserControl1 = new ChordsControl();
            chordAnalyserControl1.BackColor = Color.Red;
            chordAnalyserControl1.Left = 0;
            chordAnalyserControl1.Top = 0;
            chordAnalyserControl1.Dock = DockStyle.Fill;

            chordAnalyserControl1.Sequence1 = sequence1;

            pnlDisplay.Controls.Add(chordAnalyserControl1);

        }


    }
}

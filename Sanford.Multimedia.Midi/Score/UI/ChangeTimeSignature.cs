using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sanford.Multimedia.Midi.Score.UI
{
    public partial class ChangeTimeSignature : Form
    {
        public int Numerator
        {
            get { return (int)updNumerator.Value; }
        }

        public int Denominator
        {
            get { return (int)updDenominator.Value; }
        }

        public ChangeTimeSignature(int numerator, int denominator)
        {
            InitializeComponent();

            lblExplanation.AutoSize = true;
            string tx = "Time signatures consist of two numerals, one stacked above the other:";
            tx += "\n\nThe lower numeral indicates the note value that represents one beat(the beat unit).";
            tx += "\nThe upper numeral indicates how many such beats there are grouped together in a bar.";
            tx += "\n\nFor instance,";
            tx += "\n2 / 4 means two quarter - note(crotchet) beats per bar";
            tx += "\n3 / 8 means three eighth - note(quaver) beats per bar.";
            tx += "\n\nThe most common simple time signatures are 2 / 4, 3 / 4, and 4 / 4.";

            lblExplanation.Text = tx;

            lblActualValue.Text = numerator + "/" + denominator;
            updNumerator.Value = numerator;
            updDenominator.Value = denominator;


        }

        
    }
}

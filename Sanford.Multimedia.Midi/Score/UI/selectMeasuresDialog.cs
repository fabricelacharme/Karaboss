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
    public partial class selectMeasuresDialog : Form
    {

        public decimal MeasureFrom
        {
            get
            {
                return this.updFrom.Value;
            }
        }

        public decimal MeasureTo
        {
            get
            {
                return this.updTo.Value;
            }
        }

        public bool bAllMeasures
        {
            get
            {
                return chkAllMeasures.Checked;
            }
        }

        public selectMeasuresDialog(decimal measureFrom, int maxi)
        {
            InitializeComponent();

            if (maxi > updFrom.Maximum)
                updFrom.Maximum = maxi; updTo.Maximum = maxi;

            updFrom.Value = measureFrom;

        }

        private void updFrom_ValueChanged(object sender, EventArgs e)
        {
            if (updFrom.Value < 1)
                updFrom.Value = 1;

            if (updTo.Value < updFrom.Value)
                updTo.Value = updFrom.Value;
        }

        private void updTo_ValueChanged(object sender, EventArgs e)
        {
            if (updTo.Value < updFrom.Value)
                updTo.Value = updFrom.Value;
        }

        private void chkAllMeasures_CheckedChanged(object sender, EventArgs e)
        {
            updFrom.Enabled = !chkAllMeasures.Checked;
            updTo.Enabled = !chkAllMeasures.Checked;
        }
    }
}

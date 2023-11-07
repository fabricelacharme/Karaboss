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
    public partial class ModifyStartTimesDialog : Form
    {

        public int StartTime
        {
            get { return Int32.Parse(txtStartTime.Text); }
        }
        public int Offset
        {
            get { return Int32.Parse(txtTimeAmount.Text); }
        } 
        

        public ModifyStartTimesDialog()
        {
            InitializeComponent();
        }

    }
}

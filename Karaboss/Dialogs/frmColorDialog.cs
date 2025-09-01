using GradientApp;
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
    public partial class frmColorDialog : Form
    {
       

        public Color Color
        {
            get { return pictBox.BackColor; } 

            set { pictBox.BackColor = value; }
        }

        public frmColorDialog()
        {
            InitializeComponent();
        }

        private void cbColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbColor.SelectedItem is Color selectedColor)
            {                
                pictBox.BackColor = selectedColor;
            }
        }

        private void frmColorDialog_Load(object sender, EventArgs e)
        {
            // Set the ComboBox to display images.
            cboColor.DisplayKnownColors(cbColor);
        }
    }
}

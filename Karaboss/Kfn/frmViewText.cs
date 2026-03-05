using System;
using System.Drawing;
using System.Windows.Forms;

namespace Karaboss.Kfn
{
    public partial class frmViewText : Form
    {
        private object[] textSizes = new object[] { 14, 16, 18, 20, 22, 24, 26, 28, 30 };

        public frmViewText(string fileName, string text, string encoding)
        {
            InitializeComponent();

            Text = fileName + " (" + encoding + ")";

            cbTextSize.Items.AddRange(textSizes);
            cbTextSize.SelectedIndex = 0;

            txtElement.Font = new Font("Segoe UI", (float)cbTextSize.Items[0], FontStyle.Regular) ;
            txtElement.Text = text;


        }

        private void cbTextSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtElement.Font = new Font("Segoe UI", (float)cbTextSize.SelectedItem, FontStyle.Regular);            
        }
    }
}

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
    public partial class frmDialog : Form
    {
        public string Response
        {
            get { return txtReply.Text.Trim(); }
        }
        
        
        public frmDialog(string Question)
        {
            InitializeComponent();

            lblQuestion.Text = Question;
        }
    }
}

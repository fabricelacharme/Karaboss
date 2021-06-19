using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Karaboss.Buttons
{
    public partial class PlayButtonControl : UserControl
    {

        public new event EventHandler Click
        {
            add
            {
                base.Click += value;
                foreach (Control control in Controls)
                {
                    control.Click += value;
                }
            }
            remove
            {
                base.Click -= value;
                foreach (Control control in Controls)
                {
                    control.Click -= value;
                }
            }
        }

        public PlayButtonControl()
        {
            InitializeComponent();

            SetStyle(ControlStyles.Selectable, false);

            //this.Size = new Size(this.Button().Size.Width, this.Button().Size.Height);

        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Karaboss.Buttons
{
    /* Boutons utilisés pour le player de Karaboss
     * 
     */
    public partial class CheckButtonControl : UserControl
    {
        private Image CheckedButton() => Properties.Resources.Bouton_Check_PaleGreen_32;
        private Image UnCheckedButton() => Properties.Resources.Bouton_Cross_Red_32;
        private Image HoveredCheckedButton() => Properties.Resources.Bouton_Check_Gray_32;
        private Image HoveredUnCheckedButton() => Properties.Resources.Bouton_Cross_Gray_32;

        private bool _checked = true;
        public bool Checked
        {
            get { return _checked; }
            set
            {
                _checked = value;
                if (_checked)                
                    this.picBox.Image = this.CheckedButton();                
                else                
                    this.picBox.Image = this.UnCheckedButton();                
            }
        }

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

        public CheckButtonControl()
        {
            InitializeComponent();

            SetStyle(ControlStyles.Selectable, false);

            this.Size = new Size(this.CheckedButton().Size.Width, this.CheckedButton().Size.Height);
            this.picBox.Image = this.CheckedButton();
            this.picBox.SizeMode = PictureBoxSizeMode.StretchImage;

            this.picBox.MouseEnter += new EventHandler(picBox_MouseEnter);
            this.picBox.MouseLeave += new EventHandler(picBox_MouseLeave);

            //this.picBox.MouseEnter += (sender, args) => this.picBox.Image = this.HoveredButton();
            //this.picBox.MouseLeave += (sender, args) => this.picBox.Image = this.CheckedButton();
            //this.picBox.MouseUp += (sender, args) => this.picBox.Image = this.CheckedButton();
            //this.picBox.MouseDown += (sender, args) => this.picBox.Image = this.ClickedButton();
        }

        private void picBox_MouseLeave(object sender, EventArgs e)
        {
            if (_checked)
                this.picBox.Image = this.CheckedButton();
            else
                this.picBox.Image = this.UnCheckedButton();
        }

        private void picBox_MouseEnter(object sender, EventArgs e)
        {
            if (_checked)
                this.picBox.Image = this.HoveredCheckedButton();
            else
                this.picBox.Image = this.HoveredUnCheckedButton();
        }




    }
}

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
    public partial class MinusButtonControl : UserControl
    {
        private Image Button() => Properties.Resources.Bouton_minus_palegreen_32;        
        private Image HoveredButton() => Properties.Resources.Bouton_minus_gris_32;
        private Image ClickedButton() => Properties.Resources.Bouton_minus_vert2_32;

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

        public MinusButtonControl()
        {
            InitializeComponent();

            SetStyle(ControlStyles.Selectable, false);

            this.Size = new Size(this.Button().Size.Width, this.Button().Size.Height);
            this.picBox.Image = this.Button();
            this.picBox.SizeMode = PictureBoxSizeMode.StretchImage;
            this.picBox.MouseEnter += (sender, args) => this.picBox.Image = this.HoveredButton();
            this.picBox.MouseLeave += (sender, args) => this.picBox.Image = this.Button();
            this.picBox.MouseUp += (sender, args) => this.picBox.Image = this.Button();
            this.picBox.MouseDown += (sender, args) => this.picBox.Image = this.ClickedButton();
        }

    }
}

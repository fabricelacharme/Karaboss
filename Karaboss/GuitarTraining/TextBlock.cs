using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Karaboss.GuitarTraining
{
    public partial class TextBlock : UserControl
    {
        public SolidBrush Foreground { get; set; }
        public StringAlignment HorizontalAlignment { get; set; }
        public StringAlignment VerticalAlignment { get; set; }
        public int FontWeight { get; set; }
        public int FontSize { get; set; }
    

        public TextBlock()
        {
            InitializeComponent();
            Foreground = new SolidBrush(Color.Black);
            HorizontalAlignment = StringAlignment.Center;
            VerticalAlignment = StringAlignment.Center;

            this.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            
            Text = "0";
            
        }

        // Fond transparent
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams CP = base.CreateParams;
                CP.ExStyle |= 0x20;
                return CP;
            }
        }

        // Fond transparent
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        /// <summary>
        /// Draw control on paint event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;

                // Alignment
                StringFormat format = new StringFormat();
                format.Alignment = HorizontalAlignment;
                format.LineAlignment = VerticalAlignment;
                            
                Rectangle rect = new Rectangle(0, 0, Width, Height);
                g.DrawString(Text, Font, Foreground, rect, format);


            }
            catch (Exception ex)
            {

            }
            base.OnPaint(e);
        }

    }
}

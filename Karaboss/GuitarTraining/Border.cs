using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Karaboss.GuitarTraining
{
    public partial class Border : UserControl
    {
        //private GraphicsPath path = new GraphicsPath();
        private int radius = 14;

        public enum Shapes
        {
            Circle,
            Carret,
        }
        private Shapes _shape = Shapes.Carret;
        public Shapes Shape
        {
            get { return _shape; }
            set { _shape = value; }
        }


        public SolidBrush BackgroundBrush {get; set;}
        public Pen BorderPen { get; set; }
        public int CornerRadius { get; set; }

        private TextBlock _child = new TextBlock();
        public TextBlock Child {
           
            set {
                _child = value;
                this.Controls.Clear();
                _child.Location = new Point((Width - _child.Width)/2, (Height - _child.Height)/2);
                this.Controls.Add(_child);
                Invalidate();
            }
        }



        public Border()
        {
            InitializeComponent();

            _shape = Shapes.Carret;
            BackgroundBrush = new SolidBrush(Color.Yellow);
            BorderPen = new Pen(Color.White);
            Child = new TextBlock();
            radius = Width/2 - 1;
            CornerRadius = 5;
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
        /// Draw control on Paint event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;

                // Antialiasing
                g.SmoothingMode = SmoothingMode.AntiAlias;

                switch (_shape)
                {
                    case Shapes.Circle:
                        // Draw Circle
                        g.FillCircle(BackgroundBrush, Width/2, Height/2, radius);
                        g.DrawCircle(BorderPen, Width/2, Height/2, radius);
                        break;

                    case Shapes.Carret:
                        // Draw rectangle
                        Rectangle Rect = new Rectangle(0, 0, Width - 1, Height - 1);
                        
                        // Draw Rectangle with angles corners
                        //g.DrawRectangle(BorderPen, Rect);
                        //g.FillRectangle(BackgroundBrush, Rect);

                        // Draw rectangles with rounded corners
                        GraphicsExtensions.DrawRoundedRectangle(g, BorderPen, Rect, CornerRadius);
                        GraphicsExtensions.FillRoundedRectangle(g, BackgroundBrush, Rect, CornerRadius);


                        break;
                }
                g.Dispose();
            }
            catch (Exception ex)
            {

            }
            base.OnPaint(e);
        }

           
        private void Border_Resize(object sender, EventArgs e)
        {
            if (_shape == Shapes.Circle)
            {
                Width = Height;
                radius = Width / 2 - 1;
            }

            if (_child != null)
                _child.Location = new Point((Width - _child.Width) / 2, (Height - _child.Height) / 2);
        }
    }

    
   
    
}

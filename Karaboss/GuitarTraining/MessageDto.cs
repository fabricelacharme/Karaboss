using Sanford.Multimedia.Midi;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Karaboss.GuitarTraining
{
    public class MessageDto
    {
        public int id { get; set; }
        public ChannelCommand ChannelCommand { get; set; }
        public int MidiChannel { get; set; }
        public int Data1 { get; set; }
        public int Data2 { get; set; }
        public MessageType MessageType { get; set; }
        public int NoteDiffToPrevious { get; set; }
        public int NoteDiffToNext { get; set; }
        public int TickDiffToPrevious { get; set; }
        public int TickDiffToNext { get; set; }
        public int FretPosition { get; set; }
        public long Ticks { get; set; }
    }

    
    public class MyRectangle
    {
        public Brush Stroke { get; set; }
        public Brush Fill { get; set; }
        public int Height { get; set; }
        public Rectangle Rect { get; set; }        
        
        public MyRectangle(int x = 0, int y = 0, int W = 0, int H = 0)
        {
            Rect = new Rectangle(x, y, W, H);
        }
    }     

    /// <summary>
    /// Graphic extensions
    /// </summary>
    public static class GraphicsExtensions
    {
        public static void DrawCircle(this Graphics g, Pen pen,
                                      float centerX, float centerY, float radius)
        {
            g.DrawEllipse(pen, centerX - radius, centerY - radius,
                          radius + radius, radius + radius);
        }

        public static void FillCircle(this Graphics g, Brush brush,
                                      float centerX, float centerY, float radius)
        {
            g.FillEllipse(brush, centerX - radius, centerY - radius,
                          radius + radius, radius + radius);
        }
    

        // https://stackoverflow.com/questions/33853434/how-to-draw-a-rounded-rectangle-in-c-sharp
        public static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // top left arc  
            path.AddArc(arc, 180, 90);

            // top right arc  
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc  
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc 
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        public static void DrawRoundedRectangle(this Graphics graphics, Pen pen, Rectangle bounds, int cornerRadius)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");
            if (pen == null)
                throw new ArgumentNullException("pen");

            using (GraphicsPath path = RoundedRect(bounds, cornerRadius))
            {
                graphics.DrawPath(pen, path);
            }
        }

        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, Rectangle bounds, int cornerRadius)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");
            if (brush == null)
                throw new ArgumentNullException("brush");

            using (GraphicsPath path = RoundedRect(bounds, cornerRadius))
            {
                graphics.FillPath(brush, path);
            }
        }

    }


}

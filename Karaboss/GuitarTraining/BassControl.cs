using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sanford.Multimedia.Midi;
using System.Windows.Forms.VisualStyles;

namespace Karaboss.GuitarTraining
{
    public partial class BassControl : UserControl
    {
        private const int LowNoteID = 21;
        private const int HighNoteID = 109;

        private int FirstFretWidth = 30;
        private int FretWidth = 3;

        Dictionary<int, Border> dicNotesOn = new Dictionary<int, Border>();

        // Manche
        Color MancheColor = System.Drawing.ColorTranslator.FromHtml("#FF413225");

        // Bullets
        static Color BulletColor = System.Drawing.ColorTranslator.FromHtml("#F9F871");
        SolidBrush innerColor = new SolidBrush(BulletColor); //new SolidBrush(Color.Yellow);
        Pen outerColor = new Pen(Color.White);

        // First Frets
        SolidBrush FirstFretFillColor = new SolidBrush(Color.White);
        Pen FirstFretPenColor = new Pen(Color.Black);

        // Other frets
        //SolidBrush FretFillColor = new SolidBrush(Color.DarkGray);
        SolidBrush FretFillColor = new SolidBrush(Color.Gold);
        Pen FretPenColor = new Pen(Color.DarkGray); //new Pen(Color.Gold);


        SolidBrush stringOffColor = new SolidBrush(Color.DarkGray);
        SolidBrush stringOnColor = new SolidBrush(Color.Black); //new SolidBrush(Color.White);
        SolidBrush fontBrush = new SolidBrush(Color.Black);

        struct StringInfo
        {
            public int Row;
            public int Min;
            public int Max;
            public bool Played;
            public MyRectangle Rect;
        }
        StringInfo[] stringInfos;   // Strings infos

        List<int> FretLocations;    // Frets location

        private int deltaManche = 12;
        private int CordHeight = 1;
        private Color CordColor = Color.DarkGray;
        private Pen CordPen = new Pen(Color.DarkGray, 1);
        private SolidBrush CordBrush = new SolidBrush(Color.DarkGray);

        private MyRectangle string0 = new MyRectangle();        
        private MyRectangle string1 = new MyRectangle();
        private MyRectangle string2 = new MyRectangle();
        private MyRectangle string3 = new MyRectangle();
       

        public BassControl()
        {
            InitializeComponent();
            /*
            this.SetStyle(
               System.Windows.Forms.ControlStyles.UserPaint |
               System.Windows.Forms.ControlStyles.AllPaintingInWmPaint |
               System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer,
               true);
            */

            CordHeight = 1;
            CordColor = Color.DarkGray;
            CordPen = new Pen(CordColor, CordHeight);
            CordBrush = new SolidBrush(CordColor);

            string0.Stroke = string0.Fill = stringOffColor;
            string1.Stroke = string1.Fill = stringOffColor;
            string2.Stroke = string2.Fill = stringOffColor;
            string3.Stroke = string3.Fill = stringOffColor;

            // BASS = 4 strings            
            stringInfos = new StringInfo[4];

            /*
            // ne tient compte que des notes uniques pour chaque corde 
            // => on ne joue jamais dans les parties éloignées du manche 
            // sauf pour la corde des aigus 
            stringInfos[0] = new StringInfo() { Row = 3, Min = 28, Max = 32, Rect = string3 };      // E low 
            stringInfos[1] = new StringInfo() { Row = 2, Min = 33, Max = 37, Rect = string2 };      // A
            stringInfos[2] = new StringInfo() { Row = 1, Min = 38, Max = 42, Rect = string1 };      // D
            stringInfos[3] = new StringInfo() { Row = 0, Min = 43, Max = 78, Rect = string0 };      // G high
            */

            stringInfos[0] = new StringInfo() { Row = 3, Min = 28, Max = 52, Played = false, Rect = string3 };      // E low 
            stringInfos[1] = new StringInfo() { Row = 2, Min = 33, Max = 57, Played = false, Rect = string2 };      // A
            stringInfos[2] = new StringInfo() { Row = 1, Min = 38, Max = 62, Played = false, Rect = string1 };      // D
            stringInfos[3] = new StringInfo() { Row = 0, Min = 43, Max = 67, Played = false, Rect = string0 };      // G high

            FretLocations = new List<int>();
        }

        /// <summary>
        /// Anti flicker (does not work if you move or resize the windows parent)
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                var parms = base.CreateParams;
                parms.Style &= ~0x02000000;  // Turn off WS_CLIPCHILDREN
                return parms;
            }
        }


        /// <summary>
        /// Get StringInfo: search most appropriate string (trebble first)
        /// </summary>
        /// <param name="noteNumber"></param>
        /// <returns></returns>
        private StringInfo GetStringInfo(int noteNumber)
        {
            var stringInfoQuery = from si in stringInfos
                                  where noteNumber >= si.Min && noteNumber <= si.Max && si.Played == false
                                  select si;
            if (stringInfoQuery.Any())
            {
                // Favorise les aigus (last string = trebble)                
                var stringInfo = stringInfoQuery.Last();
                return stringInfo;
            }
            else
                return new StringInfo();
        }


        public void Send(ChannelMessage message)
        {
            if (message.Command == ChannelCommand.NoteOn &&
                message.Data1 >= LowNoteID && message.Data1 <= HighNoteID)
            {
                if (message.Data2 > 0)
                {
                    if (!dicNotesOn.ContainsKey(message.Data1))
                    {
                        var row = 0;
                        var col = 0;
                        var stringId = 0;
                        MyRectangle stringRect = new MyRectangle();

                        //We look for the StringInfo matching the
                        //note information
                        var stringInfo = GetStringInfo(message.Data1);
                                              
                        if (stringInfo.Rect != null)
                        {
                            row = stringInfo.Row;
                            col = message.Data1 - stringInfo.Min; // Exemple E Low = 40, so 40 - 40 = 0 ie fret = 0 

                            stringRect = stringInfo.Rect;
                            stringId = stringInfo.Row;

                            // paint played string
                            stringRect.Stroke = stringRect.Fill = stringOnColor;
                            stringRect.Height = 1;

                            // String played                            
                            stringInfos[3 - stringId].Played = true;

                            //This border shows which note
                            //is being played
                            var noteOn = new Border()
                            {
                                Width = 28,
                                Height = 18,
                                BackgroundBrush = innerColor,
                                BorderPen = outerColor,
                                Tag = stringId,
                                CornerRadius = 5
                            };

                            //This text block displays
                            //the fret number
                            var txt = new TextBlock()
                            {
                                Text = col.ToString(),
                                Foreground = fontBrush,
                                HorizontalAlignment = StringAlignment.Center,
                                VerticalAlignment = StringAlignment.Center,
                                //FontWeight = FontWeights.Bold,
                                FontSize = 10
                            };

                            noteOn.Child = txt;

                            // Add note to dictionary
                            dicNotesOn.Add(message.Data1, noteOn);

                            // Locate Border
                            noteOn.Top = stringRect.Rect.Y - noteOn.Height / 2;
                            if (col < FretLocations.Count)
                                noteOn.Left = FretLocations[col] - noteOn.Width; // - FretWidth;

                            this.Controls.Add(noteOn);
                            noteOn.BringToFront();
                            Invalidate();
                        }
                    }
                }
                else if (message.Data2 == 0)
                {
                    if (dicNotesOn.ContainsKey(message.Data1))
                    {
                        var noteOff = dicNotesOn[message.Data1];
                        dicNotesOn.Remove(message.Data1);
                        this.Controls.Remove(noteOff);                        

                        var stringId = (int)noteOff.Tag;
                        TurnOffString(stringId);
                    }
                }
            }
            else if (message.Command == ChannelCommand.NoteOff)
            {
                if (dicNotesOn.ContainsKey(message.Data1))
                {
                    var noteOff = dicNotesOn[message.Data1];
                    dicNotesOn.Remove(message.Data1);
                    this.Controls.Remove(noteOff);                

                    var stringId = (int)noteOff.Tag;
                    TurnOffString(stringId);
                }
            }
        }

        /// <summary>
        /// Turn off string
        /// </summary>
        /// <param name="stringId"></param>
        private void TurnOffString(int stringId)
        {
            MyRectangle stringRect = new MyRectangle();
            switch (stringId)
            {
                case 0:
                    stringRect = string0;
                    break;
                case 1:
                    stringRect = string1;
                    break;
                case 2:
                    stringRect = string2;
                    break;
                case 3:
                    stringRect = string3;
                    break;
            }
            stringRect.Height = 1;
            stringRect.Stroke = stringRect.Fill = stringOffColor;

            // string played 
            stringInfos[3 - stringId].Played = false;

            Invalidate();
        }

        /// <summary>
        /// Clear all Borders
        /// </summary>
        public void Clear()
        {            
            dicNotesOn.Clear();
            for (var i = this.Controls.Count - 1; i >= 0; i--)
            {
                if (this.Controls[i] is Border)
                {
                    var ell = this.Controls[i] as Border;
                    if (ell.Tag != null)
                        this.Controls.RemoveAt(i);
                }
            }

            for (var i = 0; i < 4; i++)
            {
                TurnOffString(i);
            }            
        }


        #region Paint
        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;

                // Antialiasing
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Draw Bass
                DrawBass(g);
                
                g.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            base.OnPaint(e);
        }

        /// <summary>
        /// Draw Bass guitar: 4 strings, 24 frets
        /// </summary>
        /// <param name="g"></param>
        private void DrawBass(Graphics g)
        {
            int H = ClientSize.Height;
            int W = ClientSize.Width;

            #region Draw Manche

            Rectangle R1 = new Rectangle(0, 0, W - 1, H - 1);            
            Pen ManchePen = new Pen(MancheColor);          
            SolidBrush MancheBrush = new SolidBrush(MancheColor);

            g.DrawRectangle(ManchePen, R1);
            g.FillRectangle(MancheBrush, R1);

            #endregion


            #region Draw frets (24)

            // Draw first fret
            FretLocations.Clear();

            // Intervall between strings
            int e = (H - 2 * deltaManche) / 3;

            Rectangle F1 = new Rectangle(0, 0, FirstFretWidth, H - 1);
            g.DrawRectangle(FirstFretPenColor, F1);
            g.FillRectangle(FirstFretFillColor, F1);

            int X = FirstFretWidth;
            int LastX = X;
            FretLocations.Add(X);

            // Draw 24 frets
            for (int i = 1; i < 25; i++)
            {
                X = FirstFretWidth + i * (W - FirstFretWidth) / 24;
                F1 = new Rectangle(X, 0, FretWidth, H);
                g.DrawRectangle(FretPenColor, F1);
                g.FillRectangle(FretFillColor, F1);

                int Radius = ((X - LastX) / 5) < H / 6 ? (X - LastX) / 5: H / 6;

                switch (i)
                {
                    case 3:
                    case 5:
                    case 7:
                    case 9:
                    case 15:
                    case 17:
                    case 19:
                    case 21:
                        // One circle
                        g.DrawCircle(Pens.White, LastX + (X - LastX) / 2, H / 2, Radius);
                        g.FillCircle(Brushes.White, LastX + (X - LastX) / 2, H / 2, Radius);
                        break;
                    case 12:
                    case 24:
                        // Two circles
                        int Y1 = deltaManche + e / 2; 
                        int Y2 = H - deltaManche - e / 2; 
                        g.DrawCircle(Pens.White, LastX + (X - LastX) / 2, Y1, Radius);
                        g.FillCircle(Brushes.White, LastX + (X - LastX) / 2, Y1, Radius);
                        g.DrawCircle(Pens.White, LastX + (X - LastX) / 2, Y2, Radius);
                        g.FillCircle(Brushes.White, LastX + (X - LastX) / 2, Y2, Radius);

                        break;

                    default:
                        break;
                }
                LastX = X;
                FretLocations.Add(X);
            }

            #endregion


            #region Draw strings

            
            string0.Rect = new Rectangle(0, deltaManche, W, CordHeight);
            g.DrawRectangle(new Pen(string0.Fill, CordHeight), string0.Rect);
            g.FillRectangle(string0.Fill, string0.Rect);


            string1.Rect = new Rectangle(0, deltaManche + e, W, CordHeight);
            g.DrawRectangle(new Pen(string1.Fill, CordHeight), string1.Rect);
            g.FillRectangle(string1.Fill, string1.Rect);


            string2.Rect = new Rectangle(0, deltaManche + 2*e, W, CordHeight);
            g.DrawRectangle(new Pen(string2.Fill, CordHeight), string2.Rect);
            g.FillRectangle(string2.Fill, string2.Rect);


            string3.Rect = new Rectangle(0, H - deltaManche, W, CordHeight);
            g.DrawRectangle(new Pen(string3.Fill, CordHeight), string3.Rect);
            g.FillRectangle(string3.Fill, string3.Rect);

            #endregion

        }

        #endregion

        private void BassControl_Resize(object sender, EventArgs e)
        {
            
        }
    }
}

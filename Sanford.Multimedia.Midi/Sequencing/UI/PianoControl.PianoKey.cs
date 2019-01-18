#region License

/* Copyright (c) 2006 Leslie Sanford
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to 
 * deal in the Software without restriction, including without limitation the 
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
 * sell copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software. 
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 * THE SOFTWARE.
 */

#endregion

#region Contact

/*
 * Leslie Sanford
 * Email: jabberdabber@hotmail.com
 */

#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Sanford.Multimedia;

namespace Sanford.Multimedia.Midi.UI
{
    public partial class PianoControl
    {
        private class PianoKey : Control
        {
            private PianoControl owner;

            private bool on = false;

            private SolidBrush onBrush = new SolidBrush(Color.SkyBlue);
            private SolidBrush offBrush = new SolidBrush(Color.White);            
            private SolidBrush textBrush = new SolidBrush(Color.DimGray);

            private Font fontNoteLetter; //= new Font("Arial", 8, FontStyle.Regular, GraphicsUnit.Pixel);

            private int noteID = 60;
            private string noteLetter = "C";

            public PianoKey(PianoControl owner)
            {
                this.owner = owner;
                this.TabStop = false;

                if (owner.Orientation == Orientation.Vertical)
                    fontNoteLetter = new Font("Arial", 10, FontStyle.Regular, GraphicsUnit.Pixel);
                else
                    fontNoteLetter = new Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Pixel);

            }

            public void PressPianoKey()
            {
                #region Guard

                if(on)
                {
                    return;
                }

                #endregion

                on = true;

                Invalidate();

                owner.OnPianoKeyDown(new PianoKeyEventArgs(noteID));
            }

            public void ReleasePianoKey()
            {
                #region Guard

                if(!on)
                {
                    return;
                }

                #endregion

                on = false;

                Invalidate();

                owner.OnPianoKeyUp(new PianoKeyEventArgs(noteID));
            }

            protected override void Dispose(bool disposing)
            {
                if(disposing)
                {
                    onBrush.Dispose();
                    offBrush.Dispose();
                }

                base.Dispose(disposing);
            }

            protected override void OnMouseEnter(EventArgs e)
            {
                if(Control.MouseButtons == MouseButtons.Left)
                {
                    PressPianoKey();
                }
                                
                
                base.OnMouseEnter(e);
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                if(on)
                {
                    ReleasePianoKey();
                }

                base.OnMouseLeave(e);
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                PressPianoKey();

                if(!owner.Focused)
                {
                    owner.Focus();
                }

                base.OnMouseDown(e);
            }

            protected override void OnMouseUp(MouseEventArgs e)
            {
                ReleasePianoKey();

                base.OnMouseUp(e);
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                if (e.X < 0 || e.X > Width || e.Y < 0 || e.Y > Height)
                {
                    Capture = false;
                }

                base.OnMouseMove(e);
            }
    

            protected override void OnPaint(PaintEventArgs e)
            {
                if(on)
                {
                    e.Graphics.FillRectangle(onBrush, 0, 0, Size.Width, Size.Height);
                }
                else
                {
                    e.Graphics.FillRectangle(offBrush, 0, 0, Size.Width, Size.Height);
                }

                e.Graphics.DrawRectangle(Pens.Black, 0, 0, Size.Width - 1, Size.Height - 1);

                // FAB: draw note letter only for C note
                if (noteID % 12 == 0)
                {
                    PointF P;
                    Size size = TextRenderer.MeasureText(noteLetter, fontNoteLetter);

                    if (owner.Orientation == Orientation.Vertical)
                        P = new PointF(Size.Width - size.Width - 10, (Size.Height - fontNoteLetter.Height) / 2);
                    else
                        P = new PointF((Size.Width - size.Width) / 2, Top + Size.Height - 30);

                    e.Graphics.DrawString(noteLetter, fontNoteLetter, textBrush, P);
                }
                base.OnPaint(e);
            }

            public string NoteLetter
            {
                get
                {
                    return noteLetter;
                }
                set
                {
                    noteLetter = value;
                }
            }

            public Color NoteOnColor
            {
                get
                {
                    return onBrush.Color;
                }
                set
                {
                    onBrush.Color = value;

                    if(on)
                    {
                        Invalidate();
                    }
                }
            }

            public Color NoteOffColor
            {
                get
                {
                    return offBrush.Color;
                }
                set
                {
                    offBrush.Color = value;

                    if(!on)
                    {
                        Invalidate();
                    }
                }
            }

            public int NoteID
            {
                get
                {
                    return noteID;
                }
                set
                {
                    #region Require

                    if(value < 0 || value > ShortMessage.DataMaxValue)
                    {
                        throw new ArgumentOutOfRangeException("NoteID", noteID,
                            "Note ID out of range.");
                    }

                    #endregion

                    noteID = value;
                }
            }

            public bool IsPianoKeyPressed
            {
                get
                {
                    return on;
                }
            }
        }
    }
}
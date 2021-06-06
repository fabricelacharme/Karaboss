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
using System.Collections;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Sanford.Multimedia.Midi.UI
{
    public partial class PianoControl : Control
    {
        
        private enum KeyType
        {
            White,
            Black
        }
        
        #region static
        private readonly static Hashtable keyTable = new Hashtable();
        // 128 notes from 0 bass (C0) to trebble 127 (G9) on horizontal mode
        private static readonly KeyType[] KeyTypeTable = 
            {
                KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White, KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White,
                KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White, KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White,
                KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White, KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White,
                KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White, KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White,
                KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White, KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White,
                KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White, KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White,
                KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White, KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White,
                KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White, KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White,
                KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White, KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White,
                KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White, KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White,
                KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White, KeyType.White, KeyType.Black, KeyType.White
            };
        private static readonly string[] NotesTable = 
        {
            "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B", "B#"
        };

        #endregion


        #region events & callbacks
        /// <summary>
        /// Events & Callbacks
        /// </summary>
        /// <param name="message"></param>
        // FAB: Regrouping of noteon noteoff ?
        private delegate void NoteMessageCallback(ChannelMessage message);

        private NoteMessageCallback noteOnCallback;
        private NoteMessageCallback noteOffCallback;
        public event EventHandler<PianoKeyEventArgs> PianoKeyDown;
        public event EventHandler<PianoKeyEventArgs> PianoKeyUp;

        #endregion


        #region private

        // Not all 128 notes taken : 21 to 109 = 88 notes
        private const int DefaultLowNoteID = 23;    // 23
        private const int DefaultHighNoteID = 108;   // 108
        //private const int DefaultLowNoteID = 0;         // CO = lowest note
        //private const int DefaultHighNoteID = 127;      // G9 = highest note

        private const double BlackKeyScale = 0.666666666;

        private SynchronizationContext context;        
        
        private int octaveOffset = 5;
        
        // The list of piano keys
        private PianoKey[] keys = null;
        private int whiteKeyCount;
        private int KeyCount;    //FAB       
                
        #endregion


        public Point CurrentPoint;

        #region properties

        private int lowNoteID = DefaultLowNoteID;
        /// <summary>
        /// Sets of gets lower note
        /// </summary>
        public int LowNoteID
        {
            get
            {
                return lowNoteID;
            }
            set
            {
                #region Require

                if (value < 0 || value > ShortMessage.DataMaxValue)
                {
                    throw new ArgumentOutOfRangeException("LowNoteID", value,
                        "Low note ID out of range.");
                }

                #endregion

                #region Guard

                if (value == lowNoteID)
                {
                    return;
                }

                #endregion

                lowNoteID = value;

                if (lowNoteID > highNoteID)
                {
                    highNoteID = lowNoteID;
                }

                //Invalidate();
                CreatePianoKeys();
                InitializePianoKeys();
            }
        }

        private int highNoteID = DefaultHighNoteID;
        /// <summary>
        /// Sets or gets higher note
        /// </summary>
        public int HighNoteID
        {
            get
            {
                return highNoteID;
            }
            set
            {
                #region Require

                if (value < 0 || value > ShortMessage.DataMaxValue)
                {
                    throw new ArgumentOutOfRangeException("HighNoteID", value,
                        "High note ID out of range.");
                }

                #endregion

                #region Guard

                if (value == highNoteID)
                {
                    return;
                }

                #endregion

                highNoteID = value;

                if (highNoteID < lowNoteID)
                {
                    lowNoteID = highNoteID;
                }

                //Invalidate();
                CreatePianoKeys();
                InitializePianoKeys();
            }
        }

        private Color noteOnColor = Color.SkyBlue;
        /// <summary>
        /// Sets or gets Note On color
        /// </summary>
        public Color NoteOnColor
        {
            get
            {
                return noteOnColor;
            }
            set
            {
                #region Guard

                if (value == noteOnColor)
                {
                    return;
                }

                #endregion

                noteOnColor = value;

                foreach (PianoKey key in keys)
                {
                    key.NoteOnColor = noteOnColor;
                }
            }
        }

        private int _scale = 20; //FAB : Unit of vertical
        /// <summary>
        /// Sets or gets unit of vertical
        /// </summary>
        public int Scale
        {
            get
            {
                return _scale;
            }
            set
            {
                _scale = value;
            }
        }
       

        private int _totallength = 0;
        public int TotalLength
        {
            get
            {
                return _totallength;
            }
        }


        private Orientation _orientation;
        /// <summary>
        /// Orientation of the piano: horizontal or vertical
        /// </summary>
        public Orientation Orientation
        {
            get
            {
                return _orientation;
            }
            set
            {
                _orientation = value;
                //Invalidate();
                CreatePianoKeys();
                InitializePianoKeys();
            }
        }

        private float _zoom = 1.0f;
        /// <summary>
        /// Sets or gets Zoom value
        /// </summary>
        public float Zoom
        {
            get
            { return _zoom; }
            set
            {                
                int newvalue = (int)(20 * value);
                if (newvalue % 2 != 0)
                {
                    while (newvalue % 2 != 0)
                    {
                        newvalue--;
                    }
                }

                if (newvalue > 0)
                {
                    _zoom = value;    
                    _scale = newvalue;      // scale must be pair for the pianoRollControl (?)

                    //Invalidate();
                    InitializePianoKeys();
                }
            }
        }

        #endregion


        static PianoControl()
        {
            keyTable.Add(Keys.A, 0);
            keyTable.Add(Keys.W, 1);
            keyTable.Add(Keys.S, 2);
            keyTable.Add(Keys.E, 3);
            keyTable.Add(Keys.D, 4);
            keyTable.Add(Keys.F, 5);
            keyTable.Add(Keys.T, 6);
            keyTable.Add(Keys.G, 7);
            keyTable.Add(Keys.Y, 8);
            keyTable.Add(Keys.H, 9);
            keyTable.Add(Keys.U, 10);
            keyTable.Add(Keys.J, 11);
            keyTable.Add(Keys.K, 12);
            keyTable.Add(Keys.O, 13);
            keyTable.Add(Keys.L, 14);
            keyTable.Add(Keys.P, 15);
        }

        // Constructor
        public PianoControl()
        {                        
            this.TabStop = true;
            
            // Orientation Default value in constructor            
            _orientation = Orientation.Vertical;

            _scale = 20;

            CreatePianoKeys();
            InitializePianoKeys();

            context = SynchronizationContext.Current;
            
            this.LostFocus += new EventHandler(PianoControl_LostFocus);
            this.GotFocus += new EventHandler(PianoControl_GotFocus);            
            this.MouseLeave += new EventHandler(PianoControl_MouseLeave);

            this.DoubleBuffered = true;

            noteOnCallback = delegate(ChannelMessage message)
            {
                if(message.Data2 > 0)
                {
                    if (Orientation == Orientation.Horizontal)
                    {
                        keys[message.Data1 - lowNoteID].NoteOnColor = ChannelColor(message.MidiChannel);
                        keys[message.Data1 - lowNoteID].PressPianoKey();
                    }
                    else
                    {
                        keys[highNoteID - message.Data1].NoteOnColor = ChannelColor(message.MidiChannel);
                        keys[highNoteID - message.Data1].PressPianoKey();
                    }
                }
                else
                {
                    if (Orientation == Orientation.Horizontal)
                        keys[message.Data1 - lowNoteID].ReleasePianoKey();
                    else
                        keys[highNoteID - message.Data1].ReleasePianoKey();
                }
            };

            noteOffCallback = delegate(ChannelMessage message)
            {
                if (Orientation == Orientation.Horizontal)
                    keys[message.Data1 - lowNoteID].ReleasePianoKey();
                else
                    keys[highNoteID - message.Data1].ReleasePianoKey();
            };            
        }


        #region events

        private void PianoControl_MouseLeave(object sender, EventArgs e)
        {
            Capture = false;
        }   
     
        private void PianoControl_GotFocus(object sender, EventArgs e)
        {            

            if (Parent.GetType() == typeof(Panel))
            {
                Panel Pa = (Panel)Parent;
                Pa.AutoScrollPosition = new Point(Math.Abs(CurrentPoint.X), Math.Abs(CurrentPoint.Y));
            }
        }

        private void PianoControl_LostFocus(object sender, EventArgs e)
        {
            if (Parent.GetType() == typeof(Panel))
            {
                Panel Pa = (Panel)Parent;
                CurrentPoint = Pa.AutoScrollPosition;
            }

        }

        #endregion


        #region private functions

        private void CreatePianoKeys()
        {
            // If piano keys have already been created.
            if(keys != null)
            {
                // Remove and dispose of current piano keys.
                foreach(PianoKey key in keys)
                {
                    Controls.Remove(key);
                    key.Dispose();
                }
            }

            keys = new PianoKey[1 + HighNoteID - LowNoteID];
            KeyCount = 1 + HighNoteID - LowNoteID;              // Total number of keys            

            whiteKeyCount = 0;
            

            for(int i = 0; i < keys.Length; i++)
            {
                keys[i] = new PianoKey(this);
                
                if (_orientation == Orientation.Horizontal)
                    keys[i].NoteID = i + LowNoteID;  // from bass to trebble for horizontal view
                else
                    keys[i].NoteID = highNoteID - i;  // from trebble to bass for vertical view

                int note = keys[i].NoteID;     
                

                if(KeyTypeTable[note] == KeyType.White)
                {
                    whiteKeyCount++;
                }
                else
                {
                    keys[i].NoteOffColor = Color.Black;
                    keys[i].BringToFront();
                }

                keys[i].NoteOnColor = NoteOnColor;

                int idx = note % 12;
                int octave = (note + 3) / 12 - 1;

                keys[i].NoteLetter = NotesTable[idx] + octave.ToString();

                Controls.Add(keys[i]);
            }            

        }

        private void InitializePianoKeys()
        {
            #region Guard

            if (keys.Length == 0)
            {
                return;
            }

            #endregion

            int blackKeyWidth = 0;
            int blackKeyHeight = 0;
            int whiteKeyWidth = 0;
            int whiteKeyHeight = 0;
            //int offset = 0;
            int n = 0;
            //int w = 0;
            int h = 0;
            int tt = 0;

            //totalheight = 0;
            _totallength = 0;

            // FAB: allow vertical orientation 
            switch (_orientation)
            {
                case Orientation.Horizontal:
                    #region horizontal

                    whiteKeyWidth = 0;

                    blackKeyWidth = _scale;
                    blackKeyHeight = (int)(Height * BlackKeyScale);

                    while (n < keys.Length)
                    {
                        int note = keys[n].NoteID;

                        if (KeyTypeTable[note] == KeyType.White)
                        {
                            if (note == highNoteID)
                            {
                                if ((note % 12 == 0)
                                   || (note % 12 == 5))
                                {
                                    whiteKeyWidth = _scale;
                                }
                                else
                                {
                                    whiteKeyWidth = 3 * _scale / 2;
                                }
                            }
                            else if (note == lowNoteID)
                            {
                                if ((note % 12 == 4)
                                    || (note % 12 == 11))
                                {
                                    whiteKeyWidth = _scale;
                                }
                                else
                                    whiteKeyWidth = 3 * _scale / 2;
                            }
                            else if ((note % 12 == 0)
                                || (note % 12 == 4)
                                || (note % 12 == 5)
                                || (note % 12 == 11))
                            {
                                whiteKeyWidth = 3 * _scale / 2;
                            }
                            else
                            {
                                whiteKeyWidth = 2 * _scale;
                            }


                            keys[n].Height = Height;
                            keys[n].Width = whiteKeyWidth;
                            keys[n].Location = new Point(h, 0);
                            h += whiteKeyWidth;
                            tt += whiteKeyWidth + 1;
                            n++;
                        }
                        else
                        {
                            keys[n].Width = _scale;
                            keys[n].Height = blackKeyHeight;

                            if (note == highNoteID)
                            {
                                keys[n].Location = new Point(0, 0);
                                h = _scale / 2;
                            }
                            else
                                keys[n].Location = new Point(h - _scale / 2, 0);


                            keys[n].BringToFront();
                            n++;
                        }
                    }
                    #endregion horizontal
                    break;

                case Orientation.Vertical:
                    #region vertical
                    whiteKeyHeight = 0;

                    blackKeyHeight = _scale;
                    blackKeyWidth = (int)(Width * BlackKeyScale);


                    while (n < keys.Length)
                    {
                        int note = keys[n].NoteID;

                        if (KeyTypeTable[note] == KeyType.White)
                        {
                            if (note == highNoteID)
                            {
                                if ((note % 12 == 0)
                                   || (note % 12 == 5))
                                {
                                    whiteKeyHeight = _scale;
                                }
                                else
                                {
                                    whiteKeyHeight = 3 * _scale / 2;
                                }
                            }
                            else if (note == lowNoteID)
                            {
                                if ((note % 12 == 4)
                                    || (note % 12 == 11))
                                {
                                    whiteKeyHeight = _scale;
                                }
                                else
                                    whiteKeyHeight = 3 * _scale / 2;
                            }
                            else if ((note % 12 == 0)
                                || (note % 12 == 4)
                                || (note % 12 == 5)
                                || (note % 12 == 11))
                            {
                                whiteKeyHeight = 3 * _scale / 2;
                            }
                            else
                            {
                                whiteKeyHeight = 2 * _scale;
                            }


                            keys[n].Width = Width;
                            keys[n].Height = whiteKeyHeight;
                            keys[n].Location = new Point(0, h);
                            h += whiteKeyHeight;
                            n++;
                        }
                        else
                        {
                            keys[n].Height = _scale;
                            keys[n].Width = blackKeyWidth;

                            if (note == highNoteID)
                            {
                                keys[n].Location = new Point(0, 0);
                                h = _scale / 2;
                            }
                            else
                                keys[n].Location = new Point(0, h - _scale / 2);


                            keys[n].BringToFront();
                            n++;
                        }
                    }
                    #endregion vertical
                    break;
            }

            
            switch (_orientation)
            {
                case Orientation.Horizontal:
                    _totallength = tt;
                    break;
                case Orientation.Vertical:
                    _totallength = keys.Length * _scale;
                    break;
            }
        }

        private Color ChannelColor(int channel)
        {            
            switch (channel)
            {

                case 0:
                    return System.Drawing.Color.LightGreen; 

                case 1:                    
                    return System.Drawing.Color.CornflowerBlue; 


                case 2:
                    return System.Drawing.Color.LightGoldenrodYellow;
                    
                case 3:
                    return System.Drawing.Color.SlateBlue;
                    
                case 4:
                    return System.Drawing.Color.LightPink;
                    
                case 5:
                    return System.Drawing.Color.LightSalmon;
                    
                case 6:
                    return System.Drawing.Color.LightSeaGreen;
                    
                case 7:
                    return System.Drawing.Color.LightSteelBlue;
                    
                case 8:
                    return System.Drawing.Color.Plum;
                    
                case 10:
                    return System.Drawing.Color.Maroon;
                    
                case 11:
                    return System.Drawing.Color.Red;
                    
                case 12:
                    return System.Drawing.Color.Chocolate;
                    
                case 13:
                    return System.Drawing.Color.NavajoWhite;
                    
                case 14:
                    return System.Drawing.Color.Green;
                    
                default:
                    return System.Drawing.Color.LightGray;
                    
            }         
        }


        #endregion


        #region public functions

        public void Send(ChannelMessage message)
        {
            if(message.Command == ChannelCommand.NoteOn &&
                message.Data1 >= LowNoteID && message.Data1 <= HighNoteID)
            {
                if(InvokeRequired)
                {                    
                    BeginInvoke(noteOnCallback, message);
                }
                else
                {
                    noteOnCallback(message);
                }
            }
            else if(message.Command == ChannelCommand.NoteOff &&
                message.Data1 >= LowNoteID && message.Data1 <= HighNoteID)
            {
                if(InvokeRequired)
                {
                    BeginInvoke(noteOffCallback, message);
                }
                else
                {
                    noteOffCallback(message);
                }
            }
        }

        public void PressPianoKey(int noteID)
        {
            #region Require

            if(noteID < lowNoteID || noteID > highNoteID)
            {
                throw new ArgumentOutOfRangeException();
            }

            #endregion

            if (Orientation == Orientation.Horizontal)
                keys[noteID - lowNoteID].PressPianoKey();
            else            
                keys[highNoteID - noteID].PressPianoKey();            
        }

        /// <summary>
        /// Mouse is over a piano key
        /// </summary>
        /// <param name="noteID"></param>
        public void IsOverPianoKey(int noteID)
        {
            #region Require

            if (noteID < lowNoteID || noteID > highNoteID)
            {
                //throw new ArgumentOutOfRangeException();
                return;
            }

            #endregion

            if (Orientation == Orientation.Horizontal)
                keys[noteID - lowNoteID].IsOver = true;
            else
                keys[highNoteID - noteID].IsOver = true;
        }

        public void ReleasePianoKey(int noteID)
        {
            #region Require

            if(noteID < lowNoteID || noteID > highNoteID)
            {
                throw new ArgumentOutOfRangeException();
            }

            #endregion

            if (Orientation == Orientation.Horizontal)
                keys[noteID - lowNoteID].ReleasePianoKey();
            else
                keys[highNoteID - noteID].ReleasePianoKey();            
        }
               
        public void PressPianoKey(Keys k)
        {
            if(!Focused)
            {
                return;
            }

            if(keyTable.Contains(k))
            {
                int noteID = (int)keyTable[k] + 12 * octaveOffset;

                if(noteID >= LowNoteID && noteID <= HighNoteID)
                {
                    if (Orientation == Orientation.Horizontal)
                    {
                        if (!keys[noteID - lowNoteID].IsPianoKeyPressed)                        
                            keys[noteID - lowNoteID].PressPianoKey();                        
                    }
                    else
                    {
                        if (!keys[highNoteID - noteID].IsPianoKeyPressed)                       
                            keys[highNoteID - noteID].PressPianoKey();                        
                    }
                }
            }
            else
            {
                if(k == Keys.D0)
                {
                    octaveOffset = 0;
                }
                else if(k == Keys.D1)
                {
                    octaveOffset = 1;
                }
                else if(k == Keys.D2)
                {
                    octaveOffset = 2;
                }
                else if(k == Keys.D3)
                {
                    octaveOffset = 3;
                }
                else if(k == Keys.D4)
                {
                    octaveOffset = 4;
                }
                else if(k == Keys.D5)
                {
                    octaveOffset = 5;
                }
                else if(k == Keys.D6)
                {
                    octaveOffset = 6;
                }
                else if(k == Keys.D7)
                {
                    octaveOffset = 7;
                }
                else if(k == Keys.D8)
                {
                    octaveOffset = 8;
                }
                else if(k == Keys.D9)
                {
                    octaveOffset = 9;
                }
            }
        }

        public void ReleasePianoKey(Keys k)
        {
            #region Guard

            if(!keyTable.Contains(k))
            {
                return;
            }

            #endregion            

            int noteID = (int)keyTable[k] + 12 * octaveOffset;
            if (noteID >= LowNoteID && noteID <= HighNoteID)
            {
                if (Orientation == Orientation.Horizontal)               
                    keys[noteID - lowNoteID].ReleasePianoKey();                
                else
                    keys[highNoteID - noteID].ReleasePianoKey();
            }
        }

        public void Reset()
        {
            for (int i = 0; i < keys.Length; i++)
            {                
                keys[i].ReleasePianoKey();
                keys[i].NoteOnColor = NoteOnColor;
            }
        }

        /// <summary>
        /// Remove all is over painting
        /// </summary>
        public void ResetIsOver(int noteID)
        {
            
            foreach (PianoKey k in keys)
            {                
                if (k.NoteID != noteID)
                    k.IsOver = false;
            }
            

             /* 
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i].IsOver = false;
            }
             */
        }

        #endregion


        #region protected override

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                foreach(PianoKey key in keys)
                {
                    key.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        protected virtual void OnPianoKeyDown(PianoKeyEventArgs e)
        {
            PianoKeyDown?.Invoke(this, e);
        }

        protected virtual void OnPianoKeyUp(PianoKeyEventArgs e)
        {
            PianoKeyUp?.Invoke(this, e);
        }

        #endregion


     
    }
}

#region License

/* Copyright (c) 2016 Fabrice Lacharme
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
 * Fabrice Lacharme
 * Email: fabrice.lacharme@gmail.com
 */

#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace BallsControl
{
    #region Declare Delegate
    public delegate void PaintedEventHandler(PaintEventArgs e);    
    #endregion Declare Delegate


    public partial class Balls : UserControl, IMessageFilter
    {

        #region Create Delegate Reference
        //public event PaintedEventHandler Painted;

        #endregion Create Delegate Reference

        #region Move form without title bar
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        private readonly HashSet<Control> controlsToMove = new HashSet<Control>();
        #endregion

        private readonly int X0;
        private readonly int FIXED_POSITION;
        private readonly int STOP_POSITION;
        private int START_POSITION;
        private int _ballsnumber;
        private bool started = false;

        private readonly int DIAMETRE = 26;
        private float SPEED = 10;
     

        private int CurrentLyricsPos = 0;

        /// <summary>
        /// Control backcolor
        /// </summary>
        public Color BallsBackColor
        {
            set
            {
                picWnd.BackColor = value;
            }
            get
            {
                return picWnd.BackColor;
            }
        }

        private float _division;
        public float Division
        {
            get { return _division; }
            set {
                if (value > 0)
                {
                    _division = value;
                    SPEED = 192/_division;
                }            
            }
        }

        /// <summary>
        /// Balls number for animation
        /// </summary>
        public int BallsNumber
        {
            set
            {
                _ballsnumber = value;
            }
            get
            {
                return _ballsnumber;
            }
        }

        private AnimBall[] manyBall; // Liste de balles       
        private AnimBall fixedBall; // Balle fixe
        private readonly List<int> LyricsTimes; // Liste des timings
        
        /*
        private void OnPainted(PaintEventArgs e)
        {
            if (Painted != null)
            {
                // nothing
                picWnd_Painted(e);
            }
        }
        */

        public Balls()
        {
            InitializeComponent();

            
            this.SetStyle(
                  System.Windows.Forms.ControlStyles.UserPaint |
                  System.Windows.Forms.ControlStyles.AllPaintingInWmPaint |
                  System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer,
                  true);            

            #region Move form without title bar
            Application.AddMessageFilter(this);
            controlsToMove.Add(this);
            controlsToMove.Add(this.picWnd);
            #endregion

            FIXED_POSITION = DIAMETRE/2;
            X0 = 3*DIAMETRE;

            STOP_POSITION = -40;
            START_POSITION = picWnd.Width;

            LyricsTimes = new List<int>();

            // Invoke the delegate
            picWnd.Painted += new BallsControl.PaintedEventHandler(PicWnd_Painted);            
        }

        /// <summary>
        /// Move form without title bar
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_LBUTTONDOWN &&
                 controlsToMove.Contains(Control.FromHandle(m.HWnd)))
            {
                ReleaseCapture();
                SendMessage(this.ParentForm.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                return true;
            }
            return false;
        }

        private void PicWnd_Load(object sender, System.EventArgs e)
        {
            // nothing
           
        }

        /// <summary>
        /// Start balls animation
        /// </summary>
        public void Start()
        {
            if (LyricsTimes.Count >= 10 )
                BallsNumber = 20;
            else
                BallsNumber = LyricsTimes.Count;

            InitBalls(BallsNumber);
            fixedBall.DrawFixedBall();
        }

        /// <summary>
        /// Stop balls animation
        /// </summary>
        public void Stop()
        {
            for (int i = 0; i < BallsNumber; i++)
            {                
                manyBall[i].MoveBall(STOP_POSITION);
            }
            
            picWnd.Invalidate();   
        }

        /// <summary>
        /// Load internal list of lyrics times
        /// </summary>
        /// <param name="LyricsT"></param>
        public void LoadTimes(List<int> LyricsT)
        {
            LyricsTimes.Clear();
            for (int i = 0; i < LyricsT.Count; i++)
            {             
               LyricsTimes.Add(LyricsT[i]);             
            }
        }
        
        /// <summary>
        /// Light on fixed ball
        /// </summary>
        public void LightFixedBall()
        {
            //if (fixedBall != null)
            //    fixedBall.BallColor = LIGHTCOLOR;
        }

        /// <summary>
        /// light off fixed ball
        /// </summary>
        public void UnlightFixedBall()
        {
            //if (fixedBall != null)
            //    fixedBall.BallColor = FIXCOLOR;
        }

        /// <summary>
        /// Move balls according to lyrics times
        /// </summary>
        /// <param name="SongPosition"></param>
        /// <param name="CurrentLyricsPos"></param>
        public void MoveBallsToLyrics(int SongPosition, int textpos)
        {
            // 21 balls: 1 fix, 20 moving to the fixed one            
            int LyricPosition;
            int delta;
            int idLyric; // = 0;

            CurrentLyricsPos = textpos;

            for (int j = 0; j < BallsNumber; j++)
            {
                // index of next lyric to sing
                idLyric = CurrentLyricsPos + j;

                if (idLyric < LyricsTimes.Count && idLyric >= 0)
                {
                    // total time for this lyric stored in lyrics array
                    LyricPosition = LyricsTimes[idLyric];

                    // substract lyric time to position of song
                    delta = X0 + (int)((LyricPosition - SongPosition) * SPEED);

                    // move ball to the result of soustraction + offset corresponding to the fixed ball
                    if (manyBall != null && j < manyBall.Length && manyBall[j] != null)
                        manyBall[j].MoveBall(delta);
                    else
                        break;

                    if (LyricPosition < SongPosition)
                    {
                        // Allume la balle fixe
                        //fixedBall.BallColor = LIGHTCOLOR;
                    }
                }
                else
                {
                    // Ball must stop ... song is finished
                    manyBall?[j].MoveBall(STOP_POSITION);
                }
            }

           
            //picWnd.Refresh();        
            picWnd.Invalidate();
        }

     

        /// <summary>
        /// Initalize list of balls
        /// </summary>
        private void InitBalls(int n)
        {
            started = true;
            
            Clear();                               

            manyBall = new AnimBall[n];

            fixedBall = new AnimBall(picWnd) {

                // Coordonnées départ (X est le point gauche haut du rectangle ellipse)
               X = FIXED_POSITION,

                //Coordonnées départ (Y est le point gauche haut du rectangle ellipse)
               Y = (picWnd.Height - DIAMETRE) / 2,

                //Ball speed
               Speed = 0,
            };

            
            for (int i = 0; i < n; i++)
            {
                manyBall[i] = new AnimBall(picWnd) {

                    // Coordonnées départ (X est le point gauche haut du rectangle ellipse)
                    X = START_POSITION,

                    //Coordonnées départ (Y est le point gauche haut du rectangle ellipse)
                    Y = (picWnd.Height - DIAMETRE) / 2,

                    //Ball speed
                    Speed = SPEED,
                };

                // Graphic optimization
                this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                this.SetStyle(ControlStyles.ResizeRedraw, true);
                this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                this.SetStyle(ControlStyles.UserPaint, true);                                         
                
            }
        }

        private void Clear()
        {
            fixedBall?.Delete(picWnd);

            if (manyBall != null)
            {
                for (int i = 0; i < BallsNumber; i++)
                {
                    if (i < manyBall.Length && manyBall[i] != null)
                        manyBall[i].Delete(picWnd);
                }
            }
        }
     

        private void PicWnd_Painted(PaintEventArgs e)
        {
            if (started)
            {
                // Antialiasing
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                //fixedBall.gDrawBall(e.Graphics);
                fixedBall.DrawFixedBall();

                // Balles mobile
                for (int i = 0; i < BallsNumber; i++)
                {                    
                    manyBall[i].DrawBalls();
                }
            }
        }

        
        private void PicWnd_Resize(object sender, EventArgs e)
        {
            if (picWnd.Width > 0)
                START_POSITION = picWnd.Width;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();

                for (int i = 0; i < BallsNumber; i++)
                {
                    manyBall[i].Dispose();
                }
                fixedBall?.Dispose();
                picWnd?.Dispose();

               
            }

            base.Dispose(disposing);
        }


    }
}

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
using System.Windows.Forms;

namespace BallsControl
{
    /// <summary>
    /// The aniBall class
    /// </summary>
    class AnimBall: PictureBox
    {
        PictureBox P;

        #region Private Variables 
        private int _totalballs;

        private int _x;
        private int _y;
        private int _speed;
        #endregion

        #region Properties

        public int X
        {
            set
            {
                _x = value;
            }
            get
            {
                return _x;
            }
        }
        public int Y
        {
            set
            {
                _y = value;
            }
            get
            {
                return _y;
            }
        }


        public int Speed
        {
            set
            {
                _speed = value;
            }
            get
            {
                return _speed;
            }
        }

        /// <summary>
        /// Total Balls for animation
        /// </summary>
        public int TotalBalls
        {
            set
            {
                _totalballs = value;
            }
            get
            {
                return _totalballs;
            }
        }
        #endregion

        public AnimBall(BallsWnd pbox)
        {
            //
            // Default Constructor
            //
            P = new PictureBox();
            P.Width = 26;
            P.Height = 26;
            
            pbox.Controls.Add(P);
        }


        public void gDrawBalls(BallsWnd pbox)
        {            
            P.Image = global::BallsControl.Properties.Resources.ball;
            P.Left = _x;
            P.Top = _y;           
            P.Show();

        }

        public void gDrawFixedBall(BallsWnd pbox)
        {         
            P.Image = global::BallsControl.Properties.Resources.ballfixed;
            P.Left = _x;
            P.Top = _y;            
            P.Show();

        }

        public void MoveBall( int pos)
        {
            // only LEFT direction                       
            if (pos > 13)
                _x = pos;
            else
                _x = -40;                
        }

        public void Delete(BallsWnd pbox)
        {
            if (P != null)
                pbox.Controls.Remove(P);
        }

    }
}

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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace VBarControl.NavButton
{
    /* A simple button used by the parent control "SideBarControl"
     * This button has a picture and a label
     * its backcolor change on MouseHover, MouseLeave and Click if the button is selectable
     * Purpose: simulate internet behaviour                 
     */
     
    public partial class NavButton : UserControl
    {
        System.Windows.Forms.ToolTip tooltipBtn;
        public new event EventHandler Click;


        #region properties
       
        private static Color _defaultbackcolor = System.Drawing.ColorTranslator.FromHtml("#1d1d1d");
        public Color defaultBackColor
        {
            get { return _defaultbackcolor; }
            set { _defaultbackcolor = value; }
        }      

        private Color _hovercolor = Color.Gray;
        public Color HoverColor
        {
            get { return _hovercolor; }
            set
            {
                _hovercolor = value;              
            }
        }

        private Color _selectedcolor = System.Drawing.ColorTranslator.FromHtml("#34495e");
        public Color SelectedColor
        {
            get { return _selectedcolor; }
            set
            {
                _selectedcolor = value;              
            }
        }

        private string _text = "button1";
        [Description("Text for this button"), Category("Data")]
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        [Localizable(true)]        
        public string BText {
            get { return _text; }
            set
            {
                _text = value;
                lblButton.Text = _text;                
            }
        }
        
        private string _tooltiptext = string.Empty; 
        [Description("Tooltip Text for this button"), Category("Data")]
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        [Localizable(true)]
        public string ToolTipText
        {
            get { return _tooltiptext; }
            set
            {
                _tooltiptext = value;
                tooltipBtn.SetToolTip(this, _tooltiptext);
               
            }
        }
        
        [Description("Image displayed in the picturebox"), Category("Design")]
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public Image Image
        {
            get { return picButton.Image; }
            set { picButton.Image = value; }
        }

        private bool _selected = false;
        public bool Selected
        {
            get { return _selected; }
            set
            {
                if (_selectable)
                {                    

                    if (value)
                    {
                        unselect();
                        _selected = true;

                        pnlButton.BackColor = _selectedcolor;
                        picButton.BackColor = _selectedcolor;
                    }
                    else
                    {
                        _selected = false;
                        pnlButton.BackColor = _defaultbackcolor;
                        picButton.BackColor = _defaultbackcolor;
                    }
                }
            }
        }

        private bool _selectable = true;
        [Description("Set if control can be selected or not"), Category("Behaviour")]
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public bool Selectable
        {
            get { return _selectable; }
            set { _selectable = value; }
        }

        #endregion

        //Constructor
        public NavButton()
        {
            InitializeComponent();
            lblButton.Text = _text;

            SetFontScheme();

            tooltipBtn = new System.Windows.Forms.ToolTip();                      
            _selected = false;
                      
            picButton.MouseHover += new EventHandler(navButton_MouseHover);
            picButton.Click += new EventHandler(navButton_Click);

            lblButton.MouseHover += new EventHandler(navButton_MouseHover);
            lblButton.Click += new EventHandler(navButton_Click);

            pnlButton.MouseLeave += new EventHandler(navButton_MouseLeave);
            pnlButton.MouseHover += new EventHandler(navButton_MouseHover);
            pnlButton.MouseUp += new MouseEventHandler(navButton_MouseUp);
            pnlButton.Click += new EventHandler(navButton_Click);

            // Accessibility : manage user size of font
            Microsoft.Win32.SystemEvents.UserPreferenceChanged += new Microsoft.Win32.UserPreferenceChangedEventHandler(this.UserPreferenceChanged);

        }

       


        #region Public functions

        public void removeHover()
        {
            if (!Selected)
            {
                pnlButton.BackColor = _defaultbackcolor;
                picButton.BackColor = _defaultbackcolor;
            }
        }

        #endregion


        #region Private Functions

        private void UserPreferenceChanged(object sender, Microsoft.Win32.UserPreferenceChangedEventArgs e)
        {
            SetFontScheme();
        }

        private void SetFontScheme()
        {            
            //lblButton.Font = SystemFonts.CaptionFont;

            lblButton.Font = new System.Drawing.Font("Segoe UI", SystemFonts.MenuFont.Size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            // Change height of resource ?
            //this.Height = (int)(60 * (SystemFonts.CaptionFont.Size / 8.25F));
            lblButton.Height = lblButton.Font.Height;
            this.Height += lblButton.Height - 13;

        }


        /// <summary>
        /// Unselect all buttons
        /// </summary>
        private void unselect()
        {            
            foreach (Control p in Parent.Controls)
            {
                if (p is NavButton)
                {
                    ((NavButton)p).Selected = false;
                }
            }
        }

        /// <summary>
        /// Remove hovercolor on all buttons
        /// </summary>
        private void removeHoverAllButtons()
        {
            foreach (Control p in Parent.Controls)
            {
                if (p is NavButton)
                {
                    ((NavButton)p).removeHover();
                }
            }
        }

        /// <summary>
        /// MouseClick raise event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void navButton_Click(object sender, EventArgs e)
        {            
            if (_selectable)
            {
                //unselect();
                picButton.BackColor = _selectedcolor;
                Selected = true;
            }
            Click?.Invoke(this, e);
        }

        /// <summary>
        /// Apply defaultbackcolor on MouseLeave event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void navButton_MouseLeave(object sender, EventArgs e)
        {
            tooltipBtn.Hide(this);

            if (!Selected)
                if (!pnlButton.ClientRectangle.Contains(PointToClient(MousePosition)))
                {
                    pnlButton.BackColor = _defaultbackcolor;
                    picButton.BackColor = _defaultbackcolor;
                }
        }

        /// <summary>
        /// Apply hover color on MouseOver event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void navButton_MouseHover(object sender, EventArgs e)
        {
            Point P = new Point();
            P.X = this.Left + this.Width / 2;
            P.Y = this.Top + this.Height;

            // Force all other buttons to go back default backcolor
            removeHoverAllButtons();

            tooltipBtn.Show(_tooltiptext, this, P);

            // Apply hovercolor only if not selected
            if (!Selected)
            {
                pnlButton.BackColor = _hovercolor;
                picButton.BackColor = _hovercolor;
            }
        }


        private void navButton_MouseUp(object sender, MouseEventArgs e)
        {
            if (!_selectable)
            {
                pnlButton.BackColor = _defaultbackcolor;
                picButton.BackColor = _defaultbackcolor;
            }

        }


        #endregion
      

    }
}

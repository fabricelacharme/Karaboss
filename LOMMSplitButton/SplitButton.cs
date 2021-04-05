using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

//Original: http://blogs.msdn.com/jfoscoding/articles/491523.aspx
//Wyatt's fixes: http://wyday.com/blog/csharp-splitbutton
namespace InstantUpdate.Controls
{
    public class SplitButton : Button
    {
        private PushButtonState _state;
        private const int PushButtonWidth = 14;
        private static int BorderSize = SystemInformation.Border3DSize.Width * 2;
        private bool skipNextOpen = false;
        private Rectangle dropDownRectangle = new Rectangle();
        private bool showSplit = false;

        private ContextMenuStrip m_SplitMenu = null;

        public SplitButton()
        {
            this.AutoSize = true;
        }

        #region Properties

        [Browsable(false)]
        public override ContextMenuStrip ContextMenuStrip
        {
            get
            {
                return m_SplitMenu;
            }
            set
            {
                m_SplitMenu = value;
            }
        }

        [DefaultValue(null)]
        public ContextMenuStrip SplitMenu
        {
            get
            {
                return m_SplitMenu;
            }
            set
            {
                m_SplitMenu = value;

                if (m_SplitMenu != null)
                {
                    m_SplitMenu.Closing -= new ToolStripDropDownClosingEventHandler(SplitMenu_Closing);
                    m_SplitMenu.Closing += new ToolStripDropDownClosingEventHandler(SplitMenu_Closing);
                    m_SplitMenu.Opening -= new CancelEventHandler(SplitMenu_Opening);
                    m_SplitMenu.Opening += new CancelEventHandler(SplitMenu_Opening);
                    ShowSplit = true;
                }
                else
                    ShowSplit = false;
            }
        }

        [DefaultValue(false)]
        public bool ShowSplit
        {
            set
            {
                if (value != showSplit)
                {
                    showSplit = value;
                    Invalidate();
                    if (this.Parent != null)
                    {
                        this.Parent.PerformLayout();
                    }
                }
            }
        }

        private PushButtonState State
        {
            get
            {
                return _state;
            }
            set
            {
                if (!_state.Equals(value))
                {
                    _state = value;
                    Invalidate();
                }
            }
        }

        #endregion Properties

        public override Size GetPreferredSize(Size proposedSize)
        {
            Size preferredSize = base.GetPreferredSize(proposedSize);

            if (showSplit && !string.IsNullOrEmpty(Text) && TextRenderer.MeasureText(Text, Font).Width + PushButtonWidth > preferredSize.Width)
            {
                return preferredSize + new Size(PushButtonWidth + BorderSize * 2, 0);
            }

            return preferredSize;
        }

        protected override bool IsInputKey(Keys keyData)
        {
            if (keyData.Equals(Keys.Down) && showSplit)
            {
                return true;
            }

            else
            {
                return base.IsInputKey(keyData);
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            if (!showSplit)
            {
                base.OnGotFocus(e);
                return;
            }

            if (!State.Equals(PushButtonState.Pressed) && !State.Equals(PushButtonState.Disabled))
            {
                State = PushButtonState.Default;
            }
        }

        protected override void OnKeyDown(KeyEventArgs kevent)
        {
            if (showSplit)
            {
                if (kevent.KeyCode.Equals(Keys.Down))
                {
                    ShowContextMenuStrip();
                }

                else if (kevent.KeyCode.Equals(Keys.Space) && kevent.Modifiers == Keys.None)
                {
                    State = PushButtonState.Pressed;
                }
            }

            base.OnKeyDown(kevent);
        }

        protected override void OnKeyUp(KeyEventArgs kevent)
        {
            if (kevent.KeyCode.Equals(Keys.Space))
            {
                if (Control.MouseButtons == MouseButtons.None)
                {
                    State = PushButtonState.Normal;
                }
            }
            else if (kevent.KeyCode.Equals(Keys.Apps))
            {
                if (Control.MouseButtons == MouseButtons.None)
                {
                    ShowContextMenuStrip();
                }
            }

            base.OnKeyUp(kevent);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            if (Enabled)
                State = PushButtonState.Normal;
            else
                State = PushButtonState.Disabled;

            base.OnEnabledChanged(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            if (!showSplit)
            {
                base.OnLostFocus(e);
                return;
            }

            if (!State.Equals(PushButtonState.Pressed) && !State.Equals(PushButtonState.Disabled))
            {
                State = PushButtonState.Normal;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!showSplit)
            {
                base.OnMouseDown(e);
                return;
            }

            if (dropDownRectangle.Contains(e.Location) && !m_SplitMenu.Visible && e.Button == MouseButtons.Left)
            {
                ShowContextMenuStrip();
            }
            else
            {
                State = PushButtonState.Pressed;
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            if (!showSplit)
            {
                base.OnMouseEnter(e);
                return;
            }

            if (!State.Equals(PushButtonState.Pressed) && !State.Equals(PushButtonState.Disabled))
            {
                State = PushButtonState.Hot;
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (!showSplit)
            {
                base.OnMouseLeave(e);
                return;
            }

            if (!State.Equals(PushButtonState.Pressed) && !State.Equals(PushButtonState.Disabled))
            {
                if (Focused)
                {
                    State = PushButtonState.Default;
                }

                else
                {
                    State = PushButtonState.Normal;
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            if (!showSplit)
            {
                base.OnMouseUp(mevent);
                return;
            }

            if (mevent.Button == MouseButtons.Right)
            {
                ShowContextMenuStrip();
            }
            else if (m_SplitMenu == null || !m_SplitMenu.Visible)
            {
                SetButtonDrawState();

                if (Bounds.Contains(Parent.PointToClient(Cursor.Position)) && !dropDownRectangle.Contains(mevent.Location))
                {
                    OnClick(new EventArgs());
                }
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            if (!showSplit)
            {
                return;
            }

            Graphics g = pevent.Graphics;
            Rectangle bounds = this.ClientRectangle;

            // draw the button background as according to the current state.
            if (State != PushButtonState.Pressed && IsDefault && !Application.RenderWithVisualStyles)
            {
                Rectangle backgroundBounds = bounds;
                backgroundBounds.Inflate(-1, -1);
                ButtonRenderer.DrawButton(g, backgroundBounds, State);

                // button renderer doesnt draw the black frame when themes are off =(
                g.DrawRectangle(SystemPens.WindowFrame, 0, 0, bounds.Width - 1, bounds.Height - 1);
            }
            else
            {
                ButtonRenderer.DrawButton(g, bounds, State);
            }

            // calculate the current dropdown rectangle.
            dropDownRectangle = new Rectangle(bounds.Right - PushButtonWidth, 0, PushButtonWidth, bounds.Height);

            int internalBorder = BorderSize;
            Rectangle focusRect =
                new Rectangle(internalBorder - 1,
                              internalBorder - 1,
                              bounds.Width - dropDownRectangle.Width - internalBorder,
                              bounds.Height - (internalBorder * 2) + 2);

            bool drawSplitLine = (State == PushButtonState.Hot || State == PushButtonState.Pressed || !Application.RenderWithVisualStyles);


            if (RightToLeft == RightToLeft.Yes)
            {
                dropDownRectangle.X = bounds.Left + 1;
                focusRect.X = dropDownRectangle.Right;

                if (drawSplitLine)
                {
                    // draw two lines at the edge of the dropdown button
                    g.DrawLine(SystemPens.ButtonShadow, bounds.Left + PushButtonWidth, BorderSize, bounds.Left + PushButtonWidth, bounds.Bottom - BorderSize);
                    g.DrawLine(SystemPens.ButtonFace, bounds.Left + PushButtonWidth + 1, BorderSize, bounds.Left + PushButtonWidth + 1, bounds.Bottom - BorderSize);
                }
            }
            else
            {
                if (drawSplitLine)
                {
                    // draw two lines at the edge of the dropdown button
                    g.DrawLine(SystemPens.ButtonShadow, bounds.Right - PushButtonWidth, BorderSize, bounds.Right - PushButtonWidth, bounds.Bottom - BorderSize);
                    g.DrawLine(SystemPens.ButtonFace, bounds.Right - PushButtonWidth - 1, BorderSize, bounds.Right - PushButtonWidth - 1, bounds.Bottom - BorderSize);
                }
            }

            // Draw an arrow in the correct location
            PaintArrow(g, dropDownRectangle);

            // Figure out how to draw the text
            TextFormatFlags formatFlags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;

            // If we dont' use mnemonic, set formatFlag to NoPrefix as this will show ampersand.
            if (!UseMnemonic)
            {
                formatFlags = formatFlags | TextFormatFlags.NoPrefix;
            }
            else if (!ShowKeyboardCues)
            {
                formatFlags = formatFlags | TextFormatFlags.HidePrefix;
            }


            if (!string.IsNullOrEmpty(this.Text))
            {
                if (Enabled)
                    TextRenderer.DrawText(g, Text, Font, focusRect, SystemColors.ControlText, formatFlags);
                else
                    TextRenderer.DrawText(g, Text, Font, focusRect, SystemColors.GrayText, formatFlags);
            }

            // draw the focus rectangle.
            if (State != PushButtonState.Pressed && Focused && ShowFocusCues)
            {
                ControlPaint.DrawFocusRectangle(g, focusRect);
            }
        }

        private void PaintArrow(Graphics g, Rectangle dropDownRect)
        {
            Point middle = new Point(Convert.ToInt32(dropDownRect.Left + dropDownRect.Width / 2), Convert.ToInt32(dropDownRect.Top + dropDownRect.Height / 2));

            //if the width is odd - favor pushing it over one pixel right.
            middle.X += (dropDownRect.Width % 2);

            Point[] arrow = new Point[] { new Point(middle.X - 2, middle.Y - 1), new Point(middle.X + 3, middle.Y - 1), new Point(middle.X, middle.Y + 2) };

            if (Enabled)
                g.FillPolygon(SystemBrushes.ControlText, arrow);
            else
                g.FillPolygon(SystemBrushes.ButtonShadow, arrow);
        }

        private void ShowContextMenuStrip()
        {
            if (skipNextOpen)
            {
                // we were called because we're closing the context menu strip
                // when clicking the dropdown button.
                skipNextOpen = false;
                return;
            }

            State = PushButtonState.Pressed;

            if (m_SplitMenu != null)
            {
                m_SplitMenu.Show(this, new Point(0, Height), ToolStripDropDownDirection.BelowRight);
            }
        }

        void SplitMenu_Opening(object sender, CancelEventArgs e)
        {
            if (m_SplitMenu.Visible)
                e.Cancel = true;
        }

        void SplitMenu_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            try
            {
                SetButtonDrawState();
            }
            catch (Exception) { }


            if (e.CloseReason == ToolStripDropDownCloseReason.AppClicked)
            {
                skipNextOpen = (dropDownRectangle.Contains(this.PointToClient(Cursor.Position))) && Control.MouseButtons == MouseButtons.Left;
            }
        }

        private void SetButtonDrawState()
        {
            if (Bounds.Contains(Parent.PointToClient(Cursor.Position)))
            {
                State = PushButtonState.Hot;
            }
            else if (Focused)
            {
                State = PushButtonState.Default;
            }
            else if (!Enabled)
            {
                State = PushButtonState.Disabled;
            }
            else
            {
                State = PushButtonState.Normal;
            }
        }
    }
}
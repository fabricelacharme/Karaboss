#region License

/* Copyright (c) 2009 Kent Andersson
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
 * Kent Andersson
 * https://www.codeproject.com/Articles/16082/Analog-and-LED-Meter?msg=1724182#xx1724182xx
 */

#endregion
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace VU_MeterLibrary
{
    internal class VuMeterDesigner : ControlDesigner
        
    {
        protected override void PostFilterProperties(System.Collections.IDictionary properties)
        {
            properties.Remove("AccessibleDescription");
            properties.Remove("AccessibleName");
            properties.Remove("AccessibleRole");
            properties.Remove("BackgroundImage");
            //properties.Remove("BackgroundImageLayout");
            properties.Remove("BorderStyle");
            properties.Remove("Cursor");
            properties.Remove("RightToLeft");
            properties.Remove("UseWaitCursor");
            properties.Remove("AllowDrop");
            properties.Remove("AutoValidate");
            properties.Remove("ContextMenuStrip");
            properties.Remove("Enabled");
            properties.Remove("ImeMode");
            //properties.Remove("TabIndex"); // Don't remove this one or the designer will break
            properties.Remove("TabStop");
            //properties.Remove("Visible");
            properties.Remove("ApplicationSettings");
            properties.Remove("DataBindings");
            properties.Remove("Tag");
            properties.Remove("GenerateMember");
            properties.Remove("Locked");
            //properties.Remove("Modifiers");
            properties.Remove("CausesValidation");
            properties.Remove("Anchor");
            properties.Remove("AutoSize");
            properties.Remove("AutoSizeMode");
            //properties.Remove("Location");
            properties.Remove("Dock");
            properties.Remove("Margin");
            properties.Remove("MaximumSize");
            properties.Remove("MinimumSize");
            properties.Remove("Padding");
            //properties.Remove("Size");
            properties.Remove("DockPadding");
            properties.Remove("AutoScrollMargin");
            properties.Remove("AutoScrollMinSize");
            properties.Remove("AutoScroll");
            properties.Remove("ForeColor");
            //properties.Remove("BackColor");
            properties.Remove("Text");
            //properties.Remove("Font");
        }
    }

    public enum MeterScale { Analog, Log10 };

    [Designer(typeof(VuMeterDesigner))]
    [System.Drawing.ToolboxBitmap(@"Vu_Meter.bmp")]

    public partial class VuMeter : UserControl
    {
        int CurrentLevel;
        int PeakLevel;
        int LedCount1 = 6, LedCount2 = 6, LedCount3 = 4;
        int calcValue, calcPeak;
        Color LedColorOn1 = Color.LimeGreen, LedColorOn2 = Color.Yellow, LedColorOn3 = Color.Red;
        Color LedColorOff1 = Color.DarkGreen, LedColorOff2 = Color.Olive, LedColorOff3 = Color.Maroon;
        Color BorderColor = Color.DimGray;
        Color DialBackColor = Color.White;
        Color DialTextLow = Color.Red;
        Color DialTextNeutral = Color.DarkGreen;
        Color DialTextHigh = Color.Black;
        Color DialNeedle = Color.Black;
        Color DialPeak = Color.Red;
        int Min = 0, Max = 65535;
        int PeakHoldTime = 1000;
        bool ShowPeak = true;
        bool Vertical = false;
        bool MeterAnalog = false;
        string MeterText = "VU";
        string[] DialText = { "-40", "-20", "-10", "-5", "0", "+6" };
        bool ShowDialText = false;
        bool AnalogDialRegionOnly = false;
        bool UseLedLightInAnalog = false;
        bool ShowLedPeakInAnalog = false;

        private double DegLow = Math.PI * 0.8, DegHigh = Math.PI * 1.2;

        protected Timer timer1;
        Size Led = new Size(6, 14);
        int LedSpacing = 3;

        [Category("Analog Meter")]
        [Description("Show textvalues in dial")]
        public bool UseLedLight
        {
            get
            {
                return UseLedLightInAnalog;
            }
            set
            {
                UseLedLightInAnalog = value;
                this.Invalidate();
            }
        }

        [Category("Analog Meter")]
        [Description("Show textvalues in dial")]
        public bool ShowLedPeak
        {
            get
            {
                return ShowLedPeakInAnalog;
            }
            set
            {
                ShowLedPeakInAnalog = value;
                this.Invalidate();
            }
        }

        [Category("Analog Meter")]
        [Description("Analog meter layout")]
        public bool AnalogMeter
        {
            get
            {
                return MeterAnalog;
            }
            set
            {
                if (value & !MeterAnalog) this.Size = new Size(100, 80);
                MeterAnalog = value;
                CalcSize();
                this.Invalidate();
            }
        }

        [Category("Analog Meter")]
        [Description("Text (max 10 letters)")]
        public string VuText
        {
            get
            {
                return MeterText;
            }
            set
            {
                if (value.Length < 11) MeterText = value;
                this.Invalidate();
            }
        }

        [Category("Analog Meter")]
        [Description("Text in dial")]
        public string[] TextInDial
        {
            get
            {
                return DialText;
            }
            set
            {
                DialText = value;
                this.Invalidate();
            }
        }

        [Category("Analog Meter")]
        [Description("Show textvalues in dial")]
        public bool ShowTextInDial
        {
            get
            {
                return ShowDialText;
            }
            set
            {
                ShowDialText = value;
                this.Invalidate();
            }
        }

        [Category("Analog Meter")]
        [Description("Only show the Analog Dial Panel (Sets BackColor to DialBackColor so antialias won't look bad)")]
        public bool ShowDialOnly
        {
            get
            {
                return AnalogDialRegionOnly;
            }
            set
            {
                AnalogDialRegionOnly = value;
                if (AnalogDialRegionOnly) this.BackColor = DialBackColor;
                this.Invalidate();
            }
        }

        [Category("Analog Meter")]
        [Description("Color on dial background")]
        public Color DialBackground
        {
            get
            {
                return DialBackColor;
            }
            set
            {
                DialBackColor = value;
                this.Invalidate();
            }
        }

        [Category("Analog Meter")]
        [Description("Color on Value < 0")]
        public Color DialTextNegative
        {
            get
            {
                return DialTextLow;
            }
            set
            {
                DialTextLow = value;
                this.Invalidate();
            }
        }

        [Category("Analog Meter")]
        [Description("Color on Value = 0")]
        public Color DialTextZero
        {
            get
            {
                return DialTextNeutral;
            }
            set
            {
                DialTextNeutral = value;
                this.Invalidate();
            }
        }

        [Category("Analog Meter")]
        [Description("Color on Value > 0")]
        public Color DialTextPositive
        {
            get
            {
                return DialTextHigh;
            }
            set
            {
                DialTextHigh = value;
                this.Invalidate();
            }
        }

        [Category("Analog Meter")]
        [Description("Color on needle")]
        public Color NeedleColor
        {
            get
            {
                return DialNeedle;
            }
            set
            {
                DialNeedle = value;
                this.Invalidate();
            }
        }

        [Category("Analog Meter")]
        [Description("Color on Peak needle")]
        public Color PeakNeedleColor
        {
            get
            {
                return DialPeak;
            }
            set
            {
                DialPeak = value;
                this.Invalidate();
            }
        }


        private MeterScale FormType = MeterScale.Log10;

        [Category("VU Meter")]
        [Description("Display value in analog or logarithmic scale")]
        public MeterScale MeterScale
        {
            get
            {
                return FormType;
            }
            set
            {
                FormType = value;
                this.Invalidate();
            }
        }

        [Category("VU Meter")]
        [Description("Led size (1 to 72 pixels)")]
        public Size LedSize
        {
            get
            {
                return Led;
            }
            set
            {
                if (value.Height < 1) Led.Height = 1;
                else if (value.Height > 72) Led.Height = 72;
                else Led.Height = value.Height;

                if (value.Width < 1) Led.Width = 1;
                else if (value.Width > 72) Led.Width = 72;
                else Led.Width = value.Width;

                CalcSize();
                this.Invalidate();
            }
        }

        [Category("VU Meter")]
        [Description("Led spacing (0 to 72 pixels)")]
        public int LedSpace
        {
            get
            {
                return LedSpacing;
            }
            set
            {
                if (value < 0) LedSpacing = 0;
                else if (value > 72) LedSpacing = 72;
                else LedSpacing = value;
                CalcSize();
                this.Invalidate();
            }
        }

        [Category("VU Meter")]
        [Description("Led bar is vertical")]
        public bool VerticalBar
        {
            get
            {
                return Vertical;
            }
            set
            {
                Vertical = value;
                CalcSize();
                this.Invalidate();
            }
        }

        [Category("VU Meter")]
        [Description("Max value from total LedCount to 65535")]
        public int LevelMax
        {
            get
            {
                return Max;
            }
            set
            {
                if (value < (Led1Count + Led2Count + Led3Count)) Max = (Led1Count + Led2Count + Led3Count);
                else if (value > 65535) Max = 65535;
                else Max = value;

                this.Invalidate();
            }
        }

        [Category("VU Meter")]
        [Description("The level shown (between Min and Max)")]
        public int Level
        {
            get
            {
                return CurrentLevel;
            }

            set
            {
                if (value != CurrentLevel)
                {
                    if (value < Min) CurrentLevel = Min;
                    else if (value > Max) CurrentLevel = Max;
                    else CurrentLevel = value;

                    if ((CurrentLevel > PeakLevel) & (ShowPeak | ShowLedPeakInAnalog))
                    {
                        PeakLevel = CurrentLevel;
                        timer1.Stop();
                        timer1.Start();
                    }
                    this.Invalidate();
                }
            }
        }

        [Category("VU Meter")]
        [Description("How many mS to hold peak indicator (50 to 10000mS)")]
        public int Peakms
        {
            get
            {
                return PeakHoldTime;
            }
            set
            {
                if (value < 50) PeakHoldTime = 50;
                else if (value > 10000) PeakHoldTime = 10000;
                else PeakHoldTime = value;
                timer1.Interval = PeakHoldTime;
                this.Invalidate();
            }
        }

        [Category("VU Meter")]
        [Description("Use peak indicator")]
        public bool PeakHold
        {
            get
            {
                return ShowPeak;
            }
            set
            {
                ShowPeak = value;
                this.Invalidate();
            }
        }

        [Category("VU Meter")]
        [Description("Number of Leds in first area (0 to 32, default 6) Total Led count must be at least 1")]
        public int Led1Count
        {
            get
            {
                return LedCount1;
            }

            set
            {
                if (value < 0) LedCount1 = 0;
                else if (value > 32) LedCount1 = 32;
                else LedCount1 = value;
                if ((LedCount1 + LedCount2 + LedCount3) < 1) LedCount1 = 1;
                CalcSize();
                this.Invalidate();
            }
        }

        [Category("VU Meter")]
        [Description("Number of Leds in middle area (0 to 32, default 6) Total Led count must be at least 1")]
        public int Led2Count
        {
            get
            {
                return LedCount2;
            }

            set
            {
                if (value < 0) LedCount2 = 0;
                else if (value > 32) LedCount2 = 32;
                else LedCount2 = value;
                if ((LedCount1 + LedCount2 + LedCount3) < 1) LedCount2 = 1;
                CalcSize();
                this.Invalidate();
            }
        }

        [Category("VU Meter")]
        [Description("Number of Leds in last area (0 to 32, default 4) Total Led count must be at least 1")]
        public int Led3Count
        {
            get
            {
                return LedCount3;
            }

            set
            {
                if (value < 0) LedCount3 = 0;
                else if (value > 32) LedCount3 = 32;
                else LedCount3 = value;
                if ((LedCount1 + LedCount2 + LedCount3) < 1) LedCount3 = 1;
                CalcSize();
                this.Invalidate();
            }
        }

        [Category("VU Meter - Colors")]
        [Description("Color of Leds in first area (Led on)")]
        public Color Led1ColorOn
        {
            get
            {
                return LedColorOn1;
            }
            set
            {
                LedColorOn1 = value;
                this.Invalidate();
            }
        }

        [Category("VU Meter - Colors")]
        [Description("Color of Leds in middle area (Led on)")]
        public Color Led2ColorOn
        {
            get
            {
                return LedColorOn2;
            }
            set
            {
                LedColorOn2 = value;
                this.Invalidate();
            }
        }

        [Category("VU Meter - Colors")]
        [Description("Color of Leds in last area (Led on)")]
        public Color Led3ColorOn
        {
            get
            {
                return LedColorOn3;
            }
            set
            {
                LedColorOn3 = value;
                this.Invalidate();
            }
        }

        [Category("VU Meter - Colors")]
        [Description("Color of Leds in first area (Led off)")]
        public Color Led1ColorOff
        {
            get
            {
                return LedColorOff1;
            }
            set
            {
                LedColorOff1 = value;
                this.Invalidate();
            }
        }

        [Category("VU Meter - Colors")]
        [Description("Color of Leds in middle area (Led off)")]
        public Color Led2ColorOff
        {
            get
            {
                return LedColorOff2;
            }
            set
            {
                LedColorOff2 = value;
                this.Invalidate();
            }
        }

        [Category("VU Meter - Colors")]
        [Description("Color of Leds in last area (Led off)")]
        public Color Led3ColorOff
        {
            get
            {
                return LedColorOff3;
            }
            set
            {
                LedColorOff3 = value;
                this.Invalidate();
            }
        }

        public VuMeter()
        {
            this.Name = "VuMeter";
            CalcSize();
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            // FAB
            // Control not selectable
            this.SetStyle(ControlStyles.Selectable, false);

            timer1 = new Timer();
            timer1.Interval = PeakHoldTime;
            timer1.Enabled = false;
            timer1.Tick += new EventHandler(timer1_Tick);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (MeterAnalog)
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                DrawAnalogBorder(g);
                DrawAnalogDial(g);
            }
            else
            {
                Graphics g = e.Graphics;
                DrawBorder(g);
                DrawLeds(g);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            if (MeterAnalog)
            {
                base.OnResize(e);
            }
            CalcSize();
            base.OnResize(e);
            this.Invalidate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            PeakLevel = CurrentLevel;
            this.Invalidate();
            timer1.Start();
        }

        private void CalcSize()
        {
            if (MeterAnalog)
            {
                this.Size = new Size(this.Width, (int)(this.Width * 0.8));
            }
            else if (Vertical)
            {
                this.Size = new Size(Led.Width + LedSpacing * 2, (LedCount1 + LedCount2 + LedCount3) * (Led.Height + LedSpacing) + LedSpacing);
            }
            else
            {
                this.Size = new Size((LedCount1 + LedCount2 + LedCount3) * (Led.Width + LedSpacing) + LedSpacing, Led.Height + LedSpacing * 2);
            }
        }


        private void DrawAnalogDial(Graphics g)
        {
            //Add code to draw "LED:s" by color in Dial (Analog and LED)
            if (UseLedLightInAnalog)
            {
                if (FormType == MeterScale.Log10)
                {
                    calcValue = (int)(Math.Log10((double)CurrentLevel / (Max / 10) + 1) * (LedCount1 + LedCount2 + LedCount3));
                    if (ShowLedPeakInAnalog) calcPeak = (int)(Math.Log10((double)PeakLevel / (Max / 10) + 1) * (LedCount1 + LedCount2 + LedCount3));
                }

                if (FormType == MeterScale.Analog)
                {
                    calcValue = (int)(((double)CurrentLevel / Max) * (LedCount1 + LedCount2 + LedCount3) + 0.5);
                    if (ShowLedPeakInAnalog) calcPeak = (int)(((double)PeakLevel / Max) * (LedCount1 + LedCount2 + LedCount3) + 0.5);
                }

                Double DegStep = (DegHigh - DegLow) / (LedCount1 + LedCount2 + LedCount3 - 1);
                double i;
                double SinI, CosI;
                Pen scalePen;
                int lc = 0;
                int LedRadiusStart = (int)(this.Width * 0.6);
                if (!ShowTextInDial) LedRadiusStart = (int)(this.Width * 0.65);
                for (i = DegHigh; i > DegLow - DegStep / 2; i = i - DegStep)
                {
                    if ((lc < calcValue) | (((lc + 1) == calcPeak) & ShowLedPeakInAnalog))
                    {
                        scalePen = new Pen(Led3ColorOn, Led.Width);
                        if (lc < LedCount1 + LedCount2) scalePen = new Pen(Led2ColorOn, Led.Width);
                        if (lc < LedCount1) scalePen = new Pen(Led1ColorOn, Led.Width);
                    }
                    else
                    {
                        scalePen = new Pen(Led3ColorOff, Led.Width);
                        if (lc < LedCount1 + LedCount2) scalePen = new Pen(Led2ColorOff, Led.Width);
                        if (lc < LedCount1) scalePen = new Pen(Led1ColorOff, Led.Width);
                    }

                    lc++;
                    SinI = Math.Sin(i);
                    CosI = Math.Cos(i);
                    g.DrawLine(scalePen, (int)((LedRadiusStart - Led.Height) * SinI + this.Width / 2),
                        (int)((LedRadiusStart - Led.Height) * CosI + this.Height * 0.9),
                        (int)(LedRadiusStart * SinI + this.Width / 2), (int)(LedRadiusStart * CosI + this.Height * 0.9));
                }
            }
            //End of code addition

            if (FormType == MeterScale.Log10)
            {
                calcValue = (int)(Math.Log10((double)CurrentLevel / (Max / 10) + 1) * Max);
                if (ShowPeak) calcPeak = (int)(Math.Log10((double)PeakLevel / (Max / 10) + 1) * Max);
            }

            if (FormType == MeterScale.Analog)
            {
                calcValue = CurrentLevel;
                if (ShowPeak) calcPeak = PeakLevel;
            }
            int DialRadiusLow = (int)(this.Width * 0.3f), DialRadiusHigh = (int)(this.Width * 0.65f);

            Pen DialPen = new Pen(DialNeedle, this.Width * 0.01f);
            double DialPos;
            if (calcValue > 0) DialPos = DegHigh - (((double)calcValue / Max) * (DegHigh - DegLow));
            else DialPos = DegHigh;
            Double SinD = Math.Sin(DialPos), CosD = Math.Cos(DialPos);
            g.DrawLine(DialPen, (int)(DialRadiusLow * SinD + this.Width * 0.5),
                (int)(DialRadiusLow * CosD + this.Height * 0.9),
                (int)(DialRadiusHigh * SinD + this.Width * 0.5),
                (int)(DialRadiusHigh * CosD + this.Height * 0.9));

            if (ShowPeak)
            {
                Pen PeakPen = new Pen(DialPeak, this.Width * 0.01f);
                if (calcPeak > 0) DialPos = DegHigh - (((double)calcPeak / Max) * (DegHigh - DegLow));
                else DialPos = DegHigh;
                Double SinP = Math.Sin(DialPos), CosP = Math.Cos(DialPos);
                g.DrawLine(PeakPen, (int)(DialRadiusLow * SinP + this.Width * 0.5),
                    (int)(DialRadiusLow * CosP + this.Height * 0.9),
                    (int)(DialRadiusHigh * SinP + this.Width * 0.5),
                    (int)(DialRadiusHigh * CosP + this.Height * 0.9));
            }
            DialPen.Dispose();
        }

        private void DrawLeds(Graphics g)
        {
            if (FormType == MeterScale.Log10)
            {
                calcValue = (int)(Math.Log10((double)CurrentLevel / (Max / 10) + 1) * (LedCount1 + LedCount2 + LedCount3));
                if (ShowPeak) calcPeak = (int)(Math.Log10((double)PeakLevel / (Max / 10) + 1) * (LedCount1 + LedCount2 + LedCount3));
            }

            if (FormType == MeterScale.Analog)
            {
                calcValue = (int)(((double)CurrentLevel / Max) * (LedCount1 + LedCount2 + LedCount3) + 0.5);
                if (ShowPeak) calcPeak = (int)(((double)PeakLevel / Max) * (LedCount1 + LedCount2 + LedCount3) + 0.5);
            }


            for (int i = 0; i < (LedCount1 + LedCount2 + LedCount3); i++)
            {

                if (Vertical)
                {
                    Rectangle current = new Rectangle(this.ClientRectangle.X + LedSpacing,
                        this.ClientRectangle.Height - ((i + 1) * (Led.Height + LedSpacing)),
                        Led.Width, Led.Height);

                    if ((i < calcValue) | (((i + 1) == calcPeak) & ShowPeak))
                    {
                        if (i < LedCount1)
                        {
                            g.FillRectangle(new SolidBrush(LedColorOn1), current);
                        }
                        else if (i < (LedCount1 + LedCount2))
                        {
                            g.FillRectangle(new SolidBrush(LedColorOn2), current);
                        }
                        else
                        {
                            g.FillRectangle(new SolidBrush(LedColorOn3), current);
                        }
                    }
                    else
                    {
                        if (i < LedCount1)
                        {
                            g.FillRectangle(new SolidBrush(LedColorOff1), current);
                        }
                        else if (i < (LedCount1 + LedCount2))
                        {
                            g.FillRectangle(new SolidBrush(LedColorOff2), current);
                        }
                        else
                        {
                            g.FillRectangle(new SolidBrush(LedColorOff3), current);
                        }
                    }

                }
                else
                {
                    Rectangle current = new Rectangle(this.ClientRectangle.X + (i * (Led.Width + LedSpacing)) + LedSpacing,
                        this.ClientRectangle.Y + LedSpacing, Led.Width, Led.Height);

                    if ((i) < calcValue | (((i + 1) == calcPeak) & ShowPeak))
                    {
                        if (i < LedCount1)
                        {
                            g.FillRectangle(new SolidBrush(LedColorOn1), current);
                        }
                        else if (i < (LedCount1 + LedCount2))
                        {
                            g.FillRectangle(new SolidBrush(LedColorOn2), current);
                        }
                        else
                        {
                            g.FillRectangle(new SolidBrush(LedColorOn3), current);
                        }
                    }
                    else
                    {
                        if (i < LedCount1)
                        {
                            g.FillRectangle(new SolidBrush(LedColorOff1), current);
                        }
                        else if (i < (LedCount1 + LedCount2))
                        {
                            g.FillRectangle(new SolidBrush(LedColorOff2), current);
                        }
                        else
                        {
                            g.FillRectangle(new SolidBrush(LedColorOff3), current);
                        }
                    }

                }

            }
        }

        private void DrawBorder(Graphics g)
        {
            Rectangle Border = new Rectangle(this.ClientRectangle.X, this.ClientRectangle.Y, this.ClientRectangle.Width, this.ClientRectangle.Height);
            g.FillRectangle(new SolidBrush(this.BackColor), Border);
        }

        private void DrawAnalogBorder(Graphics g)
        {
            if (!AnalogDialRegionOnly) g.FillRectangle(new SolidBrush(this.BackColor), 0, 0, this.Width, this.Height);

            double DegStep = (DegHigh * 1.05 - DegLow / 1.05) / 19;
            double i = DegHigh * 1.05;
            double SinI, CosI;

            PointF[] curvePoints = new PointF[40];
            for (int cp = 0; cp < 20; cp++)
            {
                i = i - DegStep;
                SinI = Math.Sin(i);
                CosI = Math.Cos(i);
                curvePoints[cp] = new PointF((float)(SinI * this.Width * 0.7 + this.Width / 2), (float)(CosI * this.Width * 0.7 + this.Height * 0.9));
                curvePoints[38 - cp] = new PointF((float)(SinI * this.Width * 0.3 + this.Width / 2), (float)(CosI * this.Width * 0.3 + this.Height * 0.9));
            }
            curvePoints[39] = curvePoints[0];
            System.Drawing.Drawing2D.GraphicsPath dialPath = new System.Drawing.Drawing2D.GraphicsPath();
            if (AnalogDialRegionOnly) dialPath.AddPolygon(curvePoints);
            else dialPath.AddRectangle(new Rectangle(0, 0, this.Width, this.Height));
            this.Region = new System.Drawing.Region(dialPath);
            g.FillPolygon(new SolidBrush(DialBackColor), curvePoints);

            // Test moving this block
            if (!UseLedLightInAnalog)
            {
                DegStep = (DegHigh - DegLow) / (LedCount1 + LedCount2 + LedCount3 - 1);
                int lc = 0;
                int LedRadiusStart = (int)(this.Width * 0.6);
                if (!ShowTextInDial) LedRadiusStart = (int)(this.Width * 0.65);
                for (i = DegHigh; i > DegLow - DegStep / 2; i = i - DegStep)
                {
                    //Graphics scale = g.Graphics;
                    Pen scalePen = new Pen(Led3ColorOn, Led.Width);
                    if (lc < LedCount1 + LedCount2) scalePen = new Pen(Led2ColorOn, Led.Width);
                    if (lc < LedCount1) scalePen = new Pen(Led1ColorOn, Led.Width);
                    lc++;
                    SinI = Math.Sin(i);
                    CosI = Math.Cos(i);
                    g.DrawLine(scalePen, (int)((LedRadiusStart - Led.Height) * SinI + this.Width / 2),
                        (int)((LedRadiusStart - Led.Height) * CosI + this.Height * 0.9),
                        (int)(LedRadiusStart * SinI + this.Width / 2), (int)(LedRadiusStart * CosI + this.Height * 0.9));
                    scalePen.Dispose();
                }
            }
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            float MeterFontSize = this.Font.SizeInPoints;
            if (this.Width > 0) MeterFontSize = MeterFontSize * (float)(this.Width / 100f);
            if (MeterFontSize < 4) MeterFontSize = 4;
            if (MeterFontSize > 72) MeterFontSize = 72;
            Font MeterFont = new Font(this.Font.FontFamily, MeterFontSize);
            g.DrawString(this.MeterText, MeterFont, new SolidBrush(this.ForeColor), this.Width / 2, this.Height * 0.43f, format);

            if (ShowDialText)
            {
                double DialTextStep = (DegHigh - DegLow) / (DialText.Length - 1);
                int dt = 0;
                MeterFontSize = MeterFontSize * 0.6f;
                int TextRadiusStart = (int)(this.Width * 0.64);
                for (i = DegHigh; i > DegLow - DialTextStep / 2; i = i - DialTextStep)
                {
                    //Graphics scale = g.Graphics;
                    Brush dtColor = new SolidBrush(DialTextHigh);
                    StringFormat dtformat = new StringFormat();
                    dtformat.Alignment = StringAlignment.Center;
                    dtformat.LineAlignment = StringAlignment.Center;
                    try
                    {
                        if (int.Parse(DialText[dt]) < 0) dtColor = new SolidBrush(DialTextLow);
                        if (int.Parse(DialText[dt]) == 0) dtColor = new SolidBrush(DialTextNeutral);
                    }
                    catch
                    {
                        dtColor = new SolidBrush(DialTextHigh);
                    }
                    Font dtfont = new Font(this.Font.FontFamily, MeterFontSize);
                    SinI = Math.Sin(i);
                    CosI = Math.Cos(i);
                    g.DrawString(DialText[dt++], dtfont, dtColor, (int)(TextRadiusStart * SinI + this.Width / 2), (int)(TextRadiusStart * CosI + this.Height * 0.9), dtformat);

                }
            }


        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);

        }
    }
}

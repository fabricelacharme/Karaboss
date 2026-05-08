using GradientApp;
using kar;
using MusicXml.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TagLib.Mpeg4;

namespace Karaboss.Mp3
{
    public partial class frmTest : Form
    {

        #region Colors

        Color _BgColor;

        Color _ActiveColor;
        Color _HighlightColor;
        Color _InactiveColor;

        Color _ActiveBorderColor;
        Color _InactiveBorderColor;
       
        Color _ActiveInstrumentalColor;

        #endregion Colors


        #region Text transform

        int _nbLyricsLines = 4;
        private float[] LinesLengths;
        int _lineHeight;
        int _linesHeight;
        string _biggestLine;
        float _AverageWidth;


        int _borderthick = 1;
        bool _bTextBackGround = false;

        #endregion Text transform


        #region Karaoke lyrics

        private kLyrics _kLyricsOrg;
        private kLyrics _kLyrics;
        public kLyrics KLyrics
        {
            get { return _kLyrics; }
            set
            {
                if (value == null) return;
                if (value.Lines == null) return;
                if (value.Lines.Count == 0) return;
                _kLyrics = value;
                _kLyricsOrg = _kLyrics.Clone();
                if (_kLyrics != null && _kLyrics.Lines.Count > 0)
                    Init();
            }
        }

        #endregion Karaoke lyrics


        #region Font

        Font m_font;
        private float emSize; // Size of the font
        private StringFormat sf;
        Font _karaokeFont;

        #endregion Font


        public frmTest()
        {
            InitializeComponent();
        }

        private void Init()
        {
            ResizeBox();

            //sf = new StringFormat(StringFormat.GenericTypographic); // { FormatFlags = StringFormatFlags.MeasureTrailingSpaces };

            // Fonts
            emSize = this.Font.Size;
            _karaokeFont = new Font("Arial Black", emSize, FontStyle.Regular, GraphicsUnit.Pixel);

            
            // Colors
            _BgColor = Parse("#FCA903");

            _ActiveColor = Parse("#00ACFF");
            _HighlightColor = Parse("#FFFF00");
            _InactiveColor = Parse("#FFFFFF");
            
            _ActiveBorderColor = Parse("#010101");
            _InactiveBorderColor = Parse("#8000FF");

            _ActiveInstrumentalColor = Parse("#808080");


            // Lyrics

            // Store all lines lengths
            LinesLengths = new float[_kLyrics.Lines.Count];
            //_biggestLine = GetBiggestLine();
            _AverageWidth = GetAverageWidth(picBox, _kLyrics, _karaokeFont.Size);
            
            AdjustText(picBox, _AverageWidth, _nbLyricsLines);


        }

        public void SetLyrics(kLyrics lyrics)
        {
            KLyrics = lyrics;
        }



        #region Paint
        private void picBox_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            DrawPicText(e);
        }

        private void pnlBox_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            DrawPnlText(e);
        }


        private void DrawPicText(PaintEventArgs e)
        {
            float lineSpacing = 1.17f;

            float w;            
            int y2 = 0;

            string FileName = "Chuck Berry - Johnny Be Good.kar";

            // Draw file name if required
            y2 = (int)MeasureStringHeight(picBox,FileName, 0.038f * _karaokeFont.Size);            
            DrawFileName(e, "Chuck Berry - Johnny Be Good", y2, 0.33f * _karaokeFont.Size);

            // Draw lines starting from this position
            y2 = (int)(0.36f * _lineHeight);

            int i;

            i = 2;            
            w = MeasureString(picBox, _kLyrics.Lines[i].ToString(), _karaokeFont.Size);
            DrawInactiveLineWithBorders(picBox, e, i, y2, w);
            y2 += _lineHeight;

            i = 3;
            w = MeasureString(picBox, _kLyrics.Lines[i].ToString(), _karaokeFont.Size);
            DrawInactiveLineWithBorders(picBox, e, i, y2, w);
            y2 += (int)(lineSpacing * _lineHeight);

            i = 0;
            w = MeasureString(picBox, _kLyrics.Lines[i].ToString(), _karaokeFont.Size);
            DrawInactiveLineWithBorders(picBox, e, i, y2, w);
            y2 += _lineHeight;

            i = 1;
            w = MeasureString(picBox, _kLyrics.Lines[i].ToString(), _karaokeFont.Size);
            DrawInactiveLineWithBorders(picBox, e, i, y2, w);


        }

        private void DrawPnlText(PaintEventArgs e) 
        {
            float w = 0;
            int y2 = 0;
            for (int i = 0; i < _kLyrics.Lines.Count; i++)
            {
                y2 += _lineHeight;
                //DrawInactiveLineWithBorders(pnlBox, e, i, y2, w);
            }
        }


        private void DrawFileName(PaintEventArgs e, string FileName, int y2, float femSize)
        {
            sf = null;
            int x0 = 0;
            Color BorderColor = _ActiveBorderColor;
            Color FillColor = _InactiveColor;
            Pen penBorder = new Pen(BorderColor, 1);
            var path = new GraphicsPath();

            // Mease FileName
            float w = MeasureString(picBox, FileName, femSize);
            float scale = (0.33f * picBox.Width) / w;            
            
            
            // Position to write FileName
            //x0 = (int)(0.66f * picBox.Width);
                        
            path.AddString(FileName, _karaokeFont.FontFamily, (int)_karaokeFont.Style, femSize, new Point(x0, y2), sf);

            
            e.Graphics.ScaleTransform(scale, 1);

            // Draw the text            
            e.Graphics.FillPath(new SolidBrush(FillColor), path);

            // Outline the text            
            e.Graphics.DrawPath(penBorder, path);

            e.Graphics.ResetTransform();
        }

        private void DrawInactiveLineWithBorders(Control pBox, PaintEventArgs e, int lineIndex, int y2, float w, bool IsActive = false)
        {
            #region Declarations

            Color BorderColor = _InactiveBorderColor;
            Color FillColor = _InactiveColor;

            if (IsActive)
            {
                BorderColor = _ActiveBorderColor;
                FillColor = _ActiveColor;
            }

            // Create a graphical path
            var path = new GraphicsPath();
            int x0 = 0;
            Pen penBorder = new Pen(BorderColor, _borderthick); // pen for border color
            int Wbg;
            RectangleF Rbg;
            string s;

            #endregion Declarations

            if (lineIndex < _kLyrics.Lines.Count())
            {
                s = _kLyrics.Lines[lineIndex].ToString();
                //x0 = HCenterText(pBox, s);     // Center text horizontally
                //x0 = (int)((pBox.Width - w) / 2);

                #region Background of text  

                if (_bTextBackGround)
                {
                    Wbg = (int)(1.04 * LinesLengths[lineIndex]);
                    // Black background to make text more visible
                    Rbg = new RectangleF((int)(0.94 * x0), (int)(1.04 * y2), Wbg, _lineHeight);
                    // background
                    e.Graphics.FillRectangle(new SolidBrush(Color.Black), Rbg);
                }

                #endregion Background of text

                // Add lines of lyrics to the Graphics path
                path.AddString(s, _karaokeFont.FontFamily, (int)_karaokeFont.Style, _karaokeFont.Size, new Point(x0, y2), sf);

                
                // ************************ Reduce the size of the text
                float scale = (pBox.Width - 50) / w;               
                if (w > 0)
                    e.Graphics.ScaleTransform(scale, 1);


                // Draw the text            
                e.Graphics.FillPath(new SolidBrush(FillColor), path);

                // Outline the text
                if (_borderthick > 0)
                    e.Graphics.DrawPath(penBorder, path);

                // ************************  Reset ScaleTransform
                e.Graphics.ResetTransform();

            }

            #region Clean up resources

            path.Dispose();
            penBorder.Dispose();

            #endregion Clean up resources
        }


        #endregion Paint


        #region Measure

        private int HCenterText(Control pBox, string s)
        {
            int res = -(int)_karaokeFont.Size / 2 + (pBox.ClientSize.Width - (int)MeasureString(pBox, s, _karaokeFont.Size)) / 2;
            return res > 0 ? res : 0;
        }

        private float MeasureString(Control pBox, string fragment, float femSize)
        {
            float ret = 0;
            if (fragment != "")
            {

                using (Graphics g = pBox.CreateGraphics())
                {
                    g.TextRenderingHint = TextRenderingHint.AntiAlias;
                    g.PageUnit = GraphicsUnit.Pixel;

                    //StringFormat sf = new StringFormat(StringFormat.GenericTypographic) { FormatFlags = StringFormatFlags.MeasureTrailingSpaces };

                    m_font = new Font(_karaokeFont.FontFamily, femSize, FontStyle.Regular, GraphicsUnit.Pixel);
                    SizeF sz = g.MeasureString(fragment, m_font, new Point(0, 0), sf);
                    ret = sz.Width;
                    g.Dispose();
                }
            }
            return ret;
        }
              

        
        private float GetAverageWidth(Control pBox, kLyrics kls, float femSize)
        {
            float L = 0;
            
            if (pBox == null) return 0;
            if (kls.Lines.Count == 0) return 0;
            
            // Calculation of the average length of the lines
            for (int i = 0; i < kls.Lines.Count(); i++)
            {
                L += MeasureString(pBox, kls.Lines[i].ToString(), femSize);
            }
            return L / kls.Lines.Count;
        }


        /// <summary>
        /// Ajust font size to fit NbLines in height and the average line lenght in width
        /// Bigger lines will be shrinked and smaller lines not changed
        /// </summary>
        /// <param name="pBox"></param>
        /// <param name="S"></param>
        /// <param name="NbLines"></param>
        private void AdjustText(Control pBox, float averageWidth,  int NbLines) 
        { 
            if (pBox == null) return;

            string S = "!/(123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

            Graphics g = pBox.CreateGraphics();
            float femsize;
            long inisize = (long)_karaokeFont.Size;
            femsize = g.DpiY * inisize / 72;

            // Try to fit inside 90% of client Height
            long comp = (long)(0.90 * pBox.ClientSize.Height);
            
            float textHeight = MeasureStringHeight(pBox, S, femsize);
            textHeight = textHeight * NbLines;

            if (textHeight > comp)
            {
                do
                {
                    inisize--;
                    if (inisize > 0)
                    {
                        femsize = g.DpiY * inisize / 72;
                        textHeight = NbLines * MeasureStringHeight(pBox, S, femsize);

                    }
                } while (textHeight > comp && inisize > 0);
            }
            else
            {
                do
                {
                    inisize++;                         
                    femsize = g.DpiY * inisize / 72;
                    textHeight = NbLines * MeasureStringHeight(pBox, S, femsize);
                } while (textHeight < comp);
            }

            // ------------------------------
            // Ajustement in width with AverageWidth
            // ------------------------------            
            long compWidth = (long)(0.95 * pBox.ClientSize.Width);
            float textWidth = averageWidth;

            if (averageWidth > compWidth)
            {
                do
                {
                    inisize--;
                    if (inisize > 0)
                    {
                        femsize = g.DpiX * inisize / 72;
                        textWidth = GetAverageWidth(pBox, _kLyrics, femsize);                        
                    }
                } while (textWidth > compWidth && inisize > 0);
            }


            if (inisize > 0)
            {
                emSize = g.DpiX * inisize / 72;
                _karaokeFont = new Font(_karaokeFont.FontFamily, emSize, FontStyle.Regular, GraphicsUnit.Pixel);

                // Vertical distance between lines          1.6 is
                // https://pimpmytype.com/line-length-line-height/ they say 1.6 is the best 
                _lineHeight = (int)(1.55 * emSize);
                // Height of the full song
                _linesHeight = _nbLyricsLines * _lineHeight;


                // Update horizontal measure of lines                
                for (int i = 0; i < _kLyrics.Lines.Count; i++)
                {
                    LinesLengths[i] = MeasureString(pBox, _kLyrics.Lines[i].ToString(), _karaokeFont.Size);
                }

            }
            g.Dispose();
        }
       


        private float MeasureStringHeight(Control pBox, string line, float femSize)
        {
            float ret = 0;

            if (line != "")
            {
                using (Graphics g = pBox.CreateGraphics())
                {

                    if (femSize > 0)
                        m_font = new Font(_karaokeFont.FontFamily, femSize, FontStyle.Regular, GraphicsUnit.Pixel);

                    SizeF sz = g.MeasureString(line, m_font, new Point(0, 0), sf);
                    ret = sz.Height;

                    g.Dispose();
                }
            }
            return ret;
        }

        
        private float MeasureLine(Control pBox, int curline, float femSize)
        {
            return MeasureString(pBox, _kLyrics.Lines[curline].ToString(), femSize);
        }


        #endregion Measure


        #region Color Functions

        /// <summary>
        /// Check text representing a color
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Color Parse(string input)
        {
            input = input.Trim();
            string strColorRegex = @"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$";
            Regex re = new Regex(strColorRegex);
            if (re.IsMatch(input))
            {
                return ColorTranslator.FromHtml(input);
            }

            Color named = Color.FromName(input);
            if (named.IsKnownColor || named.IsNamedColor)
            {
                return named;
            }
            throw new ArgumentException($"Unsupported color value: {input}", nameof(input));
        }


        #endregion Color functions


        #region form load close

        private void frmTest_Resize(object sender, EventArgs e)
        {

            //ResizeBox();

            AdjustText(picBox, _AverageWidth, _nbLyricsLines);
            picBox.Invalidate();
            //pnlBox.Invalidate();

        }

        private void ResizeBox()
        {
            picBox.Top = 0;
            picBox.Left = 0;
            picBox.Width = this.ClientSize.Width;
            picBox.Height = this.ClientSize.Height / 2;

            pnlBox.Left = 0;
            pnlBox.Top = picBox.Height;
            pnlBox.Width = this.ClientSize.Width;
            pnlBox.Height = this.ClientSize.Height / 2;
        }

        #endregion form load close
    }
}

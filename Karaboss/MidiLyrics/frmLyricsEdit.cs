#region License

/* Copyright (c) 2025 Fabrice Lacharme
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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sanford.Multimedia.Midi;
using System.IO;
using System.Text.RegularExpressions;
using Karaboss.Resources.Localization;
using Karaboss.Lrc.SharedFramework;
using Karaboss.MidiLyrics;
using Karaboss.Utilities;
using Karaboss.Mp3.Mp3Lyrics;


namespace Karaboss
{
    public partial class frmLyricsEdit : Form
    {

        /* Lyrics edition form
         * 
         * 0    - Ticks    Number of ticks
         * 1    - Time     Time in sec
         * 2    - Type     text, paragraph, linefeed
         * <3   - Chord    Chord name (optional if option bShowChord is true)>     
         * 3(4) - Note     Note value
         * 4(5) - Text     text        
         * 
         * Line break is '/' - cr
         * Paragraph is '\'  - par
         * Syllabe separator is '*'
         */

        #region declarations

        private MidiLyricsMgmt _myLyricsMgmt;        
        private List<plLyric> localplLyrics;

        #region Internal lyrics separators

        private readonly string _InternalSepLines = "¼";
        private readonly string _InternalSepParagraphs = "½";

        #endregion


        #region dgView Colors

        Color dgViewHeaderBackColor = Color.FromArgb(43, 87, 151);
        Color dgViewHeaderForeColor = Color.White;
        Color dgViewSelectionBackColor = Color.FromArgb(45, 137, 239);

        Color SepLinesColor = Color.FromArgb(239, 244, 255);
        Color SepParagrColor = Color.LightGray;

        Font dgViewHeaderFont = new Font("Arial", 12F, FontStyle.Regular);
        Font dgViewCellsFont = new Font("Arial", 16F, GraphicsUnit.Pixel);
        #endregion dgViewColors


        #region External lyrics separators

        private readonly string m_SepLine = "/";
        private readonly string m_SepParagraph = "\\";
        
        #endregion

        private bool bfilemodified = false;
        private Sequence sequence1;        
        private Track melodyTrack;        
        private ContextMenuStrip dgContextMenu;        

        enum LyricFormats
        {
            Text = 0,
            Lyric = 1
        }
        public bool bEditChords { get ; set; }
        
        int COL_TICKS;  // = 0;
        int COL_TIME;   // = 1;
        int COL_TYPE;   // = 2;
        int COL_CHORD;  // = 3 if chords
        int COL_NOTE;   // = 3 or 4 if chords;
        int COL_TEXT;   // = 4 or 5 if chords;
        

        LyricFormats TextLyricFormat;        

        int melodytracknum = 0;
        int lyricstracknum = 0;

        private string MIDIfileName = string.Empty;

        // Midifile characteristics
        //private double _duration = 0;  // en secondes
        private int _totalTicks = 0;        
        private double _ppqn;
        private double _division;
        private double _durationPercent;
        private int _tempo;
        private int _measurelen;


        // txtResult, BtnFontPlus
        private Font _lyricseditfont;
        private float _fontSize = 8.25f;

        private int _LrcMillisecondsDigits = 2;

        private readonly List<string> lsInstruments = Sanford.Multimedia.Midi.MidiFile.LoadInstruments();

        #endregion declarations

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="plLyrics"></param>
        /// <param name="myLyricsMgmt"></param>
        /// <param name="fileName"></param>
        /// <param name="EditChords"></param>
        public frmLyricsEdit(Sequence sequence, List<plLyric> plLyrics, MidiLyricsMgmt myLyricsMgmt, string fileName, bool EditChords = false)
        {
            InitializeComponent();

            bEditChords = EditChords;

            LoadOptions();
            
            //_lyricseditfont = new Font("Segoe UI", _fontSize, FontStyle.Regular, GraphicsUnit.Point);

            MIDIfileName = fileName;
            sequence1 = sequence;
            UpdateMidiTimes();
            
            // Load tempos map
            TempoUtilities.lstTempos = TempoUtilities.GetAllTempoChanges(sequence1);            

            // Load saved line and paragraph separators
            m_SepLine = Karaclass.m_SepLine;
            m_SepParagraph = Karaclass.m_SepParagraph;

            // Load list of tracks
            LoadTracks(sequence1);

            InitTxtResult();

            InitGridView();

            #region inform 2 types of lyrics are present
            // If both formats of lyrics are available, dispay button allowing to switch
            _myLyricsMgmt = myLyricsMgmt;
            int l = _myLyricsMgmt.lstpllyrics[0].Count;
            int t = _myLyricsMgmt.lstpllyrics[1].Count;
            btnDisplayOtherLyrics.Visible = (l > 0 && t > 0 );
                      
            if (l > 0 && t > 0)
            {
                // "Two types of lyrics format are available in this file: LYRIC and TEXT"
                string tx = Karaboss.Resources.Localization.Strings.TwoTypesOfLyrics;                
                
                if (myLyricsMgmt.LyricType == LyricTypes.Text)
                {
                    // "The {0} format has been choosen by Karaboss because it contains more lyrics ({1} lyrics)"
                    tx += string.Format("\n\n" + Karaboss.Resources.Localization.Strings.FormatLyricsChoosen, "TEXT", t);
                    // "You can change to {0} format ({1} lyrics) by pressing the button 'Display Others'."
                    tx += string.Format("\n\n" + Karaboss.Resources.Localization.Strings.ChangeFormatLyrics, "LYRIC", l);
                }
                else if (myLyricsMgmt.LyricType == LyricTypes.Lyric)
                {
                    tx += string.Format("\n\n" + Karaboss.Resources.Localization.Strings.FormatLyricsChoosen, "LYRIC", l);                                        
                    tx += string.Format("\n\n" + Karaboss.Resources.Localization.Strings.ChangeFormatLyrics, "TEXT", t);
                }

                MessageBox.Show(tx, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            #endregion inform 2 types of lyrics are present

            Cursor.Current = Cursors.WaitCursor;

            // Track containing the melody
            melodytracknum = myLyricsMgmt.MelodyTrackNum;
            if (melodytracknum != -1)
                melodyTrack = sequence1.tracks[melodytracknum];

            // Track containing the lyrics
            lyricstracknum = myLyricsMgmt.LyricsTrackNum;


            DisplaySelectedTrack();

            if (myLyricsMgmt.LyricType == LyricTypes.Text)
            {
                TextLyricFormat = LyricFormats.Text;
                optFormatText.Checked = true;
            }
            else
            {
                TextLyricFormat = LyricFormats.Lyric;
                optFormatLyrics.Checked = true;
            }

            // If first time = no lyrics
            if (plLyrics.Count == 0)
                LoadTrackGuide();
            else
            {
                localplLyrics = plLyrics;

                // populate cells with existing Lyrics or notes
                PopulateDataGridView(plLyrics);
                // populate viewer
                PopulateTextBox(plLyrics);
            }

            // Adapt height of cells to duration between syllabes
            HeightsToDurations();

            string displayName = string.Empty;
            if (MIDIfileName != null)
                displayName = Path.GetFileName(MIDIfileName);
            SetTitle(displayName);

            dgView.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;

            // Color separators
            ColorSepRows();

            DisplayTags();

            ResizeMe();

            Cursor.Current = Cursors.Default;
        }
                    

        #region buttons

        /// <summary>
        /// Button : save new lyrics
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnView_Click(object sender, EventArgs e)
        {
            //Load modification into local list of lyrics
            localplLyrics = LoadModifiedLyrics(true);

            // Display new lyrics in frmPlayer
            _myLyricsMgmt.ChordsOriginatedFrom = MidiLyricsMgmt.ChordsOrigins.Lyrics;
            ReplaceLyrics(localplLyrics);
        }


        /// <summary>
        /// Insert a LineFeed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnInsert_Click(object sender, EventArgs e)
        {
            InsertSepLine("cr");     
        }

        /// <summary>
        /// Insert a Paragraph
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInsertParagraph_Click(object sender, EventArgs e)
        {
            InsertSepLine("par");
      
        }

        /// <summary>
        /// Insert Linefeed or Paragraph
        /// </summary>
        /// <param name="sep"></param>
        private void InsertSepLine(string sep)
        {
            if (dgView.CurrentRow == null)
                return;

            int Row = dgView.CurrentRow.Index;
            int plTicksOn = 0;
            string plRealTime = "00:00.00";
            string plElement; // = string.Empty;

            if (dgView.Rows[Row].Cells[COL_TICKS].Value!= null && IsNumeric(dgView.Rows[Row].Cells[COL_TICKS].Value.ToString()))
            {
                plTicksOn = Convert.ToInt32(dgView.Rows[Row].Cells[COL_TICKS].Value);
                plRealTime = Utilities.LyricsUtilities.TicksToTime(plTicksOn, _division);
            }

            if (sep == "cr")
                plElement = m_SepLine;
            else
                plElement = m_SepParagraph;

            // time, type, note, text, text
            if (!bEditChords)
                dgView.Rows.Insert(Row, plTicksOn, plRealTime, sep, "", plElement);
            else
                dgView.Rows.Insert(Row, plTicksOn, plRealTime, sep, "", "", plElement);

            //Load modification into local list of lyrics
            localplLyrics = LoadModifiedLyrics();
            PopulateTextBox(localplLyrics);

            // Modify height of cells according to durations
            HeightsToDurations();

            // Color separators
            ColorSepRows();

            // File was modified
            FileModified();
        }


        /// <summary>
        /// Insert a Text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnInsertText_Click(object sender, EventArgs e)
        {
            InsertTextLine();
        }

        /// <summary>
        /// Insert text line
        /// </summary>
        private void InsertTextLine()
        {
            if (dgView.CurrentRow == null)
                return;

            int Row = dgView.CurrentRow.Index;
            int plTicksOn = 0;
            string plRealTime = "00:00.00";
            int pNote = 0;
            string pElement; 
            string pReplace; 

            if (dgView.Rows[Row].Cells[COL_TICKS].Value != null && IsNumeric(dgView.Rows[Row].Cells[COL_TICKS].Value.ToString()))
            {
                plTicksOn = Convert.ToInt32(dgView.Rows[Row].Cells[COL_TICKS].Value);
                plRealTime = Utilities.LyricsUtilities.TicksToTime(plTicksOn, _division);
            }

            pElement = "text";

            // Column Replace
            if (dgView.Rows[Row].Cells[COL_TEXT].Value != null)
                pReplace = dgView.Rows[Row].Cells[COL_TEXT].Value.ToString();
            else
                pReplace = "text";

            if (!bEditChords)
                dgView.Rows.Insert(Row, plTicksOn, plRealTime, "text", pNote, pElement, pReplace);
            else
                dgView.Rows.Insert(Row, plTicksOn, plRealTime, "text", "", pNote, pElement, pReplace);

            HeightsToDurations();

            // File was modified
            FileModified();
        }


        /// <summary>
        /// Add a space left
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSpaceLeft_Click(object sender, EventArgs e)
        {
            int Row = dgView.CurrentRow.Index;
            if (dgView.Rows[Row].Cells[COL_TEXT].Value != null)
            {
                dgView.Rows[Row].Cells[COL_TEXT].Value = "_" + dgView.Rows[Row].Cells[COL_TEXT].Value;
            }
        }

        /// <summary>
        /// Add a space right
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSpaceRight_Click(object sender, EventArgs e)
        {
            int Row = dgView.CurrentRow.Index;
            if (dgView.Rows[Row].Cells[COL_TEXT].Value != null)
            {
                dgView.Rows[Row].Cells[COL_TEXT].Value = dgView.Rows[Row].Cells[COL_TEXT].Value + "_";
                
                // File was modified
                FileModified();
            }
        }
        
        /// <summary>
        /// Delete a single row
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            DeleteLine();
        }

        private void DeleteLine()
        {
            try
            {
                int row = dgView.CurrentRow.Index;
                dgView.Rows.RemoveAt(row);

                //Load modification into local list of lyrics
                localplLyrics = LoadModifiedLyrics();
                PopulateTextBox(localplLyrics);

                // File was modified
                FileModified();
            }
            catch (Exception Ex)
            {
                string message = "Error : " + Ex.Message;
                MessageBox.Show(message, "Error deleting line",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Save as
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSave_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;

            //Load modification into local list of lyrics            
            localplLyrics = LoadModifiedLyrics(true);

            // Display new lyrics in frmPlayer
            _myLyricsMgmt.ChordsOriginatedFrom = MidiLyricsMgmt.ChordsOrigins.Lyrics;
            ReplaceLyrics(localplLyrics);

            // Save file
            SaveFileProc();
            
            Cursor.Current = Cursors.Default;

            Focus();

        }


        /// <summary>
        /// Erase all lyrics
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteAllLyrics_Click(object sender, EventArgs e)
        {
            string tx = Karaboss.Resources.Localization.Strings.DeleteAllLyrics;            
            if (MessageBox.Show(tx, "Karaboss", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                frmPlayer frmPlayer = Utilities.FormUtilities.GetForm<frmPlayer>();
                frmPlayer.DeleteAllLyrics();

                localplLyrics = new List<plLyric>();

                InitGridView();
                txtResult.Text = string.Empty;

                cbSelectTrack.SelectedIndex = 0;

                // File was modified
                FileModified();
            }

        }

        /// <summary>
        /// Play from current time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPlay_Click(object sender, EventArgs e)
        {
            // Set cursor as hourglass
            Cursor.Current = Cursors.WaitCursor;

            //Load modification into local list of lyrics
            localplLyrics = LoadModifiedLyrics();

            // Display new lyrics in frmPlayer
            _myLyricsMgmt.ChordsOriginatedFrom = MidiLyricsMgmt.ChordsOrigins.Lyrics;
            ReplaceLyrics(localplLyrics);

            // Set cursor as default
            Cursor.Current = Cursors.Default;

            if (dgView.CurrentRow != null)
            {
                int Row = dgView.CurrentRow.Index;
                if (dgView.Rows[Row].Cells[COL_TICKS].Value != null && IsNumeric(dgView.Rows[Row].Cells[COL_TICKS].Value.ToString()))
                {
                    int pTime = Convert.ToInt32(dgView.Rows[Row].Cells[COL_TICKS].Value);
                    if (Application.OpenForms.OfType<frmPlayer>().Count() > 0)
                    {
                        frmPlayer frmPlayer = Utilities.FormUtilities.GetForm<frmPlayer>();
                        frmPlayer.FirstPlaySong(pTime);
                    }
                }
            }
        }

      

        private void BtnFontPlus_Click(object sender, EventArgs e)
        {
            _fontSize++;
            _lyricseditfont = new Font(_lyricseditfont.FontFamily, _fontSize);
            
            txtResult.Font = _lyricseditfont;
        }

        private void BtnFontMoins_Click(object sender, EventArgs e)
        {
            if (_fontSize > 5)
            {
                _fontSize--;
                _lyricseditfont = new Font(_lyricseditfont.FontFamily, _fontSize);
                txtResult.Font = _lyricseditfont;
            }
            
        }



        #endregion buttons

             
        #region context menu

        private void DgView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                dgContextMenu = new ContextMenuStrip();
                dgContextMenu.Items.Clear();

                
                // Insert Text line
                ToolStripMenuItem menuInsertTextLine = new ToolStripMenuItem(Strings.InsertNewLine);
                dgContextMenu.Items.Add(menuInsertTextLine);
                menuInsertTextLine.Click += new System.EventHandler(this.MnuInsertTextLine_Click);

                // Insert LineFeed
                ToolStripMenuItem menuInsertLineBreak = new ToolStripMenuItem(Strings.InsertLineBreak);
                dgContextMenu.Items.Add(menuInsertLineBreak);
                menuInsertLineBreak.Click += new System.EventHandler(this.MnuInsertLineBreak_Click);


                // Insert paragraph
                ToolStripMenuItem menuInsertParagraph = new ToolStripMenuItem(Strings.InsertNewParagraph);
                dgContextMenu.Items.Add(menuInsertParagraph);
                menuInsertParagraph.Click += new System.EventHandler(this.MnuInsertParagraph_Click);

                // Delete line
                ToolStripMenuItem menuDeleteLine = new ToolStripMenuItem(Strings.DeleteLine);
                dgContextMenu.Items.Add(menuDeleteLine);
                menuDeleteLine.Click += new System.EventHandler(this.MnuDeleteLine_Click);


                ToolStripSeparator menusep1 = new ToolStripSeparator();
                dgContextMenu.Items.Add(menusep1);

                // Décaler vers le haut
                ToolStripMenuItem menuOffsetUp = new ToolStripMenuItem(Strings.OffsetUp);
                dgContextMenu.Items.Add(menuOffsetUp);
                menuOffsetUp.Click += new System.EventHandler(this.MnuOffsetUp_Click);

                // Décaler vers le bas
                ToolStripMenuItem menuOffsetDown = new ToolStripMenuItem(Strings.OffsetDown);
                dgContextMenu.Items.Add(menuOffsetDown);
                menuOffsetDown.Click += new System.EventHandler(this.MnuOffsetDown_Click);

                ToolStripSeparator menusep2 = new ToolStripSeparator();
                dgContextMenu.Items.Add(menusep2);


                // Copier
                ToolStripMenuItem menuCopy = new ToolStripMenuItem(Strings.Copy);
                dgContextMenu.Items.Add(menuCopy);
                menuCopy.Click += new System.EventHandler(this.MnuCopy_Click);

                // Coller
                ToolStripMenuItem menuPaste = new ToolStripMenuItem(Strings.Paste);
                dgContextMenu.Items.Add(menuPaste);
                menuPaste.Click += new System.EventHandler(this.MnuPaste_Click);


                // Display menu on the listview
                dgContextMenu.Show(dgView, dgView.PointToClient(Cursor.Position));
         
            }

        }

        private void MnuInsertLineBreak_Click(object sender, EventArgs e)
        {
            InsertSepLine("cr");
        }

        /// <summary>
        /// Insert paragraph separator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuInsertParagraph_Click(object sender, EventArgs e)
        {
            InsertSepLine("par");
        }

        /// <summary>
        /// DElete current line
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuDeleteLine_Click(object sender, EventArgs e)
        {
            DeleteLine();
        }

        /// <summary>
        /// Insert text line
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuInsertTextLine_Click(object sender, EventArgs e)
        {           
            InsertTextLine();
        }

        private void MnuPaste_Click(object sender, EventArgs e)
        {
            // Paste from Clipboard
            PasteClipboard();
            //Load modification into local list of lyrics
            localplLyrics = LoadModifiedLyrics();
            PopulateTextBox(localplLyrics);

            // Color separators
            ColorSepRows();

            // File was modified
            FileModified();
        }

        /// <summary>
        /// Paste from Clipboard
        /// </summary>
        private void PasteClipboard()
        {
            try
            {
                string s = Clipboard.GetText();                
                string[] lines = s.Split('\n');

                int iFail = 0;
                int iRow = 0;
                int iCol = 0;
                if (dgView.CurrentCell != null)
                {
                    iRow = dgView.CurrentCell.RowIndex;
                    iCol = dgView.CurrentCell.ColumnIndex;
                }
                DataGridViewCell oCell;

                string c = string.Empty;

                string plType = string.Empty;
                //int plTicksOn = 0;
                string plRealTime = string.Empty;
                //int plNote = 0;
                string strplnote = string.Empty;
                string plElement = string.Empty;


                if (dgView.Rows.Count < lines.Length)
                    dgView.Rows.Add(lines.Length - 1);
                
                foreach (string line in lines)
                {
                    if (iRow < dgView.RowCount && line.Length > 0)
                    {
                        string[] sCells = line.Split('\t');
                        
                        for (int i = 0; i < sCells.GetLength(0); ++i)
                        {
                            if (iCol + i < this.dgView.ColumnCount)
                            {
                                oCell = dgView[iCol + i, iRow];
                                if (!oCell.ReadOnly)
                                {                                    
                                    c = sCells[i];
                                    //c = c.Trim();
                                    c = c.Replace("\r", "");
                                    c = c.Replace(" ", "_");

                                    oCell.Value = c;                              
                                }
                            }
                            else
                            { break; }
                        }
                        iRow++;
                    }
                    else
                    { break; }

                    
                }
                if (iFail > 0)
                    MessageBox.Show(string.Format("{0} updates failed due" +
                                    " to read only column setting", iFail));
            }
            catch (FormatException)
            {
                MessageBox.Show("The data you pasted is in the wrong format for the cell");
                return;
            }
        }

        private void MnuCopy_Click(object sender, EventArgs e)
        {
            //DGV = this.dgView.SelectedCells;         

            if (dgView.GetCellCount(DataGridViewElementStates.Selected) > 0)
            {
                try
                {
                    Clipboard.SetDataObject(this.dgView.GetClipboardContent());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("The Clipboard could not be accessed. Please try again.\n" + ex.Message);
                }
            }
        }


        /// <summary>
        /// Offset down the third column
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuOffsetDown_Click(object sender, EventArgs e)
        {
            int r = dgView.CurrentRow.Index;
            int row; // = 0;

            for (row = dgView.Rows.Count - 1; row > r; row--) 
            {
                dgView.Rows[row].Cells[COL_TEXT].Value = dgView.Rows[row-1].Cells[COL_TEXT].Value;
            }
            dgView.Rows[r].Cells[COL_TEXT].Value = "";
            localplLyrics = LoadModifiedLyrics();
            PopulateTextBox(localplLyrics);
        }

        /// <summary>
        /// Offset up the third column
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuOffsetUp_Click(object sender, EventArgs e)
        {
            int r = dgView.CurrentRow.Index;
            int row; // = 0;

            for (row = r; row <= dgView.Rows.Count - 2; row++)
            {
                dgView.Rows[row].Cells[COL_TEXT].Value = dgView.Rows[row + 1].Cells[COL_TEXT].Value;
            }
            localplLyrics = LoadModifiedLyrics();
            PopulateTextBox(localplLyrics);
        }

        #endregion context menu
     

        #region functions
        private void LoadOptions()
        {
            try
            {
                _lyricseditfont = Properties.Settings.Default.LyricsEditFont;
                _fontSize = _lyricseditfont.Size;

                _LrcMillisecondsDigits = Properties.Settings.Default.LrcMillisecondsDigits;

                // Regardless the origin of chords (lyrics, from Xml/mxl, discovery)
                // Karaboss is working internally in Midi and chords will be saved in the lyrics 
                // Chords update will be possible if the end user decided to show the chords by clicking a specific button in the karaoke window
                switch (bEditChords)
                {
                    case false:
                        COL_TICKS = 0;
                        COL_TIME = 1;
                        COL_TYPE = 2;
                        COL_NOTE = 3;
                        COL_TEXT = 4;
                        break;
                    case true:
                        COL_TICKS = 0;
                        COL_TIME = 1;
                        COL_TYPE = 2;
                        COL_CHORD = 3;
                        COL_NOTE = 4;
                        COL_TEXT = 5;
                        break;
                    default:
                        break;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }

        private void SaveOptions()
        {
            try
            {
                Properties.Settings.Default.LyricsEditFont = _lyricseditfont;
                //Properties.Settings.Default.Save();
            }
            catch (Exception e)
            {
                Console.Write("Error: " + e.Message);
            }

        }

        /// <summary>
        /// Upadate MIDI times
        /// </summary>
        private void UpdateMidiTimes()
        {
            _totalTicks = sequence1.GetLength();
            _tempo = sequence1.Tempo;
            _ppqn = sequence1.Division;
            _division = sequence1.Division;

            _durationPercent = _tempo * (_totalTicks / _ppqn) / 1000000; //seconds
            //_duration = TempoUtilities.GetMidiDuration(_totalTicks _division);

            if (sequence1.Time != null)
                _measurelen = sequence1.Time.Measure;
        }

        public bool IsNumeric(string input)
        {
            return int.TryParse(input, out int test);
        }

        /// <summary>
        /// Set Title of the form
        /// </summary>
        private void SetTitle(string displayName)
        {            
            displayName = displayName.Replace("__", ": ");
            displayName = displayName.Replace("_", " ");
            Text = "Karaboss - " + Strings.EditWords  + " - " + displayName;
        }

        
        /// <summary>
        /// Display modifications into a textbox
        /// </summary>
        /// <param name="lLyrics"></param>
        private void PopulateTextBox(List<plLyric> lLyrics)
        {
            string plElement; 
            string tx = string.Empty;
            int iParagraph;          
            int iLineFeed; 
            string reste; 

            if (lLyrics == null)
                return;

            for (int i = 0; i < lLyrics.Count; i++)
            {
                // Affiche les blancs
                plElement = lLyrics[i].Element.Item2;
                iParagraph = plElement.LastIndexOf(_InternalSepParagraphs);
                iLineFeed = plElement.LastIndexOf(_InternalSepLines);                

                // If paragraph
                if (iParagraph == 0 || (plElement.Length > _InternalSepParagraphs.Length && iParagraph == plElement.Length - _InternalSepParagraphs.Length))
                {
                    tx += "\r\n\r\n";
                    if (plElement.Length > _InternalSepParagraphs.Length)
                    {
                        if (iParagraph == 0)
                            reste = plElement.Substring(_InternalSepParagraphs.Length, plElement.Length - _InternalSepParagraphs.Length);
                        else
                            reste = plElement.Substring(0, iParagraph);
                    }
                }
                // If Linefeed
                else if (iLineFeed == 0 || (plElement.Length > _InternalSepLines.Length && iLineFeed == plElement.Length - _InternalSepLines.Length))
                {
                    tx += "\r\n";
                    if (plElement.Length > _InternalSepLines.Length)
                    {
                        if (iLineFeed == 0)
                            reste = plElement.Substring(_InternalSepLines.Length, plElement.Length - _InternalSepLines.Length);
                        else
                            reste = plElement.Substring(0, iLineFeed);
                    }
                }
                else
                {
                    if (bEditChords)
                    {
                        // Chords edition
                        // Extract chord name from the lyrics
                        if (plElement != "" && _myLyricsMgmt.ChordsOriginatedFrom == MidiLyricsMgmt.ChordsOrigins.Lyrics)
                        {
                            // Remove chord from lyrics
                            plElement = Regex.Replace(plElement, _myLyricsMgmt.RemoveChordPattern, @"");
                        }
                    }

                    tx += plElement;
                }                             
            }

            txtResult.Text = tx;

            txtResult.SelectAll();
            txtResult.SelectionAlignment = HorizontalAlignment.Center;
        }

        /// <summary>
        /// Show line of texbox currently edited
        /// </summary>
        private void ShowCurrentLine()
        {
            int r = dgView.CurrentCell.RowIndex;
                       
            // Text before current
            string tx = string.Empty;
            string s; // = string.Empty;

            for (int row = 0; row < r; row++)
            {
                s = string.Empty;

                if (dgView.Rows[row].Cells[COL_TYPE].Value != null)
                {
                    if (dgView.Rows[row].Cells[COL_TYPE].Value.ToString() == "cr")
                        s = "\n";
                    else if (dgView.Rows[row].Cells[COL_TYPE].Value.ToString() == "par")
                        s = "\n\n";
                    else if (dgView.Rows[row].Cells[COL_TYPE].Value.ToString() == "text")
                    {
                        if (dgView.Rows[row].Cells[COL_TEXT].Value != null)
                            s = dgView.Rows[row].Cells[COL_TEXT].Value.ToString();
                    }
                }
               
                s = s.Replace("_", " ");                

                tx += s;
            }

            if (tx != "")
            {
                int start = txtResult.Text.IndexOf(tx);
                if (start == 0)
                {
                    int L = tx.Length;
                    txtResult.SelectionColor = txtResult.ForeColor;

                    txtResult.SelectionStart = 0;
                    txtResult.SelectionLength = L;
                    txtResult.SelectionColor = Color.White;                    
                }
            }
        }

        /// <summary>
        /// height of rows = duration 
        /// </summary>
        private void HeightsToDurations()
        {
            int plTicksOn; // = 0;
            int n = 0;
            int averageDuration = 0;
            int Duration; // = 0;
            int H; // = 0;
            int H0 = 22;
            int newH; // = 0;
            int delta; // = 0;
            int previousTime = 0;

            // Average duration
            for (int row = 0; row < dgView.Rows.Count; row++)
            {
                if (dgView.Rows[row].Cells[COL_TICKS].Value != null && IsNumeric(dgView.Rows[row].Cells[COL_TICKS].Value.ToString()))
                {
                    plTicksOn = Convert.ToInt32(dgView.Rows[row].Cells[COL_TICKS].Value);
                    if (previousTime == 0)
                    {
                        previousTime = plTicksOn;
                    }
                    else
                    {
                        if (plTicksOn > previousTime)
                        {
                            averageDuration += (plTicksOn - previousTime);
                            previousTime = plTicksOn;
                            n++;
                        }
                    }                    
                }
            }

            if (n > 0)
                averageDuration = averageDuration / n;

            previousTime = 0;
            for (int row = 0; row < dgView.Rows.Count; row++)
            {
                if (dgView.Rows[row].Cells[COL_TICKS].Value != null && IsNumeric(dgView.Rows[row].Cells[COL_TICKS].Value.ToString()))
                {
                    plTicksOn = Convert.ToInt32(dgView.Rows[row].Cells[COL_TICKS].Value);
                    if (plTicksOn > 0)
                    {
                        if (previousTime == 0)
                        {
                            previousTime = plTicksOn;
                        }
                        else if (plTicksOn > previousTime)
                        {
                            H = dgView.Rows[row].Height;
                            Duration = plTicksOn - previousTime;
                            delta = Duration / averageDuration;

                            if (delta > 0)
                            {
                                newH = H + H * delta;
                                if (newH > 5 * H0)
                                    newH = 5 * H0;
                                dgView.Rows[row].Height = newH;
                            }
                            previousTime = plTicksOn;
                        }
                    }
                }
            }
        }


        #endregion functions


        #region gridview

        /// <summary>
        /// Cell edition
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int val = 0;

            // If first col is edited (TICKS)
            if (dgView.CurrentCell.ColumnIndex == COL_TICKS)
            {
                if (!IsNumeric(dgView.CurrentCell.Value.ToString()))
                {
                    if (dgView.CurrentCell.RowIndex == 0)
                        dgView.Rows[dgView.CurrentCell.RowIndex].Cells[COL_TICKS].Value = 0;
                    else
                    {
                        val = 0;
                        if (dgView.Rows[dgView.CurrentCell.RowIndex - 1].Cells[COL_TICKS].Value != null && IsNumeric(dgView.Rows[dgView.CurrentCell.RowIndex - 1].Cells[COL_TICKS].Value.ToString()))
                            val = Convert.ToInt32(dgView.Rows[dgView.CurrentCell.RowIndex - 1].Cells[COL_TICKS].Value);
                        dgView.Rows[dgView.CurrentCell.RowIndex].Cells[COL_TICKS].Value = val;
                    }
                }
                else
                {
                    if (dgView.CurrentCell.RowIndex > 0)
                    {
                        if (dgView.Rows[dgView.CurrentCell.RowIndex - 1].Cells[COL_TICKS].Value != null && IsNumeric(dgView.Rows[dgView.CurrentCell.RowIndex - 1].Cells[COL_TICKS].Value.ToString()))
                            val = Convert.ToInt32(dgView.Rows[dgView.CurrentCell.RowIndex - 1].Cells[COL_TICKS].Value);
                        if (Convert.ToInt32(dgView.CurrentCell.Value) < val)
                            dgView.CurrentCell.Value = val;
                    }
                }

                // Default Type = "text"
                if (dgView.Rows[dgView.CurrentCell.RowIndex].Cells[COL_TYPE].Value == null)
                    dgView.Rows[dgView.CurrentCell.RowIndex].Cells[COL_TYPE].Value = "text";


                if (dgView.Rows[dgView.CurrentCell.RowIndex].Cells[dgView.Columns.Count - 1].Value == null)
                    dgView.Rows[dgView.CurrentCell.RowIndex].Cells[dgView.Columns.Count - 1].Value = "";

                // Ticks to time
                if (dgView.CurrentCell.Value != null && IsNumeric(dgView.CurrentCell.Value.ToString()))
                    dgView.Rows[dgView.CurrentCell.RowIndex].Cells[COL_TIME].Value = Utilities.LyricsUtilities.TicksToTime(Convert.ToInt32(dgView.CurrentCell.Value), _division);

            }
            else if (dgView.CurrentCell.ColumnIndex == COL_TIME)
            {
                // If COL_TIME is edited
                if (dgView.CurrentCell.Value != null)
                {

                    string sval = dgView.CurrentCell.Value.ToString().Trim();
                    Regex regex = new Regex(@"\d\d:\d\d.\d\d\d");
                    Match match = regex.Match(sval);
                    if (sval != "" && !match.Success)
                    {
                        if (IsNumeric(sval))
                        {
                            int total = Convert.ToInt32(sval);
                            int min = total / 60;
                            int sec = total - min * 60;

                            dgView.CurrentCell.Value = string.Format("{0:00}:{1:00}.000", min, sec);
                        }
                        else
                        {
                            MessageBox.Show("Please use format 00:00.000", "Time", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            dgView.CurrentCell.Value = "00:00.000";
                        }
                    }

                    // Time to ticks
                    //dgView.Rows[dgView.CurrentCell.RowIndex].Cells[COL_TICKS].Value = TimeToTicks(dgView.CurrentCell.Value.ToString());
                    dgView.Rows[dgView.CurrentCell.RowIndex].Cells[COL_TICKS].Value = Utilities.LyricsUtilities.TimeToTicks(dgView.CurrentCell.Value.ToString(), _division, _totalTicks);
                    // Type = text
                    dgView.Rows[dgView.CurrentCell.RowIndex].Cells[COL_TYPE].Value = "text";

                }
            }
            else if (dgView.CurrentCell.ColumnIndex == COL_TYPE)
            {
                // If COL_TYPE is edited
                if (dgView.CurrentCell.Value != null)
                {
                    string type = dgView.CurrentCell.Value.ToString();
                    switch (type)
                    {
                        case "text":
                            break;
                        case "par":
                            dgView.Rows[dgView.CurrentCell.RowIndex].Cells[COL_TEXT].Value = m_SepParagraph;
                            break;
                        case "cr":
                            dgView.Rows[dgView.CurrentCell.RowIndex].Cells[COL_TEXT].Value = m_SepLine;
                            break;
                        default:
                            dgView.Rows[dgView.CurrentCell.RowIndex].Cells[COL_TYPE].Value = "text";
                            break;
                    }
                }

            }
            else if (dgView.CurrentCell.ColumnIndex == dgView.Columns.Count - 1)
            {
                // COL TEXT

                if (dgView.CurrentCell.Value == null)
                    dgView.CurrentCell.Value = "";

                if (dgView.CurrentCell.Value.ToString() == m_SepLine)
                {
                    dgView.Rows[dgView.CurrentCell.RowIndex].Cells[COL_TYPE].Value = "cr";
                }
                else if (dgView.CurrentCell.Value.ToString() == m_SepParagraph)
                {
                    dgView.Rows[dgView.CurrentCell.RowIndex].Cells[COL_TYPE].Value = "par";
                }
                else
                    dgView.Rows[dgView.CurrentCell.RowIndex].Cells[COL_TYPE].Value = "text";

                string c = dgView.CurrentCell.Value.ToString();
                c = c.Replace(" ", "_");
                dgView.CurrentCell.Value = c;

                // Retrieve time value of previous row
                if (dgView.Rows[dgView.CurrentCell.RowIndex].Cells[COL_TICKS].Value == null || !IsNumeric(dgView.Rows[dgView.CurrentCell.RowIndex].Cells[COL_TICKS].Value.ToString()))
                {
                    if (dgView.CurrentCell.RowIndex == 0)
                        dgView.Rows[dgView.CurrentCell.RowIndex].Cells[COL_TICKS].Value = 0;
                    else
                    {
                        val = 0;
                        if (dgView.Rows[dgView.CurrentCell.RowIndex - 1].Cells[COL_TICKS].Value != null && IsNumeric(dgView.Rows[dgView.CurrentCell.RowIndex - 1].Cells[COL_TICKS].Value.ToString()))
                            val = Convert.ToInt32(dgView.Rows[dgView.CurrentCell.RowIndex - 1].Cells[COL_TICKS].Value);
                        dgView.Rows[dgView.CurrentCell.RowIndex].Cells[COL_TICKS].Value = val;
                    }
                }
            }

            //Load modification into local list of lyrics
            localplLyrics = LoadModifiedLyrics(false);
            PopulateTextBox(localplLyrics);

            // Modify height of cells according to durations
            HeightsToDurations();

            // Color separators
            ColorSepRows();



            // File was modified
            FileModified();

        }


        /// <summary>
        /// Display current line in textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            ShowCurrentLine();
        }

        private void DgView_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    {
                        foreach (DataGridViewCell C in dgView.SelectedCells)
                        {
                            if (C.ColumnIndex != 0)
                                C.Value = "";
                        }
                        //Load modification into local list of lyrics
                        localplLyrics = LoadModifiedLyrics();
                        PopulateTextBox(localplLyrics);

                        // File was modified
                        FileModified();
                        break;
                    }
            }
        }

        /// <summary>
        /// Initialize gridview
        /// </summary>
        private void InitGridView()
        {
            dgView.Rows.Clear();
            dgView.Refresh();

            // Header color
            dgView.ColumnHeadersDefaultCellStyle.BackColor = dgViewHeaderBackColor;
            dgView.ColumnHeadersDefaultCellStyle.ForeColor = dgViewHeaderForeColor;

            dgView.ColumnHeadersDefaultCellStyle.Font = dgViewHeaderFont;
            dgView.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Header Column width (with rows numbers)
            dgView.RowHeadersWidth = 60;

            dgView.RowsAdded += new DataGridViewRowsAddedEventHandler(dgView_RowsAdded);
            dgView.RowsRemoved += new DataGridViewRowsRemovedEventHandler(dgView_RowsRemoved);


            // Selection
            dgView.DefaultCellStyle.SelectionBackColor = dgViewSelectionBackColor;

            dgView.EnableHeadersVisualStyles = false;


            // Create columns
            if (!bEditChords)
            {
                dgView.ColumnCount = 5;

                dgView.Columns[0].Name = "dTime";
                dgView.Columns[0].HeaderText = "Ticks";
                dgView.Columns[0].ToolTipText = "Number of ticks";
                dgView.Columns[0].Width = 70;

                dgView.Columns[1].Name = "dRealTime";
                dgView.Columns[1].HeaderText = "Time";
                dgView.Columns[1].ToolTipText = "Time";
                dgView.Columns[0].Width = 80;

                dgView.Columns[2].Name = "dType";
                dgView.Columns[2].HeaderText = "Type";
                dgView.Columns[2].ToolTipText = "Type of lyric (cr or text)";
                dgView.Columns[2].Width = 60;

                dgView.Columns[3].Name = "dNote";
                dgView.Columns[3].HeaderText = "Note";
                dgView.Columns[3].ToolTipText = "Played note";
                dgView.Columns[3].Width = 50;

                dgView.Columns[4].Name = "dText";
                dgView.Columns[4].HeaderText = "Text";
                dgView.Columns[4].ToolTipText = "Original text";
                dgView.Columns[4].Width = 80;
            }
            else
            {
                // Chords edition
                dgView.ColumnCount = 6;

                dgView.Columns[0].Name = "dTime";
                dgView.Columns[0].HeaderText = "Ticks";
                dgView.Columns[0].ToolTipText = "Number of ticks";
                dgView.Columns[0].Width = 70;

                dgView.Columns[1].Name = "dRealTime";
                dgView.Columns[1].HeaderText = "Time";
                dgView.Columns[1].ToolTipText = "Time";
                dgView.Columns[1].Width = 80;

                dgView.Columns[2].Name = "dType";
                dgView.Columns[2].HeaderText = "Type";
                dgView.Columns[2].ToolTipText = "Type of lyric (cr or text)";
                dgView.Columns[2].Width = 60;

                dgView.Columns[3].Name = "dChord";
                dgView.Columns[3].HeaderText = "Chord";
                dgView.Columns[3].ToolTipText = "Chord";
                dgView.Columns[3].Width = 70;

                dgView.Columns[4].Name = "dNote";
                dgView.Columns[4].HeaderText = "Note";
                dgView.Columns[4].ToolTipText = "Played note";
                dgView.Columns[4].Width = 50;

                dgView.Columns[5].Name = "dText";
                dgView.Columns[5].HeaderText = "Text";
                dgView.Columns[5].ToolTipText = "Original text";
                dgView.Columns[5].Width = 80;
            }


            //Change cell font
            foreach (DataGridViewColumn c in dgView.Columns)
            {
                c.SortMode = DataGridViewColumnSortMode.NotSortable;                     // header not sortable
                c.DefaultCellStyle.Font = dgViewCellsFont;
                c.ReadOnly = false;
            }
        }

        private void dgView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            NumberRows();
        }

        private void dgView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            NumberRows();
        }


        /// <summary>
        /// Apply colors to comlumns: Line Break & Paragraph
        /// </summary>
        private void ColorSepRows()
        {
            for (int i = 0; i < dgView.Rows.Count; i++)
            {
                if (dgView.Rows[i].Cells[COL_TYPE].Value != null && dgView.Rows[i].Cells[COL_TYPE].Value.ToString() == "cr")
                {
                    dgView.Rows[i].DefaultCellStyle.BackColor = SepLinesColor;
                }
                else if (dgView.Rows[i].Cells[COL_TYPE].Value != null && dgView.Rows[i].Cells[COL_TYPE].Value.ToString() == "par")
                {
                    dgView.Rows[i].DefaultCellStyle.BackColor = SepParagrColor;
                }
            }
        }


        /// <summary>
        /// Populate datagridview with lyrics
        /// </summary>
        /// <param name="plLyrics"></param>
        private void PopulateDataGridView(List<plLyric> lLyrics)
        {
            int plTicksOn;
            string plRealTime;
            plLyric.CharTypes plType;

            string sNote;
            string plChordName;
            string plElement;
            string[] rowlyric;
            int idx;


            switch (_myLyricsMgmt.ChordsOriginatedFrom)
            {
                case MidiLyricsMgmt.ChordsOrigins.Lyrics:
                    // Chords are included inthe lyrics
                    break;
                // Chords are not included in the lyrics
                case MidiLyricsMgmt.ChordsOrigins.XmlEmbedded:
                case MidiLyricsMgmt.ChordsOrigins.MidiEmbedded:
                case MidiLyricsMgmt.ChordsOrigins.Discovery:
                    break;
            }

            string lyrics = _myLyricsMgmt.Lyrics;

            // ==================================================
            // Case: no track supplied for the melody => 0 notes, only lyrics
            // Write only the lyrics into the grid
            // ==================================================
            if (melodyTrack == null)
            {
                // On affiche la liste des lyrics 
                for (idx = 0; idx < lLyrics.Count; idx++)
                {
                    plTicksOn = lLyrics[idx].TicksOn;
                    plRealTime = Utilities.LyricsUtilities.TicksToTime(plTicksOn, _division);
                    //plNote = 0;
                    sNote = "";
                    plChordName = lLyrics[idx].Element.Item1;
                    plElement = lLyrics[idx].Element.Item2;
                    plElement = plElement.Replace(" ", "_");
                    plType = lLyrics[idx].CharType;

                    // New Row
                    switch (plType)
                    {
                        case plLyric.CharTypes.LineFeed:
                            plElement = m_SepLine;
                            break;
                        case plLyric.CharTypes.ParagraphSep:
                            plElement = m_SepParagraph;
                            break;
                        default:
                            break;
                    }

                    if (!bEditChords)
                    {
                        // No chord edition, do not modify lyrics
                        rowlyric = new string[] { plTicksOn.ToString(), plRealTime, Karaclass.plTypeToString(plType), sNote, plElement };
                    }
                    else
                    {
                        // Chords edition
                        // Extract chord name from the lyrics
                        if (plElement != "" && _myLyricsMgmt.ChordsOriginatedFrom == MidiLyricsMgmt.ChordsOrigins.Lyrics)
                        {
                            // Remove chord
                            plElement = Regex.Replace(plElement, _myLyricsMgmt.RemoveChordPattern, @"");
                        }
                        rowlyric = new string[] { plTicksOn.ToString(), plRealTime, Karaclass.plTypeToString(plType), plChordName, sNote, plElement };
                    }
                    dgView.Rows.Add(rowlyric);
                }
            }
            else
            {
                // A track is provided for the melody => write the notes and lyrics according to their startimes
                PopulateDataGridViewWithMelodyTrack(lLyrics);
            }
        }

        /// <summary>
        /// Case: A track is provided for the melody => write the notes and lyrics according to their startimes
        /// </summary>
        /// <param name="lLyrics"></param>
        private void PopulateDataGridViewWithMelodyTrack(List<plLyric> lLyrics)
        {
            int plTicksOn;
            string plRealTime;
            string plElement;
            plLyric.CharTypes plType;
            int t;
            string[] rowline;
            int plNote;
            bool bFound;
            string sNote;
            string plChordName = string.Empty;
            // Not found notes
            List<MidiNote> lstNotFound = new List<MidiNote>();

            if (lLyrics == null)
                return;

            // 1. First add rows for all the lyrics (so the order of the lyrics will be kept)
            for (int i = 0; i < lLyrics.Count; i++)
            {
                plTicksOn = lLyrics[i].TicksOn;
                plRealTime = Utilities.LyricsUtilities.TicksToTime(plTicksOn, _division);
                plChordName = lLyrics[i].Element.Item1;
                plElement = lLyrics[i].Element.Item2;
                plType = lLyrics[i].CharType;
                sNote = "";

                switch (plType)
                {
                    case plLyric.CharTypes.Text:
                        // Replace blank spaces by underscore
                        plElement = plElement.Replace(" ", "_");
                        break;
                    case plLyric.CharTypes.LineFeed:
                        plElement = m_SepLine;
                        break;
                    case plLyric.CharTypes.ParagraphSep:
                        plElement = m_SepParagraph;
                        break;
                    default:
                        break;
                }

                if (!bEditChords)
                {
                    rowline = new string[] { plTicksOn.ToString(), plRealTime, Karaclass.plTypeToString(plType), sNote, plElement };
                }
                else
                {
                    // Chords edition
                    // Extract chord name from the lyrics
                    if (plElement != "" && _myLyricsMgmt.ChordsOriginatedFrom == MidiLyricsMgmt.ChordsOrigins.Lyrics)
                    {
                        // Remove chord
                        plElement = Regex.Replace(plElement, _myLyricsMgmt.RemoveChordPattern, @"");
                    }
                    rowline = new string[] { plTicksOn.ToString(), plRealTime, Karaclass.plTypeToString(plType), plChordName, sNote, plElement };
                }

                dgView.Rows.Add(rowline);
            }

            // 2. Second, try to associate notes to existing rows
            for (int i = 0; i < melodyTrack.Notes.Count; i++)
            {
                plTicksOn = melodyTrack.Notes[i].StartTime;
                plNote = melodyTrack.Notes[i].Number;
                bFound = false;
                for (int row = 0; row < dgView.Rows.Count; row++)
                {
                    if (dgView.Rows[row].Cells[COL_TICKS].Value != null && IsNumeric(dgView.Rows[row].Cells[COL_TICKS].Value.ToString()))
                    {
                        t = Convert.ToInt32(dgView.Rows[row].Cells[COL_TICKS].Value);

                        if (t == plTicksOn)
                        {
                            // Associate notes only to text rows (lyrics)
                            if (dgView.Rows[row].Cells[COL_TYPE].Value.ToString() == "text")
                            {
                                dgView.Rows[row].Cells[COL_NOTE].Value = plNote.ToString();
                                bFound = true;
                                break;
                            }
                        }
                        else if (t > plTicksOn)
                        {
                            break;
                        }
                    }
                }

                if (!bFound)
                {
                    // Notes having no row
                    lstNotFound.Add(melodyTrack.Notes[i]);
                }
            }


            // 3. insert Lost notes  on new rows
            if (lstNotFound.Count > 0)
            {
                for (int i = 0; i < lstNotFound.Count; i++)
                {
                    plTicksOn = lstNotFound[i].StartTime;
                    plRealTime = Utilities.LyricsUtilities.TicksToTime(plTicksOn, _division);
                    plNote = lstNotFound[i].Number;

                    bFound = false;
                    for (int row = 0; row < dgView.Rows.Count; row++)
                    {
                        // insert is possible if the notes are before the last row
                        if (dgView.Rows[row].Cells[COL_TICKS].Value != null && IsNumeric(dgView.Rows[row].Cells[COL_TICKS].Value.ToString()))
                        {
                            t = Convert.ToInt32(dgView.Rows[row].Cells[COL_TICKS].Value);
                            // Insert note
                            if (t > plTicksOn)
                            {

                                if (!bEditChords)
                                    dgView.Rows.Insert(row, plTicksOn, plRealTime, "text", plNote.ToString(), "");
                                else
                                {
                                    dgView.Rows.Insert(row, plTicksOn, plRealTime, "text", plChordName, plNote.ToString(), "");
                                }

                                bFound = true;
                                break;
                            }
                        }
                    }

                    if (!bFound)
                    {
                        // Missing notes must be added at the end of the gridview, after the last row                        
                        if (!bEditChords)
                            rowline = new string[] { plTicksOn.ToString(), plRealTime, "text", plNote.ToString(), "" };
                        else
                            rowline = new string[] { plTicksOn.ToString(), plRealTime, "text", plChordName, plNote.ToString(), "" };

                        dgView.Rows.Add(rowline);
                    }
                }
            }
        }



        /// <summary>
        /// Populate DataGridView with only notes of a track
        /// </summary>
        /// <param name="tracknumber"></param>
        private void PopulateDataGridViewTrack(int tracknumber)
        {
            InitGridView();

            if (tracknumber >= 0 && tracknumber < sequence1.tracks.Count)
            {
                Track track = sequence1.tracks[tracknumber];
                int plTicksOn;
                string plRealTime;
                string plType;
                int plNote;
                string plChordName = string.Empty;
                string plElement;
                string[] row;

                int lastStartTime = -1;

                for (int i = 0; i < track.Notes.Count; i++)
                {
                    MidiNote n = track.Notes[i];
                    plTicksOn = n.StartTime;
                    if (plTicksOn != lastStartTime)
                    {
                        lastStartTime = plTicksOn;                  // avoid all notes of a chords
                        plRealTime = Utilities.LyricsUtilities.TicksToTime(plTicksOn, _division);
                        plType = "text";
                        plNote = n.Number;

                        plElement = plNote.ToString();

                        if (!bEditChords)
                            row = new string[] { plTicksOn.ToString(), plRealTime, plType, plNote.ToString(), plElement };
                        else
                            row = new string[] { plTicksOn.ToString(), plRealTime, plType, plChordName, plNote.ToString(), plElement };

                        dgView.Rows.Add(row);
                    }
                }
            }
        }


        private void NumberRows()
        {
            foreach (DataGridViewRow r in dgView.Rows)
            {
                dgView.Rows[r.Index].HeaderCell.Value =
                                    (r.Index + 1).ToString();
            }
        }

        #endregion gridview


        #region form load close resize

        /// <summary>
        /// Form load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmLyricsEdit_Load(object sender, EventArgs e)
        {
            // Récupère la taille et position de la forme

            // If window is maximized
            if (Properties.Settings.Default.frmLyricsEditMaximized)
            {

                Location = Properties.Settings.Default.frmLyricsEditLocation;
                //Size = Properties.Settings.Default.frmLyricsEditSize;
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                Location = Properties.Settings.Default.frmLyricsEditLocation;
                // Verify if this windows is visible in extended screens
                Rectangle rect = new Rectangle(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);
                foreach (Screen screen in Screen.AllScreens)
                    rect = Rectangle.Union(rect, screen.Bounds);

                if (Location.X > rect.Width)
                    Location = new Point(0, Location.Y);
                if (Location.Y > rect.Height)
                    Location = new Point(Location.X, 0);

                Size = Properties.Settings.Default.frmLyricsEditSize;
            }
        }

        /// <summary>
        /// Form closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmLyricsEdit_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bfilemodified == true)
            {
                //string tx = "Le fichier a été modifié, voulez-vous l'enregistrer ?";
                string tx = Karaboss.Resources.Localization.Strings.QuestionSavefile;

                DialogResult dr = MessageBox.Show(tx, "Karaboss", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (dr == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                else if (dr == DialogResult.Yes)
                {
                    e.Cancel = true;

                    //Load modification into local list of lyrics
                    localplLyrics = LoadModifiedLyrics();

                    // Display new lyrics in frmPlayer
                    ReplaceLyrics(localplLyrics);

                    // Save file
                    SaveFileProc();
                    return;
                }
                else
                {
                    if (Application.OpenForms.OfType<frmPlayer>().Count() > 0)
                    {
                        frmPlayer frmPlayer = Utilities.FormUtilities.GetForm<frmPlayer>();
                        frmPlayer.bfilemodified = false;
                    }
                }
            }

            // enregistre la taille et la position de la forme
            // Copy window location to app settings                
            if (WindowState != FormWindowState.Minimized)
            {
                if (WindowState == FormWindowState.Maximized)
                {
                    Properties.Settings.Default.frmLyricsEditLocation = RestoreBounds.Location;
                    Properties.Settings.Default.frmLyricsEditMaximized = true;

                }
                else if (WindowState == FormWindowState.Normal)
                {
                    Properties.Settings.Default.frmLyricsEditLocation = Location;
                    Properties.Settings.Default.frmLyricsEditSize = Size;
                    Properties.Settings.Default.frmLyricsEditMaximized = false;
                }

                SaveOptions();

                // Save settings
                Properties.Settings.Default.Save();
            }

            Dispose();
        }

        /// <summary>
        /// Form Resize
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmLyricsEdit_Resize(object sender, EventArgs e)
        {
            ResizeMe();
        }

        private void ResizeMe()
        {
            tabControl1.Top = pnlMenus.Height;
            tabControl1.Width = this.ClientSize.Width;
            tabControl1.Height = this.ClientSize.Height - pnlMenus.Height;

            // Adapt width of last column
            int W = dgView.RowHeadersWidth + 19;
            int WP = dgView.Parent.Width;
            for (int i = 0; i < dgView.Columns.Count - 1; i++)
            {
                W += dgView.Columns[i].Width;
            }
            if (WP - W > 0)
                dgView.Columns[dgView.Columns.Count - 1].Width = WP - W;

        }


        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            // Adapt width of last column
            int W = dgView.RowHeadersWidth + 19;
            int WP = dgView.Parent.Width;
            for (int i = 0; i < dgView.Columns.Count - 1; i++)
            {
                W += dgView.Columns[i].Width;
            }
            if (WP - W > 0)
                dgView.Columns[dgView.Columns.Count - 1].Width = WP - W;
        }

        #endregion form load close resize


        #region Lyrics       

        private void OptFormatText_CheckedChanged(object sender, EventArgs e)
        {
            if (optFormatText.Checked)
                TextLyricFormat = LyricFormats.Text;
        }

        private void OptFormatLyrics_CheckedChanged(object sender, EventArgs e)
        {
            if (optFormatLyrics.Checked)
                TextLyricFormat = LyricFormats.Lyric;
        }


        /// <summary>
        /// Reload localpLyrics with data from gridview
        /// format lyrics to include the chords
        /// </summary>
        private List<plLyric> LoadModifiedLyrics(bool bIncludeChordsInLyrics = false)
        {

            int line;
            if (!CheckTimes(out line))
            {
                MessageBox.Show("Time on line " + line + " is incorrect", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dgView.CurrentCell = dgView.Rows[line - 1].Cells[0];
                return null;
            }

            int plTicksOn;
            string val;
            plLyric.CharTypes plType;
            string plElement;
            string chordName = string.Empty;


            List<plLyric> lst = new List<plLyric>();

            for (int row = 0; row < dgView.Rows.Count; row++)
            {
                if (dgView.Rows[row].Cells[COL_TICKS].Value != null)
                {
                    if (dgView.Rows[row].Cells[COL_TICKS].Value != null && IsNumeric(dgView.Rows[row].Cells[COL_TICKS].Value.ToString()))
                        plTicksOn = Convert.ToInt32(dgView.Rows[row].Cells[COL_TICKS].Value);
                    else
                        plTicksOn = 0;

                    // Type
                    if (dgView.Rows[row].Cells[COL_TYPE].Value != null)
                    {
                        val = dgView.Rows[row].Cells[COL_TYPE].Value.ToString();
                        switch (val)
                        {
                            case "text":
                                plType = plLyric.CharTypes.Text;
                                break;
                            case "cr":
                                plType = plLyric.CharTypes.LineFeed;
                                break;
                            case "par":
                                plType = plLyric.CharTypes.ParagraphSep;
                                break;
                            default:
                                plType = plLyric.CharTypes.Text;
                                break;
                        }
                    }
                    else
                    {
                        plType = plLyric.CharTypes.Text;
                    }

                    // Element
                    if (dgView.Rows[row].Cells[COL_TEXT].Value != null)
                    {
                        if (plType == plLyric.CharTypes.LineFeed)
                            plElement = _InternalSepLines;
                        else if (plType == plLyric.CharTypes.ParagraphSep)
                            plElement = _InternalSepParagraphs;
                        else
                            plElement = dgView.Rows[row].Cells[COL_TEXT].Value.ToString();
                    }
                    else
                        plElement = "text";

                    // replace again spaces
                    plElement = plElement.Replace("_", " ");

                    // chords edition
                    if (bEditChords)
                    {
                        // Read chord name
                        if (dgView.Rows[row].Cells[COL_CHORD].Value != null)
                        {
                            chordName = dgView.Rows[row].Cells[COL_CHORD].Value.ToString();
                        }

                        // Modifie text to include the chordname into the lyric
                        // [Am]La petite maison [G]dans la prairie
                        if (bIncludeChordsInLyrics && chordName != "")
                        {
                            plElement = "[" + chordName + "]" + plElement;
                        }

                    }

                    lst.Add(new plLyric() { CharType = plType, Element = (chordName, plElement), TicksOn = plTicksOn });
                }
            }

            return lst;
        }

        /// <summary>
        /// Replace lyrics in frmPlayer
        /// Appelle la méthode ReplaceLyrics de frmPlayer
        /// </summary>
        private void ReplaceLyrics(List<plLyric> l)
        {
            LyricTypes ltype;

            if (TextLyricFormat == LyricFormats.Text)
                ltype = LyricTypes.Text;
            else
                ltype = LyricTypes.Lyric;


            if (Application.OpenForms.OfType<frmPlayer>().Count() > 0)
            {
                frmPlayer frmPlayer = Utilities.FormUtilities.GetForm<frmPlayer>();
                frmPlayer.ReplaceLyrics(l, ltype, melodytracknum);
            }
        }

        #endregion Lyrics


        #region Lrc import export

        #region Save lrc

        /// <summary>
        /// Menu: Export lyrics to format LRC
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuFileSaveAsLrc_Click(object sender, EventArgs e)
        {
            GetLrcSaveOptions();            
        }

        /// <summary>
        /// Get save lrc options
        /// </summary>
        /// <param name="LrcExportFormat"></param>
        private void GetLrcSaveOptions()
        {
            DialogResult dr;
            frmLrcOptions LrcOptionsDialog = new frmLrcOptions();
            dr = LrcOptionsDialog.ShowDialog();

            if (dr == System.Windows.Forms.DialogResult.Cancel)
                return;

            // Remove accents
            bool bRemoveAccents = LrcOptionsDialog.bRemoveAccents;
            // Force Upper Case
            bool bUpperCase = LrcOptionsDialog.bUpperCase;
            // Force Lower Case
            bool bLowerCase = LrcOptionsDialog.bLowerCase;
            // Remove all non-alphanumeric characters
            bool bRemoveNonAlphaNumeric = LrcOptionsDialog.bRemoveNonAlphaNumeric;
            // Save to line or to syllabes
            LrcLinesSyllabesFormats LrcLinesSyllabesFormat = LrcOptionsDialog.LrcLinesSyllabesFormat;

            _LrcMillisecondsDigits = LrcOptionsDialog.LrcMillisecondsDigits;

            // Cut lines over x characters
            bool bCutLines = LrcOptionsDialog.bCutLines;
            int LrcCutLinesChars = LrcOptionsDialog.LrcCutLinesChars;


            SaveLrcFileName(LrcLinesSyllabesFormat, bRemoveAccents, bUpperCase, bLowerCase, bRemoveNonAlphaNumeric, bCutLines, LrcCutLinesChars);
        }

        /// <summary>
        /// Select file to save and format tags
        /// </summary>
        /// <param name="LrcExportFormat"></param>
        /// <param name="LrcLinesSyllabesFormat"></param>
        /// <param name="bRemoveAccents"></param>
        /// <param name="bUpperCase"></param>
        /// <param name="bLowerCase"></param>
        /// <param name="bRemoveNonAlphaNumeric"></param>
        /// <param name="bCutLines"></param>
        /// <param name="LrcCutLinesChars"></param>
        private void SaveLrcFileName(LrcLinesSyllabesFormats LrcLinesSyllabesFormat, bool bRemoveAccents, bool bUpperCase, bool bLowerCase, bool bRemoveNonAlphaNumeric, bool bCutLines, int LrcCutLinesChars)
        {
            #region select filename

            string defExt = ".lrc";
            string fName = "New" + defExt;
            string fPath = Path.GetDirectoryName(MIDIfileName);

            string fullName;
            string defName;

            #region search name

            if (fPath == null || fPath == "")
            {
                if (Directory.Exists(CreateNewMidiFile._DefaultDirectory))
                    fPath = CreateNewMidiFile._DefaultDirectory;
                else
                    fPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            }
            else
            {
                fName = Path.GetFileName(MIDIfileName);
            }

            // Extension forced to lrc            
            string fullPath = fPath + "\\" + Path.GetFileNameWithoutExtension(fName) + defExt;
            fullName = Utilities.Files.FindUniqueFileName(fullPath);                            // Add (2), (3) etc.. if necessary    
            defName = Path.GetFileNameWithoutExtension(fullName);                               // Default name to propose to dialog

            #endregion search name                   

            string defFilter = "LRC files (*.lrc)|*.lrc|All files (*.*)|*.*";

            saveMidiFileDialog.Title = "Save to LRC format";
            saveMidiFileDialog.Filter = defFilter;
            saveMidiFileDialog.DefaultExt = defExt;
            saveMidiFileDialog.InitialDirectory = @fPath;
            saveMidiFileDialog.FileName = defName;

            if (saveMidiFileDialog.ShowDialog() != DialogResult.OK)
                return;

            #endregion

            string Tag_Tool = "Karaboss https://karaboss.lacharme.net";

            string Tag_Title = string.Empty;
            string Tag_Artist = string.Empty;
            string Tag_Album = string.Empty;
            string Tag_Lang = string.Empty;
            string Tag_By = string.Empty;
            string Tag_DPlus = string.Empty;

            fullPath = saveMidiFileDialog.FileName;

            // Search Title & Artist
            // Classic Karaoke Midi tags
            /*
            @K	(multiple) K1: FileType ex MIDI KARAOKE FILE, K2: copyright of Karaoke file
            @L	(single) Language	FRAN, ENGL        
            @W	(multiple) Copyright (of Karaoke file, not song)        
            @T	(multiple) Title1 @T<title>, Title2 @T<author>, Title3 @T<copyright>		
            @I	Information  ex Date(of Karaoke file, not song)
            @V	(single) Version ex 0100 ?             
            */
            Tag_Title = (sequence1.TTag != null && sequence1.TTag.Count > 1) ? sequence1.TTag[0] : "";
            Tag_Artist = (sequence1.TTag != null && sequence1.TTag.Count > 1) ? sequence1.TTag[1] : "";


            if (Tag_Artist == "" && Tag_Title == "")
            {
                List<string> lstTags = Utilities.LyricsUtilities.GetTagsFromFileName(fullPath);
                Tag_Artist = lstTags[0];
                Tag_Title = lstTags[1];
            }

            switch (LrcLinesSyllabesFormat)
            {
                case LrcLinesSyllabesFormats.Lines:
                    List<(double, string)> lstDgRows = LRCReadDgViewData();
                    Utilities.LyricsUtilities.SaveLRCLines(fullPath, lstDgRows, bRemoveAccents, bUpperCase, bLowerCase, bRemoveNonAlphaNumeric, Tag_Tool, Tag_Title, Tag_Artist, Tag_Album, Tag_Lang, Tag_By, Tag_DPlus, bCutLines, LrcCutLinesChars, _LrcMillisecondsDigits, _myLyricsMgmt);
                    break;
                case LrcLinesSyllabesFormats.Syllabes:
                    SaveLRCSyllabes(fullPath, bRemoveAccents, bUpperCase, bLowerCase, bRemoveNonAlphaNumeric, Tag_Tool, Tag_Title, Tag_Artist, Tag_Album, Tag_Lang, Tag_By, Tag_DPlus);
                    break;
            }                                
        }
      
            
        /// <summary>
        /// Read dgView data
        /// </summary>
        /// <returns></returns>
        private List<(double, string)> LRCReadDgViewData()
        {
            string sTime;
            double time;
            string sLyric;                    
            
            object vLyric;
            object vTime;
            
            // Store rows of dgView in a list
            // the aim is to have the same procedure between frmLyricsEdit and frmMp3LyricsEdit            

            List<(double, string)> lstDgRows = new List<(double, string)>();           
            for (int i = 0; i < dgView.Rows.Count; i++)
            {
                vTime = dgView.Rows[i].Cells[COL_TIME].Value;
                vLyric = dgView.Rows[i].Cells[COL_TEXT].Value;                
                if (vTime != null && vLyric != null)
                {
                    sTime = vTime.ToString();
                    // Convert times to milliseconds (to have the same entry format with frmMp3LyricsEdit)
                    time = Mp3LyricsMgmtHelper.TimeToMs(sTime);
                    sLyric = vLyric.ToString();

                    lstDgRows.Add((time, sLyric));
                }
            }            

            return lstDgRows;      
        }

        /// <summary>
        /// Save Lyrics .lrc file format and by lines
        /// </summary>
        /// <param name="File"></param>
        /// <param name="lstDgRows"></param>
        /// <param name="bRemoveAccents"></param>
        /// <param name="bUpperCase"></param>
        /// <param name="bLowerCase"></param>
        /// <param name="bRemoveNonAlphaNumeric"></param>
        /// <param name="Tag_Tool"></param>
        /// <param name="Tag_Title"></param>
        /// <param name="Tag_Artist"></param>
        /// <param name="Tag_Album"></param>
        /// <param name="Tag_Lang"></param>
        /// <param name="Tag_By"></param>
        /// <param name="Tag_DPlus"></param>
        /// <param name="bControlLength"></param>
        /// <param name="MaxLength"></param>
        private void SaveLRCLines2(string File, List<(double, string)> lstDgRows, bool bRemoveAccents, bool bUpperCase, bool bLowerCase, bool bRemoveNonAlphaNumeric, string Tag_Tool, string Tag_Title, string Tag_Artist, string Tag_Album, string Tag_Lang, string Tag_By, string Tag_DPlus, bool bControlLength, int MaxLength)
        {
            string sLine;

            string lrcs;
            string cr = "\r\n";

            #region meta data

            // List to store lines
            List<string> lstHeaderLines = new List<string>();

            // Store meta datas
            List<string> TagsList = new List<string> { Tag_Tool, Tag_Title, Tag_Artist, Tag_Album, Tag_Lang, Tag_Album, Tag_DPlus };
            List<string> TagsNames = new List<string> { "Tool:", "Ti:", "Ar:", "Al:", "La:", "By:", "D+:" };
            string Tag;
            string TagName;
            for (int i = 0; i < TagsList.Count; i++)
            {
                Tag = TagsList[i];
                TagName = TagsNames[i];
                Tag = bRemoveAccents ? Utilities.LyricsUtilities.RemoveDiacritics(Tag) : Tag;
                Tag = bRemoveNonAlphaNumeric ? Utilities.LyricsUtilities.RemoveNonAlphaNumeric(Tag) : Tag;
                if (Tag != "")
                {
                    sLine = "[" + TagName + Tag + "]";
                    lstHeaderLines.Add(sLine);
                }
            }
            #endregion meta data

           
            // Make treatment of lyrics (same for frmLyricsEdit and frmMp3LyricsEdit)
            List<string> lstLyricsItems = Utilities.LyricsUtilities.LrcExtractDgRows(lstDgRows, _LrcMillisecondsDigits, bRemoveAccents, bUpperCase, bLowerCase, bRemoveNonAlphaNumeric, _myLyricsMgmt);

            // Store lyrics in lines (remove timestamps from lines, except for the first word)
            // [00:04.59]It's_been_a_hard_day's_night
            List<string> lstLines = Utilities.LyricsUtilities.GetLrcLines(lstLyricsItems, _LrcMillisecondsDigits);

            // Store timestamps + lyrics in lines (add spaces if not existing)
            // initial [00:04.59]It's[00:04.83]_been[00:05.05]_a[00:05.27]_hard[00:06.15]_day's[00:06.81]_night[00:08.14]
            // result [00:04.59]It's [00:04.83]_been [00:05.05]_a [00:05.27]_hard [00:06.15]_day's [00:06.81]_night [00:08.14]
            List<string> lstTimeLines = Utilities.LyricsUtilities.GetLrcTimeLines(lstLyricsItems, _LrcMillisecondsDigits);

            // Store lyrics by line and cut lines to MaxLength characters using lstTimeLines
            List<string> lstLinesCut = new List<string>();
            if (bControlLength)
            {
                lstLinesCut = Utilities.LyricsUtilities.GetLrcLinesCut(lstTimeLines, MaxLength, _LrcMillisecondsDigits);
            }


            #region send all to string 

            // Header
            lrcs = string.Empty;
            for (int i = 0; i < lstHeaderLines.Count; i++)
            {
                lrcs += lstHeaderLines[i] + cr;
            }

            // Select cut or not cut
            if (bControlLength)
            {
                // If cut lines to 32 chars
                for (int i = 0; i < lstLinesCut.Count; i++)
                {
                    lrcs += lstLinesCut[i].Replace("_", " ").Replace("] ", "]") + cr;
                }
            }
            else
            {
                // No cut
                for (int i = 0; i < lstLines.Count; i++)
                {
                    // Replace underscores located in the middle of the lyrics
                    // ex: " the_air,_(get_to_poppin')"                    
                    lrcs += lstLines[i].Replace("]_", "]").Replace(" ", "").Replace("_", " ") + cr;
                }
            }
            #endregion send all to string


            // Open file
            #region open file
            try
            {
                System.IO.File.WriteAllText(File, lrcs);
                System.Diagnostics.Process.Start(@File);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            #endregion open file
        }


        /// <summary>
        /// Save lyrics to new LRC format [01:54.60]Pa<01:55.32>ro<01:56.15>les
        /// </summary>
        /// <param name="FileName"></param>
        private void SaveLRCSyllabes(string File, bool bRemoveAccents, bool bUpperCase, bool bLowerCase, bool bRemoveNonAlphaNumeric, string Tag_Tool, string Tag_Title, string Tag_Artist, string Tag_Album, string Tag_Lang, string Tag_By, string Tag_DPlus)
        {
            string sTime;
            string sLyric;
            object vLyric;
            
            object vTime;
            long time;
            TimeSpan ts;            

            string lrcs = string.Empty;
            string cr = "\r\n";                       
            string lines = string.Empty;
           

            List<string> TagsList = new List<string> { Tag_Tool, Tag_Title, Tag_Artist, Tag_Album, Tag_Lang, Tag_Album, Tag_DPlus };
            List<string> TagsNames = new List<string> { "Tool:", "Ti:", "Ar:", "Al:", "La:", "By:", "D+:" };
            string Tag;
            string TagName;
            for (int i = 0; i < TagsList.Count; i++)
            {
                Tag = TagsList[i];
                TagName = TagsNames[i];
                Tag = bRemoveAccents ? Utilities.LyricsUtilities.RemoveDiacritics(Tag) : Tag;
                Tag = bRemoveNonAlphaNumeric ? Utilities.LyricsUtilities.RemoveNonAlphaNumeric(Tag) : Tag;
                if (Tag != "")
                    lrcs += "[" + TagName + Tag + "]" + cr;
            }

            bool bLineFeed = true;

            // new format of lrc
            // [01:54.60]La <01:55.32>petite <01:56.15>maison
            // Start line is [01:54.60]La
            // syllabes are <01:55.32>petite <01:56.15>maison

            // separate words and syllabes
            // Store results in a list
            List<(string stime, string lyric)> results = new List<(string, string)>();

            // Apply treatments choosen
            for (int i = 0; i < dgView.Rows.Count; i++)
            {                                
                vLyric = dgView.Rows[i].Cells[COL_TEXT].Value;
                vTime = dgView.Rows[i].Cells[COL_TIME].Value;  // [01:02:123]
                // vTime = dgView.Rows[i].Cells[COL_TICKS].Value;  // non !!! les ticks n'ont rien à voir avec les durées

                if (vTime != null && vLyric != null && vTime.ToString() != "" && vLyric.ToString() != "")
                {
                    sLyric = vLyric.ToString();
                    sLyric = sLyric.Replace("_", " ");

                    if (sLyric.Trim() != m_SepLine && sLyric.Trim() != m_SepParagraph)
                    {
                        // Remove chords
                        if (_myLyricsMgmt != null && _myLyricsMgmt.RemoveChordPattern != null)
                            sLyric = Regex.Replace(sLyric, _myLyricsMgmt.RemoveChordPattern, @"");

                        // Remove accents
                        sLyric = bRemoveAccents ? Utilities.LyricsUtilities.RemoveDiacritics(sLyric) : sLyric;

                        //Uppercase letters
                        sLyric = bUpperCase ? sLyric.ToUpper() : sLyric;

                        // Lowercase letters
                        sLyric = bLowerCase ? sLyric.ToLower() : sLyric;

                        // Remove non-alphanumeric chars
                        sLyric = bRemoveNonAlphaNumeric ? Utilities.LyricsUtilities.RemoveNonAlphaNumeric(sLyric) : sLyric;

                        // Save also empty lyrics
                        

                        // Translate time to right format of milliseconds
                        sTime = vTime.ToString();
                        if (_LrcMillisecondsDigits == 2)
                        {
                            time = (long)Mp3LyricsMgmtHelper.TimeToMs(sTime); // [00:01.123] => 1256
                            ts = TimeSpan.FromMilliseconds(time);
                            sTime = string.Format("{0:00}:{1:00}.{2:00}", ts.Minutes, ts.Seconds, Math.Round(ts.Milliseconds / (double)10));
                        }


                        if (bLineFeed)
                        {
                            // Format of timestamp is []                                                        
                            results.Add(("[" + sTime + "]", sLyric));
                        }
                        else
                        {
                            // Format of timestamp is <> + space before
                            results.Add(("<" + sTime + ">", sLyric));
                        }
                        
                        bLineFeed = false;

                    } 
                    else
                    {
                        bLineFeed = true;
                        
                    }
                }
            }

            string nextLyric = string.Empty;
            string nextTime = string.Empty;
            
            bool bKeepForNextSyllabe = false;
            string keepLyric = string.Empty;
            string keepTime = string.Empty;

            
            // Add a trailing "-" to syllabes without space with the next syllabe (ie it is a word composed of several syllabes)
            for (int i = 0; i < results.Count; i++)
            {
                sTime = results[i].stime;
                sLyric = results[i].lyric;

                if (i < results.Count - 1)
                {
                    nextLyric = results[i + 1].lyric;
                    nextTime = results[i + 1].stime;
                }
                else
                {
                    nextLyric = "";
                    nextTime = "";
                }
                // No trailing space in the current, no starting space in the next and the next is not a new line ([]) 
                // => this syllabe must be merged with the next one
                if (!sLyric.EndsWith(" ") && nextLyric.Length > 0 && !nextLyric.StartsWith(" ") && nextTime.IndexOf("[") == -1)
                {
                    results[i] = (results[i].stime, results[i].lyric + "-");
                }
            }

            for (int i = 0; i < results.Count; i++)
            {                                
                sTime = results[i].stime;
                sLyric = results[i].lyric;

                // Keep all syllabes ending with a trailing "-" until a syllabe without a "-"
                if (sLyric.EndsWith("-")) 
                {
                    sLyric = sLyric.Substring(0, sLyric.Length - 1).Trim();  // remove the "-"
                    keepLyric += sLyric;                                     // add syllabe to previous ones   
                    if (keepTime == "")
                        keepTime = sTime;                                    // keep only the first timestamp (beginning of the word)   

                    if (sTime.IndexOf("[") > -1)                             // if new line, store previous one
                    {
                        // Store previous line 
                        if (lrcs.Trim().Length > 0)
                        {
                            lines += lrcs + cr;
                        }
                        lrcs = "";
                    }

                    // Skip 
                    continue;
                } 
                else if (keepLyric != "") 
                {
                    // no trailing "-" and there are syllabes into keeplyric => this is the last syllabe of a word
                    bKeepForNextSyllabe = true;
                }                                                
                
                // This is the start of a new line
                if (sTime.IndexOf("[") > -1)
                {
                    // Store previous line 
                    if (lrcs.Trim().Length > 0)
                    {
                        lines += lrcs + cr;
                    }
                    // Format of timestamp is [] 
                    lrcs = sTime + sLyric.Trim();
                }
                else
                {
                    // This is a normal syllabe 
                    if (!bKeepForNextSyllabe)
                    {
                        // Format of timestamp is <> + space before
                        lrcs += " " + sTime + sLyric.Trim();
                    }
                    else
                    {
                        // this is The last syllabe of a word
                        if (keepTime.IndexOf("[") > -1)
                        {
                            // if the word stored in keeplyric was a starting line []
                            lrcs += keepTime + keepLyric + sLyric.Trim();
                        }
                        else
                        {
                            // if the word stored in keeplyric was a normal word, add a space before the <00:00.000>  
                            lrcs += " " + keepTime + keepLyric + sLyric.Trim();
                        }

                        // Reset variables used to store syllabes of a word
                        bKeepForNextSyllabe = false;
                        keepLyric = "";
                        keepTime = "";
                    }
                }
            }

            if (lrcs.Trim().Length > 0)
                lines += lrcs + cr;

            try
            {
                System.IO.File.WriteAllText(File, lines);
                System.Diagnostics.Process.Start(@File);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
       

        #endregion Save lrc


        #region Load lrc

        /// <summary>
        /// Load a text file LRC format (times stamps + lyrics)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuEditLoadLRCFile_Click(object sender, EventArgs e)
        {
            openFileDialog.Title = "Open a .lrc file";
            openFileDialog.DefaultExt = "lrc";
            openFileDialog.Filter = "lrc files|*.lrc|All files|*.*";

            // Get initial directory from midi file
            if (MIDIfileName != null || MIDIfileName != "")
                openFileDialog.InitialDirectory = Path.GetDirectoryName(MIDIfileName);


            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;

                try
                {
                    using (StreamReader sr = new StreamReader(fileName))
                    {
                        String lines = sr.ReadToEnd();
                        LoadLRCFile(lines);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("The file could not be read:" + ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Load a LRC file (times + lyrics)
        /// </summary>
        /// <param name="Source"></param>
        private void LoadLRCFile(string Source)
        {
            //bool bUpdateMode = false;
            bool bSeparatorsInGrid = false;
            bool bSeparatorsInLrc = false;
            int plTicksOn = 0;
            string plRealTime;
            string plType;
            string plNote =string.Empty; 
            string plElement;
            string chordName = "";
            string s;
            bool bSpaceBeforeSyllabes = false;

            /* Lyrics edition form
            * 
            * 0    - Ticks    Number of ticks
            * 1    - Time     Time in sec
            * 2    - Type     text, paragraph, linefeed
            * <3   - Chord    Chord name (optional if option bShowChord is true)>     
            * 3(4) - Note     Note value
            * 4(5) - Text     text        
            * 
            * Line break is '/' - cr
            * Paragraph is '\'  - par
            * Syllabe separator is '*'
            */
            Cursor.Current = Cursors.WaitCursor;

            List<(int,string, string, string,string, string)> grdRowsChords = new List<(int, string, string, string, string, string)> ();                        
            List<(int, string, string, string, string)> grdRowsNoChords = new List<(int, string, string, string, string)>();
            
            
            // Two use cases
            // 1. Newly created file through menu "add lyrics" => no linefeed: bUpdateMode = false
            // 2. Existing file with linefeeds: bUpdateMode = true

            // Check existence of separators in the gridview
            for (int i = 0; i < dgView.Rows.Count - 1; i++)
            {
                s = dgView.Rows[i].Cells[COL_TYPE].Value.ToString();
                if (s == "cr" || s == "par")
                {
                    bSeparatorsInGrid = true;
                    break;
                }
            }

            Karaboss.Lrc.SharedFramework.Lyrics lyrics = new Karaboss.Lrc.SharedFramework.Lyrics();
            lyrics.ArrangeLyrics(Source);


            // Check existence of separators in the lrc file
            for (int i = 0; i < lyrics.Count; i++)
            {
                s = lyrics[i].OriLyrics;
                if (s.Trim() == m_SepLine || s.Trim() == m_SepParagraph)
                {
                    bSeparatorsInLrc = true; break;
                }
            }

            
            int lines = lyrics.Count;
            // If separators in the lrc file, the number of lines to import is equal to the number of lyrics
            if (!bSeparatorsInLrc && !bSeparatorsInGrid)
            {
                // If no separators in the lrc file, each lyric will be followed by a separator,
                // so the number of line is twice
                lines = 2 * lyrics.Count;
            }

            // Adjust dgview rows to lyrics count
            if (dgView.Rows.Count < lines)
            {
               dgView.Rows.Add(lines - dgView.Rows.Count);
            }

            // Is there a space beween times and lyrics ?
            // like [00:02.000] hello instead of [00:02.000]hello            
           
            for (int i = 0; i < lyrics.Count; i++)
            {
                s = lyrics[i].OriLyrics;
                    
                if (bSeparatorsInLrc && (s.Trim() == m_SepLine || s.Trim() == m_SepParagraph))
                {
                    // Lyrics have separators
                    // Search for ' /' or ' \'
                    if (s.StartsWith(" " + m_SepLine) || s.StartsWith(" " + m_SepParagraph))
                        bSpaceBeforeSyllabes = true;
                    break;
                }
                else
                {
                    //Lyrics do not have separators
                    // Search for '] ' on all lyrics ?
                    if (!s.StartsWith("] "))
                        bSpaceBeforeSyllabes = true;
                    else
                    {
                        bSpaceBeforeSyllabes = false;
                        break;
                    }
                }
            }

            int row = 0;

            // Loop in lines
            for (int i = 0; i < lyrics.Count; i++)
            {
                plNote = "";
                LyricsLine lyline = lyrics[i];
                plRealTime = lyline.Timeline;
                plTicksOn = Utilities.LyricsUtilities.TimeToTicks(plRealTime, _division, _totalTicks);
                s = lyline.OriLyrics;
                    
                if (s.Length > 0 && s.StartsWith(" "))                    
                    s = bSpaceBeforeSyllabes ? s.Substring(1) : s;
                
                    
                // ====================================
                // syllabes or lines
                // ====================================
                if (s != m_SepLine && s != m_SepParagraph)
                {
                    // insert TEXT
                    plType = "text";

                    // Ticks & RealTime
                    // If ticks exist, do not change them with the ticks coming from the file
                    // If not, take the value from the lrc file
                    if (dgView.Rows[row].Cells[COL_TICKS].Value != null)
                    {
                        if (IsNumeric(dgView.Rows[row].Cells[COL_TICKS].Value.ToString()))
                        {
                            int localplTicksOn = Convert.ToInt32(dgView.Rows[row].Cells[COL_TICKS].Value);
                            if (localplTicksOn != plTicksOn)
                            {
                                if (Math.Abs(localplTicksOn - plTicksOn) == 1)
                                    plTicksOn = localplTicksOn;
                                else 
                                { 
                                    // If time value of existing row is different than the time value of lrc line
                                    // What to do ?
                                    MessageBox.Show( string.Format("Load Lrc failed. Ticks value are different, tick {0}, line {1}, lyric {2}",localplTicksOn, i, s), "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }
                        }
                    }
                        
                    // Note
                    if (dgView.Rows[row].Cells[COL_NOTE].Value == null)
                        plNote = "";
                    else
                    {
                        if (IsNumeric(dgView.Rows[row].Cells[COL_NOTE].Value.ToString()))
                            plNote = dgView.Rows[row].Cells[COL_NOTE].Value.ToString();
                    }

                    // Element
                    if (s.EndsWith("#"))
                        s = s.Substring(0, s.Length - 1);

                    // replace leading or trailing space by '_'
                    /*
                    StringBuilder sb = new StringBuilder(s);                    
                    if (s.StartsWith(@" "))
                        sb[0] = '_';
                    if (s.EndsWith(@" "))                    
                        sb[s.Length - 1] = '_';
                    s = sb.ToString();
                    */
                    // Replace all spaces including spaces inside the lyric
                    s = s.Replace(" ", "_");

                    plElement = s;

                    // If not linefeed or paragraph in the lrc file, add a separator before the line or the syllabe
                    if (!bSeparatorsInLrc && !bSeparatorsInGrid)
                    {
                        if (!bEditChords)
                        {
                            grdRowsNoChords.Add( (plTicksOn, plRealTime, "cr", "", m_SepLine) );
                            grdRowsNoChords.Add((plTicksOn, plRealTime, plType, plNote, plElement));                                
                        }
                        else
                        {
                            grdRowsChords.Add( (plTicksOn, plRealTime, "cr", "", "", m_SepLine) );
                            grdRowsChords.Add((plTicksOn, plRealTime, plType, chordName, plNote, plElement));
                        }
                    }
                    else
                    {
                        if (!bEditChords)
                            grdRowsNoChords.Add((plTicksOn, plRealTime, plType, plNote, plElement));
                        else
                            grdRowsChords.Add((plTicksOn, plRealTime, plType, chordName, plNote, plElement));
                    }

                    row++;

                }
                // ====================================
                // Linefeeds & Paragraphs
                // means that bSeparatorsInLrc is true
                // ====================================
                else if (s == m_SepLine || s == m_SepParagraph)
                {                        
                    if (s == m_SepLine)
                    {
                        plType = "cr";
                        plElement = m_SepLine;
                    }
                    else
                    {
                        plType = "par";
                        plElement = m_SepParagraph;
                    }

                    if (dgView.Rows[row].Cells[COL_TICKS].Value != null && IsNumeric(dgView.Rows[row].Cells[COL_TICKS].Value.ToString()))
                    {
                        plTicksOn = Convert.ToInt32(dgView.Rows[row].Cells[COL_TICKS].Value);
                        plRealTime = Utilities.LyricsUtilities.TicksToTime(plTicksOn, _division);
                    }

                    chordName = "";

                    // Note
                    plNote = "";

                    // When we start from scratch, ie select a track with notes and add lyrics from text, there is no linefeed
                    // So we have to unsert new rows with linefeeds                        
                    if (bSeparatorsInGrid)
                        row++;

                    if (!bEditChords)
                        grdRowsNoChords.Add((plTicksOn, plRealTime, plType, plNote, plElement));
                    else
                        grdRowsChords.Add((plTicksOn, plRealTime, plType, chordName, plNote, plElement));

                }
            
            
            }
            

            // Write grdRowsChords in the grid
            if (!bEditChords)
            {
                for (int i = 0; i < grdRowsNoChords.Count; i++)
                {
                    dgView.Rows[i].Cells[COL_TICKS].Value = grdRowsNoChords[i].Item1;
                    dgView.Rows[i].Cells[COL_TIME].Value = grdRowsNoChords[i].Item2;
                    dgView.Rows[i].Cells[COL_TYPE].Value = grdRowsNoChords[i].Item3;
                    dgView.Rows[i].Cells[COL_NOTE].Value = grdRowsNoChords[i].Item4;
                    dgView.Rows[i].Cells[COL_TEXT].Value = grdRowsNoChords[i].Item5;
                }
            }
            else
            {
                for (int i = 0; i < grdRowsChords.Count; i++ )
                {
                    dgView.Rows[i].Cells[COL_TICKS].Value = grdRowsChords[i].Item1;
                    dgView.Rows[i].Cells[COL_TIME].Value = grdRowsChords[i].Item2;
                    dgView.Rows[i].Cells[COL_TYPE].Value = grdRowsChords[i].Item3;
                    dgView.Rows[i].Cells[COL_CHORD].Value = grdRowsChords[i].Item4;
                    dgView.Rows[i].Cells[COL_NOTE].Value = grdRowsChords[i].Item5;
                    dgView.Rows[i].Cells[COL_TEXT].Value = grdRowsChords[i].Item6;
                }
            }


            //Load modification into local list of lyrics
            localplLyrics = LoadModifiedLyrics();
            PopulateTextBox(localplLyrics);

            // Color separators
            ColorSepRows();

            // File was modified
            FileModified();

            Cursor.Current = Cursors.Default;
        }

        #endregion Load lrc


        #endregion


        #region menus


        /// <summary>
        /// Quit windowx
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuFileQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Menu: load times of a melody track to help lyrics entering
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuEditLoadTrack_Click(object sender, EventArgs e)
        {
            DialogResult dr; // = new DialogResult();
            frmLyricsSelectTrack TrackDialog = new frmLyricsSelectTrack(sequence1);
            dr = TrackDialog.ShowDialog();

            if (dr == System.Windows.Forms.DialogResult.Cancel)
                return;

            // Get track number for melody
            // -1 if no track
            melodytracknum = TrackDialog.TrackNumber - 1;

            if (melodytracknum == -1)
            {
                //MessageBox.Show("No track found for the melody", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Information);
                dgView.Rows.Clear();
                return;
            }

            LoadTrackGuide();
        }

        /// <summary>
        /// Select track audio guide & lyrics format
        /// </summary>
        private void LoadTrackGuide()
        {

            PopulateDataGridViewTrack(melodytracknum);
            localplLyrics = LoadModifiedLyrics();
            PopulateTextBox(localplLyrics);
        }

        /// <summary>
        /// Menu : about
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuHelpAbout_Click(object sender, EventArgs e)
        {
            frmAboutDialog dlg = new frmAboutDialog();
            dlg.ShowDialog();
        }


        #endregion menus


        #region Save

        /// <summary>
        /// Menu File Save
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuFileSave_Click(object sender, EventArgs e)
        {
            //Load modification into local list of lyrics
            // Format lyric by including chord
            localplLyrics = LoadModifiedLyrics();

            // Display new lyrics in frmPlayer
            _myLyricsMgmt.ChordsOriginatedFrom = MidiLyricsMgmt.ChordsOrigins.Lyrics;
            ReplaceLyrics(localplLyrics);

            // save file
            SaveFileProc();
        }

        /// <summary>
        /// Menu File Save as
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuFileSaveAs_Click(object sender, EventArgs e)
        {
            SaveAsFileProc();
        }


        /// <summary>
        /// File was modified
        /// </summary>
        private void FileModified()
        {
            bfilemodified = true;
            string fName = Path.GetFileName(MIDIfileName);
            if (fName != null && fName != "")
            {
                string fExt = Path.GetExtension(fName);             // Extension
                fName = Path.GetFileNameWithoutExtension(fName);    // name without extension

                string fShortName = fName.Replace("*", "");
                if (fShortName == fName)
                    fName += "*";

                fName += fExt;
                SetTitle(fName);
            }
        }

        private void SaveFileProc()
        {
            string fName = Path.GetFileName(MIDIfileName);
            string fPath = Path.GetDirectoryName(MIDIfileName);

            if (fPath == null || fPath == "" || fName == null || fName == "")
            {
                SaveAsFileProc();
                return;
            }

            string fullName = fPath + "\\" + fName;
            if (File.Exists(fullName) == false)
            {
                SaveAsFileProc();
                return;
            }

            if (Application.OpenForms.OfType<frmPlayer>().Count() > 0)
            {
                frmPlayer frmPlayer = Utilities.FormUtilities.GetForm<frmPlayer>();
                frmPlayer.InitSaveFile(fullName);

                // Reset title
                bfilemodified = false;
                string displayName = fName;
                SetTitle(displayName);
            }
        }

        /// <summary>
        /// Function: save as file
        /// </summary>
        private void SaveAsFileProc()
        {
            string fName = "New.kar";
            string fPath = Path.GetDirectoryName(MIDIfileName);
            string fullName;
            string defName;


            #region search name

            if (fPath == null || fPath == "")
            {
                if (Directory.Exists(CreateNewMidiFile._DefaultDirectory))
                    fPath = CreateNewMidiFile._DefaultDirectory;
                else
                    fPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            }
            else
            {
                fName = Path.GetFileName(MIDIfileName);
            }


            string fullPath = fPath + "\\" + fName;
            string defExt = Path.GetExtension(fName);                           // Extension
            fullName = Utilities.Files.FindUniqueFileName(fullPath);    // Add (2), (3) etc.. if necessary    
            defName = Path.GetFileNameWithoutExtension(fullName);               // Default name to propose to dialog

            #endregion search name                   

            string defFilter = "MIDI files (*.mid)|*.mid|Kar files (*.kar)|*.kar|All files (*.*)|*.*";
            if (defExt == ".kar")
                defFilter = "Kar files (*.kar)|*.kar|MIDI files (*.mid)|*.mid|All files (*.*)|*.*";

            saveMidiFileDialog.Title = "Save MIDI file";
            saveMidiFileDialog.Filter = defFilter;
            saveMidiFileDialog.DefaultExt = defExt;
            saveMidiFileDialog.InitialDirectory = @fPath;
            saveMidiFileDialog.FileName = defName;

            if (saveMidiFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveMidiFileDialog.FileName;

                MIDIfileName = fileName;

                if (Application.OpenForms.OfType<frmPlayer>().Count() > 0)
                {
                    frmPlayer frmPlayer = Utilities.FormUtilities.GetForm<frmPlayer>();
                    frmPlayer.InitSaveFile(fileName);

                    bfilemodified = false;
                    string displayName = Path.GetFileName(MIDIfileName);
                    SetTitle(displayName);
                }
            }
        }

        #endregion Save


        #region switch to other available format
        private void btnDisplayOtherLyrics_Click(object sender, EventArgs e)
        {
            int l = _myLyricsMgmt.lstpllyrics[0].Count;
            int t = _myLyricsMgmt.lstpllyrics[1].Count;

            if (l > 0 && t > 0)
            {
                // "Two types of lyrics format are available in this file: LYRIC and TEXT"
                string tx = Karaboss.Resources.Localization.Strings.TwoTypesOfLyrics;
                if (_myLyricsMgmt.LyricType == LyricTypes.Text)
                {
                    // "The current format is {0} ({1} lyrics)"
                    tx += string.Format("\n\n" + Karaboss.Resources.Localization.Strings.CurrentLyricFormatIs, "TEXT", t);
                    // "Would you like to change to {0} format? ({1} lyrics)"
                    tx += string.Format("\n\n" + Karaboss.Resources.Localization.Strings.WantToChangeFormatLyrics, "LYRIC", l);
                }
                else if (_myLyricsMgmt.LyricType == LyricTypes.Lyric)
                {
                    tx += string.Format("\n\n" + Karaboss.Resources.Localization.Strings.CurrentLyricFormatIs, "LYRIC", l);
                    tx += string.Format("\n\n" + Karaboss.Resources.Localization.Strings.WantToChangeFormatLyrics, "TEXT", t);
                }

                if (MessageBox.Show(tx, "Karaboss", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _myLyricsMgmt.LyricType = ((_myLyricsMgmt.LyricType == LyricTypes.Text) ? LyricTypes.Lyric : LyricTypes.Text);

                    if (_myLyricsMgmt.LyricType == LyricTypes.Lyric)
                    {
                        localplLyrics = _myLyricsMgmt.lstpllyrics[0];
                        TextLyricFormat = LyricFormats.Lyric;
                        optFormatLyrics.Checked = true;
                    }
                    else if (_myLyricsMgmt.LyricType == LyricTypes.Text)
                    {
                        localplLyrics = _myLyricsMgmt.lstpllyrics[1];
                        TextLyricFormat = LyricFormats.Text;
                        optFormatText.Checked = true;
                    }

                    Cursor.Current = Cursors.WaitCursor;

                    // Populate Grid & Textbox
                    DisplayOtherFormat();

                    // Display new lyrics in frmPlayer
                    ReplaceLyrics(localplLyrics);

                    Cursor.Current = Cursors.Default;
                }
            }
        }

        private void DisplayOtherFormat()
        {
            InitGridView();
            txtResult.Text = string.Empty;

            // File was modified
            FileModified();

            // populate cells with existing Lyrics or notes
            PopulateDataGridView(localplLyrics);
            // populate viewer
            PopulateTextBox(localplLyrics);


            // Adapt height of cells to duration between syllabes
            HeightsToDurations();

        }

        #endregion switch to other available format


        #region Tags


        /// <summary>
        /// Display tags
        /// </summary>
        private void DisplayTags()
        {
            string cr = Environment.NewLine;
            //int i = 0;

            // Classic Karaoke Midi tags
            /*
            @K	(multiple) K1: FileType ex MIDI KARAOKE FILE, K2: copyright of Karaoke file
            @L	(single) Language	FRAN, ENGL        
            @W	(multiple) Copyright (of Karaoke file, not song)        
            @T	(multiple) Title1 @T<title>, Title2 @T<author>, Title3 @T<copyright>		
            @I	Information  ex Date(of Karaoke file, not song)
            @V	(single) Version ex 0100 ?             
            */
            if (sequence1.KTag != null)
            {
                for (int i = 0; i < sequence1.KTag.Count; i++)
                {
                    txtKTag.Text += sequence1.KTag[i] + cr;
                }
                for (int i = 0; i < sequence1.WTag.Count; i++)
                {
                    txtWTag.Text += sequence1.WTag[i] + cr;
                }
                for (int i = 0; i < sequence1.TTag.Count; i++)
                {
                    txtTTag.Text += sequence1.TTag[i] + cr;
                }
                for (int i = 0; i < sequence1.ITag.Count; i++)
                {
                    txtITag.Text += sequence1.ITag[i] + cr;
                }
                for (int i = 0; i < sequence1.VTag.Count; i++)
                {
                    txtVTag.Text += sequence1.VTag[i] + cr;
                }
                for (int i = 0; i < sequence1.LTag.Count; i++)
                {
                    txtLTag.Text += sequence1.LTag[i] + cr;
                }
            }
        }

        /// <summary>
        /// Save tags
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveTags_Click(object sender, EventArgs e)
        {
            bool bModified = false;
            string tx; // = string.Empty;         

            string[] S;
            string newline; // = string.Empty;

            sequence1.ITag.Clear();
            sequence1.KTag.Clear();
            sequence1.LTag.Clear();
            sequence1.TTag.Clear();
            sequence1.VTag.Clear();
            sequence1.WTag.Clear();

            tx = txtITag.Text.Trim();
            S = tx.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in S)
            {
                newline = line.Trim();
                if (newline != "")
                    sequence1.ITag.Add(line.Trim());
            }
            tx = txtKTag.Text.Trim();
            S = tx.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in S)
            {
                newline = line.Trim();
                if (newline != "")
                    sequence1.KTag.Add(line.Trim());
            }
            tx = txtLTag.Text.Trim();
            S = tx.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in S)
            {
                newline = line.Trim();
                if (newline != "")
                    sequence1.LTag.Add(line.Trim());
            }
            tx = txtTTag.Text.Trim();
            S = tx.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in S)
            {
                newline = line.Trim();
                if (newline != "")
                    sequence1.TTag.Add(line.Trim());
            }
            tx = txtVTag.Text.Trim();
            S = tx.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in S)
            {
                newline = line.Trim();
                if (newline != "")
                    sequence1.VTag.Add(line.Trim());
            }
            tx = txtWTag.Text.Trim();
            S = tx.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in S)
            {
                newline = line.Trim();
                if (newline != "")
                    sequence1.WTag.Add(line.Trim());
            }          

            if (sequence1.ITag.Count != 0 || sequence1.KTag.Count != 0 || sequence1.LTag.Count != 0 || sequence1.TTag.Count != 0 || sequence1.VTag.Count != 0 || sequence1.WTag.Count != 0)
            {
                bModified = true;
            }


            if (bModified == true)
            {
                AddTags();

                if (Application.OpenForms.OfType<frmPlayer>().Count() > 0)
                {
                    frmPlayer frmPlayer = Utilities.FormUtilities.GetForm<frmPlayer>();                    
                    frmPlayer.FileModified();
                }
                MessageBox.Show("Tags saved successfully", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Information);               
            }
        }

        /// <summary>
        /// Add tags to midi file
        /// </summary>
        private void AddTags()
        {
            int i; // = 0;

            // @#Title      Title
            // @#Artist     Artist
            // @#Album      Album
            // @#Copyright  Copyright
            // @#Date       Date
            // @#Editor     Editor
            // @#Genre      Genre        
            // @#Evaluation Evaluation
            // @#Comment    Comment

            // Remove prev tags
            Track track = sequence1.tracks[0];
            track.RemoveTagsEvent("@#");

            string Comment = "@#Comment=" + sequence1.TagComment;
            AddTag(Comment);

            string Evaluation = "@#Evaluation=" + sequence1.TagEvaluation;
            AddTag(Evaluation);

            string Genre = "@#Genre=" + sequence1.TagGenre;
            AddTag(Genre);

            string Editor = "@#Editor=" + sequence1.TagEditor;
            AddTag(Editor);

            string Date = "@#Date=" + sequence1.TagDate;
            AddTag(Date);

            string Copyright = "@#Copyright=" + sequence1.TagCopyright;
            AddTag(Copyright);

            string Album = "@#Album=" + sequence1.TagAlbum;
            AddTag(Album);

            string Artist = "@#Artist=" + sequence1.TagArtist;
            AddTag(Artist);

            string Title = "@#Title=" + sequence1.TagTitle;
            AddTag(Title);

            // Classic Karaoke tags
            string tx; // = string.Empty;
            track.RemoveTagsEvent("@I");
            track.RemoveTagsEvent("@K");
            track.RemoveTagsEvent("@L");
            track.RemoveTagsEvent("@T");
            track.RemoveTagsEvent("@V");
            track.RemoveTagsEvent("@W");

            for (i = sequence1.ITag.Count - 1; i >= 0; i--)
            {
                tx = "@I" + sequence1.ITag[i];
                AddTag(tx);
            }
            for (i = sequence1.KTag.Count - 1; i >= 0; i--)
            {
                tx = "@K" + sequence1.KTag[i];
                AddTag(tx);
            }
            for (i = sequence1.LTag.Count - 1; i >= 0; i--)
            {
                tx = "@L" + sequence1.LTag[i];
                AddTag(tx);
            }
            for (i = sequence1.TTag.Count - 1; i >= 0; i--)
            {
                tx = "@T" + sequence1.TTag[i];
                AddTag(tx);
            }
            for (i = sequence1.VTag.Count - 1; i >= 0; i--)
            {
                tx = "@V" + sequence1.VTag[i];
                AddTag(tx);
            }
            for (i = sequence1.WTag.Count - 1; i >= 0; i--)
            {
                tx = "@W" + sequence1.WTag[i];
                AddTag(tx);
            }
        }

        /// <summary>
        /// Insert Tag at tick 0
        /// </summary>
        /// <param name="strTag"></param>
        private void AddTag(string strTag)
        {
            Track track = sequence1.tracks[0];
            int currentTick = 0;
            string currentElement = strTag;

            // Transforme en byte la nouvelle chaine
            byte[] newdata = new byte[currentElement.Length];
            for (int u = 0; u < newdata.Length; u++)
            {
                newdata[u] = (byte)currentElement[u];
            }

            MetaMessage mtMsg;

            mtMsg = new MetaMessage(MetaType.Text, newdata);

            // Insert new message
            track.Insert(currentTick, mtMsg);
        }

        #endregion


        #region Text import export

        /// <summary>
        /// Save text with all separators
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuSaveAsText_Click(object sender, EventArgs e)
        {
            #region select filename
            string defExt = ".txt";
            string fName = "New" + defExt;
            string fPath = Path.GetDirectoryName(MIDIfileName);

            string fullName; // = string.Empty;
            string defName; // = string.Empty;

            #region search name
            if (fPath == null || fPath == "")
            {
                if (Directory.Exists(CreateNewMidiFile._DefaultDirectory))
                    fPath = CreateNewMidiFile._DefaultDirectory;
                else
                    fPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            }
            else
            {
                fName = Path.GetFileName(MIDIfileName);
            }

                                                         // Extension forced to text                       
            string fullPath = fPath + "\\" + fName;
            fullName = Utilities.Files.FindUniqueFileName(fullPath);            // Add (2), (3) etc.. if necessary    
            defName = Path.GetFileNameWithoutExtension(fullName);               // Default name to propose to dialog

            #endregion search name                   

            string defFilter = "TEXT files (*.txt)|*.txt|All files (*.*)|*.*";

            saveMidiFileDialog.Title = "Save to TEXT format";
            saveMidiFileDialog.Filter = defFilter;
            saveMidiFileDialog.DefaultExt = defExt;
            saveMidiFileDialog.InitialDirectory = @fPath;
            saveMidiFileDialog.FileName = defName;

            if (saveMidiFileDialog.ShowDialog() != DialogResult.OK)
                return;

            #endregion

            string FileName = saveMidiFileDialog.FileName;

            SaveTextWithSep(FileName);
        }

        private void SaveTextWithSep(string File)
        {            
            string sLyric;
            string sLine = string.Empty;
            string sType;
            object vLyric;
            object vTime;
            object vType;
            string lrcs = string.Empty;
            string cr = "\r\n";            

            bool bStartLine = true;

            // Save syllabe by syllabe
            for (int i = 0; i < dgView.Rows.Count; i++)
            {
                vLyric = dgView.Rows[i].Cells[COL_TEXT].Value;
                vTime = dgView.Rows[i].Cells[COL_TIME].Value;
                vType = dgView.Rows[i].Cells[COL_TYPE].Value;

                if (vTime != null && vLyric != null && vType != null)
                {
                    sLyric = vLyric.ToString().Trim();
                    sType = vType.ToString().Trim();

                    if (sLyric == "")
                        sLyric = "_~~";

                    if (sLyric != "" && sType != "cr" && sType != "par")
                    {
                        if (bStartLine)
                        {
                            sLine = sLyric + _InternalSepLines;
                            bStartLine = false;
                        }
                        else
                        {
                            // Line continuation
                            sLine += sLyric + _InternalSepLines;
                        }
                    }
                    else if (sType == "cr" || sType == "par")
                    {
                        // Save current line
                        if (sLine != "")
                        {
                            if (sType == "cr")
                                lrcs += sLine + cr;
                            else
                                lrcs += sLine + cr + cr;
                        }

                        // Reset all
                        bStartLine = true;
                        sLine = string.Empty;
                    }
                }
            }

            // Save last line
            if (sLine != "")
            {
                lrcs += sLine; // + cr;
            }


            // * "Oh!_la!_la_la!_vie!_en!_rose!\r\nLe!_rose!_qu'on!_nous!_pro!pose!\r\nD'avoir!_les!_quan!ti!tés!_d'choses!\r\nQui!_donnent!_en!vie!_d'autre!_chose!\r\nAie!_on!_nous!_fait!_croire!\r\nQue!_le!_bonheur!_c'est!_d'a!voir!\r\nDe!_l'avoir!_plein!_nos!_ar!moires!\r\nDérisions!_de!_nous!_dé!ri!soires!\r\ncar...!\r\nFoule!_sen!ti!men!tale!\r\nOn_a!_soif!_d'idéal!\r\nAttiré!_par!_les!_é!toiles,!_les!_voiles!\r\nQue!_des!_choses!_pas!_com!mer!ciales!\r\nFoule!_sen!ti!men!tale!\r\nIl!_faut!_voir!_comme!_on!_nous!_parle!\r\nComme!_on!_nous!_parle!\r\nIl!_se!_dé!ga!ge!\r\nDe!_ces!_cartons!_d'em!bal!lage!\r\nDes!_gens!_lavés!_hors!_d'u!sage!\r\nEt!_triste!_et!_sans_au!cun!_a!van!tage!\r\nOn!_nous!_in!flige!\r\nDes!_désirs!_qui!_nous!_af!fligent!\r\nOn!_nous!_prend!_faut!_pas!_dé!con!ner!_dès!_qu'on!_est!_né!\r\nPour!_des!_cons!_alors!_qu'on!_est!\r\nDes!_foule!_sen!ti!men!tale!\r\nOn_a!_soif!_d'idéal!\r\nAttiré!_par!_les!_é!toiles,!_les!_voiles!\r\nQue!_des!_choses!_pas!_com!mer!ciales!\r\nFoule!_sen!ti!men!tale!\r\nIl!_faut

            lrcs = lrcs.Replace(_InternalSepLines + "_", " ");
            lrcs = lrcs.Replace("_" + _InternalSepLines, " ");
            lrcs = lrcs.Replace(_InternalSepLines + cr, cr);
            lrcs = lrcs.Replace(" " + cr, cr);
            lrcs = lrcs.Replace(_InternalSepLines, "*");

            // Delete last sep
            if (lrcs.Length > 0 && (lrcs.Substring(lrcs.Length - 1, 1) == "*"))
            {
                lrcs = lrcs.Substring(0, lrcs.Length - 1);
            }

            try
            {
                System.IO.File.WriteAllText(File, lrcs);
                System.Diagnostics.Process.Start(@File);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Menu: load a text file containing the melody
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuEditLoadMelodyText_Click(object sender, EventArgs e)
        {
            openFileDialog.Title = "Open a text file";
            openFileDialog.DefaultExt = "txt";
            openFileDialog.Filter = "Txt files|*.txt|All files|*.*";
            if (MIDIfileName != null || MIDIfileName != "")
                openFileDialog.InitialDirectory = Path.GetDirectoryName(MIDIfileName);

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;
                try
                {
                    using (StreamReader sr = new StreamReader(fileName))
                    {
                        String lines = sr.ReadToEnd();
                        LoadTextFile(lines);
                    }
                }
                catch (Exception errl)
                {
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(errl.Message);
                }
            }
        }

        /// <summary>
        /// Use case : create from scratch, no lyrics at all 
        /// 1. select a track with notes
        /// 2. Loadt text file
        /// </summary>
        /// <param name="source"></param>
        private void LoadTextFile(string source)
        {
            bool bUpdateMode = false;
            string s;
            string chordName = "";

            // Split into peaces of words
            source = source.Replace("\r\n", " <cr> ");
            source = source.Replace(" <cr>  <cr> ", " <cr> <cr> ");
            source = source.Replace("<cr> <cr>", "<par>");

            // Split syllabes 
            // go*ing => go# ing
            source = source.Replace("*", "# ");

            // Separate words by space
            string[] stringSeparators = new string[] { " " };
            string[] result = source.Split(stringSeparators, StringSplitOptions.None);
            for (int i = 0; i < result.Length; i++)
            {
                if (result[i] == "")
                    result[i] = "<cr>";
            }

            // Check existence of cr or par
            for (int i = 0; i < dgView.Rows.Count - 1; i++)
            {
                s = dgView.Rows[i].Cells[COL_TYPE].Value.ToString();
                if (s == "cr" || s == "par")
                {
                    bUpdateMode = true;
                    break;
                }
            }

            // Add missing lines before
            int addl = result.Length - dgView.Rows.Count;
            if (addl > 0)
            {
                for (int i = 0; i < addl; i++)
                {
                    dgView.Rows.Add();
                }
            }

            // write lyrics on each line            
            int plTicksOn = 0;
            string plRealTime = "00:00.00";
            int plNote = 0;
            string plType;
            string plElement;


            for (int i = 0; i < result.Length; i++)
            {
                s = result[i];

                if (i < dgView.Rows.Count)
                {
                    if (s != "<cr>" && s != "<par>")
                    {
                        // insert TEXT
                        plType = "text";

                        if (dgView.Rows[i].Cells[COL_TICKS].Value == null)
                        {
                            dgView.Rows[i].Cells[COL_TICKS].Value = plTicksOn;
                            plRealTime = Utilities.LyricsUtilities.TicksToTime(plTicksOn, _division);
                            dgView.Rows[i].Cells[COL_TIME].Value = plRealTime;
                        }
                        else
                        {
                            plTicksOn = 0;
                            if (IsNumeric(dgView.Rows[i].Cells[COL_TICKS].Value.ToString()))
                                plTicksOn = Convert.ToInt32(dgView.Rows[i].Cells[COL_TICKS].Value);
                        }

                        dgView.Rows[i].Cells[COL_TYPE].Value = plType;

                        if (dgView.Rows[i].Cells[COL_NOTE].Value == null)
                            dgView.Rows[i].Cells[COL_NOTE].Value = 0;

                        if (s.EndsWith("#"))
                            plElement = s.Substring(0, s.Length - 1);
                        else
                            plElement = s + "_";

                        dgView.Rows[i].Cells[COL_TEXT].Value = plElement;

                    }
                    else if (s == "<cr>" || s == "<par>")
                    {
                        if (s == "<cr>")
                        {
                            // insert <CR>;
                            plType = "cr";
                            plElement = m_SepLine;
                        }
                        else
                        {
                            plType = "par";
                            plElement = m_SepParagraph;
                        }

                        if (dgView.Rows[i].Cells[COL_TICKS].Value != null && IsNumeric(dgView.Rows[i].Cells[COL_TICKS].Value.ToString()))
                        {
                            plTicksOn = Convert.ToInt32(dgView.Rows[i].Cells[COL_TICKS].Value);
                            plRealTime = Utilities.LyricsUtilities.TicksToTime(plTicksOn, _division);
                        }
                        if (dgView.Rows[i].Cells[COL_NOTE].Value == null)
                            dgView.Rows[i].Cells[COL_NOTE].Value = 0;

                        if (dgView.Rows[i].Cells[COL_NOTE].Value != null && IsNumeric(dgView.Rows[i].Cells[COL_NOTE].Value.ToString()))
                            plNote = Convert.ToInt32(dgView.Rows[i].Cells[COL_NOTE].Value);


                        // When we start from scratch, ie select a track with notes and add lyrics from text, there is no linefeed
                        // So we have to unsert new rows with linefeeds
                        // Insert new row 
                        if (!bUpdateMode)
                        {
                            if (!bEditChords)
                                dgView.Rows.Insert(i, plTicksOn, plRealTime, plType, plNote.ToString(), plElement, plElement);
                            else
                                dgView.Rows.Insert(i, plTicksOn, plRealTime, plType, chordName, plNote.ToString(), plElement, plElement);
                        }
                        else
                        {

                            // other use case: When we want to update the lyrics, the linefeeds already exist
                            // So we just have to update the lines
                            // Udpdate

                            dgView.Rows[i].Cells[COL_TICKS].Value = plTicksOn;
                            dgView.Rows[i].Cells[COL_TIME].Value = plRealTime;
                            dgView.Rows[i].Cells[COL_TYPE].Value = plType;
                            dgView.Rows[i].Cells[COL_NOTE].Value = plNote;
                            dgView.Rows[i].Cells[COL_TEXT].Value = plElement;

                        }
                    }
                }
            }

            //Load modification into local list of lyrics
            localplLyrics = LoadModifiedLyrics();
            PopulateTextBox(localplLyrics);

            // Color separators
            ColorSepRows();

            // File was modified
            FileModified();
        }

        #endregion Text import export


        #region Tracks

        /// <summary>
        /// Load list of tracks
        /// </summary>
        /// <param name="sequence1"></param>
        private void LoadTracks(Sequence sequence1)
        {
            string name;
            string item;

            //item = "No melody track";
            item = Karaboss.Resources.Localization.Strings.NoMelodyTrack;
            cbSelectTrack.Items.Add(item);

            for (int i = 0; i < sequence1.tracks.Count; i++)
            {
                Track track = sequence1.tracks[i];

                if (track.Name == null)
                    name = "";
                else
                    name = track.Name;

                int patch = track.ProgramChange;
                if (patch > 127)
                    patch = 0;
                item = (i + 1).ToString("00") + " [" + track.MidiChannel.ToString("00") + "] - " + name + " - (" + MidiFile.PCtoInstrument(track.ProgramChange) + ")";
                cbSelectTrack.Items.Add(item);
            }

            cbSelectTrack.SelectedIndex = 0;

        }

        /// <summary>
        /// Display the selected track for le melody
        /// </summary>
        private void DisplaySelectedTrack()
        {
            try
            {
                //cbSelectTrack.SelectedIndex = melodytracknum + 1;
                cbSelectTrack.SelectedIndex = lyricstracknum + 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void cbSelectTrack_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbSelectTrack.SelectedIndex == 0)
                return;
            
            int newmelodytracknum = cbSelectTrack.SelectedIndex - 1;
            
            //if (newmelodytracknum != -1 && newmelodytracknum != melodytracknum && localplLyrics != null)
            //{
                melodytracknum = cbSelectTrack.SelectedIndex - 1;
                melodyTrack = sequence1.tracks[melodytracknum];

                Cursor.Current = Cursors.WaitCursor;

                InitGridView();
                // populate cells with existing Lyrics or notes
                PopulateDataGridView(localplLyrics);
                // populate viewer
                PopulateTextBox(localplLyrics);

                HeightsToDurations();

                // Color separators
                ColorSepRows();

                Cursor.Current = Cursors.Default;
            //}
        }

        #endregion


        #region TxtResult


        private void InitTxtResult()
        {
            txtResult.Font = _lyricseditfont;
        }

        #endregion TxtResult


        #region Text

        /// <summary>
        /// Check if times in dgview are greater than previous ones
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private bool CheckTimes(out int line)
        {
            long time;
            long lasttime = -1;

            for (int i = 0; i < dgView.RowCount; i++)
            {
                if (dgView.Rows[i].Cells[0].Value == null || dgView.Rows[i].Cells[0].Value.ToString() == "") continue;

                time = Convert.ToInt32(dgView.Rows[i].Cells[0].Value);

                if (time > lasttime)
                    lasttime = time;
                else if (time < lasttime)
                {
                    line = i + 1;
                    return false;
                }
            }
            line = -1;
            return true;
        }

        #endregion Text

    }
}

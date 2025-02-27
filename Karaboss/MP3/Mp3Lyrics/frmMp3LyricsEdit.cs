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
using Karaboss.Resources.Localization;
using Karaboss.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using TagLib;
using TagLib.Id3v2;

namespace Karaboss.Mp3.Mp3Lyrics
{
    public partial class frmMp3LyricsEdit: Form
    {
        bool bfilemodified = false;


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

        // txtResult, BtnFontPlus
        private Font _lyricseditfont;
        private float _fontSize = 8.25f;

        private string _filename;

        // Manage locally lyrics
        List<List<keffect.KaraokeEffect.kSyncText>> localSyncLyrics;
        


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="FileName"></param>
        public frmMp3LyricsEdit(string FileName)
        {
            InitializeComponent();

            _filename = FileName;

            LoadOptions();

            // Inits
            SetTitle(Path.GetFileName(_filename));
            SetOriginOfLyrics();
            InitTxtResult();
            InitGridView();

            PopulateDataGridView();
            localSyncLyrics = GetUniqueSource();
            PopulateTextBox(localSyncLyrics);
        }

        #region Form Load and Close

        /// <summary>
        /// Set Title of the form
        /// </summary>
        private void SetTitle(string displayName)
        {
            displayName = displayName.Replace("__", ": ");
            displayName = displayName.Replace("_", " ");
            Text = "Karaboss - " + Strings.EditWords + " - " + displayName;
        }

        private void frmMp3LyricsEdit_Load(object sender, EventArgs e)
        {
            // Récupère la taille et position de la forme

            // If window is maximized
            if (Properties.Settings.Default.frmMp3LyricsEditMaximized)
            {

                Location = Properties.Settings.Default.frmMp3LyricsEditLocation;
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                Location = Properties.Settings.Default.frmMp3LyricsEditLocation;
                // Verify if this windows is visible in extended screens
                Rectangle rect = new Rectangle(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);
                foreach (Screen screen in Screen.AllScreens)
                    rect = Rectangle.Union(rect, screen.Bounds);

                if (Location.X > rect.Width)
                    Location = new Point(0, Location.Y);
                if (Location.Y > rect.Height)
                    Location = new Point(Location.X, 0);

                Size = Properties.Settings.Default.frmMp3LyricsEditSize;
            }
        }

        private void frmMp3LyricsEdit_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bfilemodified == true)
            {
                //string tx = "Le fichier a été modifié, voulez-vous l'enregistrer ?";
                String tx = Karaboss.Resources.Localization.Strings.QuestionSavefile;

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
                    //localplLyrics = LoadModifiedLyrics();

                    // Display new lyrics in frmPlayer
                    //ReplaceLyrics(localplLyrics);

                    // Save file
                    //SaveFileProc();
                    return;
                }
                else
                {
                    if (Application.OpenForms.OfType<frmMp3Player>().Count() > 0)
                    {
                        frmMp3Player frmMp3Player = Utilities.FormUtilities.GetForm<frmMp3Player>();
                        frmMp3Player.bfilemodified = false;
                    }
                }
            }

            // enregistre la taille et la position de la forme
            // Copy window location to app settings                
            if (WindowState != FormWindowState.Minimized)
            {
                if (WindowState == FormWindowState.Maximized)
                {
                    Properties.Settings.Default.frmMp3LyricsEditLocation = RestoreBounds.Location;
                    Properties.Settings.Default.frmMp3LyricsEditMaximized = true;

                }
                else if (WindowState == FormWindowState.Normal)
                {
                    Properties.Settings.Default.frmMp3LyricsEditLocation = Location;
                    Properties.Settings.Default.frmMp3LyricsEditSize = Size;
                    Properties.Settings.Default.frmMp3LyricsEditMaximized = false;
                }

                //SaveOptions();

                // Save settings
                Properties.Settings.Default.Save();
            }

            Dispose();
        }

        private void frmMp3LyricsEdit_Resize(object sender, EventArgs e)
        {
            ResizeMe();
        }

        private void ResizeMe()
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

        #endregion Form Load and Close


        #region Populate gridview


        private List<List<keffect.KaraokeEffect.kSyncText>> GetUniqueSource()
        {            
            // Origin = synchronized lyrics frame            
            if (Mp3LyricsMgmtHelper.MySyncLyricsFrame != null)
            {
                return Mp3LyricsMgmtHelper.GetKEffectSyncLyrics(Mp3LyricsMgmtHelper.MySyncLyricsFrame);       
            }
            else if (Mp3LyricsMgmtHelper.SyncLyrics != null)
            {
                return Mp3LyricsMgmtHelper.SyncLyrics;
            }

            return null;
        }


        /// <summary>
        /// Populate gridview with lyrics
        /// </summary>
        private void PopulateDataGridView()
        {
            // Origine = lrc            
            List<List<keffect.KaraokeEffect.kSyncText>> SyncLyrics = Mp3LyricsMgmtHelper.SyncLyrics;

            // Origin = synchronized lyrics frame
            SynchronisedLyricsFrame SynchedLyrics = Mp3LyricsMgmtHelper.MySyncLyricsFrame;
            
            if (SynchedLyrics != null)
            {
                for (int i = 0; i < SynchedLyrics.Text.Count(); i++)
                {
                    // Put "/" everywhere
                    string text = SynchedLyrics.Text[i].Text;
                    text = text.Replace("\r\n", m_SepLine);
                    text = text.Replace("\r", m_SepLine);
                    text = text.Replace("\n", m_SepLine);
                    text = text.Replace(" ", "_");

                    long time = SynchedLyrics.Text[i].Time;
                    dgView.Rows.Add(time, text);
                }
            }
            else if (SyncLyrics != null)
            {
                
                // For each line
                for (int j = 0; j < SyncLyrics.Count; j++)
                {
                    // For each syllabes
                    for (int i = 0; i < SyncLyrics[j].Count; i ++)
                    {
                        long time = SyncLyrics[j][i].Time;
                        string text = SyncLyrics[j][i].Text;

                        // Put "/" everywhere
                        text = text.Replace("\r\n", m_SepLine);
                        text = text.Replace("\r", m_SepLine);
                        text = text.Replace("\n", m_SepLine);
                        text = text.Replace(" ", "_");

                        dgView.Rows.Add(time, text);
                    }
                }                                           
            }

            //NumberRows();

        }


        private void NumberRows()
        {
            foreach (DataGridViewRow r in dgView.Rows)
            {
                dgView.Rows[r.Index].HeaderCell.Value =
                                    (r.Index + 1).ToString();
            }
        }

        #endregion Populate gridview


        #region Menus

        /// <summary>
        /// Menu File Quit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuFileQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void mnuHelpAbout_Click(object sender, EventArgs e)
        {
            frmAboutDialog dlg = new frmAboutDialog();
            dlg.ShowDialog();
        }

        #endregion Menus


        #region gridview edition
        
        /// <summary>
        /// Insert a new line
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInsertText_Click(object sender, EventArgs e)
        {
            InsertTextLine();
        }

        private void InsertTextLine()
        {
            if (dgView.CurrentRow == null)
                return;

            string time = "";
            string text = "";

            int Row = dgView.CurrentRow.Index;
            dgView.Rows.Insert(Row, time, text);

            localSyncLyrics = LoadModifiedLyrics();
            if (localSyncLyrics != null)
                PopulateTextBox(localSyncLyrics);

        }

        /// <summary>
        /// Delete a line
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteLine();
        }

        private void DeleteLine()
        {
            try
            {
                int row = dgView.CurrentRow.Index;
                dgView.Rows.RemoveAt(row);

                localSyncLyrics = LoadModifiedLyrics();
                if (localSyncLyrics != null)
                    PopulateTextBox(localSyncLyrics);
            }
            catch (Exception Ex)
            {
                string message = "Error : " + Ex.Message;
                MessageBox.Show(message, "Error deleting line",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Insert a LineFeed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInsertCr_Click(object sender, EventArgs e)
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

        public bool IsNumeric(string input)
        {
            return int.TryParse(input, out int test);
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
            long time = 0;            
            string lyric;

            if (dgView.Rows[Row].Cells[0].Value != null && IsNumeric(dgView.Rows[Row].Cells[0].Value.ToString()))
            {
                time = Convert.ToInt32(dgView.Rows[Row].Cells[0].Value);
            }

            if (sep == "cr")
                lyric = m_SepLine;
            else
                lyric = m_SepParagraph;

            // time, type, note, text, text
            dgView.Rows.Insert(Row, time, lyric);


            //Load modification into local list of lyrics
            localSyncLyrics = LoadModifiedLyrics();
            if (localSyncLyrics != null)
                PopulateTextBox(localSyncLyrics);

            // Modify height of cells according to durations
            //HeightsToDurations();

            // Color separators
            //ColorSepRows();

            // File was modified
            //FileModified();
        }

        /// <summary>
        /// Cell edition
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string lyric;

            if (dgView.CurrentCell.ColumnIndex == 1 && dgView.CurrentRow.Cells[1].Value != null) {
                lyric = dgView.CurrentRow.Cells[1].Value.ToString();
                lyric = lyric.Replace(" ", "_");
                dgView.CurrentRow.Cells[1].Value = lyric;
            }

            localSyncLyrics = LoadModifiedLyrics();
            if (localSyncLyrics != null)
                PopulateTextBox(localSyncLyrics);
        }

        #endregion gridview edition


        #region init

        private void LoadOptions()
        {
            _lyricseditfont = Properties.Settings.Default.LyricsEditFont;
            _fontSize = _lyricseditfont.Size;
        }

        private void SetOriginOfLyrics()
        {
            
            switch (Mp3LyricsMgmtHelper.m_mp3lyricstype)
            {
                case Mp3LyricsTypes.LyricsWithTimeStamps:
                    lblLyricsOrigin.Text = "Origin of lyrics = mp3: " + Path.GetFileName(_filename);
                    break;
                case Mp3LyricsTypes.LRCFile:
                    string lrcFile = Path.ChangeExtension(_filename, ".lrc");
                    if (System.IO.File.Exists(lrcFile))
                        lblLyricsOrigin.Text = "Origin of lyrics = lrc: " + Path.GetFileName(lrcFile);
                    break;
                case Mp3LyricsTypes.LyricsWithoutTimeStamps:        // Lyrics does not exist => display frmMp3LyricsEdit
                    lblLyricsOrigin.Text = "Origin of lyrics = None";
                    break;
                case Mp3LyricsTypes.None:
                    lblLyricsOrigin.Text = "Origin of lyrics = None";
                    break;
            }
        }

        private void InitTxtResult()
        {
            txtResult.Font = _lyricseditfont;
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
            dgView.RowHeadersWidth =  60;

            dgView.RowsAdded += new DataGridViewRowsAddedEventHandler(dgView_RowsAdded);
            dgView.RowsRemoved += new DataGridViewRowsRemovedEventHandler(dgView_RowsRemoved);

            // Selection
            dgView.DefaultCellStyle.SelectionBackColor = dgViewSelectionBackColor;

            dgView.EnableHeadersVisualStyles = false;
         
            // Chords edition
            dgView.ColumnCount = 2;

            dgView.Columns[0].Name = "dTime";
            dgView.Columns[0].HeaderText = "Time";
            dgView.Columns[0].ToolTipText = "Time";
            dgView.Columns[0].Width = 80;

            dgView.Columns[1].Name = "dText";
            dgView.Columns[1].HeaderText = "Text";
            dgView.Columns[1].ToolTipText = "Text";
            dgView.Columns[1].Width = 200;                     

            //Change cell font
            foreach (DataGridViewColumn c in dgView.Columns)
            {
                c.SortMode = DataGridViewColumnSortMode.NotSortable;                     // header not sortable
                c.DefaultCellStyle.Font = dgViewCellsFont;
                c.ReadOnly = false;
            }

            

            ResizeMe();
        }

        private void dgView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            NumberRows();
        }

        private void dgView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            NumberRows();
        }

        #endregion init


        #region Save mp3 Lyrics

        /// <summary>
        /// Button: save mp3 lyrics
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveMp3Lyrics();             
        }

        /// <summary>
        /// Menu: save mp3 lyrics
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuFileSave_Click(object sender, EventArgs e)
        {
            SaveMp3Lyrics();
        }

        /// <summary>
        /// Save mp3 lyrics
        /// </summary>
        private void SaveMp3Lyrics()
        {
           // it is not possible to save the file on the same file (file locked)
            string mp3file = Files.FindUniqueFileName(_filename);

            saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Mp3 files (*.mp3)|*.mp3|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.InitialDirectory = Path.GetDirectoryName(mp3file);
            saveFileDialog.FileName = Path.GetFileName(mp3file);

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {

                string filename = saveFileDialog.FileName;

                // Copy file to another name                
                System.IO.File.Copy(_filename, filename, true);

                // Save sync lyrics into the copy of initial file
                SaveFrame(filename);
            }
        }
      

        /// <summary>
        /// Save changes to the synchronized lyrics frame
        /// </summary>
        /// <param name="FileName"></param>
        private void SaveFrame(string FileName)
        {
            string lyric;

            TagLib.File file = TagLib.File.Create(FileName);
            TagLib.Tag _tag = file.GetTag(TagTypes.Id3v2);
            
            // Reset frame text
            if (Mp3LyricsMgmtHelper.MySyncLyricsFrame == null)
            {                
                Mp3LyricsMgmtHelper.MySyncLyricsFrame = new SynchronisedLyricsFrame("Karaboss", "en", SynchedTextType.Lyrics);
            }

            // How many valid lines ?
            //int lines = dgView.Rows.Count;
            int lines = 0;
            for (int i = 0; i < dgView.Rows.Count; i++)
            {
                if (dgView.Rows[i].Cells[0].Value != null)
                    lines++;
            }


            Mp3LyricsMgmtHelper.MySyncLyricsFrame.Text = new SynchedText[lines];
            
            // Read all rows and store into the frame
            for (int i = 0; i < lines; i++)
            {

                if (dgView.Rows[i].Cells[0].Value != null)
                {
                    Mp3LyricsMgmtHelper.MySyncLyricsFrame.Text[i] = new SynchedText();
                    Mp3LyricsMgmtHelper.MySyncLyricsFrame.Text[i].Time = long.Parse(dgView.Rows[i].Cells[0].Value.ToString());

                    // Modify lyrics
                    // \ => '\n'
                    // _ => " "
                    lyric = dgView.Rows[i].Cells[1].Value.ToString();
                    lyric = lyric.Replace(m_SepLine, "\n");
                    lyric = lyric.Replace("_", " ");
                    Mp3LyricsMgmtHelper.MySyncLyricsFrame.Text[i].Text = lyric;
                }
            }            

            Mp3LyricsMgmtHelper.SetTags(FileName, Mp3LyricsMgmtHelper.MySyncLyricsFrame);
        }

        #endregion Save mp3 Lyrics


        #region lrc

        #region save lrc
        private void mnuFileSaveAsLrc_Click(object sender, EventArgs e)
        {
            string text;
            string time;

            SyncText[] SyncTexts = new SyncText[dgView.RowCount];

            for (int i = 0; i < dgView.RowCount - 1; i++)
            {
                time = dgView.Rows[i].Cells[0].Value.ToString();
                text = dgView.Rows[i].Cells[1].Value.ToString();
                SyncTexts[i] = new SyncText(long.Parse(time), text);
            }
            saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Lyrics files (*.lrc)|*.lrc|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.InitialDirectory = Path.GetDirectoryName(_filename);
            saveFileDialog.FileName = Path.GetFileNameWithoutExtension(_filename) + ".lrc";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = saveFileDialog.FileName;
                SaveLyricsToLrcFormat(SyncTexts, filename);
            }

        }

        /// <summary>
        /// Save to text file
        /// </summary>
        /// <param name="SyncTexts"></param>
        /// <param name="filename"></param>
        private void SaveLyricsToLrcFormat(SyncText[] SyncTexts, string filename)
        {
            long time;
            TimeSpan ts;
            string tsp;
            string lyric;

            try
            {
                // Save to text file
                for (int i = 0; i < SyncTexts.Length; i++)
                {
                    if (SyncTexts[i].Text != null && SyncTexts[i].Text != "")
                    {
                        // the time is in millisecond format. It must be converted to timestamp format
                        
                        time = SyncTexts[i].Time;
                        ts = TimeSpan.FromMilliseconds(time);                        
                        tsp = string.Format("{0:00}:{1:00}.{2:000}", ts.Minutes, ts.Seconds, ts.Milliseconds);

                        lyric = SyncTexts[i].Text;
                        lyric = lyric.Replace("_", " ");

                        string line = "[" + tsp + "]" + lyric + "\r\n";
                        System.IO.File.AppendAllText(filename, line);
                    }
                }

                // Open lrc file
                if (System.IO.File.Exists(filename)) 
                {
                    System.Diagnostics.Process.Start(@filename);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving lyrics to lrc: " + ex.Message);
            }
        }

        #endregion save lrc


        #region load lrc

        /// <summary>
        /// Menu: load lrc file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuEditLoadLRCFile_Click(object sender, EventArgs e)
        {
            openFileDialog.Title = "Open a .lrc file";
            openFileDialog.DefaultExt = "lrc";
            openFileDialog.Filter = "lrc files|*.lrc|All files|*.*";
            
            // Get initial directory from mp3 file
            if (_filename != null || _filename != "")
                openFileDialog.InitialDirectory = Path.GetDirectoryName(_filename);

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;

                try
                {
                    lblLyricsOrigin.Text = "Origin of lyrics = lrc: " + Path.GetFileName(fileName);
                    LoadLRCFile(fileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("The file could not be read:" + ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Load a LRC file (timestamps + lyrics)
        /// </summary>
        /// <param name="Source"></param>
        private void LoadLRCFile(string FileName)
        {                        
            long time;
            string text;
            

            Cursor.Current = Cursors.WaitCursor;

            Mp3LyricsMgmtHelper.SyncLyrics = Mp3LyricsMgmtHelper.GetKEffectLrcLyrics(FileName);
            List<List<keffect.KaraokeEffect.kSyncText>> SyncLyrics = Mp3LyricsMgmtHelper.SyncLyrics;            

           InitGridView();
           
            // For each line
            for (int j = 0; j < SyncLyrics.Count; j++)
            {
                // For each syllabes
                for (int i = 0; i < SyncLyrics[j].Count; i++)
                {
                    time = SyncLyrics[j][i].Time;
                    text = SyncLyrics[j][i].Text;

                    // Put "/" everywhere
                    text = text.Replace("\r\n", m_SepLine);
                    text = text.Replace("\r", m_SepLine);
                    text = text.Replace("\n", m_SepLine);
                    text = text.Replace(" ", "_");

                    dgView.Rows.Add(time, text);
                }
            }
           
            Cursor.Current = Cursors.Default;
        }



        #endregion load lrc

        #endregion lrc


        #region Text

        /// <summary>
        /// Display text into the rich textbox
        /// </summary>
        /// <param name="lLyrics"></param>
        private void PopulateTextBox(List<List<keffect.KaraokeEffect.kSyncText>> lSyncLyrics)
        {
            string line = string.Empty;
            string tx = string.Empty;
            string cr = "\r\n";
            string Element;

            for (int j = 0; j < lSyncLyrics.Count; j++)
            {
                line = string.Empty;

                for (int i = 0; i < lSyncLyrics[j].Count; i ++)
                {
                    Element = lSyncLyrics[j][i].Text;
                    Element = Element.Replace(Environment.NewLine, "");
                    line += Element;
                }                
                tx += line + cr;
            }

            txtResult.Text = tx;

            txtResult.SelectAll();
            txtResult.SelectionAlignment = HorizontalAlignment.Center;
        }

        /// <summary>
        /// Store gridview into 
        /// </summary>
        /// <returns></returns>
        private List<List<keffect.KaraokeEffect.kSyncText>> LoadModifiedLyrics()
        {
            int line;
            if (!CheckTimes(out line))
            {
                MessageBox.Show("Time on line " + line + " is incorrect", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dgView.CurrentCell  = dgView.Rows[line - 1].Cells[0];
                return null;
            }
            
            long time;
            string text;
            string cr = "\r\n";

            List<keffect.KaraokeEffect.kSyncText> SyncLine = new List<keffect.KaraokeEffect.kSyncText>();
            List<List<keffect.KaraokeEffect.kSyncText>> lSyncLyrics = new List<List<keffect.KaraokeEffect.kSyncText>>();
            keffect.KaraokeEffect.kSyncText kst;

            for (int i = 0; i < dgView.RowCount; i ++)
            {
                if (dgView.Rows[i].Cells[0].Value == null 
                    || dgView.Rows[i].Cells[1].Value == null 
                    || !IsNumeric(dgView.Rows[i].Cells[0].Value.ToString())) continue;
                
                time = Convert.ToInt32(dgView.Rows[i].Cells[0].Value);
                text = dgView.Rows[i].Cells[1].Value.ToString();
                
                // If start of line
                if (text.IndexOf(m_SepLine) != -1)
                {
                    if (SyncLine.Count > 0)
                        lSyncLyrics.Add(SyncLine);
                    SyncLine = new List<keffect.KaraokeEffect.kSyncText>();

                    text = text.Replace(m_SepLine, "");
                    text = cr + text;
                    
                }
                
                text = text.Replace("_", " ");
                kst = new keffect.KaraokeEffect.kSyncText(time, text);
                SyncLine.Add(kst);
            }

            if (SyncLine.Count > 0)
                lSyncLyrics.Add(SyncLine);

            return lSyncLyrics;
        }

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

        private void btnDeleteAllLyrics_Click(object sender, EventArgs e)
        {
            string tx = Karaboss.Resources.Localization.Strings.DeleteAllLyrics;
            if (MessageBox.Show(tx, "Karaboss", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                //frmPlayer frmPlayer = Utilities.FormUtilities.GetForm<frmPlayer>();
                //frmPlayer.DeleteAllLyrics();

                //localplLyrics = new List<plLyric>();

                InitGridView();
                txtResult.Text = string.Empty; 

                // File was modified
                //FileModified();
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


        #endregion Text

       
    }
}

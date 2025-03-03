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
using System.Runtime.Remoting.Contexts;
using System.Text.RegularExpressions;
using System.Text;
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

        private int _LrcMillisecondsDigits = 2;

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
                    SaveMp3Lyrics();
                    
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
            
            FileModified();
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
            FileModified();
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

            FileModified();
        }

        #endregion gridview edition


        #region init

        private void LoadOptions()
        {
            _lyricseditfont = Properties.Settings.Default.LyricsEditFont;
            _fontSize = _lyricseditfont.Size;

            // 2 or 3 digits for timestamp format 00:00.00 or 00:00.000
            _LrcMillisecondsDigits = Properties.Settings.Default.LrcMillisecondsDigits;
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

            bfilemodified = false;
        }


        /// <summary>
        /// File was modified
        /// </summary>
        private void FileModified()
        {
            bfilemodified = true;
            string fName = Path.GetFileName(_filename);
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


        #endregion Save mp3 Lyrics


        #region lrc

        #region save lrc
        /// <summary>
        /// Menu: save to format LRC
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
            string fPath = Path.GetDirectoryName(_filename);

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
                fName = Path.GetFileName(_filename);
            }

            // Extension forced to lrc            
            string fullPath = fPath + "\\" + Path.GetFileNameWithoutExtension(fName) + defExt;
            fullName = Utilities.Files.FindUniqueFileName(fullPath);                            // Add (2), (3) etc.. if necessary    
            defName = Path.GetFileNameWithoutExtension(fullName);                               // Default name to propose to dialog

            #endregion search name                   

            string defFilter = "LRC files (*.lrc)|*.lrc|All files (*.*)|*.*";

            saveFileDialog.Title = "Save to LRC format";
            saveFileDialog.Filter = defFilter;
            saveFileDialog.DefaultExt = defExt;
            saveFileDialog.InitialDirectory = @fPath;
            saveFileDialog.FileName = defName;

            if (saveFileDialog.ShowDialog() != DialogResult.OK)
                return;

            #endregion

            string Tag_Tool = "Karaboss https://karaboss.lacharme.net";

            string Tag_Title = string.Empty;
            string Tag_Artist = string.Empty;
            string Tag_Album = string.Empty;
            string Tag_Lang = string.Empty;
            string Tag_By = string.Empty;
            string Tag_DPlus = string.Empty;

            fullPath = saveFileDialog.FileName;

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
            //Tag_Title = (sequence1.TTag != null && sequence1.TTag.Count > 1) ? sequence1.TTag[0] : "";
            //Tag_Artist = (sequence1.TTag != null && sequence1.TTag.Count > 1) ? sequence1.TTag[1] : "";


            if (Tag_Artist == "" && Tag_Title == "")
            {
                List<string> lstTags = Utilities.LyricsUtilities.GetTagsFromFileName(fullPath);
                Tag_Artist = lstTags[0];
                Tag_Title = lstTags[1];
            }

            switch (LrcLinesSyllabesFormat)
            {
                case LrcLinesSyllabesFormats.Lines:
                    SaveLRCLines(fullPath, bRemoveAccents, bUpperCase, bLowerCase, bRemoveNonAlphaNumeric, Tag_Tool, Tag_Title, Tag_Artist, Tag_Album, Tag_Lang, Tag_By, Tag_DPlus, bCutLines, LrcCutLinesChars);
                    break;
                case LrcLinesSyllabesFormats.Syllabes:
                    SaveLRCSyllabes(fullPath, bRemoveAccents, bUpperCase, bLowerCase, bRemoveNonAlphaNumeric, Tag_Tool, Tag_Title, Tag_Artist, Tag_Album, Tag_Lang, Tag_By, Tag_DPlus);
                    break;
            }
        }

        /// <summary>
        /// Save Lyrics .lrc file format and by lines
        /// </summary>
        /// <param name="File"></param>
        /// <param name="Tag_Title"></param>
        /// <param name="Tag_Artist"></param>
        /// <param name="Tag_Album"></param>
        /// <param name="Tag_Lang"></param>
        /// <param name="Tag_By"></param>
        /// <param name="Tag_DPlus"></param>
        private void SaveLRCLines(string File, bool bRemoveAccents, bool bUpperCase, bool bLowerCase, bool bRemoveNonAlphaNumeric, string Tag_Tool, string Tag_Title, string Tag_Artist, string Tag_Album, string Tag_Lang, string Tag_By, string Tag_DPlus, bool bControlLength, int MaxLength)
        {
            string sLine;
            string sTime;
            long time;
            TimeSpan ts;
            string tsp;
            string sLyric;
            string sType;
            object vLyric;
            object vTime;
            object vType;
            string lrcs;
            string cr = "\r\n";
            string strSpaceBetween;
            bool bSpaceBetwwen = false;

            // Space between time and lyrics [00:02.872]lyric
            if (bSpaceBetwwen)
                strSpaceBetween = " ";
            else
                strSpaceBetween = string.Empty;

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
                    sLine = "[" + TagName + strSpaceBetween + Tag + "]";
                    // lrcs += sLine + cr;
                    lstHeaderLines.Add(sLine);
                }
            }
            #endregion meta data


            #region List of lyrics

            // Put everuthing into a string
            string tx = string.Empty;
            for (int i = 0; i < dgView.Rows.Count; i++)
            {
                vLyric = dgView.Rows[i].Cells[1].Value;
                vTime = dgView.Rows[i].Cells[0].Value;
                if (vTime != null && vLyric != null)
                    tx += vTime.ToString() + vLyric.ToString();
            }

            // Replace "]/" by "]"
            tx = tx.Replace("]/", "]");

            // Replace double [][] by last one (to have the right timestamp)
            string[] lst = tx.Split(']', '[');


            // Store lyrics in a list
            // sTime, sType, sLyric
            List<(string, string, string)> lstLyricsItems = new List<(string, string, string)>();

            for (int i = 0; i < dgView.Rows.Count; i++)
            {
                vLyric = dgView.Rows[i].Cells[1].Value;
                vTime = dgView.Rows[i].Cells[0].Value;
                // Same as vLyric
                vType = dgView.Rows[i].Cells[1].Value;

                if (vTime != null && vLyric != null && vType != null)
                {
                    // lyrics: Trim all and replace underscore by a space
                    sLyric = vLyric.ToString().Trim();

                    sTime = vTime.ToString();
                    time = long.Parse(sTime);
                    ts = TimeSpan.FromMilliseconds(time);
                    
                    if (_LrcMillisecondsDigits == 2)
                        tsp = string.Format("{0:00}:{1:00}.{2:00}", ts.Minutes, ts.Seconds, ts.Milliseconds/10);
                    else
                        tsp = string.Format("{0:00}:{1:00}.{2:000}", ts.Minutes, ts.Seconds, ts.Milliseconds);

                    sTime = "[" + tsp + "]";  // Transform to [00:00.000] format


                    // Case: only a "/" or a "\"
                    // Case: a string beginning with a "/" or a "\"
                    // Case: a string

                    if (sLyric.Trim() == m_SepParagraph )
                    {
                        sType = "par";
                        lstLyricsItems.Add((sTime, sType, m_SepParagraph));
                    }
                    else if ( sLyric.Trim() == m_SepLine) 
                    {
                        sType = "cr";
                        lstLyricsItems.Add((sTime, sType, m_SepLine));
                    }
                    else if (sLyric.IndexOf(m_SepParagraph) > -1)
                    {
                        sLyric = sLyric.Replace(m_SepParagraph, "");
                        sType = "text";
                        /* Case of lyric containing spaces in the middle: only replace first or last occurence of underscore
                         * We must keep the undercores located inside the string for next split with spaces
                         * ex: _the_air,_(get_to_poppin')
                         * So big bug if we use sLyric = sLyric.Replace("_", " ");
                        */
                        if (sLyric.Length > 0)
                        {
                            // replace leading or trailing underscore by a space ' '
                            StringBuilder sb = new StringBuilder(sLyric);
                            if (sLyric.StartsWith(@"_"))
                                sb[0] = ' ';
                            if (sLyric.EndsWith(@"_"))
                                sb[sLyric.Length - 1] = ' ';
                            sLyric = sb.ToString();
                        }
                        if (sLyric != "")   // Universal code for syllabes & lines: lyrics like " " can be added
                        {
                            // Remove accents
                            sLyric = bRemoveAccents ? Utilities.LyricsUtilities.RemoveDiacritics(sLyric) : sLyric;

                            //Uppercase letters
                            sLyric = bUpperCase ? sLyric.ToUpper() : sLyric;

                            // Lowercase letters
                            sLyric = bLowerCase ? sLyric.ToLower() : sLyric;

                            // Remove non alphanumeric chars
                            // Prevent removal of '_' character 
                            sLyric = sLyric.Replace("_", " ");
                            sLyric = bRemoveNonAlphaNumeric ? Utilities.LyricsUtilities.RemoveNonAlphaNumeric(sLyric) : sLyric;
                            sLyric = sLyric.Replace(" ", "_");

                            lstLyricsItems.Add((sTime, "cr", m_SepLine));
                            lstLyricsItems.Add((sTime, sType, sLyric));
                        }
                    }
                    else if (sLyric.IndexOf(m_SepLine) > -1)
                    {
                        sLyric = sLyric.Replace(m_SepLine, "");
                        sType = "text";
                        /* Case of lyric containing spaces in the middle: only replace first or last occurence of underscore
                         * We must keep the undercores located inside the string for next split with spaces
                         * ex: _the_air,_(get_to_poppin')
                         * So big bug if we use sLyric = sLyric.Replace("_", " ");
                        */
                        if (sLyric.Length > 0)
                        {
                            // replace leading or trailing underscore by a space ' '
                            StringBuilder sb = new StringBuilder(sLyric);
                            if (sLyric.StartsWith(@"_"))
                                sb[0] = ' ';
                            if (sLyric.EndsWith(@"_"))
                                sb[sLyric.Length - 1] = ' ';
                            sLyric = sb.ToString();
                        }
                        if (sLyric != "")   // Universal code for syllabes & lines: lyrics like " " can be added
                        {
                            // Remove accents
                            sLyric = bRemoveAccents ? Utilities.LyricsUtilities.RemoveDiacritics(sLyric) : sLyric;

                            //Uppercase letters
                            sLyric = bUpperCase ? sLyric.ToUpper() : sLyric;

                            // Lowercase letters
                            sLyric = bLowerCase ? sLyric.ToLower() : sLyric;

                            // Remove non alphanumeric chars
                            // Prevent removal of '_' character 
                            sLyric = sLyric.Replace("_", " ");
                            sLyric = bRemoveNonAlphaNumeric ? Utilities.LyricsUtilities.RemoveNonAlphaNumeric(sLyric) : sLyric;
                            sLyric = sLyric.Replace(" ", "_");

                            lstLyricsItems.Add((sTime, "cr", m_SepLine));
                            lstLyricsItems.Add((sTime, sType, sLyric));
                        }
                    }
                    else
                    {                        
                        sType = "text";
                        /* Case of lyric containing spaces in the middle: only replace first or last occurence of underscore
                         * We must keep the undercores located inside the string for next split with spaces
                         * ex: _the_air,_(get_to_poppin')
                         * So big bug if we use sLyric = sLyric.Replace("_", " ");
                        */
                        if (sLyric.Length > 0)
                        {
                            // replace leading or trailing underscore by a space ' '
                            StringBuilder sb = new StringBuilder(sLyric);
                            if (sLyric.StartsWith(@"_"))
                                sb[0] = ' ';
                            if (sLyric.EndsWith(@"_"))
                                sb[sLyric.Length - 1] = ' ';
                            sLyric = sb.ToString();
                        }
                        if (sLyric != "")   // Universal code for syllabes & lines: lyrics like " " can be added
                        {
                            // Remove accents
                            sLyric = bRemoveAccents ? Utilities.LyricsUtilities.RemoveDiacritics(sLyric) : sLyric;

                            //Uppercase letters
                            sLyric = bUpperCase ? sLyric.ToUpper() : sLyric;

                            // Lowercase letters
                            sLyric = bLowerCase ? sLyric.ToLower() : sLyric;

                            // Remove non alphanumeric chars
                            // Prevent removal of '_' character 
                            sLyric = sLyric.Replace("_", " ");
                            sLyric = bRemoveNonAlphaNumeric ? Utilities.LyricsUtilities.RemoveNonAlphaNumeric(sLyric) : sLyric;
                            sLyric = sLyric.Replace(" ", "_");

                            lstLyricsItems.Add((sTime, sType, sLyric));
                        }
                    }                                            
                }
            }

            #endregion List of Lyrics

            // Store lyrics in lines            
            List<string> lstLines = Utilities.LyricsUtilities.GetLrcLines(lstLyricsItems, strSpaceBetween);

            // Store timestamps + lyrics in lines
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

            // Lines
            if (bControlLength)
            {
                for (int i = 0; i < lstLinesCut.Count; i++)
                {
                    // Replace underscores located in the middle of the lyrics
                    // ex: " the_air,_(get_to_poppin')"
                    lrcs += lstLinesCut[i].Replace("_", " ") + cr;
                }
            }
            else
            {
                for (int i = 0; i < lstLines.Count; i++)
                {
                    // Replace underscores located in the middle of the lyrics
                    // ex: " the_air,_(get_to_poppin')"
                    lrcs += lstLines[i].Replace("_", " ") + cr;
                }
            }
            #endregion send all to string

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
        /// Save lyrics to new LRC format [01:54.60]Pa<01:55.32>ro<01:56.15>les
        /// </summary>
        /// <param name="FileName"></param>
        private void SaveLRCSyllabes(string File, bool bRemoveAccents, bool bUpperCase, bool bLowerCase, bool bRemoveNonAlphaNumeric, string Tag_Tool, string Tag_Title, string Tag_Artist, string Tag_Album, string Tag_Lang, string Tag_By, string Tag_DPlus)
        {
            string sTime;
            long time;
            TimeSpan ts;
            string tsp;

            string sType;
            string sLyric;
            
            object vLyric;
            object vTime;
            object vType;

            string lrcs = string.Empty;
            string cr = "\r\n";
            string strSpaceBetween;
            bool bSpaceBetwwen = false;
            string lines = string.Empty;

            // Space between time and lyrics [00:02.872]lyric
            if (bSpaceBetwwen)
                strSpaceBetween = " ";
            else
                strSpaceBetween = string.Empty;

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
                    lrcs += "[" + TagName + strSpaceBetween + Tag + "]" + cr;
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
                vLyric = dgView.Rows[i].Cells[1].Value;
                vTime = dgView.Rows[i].Cells[0].Value;
                vType = dgView.Rows[i].Cells[1].Value;

                if (vTime != null && vLyric != null && vTime.ToString() != "" && vLyric.ToString() != "")
                {
                    
                    sLyric = vLyric.ToString();
                    sLyric = sLyric.Replace("_", " ");

                    sTime = vTime.ToString();
                    time = long.Parse(sTime);
                    ts = TimeSpan.FromMilliseconds(time);
                    
                    if(_LrcMillisecondsDigits == 2)
                        tsp = string.Format("{0:00}:{1:00}.{2:00}", ts.Minutes, ts.Seconds, ts.Milliseconds/10);
                    else
                        tsp = string.Format("{0:00}:{1:00}.{2:000}", ts.Minutes, ts.Seconds, ts.Milliseconds);

                    sTime = tsp;  // Transform to [00:00.000] format


                    // /hey
                    // jude
                    sType = vType.ToString().Trim();
                    if (sType.IndexOf(m_SepParagraph) != -1 || sType.IndexOf(m_SepLine) != -1)                    
                        bLineFeed = true;
                        

                    sLyric = sLyric.Replace(m_SepParagraph, "");
                    sLyric = sLyric.Replace(m_SepLine, "");

                    
                    // Remove accents
                    sLyric = bRemoveAccents ? Utilities.LyricsUtilities.RemoveDiacritics(sLyric) : sLyric;

                    //Uppercase letters
                    sLyric = bUpperCase ? sLyric.ToUpper() : sLyric;

                    // Lowercase letters
                    sLyric = bLowerCase ? sLyric.ToLower() : sLyric;

                    // Remove non-alphanumeric chars
                    sLyric = bRemoveNonAlphaNumeric ? Utilities.LyricsUtilities.RemoveNonAlphaNumeric(sLyric) : sLyric;

                    // Save also empty lyrics
                    //sTime = vTime.ToString();

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

                    localSyncLyrics = LoadModifiedLyrics();
                    if (localSyncLyrics != null)
                        PopulateTextBox(localSyncLyrics);

                    FileModified();
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
        /// Show line of texbox currently edited
        /// </summary>
        private void ShowCurrentLine()
        {
            int r = dgView.CurrentCell.RowIndex;

            // Text before current
            string tx = string.Empty;
            string s; 

            for (int row = 0; row < r; row++)
            {
                s = string.Empty;

                if (dgView.Rows[row].Cells[1].Value != null && dgView.Rows[row].Cells[1].Value.ToString() != "")
                {                                       
                    s = dgView.Rows[row].Cells[1].Value.ToString();

                    if (row == 0)
                    {
                        if (s.StartsWith(m_SepLine))
                            s = s.Replace(m_SepLine, "");   // J'me comprends
                    }

                    s = s.Replace(m_SepParagraph, "\n\n");
                    s = s.Replace(m_SepLine, "\n");                                        
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
                try
                {
                    dgView.CurrentCell = dgView.Rows[line - 1].Cells[0];
                }
                catch (Exception e) 
                {
                    Console.WriteLine(e.Message);
                }
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
                FileModified();
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

        private void dgView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            ShowCurrentLine();
        }

        #endregion Text
    }
}

#region License

/* Copyright (c) 2018 Fabrice Lacharme
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
using PicControl;
using System.IO;
using System.Text.RegularExpressions;
using Karaboss.Resources.Localization;

namespace Karaboss
{
    public partial class frmLyricsEdit : Form
    {

        /* Lyrics edition form
         * 
         * 0 - Ticks    0
         * 1 - Time
         * 2 - Type     1
         * 3 - Note     2
         * 4 - Text     3
         * 5 - Replace  4
         * 
         * 
         * 
         */

        frmPlayer frmPlayer;

        private bool bfilemodified = false;

        private Sequence sequence1;
        private List<pictureBoxControl.plLyric> localplLyrics;

        private Track melodyTrack;
        private CLyric myLyric;

        private ContextMenuStrip dgContextMenu;
        private DataGridViewSelectedCellCollection DGV;

        enum LyricFormat
        {
            Text = 0,
            Lyric = 1
        }       

        const int COL_TICKS = 0;
        const int COL_TIME = 1;
        const int COL_TYPE = 2;
        const int COL_NOTE = 3;
        const int COL_TEXT = 4;
        const int COL_REPLACE = 5;

        LyricFormat TextLyricFormat;        

        int melodytracknum = 0;

        private string MIDIfileName = string.Empty;

        // Midifile characteristics
        private double _duration = 0;  // en secondes
        private int _totalTicks = 0;
        private int _bpm = 0;
        private double _ppqn;
        private int _tempo;
        private int _measurelen;


        public frmLyricsEdit(Sequence sequence, List<pictureBoxControl.plLyric> plLyrics, CLyric mylyric, string fileName)
        {
            InitializeComponent();            

            MIDIfileName = fileName;
            sequence1 = sequence;
            UpdateMidiTimes();

            myLyric = mylyric;
            InitGridView();
            
            melodytracknum = myLyric.melodytracknum;
            if (melodytracknum != -1)
                melodyTrack = sequence1.tracks[melodytracknum];

            if (myLyric.lyrictype == "text")
            {
                TextLyricFormat = LyricFormat.Text;
                optFormatText.Checked = true;
            }
            else
            {
                TextLyricFormat = LyricFormat.Lyric;
                optFormatLyrics.Checked = true;
            }

            // If first time = no lyrics
            if (plLyrics.Count == 0)
                LoadTrackGuide();
            else
            {               
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

            ResizeMe();
        }


        /// <summary>
        /// Upadate MIDI times
        /// </summary>
        private void UpdateMidiTimes()
        {
            _totalTicks = sequence1.GetLength();
            _tempo = sequence1.Tempo;            
            _ppqn = sequence1.Division;
            _duration = _tempo * (_totalTicks / _ppqn) / 1000000; //seconds            

            if (sequence1.Time != null)
                _measurelen = sequence1.Time.Measure;
        }


        private string TicksToTime(int ticks)
        {
            double dur = _tempo * (ticks / _ppqn) / 1000000; //seconds     
            int Min = (int)(dur / 60);
            int Sec = (int)(dur - (Min * 60));            


            int Cent = (int)(100*(dur - (Min * 60) - Sec));

            string tx = string.Format("{0:00}:{1:00}:{2:00}", Min, Sec, Cent);
            return tx;
        }

        /// <summary>
        /// Retrieve Lyrics format from frmPlayer
        /// </summary>
        private void GuessLyricsFormat()
        {
            if (Application.OpenForms.OfType<frmPlayer>().Count() > 0)
            {
                frmPlayer = GetForm<frmPlayer>();
            }
            else
            {
                TextLyricFormat = LyricFormat.Text;
                optFormatLyrics.Checked = true;
                return;
            }

            if (frmPlayer.myLyric.lyrictype == "text")
            {
                TextLyricFormat = LyricFormat.Text;
                optFormatText.Checked = true;                                
            }
            else
            {
                TextLyricFormat = LyricFormat.Lyric;
                optFormatLyrics.Checked = true;
            }
            melodytracknum = frmPlayer.myLyric.melodytracknum;
        }


        #region gridview
        public bool IsNumeric(string input)
        {
            int test;
            return int.TryParse(input, out test);
        }

        private void DgView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int val = 0;

            // If first col is edited
            if (dgView.CurrentCell.ColumnIndex == 0)
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



            }
            else  if (dgView.CurrentCell.ColumnIndex == dgView.Columns.Count - 1)
            {
                // If last col is edited

                if (dgView.CurrentCell.Value == null)
                    dgView.CurrentCell.Value = "";

                dgView.Rows[dgView.CurrentCell.RowIndex].Cells[COL_TYPE].Value = "text";

                string c = dgView.CurrentCell.Value.ToString();
                c = c.Replace(" ", "_");
                dgView.CurrentCell.Value = c;

                // Retrieve time value of previous row
                if (dgView.Rows[dgView.CurrentCell.RowIndex].Cells[COL_TICKS].Value == null || !IsNumeric( dgView.Rows[dgView.CurrentCell.RowIndex].Cells[COL_TICKS].Value.ToString()))
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
            LoadModifiedLyrics();
            PopulateTextBox(localplLyrics);

            // File was modified
            FileModified();
            
        }

        /// <summary>
        /// Display current line in textboxbx
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

            dgView.ColumnHeadersDefaultCellStyle.BackColor = Color.Navy;
            dgView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgView.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 12F, FontStyle.Bold);
            dgView.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;           

            //Change cell font
            foreach (DataGridViewColumn c in dgView.Columns)
            {
                c.SortMode = DataGridViewColumnSortMode.NotSortable;                     // header not sortable
                c.DefaultCellStyle.Font = new Font("Arial", 16F, GraphicsUnit.Pixel);
                c.ReadOnly = false;               
            }
        }

        /// <summary>
        /// Populate datagridview with lyrics
        /// </summary>
        /// <param name="plLyrics"></param>
        private void PopulateDataGridView(List<pictureBoxControl.plLyric> lLyrics)
        {
            //string[] row;
            bool bfound = false;

            int plTime = 0;
            string plRealTime = "00:00";
            string plType = string.Empty;
            int plNote = 60;
            string plElement = string.Empty;

            int idx = 0;


            if (melodyTrack == null)
            {
                // On affiche la liste des lyrics 
                for (idx = 0; idx < lLyrics.Count; idx++)
                {
                    plTime = lLyrics[idx].Time;
                    plRealTime = TicksToTime(plTime);           // TODO
                    plNote = 0;
                    plElement = lLyrics[idx].Element;
                    plElement = plElement.Replace(" ", "_");
                    plType = lLyrics[idx].Type;

                    // New Row
                    string[] rowlyric = { plTime.ToString(), plRealTime, plType, plNote.ToString(), plElement, plElement };
                    dgView.Rows.Add(rowlyric);
                }
            }
            else
            {
                // Variante 1 : on affiche les lyrics par défaut et on essaye de raccrocher les notes
                for (int i = 0; i < lLyrics.Count; i++)
                {
                    bfound = false;
                    plTime = lLyrics[i].Time;
                    plRealTime = TicksToTime(plTime);           // TODO
                    plNote = 0;
                    plElement = lLyrics[i].Element;
                    plElement = plElement.Replace(" ", "_");
                    plType = lLyrics[i].Type;

                    if (idx < lLyrics.Count)
                    {
                        // Afficher les notes dont le start est avant celui du Lyric courant
                        while (idx < melodyTrack.Notes.Count && melodyTrack.Notes[idx].StartTime < plTime)
                        {
                            int beforeplTime = melodyTrack.Notes[idx].StartTime;
                            string beforeplRealTime = TicksToTime(beforeplTime);
                            int beforeplNote = melodyTrack.Notes[idx].Number;
                            string beforeplElement = "";
                            string beforeplType = "text";
                            string[] rownote = { beforeplTime.ToString(), beforeplRealTime, beforeplType, beforeplNote.ToString(), beforeplElement, beforeplElement };
                            dgView.Rows.Add(rownote);
                            idx++;
                            if (idx >= melodyTrack.Notes.Count)
                                break;

                        }
                        // Afficher la note dont le start est égal à celui du lyric courant
                        if (idx < melodyTrack.Notes.Count && melodyTrack.Notes[idx].StartTime == plTime) 
                        {
                            plNote = melodyTrack.Notes[idx].Number;
                            string[] rowlyric = { plTime.ToString(), plRealTime, plType, plNote.ToString(), plElement, plElement };
                            dgView.Rows.Add(rowlyric);
                            bfound = true; // lyric inscrit dans la grille
                            // Incrémente le compteur de notes si différent de retour chariot
                            if (plType != "cr")
                                idx++;
                        }
                       
                    }

                    // Lyric courant pas inscrit dans la grille ?
                    if (bfound == false)
                    {
                        string[] rowlyric = { plTime.ToString(), plRealTime, plType, plNote.ToString(), plElement, plElement };
                        dgView.Rows.Add(rowlyric);
                    }
                }

                // Il reste des notes ?
                while (idx < melodyTrack.Notes.Count)
                {
                    int afterplTime = melodyTrack.Notes[idx].StartTime;
                    string afterplRealTime = TicksToTime(afterplTime);
                    int afterplNote = melodyTrack.Notes[idx].Number;
                    string afterplElement = "";
                    string afterplType = "text";
                    string[] rownote = { afterplTime.ToString(), afterplRealTime, afterplType, afterplNote.ToString(), afterplElement, afterplElement };
                    dgView.Rows.Add(rownote);
                    idx++;
                    if (idx >= melodyTrack.Notes.Count)
                        break;

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
                int plTime = 0;
                string plRealTime = string.Empty;
                string plType = string.Empty;
                int plNote = 0;
                string plElement = string.Empty;

                for (int i = 0; i < track.Notes.Count; i++)
                {
                    MidiNote n = track.Notes[i];
                    plTime = n.StartTime;
                    plRealTime = TicksToTime(plTime);
                    plType = "text";
                    plNote = n.Number;
                    plElement = plNote.ToString();

                    string[] row = { plTime.ToString(), plRealTime, plType, plNote.ToString(), plNote.ToString(), plElement };
                    dgView.Rows.Add(row);
                }
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
                string tx = "Le fichier a été modifié, voulez-vous l'enregistrer ?";
                if (MessageBox.Show(tx, "Karaboss", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    e.Cancel = true;

                    //Load modification into local list of lyrics
                    LoadModifiedLyrics();

                    // Display new lyrics in frmLyrics
                    ReplaceLyrics();

                    // Save file
                    SaveFileProc();
                    return;
                }
                else
                {
                    if (Application.OpenForms.OfType<frmPlayer>().Count() > 0)
                    {
                        frmPlayer frmPlayer = GetForm<frmPlayer>();
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
                // Save settings
                Properties.Settings.Default.Save();
            }

            Dispose();
        }

       

        #endregion form load close resize


        #region buttons

        /// <summary>
        /// Button : save new lyrics
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnView_Click(object sender, EventArgs e)
        {

            //Load modification into local list of lyrics
            LoadModifiedLyrics();

            // Display new lyrics in frmLyrics
            ReplaceLyrics();
        }


        /// <summary>
        /// Insert a CR
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnInsert_Click(object sender, EventArgs e)
        {
            int Row = dgView.CurrentRow.Index;
            int plTime = 0;
            string plRealTime = "00:00";

            if (dgView.Rows[Row].Cells[COL_TICKS].Value != null)
            {
                plTime = Convert.ToInt32(dgView.Rows[Row].Cells[COL_TICKS].Value);
                plRealTime = TicksToTime(plTime);
            }

            // time, type, note, text, text
            dgView.Rows.Insert(Row, plTime, plRealTime,"cr", "", "", "");

            //Load modification into local list of lyrics
            LoadModifiedLyrics();
            PopulateTextBox(localplLyrics);

            HeightsToDurations();

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
            InsertLine();
        }

        private void InsertLine()
        {
            int Row = dgView.CurrentRow.Index;
            int plTime = 0;
            string plRealTime = "00:00";
            int pNote = 0;
            string pElement = string.Empty;
            string pReplace = string.Empty;

            if (dgView.Rows[Row].Cells[COL_TICKS].Value != null)
            {
                plTime = Convert.ToInt32(dgView.Rows[Row].Cells[COL_TICKS].Value);
                plRealTime = TicksToTime(plTime);
            }

            pElement = "text";

            // Column Replace
            if (dgView.Rows[Row].Cells[COL_REPLACE].Value != null)
                pReplace = dgView.Rows[Row].Cells[COL_REPLACE].Value.ToString();
            else
                pReplace = "text";


            dgView.Rows.Insert(Row, plTime, plRealTime, "text", pNote, pElement, pReplace);

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
            if (dgView.Rows[Row].Cells[COL_REPLACE].Value != null)
            {
                dgView.Rows[Row].Cells[COL_REPLACE].Value = "_" + dgView.Rows[Row].Cells[COL_REPLACE].Value;
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
            if (dgView.Rows[Row].Cells[COL_REPLACE].Value != null)
            {
                dgView.Rows[Row].Cells[COL_REPLACE].Value = dgView.Rows[Row].Cells[COL_REPLACE].Value + "_";
                
                // File was modified
                FileModified();
            }
        }
        
        /// <summary>
        /// Delete a row
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
                LoadModifiedLyrics();
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
            //Load modification into local list of lyrics
            LoadModifiedLyrics();

            // Display new lyrics in frmLyrics
            ReplaceLyrics();

            // Save file
            SaveFileProc();

            Focus();
        }

        /// <summary>
        /// Play from current time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPlay_Click(object sender, EventArgs e)
        {
            //Load modification into local list of lyrics
            LoadModifiedLyrics();

            // Display new lyrics in frmLyrics
            ReplaceLyrics();

            int Row = dgView.CurrentRow.Index;
            if (dgView.Rows[Row].Cells[COL_TICKS].Value != null)
            {
                int pTime = Convert.ToInt32(dgView.Rows[Row].Cells[COL_TICKS].Value);
                if (Application.OpenForms.OfType<frmPlayer>().Count() > 0)
                {
                    frmPlayer frmPlayer = GetForm<frmPlayer>();
                    frmPlayer.FirstPlaySong(pTime);
                }
            }
        }

 


        #endregion buttons


        #region menus

        /// <summary>
        /// Menu File Save
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuFileSave_Click(object sender, EventArgs e)
        {
            //Load modification into local list of lyrics
            LoadModifiedLyrics();

            // Display new lyrics in frmLyrics
            ReplaceLyrics();            
            
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
            LoadTrackGuide();
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
                        //Console.WriteLine(lines);

                        LoadTextGuide(lines);
                    }
                }
                catch (Exception errl)
                {
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(errl.Message);
                }
            }

        }

        private void LoadTextGuide(string source)
        {            
            source = source.Replace("\r\n", " <cr> ");
            source = source.Replace(" <cr>  <cr> ", " <cr> <cr> ");
            string[] stringSeparators = new string[] { " " };
            string[] result = source.Split(stringSeparators, StringSplitOptions.None);
            for (int i = 0; i < result.Length; i++)
            {
                if (result[i] == "")
                    result[i] = "<cr>";
            }

            string s = string.Empty;
            string d = string.Empty;
            int plTime = 0;
            string plRealTime = "00:00";

            for (int i = 0; i < result.Length; i++)
            {
                s = result[i];
                if (i < dgView.Rows.Count)
                {
                    if (s != "<cr>")
                        dgView.Rows[i].Cells[COL_REPLACE].Value = s + " ";
                    else
                    {
                        if (dgView.Rows[i].Cells[COL_TICKS].Value != null)
                        {
                            plTime = (int)dgView.Rows[i].Cells[COL_TICKS].Value;
                            plRealTime = TicksToTime(plTime);
                        }
                        dgView.Rows.Insert(i, plTime, plRealTime ,"cr", dgView.Rows[i].Cells[COL_NOTE].Value.ToString(), "");
                    }
                }
            }

        }

        /// <summary>
        /// Select track audio guide & lyrics format
        /// </summary>
        private void LoadTrackGuide()
        {                     
            PopulateDataGridViewTrack(melodytracknum);                        
            LoadModifiedLyrics();
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


        #region context menu


        private void DgView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                dgContextMenu = new ContextMenuStrip();
                dgContextMenu.Items.Clear();

                
                // Insert line
                ToolStripMenuItem menuInsertLine = new ToolStripMenuItem(Strings.InsertNewLine);
                dgContextMenu.Items.Add(menuInsertLine);
                menuInsertLine.Click += new System.EventHandler(this.MnuInsertLine_Click);

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

        private void MnuDeleteLine_Click(object sender, EventArgs e)
        {
            DeleteLine();
        }

        private void MnuInsertLine_Click(object sender, EventArgs e)
        {           
            InsertLine();
        }

        private void MnuPaste_Click(object sender, EventArgs e)
        {
            int line = dgView.CurrentCell.RowIndex;
            int k = dgView.CurrentCell.ColumnIndex;            

            if (DGV.Count > 0)
            {
                for (int i = 0; i <= DGV.Count - 1; i++)
                {
                    dgView.Rows[line].Cells[k].Value = DGV[i].Value;
                    line++;
                }                                             
            }
        }

        private void MnuCopy_Click(object sender, EventArgs e)
        {
            DGV = this.dgView.SelectedCells;         

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
            int row = 0;

            for (row = dgView.Rows.Count - 1; row > r; row--) 
            {
                dgView.Rows[row].Cells[COL_REPLACE].Value = dgView.Rows[row-1].Cells[COL_REPLACE].Value;
            }
            dgView.Rows[r].Cells[COL_REPLACE].Value = "";
            LoadModifiedLyrics();
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
            int row = 0;

            for (row = r; row <= dgView.Rows.Count - 2; row++)
            {
                dgView.Rows[row].Cells[COL_REPLACE].Value = dgView.Rows[row + 1].Cells[COL_REPLACE].Value;
            }
            LoadModifiedLyrics();
            PopulateTextBox(localplLyrics);
        }

        #endregion context menu


        #region functions

        /// <summary>
        /// File was modified
        /// </summary>
        private void FileModified()
        {
            bfilemodified = true;
            string fName =  Path.GetFileName(MIDIfileName);
            if (fName != null && fName != "")
            {
                string fExt = Path.GetExtension(fName);             // Extension
                fName = Path.GetFileNameWithoutExtension(fName);    // name without extension

                string fShortName = fName.Replace("*", "");
                if (fShortName == fName)
                    fName = fName + "*";

                fName = fName + fExt;         
                SetTitle(fName);
            }
        }

        /// <summary>
        /// Set Title of the form
        /// </summary>
        private void SetTitle(string displayName)
        {
            displayName = displayName.Replace("__", ": ");
            displayName = displayName.Replace("_", " ");
            Text = "Karaboss - Edit Words - " + displayName;
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
                frmPlayer frmPlayer = GetForm<frmPlayer>();
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
            
            string fullName = string.Empty;
            string defName = string.Empty;

            #region search name
            if (fPath == null || fPath == "")
            {                
                if (Directory.Exists(CreateNewMidiFile.DefaultDirectory))
                    fPath = CreateNewMidiFile.DefaultDirectory;
                else
                    fPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            }
            else
            {
                fName = Path.GetFileName(MIDIfileName);
            }

            string inifName = fName;                            // Original name with extension
            string defExt = Path.GetExtension(fName);           // Extension
            fName = Path.GetFileNameWithoutExtension(fName);    // name without extension
            defName = fName;                                    // Proposed name for dialog box

            fullName = fPath + "\\" + inifName;

            if (File.Exists(fullName) == true)
            {
                // Remove all (1) (2) etc..
                string pattern = @"[(\d)]";
                string replace = @"";
                inifName = Regex.Replace(fName, pattern, replace);



                int i = 1;
                string addName = "(" + i.ToString() + ")";
                defName = inifName + addName + defExt;
                fullName = fPath + "\\" + defName;

                while (File.Exists(fullName) == true)
                {
                    i++;
                    defName = inifName + "(" + i.ToString() + ")" + defExt;
                    fullName = fPath + "\\" + defName;
                }
            }

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
                    frmPlayer frmPlayer = GetForm<frmPlayer>();
                    frmPlayer.InitSaveFile(fileName);
                    
                    bfilemodified = false;
                    string displayName = Path.GetFileName(MIDIfileName);
                    SetTitle(displayName);
                }
            }

        }

        /// <summary>
        /// Load modification into list of lyrics
        /// Recharge la liste localpLyrics avec les données de la gridview
        /// </summary>
        private void LoadModifiedLyrics()
        {
            int plTime = 0;
            string val = string.Empty;
            string plType = string.Empty;
            string plElement = string.Empty;
            string plReplace = string.Empty;


            localplLyrics = new List<pictureBoxControl.plLyric>();

            for (int row = 0; row < dgView.Rows.Count; row++)
            {
                if (dgView.Rows[row].Cells[COL_TICKS].Value != null)
                {
                    if (dgView.Rows[row].Cells[COL_TICKS].Value != null && dgView.Rows[row].Cells[COL_TICKS].Value.ToString() != "")
                        plTime = Convert.ToInt32(dgView.Rows[row].Cells[COL_TICKS].Value);
                    else
                        plTime = 0;

                    // Type
                    if (dgView.Rows[row].Cells[COL_TYPE].Value != null)
                    {
                        val = dgView.Rows[row].Cells[COL_TYPE].Value.ToString();
                        if (val == "text" || val == "cr")
                            plType = val;
                        else
                            plType = "text";
                    }
                    else
                    {
                        plType = "text";
                    }

                    // Element
                    if (dgView.Rows[row].Cells[COL_REPLACE].Value != null)
                    {
                        if (plType == "cr")
                            plElement = "\r";
                        else
                            plElement = dgView.Rows[row].Cells[COL_REPLACE].Value.ToString();
                    }
                    else
                        plElement = "text";

                    // replace again spaces
                    plElement = plElement.Replace("_", " ");
                    localplLyrics.Add(new pictureBoxControl.plLyric() { Type = plType, Element = plElement, Time = plTime });
                }
            }

        }

        /// <summary>
        /// Replace lyrics in frmPlayer
        /// Appelle la méthode ReplaceLyrics de frmPlayer
        /// </summary>
        private void ReplaceLyrics()
        {
            string ltype;

            if (TextLyricFormat == LyricFormat.Text)
                ltype = "text";
            else
                ltype = "lyric";

            if (Application.OpenForms.OfType<frmPlayer>().Count() > 0)
            {
                frmPlayer frmPlayer = GetForm<frmPlayer>();
                frmPlayer.ReplaceLyrics(localplLyrics, ltype, melodytracknum);
            }

        }

        /// <summary>
        /// Locate form
        /// </summary>
        /// <typeparam name="TForm"></typeparam>
        /// <returns></returns>
        private TForm GetForm<TForm>()
            where TForm : Form
        {
            return (TForm)Application.OpenForms.OfType<TForm>().FirstOrDefault();
        }

        /// <summary>
        /// Display modifications into a textbox
        /// </summary>
        /// <param name="lLyrics"></param>
        private void PopulateTextBox(List<pictureBoxControl.plLyric> lLyrics)
        {
            string plElement = string.Empty;
            string plType = string.Empty;
            string tx = string.Empty;

            for (int i = 0; i < lLyrics.Count; i++)
            {
                // Affiche les blancs
                plElement = lLyrics[i].Element;
                plElement = plElement.Replace("\r", "\r\n");

                plType = lLyrics[i].Type;

                tx += plElement;

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
            string s = string.Empty;
            for (int row = 0; row < r; row++)
            {
                s = dgView.Rows[row].Cells[COL_REPLACE].Value.ToString();
                if (s == "" && dgView.Rows[row].Cells[COL_TYPE].Value != null && dgView.Rows[row].Cells[COL_TYPE].Value.ToString() == "cr")
                    s = "\r";
                s = s.Replace("_", " ");
                s = s.Replace("\r", "\n");
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
            int plTime = 0;
            int n = 0;
            int averageDuration = 0;
            int Duration = 0;
            int H = 0;
            int H0 = 22;
            int newH = 0;
            int delta = 0;
            int previousTime = 0;

            // Average duration
            for (int row = 0; row < dgView.Rows.Count; row++)
            {
                if (dgView.Rows[row].Cells[COL_TICKS].Value != null)
                {
                    plTime = Convert.ToInt32(dgView.Rows[row].Cells[COL_TICKS].Value);
                    if (previousTime == 0)
                    {
                        previousTime = plTime;
                    }
                    else
                    {
                        if (plTime > previousTime)
                        {
                            averageDuration += (plTime - previousTime);
                            previousTime = plTime;
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
                if (dgView.Rows[row].Cells[COL_TICKS].Value != null)
                {
                    plTime = Convert.ToInt32(dgView.Rows[row].Cells[COL_TICKS].Value);
                    if (plTime > 0)
                    {
                        if (previousTime == 0)
                        {
                            previousTime = plTime;
                        }
                        else if (plTime > previousTime)
                        {
                            H = dgView.Rows[row].Height;
                            Duration = plTime - previousTime;
                            delta = Duration / averageDuration;

                            if (delta > 0)
                            {
                                newH = H + H * delta;
                                if (newH > 5 * H0)
                                    newH = 5 * H0;
                                dgView.Rows[row].Height = newH;
                            }
                            previousTime = plTime;
                        }
                    }
                }
            }
        }


        #endregion functions


        #region Option lyrics format
        private void OptFormatText_CheckedChanged(object sender, EventArgs e)
        {
            if (optFormatText.Checked)
                TextLyricFormat = LyricFormat.Text;
        }

        private void OptFormatLyrics_CheckedChanged(object sender, EventArgs e)
        {
            if (optFormatLyrics.Checked)
                TextLyricFormat = LyricFormat.Lyric;
        }

        #endregion

        private void frmLyricsEdit_Resize(object sender, EventArgs e)
        {
            ResizeMe();
        }

        private void ResizeMe()
        {
            btnInsertCr.Left = 3;
            btnInsertCr.Top = 3;

            btnInsertText.Left = 3;
            btnInsertText.Top = 32;

            btnDelete.Left = 3;
            btnDelete.Top = 61;

            btnSpaceLeft.Left = 112;
            btnSpaceLeft.Top = 3;

            btnSpaceRight.Left = 112;
            btnSpaceRight.Top = 32;


            btnView.Left = 176;
            btnView.Top = 3;

            btnSave.Left = 176;
            btnSave.Top = 32;

            btnPlay.Left = 176;
            btnPlay.Top = 61;

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

        private void BtnFontPlus_Click(object sender, EventArgs e)
        {
            float emSize = txtResult.Font.Size;
            emSize++;
            txtResult.Font = new Font(txtResult.Font.FontFamily, emSize);           
        }

        private void BtnFontMoins_Click(object sender, EventArgs e)
        {
            float emSize = txtResult.Font.Size;
            emSize--;
            if (emSize > 5)            
                txtResult.Font = new Font(txtResult.Font.FontFamily, emSize);
        }
    }
}

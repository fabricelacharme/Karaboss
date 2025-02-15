﻿using Karaboss.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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

        public frmMp3LyricsEdit(string FileName)
        {
            InitializeComponent();

            _filename = FileName;

            // Inits
            InitTxtResult();
            InitGridView();

            PopulateDataGridView();
        }

        #region Form Load and Close
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

        /// <summary>
        /// Populate gridview with lyrics
        /// </summary>
        private void PopulateDataGridView()
        {

            SyncText[] SyncLyrics = Mp3LyricsMgmtHelper.SyncTexts;

            SynchronisedLyricsFrame SynchedLyrics = Mp3LyricsMgmtHelper.MySyncLyricsFrame;
            
            if (SynchedLyrics != null)
            {
                for (int i = 0; i < SynchedLyrics.Text.Count(); i++)
                {
                    string text = SynchedLyrics.Text[i].Text;
                    text = text.Replace("\r\n", m_SepLine);
                    text = text.Replace("\r", m_SepLine);
                    text = text.Replace("\n", m_SepLine);

                    text = text.Replace(" ", "_");

                    long time = SynchedLyrics.Text[i].Time;
                    dgView.Rows.Add(time, text);
                }
            }
            else
            {
                for (int i = 0; i < SyncLyrics.Length; i++)
                {
                    long time = SyncLyrics[i].Time;
                    string text = SyncLyrics[i].Text;
                    dgView.Rows.Add(time, text);
                }
            }
        }


        #endregion Populate gridview


        #region Menu File
        /// <summary>
        /// Menu File Quit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuFileQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion Menu File


        #region buttons
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

        }

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


            }
            catch (Exception Ex)
            {
                string message = "Error : " + Ex.Message;
                MessageBox.Show(message, "Error deleting line",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion buttons


        #region init

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
        }

        #endregion init


        #region Save Lyrics

        /// <summary>
        /// Button: save lyrics
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
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

            string mp3file = Files.FindUniqueFileName(_filename);

            saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Mp3 files (*.mp3)|*.mp3|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.InitialDirectory = Path.GetDirectoryName(mp3file);
            saveFileDialog.FileName = mp3file;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                
                string filename = saveFileDialog.FileName;

                // Copy file to another name                
                System.IO.File.Copy(_filename, filename, true);

                // Save sync lyrics into the copy of initial file
                SaveFrame(filename);
            }


            
            return;

            saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Lyrics files (*.lrc)|*.lrc|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.InitialDirectory = Path.GetDirectoryName(_filename);
            saveFileDialog.FileName = Path.GetFileNameWithoutExtension(_filename) + ".lrc";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = saveFileDialog.FileName;
                SaveLyricsToText(SyncTexts, filename);
            }

        }

        /// <summary>
        /// Save to text file
        /// </summary>
        /// <param name="SyncTexts"></param>
        /// <param name="filename"></param>
        private void SaveLyricsToText(SyncText[] SyncTexts, string filename)
        {
            try
            {
                // Save to text file
                for (int i = 0; i < SyncTexts.Length; i++)
                {
                    if (SyncTexts[i].Text != null && SyncTexts[i].Text != "")
                    {
                        string line = SyncTexts[i].Time + " " + SyncTexts[i].Text + "\r\n";
                        System.IO.File.AppendAllText(filename, line);
                    }
                }

                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving lyrics: " + ex.Message);
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
            Mp3LyricsMgmtHelper.MySyncLyricsFrame.Text = new SynchedText[dgView.RowCount - 1];
            
            // Read all rows and store into the frame
            for (int i = 0; i < dgView.RowCount - 1; i++)
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

            Mp3LyricsMgmtHelper.SetTags(FileName, Mp3LyricsMgmtHelper.MySyncLyricsFrame);
        }


        #endregion Save Lyrics

    }
}

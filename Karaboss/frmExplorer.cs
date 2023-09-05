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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sanford.Multimedia.Midi;
using System.IO; // debug
using Karaboss.Resources.Localization;
using Karaboss.Configuration;
using Sanford.Multimedia.Midi.UI;
using PrgAutoUpdater;
using System.Net;
using System.Xml;
using Karaboss.Pages.ABCnotation;
using Karaboss.Mru;

namespace Karaboss
{

    public partial class frmExplorer : Form
    {

        #region configuration
        private ConfigurationForm m_configurationForm;

        #endregion configuration
     
        private string songRoot;        // Directory of songs
        private string CurrentPath;     // Current file full path          

        #region MIDI devices
        // Sound input, output devices 
        private OutputDevice outDevice;
        private int outDeviceID = 0;
        private InputDevice inDevice;
        private int inDeviceID = 0;
        #endregion

        #region delete directories
        private bool bQuestionDelete = false;   // Delete empty directories question asked
        private bool bAbortDelete = false;      // Abort delete directories
        private int iNbDelete = 0;              // Number of deleted directories
        #endregion

        private string commandlinePath = string.Empty;
        public int NumInstance = 1;     
        private DisplayMidiInfos MidiInfos;

        // The MruList
        int MruFilesCount = 10;
        MruList MyMruList;

        public frmExplorer(string[] args, int numinstance)
        {
            InitializeComponent();

            lblItags.Font = new Font("Segoe UI", 9F); 
            lblKtags.Font = new Font("Segoe UI", 9F);
            lblLtags.Font = new Font("Segoe UI", 9F);
            lblTtags.Font = new Font("Segoe UI", 9F);
            lblVtags.Font = new Font("Segoe UI", 9F);
            lblWtags.Font = new Font("Segoe UI", 9F);


            MidiInfos = new DisplayMidiInfos
            {
                busy = false
            };

            #region sideBarControl events             

            // The left bar  
            sideBarControl.DisplayHome += new EventHandler(SideBarControl_DisplayHome);
            sideBarControl.DisplaySearch += new EventHandler(SideBarControl_DisplaySearch);
            sideBarControl.DisplayFiles += new EventHandler(SideBarControl_DisplayFiles);
            sideBarControl.DisplayPlaylists += new EventHandler(SideBarControl_DisplayPlaylists);
            sideBarControl.DisplayConnected += new EventHandler(SideBarControl_DisplayConnected);

            sideBarControl.PlayFile += new EventHandler(SideBarControl_PlayFile);
            sideBarControl.EditFile += new EventHandler(SideBarControl_EditFile);
            sideBarControl.DisplayPianoTraining += new EventHandler(SideBarControl_DisplayPianoTraining);
            sideBarControl.DisplayGuitarTraining += new EventHandler(SideBarControl_DisplayGuitarTraining);

            #endregion

            #region SearchControl events            
            // The SEARCH page
            searchControl.SelectedIndexChanged += new Search.SelectedIndexChangedEventHandler(Global_SelectedIndexChanged);
            searchControl.PlayMidi += new Search.PlayMidiEventHandler(Global_PlayMidi);
            searchControl.PlayCDG += new Search.PlayCDGEventHandler(Global_PlayCDG);
            searchControl.SearchContentChanged += new Search.ContentChangedEventHandler(Search_ContentChanged);
            searchControl.NavigateTo += new Search.NavigateToEventHandler(Item_NavigateTo);
            searchControl.SongRootChanged += new Search.SongRootChangedEventHandler(Search_SongRootChanged);
            #endregion

            #region explorerControl events
            // The EXPLORER page           
            xplorerControl.SelectedIndexChanged += new xplorer.SelectedIndexChangedEventHandler(Global_SelectedIndexChanged);            
            xplorerControl.PlayMidi += new xplorer.PlayMidiEventHandler(Global_xPlayMidi);
            xplorerControl.PlayCDG += new xplorer.PlayCDGEventHandler(Global_PlayCDG);
            xplorerControl.PlayText += new xplorer.PlayTextEventHandler(Global_xPlayText);
            xplorerControl.LvContentChanged += new xplorer.ContentChangedEventHandler(Xplorer_ContentChanged);
            xplorerControl.CreateNewMidiFile += new xplorer.CreateNewMidiFileEventHandler(Xplorer_CreateNewMidiFile);

            #endregion          

            #region playlistsControl events
            // The PLAYLISTS page
            playlistsControl.SelectedIndexChanged += new playlists.SelectedIndexChangedEventHandler(Global_SelectedIndexChanged);
            playlistsControl.PlayMidi += new playlists.PlayMidiEventHandler(Global_PlayMidi);
            playlistsControl.PlayCDG += new playlists.PlayCDGEventHandler(Global_PlayCDG);
            playlistsControl.PlayText += new playlists.PlayTextEventHandler(Global_PlayText);
            playlistsControl.NavigateTo += new playlists.NavigateToEventHandler(Item_NavigateTo);
            #endregion


            // Display explorer page at startup
            searchControl.Visible = false;            
            playlistsControl.Visible = false;
            xplorerControl.Visible = true;
            
            connectedControl.Visible = false;
                        
            // Load saved configuration
            LoadSavedValues();                    

            NumInstance = numinstance;
            if (NumInstance > 1)
                Text = Text + " (" + NumInstance + ")";

            // Display songs directory on PlacesToolBar
            DisplaySongsDirectory();

            // Tooltips
            string eop = songRoot.Split('\\').Last();
            sideBarControl.ToolTipTextHome = "Songs library (" + eop + ")";

            // Display controls on the form
            ResizeElements();
            

            // Manage arguments when launched by double clicking on a file in the Windows Explorer            
            if (args.Length > 0)
            {                
                commandlinePath = args[0];               
            }

        }


        #region Content Changed
        /// <summary>
        /// Manage event songroot changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="path"></param>
        private void Search_SongRootChanged(object sender, string path)
        {
            songRoot = path;
            string eop = songRoot.Split('\\').Last();
            sideBarControl.ToolTipTextHome = "Songs library (" + eop + ")";
        }

        /// <summary>
        /// Open folder option of Search & Playlists
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="path"></param>
        private void Item_NavigateTo(object sender, string path, string file = "")
        {
            sideBarControl.SelectedItem = VBarControl.SideBarControl.SideBarControl.Selectables.Files;

            xplorerControl.Visible = true;
            playlistsControl.Visible = false;
            searchControl.Visible = false;
           
            path = "file:///" + path.Replace("\\", "/");
            xplorerControl.Navigate(path, file);
        }

        private void Search_ContentChanged(object sender, string strContent)
        {
            tssMiddle.Text = strContent;
        }

        private void Xplorer_ContentChanged(object sender, string strContent, string strPath)
        {
            tssMiddle.Text = strContent;
            tssLeft.Text = strPath;
        }

        #endregion


        #region sideBarControl events

        /// <summary>
        /// Display Home folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SideBarControl_DisplayHome(object sender, EventArgs e)
        {
            songRoot = GetSongsDirectory();
            string songRootUri = "file:///" + songRoot.Replace("\\", "/");
            xplorerControl.Navigate(songRootUri);

            xplorerControl.Visible = true;
            
            playlistsControl.Visible = false;
            searchControl.Visible = false;
            connectedControl.Visible = false;
            pnlFileInfos.Visible = true;

            ShowPlayEditButtons(true);            

            xplorerControl.Refresh();            

            tssLeft.Text = xplorerControl.CurrentFolder;
            tssMiddle.Text = xplorerControl.CurrentContent;

            InitDisplayMidiFileInfos(xplorerControl.SelectedFile);

        }

        private void ShowPlayEditButtons(bool bVisible)
        {
            sideBarControl.ShowButton("btnPlay", bVisible);
            sideBarControl.ShowButton("btnEdit", bVisible);
            sideBarControl.ShowButton("btnPianoTraining", bVisible);
            sideBarControl.ShowButton("btnGuitarTraining", bVisible);
        }

        /// <summary>
        /// Display search page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SideBarControl_DisplaySearch(object sender, EventArgs e)
        {
            DisplaySearch();

            searchControl.Visible = true;
            xplorerControl.Visible = false;
            playlistsControl.Visible = false;
            connectedControl.Visible = false;
            pnlFileInfos.Visible = true;
            mnuEdit.Visible = false;

            ShowPlayEditButtons(true);

            searchControl.Refresh();
            
        }

        private void DisplaySearch()
        {
            tssLeft.Text = "Ready";
            tssMiddle.Text = "";

            songRoot = GetSongsDirectory();

            searchControl.SongRoot = songRoot;

            // Bibliothèque
            DisplaySongsDirectory();

            InitDisplayMidiFileInfos(searchControl.SelectedFile);
        }

        /// <summary>
        /// Display Explorer page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SideBarControl_DisplayFiles(object sender, EventArgs e)
        {            
            xplorerControl.Visible = true;
            playlistsControl.Visible = false;
            searchControl.Visible = false;
            connectedControl.Visible = false;
            pnlFileInfos.Visible = true;
            mnuEdit.Visible = true;

            ShowPlayEditButtons(true);

            xplorerControl.Refresh();

            tssLeft.Text = xplorerControl.CurrentFolder;
            tssMiddle.Text = xplorerControl.CurrentContent;

            InitDisplayMidiFileInfos(xplorerControl.SelectedFile);

        }

        /// <summary>
        /// Display playlists page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SideBarControl_DisplayPlaylists(object sender, EventArgs e)
        {
            tssLeft.Text = "Ready";
            tssMiddle.Text = "";

            playlistsControl.Visible = true;
            xplorerControl.Visible = false;
            searchControl.Visible = false;
            connectedControl.Visible = false;
            pnlFileInfos.Visible = true;
            mnuEdit.Visible = false;

            ShowPlayEditButtons(true);

            playlistsControl.Refresh();

        }

        private void SideBarControl_DisplayConnected(object sender, EventArgs e)
        {
            DisplayConnected();
        }

        private void DisplayConnected()
        {
            connectedControl.Visible = true;
            xplorerControl.Visible = false;
            playlistsControl.Visible = false;
            searchControl.Visible = false;
            mnuEdit.Visible = false;

            ShowPlayEditButtons(false);

            tssLeft.Text = "Ready";
            tssMiddle.Text = "";

            pnlFileInfos.Visible = false;
        }

        /// <summary>
        /// Display the player MIDI or CDG according to file extension
        /// and launch play
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SideBarControl_PlayFile(object sender, EventArgs e)
        {
            string filename = string.Empty;

            if (searchControl.Visible)            
                filename = searchControl.SelectedFile;            
            else if (xplorerControl.Visible)
            {
                if (xplorerControl.SelectedItems.Length == 1)
                    filename = xplorerControl.SelectedItems[0].FileSystemPath;                
            }
            else if (playlistsControl.Visible)
                filename = playlistsControl.SelectedFile;
            

            if (filename != null && filename != "" && File.Exists(filename))
            {
                //DisplayMidiPlayer(filename, null, true);
                SelectPlayer(filename, true);
            }
            else
            {
                MessageBox.Show("Please select a file", "Karaboss", MessageBoxButtons.OK);
            }

        }

        /// <summary>
        /// Display the player MIDI or CDG according to file extension
        ///  and does not launch play to allow edit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SideBarControl_EditFile(object sender, EventArgs e)
        {
            string filename = string.Empty;

            if (searchControl.Visible)
                filename = searchControl.SelectedFile;
            else if (xplorerControl.Visible)
            {
                if (xplorerControl.SelectedItems.Length == 1)
                    filename = xplorerControl.SelectedItems[0].FileSystemPath;
            }
            else if (playlistsControl.Visible)
                filename = playlistsControl.SelectedFile;
            

            if (filename != null && filename != "" && File.Exists(filename))
            {                
                SelectPlayer(filename, false);
            }
            else
            {
                MessageBox.Show("Please select a file", "Karaboss", MessageBoxButtons.OK);
            }

        }

        /// <summary>
        /// Display the piano training page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SideBarControl_DisplayPianoTraining(object sender, EventArgs e)
        {            
            DisplayPianoTraining();
        }

        private void DisplayPianoTraining()
        {
            string filename = string.Empty;
            if (searchControl.Visible)
                filename = searchControl.SelectedFile;
            else if (xplorerControl.Visible)
            {
                if (xplorerControl.SelectedItems.Length == 1)
                    filename = xplorerControl.SelectedItems[0].FileSystemPath;
            }
            else if (playlistsControl.Visible)
                filename = playlistsControl.SelectedFile;

            if (filename == null || filename == "" || !File.Exists(filename) || !Karaclass.IsMidiExtension(filename))
            {
                MessageBox.Show("Please select a file", "Karaboss", MessageBoxButtons.OK);
                return;
            }

            // ferme le formulaire frmPlayer
            if (Application.OpenForms.OfType<frmPlayer>().Count() > 0)
            {
                Application.OpenForms["frmPlayer"].Close();
            }

            // Ferme le formulaire frmPianoTraining            
            if (Application.OpenForms["frmPianoTraining"] != null)
                Application.OpenForms["frmPianoTraining"].Close();

            // Ferme le formulaire frmGuitarTraining            
            if (Application.OpenForms["frmGuitarTraining"] != null)
                Application.OpenForms["frmGuitarTraining"].Close();

            ResetOutPutDevice();
            // Affiche le formulaire frmPianoTraining 
            Form frmPianoTraining = new frmPianoTraining(outDevice, filename);
            frmPianoTraining.Show();            
            frmPianoTraining.Activate();
        }

        /// <summary>
        /// Display the guitar training page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SideBarControl_DisplayGuitarTraining(object sender, EventArgs e)
        {
            DisplayGuitarTraining();
        }

        private void DisplayGuitarTraining()
        {
            string filename = string.Empty;
            if (searchControl.Visible)
                filename = searchControl.SelectedFile;
            else if (xplorerControl.Visible)
            {
                if (xplorerControl.SelectedItems.Length == 1)
                    filename = xplorerControl.SelectedItems[0].FileSystemPath;
            }
            else if (playlistsControl.Visible)
                filename = playlistsControl.SelectedFile;

            if (filename == null || filename == "" || !File.Exists(filename) || !Karaclass.IsMidiExtension(filename))
            {
                MessageBox.Show("Please select a file", "Karaboss", MessageBoxButtons.OK);
                return;
            }

            // ferme le formulaire frmPlayer
            if (Application.OpenForms.OfType<frmPlayer>().Count() > 0)
            {
                Application.OpenForms["frmPlayer"].Close();
            }

            // Ferme le formulaire frmPianoTraining            
            if (Application.OpenForms["frmPianoTraining"] != null)
                Application.OpenForms["frmPianoTraining"].Close();

            // Ferme le formulaire frmGuitarTraining            
            if (Application.OpenForms["frmGuitarTraining"] != null)
                Application.OpenForms["frmGuitarTraining"].Close();


            ResetOutPutDevice();
            // Affiche le formulaire frmGuitarTraining 
            Form frmGuitarTraining = new frmGuitarTraining(outDevice, filename);
            frmGuitarTraining.Show();            
            frmGuitarTraining.Activate();
        }


        /// <summary>
        /// Open a file selected from the MRU list.
        /// </summary>
        /// <param name="file_name"></param>
        private void MyMruList_FileSelected(string FileName)
        {
            OpenMruFile(FileName);
        }

        private void OpenMruFile(string FullPath)
        {
            try
            {
                
                sideBarControl.SelectedItem = VBarControl.SideBarControl.SideBarControl.Selectables.Files;
                xplorerControl.Visible = true;
                playlistsControl.Visible = false;
                searchControl.Visible = false;

                string path = Path.GetDirectoryName(FullPath);

                //path must be like "file:///c:/users/a453868/Music/karaoke/sasin";
                path = "file:///" + path.Replace("\\", "/");
                string file = Path.GetFileName(FullPath);
                xplorerControl.Navigate(path, file);

                // Add the file to the MRU list.
                MyMruList.AddFile(FullPath);

            }
            catch (Exception ex)
            {
                // Remove the file from the MRU list.
                MyMruList.RemoveFile(FullPath);

                // Tell the user what happened.
                MessageBox.Show(ex.Message);
            }

        }

        #endregion


        #region playListsControl events

        /// <summary>
        /// Call by frmPlayer to display the current song played in a playlist
        /// </summary>
        /// <param name="song"></param>
        public void DisplaySong(string song)
        {
            playlistsControl.DisplaySong(song);
        }       

        #endregion


        #region functions

        /// <summary>
        /// Manage line arguments
        /// </summary>
        /// <param name="args"></param>
        public void UseNewArguments(string[] args)
        {           
            if (args.Length > 0)
            {                
                string fullpath = args[0];

                if (File.Exists(fullpath))
                {
                    string path = Path.GetDirectoryName(fullpath);
                    //path must be like "file:///c:/users/a453868/Music/karaoke/sasin";
                    path = "file:///" + path.Replace("\\", "/");
                    
                    //Navigate to the path of the double-clicked file
                    xplorerControl.Navigate(path);

                    // Display Midi player if the extension is .mid or .kar
                    //DisplayMidiPlayer(fullpath, null, true);
                    SelectPlayer(fullpath, true);
                }
            }
        }

            
        /// <summary>
        /// Display the library
        /// </summary>
        public void DisplaySongsDirectory()
        {
            string itempath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            FlShell.ShellItem shi;

            if (songRoot != Environment.GetFolderPath(Environment.SpecialFolder.MyMusic))
            {
                itempath = "file:///" + songRoot.Replace("\\", "/");
                shi = new FlShell.ShellItem(itempath);
                
            }
            else
            {
                shi = new FlShell.ShellItem(Environment.SpecialFolder.MyMusic);
            }

            playlistsControl.SongRoot = songRoot;

            
        }

        /// <summary>
        /// Retrieve root directory to search songs in the saved parameters
        /// </summary>
        /// <returns></returns>
        private string GetSongsDirectory()
        {
            string inipath = string.Empty;
            inipath = Properties.Settings.Default.SongRoot;

            // Cas de la première fois (string empty)
            if (inipath == null || inipath == "" || inipath == "C:\\\\" || Directory.Exists(inipath) == false)
                inipath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

            //if (songRoot == null || songRoot == "" || songRoot == "C:\\\\" || Directory.Exists(songRoot) == false)
            //    songRoot = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

            return inipath;
        }

        private void LoadSavedValues()
        {
            Karaclass.m_textEncoding = Properties.Settings.Default.textEncoding;
            Karaclass.m_lang = Properties.Settings.Default.lang;
            Karaclass.m_MuteMelody = Properties.Settings.Default.MuteMelody;
            Karaclass.m_DisplayBalls = Properties.Settings.Default.DisplayBalls;
            Karaclass.M_filePlaylistGroups = Properties.Settings.Default.filePlaylistGroups;
            Karaclass.m_drivePlaylists = Properties.Settings.Default.drivePlaylists;
            Karaclass.m_CountdownSongs = Properties.Settings.Default.CountdownSongs;
            Karaclass.m_PauseBetweenSongs = Properties.Settings.Default.bPauseBetweenSongs;
            Karaclass.m_TransposeAmount = Properties.Settings.Default.TransposeAmount;
            Karaclass.m_Velocity = Properties.Settings.Default.Velocity;

            Karaclass.m_SepLine = Properties.Settings.Default.SepLine;
            Karaclass.m_SepSyllabe = Properties.Settings.Default.SepSyllabe;
            Karaclass.m_SepParagraph = Properties.Settings.Default.SepParagraph;
            Karaclass.m_ShowParagraph = Properties.Settings.Default.bShowParagraph;
            Karaclass.m_ForceUppercase = Properties.Settings.Default.bForceUppercase;

            Karaclass.m_SaveDefaultOutputDevice = Properties.Settings.Default.SaveDefaultOutputDevice;

            // Listview column length
            int l = Properties.Settings.Default.lvFileNameColumn;
            xplorerControl.SetColumnWidth(0, l);

            // Directory of songs
            songRoot = GetSongsDirectory();

        }


        #endregion functions


        #region Midi

        /// <summary>
        /// Bouton xplorercontrol create new midi file
        /// </summary>
        /// <param name="sender"></param>
        private void Xplorer_CreateNewMidiFile(object sender)
        {
            NewMidiFile();
        }


        /// <summary>
        /// Same proc for xplorer, search and playlist controls to display MIDI infos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="fileName"></param>
        private void Global_SelectedIndexChanged(object sender, string fileName)
        {
            MidiInfos.Clear();
            if (fileName != null && fileName != "")
            {
                InitDisplayMidiFileInfos(fileName);

            }
            else
                DisplayMidiLabels();

        }
       

        /// <summary>
        /// Initialize the display of internal infos of a MIDI file in the bottom panel
        /// </summary>
        /// <param name="fileName"></param>
        private void InitDisplayMidiFileInfos(string fileName)
        {
            if (Karaclass.IsMidiExtension(fileName) && MidiInfos.busy == false)
            {               
                MidiInfos.busy = true;                             
                ResetMidiFile();                

                // load file and display MIDI infos
                CurrentPath = fileName;
                LoadAsyncMidiFile(fileName);
            }
            else
            {
                MidiInfos.Clear();
                DisplayMidiLabels();
            }
        }
       
        private void ResetMidiFile()
        {
            OpenMidiFileOptions.TextEncoding = Karaclass.m_textEncoding;           
            OpenMidiFileOptions.SplitHands = false;
        }

        /// <summary>
        /// Load async the midi file in the sequencer
        /// wait for the event HandleLoadCompleted
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadAsyncMidiFile(string fileName)
        {
            try
            {
                if (fileName != "\\")
                {
                    sequence1.LoadAsync(fileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }


        /// <summary>
        /// Event: sequence loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleLoadCompleted(object sender, AsyncCompletedEventArgs e)
        {           
            string tx = string.Empty;
            int i;
            string cr = Environment.NewLine;

            // Remove all MIDI events after last note
            sequence1.Clean();

            // song duration
            #region song duration
            double ppqn = sequence1.Division;
            double totalticks = sequence1.GetLength();
            double tempo = sequence1.Tempo;
            double duration = tempo * (totalticks / ppqn) / 1000000; //seconds

            int Min = (int)(duration / 60);
            int Sec = (int)(duration - (Min * 60));                    
            
            
            MidiInfos.Tracks = sequence1.tracks.Count;
            MidiInfos.Format = sequence1.OrigFormat;
            MidiInfos.Duration = string.Format("{0:00}:{1:00}", Min, Sec);
            MidiInfos.Lyrics = sequence1.HasLyrics;
            #endregion

            // Karaoke tags
            #region tags
            if (sequence1.KTag != null)
            {
                tx = "";
                for (i = 0; i < sequence1.KTag.Count; i++)
                {
                    tx += sequence1.KTag[i] + cr;
                }
                MidiInfos.KTags = tx;
            }

            // Version
            if (sequence1.VTag != null)
            {
                tx = "";
                for (i = 0; i < sequence1.VTag.Count; i++)
                {
                    tx += sequence1.VTag[i] + cr;
                }
                MidiInfos.VTags = tx;
            }

            // Lang
            if (sequence1.LTag != null)
            {
                tx = "";
                for (i = 0; i < sequence1.LTag.Count; i++)
                {
                    tx += sequence1.LTag[i] + cr;
                }
                MidiInfos.LTags = tx;
            }

            // Copyright of karaoke
            if (sequence1.WTag != null)
            {
                tx = "";
                for (i = 0; i < sequence1.WTag.Count; i++)
                {
                    tx += sequence1.WTag[i] + cr;
                }
                MidiInfos.WTags = tx;
            }

            // Song infos
            if (sequence1.TTag != null)
            {
                tx = "";
                for (i = 0; i < sequence1.TTag.Count; i++)
                {
                    tx += sequence1.TTag[i].Replace('\n', ' ').Replace('\r', ' ') + cr;
                }
                MidiInfos.TTags = tx;
            }

            // Infos
            if (sequence1.ITag != null)
            {
                tx = "";
                for (i = 0; i < sequence1.ITag.Count; i++)
                {
                    tx += sequence1.ITag[i].Replace('\n', ' ').Replace('\r', ' ') + cr;
                }
                MidiInfos.ITags = tx;
            }

            #endregion

            // display infos on bottom panel
            DisplayMidiLabels();

            if (playlistsControl.Visible)
                playlistsControl.SelectedFileLength = MidiInfos.Duration;

            // Allow new search Midi file infos
            MidiInfos.busy = false;
        }
        
        
        /// <summary>
        /// Display Midi information on bottom panel
        /// </summary> 
        private void DisplayMidiLabels()
        {
            try
            {

                string fileName = CurrentPath;

                // TODO
                // https://codingly.com/2008/08/04/invokerequired-invoke-synchronizationcontext/
                // Opération inter-threads non valide : le contrôle ‘MyControl’ a fait l’objet d’un accès 
                // à partir d’un thread autre que celui sur lequel il a été créé.
                if (this.InvokeRequired)
                {
                    this.Invoke((MethodInvoker) DisplayMidiLabels);
                    return;
                }

                lblTracks.Text = "Tracks: " + MidiInfos.Tracks.ToString();
                lblFormat.Text = "Format: " + MidiInfos.Format.ToString(); ;
                lblDuration.Text = "Duration: " + MidiInfos.Duration;
                if (MidiInfos.Lyrics)
                {
                    if (Path.GetExtension(fileName).ToLower() == ".mid")
                        lblLyrics.ForeColor = Color.Red;
                    else
                        lblLyrics.ForeColor = Color.Black;

                    lblLyrics.Text = "Lyrics: " + "Yes";
                }
                else
                {
                    lblLyrics.ForeColor = Color.Black;
                    lblLyrics.Text = "Lyrics: " + "No";
                }

                lblItags.Text = MidiInfos.ITags;
                lblKtags.Text = MidiInfos.KTags;
                lblLtags.Text = MidiInfos.LTags;
                lblTtags.Text = MidiInfos.TTags;
                lblVtags.Text = MidiInfos.VTags;
                lblWtags.Text = MidiInfos.WTags;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        #endregion midi


        #region Players     

        // Specific xplorerControl    
        // Lauch a file from explorer, no playlist
        private void Global_xPlayMidi(object sender, FileInfo fi, bool bplay)
        {
            DisplayMidiPlayer(fi.FullName, null, bplay);
        }

        private void Global_xPlayText(object sender, FileInfo fi, bool bplay)
        {
            DisplayTextPlayer(fi.FullName, null, bplay);
        }

        // Specific explorerControl

        private void Global_PlayCDG(object sender, FileInfo fi, bool bplay)
        {
            string fpath = fi.FullName;
            LaunchCDGPlayer(fpath, true);
        }

        private void Global_PlayMidi(object sender, FileInfo fi, Playlist pl, bool bplay)
        {
            DisplayMidiPlayer(fi.FullName, pl, bplay);
        }

        private void Global_PlayText(object sender, FileInfo fi, Playlist pl, bool bplay)
        {
            DisplayTextPlayer(fi.FullName, pl, bplay);
        }



        /// <summary>
        /// Display the CDG player
        /// </summary>
        /// <param name="fpath"></param>
        /// <param name="fname"></param>
        /// <param name="bPlayNow"></param>
        private void LaunchCDGPlayer(string fpath, bool bPlayNow)
        {            
            if (fpath != null)
            {
                if (File.Exists(fpath) == false)
                {
                    MessageBox.Show("The file " + fpath + " doesn not exists!", "Karaboss", MessageBoxButtons.OK);                    
                    return;
                }
            }

            Cursor.Current = Cursors.WaitCursor;

            // Affiche le formulaire frmCDGPlayer 
            if (Application.OpenForms["frmCDGPlayer"] == null)
            {
                Form frmCDGPlayer = new frmCDGPlayer(fpath);
                frmCDGPlayer.Show();
            }
            else
            {
                Application.OpenForms["frmCDGPlayer"].Close();
                Form frmCDGPlayer = new frmCDGPlayer(fpath);
                frmCDGPlayer.Show();
            }
        }

        /// <summary>
        /// Displat ABC, MML text player
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="bPlayNow"></param>
        private void DisplayTextPlayer(string fpath, Playlist pl, bool bPlayNow)
        {
            if (fpath == null)
                return;
            
            // edit or play a midi file
            if (fpath != "new file")
            {
                // Launch an existing file                    
                if (File.Exists(fpath) == false)
                {
                    MessageBox.Show("The file " + fpath + " doesn not exists!", "Karaboss", MessageBoxButtons.OK);
                    return;
                }
            }
            else
            {
                // create a new file
                fpath = null;
            }

            Cursor.Current = Cursors.WaitCursor;

            #region Close Windows
            // ferme le formulaire frmPianoTraining
            if (Application.OpenForms.OfType<frmPianoTraining>().Count() > 0)
            {
                Application.OpenForms["frmPianoTraining"].Close();
            }

            // ferme le formulaire frmGuitarTraining
            if (Application.OpenForms.OfType<frmGuitarTraining>().Count() > 0)
            {
                Application.OpenForms["frmGuitarTraining"].Close();
            }

            // ferme le formulaire frmPlayer
            if (Application.OpenForms.OfType<frmPlayer>().Count() > 0)
            {
                Application.OpenForms["frmPlayer"].Close();
            }

            #endregion

            // Affiche le formulaire frmPlay 
            if (Application.OpenForms["FrmTextPlayer"] != null)
                Application.OpenForms["FrmTextPlayer"].Close();

            ResetOutPutDevice();
           
            Form frmTextPlayer = new Karaboss.Pages.ABCnotation.FrmTextPlayer(outDevice, fpath, bPlayNow);
            frmTextPlayer.Show();
            frmTextPlayer.Activate();

        }

        /// <summary>
        /// Display the Midi player
        /// </summary>
        /// <param name="filename"></param>
        private void DisplayMidiPlayer(string fpath, Playlist pl, bool bPlayNow)
        {
            if (fpath == null)
                return;

            // No Playlist
            // * Edit or play a midi file
            // * Create a new midi file
            if (pl == null)
            {
                // edit or play a midi file
                if (fpath != "new file")
                {
                    // Launch an existing file                    
                    if (File.Exists(fpath) == false)
                    {
                        MessageBox.Show("The file " + fpath + " doesn not exists!", "Karaboss", MessageBoxButtons.OK);
                        return;
                    }                  
                }
                else
                {
                    // create a new file
                    fpath = null;          
                }
            }
            else
            {
                // PLAYLIST MODE
                #region playlist mode
                // Launch an existing file                    
                if (File.Exists(fpath) == false)
                {
                    MessageBox.Show("The file " + fpath + " doesn not exists!", "Karaboss", MessageBoxButtons.OK);
                    return;
                }

                #endregion
            }

            // Launch player with a Playlist                               
            Cursor.Current = Cursors.WaitCursor;

            #region Close Windows
            // ferme le formulaire frmPianoTraining
            if (Application.OpenForms.OfType<frmPianoTraining>().Count() > 0)
            {
                Application.OpenForms["frmPianoTraining"].Close();
            }

            // ferme le formulaire frmGuitarTraining
            if (Application.OpenForms.OfType<frmGuitarTraining>().Count() > 0)
            {
                Application.OpenForms["frmGuitarTraining"].Close();
            }

            // Ferme le formulaire FrmTextPlayer
            if (Application.OpenForms.OfType<FrmTextPlayer>().Count() > 0)
            {
                Application.OpenForms["FrmTextPlayer"].Close();
            }
            #endregion

            // Affiche le formulaire frmPlay 
            if (Application.OpenForms["frmPlayer"] != null)
                Application.OpenForms["frmPlayer"].Close();

            ResetOutPutDevice();

            // Add the file to the MRU list.
            MyMruList.AddFile(fpath);

            Form frmPlayer = new frmPlayer(NumInstance, fpath, pl, bPlayNow, outDevice, songRoot);
            frmPlayer.Show();
            frmPlayer.Activate();


        }        


        #endregion Players


        #region form load close

        protected override void OnLoad(EventArgs e)
        {
            sequence1.LoadCompleted += HandleLoadCompleted;

            // Set message on splash windows because the loading of sound fonts takes a very long time
            // if a big file is used
            SplashMsg("Loading Sounfonts");

            // Select output device            
            SelectOutPutDevice();

            // Select input device
            SelectInputDevice();

            // Remove message on splash windows for sound fonts
            SplashMsg("");            

            base.OnLoad(e);
        }

        /// <summary>
        /// Locate form
        /// </summary>
        /// <typeparam name="TForm"></typeparam>
        /// <returns></returns>
        private TForm getForm<TForm>()
            where TForm : Form
        {
            return (TForm)Application.OpenForms.OfType<TForm>().FirstOrDefault();
        }

        private void ShowSplashSoundFonts()
        {                        
            // Splash window because the the loading of sound fonts takes a very long time if a big file is used
            frmLoading frmLoading = new frmLoading();
            frmLoading.Show();
            frmLoading.Refresh();
            frmLoading.Activate();
            
        }

        private void RemoveSplashSoundFonts()
        {                       
            // Remove splash windows for sound fonts
            if (Application.OpenForms.OfType<frmLoading>().Count() > 0)
            {
                frmLoading frmLoading = getForm<frmLoading>();
                frmLoading.Close();
            }           
        }

        private void SplashMsg(string message)
        {
            if (Application.OpenForms.OfType<frmSplashScreen>().Count() > 0)
            {
                frmSplashScreen frmSplashScreen = getForm<frmSplashScreen>();
                frmSplashScreen.Msg(message);
            }
        }


        /// <summary>
        /// form load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmExplorer_Load(object sender, EventArgs e)
        {
            #region setwindowlocation
            // Récupère la taille et position de la forme
            // Set window location
            if (Properties.Settings.Default.frmExplorerMaximized)
            {
                Location = Properties.Settings.Default.frmExplorerLocation;
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                Location = Properties.Settings.Default.frmExplorerLocation;
                // Verify if this windows is visible in extended screens
                Rectangle rect = new Rectangle(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);
                foreach (Screen screen in Screen.AllScreens)
                    rect = Rectangle.Union(rect, screen.Bounds);

                if (Location.X > rect.Width)
                    Location = new Point(0, Location.Y);
                if (Location.Y > rect.Height)
                    Location = new Point(Location.X, 0);

                Size = Properties.Settings.Default.frmExplorerSize;
            }
            #endregion

            this.Activate();


            // Dimension gauche du Splitter
            int D = Properties.Settings.Default.frmExplorerSplitterDistance;
            if (D > 0)
                xplorerControl.SplitterDistance = Properties.Settings.Default.frmExplorerSplitterDistance;


            // Positionne le treeview sur le Fullpath du treeview sauvegardé
            string inipath = string.Empty;
            inipath = Karaclass.GetStartDirectory();

            // Mru Files
            MyMruList = new MruList(System.AppDomain.CurrentDomain.FriendlyName, MnuFileRecentFiles, MruFilesCount);
            MyMruList.FileSelected += MyMruList_FileSelected;


            // If program was launched from another location than startup path (double click on a midi file for eg)
            if (commandlinePath != "")
            {

                string path = Path.GetDirectoryName(commandlinePath);

                //path must be like "file:///c:/users/a453868/Music/karaoke/sasin";
                path = "file:///" + path.Replace("\\", "/");                
                xplorerControl.Navigate(path);

                SelectPlayer(commandlinePath, true);                
            }
            else
            {
                // First launch: select item in listview
                xplorerControl.bDoSelect = true;

                xplorerControl.Navigate(inipath);

                // after, no select
                xplorerControl.bDoSelect = false;
            }
        }

        private void SelectPlayer(string cmdpath, bool bPlayNow)
        {            
            string ext = Path.GetExtension(cmdpath);
            switch (ext.ToLower())
            {
                case ".mid":
                case ".kar":
                    {
                        DisplayMidiPlayer(cmdpath, null, bPlayNow);
                        break;
                    }

                case ".zip":
                case ".cdg":
                    {
                        LaunchCDGPlayer(cmdpath, bPlayNow);
                        break;
                    }

                case ".mml":
                case ".abc":
                    {
                        DisplayTextPlayer(cmdpath, null, bPlayNow);
                        break;
                    }

                default:
                    try
                    {
                        System.Diagnostics.Process.Start(cmdpath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    break;
            }
        }


        protected override void OnClosed(EventArgs e)
        {
            sequence1.Dispose();
            
            if (outDevice != null)
            {
                outDevice.Close();
                outDevice.Dispose();
            }

            if (inDevice != null)
            {
                inDevice.Close();
                inDevice.Dispose();
            }

            base.OnClosed(e);
        }

        /// <summary>
        /// form closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmExplorer_FormClosing(object sender, FormClosingEventArgs e)
        {
            frmMessageBox frmQ = new frmMessageBox
            {
                Msg = Strings.QuitApplication,
                Title = Application.ProductName
            };
            frmQ.ShowDialog();

            bool okButtonClicked = frmQ.OKButtonClicked;

            // Quitter l'application                      
            if (okButtonClicked)
            {

                // enregistre la taille et la position de la forme
                // Copy window location to app settings                
                if (WindowState != FormWindowState.Minimized)
                {
                    if (WindowState == FormWindowState.Maximized)
                    {
                        Properties.Settings.Default.frmExplorerLocation = RestoreBounds.Location;
                        Properties.Settings.Default.frmExplorerMaximized = true;

                    }
                    else if (WindowState == FormWindowState.Normal)
                    {
                        Properties.Settings.Default.frmExplorerLocation = Location;
                        Properties.Settings.Default.frmExplorerSize = Size;
                        Properties.Settings.Default.frmExplorerMaximized = false;
                    }

                    // Save Largeur gauche Splitter
                    Properties.Settings.Default.frmExplorerSplitterDistance = xplorerControl.SplitterDistance;


                    // Save settings
                    Properties.Settings.Default.Save();
                }

                // Save current node                 
                string path = xplorerControl.CurrentFolder.ToString();
                path = "file:///" + path.Replace("\\", "/");

                Karaclass.SaveStartDirectory(path);

                // Save listview column length
                int l = xplorerControl.GetColumnWidth(0);
                Properties.Settings.Default.lvFileNameColumn = l;

                Properties.Settings.Default.Save();

                e.Cancel = false;

                // Close existing windows
                if (Application.OpenForms.OfType<frmLoading>().Count() > 0)
                {
                    frmLoading frmLoading = GetForm<frmLoading>();
                    frmLoading.Close();
                }

                if (Application.OpenForms.OfType<frmPlayer>().Count() > 0)
                {
                    frmPlayer frmPlayer = GetForm<frmPlayer>();
                    frmPlayer.Close();
                }

                if (Application.OpenForms.OfType<frmPianoTraining>().Count() > 0)
                {
                    frmPianoTraining frmPianoTraining = GetForm<frmPianoTraining>();
                    frmPianoTraining.Close();
                }

                if (Application.OpenForms.OfType<frmGuitarTraining>().Count() > 0)
                {
                    frmGuitarTraining frmGuitarTraining = GetForm<frmGuitarTraining>();
                    frmGuitarTraining.Close();
                }


                if (Application.OpenForms.OfType<frmExternalMidiPlay>().Count() > 0)
                {
                    frmExternalMidiPlay frmExternalMidiPlay = GetForm<frmExternalMidiPlay>();
                    frmExternalMidiPlay.Close();
                }

                if (Application.OpenForms.OfType<frmExternalMidiRecord>().Count() > 0)
                {
                    frmExternalMidiRecord frmExternalMidiRecord = GetForm<frmExternalMidiRecord>();
                    frmExternalMidiRecord.Close();
                }

                if (Application.OpenForms.OfType<FrmTextPlayer>().Count() > 0)
                {
                    FrmTextPlayer frm = GetForm<FrmTextPlayer>();
                    frm.Close();
                }


                Dispose();
                //Environment.Exit(Environment.ExitCode);
                
            }
            else
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Form Resize
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmExplorer_Resize(object sender, EventArgs e)
        {
            ResizeElements();
        }

        /// <summary>
        /// Display controls on the form 
        /// </summary>
        private void ResizeElements()
        {            
            sideBarControl.Left = 0;
            sideBarControl.Top = mnuExplorer.Height;
            sideBarControl.Height = ClientSize.Height - mnuExplorer.Height - statusBar.Height;

            //int W = ClientSize.Width - sideBarControl.Width - 17;
            int W = ClientSize.Width - sideBarControl.Width;

            // Despite statusbar is docked bottom, Statusbar is over it :-) 
            // so bring it to front
            statusBar.BringToFront();

            pnlFileInfos.Left = sideBarControl.Width;
            pnlFileInfos.Width = W;
            pnlFileInfos.Top = ClientSize.Height - pnlFileInfos.Height - statusBar.Height;

            xplorerControl.Left = sideBarControl.Width;
            playlistsControl.Left = sideBarControl.Width;
            searchControl.Left = sideBarControl.Width;
            connectedControl.Left = sideBarControl.Width;

            xplorerControl.Top = mnuExplorer.Height;
            playlistsControl.Top = xplorerControl.Top;
            searchControl.Top = xplorerControl.Top;
            connectedControl.Top = xplorerControl.Top;

            xplorerControl.Width = W;
            playlistsControl.Width = xplorerControl.Width;
            searchControl.Width = xplorerControl.Width;
            connectedControl.Width = W;

            xplorerControl.Height = ClientSize.Height - mnuExplorer.Height - pnlFileInfos.Height - statusBar.Height;
            playlistsControl.Height = xplorerControl.Height;
            searchControl.Height = xplorerControl.Height;
            connectedControl.Height = ClientSize.Height - mnuExplorer.Height - statusBar.Height;

            // Labels MIDI, K, I, T, W            
            int totalW = W - lblKtags.Left;
            int iW = totalW / 4;
            if (iW < 200)
                iW = 200;

            lblKtags.Width = lblItags.Width = lblTtags.Width = lblWtags.Width =  iW;

            lblTtags.Left = 180;

            lblKtags.Left = lblTtags.Left + iW;

            lblItags.Left = lblKtags.Left + iW;

            lblWtags.Left = lblItags.Left + iW;


        }

        #endregion


        #region menus


        #region menu file

        /// <summary>
        /// menu: open the player with the selected file and play
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuFileOpen_Click(object sender, System.EventArgs e)
        {
            string filename = string.Empty;

            if (searchControl.Visible)
            {
                if (searchControl.SelectedFile != null)
                    filename = searchControl.SelectedFile;
            }
            else if (xplorerControl.Visible)
            {
                if (xplorerControl.SelectedItems.Length == 1)
                    filename = xplorerControl.SelectedItems[0].FileSystemPath;
            }
            else if (playlistsControl.Visible)
            {
                if (playlistsControl.SelectedFile != null)
                    filename = playlistsControl.SelectedFile;
            }

            if (filename != null && filename != "")
            {
                //DisplayMidiPlayer(filename, null, true);
                SelectPlayer(filename, true);
            }
            else
                MessageBox.Show("Please select a file", "Karaboss", MessageBoxButtons.OK);            
        }


        /// <summary>
        /// menu: open the player with the selected file but don't play 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuFileEdit_Click(object sender, EventArgs e)
        {
            string filename = string.Empty;
            if (searchControl.Visible)
            {
                if (searchControl.SelectedFile != null)
                    filename = searchControl.SelectedFile;
            }
            else if (xplorerControl.Visible)
            {
                if (xplorerControl.SelectedItems.Length == 1)
                    filename = xplorerControl.SelectedItems[0].FileSystemPath;
            }
            else if (playlistsControl.Visible)
            {
                if (playlistsControl.SelectedFile != null)
                    filename = playlistsControl.SelectedFile;
            }

            if (filename != null && filename != "")
                //DisplayMidiPlayer(filename, null, false);            
                SelectPlayer(filename, false);
            else
                MessageBox.Show("Please select a file", "Karaboss", MessageBoxButtons.OK);
            
        }

        /// <summary>
        /// Open empty midi file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuFileNew_Click(object sender, EventArgs e)
        {
           NewMidiFile();
        }

        /// <summary>
        /// Create a new score midi file
        /// </summary>
        private void NewMidiFile()
        {
            int numerator = 4;
            int denominator = 4;
            int division = 480;
            int tempo = 500000;
            int measures = 35;

            // Display dialog windows new midi file
            DialogResult dr = new DialogResult();
            Sanford.Multimedia.Midi.Score.UI.frmNewMidiFile MidiFileDialog = new Sanford.Multimedia.Midi.Score.UI.frmNewMidiFile(numerator, denominator, division, tempo, measures);
            dr = MidiFileDialog.ShowDialog();

            if (dr == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            
            CreateNewMidiFile.Numerator = MidiFileDialog.Numerator;
            CreateNewMidiFile.Denominator = MidiFileDialog.Denominator;
            CreateNewMidiFile.Division = MidiFileDialog.Division;
            CreateNewMidiFile.Tempo = MidiFileDialog.Tempo;
            CreateNewMidiFile.Measures = MidiFileDialog.Measures;
            CreateNewMidiFile.DefaultDirectory = this.xplorerControl.CurrentFolder;
           
            DisplayMidiPlayer("new file", null, false);
        }              

        /// <summary>
        /// menu: quit application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuFileQuit_Click(object sender, EventArgs e)
        {
            // Quitter l'application
            //Application.Exit();
            Application.ExitThread();
        }

        #endregion

        #region menu Edit

        /// <summary>
        /// Rename files using upper directory (F3)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuEditRenameAll_Click(object sender, EventArgs e)
        {
            xplorerControl.RenameAllQuestion();
        }

        /// <summary>
        /// Replace (F4)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuEditReplaceAll_Click(object sender, EventArgs e)
        {
            xplorerControl.ReplaceAllQuestion();
        }

        /// <summary>
        /// invert singer and song (F7)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuInvertAuthorSong_Click(object sender, EventArgs e)
        {
            xplorerControl.InvertAuthorAndSong();
        }

        #endregion


        #region menu Display


        /// <summary>
        /// Display search panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuDisplaySearch_Click(object sender, EventArgs e)
        {
            searchControl.Visible = true;
            playlistsControl.Visible = false;
            xplorerControl.Visible = false;
            connectedControl.Visible = false;

            searchControl.Refresh();
        }

        /// <summary>
        /// Display explorer panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuDisplayExplore_Click(object sender, EventArgs e)
        {
            searchControl.Visible = false;
            xplorerControl.Visible = true;
            playlistsControl.Visible = false;
            connectedControl.Visible = false;

            xplorerControl.Refresh();
        }


        /// <summary>
        /// Display playlists panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuDisplayPlaylist_Click(object sender, EventArgs e)
        {
            playlistsControl.Visible = true;
            xplorerControl.Visible = false;
            searchControl.Visible = false;
            connectedControl.Visible = false;

            playlistsControl.Refresh();
        }

        /// <summary>
        /// Display Search Web Servives
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuDisplayConnected_Click(object sender, EventArgs e)
        {
            DisplayConnected();
        }

        /// <summary>
        /// Display Piano Training
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuDisplayPianoTraining_Click(object sender, EventArgs e)
        {
            DisplayPianoTraining();
        }

        /// <summary>
        /// Display Guitar Training
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuDisplayGuitarTraining_Click(object sender, EventArgs e)
        {
            DisplayGuitarTraining();
        }

        #endregion


        #region menu midi

        /// <summary>
        /// menu: Midi Input Device
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuMidiInputDevice_Click(object sender, EventArgs e)
        {
            if (InputDevice.DeviceCount == 0)
            {
                MessageBox.Show("No Midi Input Device connected", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult dr = new DialogResult();
            Sanford.Multimedia.Midi.UI.InputDeviceDialog MidiInputDialog = new Sanford.Multimedia.Midi.UI.InputDeviceDialog(inDeviceID);
            dr = MidiInputDialog.ShowDialog();

            if (dr == DialogResult.OK)
            {
                inDeviceID = MidiInputDialog.InputDeviceID;
                SelectInputDevice();
            }
        }

        /// <summary>
        /// menu: Midi Output device
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuMidiOutputDevice_Click(object sender, EventArgs e)
        {
            DialogResult dr = new DialogResult();
            OutputDeviceDialog MidiOutputDialog = new OutputDeviceDialog(outDeviceID, Karaclass.m_SaveDefaultOutputDevice);
            

            dr = MidiOutputDialog.ShowDialog();

            if (dr == DialogResult.OK)
            {
                // Splash window because the the loading of sound fonts takes a very long time if a big file is used
                ShowSplashSoundFonts();

                outDeviceID = MidiOutputDialog.OutputDeviceID;

                // Save preferences for Output dev
                SaveDefaultOutputDevice(MidiOutputDialog.bSavePreferences);

                SelectOutPutDevice();

                // Remove splash window
                RemoveSplashSoundFonts();

            }

        }

        /// <summary>
        /// Play an external MIDI instrument
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuMidiExternalPlay_Click(object sender, EventArgs e)
        {
            if (InputDevice.DeviceCount == 0)
            {
                MessageBox.Show("No Midi Input Device connected", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (Application.OpenForms.OfType<frmExternalMidiPlay>().Count() == 0)
            {
                try
                {
                    frmExternalMidiPlay frmExternalMidiPlay = new frmExternalMidiPlay(inDevice, outDevice);
                    frmExternalMidiPlay.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// Record an external MIDI instrument
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuMidiExternalRecord_Click(object sender, EventArgs e)
        {
            if (InputDevice.DeviceCount == 0)
            {
                MessageBox.Show("No Midi Input Device connected", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (Application.OpenForms.OfType<frmExternalMidiRecord>().Count() == 0)
            {
                try
                {
                    frmExternalMidiRecord frmExternalMidiRecord = new frmExternalMidiRecord(inDevice, outDevice);
                    frmExternalMidiRecord.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        #endregion


        #region menu help

        /// <summary>
        /// Menu: About
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuHelpAbout_Click(object sender, System.EventArgs e)
        {
            Form frmAboutDialog = new frmAboutDialog();
            frmAboutDialog.ShowDialog();
        }

        /// <summary>
        /// Check if new version is available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuHelpCheckNewVersion_Click(object sender, EventArgs e)
        {
            bool bHasError = false;

            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            string fileName = System.Reflection.Assembly.GetEntryAssembly().Location;
            FileInfo f = new FileInfo(fileName);
            long size = f.Length;

            string RemoteUrl = Properties.Settings.Default.RemoteUrl;

            IAutoUpdater autoUpdater = new AutoUpdater(Application.ProductName, version, size, RemoteUrl);

            try
            {
                autoUpdater.Update();

                // check if remote URL has changed
                string url = autoUpdater.getRemoteUrl();
                if (url.ToLower() != RemoteUrl.ToLower())
                {
                    Properties.Settings.Default.RemoteUrl = url;
                    Properties.Settings.Default.Save();
                }
            }
            catch (WebException exp)
            {
                MessageBox.Show("Can not find the specified resource. " + exp.Message);
                bHasError = true;
            }
            catch (XmlException exp)
            {
                bHasError = true;
                MessageBox.Show("Download the upgrade file error. " + exp.Message);
            }
            catch (NotSupportedException exp)
            {
                bHasError = true;
                MessageBox.Show("Upgrade address configuration error. " + exp.Message);
            }
            catch (ArgumentException exp)
            {
                bHasError = true;
                MessageBox.Show("Download the upgrade file error. " + exp.Message);
            }
            catch (Exception exp)
            {
                bHasError = true;
                MessageBox.Show("An error occurred during the upgrade process. " + exp.Message);
            }
            finally
            {
                if (bHasError == true)
                {
                    try
                    {
                        //autoUpdater.RollBack();
                    }
                    catch (Exception)
                    {
                        //Log the message to your file or database
                    }
                }

            }
        }

        #endregion
       

        #region menu tools


        /// <summary>
        /// Display option form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuToolsOption_Click(object sender, EventArgs e)
        {
            LoadConfigurationForm();
        }

        #region tools
        /// <summary>
        /// Search doubles MD5 compared to reference
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuToolsMngtFilesSearchDoubles_Click(object sender, EventArgs e)
        {
            
            string referencepath = songRoot;
            string selectedpath = xplorerControl.SelectedFolder.FileSystemPath;

            frmToools fTool = new frmToools(referencepath, selectedpath, "SearchMD5Doubles");            
            fTool.ShowDialog();
                          
        }

        /// <summary>
        /// Search doubles MD5 in a directory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuToolsMngtFilesSearchDoublesSingle_Click(object sender, EventArgs e)
        {
            
            string selectedpath = xplorerControl.SelectedFolder.FileSystemPath;
            string referencepath = selectedpath;

            frmToools fTool = new frmToools(referencepath, selectedpath, "SearchMD5Doubles");
            fTool.ShowDialog();
            
        }

        private void MnuToolsMngtFilesSearchSameSizeComparedToReference_Click(object sender, EventArgs e)
        {
            
            string referencepath = songRoot;
            string selectedpath = xplorerControl.SelectedFolder.FileSystemPath;


            frmToools fTool = new frmToools(referencepath, selectedpath, "SearchSizeDoubles");
            fTool.ShowDialog();
            
        }

        private void MnuToolsMngtFilesSearchSameSizeInASingleDirectory_Click(object sender, EventArgs e)
        {
            
            string selectedpath = xplorerControl.SelectedFolder.FileSystemPath;
            string referencepath = selectedpath;

            frmToools fTool = new frmToools(referencepath, selectedpath, "SearchSizeDoubles");
            fTool.ShowDialog();
            
        }


        private void MnuToolsMngtFilesSearchSameNameComparedToReference_Click(object sender, EventArgs e)
        {
            
            string referencepath = songRoot;
            string selectedpath = xplorerControl.SelectedFolder.FileSystemPath;


            frmToools fTool = new frmToools(referencepath, selectedpath, "SearchNameDoubles");
            fTool.ShowDialog();
            
        }

        private void MnuToolsMngtFilesSearchSameNameInASingleDirectory_Click(object sender, EventArgs e)
        {
            
            string selectedpath = xplorerControl.SelectedFolder.FileSystemPath;
            string referencepath = selectedpath;

            frmToools fTool = new frmToools(referencepath, selectedpath, "SearchNameDoubles");
            fTool.ShowDialog();
            
        }
        #endregion

      



        #endregion

        #endregion menus


        #region functions
        public void RefreshExplorer()
        {
            xplorerControl.RefreshContents();
        }

        #endregion


        #region divers

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

        #endregion divers


        #region configuration control

        private void LoadConfigurationForm()
        {
            // Configuration form
            m_configurationForm = new ConfigurationForm();
            m_configurationForm.ApplyBtn += new EventHandler(OnApplyClicked);
            PopulateConfigurationForm();

            m_configurationForm.ShowDialog();
        }

        private void PopulateConfigurationForm()
        {
            ConfigurationTreeNode config = new ConfigurationTreeNode(Strings.Language, new LangControl(Strings.Language));
            m_configurationForm.AddConfigItem(config);

            config = new ConfigurationTreeNode(Strings.SongTextEncoding, new SongEncodingControl(Strings.SongTextEncoding));
            m_configurationForm.AddConfigItem(config);

            config = new ConfigurationTreeNode(Strings.KaraokeOptions, new KaraokeControl(Strings.KaraokeOptions));
            m_configurationForm.AddConfigItem(config);

            config = new ConfigurationTreeNode(Strings.UpdateKaraboss, new UpdControl(Strings.UpdateKaraboss));
            m_configurationForm.AddConfigItem(config);

            config = new ConfigurationTreeNode("Playlists", new PlaylistsControl("Playlists"));
            m_configurationForm.AddConfigItem(config);

            config = new ConfigurationTreeNode(Strings.MidiEditor, new MidiEditorControl(Strings.MidiEditor));
            m_configurationForm.AddConfigItem(config);


        }

        private void OnApplyClicked(object sender, EventArgs e)
        {
            MessageBox.Show("New settings applied sucessful");
        }



        #endregion configuration control


        #region EraseEmptyDirs

        /// <summary>
        /// Delete empty directories
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuToolsMngtFilesDeleteEmptyDirs_Click(object sender, EventArgs e)
        {
            EraseEmptyDirsQuestion();
        }


        public void EraseEmptyDirsQuestion()
        {
            
            if ((xplorerControl.SelectedFolder != null) && xplorerControl.SelectedFolder.IsFolder)
            {
                string tx = string.Empty;
                string fullpath = xplorerControl.SelectedFolder.FileSystemPath;
                iNbDelete = 0;

                if (Directory.GetFiles(fullpath).Length > 0 ||
                Directory.GetDirectories(fullpath).Length > 0)
                {                                                           

                    if (fullpath != "")
                    {
                        tx = "This function will delete all empty directories starting from\n";
                        tx += "<" + fullpath + ">\n\n";
                        tx += "Continue?";

                        if (MessageBox.Show(tx, "Karaboss - Delete directories", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {
                            Cursor = Cursors.WaitCursor;
                            bQuestionDelete = false;
                            bAbortDelete = false;
                            iNbDelete = 0;

                            EraseEmptyDirs(fullpath);                               
                        }

                        Cursor = Cursors.Default;
                        xplorerControl.RefreshContents();

                    }
                    
                }

                tx = "Delete of Empty directories done.\n";
                tx += iNbDelete + " directories deleted.";
                MessageBox.Show(tx);

            }
            
        }


        private void EraseEmptyDirs(string startLocation)
        {

            if (bAbortDelete)
                return;

            string tx = string.Empty;

            try
            {
                foreach (var directory in Directory.GetDirectories(startLocation))
                {
                    // look at childs
                    EraseEmptyDirs(directory);

                    // If empty, proceed to delete
                    if (Directory.GetFiles(directory).Length == 0 &&
                        Directory.GetDirectories(directory).Length == 0)
                    {
                        if (bQuestionDelete == false)
                        {
                            bQuestionDelete = true;

                            tx = "Warning, you have choosen to delete directories, the first one is\n";
                            tx += directory + "\n\nContinue?";
                            if (MessageBox.Show(tx, "Karaboss - Delete directories", MessageBoxButtons.OKCancel) == DialogResult.OK)
                            {                            
                                bAbortDelete = false;
                                try
                                {
                                    Directory.Delete(directory, false);
                                    iNbDelete++;
                                }
                                catch (Exception e)
                                {
                                    MessageBox.Show(e.Message, "Karaboss - Delete directories", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    bAbortDelete = true;
                                    return;
                                }
                                
                            }
                            else
                            {
                                bAbortDelete = true;
                                return;
                            }
                        }
                        else
                        {
                            try
                            {
                                Directory.Delete(directory, false);
                                iNbDelete++;
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.Message, "Karaboss - Delete directories", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                bAbortDelete = true;
                                return;
                            }
                        }

                    }
                }              

            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        #endregion


        #region prompt
        /// <summary>
        /// Promt dialog window to get replacement string
        /// </summary>
        private static class Prompt
        {
            public static DialogResult ShowDialog(string caption, ref string[] value)
            {
                Form prompt = new Form() {
                    StartPosition = FormStartPosition.CenterScreen,
                    MinimizeBox = false,
                    MaximizeBox = false,
                };

                int wd = 400;
                int ht = 200;

                prompt.Width = wd;
                prompt.Height = ht;
                prompt.Text = caption;

                string[] result = new string[2];

                Label textLabel1 = new Label() { Left = 10, Top = 20, Text = "String to search" };
                TextBox inputBox1 = new TextBox() { Left = 120, Top = 20, Width = 180 };
                inputBox1.Text = value[0];

                Label textLabel2 = new Label() { Left = 10, Top = 50, Text = "String to replace" };
                TextBox inputBox2 = new TextBox() { Left = 120, Top = 50, Width = 180 };
                inputBox2.Text = value[1];

                Button btnConfirmation = new Button() { Text = "Ok", Left = wd / 2 - 100 - 10, Width = 100, Top = 100 };
                Button btnCancel = new Button() { Text = "Cancel", Left = wd / 2 + 10, Width = 100, Top = 100 };

                btnConfirmation.DialogResult = DialogResult.OK;
                btnCancel.DialogResult = DialogResult.Cancel;

                btnConfirmation.Click += (sender, e) => { prompt.Close(); };
                btnCancel.Click += (sender, e) => { prompt.Close(); };

                prompt.Controls.Add(btnConfirmation);
                prompt.Controls.Add(btnCancel);
                prompt.Controls.Add(textLabel1);
                prompt.Controls.Add(inputBox1);
                prompt.Controls.Add(textLabel2);
                prompt.Controls.Add(inputBox2);

                prompt.AcceptButton = btnConfirmation;
                prompt.CancelButton = btnCancel;

                textLabel1.TabIndex = 0;
                inputBox1.TabIndex = 1;
                textLabel2.TabIndex = 2;
                inputBox2.TabIndex = 3;
                btnConfirmation.TabIndex = 4;
                btnCancel.TabIndex = 5;


                DialogResult dialogResult = prompt.ShowDialog();



                value[0] = inputBox1.Text;
                value[1] = inputBox2.Text;

                //return result;
                return dialogResult;
            }


        }

        #endregion


        #region outputdevice

        /// <summary>
        /// Save preferences Output Device
        /// </summary>
        private void SaveDefaultOutputDevice(bool bPref)
        {
            Karaclass.m_SaveDefaultOutputDevice = bPref;
            Properties.Settings.Default.SaveDefaultOutputDevice = bPref;

            if (bPref)                        
                Properties.Settings.Default.DefaultOutputDevice = outDeviceID;                        

            // Save settings
            Properties.Settings.Default.Save();
            
            
        }

        private void ResetOutPutDevice()
        {            
            try
            {
                if (outDevice != null)
                    outDevice.Dispose();
                outDevice = new OutputDevice(outDeviceID);

            }
            catch (Exception ex)
            {
                Console.Write("ERROR " + ex.Message);
                
                // Load other one
                int devices = OutputDeviceBase.DeviceCount;
                if (outDeviceID < devices - 1)
                    outDevice = new OutputDevice(outDeviceID + 1);
                else if (outDeviceID > 0)
                    outDevice = new OutputDevice(outDeviceID - 1);
                else
                    outDevice = null;
            }            
        }

        /// <summary>
        /// Select output Midi device
        /// </summary>
        private void SelectOutPutDevice()
        {
            // Remarque : 
            // Si OutDeviceID = celui de VirtualMidiSynth, on charge à ce moment précis le fichier soundfont
            // cela peut durer au moins 30 sec !!!
            // Mettre une page d'attente    
            
            // Default output device was saved
            if (Karaclass.m_SaveDefaultOutputDevice)
            {
                if (Properties.Settings.Default.DefaultOutputDevice < OutputDeviceBase.DeviceCount)
                    outDeviceID = Properties.Settings.Default.DefaultOutputDevice;
            }

            try
            {
                if (outDevice != null && outDevice.DeviceID != outDeviceID)
                {
                    outDevice.Close();
                    outDevice.Dispose();
                }
                if (outDevice == null || (outDevice != null && outDevice.DeviceID != outDeviceID))
                    outDevice = new OutputDevice(outDeviceID);                

            }
            catch (Exception ex)
            {
                Console.Write("ERROR " + ex.Message);

                // Load other one
                int devices = OutputDeviceBase.DeviceCount;
                if (outDeviceID < devices - 1)
                    outDevice = new OutputDevice(outDeviceID + 1);
                else if (outDeviceID > 0)
                    outDevice = new OutputDevice(outDeviceID - 1);
                else
                    outDevice = null;
            }
            
        }

        /// <summary>
        /// Slect input Midi device
        /// </summary>
        private void SelectInputDevice()
        {
            if (InputDevice.DeviceCount == 0)
                return;

            try
            {
                if (inDevice != null && inDevice.DeviceID != inDeviceID)
                    inDevice.Dispose();
                if (inDevice == null || (inDevice != null && inDevice.DeviceID != inDeviceID))
                    inDevice = new InputDevice(inDeviceID);
            }
            catch (Exception ex)
            {
                Console.Write("ERROR " + ex.Message);

                // Load other one
                inDevice = null;
            }
        }



        #endregion

       
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Karaboss.Properties;
using MarkedEditBox;

namespace Karaboss.Pages.ABCnotation
{
    public partial class FrmABCnotation : Form
    {
        #region Properties and types
        protected int _nSelectedFile = -1;
        protected ColumnSorter _sorter = new ColumnSorter();
        protected int _nEditFirstLine = 0;

        private enum SONG_COLUMN { Title = 0, Path = 1, Index = 2 };
        private enum PROMPT_LOGIN { Yes, No };

        private bool IsCommand(String s) { return s.Trim()[0] == '/'; }

        //protected FormABCRef _abcref = new FormABCRef();

        private LOTROFocuser _focuser = new LOTROFocuser();

        //public static List<FormToolbar> Toolbars = new List<FormToolbar>();
        #endregion

        #region Form methods
        public FrmABCnotation()
        {//====================================================================
            InitializeComponent();
            dlgSaveAs.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Resources.ABCResource.MusicSubfolder;
            dlgOpenFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Resources.ABCResource.MusicSubfolder;
            return;
        }

        private void OnLoad(object sender, EventArgs e)
        {//--------------------------------------------------------------------
            if (Settings.Default.FavoriteSongs == null) Settings.Default.FavoriteSongs = new FavoriteSongs();
            //if (Properties.Settings.Default.Macros == null) Properties.Settings.Default.Macros = new MacroList();
            //if (Properties.Settings.Default.Macros.Items == null) Properties.Settings.Default.Macros.Items = new List<Macro>();

            // Set up the sorting style we want in the list views
            lstFiles.Columns[0].Tag = SortType.TITLE;
            lstFiles.Columns[1].Tag = SortType.PATH;
            lstFiles.Columns[2].Tag = SortType.INTEGER;

            // Auto-binding size or AOT causes all sorts of mess 
            Size = (Size)Settings.Default.FrmABCnotationSize;
            TopMost = Settings.Default.FrmABCnotationAOT;
            Location = (Point)Settings.Default.FrmABCnotationLocation;

            if (Settings.Default.LoadABCAtStartup)
            {
                ReloadFileList();
                ShowSelectedFile();
            }


            // Default to immediate play
            btnPlay.Tag = Song.PlayType.Immediate;
            btnPlay.Text = mniDDPlay.Text;

            // And to recite lyrics in the first option, probably /say
            cmbReciteChannel.SelectedIndex = 0;

            // Fill in the tags if they've been customized
            if (Settings.Default.TagsEdit != null) rteEdit.Tags = Settings.Default.TagsEdit;
            //if (Settings.Default.TagsPerform != null) rtePerform.Tags = Settings.Default.TagsPerform;

            rteEdit.AutoTag = Settings.Default.HighlightABC;

            menustripMain.Left = 0;

            //FormMacroManager fmm = new FormMacroManager();
            //fmm.ShowDialog();
            //CreateTestMacros();
            if (null == Settings.Default.Toolbars) Settings.Default.Toolbars = new LotroToolbarList();
            if (0 == Settings.Default.Toolbars.Items.Count)
            {
                LotroToolbar tb = new LotroToolbar();
                tb.Name = "All Macros";
                //foreach (Macro mac in Properties.Settings.Default.Macros.Items) tb.Items.Add(new LotroToolbarItem(mac));
                Settings.Default.Toolbars.Items.Add(tb);
                Settings.Default.Save();
            }

            // Load up all the toolbars
            foreach (LotroToolbar tb in Settings.Default.Toolbars.Items)
            {
                /*
                FormToolbar ft = new FormToolbar(tb);
                Toolbars.All.Add(ft);
                if (tb.Visible) ft.Show();
                */
            }


            // Kick off the timer that makes LOTRO music play while LOMM has focus
            _focuser.Start();

            return;
        }

     

        private void OnClosing(object sender, FormClosingEventArgs e)
        {//--------------------------------------------------------------------
            _focuser.Stop();

            //Settings.Default.TagsPerform = new MarkedEditBox.RegexTagBag(rtePerform.Tags);
            Settings.Default.TagsEdit = new MarkedEditBox.RegexTagBag(rteEdit.Tags);
            Settings.Default.FrmABCnotationLocation = Location;
            Settings.Default.FrmABCnotationSize = Size;
            Settings.Default.FrmABCnotationAOT = TopMost;
            Settings.Default.Save();
            return;
        }

        private void OnQuickReference(object sender, EventArgs e)
        {//====================================================================
            //_abcref.Show();
            //_abcref.WindowState = FormWindowState.Normal;
            //_abcref.Visible = true;
            return;
        }

        private void OnExit(object sender, EventArgs e)
        {//====================================================================
            //_abcref.Close();
            Close();
            return;
        }

        private void OnActivated(object sender, EventArgs e)
        {   //====================================================================
            _focuser.Start();
            return;
        }

        #endregion

        #region File List Management
        private void OnColumnClick(object sender, ColumnClickEventArgs e)
        {//--------------------------------------------------------------------
            ListView lv = sender as ListView;
            _sorter.CurrentCol = e.Column;
            _sorter.SortType = (SortType)lv.Columns[e.Column].Tag;
            lv.ListViewItemSorter = _sorter as System.Collections.IComparer;
            lv.Sort();
            return;
        }

        private void OnSaveAs(object sender, EventArgs e)
        {//--------------------------------------------------------------------
            if (dlgSaveAs.ShowDialog() == DialogResult.OK)
            {
                SaveFileAs(dlgSaveAs.FileName);
                ReloadFileList(); //TODO: Is this too slow?
                ListViewItem item = lstFiles.FindItemWithText(new FileInfo(dlgSaveAs.FileName).Name);
                if (item != null)
                {
                    item.Selected = true;
                    lstFiles.EnsureVisible(item.Index);
                }
                ShowSelectedFile();
            }
            return;
        }

        private void MakeNewFile(String strName)
        {//--------------------------------------------------------------------
            FileInfo fi = new FileInfo(strName);
            if (fi.Exists) fi.Delete();

            // Here's something annoying: StreamWriter doesn't Close() when it goes out of scope
            //TODO: should this include a "using" statement?
            String strBaseABC = Resources.ABCResource.NewABC + "\0";
            StreamWriter sw = fi.CreateText();
            sw.Write(strBaseABC);
            sw.Close();
        }

        private void OnFileNew(object sender, EventArgs e)
        {//--------------------------------------------------------------------
            if (dlgSaveAs.ShowDialog() == DialogResult.OK)
            {
                String strName = dlgSaveAs.FileName;
                MakeNewFile(strName);

                ReloadFileList(); //TODO: Is this too slow?
                ListViewItem item = lstFiles.FindItemWithText(new FileInfo(strName).Name);
                if (item != null)
                {
                    item.Selected = true;
                    lstFiles.EnsureVisible(item.Index);
                }
                ShowSelectedFile();
            }
            return;
        }

        private void AddFilesToList(FileInfo[] afi)
        {//====================================================================   
            foreach (FileInfo fi in afi)
            {
                List<Song> lstSongs = Song.SongsFromFile(fi);
                foreach (Song song in lstSongs)
                {
                    ListViewItem li = new ListViewItem(song.Title);
                    li.Tag = song;
                    // These have to be in this order or else we get an argument out of range. 
                    // There might be a way to set the number of columns in the subitems collection, 
                    // but this works
                    li.SubItems.Insert((int)SONG_COLUMN.Path, new ListViewItem.ListViewSubItem(li, song.ShortName));
                    li.SubItems.Insert((int)SONG_COLUMN.Index, new ListViewItem.ListViewSubItem(li, song.Index));
                    li.ToolTipText = song.ToolTip;
                    if (Settings.Default.FavoriteSongs != null && Settings.Default.FavoriteSongs.Items != null)
                    {
                        li.Checked = Settings.Default.FavoriteSongs.Items.Contains(new FavoriteSong(song.ShortName, song.Index));
                    }
                    lstFiles.Items.Add(li);
                }
            }

            return;
        }

        private void ReloadFileList()
        {   //====================================================================
            // Get a prompt to save the file if necessary
            if (lstFiles.SelectedItems.Count > 0) lstFiles.SelectedItems[0].Selected = false;
            bool bInitialLoad = lstFiles.Items.Count <= 0;
            lstFiles.Items.Clear();
            //lstLyrics.Items.Clear();
            rteEdit.Clear();
            //rtePerform.Clear();
            SetChangedState(false);

            DirectoryInfo di = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + "/" + Resources.ABCResource.MusicSubfolder);
            try
            {
                AddFilesToList(di.GetFiles("*.abc", SearchOption.AllDirectories));
                AddFilesToList(di.GetFiles("*.txt", SearchOption.AllDirectories));
            }
            catch (Exception ex )
            {

            }

            if (bInitialLoad)
            {
                // The list box was empty, so let's resize the columns. We don't do it otherwise because the user may have resized to their preference
                lstFiles.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                lstFiles.Columns[1].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                lstFiles.Columns[2].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);

                // Simulate a click on the "Title" column. Much more useful than starting with
                // the filename sorted.
                ColumnClickEventArgs eClick = new ColumnClickEventArgs(0);
                OnColumnClick(lstFiles, eClick);
            }

            return;
        }

        private void OnRefresh(object sender, EventArgs e)
        {//====================================================================
            ReloadFileList();
            return;
        }

        private void OnFileSelect(object sender, EventArgs e)
        {//--------------------------------------------------------------------
            if (btnSave.Enabled)
            {
                DialogResult res = MessageBox.Show("The song has changed. Do you want to save the changes?", "LOTRO Music Manager", MessageBoxButtons.YesNoCancel);
                switch (res)
                {
                    default:
                        // This is a weird error condition.
                        break;

                    case DialogResult.Yes:
                        // Just save and continue
                        //TODO: Need FQN
                        ((Song)lstFiles.Items[_nSelectedFile].Tag).Save();
                        break;

                    case DialogResult.Cancel:
                        // Re-select the old line
                        try
                        {
                            lstFiles.Items[_nSelectedFile].Selected = true;
                        }
                        catch (Exception ex) { ex.ToString(); } // Makes the warning go away. I *know* I want to ignore this error case.
                        break;

                    case DialogResult.No:
                        // Nothing to do. Just exit the switch and carry on
                        break;
                }
            }

            ShowSelectedFile();
            if (lstFiles.SelectedItems.Count > 0)
            {
                _nSelectedFile = lstFiles.SelectedIndices[0];
            }
            else
            {
                _nSelectedFile = -1;
            }
            btnPlay.Enabled = true;
            return;
        } // OnFileSelect

        private void OnFileDoubleClick(object sender, EventArgs e)
        {   //====================================================================
            PlaySong(Song.PlayType.Immediate);
            return;
        }

        private void OnOpenInEditor(object sender, EventArgs e)
        {   //====================================================================
            if (0 == lstFiles.SelectedItems.Count) return;

            if (((Song)lstFiles.SelectedItems[0].Tag).FileName.ToLower().EndsWith(".txt"))
            {
                // Just execute the file, since they may have a preferred editor
                Process.Start("\"" + ((Song)lstFiles.SelectedItems[0].Tag).FileName + "\"");
            }
            else
            if (((Song)lstFiles.SelectedItems[0].Tag).FileName.ToLower().EndsWith(".abc"))
            {
                // They may not have an association for .abc
                Process.Start("notepad.exe", "\"" + ((Song)lstFiles.SelectedItems[0].Tag).FileName + "\"");
            }

            return;
        }
        #endregion

        #region LOTRO Music
        private void PlaySong(Song.PlayType playtype)
        {   //====================================================================
            if (lstFiles.SelectedItems.Count == 0) return;

            SaveFile();
            ((Song)lstFiles.SelectedItems[0].Tag).Play(playtype);
            _focuser.Start();
            return;
        }

        private void OnToggleMusicMode(object sender, EventArgs e)
        {   //--------------------------------------------------------------------
            //RemoteController.SendText(Resources.ToggleMusicCommand);
            Activate(); // Return focus to the app
            return;
        }

        private void OnPlay(object sender, EventArgs e)
        {//--------------------------------------------------------------------
            // This means the "Play" button has been pressed. But it might be
            // the "wait to play" button at the moment. We'll pull that fact 
            // off the button tag.
            PlaySong((Song.PlayType)btnPlay.Tag);
            return;
        } // OnPlay

        private void OnStartSync(object sender, EventArgs e)
        {
        }

        private void OnWaitToPlay(object sender, EventArgs e)
        {//--------------------------------------------------------------------
            // Switch the button to be sync from now on
            btnPlay.Tag = Song.PlayType.Sync;
            btnPlay.Text = mniDDPlaySync.Text;
            btnPlay.PerformClick();
            return;
        }

        private void OnStartSyncPlay(object sender, EventArgs e)
        {//--------------------------------------------------------------------
            // Switch the button to be sync from now on
            btnPlay.Tag = Song.PlayType.Sync;
            btnPlay.Text = mniDDPlaySync.Text;
            //RemoteController.SendText(Resources.StartSyncCommand);
            return;
        }

        private void OnDropDownPlay(object sender, EventArgs e)
        {//--------------------------------------------------------------------
            // Switch the button to be immediate from now on
            btnPlay.Tag = Song.PlayType.Immediate;
            btnPlay.Text = mniDDPlay.Text;
            btnPlay.PerformClick();
            return;
        }

        private void OnStopSong(object sender, EventArgs e)
        {//--------------------------------------------------------------------
            btnPlay.Text = mniDDPlay.Text;
            btnPlay.Tag = Song.PlayType.Immediate;
            //RemoteController.ExecuteFunction("MusicEndSong");
            return;
        } // OnDDStopSong

        #endregion

        #region File Editing
        private void SetChangedState(bool b)
        {//====================================================================
            btnSave.Enabled = b; mniSaveABC.Enabled = b;
            btnUndo.Enabled = b; mniUndoAll.Enabled = b;
            return;
        }

        private void ShowSelectedFile()
        {//--------------------------------------------------------------------
            if (lstFiles.SelectedItems.Count > 0)
            {
                // Clear selections because we can get in a weird state otherwise
                rteEdit.SelectionStart = 0; rteEdit.SelectionLength = 0;
                rtePerform.SelectionStart = 0; rtePerform.SelectionLength = 0;

                _nEditFirstLine = ((Song)lstFiles.SelectedItems[0].Tag).FirstLine;

                rteEdit.Text = ((Song)lstFiles.SelectedItems[0].Tag).Text;
                rtePerform.Text = rteEdit.Text;
                rteEdit.Enabled = true; // Allow edits

                RefreshLyricsList();
            }
            else
            {
                //lstLyrics.Items.Clear();
                rteEdit.Clear();
                //rtePerform.Clear();
                rteEdit.Enabled = false; // Disallow edits
            }
            slEditLocation.Text = "";
            SetChangedState(false); // No changes yet, so no save or undo
            return;
        } // ShowSelectedFile

        private void RefreshLyricsList()
        {//====================================================================
            lstLyrics.Items.Clear();
            for (int i = 0; i < rteEdit.Lines.Length; i += 1)
            {
                if (ABC.IsLyrics(rteEdit.Lines[i]))
                {
                    lstLyrics.Items.Add(new ABCLine(ABC.RemoveHeaderTag(rteEdit.Lines[i]), i));
                }
            }
        }

        private void OnABCChanged(object sender, EventArgs e)
        {//--------------------------------------------------------------------
            SetChangedState(true);
            return;
        }

        private void OnUndoAll(object sender, EventArgs e)
        {//--------------------------------------------------------------------
            // Just re-load the file
            ShowSelectedFile();
            return;
        } // OnUndoAll

        private void OnDeleteFile(object sender, EventArgs e)
        {//====================================================================
            if (lstFiles.SelectedItems.Count == 0) return;

            if (MessageBox.Show("Really delete " + ((Song)lstFiles.SelectedItems[0].Tag).FileName + "?", "LOMM File Delete", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                FileInfo fi = new FileInfo(((Song)lstFiles.SelectedItems[0].Tag).FileName);
                fi.Delete();

                ReloadFileList();
                SetChangedState(false);
            }
            return;
        }

        private void SaveFileAs(String strFileName)
        {//--------------------------------------------------------------------
            if (lstFiles.SelectedItems.Count > 0)
            {
                ((Song)lstFiles.SelectedItems[0].Tag).Text = rteEdit.Text;
                ((Song)lstFiles.SelectedItems[0].Tag).SaveAs(strFileName);
                SetChangedState(false);
            }
        }

        private void SaveFile()
        {//--------------------------------------------------------------------
            if (lstFiles.SelectedItems.Count > 0)
            {
                ((Song)lstFiles.SelectedItems[0].Tag).Text = rteEdit.Text;
                ((Song)lstFiles.SelectedItems[0].Tag).Save();
                SetChangedState(false);
            }
        }

        private void OnSaveABC(object sender, EventArgs e)
        {//--------------------------------------------------------------------
            SaveFile();
            return;
        }

        private void OnCaretMoved(object sender, MarkedEditBox.CaretMovedEventArgs e)
        {   //====================================================================
            slEditLocation.Text = "Line " + (e.Row + _nEditFirstLine).ToString() + ", Character " + (e.Col + 1).ToString();
            return;
        }

        private void OnEditorSelectAll(object sender, EventArgs e)
        {   //====================================================================
            rteEdit.SelectAll();
            return;
        }

        private void OnEditCopy(object sender, EventArgs e)
        {   //--------------------------------------------------------------------
            rteEdit.Copy();
            return;
        }

        private void OnEditCut(object sender, EventArgs e)
        {   //--------------------------------------------------------------------
            rteEdit.Cut();
            return;
        }

        private void OnEditPaste(object sender, EventArgs e)
        {   //--------------------------------------------------------------------
            rteEdit.Paste();
            return;
        }

        #endregion

        
        #region Settings
        private void OnOptions(object sender, EventArgs e)
        {
            /*
            String strInitialToolbarOpacity = Settings.Default.ToolbarOpacity;
            FormOptions dlg = new FormOptions(this);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Settings.Default.KeepLOTROFocused = dlg.KeepLOTROFocused;
                Settings.Default.Opacity = Opacity.ToString();
                Settings.Default.AOT = dlg.AOT;

                TopMost = Settings.Default.AOT;
                rteEdit.AutoTag = Settings.Default.HighlightABC;
            }
            else
            {
                Opacity = Double.Parse(Settings.Default.Opacity);
                foreach (FormToolbar frm in Toolbars.All) frm.Opacity = Double.Parse(Settings.Default.ToolbarOpacity);
            }
            Settings.Default.Save();
            */
            return;
        }
        #endregion
        
        
        #region Lyrics Playing
        private void ReciteLine()
        {   //--------------------------------------------------------------------
            if (lstLyrics.SelectedItems.Count != 1) return;

            //RemoteController.SendText(cmbReciteChannel.Text + " " + lstLyrics.SelectedItem.ToString());
            lstLyrics.SelectedIndex = (lstLyrics.SelectedIndex < lstLyrics.Items.Count) ? lstLyrics.SelectedIndex + 1 : 0;
            lstLyrics.Focus();
        }

        protected static bool _bInProgrammaticUIChange = false;
        private void OnPerformCaretMoved(object sender, MarkedEditBox.CaretMovedEventArgs e)
        {   //====================================================================
            if (_bInProgrammaticUIChange) return;
            _bInProgrammaticUIChange = true;

            rtePerform.SelectLine(rtePerform.InsertionRow);

            if (rtePerform.Lines.Length > 0 && ABC.IsLyrics(rtePerform.Lines[rtePerform.InsertionRow]))
            {
                // We have a lyrics line
                btnPerform.Text = "Recite Line";

                // Select the right item in the listbox
                lstLyrics.SelectedIndex = -1;
                for (int i = 0; i < lstLyrics.Items.Count; i += 1)
                {
                    ABCLine line = (ABCLine)lstLyrics.Items[i];
                    if (line.SourceLine == rtePerform.InsertionRow)
                    {
                        lstLyrics.SelectedIndex = i;
                        break;
                    }
                }
            } // Lyrics line
            else
            {
                btnPerform.Text = "Recite Next Line";
            } // Not a lyrics line

            _bInProgrammaticUIChange = false;
            return;
        }

        private void OnLyricsListSelectedIndexChanged(object sender, EventArgs e)
        {   //====================================================================
            if (_bInProgrammaticUIChange) return;
            _bInProgrammaticUIChange = true;

            // Select the right line in the rtf view
            ABCLine line = (ABCLine)lstLyrics.SelectedItem;
            rtePerform.SelectLine(line.SourceLine);

            _bInProgrammaticUIChange = false;
            return;
        }

        private void OnPerformKeyPress(object sender, KeyPressEventArgs e)
        {   //====================================================================
            e.Handled = false;
            switch (e.KeyChar)
            {
                // No right and left, only up and down
                case (char)System.Windows.Forms.Keys.Right:
                    e.KeyChar = (char)System.Windows.Forms.Keys.Down;
                    break;
                case (char)System.Windows.Forms.Keys.Left:
                    e.KeyChar = (char)System.Windows.Forms.Keys.Up;
                    break;

                // Return means to play the line
                case (char)System.Windows.Forms.Keys.Return:
                    ReciteLine();
                    e.Handled = true;
                    break;
            }
            return;
        }

        private void OnPerformButton(object sender, EventArgs e)
        {   //====================================================================
            ReciteLine();
            return;
        }

        private void OnPerformTextPaneSizeChanged(object sender, EventArgs e)
        {   //====================================================================
            // For some reason, the Fill property isn't resizing the marked edit
            // control properly, so let's do it manually.
            rtePerform.Width = splitPerform.Panel2.Width;
            rtePerform.Height = splitPerform.Panel2.Height;
            return;
        }

        private void OnPlayNow(object sender, EventArgs e)
        {   //====================================================================
            PlaySong(Song.PlayType.Immediate);
            return;
        }

        private void OnPerformPlayAndRecite(object sender, EventArgs e)
        {   //====================================================================
            if (lstLyrics.Items.Count > 0) lstLyrics.SelectedIndex = 0;
            PlaySong(Song.PlayType.Immediate);
            ReciteLine();
            return;
        }

        private void OnPerformStopSong(object sender, EventArgs e)
        {   //--------------------------------------------------------------------
            //RemoteController.ExecuteFunction("MusicEndSong");
            return;
        }

        private void OnPerformWaitToPlay(object sender, EventArgs e)
        {   //====================================================================
            PlaySong(Song.PlayType.Sync);
            return;

        }

        private void OnPerformStartGroup(object sender, EventArgs e)
        {   //--------------------------------------------------------------------
            //RemoteController.SendText(Resources.StartSyncCommand);
            return;
        }

        private void OnPerformStartGroupAndRecite(object sender, EventArgs e)
        {   //--------------------------------------------------------------------
            if (lstLyrics.Items.Count > 0) lstLyrics.SelectedIndex = 0;
            //RemoteController.SendText(Resources.StartSyncCommand);
            ReciteLine();
            return;
        }

        private void OnLyricListDblClick(object sender, EventArgs e)
        {//====================================================================
            ReciteLine();
            return;
        }

        private void OnLyricListKeyPress(object sender, KeyPressEventArgs e)
        {   //--------------------------------------------------------------------
            ReciteLine();
            return;
        }

        private void OnTabSelectedChanged(object sender, EventArgs e)
        {   //====================================================================
            // The edit pane may have made changes that need to be reflected in the perform pane
            if (tabsMain.SelectedTab == tpgPerform && btnSave.Enabled)
            {
                rtePerform.Text = rteEdit.Text;
                RefreshLyricsList();
            }
            return;
        }

        #endregion
        


        #region Song List Context Menu
        private void OnSongListCopyTitle(object sender, EventArgs e)
        {   //====================================================================
            if (lstFiles.SelectedItems.Count > 0)
            {
                Clipboard.SetText(lstFiles.SelectedItems[0].Text);
            }
            return;
        }

        private void OnSongListCopyFilename(object sender, EventArgs e)
        {   //--------------------------------------------------------------------
            if (lstFiles.SelectedItems.Count > 0)
            {
                Clipboard.SetText(((Song)lstFiles.SelectedItems[0].Tag).ShortName);
            }
            return;
        }

        private void OnSongListCopyFQFilename(object sender, EventArgs e)
        {   //--------------------------------------------------------------------
            if (lstFiles.SelectedItems.Count > 0)
            {
                Clipboard.SetText(((Song)lstFiles.SelectedItems[0].Tag).FileName);
            }
            return;
        }

        private void OnSongListCopyInfoBlock(object sender, EventArgs e)
        {   //--------------------------------------------------------------------
            if (lstFiles.SelectedItems.Count > 0)
            {
                Clipboard.SetText(lstFiles.SelectedItems[0].ToolTipText);
            }
            return;
        }
        #endregion


        
        #region Macros and Toolbars
        private void OnManageMacros(object sender, EventArgs e)
        {
            /*
            FormMacroManager fmm = new FormMacroManager();
            fmm.ShowDialog();

            // Need to refresh toolbars if 
            // - macro deleted
            // - icon changed
            // - macro renamed
            if (fmm.NeedToolbarsRefreshed) foreach (FormToolbar frm in Toolbars.All) frm.RefreshToolbarItems();
            return;
            */
        }

        private void OnViewMenuOpening(object sender, EventArgs e)
        {   //====================================================================
            mniViewToolbars.DropDown.Items.Clear();
            foreach (LotroToolbar ltb in Settings.Default.Toolbars.Items)
            {
                ToolStripMenuItem tsmi = new ToolStripMenuItem();
                tsmi.Text = ltb.Name;
                tsmi.Tag = ltb;
                tsmi.Checked = ltb.Visible;
                tsmi.Click += new EventHandler(OnViewToolbarsItemClick);
                mniViewToolbars.DropDown.Items.Add(tsmi);
            }
            return;
        }

        
        void OnResetAllToolbars(object sender, EventArgs e)
        {   //====================================================================
            if (MessageBox.Show("Really reset all toolbar locations? This will move every toolbar.", "Reset all toolbars", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                int n = 0; // Increasing offset to cascade the toolbars
                //foreach (FormToolbar ft in Toolbars.All) { ft.Location = ft.Toolbar.Location = new Point(Location.X + n, Location.Y + n); n += 20; }
            }
            return;
        }

        void OnRemoveToolbar(object sender, EventArgs e)
        {   //====================================================================
            String[] astr = new String[Settings.Default.Toolbars.Items.Count];
            for (int i = 0; i < Settings.Default.Toolbars.Items.Count; i += 1) astr[i] = Settings.Default.Toolbars.Items[i].Name;
            /*
            FormInputChoice frm = new FormInputChoice("Remove toolbar", "Select toolbar to remove", astr, "");
            if (frm.ShowDialog() == DialogResult.OK && frm.SelectedIndex != -1)
            {
                Settings.Default.Toolbars.Items.Remove(Toolbars.All[frm.SelectedIndex].Toolbar);
                FormToolbar ft = Toolbars.All[frm.SelectedIndex];
                Toolbars.All.RemoveAt(frm.SelectedIndex);
                ft.Close();
                ft.Dispose();
            }
            return;
            */
        }

        
        void OnNewToolbar(object sender, EventArgs e)
        {   //====================================================================
            /*
            FormInputPrompt frm = new FormInputPrompt("New Toolbar", "Name:", "toolbar" + Settings.Default.Toolbars.Items.Count.ToString());
            if (frm.ShowDialog() == DialogResult.OK && frm.Value != String.Empty)
            {
                LotroToolbar ltb = new LotroToolbar();
                ltb.Name = frm.Value;
                ltb.Direction = LotroToolbar.BarDirection.Horizontal;
                ltb.Location = Location; // Gotta start somewhere.....
                ltb.Visible = true;

                Settings.Default.Toolbars.Items.Add(ltb);

                FormToolbar ft = new FormToolbar(ltb);
                Toolbars.All.Add(ft);
                if (ltb.Visible) ft.Show();
            }
            return;
            */
        }

        void OnViewToolbarsItemClick(object sender, EventArgs e)
        {   //====================================================================
            ToolStripMenuItem tsmi = sender as ToolStripMenuItem; if (tsmi == null) return;
            LotroToolbar ltb = tsmi.Tag as LotroToolbar; if (ltb == null) return;
            ltb.Visible = !ltb.Visible;
            tsmi.Checked = ltb.Visible;
            //foreach (FormToolbar frm in Toolbars.All) if (frm.Toolbar == ltb) frm.Visible = ltb.Visible;
            return;
        }
        
        #endregion
        
        #region Favorite Songs
        private void OnFavoriteSongsDropDown(object sender, EventArgs e)
        {
        }

        private void OnPlayFavoriteSong(object sender, EventArgs e)
        {
        }

        private void OnSelectedFavoriteSongChange(object sender, EventArgs e)
        {
        }

        private void OnFavoriteSongListChanged(object sender, ItemCheckedEventArgs e)
        {   //====================================================================
            switch (e.Item.Checked)
            {
                case true:
                    // Add the item to the list
                    Settings.Default.FavoriteSongs.Items.Add(new FavoriteSong(e.Item.SubItems[(int)SONG_COLUMN.Path].Text,
                                                                              e.Item.SubItems[(int)SONG_COLUMN.Index].Text,
                                                                              e.Item.SubItems[(int)SONG_COLUMN.Title].Text));
                    break;

                case false:
                    // Remove from the list - title is irrelevant except for display, so we don't need to pass it
                    Settings.Default.FavoriteSongs.Items.Remove(new FavoriteSong(e.Item.SubItems[(int)SONG_COLUMN.Path].Text, e.Item.SubItems[(int)SONG_COLUMN.Index].Text));
                    break;
            }
            return;
        }

       
        #endregion

        
        #region Help
        
        private void OnHelpContentsClick(object sender, EventArgs e)
        {   //====================================================================
            try { Process.Start("lomm.chm"); } catch {; }
            return;
        }

        
        private void OnHelpAbout(object sender, EventArgs e)
        {//====================================================================
            /*
            FormAboutBox ab = new FormAboutBox();
            ab.ShowDialog();
            return;
            */
        }
        #endregion

        
        private void OnExportMacros(object sender, EventArgs e)
        {   //====================================================================
            //FormExportMacros frm = new FormExportMacros();
            //frm.ShowDialog();
            return;
        }

        private void OnImportMacros(object sender, EventArgs e)
        {   //====================================================================
            if (dlgOpenFile.ShowDialog() == DialogResult.OK)
            {
                /*
                StreamReader sr = new StreamReader(dlgOpenFile.FileName);
                MacroList ml = MacroList.FromXML(sr);
                foreach (Macro m in ml.Items)
                {
                    Settings.Default.Macros.Add(m);
                }
                */
            }
            return;
            
        }
        


    }
}

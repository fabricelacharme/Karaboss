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
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hqub.MusicBrainz.API.Entities;
using Hqub.MusicBrainz.API.Entities.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Karaboss.Pages
{

    public partial class ConnectedControl : UserControl
    {
        class ClsAlbums
        {
            public string Title { get; set; }
            public string Date { get; set; }
            public string Type { get; set; }
            public string PrimaryType { get; set; }
            public string Song { get; set; }
            public string Id { get; set; }
        }

        class ClsArtist
        {
            public string Name { get; set; }            
            public int Score { get; set; }
            public string Id { get; set; }
            public List<ClsAlbums> albums = new List<ClsAlbums>();

            public void SortAlbums()
            {
                ClsAlbums[] arrayMenu = new ClsAlbums[albums.Count];

                for (int i = 0; i < albums.Count; i++)
                {
                    ClsAlbums album = albums[i];
                    arrayMenu[i] = album;
                }
                Array.Sort(arrayMenu, delegate (ClsAlbums pli1, ClsAlbums pli2)
                { return pli1.Date.CompareTo(pli2.Date); });

                // restore sorted collection
                albums.Clear();
                for (int i = 0; i < arrayMenu.Length; i++)
                {
                    albums.Add(arrayMenu[i]);

                }
            }

        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        private string result = string.Empty;
        private ListViewItemComparer _lvwItemComparerlvArtists;

        Button btnSearchArtist;
        Button btnSearchSong;

        int offset_Albums = 0;

        private ContextMenuStrip ImgContextMenu;
        private string InitialDir = string.Empty;
        

        public ConnectedControl()
        {
            InitializeComponent();

            InitializelvArtists();
            InitializelvAlbums();
            InitializelvTracks();
            ResizeElements();         
        }

        #region override
        // ADD a button Search
        protected override void OnLoad(EventArgs e)
        {

            btnSearchArtist = new Button() {
                Size = new Size(25, txtSearchArtist.ClientSize.Height + 2),
                //Location = new Point(txtSearchArtist.ClientSize.Width - btnSearchArtist.Width, -1),
                Location = new Point(txtSearchArtist.ClientSize.Width - 25, -1),
                Cursor = Cursors.Default,
                Image = Properties.Resources.Action_Search_icon,
            };
            txtSearchArtist.Controls.Add(btnSearchArtist);

            btnSearchArtist.Click += new EventHandler(BtnSearchArtist_Click);

            // Send EM_SETMARGINS to prevent text from disappearing underneath the button
            SendMessage(txtSearchArtist.Handle, 0xd3, (IntPtr)2, (IntPtr)(btnSearchArtist.Width << 16));


            btnSearchSong = new Button() {
                Size = new Size(25, txtSearchSong.ClientSize.Height + 2),
                //Location = new Point(txtSearchSong.ClientSize.Width - btnSearchSong.Width, -1),
                Location = new Point(txtSearchSong.ClientSize.Width - 25, -1),
                Cursor = Cursors.Default,
                Image = Properties.Resources.Action_Search_icon,
            };
            txtSearchSong.Controls.Add(btnSearchSong);

            btnSearchSong.Click += new EventHandler(BtnSearchSong_Click);

            // Send EM_SETMARGINS to prevent text from disappearing underneath the button
            SendMessage(txtSearchSong.Handle, 0xd3, (IntPtr)2, (IntPtr)(btnSearchSong.Width << 16));
            
        }

       
        #endregion


        #region lvArtists

        // Initialize lvArtists
        private void InitializelvArtists()
        {
            lvArtists.Dock = DockStyle.Fill;

            // Set the view to show details.
            lvArtists.View = View.Details;

            // Allow the user to edit item text.
            lvArtists.LabelEdit = true;

            // Allow the user to rearrange columns.
            lvArtists.AllowColumnReorder = true;

            // Select the item and subitems when selection is made.
            lvArtists.FullRowSelect = true;

            // Display grid lines.
            lvArtists.GridLines = true;

            // Keep selection active
            lvArtists.HideSelection = false;

            // Sort the items in the list in ascending order.
            lvArtists.Sorting = SortOrder.Ascending;

            // Attach Subitems to the ListView
            lvArtists.Columns.Add("Score", 50, HorizontalAlignment.Left);
            lvArtists.Columns.Add("Nom", 200, HorizontalAlignment.Left);
            lvArtists.Columns.Add("Nom de tri", 100, HorizontalAlignment.Left);
            lvArtists.Columns.Add("Type", 100, HorizontalAlignment.Left);
            lvArtists.Columns.Add("Genre", 100, HorizontalAlignment.Left);
            lvArtists.Columns.Add("Région", 100, HorizontalAlignment.Left);
            lvArtists.Columns.Add("Commence par", 100, HorizontalAlignment.Left);
            lvArtists.Columns.Add("Région de début", 100, HorizontalAlignment.Left);
            lvArtists.Columns.Add("Fin", 100, HorizontalAlignment.Left);
            lvArtists.Columns.Add("Région de fin", 100, HorizontalAlignment.Left);
            lvArtists.Columns.Add("Id", 100, HorizontalAlignment.Left);
            lvArtists.Columns.Add("Search", 20, HorizontalAlignment.Left);

            // The ListViewItemSorter property allows you to specify the
            // object that performs the sorting of items in the ListView.
            // You can use the ListViewItemSorter property in combination
            // with the Sort method to perform custom sorting.
            _lvwItemComparerlvArtists = new Karaboss.Pages.ListViewItemComparer();
            this.lvArtists.ListViewItemSorter = _lvwItemComparerlvArtists;
        }

        /// <summary>
        /// Listview: search for a single artist
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LvArtists_DoubleClick(object sender, EventArgs e)
        {
            SearchFromLvArtists();
        }

        private void lvArtists_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenuStrip lvContextMenu = new ContextMenuStrip();
                lvContextMenu.Items.Clear();

                // Menu search albums                
                ToolStripMenuItem MnuSearchAlbums = new ToolStripMenuItem("Search");
                lvContextMenu.Items.Add(MnuSearchAlbums);

                MnuSearchAlbums.Click += new EventHandler(MnuSearchAlbums_Click);

                // Display menu
                lvContextMenu.Show(lvArtists, lvArtists.PointToClient(Cursor.Position));
            }
        }

        private void MnuSearchAlbums_Click(object sender, EventArgs e)
        {
            SearchFromLvArtists();
        }

        /// <summary>
        /// Search albums or tracks selected in lvArtists
        /// </summary>
        private void SearchFromLvArtists()
        {
            if (lvArtists.SelectedItems.Count == 0)
                return;

            // Erase labels for single artist
            ResetArtistLabels();

            ListViewItem lvi = lvArtists.SelectedItems[0];
            string idartist = string.Empty;
            string idalbum = string.Empty;
            string SearchType = lvi.SubItems[11].Text;

            if (SearchType == "Artist")
            {
                idartist = lvi.SubItems[10].Text;
                // Search all albums for an artist
                GetOneArtistAndAlbums(idartist);
            }
            else if (SearchType == "Album")
            {
                idartist = lvi.SubItems[9].Text;
                idalbum = lvi.SubItems[10].Text;
                // Display tracks for selected album
                GetOneArtistAndTracks(idartist, idalbum);
            }
        }

        private void DisplaySingleAlbum()
        {
            lvAlbums.Items.Clear();
            ListViewItem lviArtist = lvArtists.SelectedItems[0];

            // 0 - date
            ListViewItem lviAlbum = new ListViewItem(lviArtist.SubItems[6].Text);

            // 1 - Album title
            lviAlbum.SubItems.Add(lviArtist.SubItems[2].Text);

            // 2 cover front
            lviAlbum.SubItems.Add("");

            // 3 - parutions
            lviAlbum.SubItems.Add("");

            // 4 - nb tracks
            lviAlbum.SubItems.Add("");

            // 5 - Id
            lviAlbum.SubItems.Add(lviArtist.SubItems[10].Text);



            // Add the list items to the ListView
            lvAlbums.Items.Add(lviAlbum);

        }


        #endregion


        #region lvAlbums

        // Initialize lvArtists
        private void InitializelvAlbums()
        {
            //lvAlbums.Dock = DockStyle.Fill;

            // Set the view to show details.
            lvAlbums.View = View.Details;

            // Allow the user to edit item text.
            lvAlbums.LabelEdit = true;

            // Allow the user to rearrange columns.
            lvAlbums.AllowColumnReorder = true;

            // Select the item and subitems when selection is made.
            lvAlbums.FullRowSelect = true;

            // Display grid lines.
            lvAlbums.GridLines = true;

            // Keep selection active
            lvAlbums.HideSelection = false;


            // Sort the items in the list in ascending order.
            lvAlbums.Sorting = SortOrder.Ascending;

            // Attach Subitems to the ListView
            lvAlbums.Columns.Add("Année", 50, HorizontalAlignment.Left);
            lvAlbums.Columns.Add("Titre", 250, HorizontalAlignment.Left);
            lvAlbums.Columns.Add("CovertArt", 80, HorizontalAlignment.Left);
            lvAlbums.Columns.Add("Parutions", 60, HorizontalAlignment.Left);
            lvAlbums.Columns.Add("Tracks", 50, HorizontalAlignment.Left);
            lvAlbums.Columns.Add("Id", 10, HorizontalAlignment.Left);

            // The ListViewItemSorter property allows you to specify the
            // object that performs the sorting of items in the ListView.
            // You can use the ListViewItemSorter property in combination
            // with the Sort method to perform custom sorting.
            //_lvwItemComparerlvAlbums = new ListViewItemComparer();
            //this.lvAlbums.ListViewItemSorter = _lvwItemComparerlvAlbums;

            forwardButton.Enabled = false;
            backButton.Enabled = false;


        }

        /// <summary>
        /// ListView: search tracks for an album
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LvAlbums_DoubleClick(object sender, EventArgs e)
        {
            SearchTracks();
        }

        private void SearchTracks()
        {
            if (lvAlbums.SelectedItems.Count == 0)
                return;

            Cursor.Current = Cursors.WaitCursor;

            ListViewItem lvi = lvAlbums.SelectedItems[0];
            string id = lvi.SubItems[5].Text;

            DisplayCover(id);
            GetTracksList(id);
        }

        private void lvAlbums_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenuStrip lvContextMenu = new ContextMenuStrip();
                lvContextMenu.Items.Clear();

                // Menu search albums                
                ToolStripMenuItem MnuSearchTracks = new ToolStripMenuItem("Search tracks");
                lvContextMenu.Items.Add(MnuSearchTracks);

                MnuSearchTracks.Click += new EventHandler(MnuSearchTracks_Click);

                // Display menu
                lvContextMenu.Show(lvAlbums, lvAlbums.PointToClient(Cursor.Position));
            }
        }

        private void MnuSearchTracks_Click(object sender, EventArgs e)
        {
            SearchTracks();
        }


        private void LvAlbums_Resize(object sender, EventArgs e)
        {
            ResizeElements();
        }

        #endregion


        #region lvTracks

        // Initialize lvTracks
        private void InitializelvTracks()
        {
            //lvTracks.Height = pnlTracks.Height - lvTracks.Top;
            //lvTracks.Width = pnlTracks.Width - 2 * lvTracks.Left;

            //lvTracks.Dock = DockStyle.Fill;

            // Set the view to show details.
            lvTracks.View = View.Details;

            // Allow the user to edit item text.
            lvTracks.LabelEdit = true;

            // Allow the user to rearrange columns.
            lvTracks.AllowColumnReorder = true;

            // Select the item and subitems when selection is made.
            lvTracks.FullRowSelect = true;

            // Display grid lines.
            lvTracks.GridLines = true;

            // Keep selection active
            lvTracks.HideSelection = false;

            // Sort the items in the list in ascending order.
            //lvTracks.Sorting = SortOrder.Ascending;
            lvTracks.Sorting = SortOrder.None;

            // Attach Subitems to the ListView
            lvTracks.Columns.Add("Track", 40, HorizontalAlignment.Left);
            lvTracks.Columns.Add("Titre", 170, HorizontalAlignment.Left);
            lvTracks.Columns.Add("Durée", 50, HorizontalAlignment.Left);
            //lvTracks.Columns.Add("Disambiguation", 250, HorizontalAlignment.Left);
            //lvTracks.Columns.Add("Credits", 50, HorizontalAlignment.Left);
            //lvTracks.Columns.Add("Score", 50, HorizontalAlignment.Left);
            lvTracks.Columns.Add("Id", 10, HorizontalAlignment.Left);

        }

        private void LoadLstTracks(Release release, ListView lv)
        {
            Cursor.Current = Cursors.WaitCursor;

            // Clear the ListView control
            lv.Items.Clear();

            // Display items in the ListView control
            MediumList MediumList = release.MediumList;


            Medium med = release.MediumList.Items.Count > 0 ? release.MediumList.Items.First() : new Medium(); ;
            string format = med.Format;

            if (med.Tracks != null)
            {
                for (int i = 0; i < med.Tracks.QueryCount; i++)
                {
                    var track = med.Tracks.Items[i];
                    int position = track.Position;

                    var recording = track.Recording;
                    //var len = TimeSpan.FromMilliseconds(recording.Length).ToString("m\\:ss");
                    string duration = recording.Length.ToString();
                    TimeSpan dur = TimeSpan.FromMilliseconds(double.Parse(duration));


                    string title = recording.Title;
                    string trackNumber = (i + 1).ToString();

                    ListViewItem lvi = new ListViewItem(trackNumber);
                    lvi.SubItems.Add(title);

                    duration = String.Format("{0:D2}:{1:D2}", dur.Minutes, dur.Seconds);
                    lvi.SubItems.Add(duration);
                    lvi.SubItems.Add(track.Id);
                    // Add the list items to the ListView
                    lv.Items.Add(lvi);
                }
            }
            Cursor.Current = Cursors.Default;
        }

        private void LoadListTracks(RecordingList tracks, ListView lv)
        {
            Cursor.Current = Cursors.WaitCursor;

            // Clear the ListView control
            lv.Items.Clear();

            // Display items in the ListView control
            for (int i = 0; i < tracks.Items.Count; i++)
            {

                // Define the list items
                Recording track = tracks.Items[i];
                string trackNumber = (i + 1).ToString();


                //ListViewItem lvi = new ListViewItem(track.Title);
                ListViewItem lvi = new ListViewItem(trackNumber);


                lvi.SubItems.Add(track.Title);

                string duration = track.Length.ToString();
                TimeSpan dur = TimeSpan.FromMilliseconds(double.Parse(duration));

                duration = String.Format("{0:D2}:{1:D2}", dur.Minutes, dur.Seconds);
                lvi.SubItems.Add(duration);


                lvi.SubItems.Add(track.Disambiguation);
                lvi.SubItems.Add(track.Credits.ToString());
                lvi.SubItems.Add(track.Score.ToString());
                lvi.SubItems.Add(track.Id);

                // Add the list items to the ListView
                lv.Items.Add(lvi);

            }
            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Search for lyrics
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LvTracks_DoubleClick(object sender, EventArgs e)
        {
            SearchLyrics();
        }
   
        private void lvTracks_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenuStrip lvContextMenu = new ContextMenuStrip();
                lvContextMenu.Items.Clear();

                // Menu search albums                
                ToolStripMenuItem MnuSearchLyrics = new ToolStripMenuItem("Search Lyrics");
                lvContextMenu.Items.Add(MnuSearchLyrics);

                MnuSearchLyrics.Click += new EventHandler(MnuSearchLyrics_Click);

                // Display menu
                lvContextMenu.Show(lvTracks, lvTracks.PointToClient(Cursor.Position));
            }
        }

        private void MnuSearchLyrics_Click(object sender, EventArgs e)
        {
            SearchLyrics();
        }

        #endregion


        #region lyrics
      
        /// <summary>
        /// Search Lyrics
        /// </summary>
        private void SearchLyrics()
        {
            string artist = lblArtistNomDeTri.Text;
            int idx = artist.IndexOf(',');
            if (idx >= 0)
                artist = artist.Substring(idx + 1, artist.Length - idx - 1).Trim() + " " + artist.Substring(0, idx).Trim();

            string title = lvTracks.SelectedItems[0].SubItems[1].Text;

            Cursor = Cursors.WaitCursor;

            string provider = string.Empty;


            // First try AZlyrics
            var azLyrics = new AzLyrics.Api.AzLyrics(artist, title);
            var lyrics = azLyrics.GetLyrics();            
            if (lyrics.Length > 10 && azLyrics.Error == 0)
            {
                //Console.Write(lyrics);
                provider = "LYRICS FROM AZlyrics.com";

                lyrics = lyrics.Replace("\r\n", "[]");
                lyrics = lyrics.Replace("\n", "][");
                lyrics = lyrics.Replace("[]", "\r\n");
                lyrics = lyrics.Replace("][", "\r\n");
                lyrics = provider + "\r\n\r\n\r\n" + lyrics;
                txtLyrics.Text = lyrics;
            }
            else
            {
                // Second try LyricsWikia
                var lyricsWikia = new LyricsWikia.Api.LyricsWikia(artist, title);
                lyrics = lyricsWikia.GetLyrics();

                if (lyrics.Length > 10 && lyricsWikia.Error == 0)
                {
                    provider = "LYRICS FROM lyrics.wikia.com/wiki";

                    lyrics = lyrics.Replace("\r\n", "[]");
                    lyrics = lyrics.Replace("\n", "][");
                    lyrics = lyrics.Replace("[]", "\r\n");
                    lyrics = lyrics.Replace("][", "\r\n");

                    lyrics = provider + "\r\n\r\n\r\n" + artist + "\r\n\r\n" + title + "\r\n\r\n" + lyrics;

                    txtLyrics.Text = lyrics;
                }              
                else
                {
                    // Third try SongLyrics
                    var songLyrics = new SongLyrics.Api.SongLyrics(artist, title);
                    lyrics = songLyrics.GetLyrics();

                    if (lyrics.Length > 10 && songLyrics.Error == 0)
                    {
                        provider = "LYRICS FROM www.songlyrics.com";

                        lyrics = lyrics.Replace("\r\n", "[]");
                        lyrics = lyrics.Replace("\n", "][");
                        lyrics = lyrics.Replace("[]", "\r\n");
                        lyrics = lyrics.Replace("][", "\r\n");

                        lyrics = provider + "\r\n\r\n\r\n" + artist + "\r\n\r\n" + title + "\r\n\r\n" + lyrics;

                        txtLyrics.Text = lyrics;
                    }
                    else
                        txtLyrics.Text = "No results for:\r\n\r\n" + "Artist: " + artist + "\r\n\r\nTitle: " + title;
                }

            }

            Cursor = Cursors.Default;

        }

        #endregion


        #region get artist from song name

        /// <summary>
        /// Launch search by clicling the search button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSearchSong_Click(object sender, EventArgs e)
        {
            LaunchSearchArtistsFromSong();
        }

        /// <summary>
        /// Launch search by keyboard ENTER
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtSearchSong_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    LaunchSearchArtistsFromSong();
                    break;
            }
        }      

        private static async Task<RecordingList> GetArtistFromSong(string song, bool bFilterAlbumSingle, bool bExactPhrase, string artist = "")
        {
            //
            // See documentation for search syntax of the Lucene text search engine:
            // https://musicbrainz.org/doc/Indexed_Search_Syntax
            //
            string query = string.Empty;

            //bool bfilterAlbums  return only albums & singles
            //bool bExactPhrase : all the words of the query song           

            try
            {
                if (bExactPhrase)
                    song = song.Replace(" ", " AND ");

                if (bFilterAlbumSingle)
                {
                    //query = "type:\"single\" OR type:\"album\" AND ";
                    query = "type:single OR type:album AND ";
                    //query = "type:single OR type:album AND NOT type:compilation AND NOT type:live AND ";
                }

                if (artist != "")
                    query += song + " AND artist:\"" + artist + "\"";
                else
                    query += song;

               

                var artists = await Recording.SearchAsync(query, 100);
                return artists;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return null;
            }
        }

        private async void LaunchSearchArtistsFromSong()
        {
            string song = txtSearchSong.Text.Trim();
            string artist = txtSearchArtist.Text.Trim();

            if (song == "")
            {
                MessageBox.Show("Please enter a song name");
                return;
            }

            bool bExactPhrase = chkSongExactPhrase.Checked;
            bool bFilterAlbumSingle = chkFilterAlbumSingle.Checked;

            var reclist = await GetArtistFromSong(song, bFilterAlbumSingle, bExactPhrase, artist);

            if (reclist == null)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show("No artists found");
                return;
            }

            List<ClsArtist> lstRes = FilterSearchArstistsFromSong(song, reclist, bFilterAlbumSingle);
            DisplaySearchArtistsFromSong(lstRes);              
        }

        /// <summary>
        /// Filter results
        /// </summary>
        /// <param name="song"></param>
        /// <param name="reclist"></param>
        /// <returns></returns>
        private List<ClsArtist> FilterSearchArstistsFromSong(string song, RecordingList reclist, bool bFilterAlbumSingle)
        {                      
            string ArtistRelease = string.Empty;
            string albumTitle = string.Empty;
            string albumId = string.Empty;
            string albumSong = string.Empty;
            string albumDate = string.Empty;
            string albumType = string.Empty;
            string albumPrimaryType = string.Empty;
            string albumCredits = string.Empty;

            // Extract single artists from list with Id
            List<ClsArtist> artists = new List<ClsArtist>();
            Release release;

            foreach (var rec in reclist.Items)
            {
                // artist
                Artist artist = (Artist)rec.Credits.First().Artist;

                // Album
                albumTitle = "";
                

                if (rec.Releases != null && rec.Releases.Items.Count > 0)
                {
                    for (int i = 0; i < rec.Releases.Items.Count; i++)
                    {
                        release = rec.Releases.Items[i];
                        albumTitle = release.Title;
                        albumType = release.ReleaseGroup.Type; // Compilation

                        albumCredits = "";
                        if (release.Credits != null && release.Credits.Count > 0)
                            albumCredits = release.Credits[0].Artist.Name;

                        albumDate = "";
                        if (release.Date != null && release.Date.Length >= 4)
                            albumDate = release.Date.Substring(0, 4);
                        

                        bool bCondition = (albumDate != "" && albumCredits != "Various Artists" && albumTitle != "");
                        if (bFilterAlbumSingle)
                            bCondition = bCondition && (albumType == "Album" || albumType == "Single");


                        //if (albumDate != "" && albumCredits != "Various Artists" && albumTitle != ""  && (albumType == "Album" || albumType == "Single"))
                        if (bCondition)
                        {
                            
                            albumPrimaryType = release.ReleaseGroup.PrimaryType; 

                            string status = release.Status;

                            albumTitle = release.Title;
                            albumId = rec.Releases.Items[i].Id;
                            albumSong = rec.Title;
                            

                            // artists already in list?
                            int idx = artists.IndexOf(artists.Where(z => z.Id == artist.Id).FirstOrDefault());
                            if (idx < 0)
                            {
                                // Create new entry for this artist in the list 
                                ClsArtist myArtist = new ClsArtist() {
                                    Name = artist.Name,
                                    Id = artist.Id,
                                    Score = artist.Score,
                                };

                                ClsAlbums myAlbum = new ClsAlbums() {
                                    Title = albumTitle,
                                    Song = albumSong,
                                    Date = albumDate,
                                    Type = albumType,
                                    PrimaryType = albumPrimaryType,
                                    Id = albumId,
                                };
                                myArtist.albums.Add(myAlbum);

                                artists.Add(myArtist);

                                // Increase score if the song title equals the searched song title
                                if (rec.Title.ToLower() == song.ToLower())
                                    myArtist.Score++;
                            }
                            else
                            {
                                // Artist already exists in the list, so do just add a new album to his/her list of albums
                                ClsArtist myArtist = artists[idx];

                                int x = myArtist.albums.IndexOf(myArtist.albums.Where(z => z.Title == albumTitle && z.Date == albumDate).FirstOrDefault());
                                if (x < 0)
                                {
                                    ClsAlbums myAlbum = new ClsAlbums() {
                                        Title = albumTitle,
                                        Song = albumSong,
                                        Date = albumDate,
                                        Type = albumType,
                                        PrimaryType = albumPrimaryType,
                                        Id = albumId,
                                    };
                                    myArtist.albums.Add(myAlbum);

                                    // Increase score only if the song title equals the searched song title
                                    if (rec.Title.ToLower() == song.ToLower())
                                        myArtist.Score++;
                                }
                            }
                        }
                    }

                }              
            }

            return artists;
        }

        /// <summary>
        /// Display results in the listview
        /// </summary>
        /// <param name="song"></param>
        /// <param name="reclist"></param>
        private void DisplaySearchArtistsFromSong(List<ClsArtist> artists)
        {
            // Select artitst tab
            tabControl1.SelectedTab = tabControl1.TabPages[0];
            // Clear list
            lvArtists.Items.Clear();                      

            // Populate listview
            foreach (ClsArtist artist in artists)
            {
                // 0 - Score
                // 1 - Artist
                // 2 - Album
                // 3 - Type
                // 3 - Primary type
                // 4 - date
                // 5 - song                                

                // 0 - score                
                ListViewItem lvi = new ListViewItem(artist.Score.ToString());

                // 1 - artist
                lvi.SubItems.Add(artist.Name);

                // 2 - album
                lvi.SubItems.Add("");

                // 3 - Type
                lvi.SubItems.Add("");

                // 4 - Primary type
                lvi.SubItems.Add("");

                // 5 - Date
                lvi.SubItems.Add("");

                // 6 - song
                lvi.SubItems.Add("");

                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");

                // 10 - Id 
                lvi.SubItems.Add(artist.Id);

                // 11 - Type search
                lvi.SubItems.Add("Artist");

                // Add the list items to the ListView
                lvArtists.Items.Add(lvi);

                artist.SortAlbums();

                // List of albums
                foreach (ClsAlbums album in artist.albums)
                {
                    // 0 - score
                    lvi = new ListViewItem(artist.Score.ToString());

                    // - 1 - artist
                    lvi.SubItems.Add("-");

                    // 2 - Album
                    lvi.SubItems.Add(album.Title);

                    // 3 - Type
                    lvi.SubItems.Add(album.Type);

                    // 4 - Primary type
                    lvi.SubItems.Add(album.PrimaryType);

                    // 5 - song
                    lvi.SubItems.Add(album.Song);

                    // 6 - Date
                    lvi.SubItems.Add(album.Date);
                    
                    lvi.SubItems.Add("");
                    lvi.SubItems.Add("");

                    // artist Id
                    lvi.SubItems.Add(artist.Id);

                    // 10 - AlbumId
                    lvi.SubItems.Add(album.Id);

                    // 11 - Type search
                    lvi.SubItems.Add("Album");

                    // Add the list items to the ListView
                    lvArtists.Items.Add(lvi);
                }
            }

            // Sort listview by score
            lvArtists.Sort();
        }

        #endregion


        #region search list of artists by name

        /// <summary>
        /// Button: search artist infos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSearchArtist_Click(object sender, EventArgs e)
        {
            LaunchSearchArtists();
        }

        /// <summary>
        /// Textbox: search for artist infos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtSearchArtist_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    LaunchSearchArtists();
                    break;
            }
        }

        private void LaunchSearchArtists()
        {
            string artistName = txtSearchArtist.Text.Trim();
            if (artistName == "")
            {
                MessageBox.Show("Please enter an artist name");
                return;
            }
            ResetArtistLabels();
            SearchArtistsList(artistName);

        }

        /// <summary>
        /// Get and display list of artists
        /// </summary>
        /// <param name="artistName"></param>
        private async void SearchArtistsList(string artistName)
        {
            Cursor.Current = Cursors.WaitCursor;
            lvArtists.Items.Clear();

            ArtistList artists = await GetArtists(artistName);

            tabControl1.SelectedTab = tabControl1.TabPages[0];
            if (artists == null)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show("No artists found");
                return;
            }

            LoadListlvArtists(artists, lvArtists);

        }

        private static void LoadListlvArtists(ArtistList artists, ListView lv)
        {
            Cursor.Current = Cursors.WaitCursor;
            // Clear the ListView control
            lv.Items.Clear();

            // Display items in the ListView control
            for (int i = 0; i < artists.Items.Count; i++)
            {

                // Define the list items
                Artist artist = artists.Items[i];
                ListViewItem lvi = new ListViewItem(artist.Score.ToString());
                lvi.SubItems.Add(artist.Name + " " + artist.Disambiguation);
                lvi.SubItems.Add(artist.SortName);
                lvi.SubItems.Add(artist.Type);
                lvi.SubItems.Add(artist.Gender);
                lvi.SubItems.Add(artist.Country);
                lvi.SubItems.Add(artist.LifeSpan.Begin);
                lvi.SubItems.Add("");
                lvi.SubItems.Add(artist.LifeSpan.End);
                lvi.SubItems.Add("");
                lvi.SubItems.Add(artist.Id);

                // 11 - Type search
                lvi.SubItems.Add("Artist");

                // Add the list items to the ListView
                lv.Items.Add(lvi);
            }
            lv.Sort();

            Cursor.Current = Cursors.Default;
        }

        private static async Task<ArtistList> GetArtists(string name)
        {
            var artists = await Artist.SearchAsync(name);
            return artists;
        }

        #endregion


        #region search for 1 single artist  

        private async void GetOneArtist(string id)
        {
            Artist artist = await GetSingleArtist(id);
            DisplaySingleArtistValues(artist);

        }

        private async void GetOneArtistAndAlbums(string id)
        {
            Artist artist = await GetSingleArtist(id);
            DisplaySingleArtistValues(artist);

            // Then search albums
            SearchAlbums();
        }

        private async void GetOneArtistAndTracks(string idartist, string idalbum)
        {
            Artist artist = await GetSingleArtist(idartist);
            DisplaySingleArtistValues(artist);

            // then search all tracks for an album
            DisplayCover(idalbum);
            GetTracksList(idalbum);
            DisplaySingleAlbum();
        }


        /// <summary>
        /// Async get single artist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static async Task<Artist> GetSingleArtist(string id)
        {
            var artist = await Artist.GetAsync(id);
            return artist;
        }

        /// <summary>
        /// Display artist values in labels (right screen)
        /// </summary>
        /// <param name="artist"></param>
        public void DisplaySingleArtistValues(Artist artist)
        {
            if (artist == null)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show("No artist found");
                return;
            }

            lblArtistCodeIPI.Text = "";
            lblArtistCodeISNI.Text = "";

            if (artist.LifeSpan != null)
                lblArtistDateNaissance.Text = artist.LifeSpan.Begin;

            lblArtistEvaluation.Text = "";
            lblArtistExternalLinks.Text = "";
            lblArtistGenre.Text = artist.Gender;
            lblArtistLieuNaissance.Text = "";
            lblArtistNomDeTri.Text = artist.SortName;
            lblArtistRegion.Text = artist.Country;

            lblArtistId.Text = artist.Id;


            lblArtistTags.Text = "";
            if (artist.Tags != null)
            {
                string tags = string.Empty;
                foreach (Tag tag in artist.Tags.Items)
                {
                    tags += tag.Name + " ";
                }
                lblArtistTags.Text = tags;
            }

            lblArtistType.Text = artist.Type;
        }

        #endregion


        #region Search all the albums by artist

        private async void DisplayAllAlbums(string id)
        {
            int offset = 0;
            int nbResults = 0;
            int limit = 100;

            List<Release> AllReleases = new List<Release>();

            ReleaseList releases = await ShowReleasesByArtist(id, limit, offset);
            nbResults = releases.Items.Count;

            for (int i = 0; i < releases.Items.Count; i++)
            {
                AllReleases.Add(releases.Items[i]);
            }

            while (nbResults > 0)
            {
                try
                {
                    offset += limit;
                    releases = await ShowReleasesByArtist(id, limit, offset);
                    for (int i = 0; i < releases.Items.Count; i++)
                    {
                        AllReleases.Add(releases.Items[i]);
                    }
                    nbResults = releases.Items.Count;
                }
                catch (Exception ex)
                {
                    nbResults = 0;
                    Console.Write(ex.Message);
                }
            }

            // Filter results
            List<Release> myreleases = FilterListAllReleases(AllReleases);

            // Display results 
            //tabControl1.Visible = true;
            DisplayListReleases(myreleases, limit, nbResults);

            tabControl1.SelectedTab = tabControl1.TabPages[1];
            lblAlbumsPage.Text = offset_Albums.ToString();
        }

        #endregion


        #region Search albums by artist

        private void SearchAlbums()
        {
            if (lblArtistId.Text == "")
            {
                MessageBox.Show("Please select an artist");
                return;
            }

            offset_Albums = 0;
            lblAlbumsPage.Text = offset_Albums.ToString();
            string id = lblArtistId.Text;

            //DisplayAlbums(id, offset_Albums);
            
            // Search for ALL recording
            // does not work if too many (Beatles for ex)
            DisplayAllAlbums(id);
        }

        #region buttons pages

        private void BackButton_Click(object sender, EventArgs e)
        {
            if (lblArtistId.Text == "")
            {
                MessageBox.Show("Please select an artist");
                return;
            }

            offset_Albums -= 100;
            if (offset_Albums < 0)
                offset_Albums = 0;

            string id = lblArtistId.Text;
            Cursor.Current = Cursors.WaitCursor;
            DisplayAlbums(id, offset_Albums);
        }

        private void ForwardButton_Click(object sender, EventArgs e)
        {
            if (lblArtistId.Text == "")
            {
                MessageBox.Show("Please select an artist");
                return;
            }

            offset_Albums += 100;
            string id = lblArtistId.Text;
            Cursor.Current = Cursors.WaitCursor;

            DisplayAlbums(id, offset_Albums);
        }

        #endregion

        /// <summary>
        /// Old code (page by page)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="offset"></param>
        private async void DisplayAlbums(string id, int offset)
        {
            int limit = 100;            

            // All releases
            ReleaseList releases = await ShowReleasesByArtist(id, limit, offset);
            int nbResults = releases.Items.Count;

            // Filter results
            List<Release> myreleases = FilterListReleases(releases);

            // Display results            
            DisplayListReleases(myreleases, limit, nbResults);

            tabControl1.SelectedTab = tabControl1.TabPages[1];
            lblAlbumsPage.Text = offset_Albums.ToString();            
        }

        // marche
        private static async Task<ReleaseList> ShowReleasesByArtist(string id, int limit ,int offset)
        {
            Cursor.Current = Cursors.WaitCursor;

            // See https://musicbrainz.org/doc/Development/XML_Web_Service/Version_2

            // parameters 
            //string[] pars = new string[] { "media", "artist-credits" };
            string[] pars = new string[] { "media" };
            var releases = await Release.BrowseAsync("artist", id, limit, offset, pars);
            //var releases = await Release.BrowseAsync("artist", id, limit, offset, "media");

            //var releases = await Release.BrowseAsync("artist", id, limit, offset, "artist-credits");
            //var releases = await Release.BrowseAsync("artist", id, limit, offset, "discids");
            //var releases = await Release.BrowseAsync("recording", id, limit, offset, "media"); // KO
            //var releases = await Release.BrowseAsync("artist", id, limit, offset, "various-artists");
            //var releases = await Release.BrowseAsync("artist", id, limit, offset, "releases");


            return releases;
        }

        private static async Task<ReleaseGroupList> ShowReleaseGroupsByArtist(string id, int limit, int offset)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                var releasegrouplist = await ReleaseGroup.BrowseAsync("artist", id, limit, offset);
                //var releasegrouplist = await ReleaseGroup.BrowseAsync("artist", id, limit, offset, "media");
                //var releasegrouplist = await ReleaseGroup.BrowseAsync("artist", id, limit, offset, "releases-group");
                //var releasegrouplist = await ReleaseGroup.BrowseAsync("artist", id, limit, offset, "Releases");
                //var releasegrouplist = await ReleaseGroup.BrowseAsync("artist", id, limit, offset, "release");
                //var releasegrouplist = await ReleaseGroup.BrowseAsync("artist", id, limit, offset, "release-groups");

                //var releasegrouplist = await ReleaseGroup.BrowseAsync("release", id, limit, offset);
                //var releasegrouplist = await ReleaseGroup.BrowseAsync("releases", id, limit, offset);

                return releasegrouplist;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return null;
            }

            
        }

        /// <summary>
        /// New code : filter all realeases
        /// </summary>
        /// <param name="releases"></param>
        /// <returns></returns>
        private List<Release> FilterListAllReleases(List<Release> releases)
        {
            List<Release> myreleases = new List<Release>();

            string Title = string.Empty;
            for (int i = 0; i < releases.Count; i++)
            {
                Release release = releases[i];

                // Keep only those having front cover
                if (release.CoverArtArchive.Front && release.Date != null)
                {

                    // Search if Title alredy exists in the filter list
                    int idx = myreleases.IndexOf(myreleases.Where(z => z.Title.ToLower() == release.Title.ToLower()).FirstOrDefault());
                    if (idx < 0)
                    {
                        myreleases.Add(release);
                    }
                    else
                    {
                        Release rel = myreleases[idx];
                        int currentyear = Convert.ToInt32(rel.Date.Substring(0, 4));
                        int newyear = Convert.ToInt32(release.Date.Substring(0, 4));

                        if (newyear <= currentyear && release.CoverArtArchive.Front == true)
                        {
                            release.MediumList = rel.MediumList;
                            myreleases[idx] = release;
                        }

                        // Add to existing release only a new medium to increase the medium count
                        // suppose that only one item ...
                        Medium medium = release.MediumList.Items[0];
                        rel.MediumList.Items.Add(medium);
                    }
                }
            }

            return myreleases;
        }

        /// <summary>
        /// Old code : filter only 100 releases
        /// </summary>
        /// <param name="releases"></param>
        /// <returns></returns>
        private List<Release> FilterListReleases(ReleaseList releases)
        {
            List<Release> myreleases = new List<Release>();

            string Title = string.Empty;
            for (int i = 0; i < releases.Items.Count; i++)
            {
                Release release = releases.Items[i];

                // Keep only those having front cover
                if (release.CoverArtArchive.Front && release.Date != null)
                {

                    // Search if Title alredy exists in the filter list
                    int idx = myreleases.IndexOf(myreleases.Where(z => z.Title.ToLower() == release.Title.ToLower()).FirstOrDefault());
                    if (idx < 0)
                    {
                        myreleases.Add(release);
                    }
                    else
                    {
                        Release rel = myreleases[idx];
                        int currentyear = Convert.ToInt32(rel.Date.Substring(0, 4));
                        int newyear = Convert.ToInt32(release.Date.Substring(0, 4));

                        if (newyear <= currentyear && release.CoverArtArchive.Front == true)
                        {
                            release.MediumList = rel.MediumList;
                            myreleases[idx] = release;
                        }

                        // Add to existing release only a new medium to increase the medium count
                        // suppose that only one item ...
                        Medium medium = release.MediumList.Items[0];
                        rel.MediumList.Items.Add(medium);
                    }
                }
            }

            return myreleases;
        }

        /// <summary>
        /// Display releases on lvAlbums
        /// </summary>
        /// <param name="myreleases"></param>
        /// <param name="limit"></param>
        /// <param name="nbresults"></param>
        private void DisplayListReleases(List<Release> myreleases, int limit, int nbresults)
        {
            Cursor.Current = Cursors.WaitCursor;

            // If nb results sent < nb results asked
            if (nbresults < limit)                            
                forwardButton.Enabled = false;           
            else                            
                forwardButton.Enabled = true;            

            if (offset_Albums == 0)                            
                backButton.Enabled = false;            
            else                            
                backButton.Enabled = true;            

            // Clear the ListView control
            lvAlbums.Items.Clear();

            // Display items in the ListView control
            foreach (Release release in myreleases)
            {
                if (release.Date != null)
                {
                    // 0 - date
                    ListViewItem lvi = new ListViewItem(release.Date.Substring(0, 4));

                    // 1 - Title
                    lvi.SubItems.Add(release.Title);

                    // 2 cover front
                    if (release.CoverArtArchive != null && release.CoverArtArchive.Front == true)
                        lvi.SubItems.Add("Yes");
                    else
                        lvi.SubItems.Add("");

                    // 3 - parutions
                    lvi.SubItems.Add(release.MediumList.Items.Count.ToString());

                    // 4 - nb tracks
                    if (release.MediumList.Items.Count > 0)
                        lvi.SubItems.Add(release.MediumList.Items[0].Tracks.QueryCount.ToString());
                    else
                        lvi.SubItems.Add("");

                    // 5 - Id
                    lvi.SubItems.Add(release.Id);

                    // Add the list items to the ListView
                    lvAlbums.Items.Add(lvi);
                }
            }
            Cursor.Current = Cursors.Default;
        }

        #endregion


        #region Search tracks by album

        /// <summary>
        /// Button: search tracks for an album
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnTracks_Click(object sender, EventArgs e)
        {
            SearchTracks();
        }

        /// <summary>
        /// Get and display tracks
        /// </summary>
        /// <param name="id"></param>
        private async void GetTracksList(string id)
        {
            var release = await Release.GetAsync(id, "recordings");
         
            tabControl1.SelectedTab = tabControl1.TabPages[1];
            LoadLstTracks(release, lvTracks);          
        }


        #endregion      


        #region display Cover

        /// <summary>
        /// Display front cover of an album
        /// </summary>
        /// <param name="id"></param>
        private void DisplayCover(string id)
        {
            Uri uri = CoverArtArchive.GetCoverArtUri(id);
            MemoryStream ms = CoverArtArchive.GetImageUri(uri);

            if (ms == null)
                return;

            try
            {
                Image img = Image.FromStream(ms);
                picCover.Image = img;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Karaboss - Get Cover Image", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Console.Write(ex.Message);
                picCover.Image = null;
            }                        
            Cursor.Current = Cursors.Default;
        }

        #endregion


        #region Utils

        private void ResetArtistLabels()
        {
            picCover.Image = null;

            lblArtistNomDeTri.Text = "";
            lblArtistType.Text = "";
            lblArtistGenre.Text = "";
            lblArtistDateNaissance.Text = "";
            lblArtistLieuNaissance.Text = "";
            lblArtistRegion.Text = "";
            lblArtistCodeIPI.Text = "";
            lblArtistCodeISNI.Text = "";
            lblArtistId.Text = "";
            lblArtistEvaluation.Text = "";
            lblArtistTags.Text = "";
            lblArtistExternalLinks.Text = "";

            //lvArtists.Items.Clear();
            lvAlbums.Items.Clear();
            lvTracks.Items.Clear();
            txtLyrics.Text = "";
        }
      
        #endregion


        #region Control load resize
        private void ConnectedControl_Resize(object sender, EventArgs e)
        {
            ResizeElements();
        }
        

        private void ConnectedControl_Load(object sender, EventArgs e)
        {
            ResizeElements();
        }

        private void ResizeElements()
        {                     
            lvAlbums.Width = 500;
            pnlTracks.Left = lvAlbums.Width;
            pnlTracks.Height = tabControl1.TabPages[1].Height;

            lvTracks.Height = lvAlbums.Height - lvTracks.Top;
            lvTracks.Width = pnlTracks.Width - 2 * lvTracks.Left;

            txtLyrics.Top = pnlTracks.Top;
            txtLyrics.Left = pnlTracks.Left + pnlTracks.Width;
            txtLyrics.Height = pnlTracks.Height;
            txtLyrics.Width = pnlTracks.Width;
        }


        #endregion


        #region picturebox
        private void PicCover_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && picCover.Image != null)
            {
                ImgContextMenu = new ContextMenuStrip();
                ImgContextMenu.Items.Clear();

                // Menu delete                
                ToolStripMenuItem menuSavePicture = new ToolStripMenuItem("Save picture");
                ImgContextMenu.Items.Add(menuSavePicture);

                menuSavePicture.Click += new System.EventHandler(MenuSavePicture_Click);

                // Display menu
                ImgContextMenu.Show(picCover, picCover.PointToClient(Cursor.Position));

            }
        }

        private void MenuSavePicture_Click(object sender, EventArgs e)
        {
            saveFileDlg.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif|Png Image|*.png";
            saveFileDlg.Title = "Save an Image File";
            saveFileDlg.InitialDirectory = InitialDir;
            saveFileDlg.FileName = "FrontCover";

            saveFileDlg.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (saveFileDlg.FileName != "")
            {
                try
                {

                    InitialDir = Path.GetDirectoryName(saveFileDlg.FileName);
                    // Saves the Image via a FileStream created by the OpenFile method.
                    System.IO.FileStream fs =
                       (System.IO.FileStream)saveFileDlg.OpenFile();
                    // Saves the Image in the appropriate ImageFormat based upon the
                    // File type selected in the dialog box.
                    // NOTE that the FilterIndex property is one-based.
                    switch (saveFileDlg.FilterIndex)
                    {
                        case 1:
                            picCover.Image.Save(fs,
                               System.Drawing.Imaging.ImageFormat.Jpeg);
                            break;

                        case 2:
                            picCover.Image.Save(fs,
                               System.Drawing.Imaging.ImageFormat.Bmp);
                            break;

                        case 3:
                            picCover.Image.Save(fs,
                               System.Drawing.Imaging.ImageFormat.Gif);
                            break;

                        case 4:
                            picCover.Image.Save(fs,
                               System.Drawing.Imaging.ImageFormat.Png);
                            break;

                    }

                    fs.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        #endregion

       
    }
}

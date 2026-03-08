using GradientApp;
using Hqub.MusicBrainz.API.Entities;
using Karaboss.Lrc.SharedFramework;
using KFNViewer;
using Mozilla.NUniversalCharDet;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TagLib.Id3v2;

namespace Karaboss.Kfn
{
    public partial class frmKfnExport : Form
    {

        private KFN KFN;
        private string exportType;
        private ID3Tags ID3Class = new ID3Tags();
        private readonly FolderBrowserDialog FolderBrowserDialog = new FolderBrowserDialog();
        private Dictionary<int, string> encodings = new Dictionary<int, string>
        {
            { 0, "ANSI (System default)" },
            { 65001, "UTF-8 (KFN default)" }
        };

        public frmKfnExport(string exportType, KFN KFN)
        {
            InitializeComponent();

            lblVideo.Visible = (exportType == "EMZ");
            cbVideoSelect.Visible = (exportType == "EMZ");
            btnPlayVideo.Visible = (exportType == "EMZ");
            chkDeleteID3Tags.Checked = true;
            chkDeleteID3Tags.Visible = (exportType == "MP3+LRC");
            lblArtist.Visible = (exportType != "EMZ");
            lblTitle.Visible = (exportType != "EMZ");
            cbArtistSelect.Visible = (exportType != "EMZ");
            cbTitleSelect.Visible = (exportType != "EMZ") ;
            lblEnc.Visible = (exportType != "EMZ");
            cbEncSelect.Visible = (exportType != "EMZ");

            // AUDIO
            #region audio
            List<KFN.ResourceFile> audios = KFN.Resources.Where(r => r.FileType == "Audio").ToList();
            string audioSource = KFN.GetAudioSourceName();

            //cbAudioSelect.ItemsSource = audios;                        
            //cbAudioSelect.DisplayMemberPath = "FileName";
            cbAudioSelect.DataSource = audios;
            cbAudioSelect.ValueMember = "FileName";            
            cbAudioSelect.DisplayMember = "FileName";           
            /*
            for (int i = 0; i < audios.Count; i++)
            {
                cbAudioSelect.Items.Add(audios[i].FileName);
            }
            */

            cbAudioSelect.SelectedItem = audios.Where(a => a.FileName == audioSource).FirstOrDefault().FileName;                        
                        
            if (cbAudioSelect.SelectedItem == null)
            {
                MessageBox.Show("Can`t find audio source!");
                return;
            }
            if (audios.Count == 1) { cbAudioSelect.Enabled = false; }
            #endregion audio


            // LYRICS
            #region lyrics
            Dictionary<string, string> lyrics = new Dictionary<string, string>();
            List<KFN.ResourceFile> texts = KFN.Resources.Where(r => r.FileType == "Text").ToList();
            foreach (KFN.ResourceFile resource in texts)
            {
                lyrics.Add(resource.FileName, this.GetResourceText(resource));
            }

            KFN.ResourceFile songIni = KFN.Resources.Where(r => r.FileName == "Song.ini").First();
            byte[] data = KFN.GetDataFromResource(songIni);
            string iniText = new string(Encoding.UTF8.GetChars(data));
            SongINI sINI = new SongINI(iniText);
            foreach (SongINI.BlockInfo block in sINI.Blocks.Where(b => b.Id == "1" || b.Id == "2"))
            {
                string lyricFromBlock = (exportType == "EMZ")
                    ? KFN.INIToELYR(block.Content)
                    : KFN.INIToExtLRC(block.Content);
                if (lyricFromBlock != null)
                {
                    lyrics.Add("Song.ini: " + block.Name, lyricFromBlock);
                }
                else
                {
                    lyrics.Add("Song.ini: " + block.Name, "Can`t convert lyric from Song.ini");
                }
            }

            //cbLyricSelect.DisplayMemberPath = "Key";
            //foreach (var l in lyrics)
            //{
            //    cbLyricSelect.Items.Add(l.Key);              
            //}                        
            //cbLyricSelect.ItemsSource = lyrics;
            cbLyricSelect.DataSource = new BindingSource(lyrics, null);
            cbLyricSelect.ValueMember = "Value";
            cbLyricSelect.DisplayMember = "Key";

            if (cbLyricSelect.Items.Count > 0)
                cbLyricSelect.SelectedIndex = 0;

            if (lyrics.Count == 1) { cbLyricSelect.Enabled = false; }



            txtLyricPreview.Text = ((KeyValuePair<string, string>)cbLyricSelect.SelectedItem).Value.Replace("\n", Environment.NewLine);
            /*
            foreach (var l in lyrics)
            {
                if (cbLyricSelect.SelectedItem.ToString() == l.Key)
                {
                    txtLyricPreview.Text = l.Value.Replace("\n", Environment.NewLine);
                    break;
                }
            }
            */
            #endregion lyrics


            // ARTIST-TITLE
            #region artist title
            if (exportType == "MP3+LRC")
            {
                List<string> artists = new List<string> { null };
                List<string> titles = new List<string> { null };

                KeyValuePair<string, string> kfnArtist = KFN.Properties.Where(p => p.Key == "Artist").FirstOrDefault();
                if (kfnArtist.Value != null && kfnArtist.Value.Length > 0) { artists.Add(kfnArtist.Value); }
                KeyValuePair<string, string> kfnTitle = KFN.Properties.Where(p => p.Key == "Title").FirstOrDefault();
                if (kfnTitle.Value != null && kfnTitle.Value.Length > 0) { titles.Add(kfnTitle.Value); }

                foreach (KFN.ResourceFile resource in KFN.Resources.Where(r => r.FileType == "Audio"))
                {
                    string[] atFromID3 = ID3Class.GetArtistAndTitle(KFN.GetDataFromResource(resource));
                    if (atFromID3[0] != null) { artists.Add(atFromID3[0]); }
                    if (atFromID3[1] != null) { titles.Add(atFromID3[1]); }
                }
                artists = artists.Distinct().ToList();
                titles = titles.Distinct().ToList();

                //cbArtistSelect.ItemsSource = artists;
                foreach (string artist in artists)
                {
                    if (string.IsNullOrEmpty(artist)) continue;
                    cbArtistSelect.Items.Add(artist);
                }
                if (cbArtistSelect.Items.Count > 0)
                    cbArtistSelect.SelectedIndex = 0;

                //cbTitleSelect.ItemsSource = titles;
                foreach (string title in titles)
                {
                    if (string.IsNullOrEmpty(title)) continue;
                    cbTitleSelect.Items.Add(title);
                }
                if (cbTitleSelect.Items.Count > 0)
                    cbTitleSelect.SelectedIndex = 0;

                //cbEncSelect.ItemsSource = this.encodings;
                //cbEncSelect.DisplayMemberPath = "Value";
                foreach (var encoding in encodings)
                {
                    if (string.IsNullOrEmpty(encoding.Value)) continue;
                    cbEncSelect.Items.Add(encoding.Value);
                }
                if (cbEncSelect.Items.Count > 0)
                    cbEncSelect.SelectedIndex = 0;
            }
            #endregion artist title

            // VIDEO
            #region video
            if (exportType == "EMZ")
            {
                List<KFN.ResourceFile> videos = KFN.Resources.Where(r => r.FileType == "Video").ToList();
                if (videos.Count == 0)
                {
                    videos.Add(new KFN.ResourceFile("Video", "video not found", 0, 0, 0, false));
                    cbVideoSelect.Enabled = false;
                }
                else
                {
                    videos.Add(new KFN.ResourceFile("Video", "don`t use video", 0, 0, 0, false));
                }
                //cbVideoSelect.ItemsSource = videos;
                //cbVideoSelect.DisplayMemberPath = "FileName";

                cbVideoSelect.DataSource = videos;
                cbVideoSelect.ValueMember = "FileName";
                cbVideoSelect.DisplayMember = "FileName";

                //foreach (var video in videos)
                //{
                //    if (string.IsNullOrEmpty(video.FileName)) continue;
                //    cbVideoSelect.Items.Add(video.FileName);
                //}

                if (cbVideoSelect.Items.Count > 0)
                    cbVideoSelect.SelectedIndex = 0;
            }
            #endregion video

        }



        private string GetResourceText(KFN.ResourceFile resource)
        {
            byte[] data = KFN.GetDataFromResource(resource);

            ////UTF-8
            int detEncoding = 65001;
            UniversalDetector Det = new UniversalDetector(null);
            Det.HandleData(data, 0, data.Length);
            Det.DataEnd();
            string enc = Det.GetDetectedCharset();
            if (enc != null && enc != "Not supported")
            {
                // fix encoding for 1251 upper case and MAC
                //if (enc == "KOI8-R" || enc == "X-MAC-CYRILLIC") { enc = "WINDOWS-1251"; }
                Encoding denc = Encoding.GetEncoding(enc);
                detEncoding = denc.CodePage;
            }

            return new string(Encoding.GetEncoding(detEncoding).GetChars(data));
        }


        private void UpdateArtistTitleInLRC(string artist, string title)
        {
            string origText = txtLyricPreview.Text;
            if (artist != null && artist.Length > 0)
            {
                if (Regex.IsMatch(origText, @"\[ar:[^\]]+\]"))
                {
                    origText = Regex.Replace(origText, @"\[ar:[^\n]+", "[ar:" + artist + "]");
                }
                else
                {
                    origText = "[ar:" + artist + "]\n" + origText;
                }
            }
            if (title != null && title.Length > 0)
            {
                if (Regex.IsMatch(origText, @"\[ti:[^\]]+\]"))
                {
                    origText = Regex.Replace(origText, @"\[ti:[^\n]+", "[ti:" + title + "]");
                }
                else
                {
                    origText = "[ti:" + title + "]\n" + origText;
                }
            }
            txtLyricPreview.Text = origText;
        }


        private void btnPlayVideo_Click(object sender, EventArgs e)
        {

        }

        private void btnPlayAudio_Click(object sender, EventArgs e)
        {

        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            KFN.ResourceFile audio = (KFN.ResourceFile)cbAudioSelect.SelectedItem;
            if (audio == null) { return; }

            string lyric = txtLyricPreview.Text;
            if (lyric.Length == 0 || lyric.Contains("Can`t convert lyric from Song.ini")) { return; }

            FileInfo kfnFile = new FileInfo(KFN.FullFileName);
            FolderBrowserDialog.SelectedPath = kfnFile.DirectoryName;
            if (FolderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string exportFolder = FolderBrowserDialog.SelectedPath;
                try
                {
                    System.Security.AccessControl.DirectorySecurity ds = Directory.GetAccessControl(exportFolder);
                }
                catch (UnauthorizedAccessException error)
                {
                    MessageBox.Show(error.Message);
                    return;
                }

                if (this.exportType == "EMZ")
                {
                    KFN.ResourceFile video = (KFN.ResourceFile)cbVideoSelect.SelectedItem;
                    byte[] fileData = KFN.createEMZ(lyric, video.FileLength > 0, video, audio);
                    if (fileData == null)
                    {
                        MessageBox.Show((KFN.isError != null)
                            ? KFN.isError
                            : "Fail to create EMZ!");
                        return;
                    }
                    string emzFileName = kfnFile.Name.Substring(0, kfnFile.Name.Length - kfnFile.Extension.Length) + ".emz";
                    using (FileStream fs = new FileStream(exportFolder + "\\" + emzFileName, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(fileData, 0, fileData.Length);
                    }
                    MessageBox.Show("Export OK: " + exportFolder + "\\" + emzFileName);
                }
                else if (this.exportType == "MP3+LRC")
                {
                    FileInfo audioFile = new FileInfo(audio.FileName);
                    string mp3FileName = kfnFile.Name.Substring(0, kfnFile.Name.Length - kfnFile.Extension.Length) + audioFile.Extension;
                    string lrcFileName = kfnFile.Name.Substring(0, kfnFile.Name.Length - kfnFile.Extension.Length) + ".lrc";

                    byte[] mp3Data = KFN.GetDataFromResource(audio);
                    if (chkDeleteID3Tags.Checked == true)
                    {
                        mp3Data = ID3Class.RemoveAllTags(mp3Data);
                    }
                    using (FileStream fs = new FileStream(exportFolder + "\\" + mp3FileName, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(mp3Data, 0, mp3Data.Length);
                    }

                    int encCode = ((KeyValuePair<int, string>)cbEncSelect.SelectedItem).Key;
                    Encoding lrcEnc = (encCode == 0) ? Encoding.Default : Encoding.GetEncoding(encCode);
                    byte[] lrcData = lrcEnc.GetBytes(lyric);
                    byte[] bom = lrcEnc.GetPreamble();
                    using (FileStream fs = new FileStream(exportFolder + "\\" + lrcFileName, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(bom, 0, bom.Length);
                        fs.Write(lrcData, 0, lrcData.Length);
                    }
                    MessageBox.Show("Export OK: " + exportFolder + "\\" + mp3FileName);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cbVideoSelect_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cbAudioSelect_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cbLyricSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtLyricPreview.Text = ((KeyValuePair<string, string>)cbLyricSelect.SelectedItem).Value.Replace("\n", Environment.NewLine);            
        }

        private void cbArtistSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.UpdateArtistTitleInLRC((string)cbArtistSelect.SelectedItem, null);
        }

        private void cbTitleSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.UpdateArtistTitleInLRC(null, (string)cbTitleSelect.SelectedItem);
        }

        private void cbEncSelect_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void chkDeleteID3Tags_CheckedChanged(object sender, EventArgs e)
        {

        }

        #region form load close
        private void frmKfnExport_Load(object sender, EventArgs e)
        {

        }

        private void frmKfnExport_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void frmKfnExport_Resize(object sender, EventArgs e)
        {
            txtLyricPreview.Width = this.ClientSize.Width - txtLyricPreview.Left;
            txtLyricPreview.Height = ClientSize.Height - txtLyricPreview.Top;
        }

        #endregion form load close

    }
}

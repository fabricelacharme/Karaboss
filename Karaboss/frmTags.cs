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
using System.Windows.Forms;
using Sanford.Multimedia.Midi;

namespace Karaboss
{
    public partial class frmTags : Form
    {
        private Sequence sequence1;
        
        public frmTags(Sequence seq)
        {
            InitializeComponent();

            sequence1 = seq;

            // Display tags
            DisplayTags();

        }


        private void DisplayTags()
        {
            string cr = Environment.NewLine;
            int i = 0;

            // New tags
            txtTitle.Text = sequence1.TagTitle;
            txtArtist.Text = sequence1.TagArtist;
            txtAlbum.Text = sequence1.TagAlbum;
            txtDate.Text = sequence1.TagDate;
            txtGenre.Text = sequence1.TagGenre;
            txtEditor.Text = sequence1.TagEditor;
            txtCopyright.Text = sequence1.TagCopyright;
            txtEvaluation.Text = sequence1.TagEvaluation;
            txtComment.Text = sequence1.TagComment;
            
            
            // Classic Karaoke Midi tags
            /*
            @K	(multiple) K1: FileType ex MIDI KARAOKE FILE, K2: copyright of Karaoke file
            @L	(single) Language	FRAN, ENGL        
            @W	(multiple) Copyright (of Karaoke file, not song)        
            @T	(multiple) Title1 @T<title>, Title2 @T<author>, Title3 @T<copyright>		
            @I	Information  ex Date(of Karaoke file, not song)
            @V	(single) Version ex 0100 ?             
            */

            for (i =0; i < sequence1.KTag.Count; i++) {
                txtKTag.Text += sequence1.KTag[i] + cr;
            }
            for (i = 0; i < sequence1.WTag.Count; i++)
            {
                txtWTag.Text += sequence1.WTag[i] + cr;
            }
            for (i = 0; i < sequence1.TTag.Count; i++)
            {
                txtTTag.Text += sequence1.TTag[i] + cr;
            }
            for (i = 0; i < sequence1.ITag.Count; i++)
            {
                txtITag.Text += sequence1.ITag[i] + cr;
            }
            for (i = 0; i < sequence1.VTag.Count; i++)
            {
                txtVTag.Text += sequence1.VTag[i] + cr;
            }
            for (i = 0; i < sequence1.LTag.Count; i++)
            {
                txtLTag.Text += sequence1.LTag[i] + cr;
            }            
        }

        /// <summary>
        /// Button: Save tags into the Midi file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSave_Click(object sender, EventArgs e)
        {
            bool bModified = false;
            string tx = string.Empty;

            sequence1.TagTitle = txtTitle.Text.Trim();
            sequence1.TagArtist = txtArtist.Text.Trim();
            sequence1.TagAlbum = txtAlbum.Text.Trim();
            sequence1.TagDate = txtDate.Text.Trim();
            sequence1.TagGenre = txtGenre.Text.Trim();
            sequence1.TagEditor = txtEditor.Text.Trim();
            sequence1.TagCopyright = txtCopyright.Text.Trim();
            sequence1.TagEvaluation = txtEvaluation.Text.Trim();
            sequence1.TagComment = txtComment.Text.Trim();

            string[] S;
            string newline = string.Empty;


            sequence1.ITag.Clear();
            sequence1.KTag.Clear();
            sequence1.LTag.Clear();
            sequence1.TTag.Clear();
            sequence1.VTag.Clear();
            sequence1.WTag.Clear();

            tx = txtITag.Text.Trim();
            S = tx.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in S ) {
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
           

            if (sequence1.TagTitle != "" || sequence1.TagArtist != "" || sequence1.TagAlbum != "" || sequence1.TagDate != "" || sequence1.TagGenre != "" || sequence1.TagEditor != "" || sequence1.TagCopyright != "" || sequence1.TagEvaluation != "" || sequence1.TagComment != "")
            {
                bModified = true;    
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
                    frmPlayer frmPlayer = GetForm<frmPlayer>();
                    //frmPlayer.bfilemodified = true;
                    frmPlayer.FileModified();
                }
                MessageBox.Show("Tags saved successfully", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Close();
            }
        }
        
        /// <summary>
        /// Add tags to midi file
        /// </summary>
        private void AddTags()
        {
            int i = 0;

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
            string tx = string.Empty;
            track.RemoveTagsEvent("@I");
            track.RemoveTagsEvent("@K");
            track.RemoveTagsEvent("@L");
            track.RemoveTagsEvent("@T");
            track.RemoveTagsEvent("@V");
            track.RemoveTagsEvent("@W");

            for ( i = sequence1.ITag.Count - 1; i >= 0; i--)
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

        
    }
}

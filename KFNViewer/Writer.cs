using KFNViewer.Properties;
using KFNViewer.SongIni;
using KFNViewer.Utilities;
using Mozilla.NUniversalCharDet;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using static KFN;

namespace KFNViewer
{
    public class KfnWriter
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Comment { get; set; }
        public string Year { get; set; }
        public string Author { get; set; }

        public string BgColor { get; set; }
        public (string, uint) Font { get; set; }
        
        public string fullFileName {  get; set; }
        public List<string> lstAudioFiles {  get; set; }
        public string lyricsFileName { get; set; }
        public List<string> lstImages { get; set; }

        public string SongIniFile { get; set; }

        public int Offset { get; set; }

        private List<ResourceFile> resources = new List<ResourceFile>();

        public List<ResourceFile> Resources
        {
            get { return this.resources; }
        }

        // Song.ini file
        public KfnIni kfnIni { get; set; }


        private Dictionary<int, string> fileTypes = new Dictionary<int, string> {
        {0, "Text"},
        {1, "Config"},
        {2, "Audio"},
        {3, "Image"},
        {4, "Font"},
        {5, "Video"},
        {6, "Visualization"}
         };

        private int GetFileTypeId(string type)
        {
            KeyValuePair<int, string> ftype = this.fileTypes.Where(ft => ft.Value == type).FirstOrDefault();
            return (ftype.Value == null) ? -1 : ftype.Key;
        }

        // US-ASCII
        private int resourceNamesEncodingAuto = 20127;

        public KfnWriter(string fpath, List<string> LstAudioFiles, string LyricsFile, List<string> lstImg, string title, string artist, string comment, string year, string author, string bgColor, (string, uint) font)
        {
            fullFileName = fpath;
            lstAudioFiles = LstAudioFiles;
            lyricsFileName = LyricsFile;
            lstImages = lstImg;
                        

            Title = title;
            Artist = artist;
            Comment = comment;
            Year = year;
            Author = author;
            BgColor = bgColor;
            Font = font;

            Offset = 0;
                        
            // Initialize list of resources
            resources = new List<ResourceFile>();

            // Initalize Song.ini
            kfnIni = new KfnIni();
        }
                        

      
        /// <summary>
        /// Create a new KFN file
        /// </summary>
        public void CreateKFN()
        {           
            string Source = Path.GetFileName(lstAudioFiles[0]);

            // Populate resources with audios, images except Song.ini
            AddFilesToRessource();

            // Convert LRC File
            (List<int>, List<string>) Eff2Lyrics = LrcToIni(lyricsFileName);
            

            // Create Song.ini
            CreateIniFile(Source, Title, Artist, Comment, Year, Author, BgColor, Font, Eff2Lyrics);

            // Add Song.ini
            SongIniFile = kfnIni.ToString();
            AddSongIniToResources(SongIniFile);                                          

            try
            {
                using (FileStream fs = new FileStream(fullFileName, FileMode.Create, FileAccess.ReadWrite))
                {                                                                                
                    // Write bytes properties
                    WriteBytesProperties(fs, Title, Comment, Source);

                    // Write bytes resources
                    WriteBytesResources(fs);

                    // Write files contents
                    WriteFilesContents(fs);                    
                }

                string tx = string.Format("The File \n{0} \nwas created in the directory\n {1}", Path.GetFileName(fullFileName), Path.GetDirectoryName(fullFileName));
                MessageBox.Show(tx, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }                
        }

        /// <summary>
        /// Write properties
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="sTitle"></param>
        /// <param name="sComment"></param>
        private void WriteBytesProperties(FileStream fs, string sTitle, string sComment, string mp3file)
        {
            #region example

            /*                                         
            *   prop (5 bytes)    propvalue (4 bytes)                                      
           [0]: ("75,70,78,66", "", "KFNB", "signature")
           [1]: ("68,73,70,77,1", "1,0,0,0", "DIFM", "Man difficult")
           [2]: ("68,73,70,87,1", "1,0,0,0", "DIFW", "Woman difficult")
           [3]: ("71,78,82,69,1", "255,255,255,255", "GNRE", "Genre")
           [4]: ("83,70,84,86,1", "21,90,20,1", "SFTV", "SFTV")
           [5]: ("77,85,83,76,1", "184,0,0,0", "MUSL", "MUSL")
           [6]: ("65,78,77,69,1", "13,0,0,0", "ANME", "ANME")
           [7]: ("84,89,80,69,1", "0,0,0,0", "TYPE", "TYPE")

           [8]: ("70,76,73,68,2", "16,0,0,0", "FLID", "AES-ECB-128 Key")
           [9]: ("76,65,78,71,2", "2,0,0,0", "LANG", "Language")
           [10]: ("84,73,84,76,2", "15,0,0,0", "TITL", "Title")
           [11]: ("83,79,82,67,2", "23,0,0,0", "SORC", "Source")
           [12]: ("67,79,77,77,2", "64,0,0,0", "COMM", "Comment")

           [13]: ("82,71,72,84,1", "0,0,0,0", "RGHT", "RGHT")
           [14]: ("80,82,79,86,1", "0,0,0,0", "PROV", "PROV")
           [15]: ("73,68,85,83,2", "16,0,0,0", "IDUS", "IDUS")
           [16]: ("69,78,68,72,1", "", "ENDH", "End of properties")
           */
            /*
            ?properties
                Count = 8
                [0]: { [Man difficult, 1]}
                [1]: { [Woman difficult, 1]}
                [2]: { [Genre, Not set]}
                [3]: { [AES-ECB - 128 Key, Not present]}
                [4]: { [Language, en]}
                [5]: { [Title, Chiens -Louane]}
                [6]: { [Source, 1,I,Chiens - Louane.mp3]}
                [7]: { [Comment, Converti de  le 02 / 03 / 2026 avec KarPbo V1.2.0(c)2009 A.Agapoff.]}
            */

            #endregion example

            //string prop;
            byte[] propValue = new byte[4];
            byte[] data;
            byte[] datalength = new byte[4];
            int length = 0;

            // [0] Signature (4 bytes [75,70,78,66])
            string signature = "KFNB";
            byte[] bsignature = Encoding.GetEncoding(this.resourceNamesEncodingAuto).GetBytes(signature);
            fs.Write(bsignature, 0, bsignature.Length);

            // [1] Man Difficult = "DIFM";
            WriteProp(fs, new byte[] { 68, 73, 70, 77, 1 }, new byte[] { 1, 0, 0, 0 });

            // [2]: Woman difficult = "DIFW"
            WriteProp(fs, new byte[] { 68, 73, 70, 87, 1 }, new byte[] { 1, 0, 0, 0 });


            // [3]: ("", "255,255,255,255", "GNRE", "Genre")
            WriteProp(fs, new byte[] { 71, 78, 82, 69, 1 }, new byte[] { 255, 255, 255, 255 });


            // [4]: "SFTV", "SFTV"
            WriteProp(fs, new byte[] { 83, 70, 84, 86, 1 }, new byte[] { 21, 90, 20, 1 });

            // [5]: "MUSL", "MUSL"
            WriteProp(fs, new byte[] { 77, 85, 83, 76, 1 }, new byte[] { 184, 0, 0, 0 });


            // [6]: "ANME", "ANME"
            WriteProp(fs, new byte[] { 65, 78, 77, 69, 1 }, new byte[] { 13, 0, 0, 0 });                      
           

            // [7]: "TYPE", "TYPE"
            WriteProp(fs, new byte[] { 84, 89, 80, 69, 1 }, new byte[] { 0, 0, 0, 0 });

            // [8]: "FLID", "AES-ECB-128 Key"
            WriteProp(fs, new byte[] { 70, 76, 73, 68, 2 }, new byte[] { 16, 0, 0, 0 });            
            propValue = new byte[] { 16, 0, 0, 0 };
            data = new byte[BitConverter.ToUInt32(propValue, 0)];
            fs.Write(data, 0, data.Length);

            // [9]: "LANG", "Language"
            string lang = "en";
            // convert length to byte[4]
            datalength = BitConverter.GetBytes(lang.Length);
            WriteProp(fs, new byte[] { 76, 65, 78, 71, 2 }, datalength);            
            
            propValue = Encoding.GetEncoding(this.resourceNamesEncodingAuto).GetBytes(lang);
            fs.Write(propValue, 0, propValue.Length);

            //[10]: "TITL", "Title"
            string title = sTitle;
            // convert length to byte[4]
            datalength = BitConverter.GetBytes(title.Length);      
            WriteProp(fs, new byte[] { 84, 73, 84, 76, 2 }, datalength);
            propValue = Encoding.GetEncoding(this.resourceNamesEncodingAuto).GetBytes(title);
            fs.Write(propValue, 0, propValue.Length);

            
            // [11]: "SORC", "Source"
            string source = "1,I," + mp3file;            
            // convert length to byte[4]
            datalength = BitConverter.GetBytes(source.Length);
            WriteProp(fs, new byte[] { 83, 79, 82, 67, 2 }, datalength);                       
            propValue = Encoding.GetEncoding(this.resourceNamesEncodingAuto).GetBytes(source);
            fs.Write(propValue, 0, propValue.Length);


            // [12]: "COMM", "Comment"
            string comment = sComment;
            // convert length to byte[4]
            datalength = BitConverter.GetBytes(comment.Length);
            WriteProp(fs, new byte[] { 67, 79, 77, 77, 2 }, datalength);
            propValue = Encoding.GetEncoding(this.resourceNamesEncodingAuto).GetBytes(comment);                                    
            fs.Write(propValue, 0, propValue.Length);

            // [13]: "RGHT", "RGHT"
            // Change the publishing rights to 0 = NOT protected
            WriteProp(fs, new byte[] { 82, 71, 72, 84, 1 }, new byte[] { 0, 0, 0, 0 });

            // [14]: (",1", "0,0,0,0", "PROV", "PROV")
            //WriteProp(fs, new byte[] { 80, 82, 79, 86, 1 }, new byte[] { 10, 0, 0, 0 });

            // [15]: "IDUS", "IDUS"
            //WriteProp(fs, new byte[] { 73, 68, 85, 83, 2 }, new byte[] { 16, 0, 0, 0 });
            // Ecrire
            //propValue = new byte[] { 16, 0, 0, 0 };
            //data = new byte[BitConverter.ToUInt32(propValue, 0)];
            //fs.Write(data, 0, data.Length);


            // [16]: ("69,78,68,72,1", "", "ENDH", "End of properties")
            data = new byte[] { 69, 78, 68, 72, 1 };
            fs.Write(data, 0, data.Length);

            // Add 4 bytes for nothing ????
            data = new byte[] { 0, 0, 0, 0 };
            fs.Write(data, 0, data.Length);
        }


        private void WriteProp(FileStream fs, byte[] prop, byte[] propValue)
        {
            fs.Write(prop, 0, prop.Length);
            fs.Write(propValue, 0, propValue.Length);
        }

       
        /// <summary>
        /// Write resources informations in bytes
        /// </summary>
        /// <param name="fs"></param>
        private void WriteBytesResources(FileStream fs)
        {            
            int nbchars;
            byte[] bnbchars;
            byte[] datalength = new byte[4];

            // Write number of resources                        
            int resourcesCount = resources.Count;

            byte[] numOfResources = BitConverter.GetBytes(resourcesCount);
            fs.Write(numOfResources, 0, numOfResources.Length);

            for (int i = 0; i < resourcesCount; i++)
            {
                ResourceFile res = resources[i];

                // [0] name = "Chiens - Louane.mp3"                                        
                byte[] resourceName = Encoding.GetEncoding(this.resourceNamesEncodingAuto).GetBytes(res.FileName);
                nbchars = resourceName.Length;    // 19 chars
                bnbchars = BitConverter.GetBytes(nbchars);
                // First write the length of the name (ie 19 for "Chiens - Louane.mp3")
                fs.Write(bnbchars, 0, bnbchars.Length);
                // Second write the name itself
                fs.Write(resourceName, 0, resourceName.Length);

                // [1] Type
                byte[] type = BitConverter.GetBytes(this.GetFileTypeId(res.FileType));
                fs.Write(type, 0, type.Length);

                // [2] Length
                byte[] resourceLenght = BitConverter.GetBytes(res.FileLength);
                fs.Write(resourceLenght, 0, resourceLenght.Length);

                // [3} Offset
                byte[] resourceOffet = BitConverter.GetBytes(res.FileOffset);
                fs.Write(resourceOffet, 0, resourceOffet.Length);

                // [4] EncryptedLenght
                byte[] resourceEncLength = BitConverter.GetBytes(res.EncLength);
                fs.Write(resourceEncLength, 0, resourceEncLength.Length);

                //[5] is encypted
                byte[] IsEncrypted = BitConverter.GetBytes(res.IsEncrypted);
                nbchars = IsEncrypted.Length;
                bnbchars = BitConverter.GetBytes(nbchars);
                fs.Write(new byte[] { 0, 0, 0, 0 }, 0, 4);

            }
        }
    

        /// <summary>
        /// Write files contents of audios, images and Song.ini  at the end of kfn file
        /// </summary>
        /// <param name="fs"></param>
        private void WriteFilesContents(FileStream fs)
        {
            byte[] fileBytes;

            // Write mp3
            for (int i = 0; i < lstAudioFiles.Count; i++)
            {
                if (File.Exists(lstAudioFiles[i]))
                {
                    fileBytes = System.IO.File.ReadAllBytes(lstAudioFiles[i]);
                    fs.Write(fileBytes, 0, fileBytes.Length);
                }
            }

            // Write images
            for (int i = 0; i < lstImages.Count; i++)
            {
                if (File.Exists(lstImages[i]))
                {
                    fileBytes = System.IO.File.ReadAllBytes(lstImages[i]);
                    fs.Write(fileBytes, 0, fileBytes.Length);
                }
            }

            // Last, write Song.ini
            fileBytes = Encoding.UTF8.GetBytes(SongIniFile);
            fs.Write(fileBytes, 0, fileBytes.Length);
        }


        /// <summary>
        /// Create Song.ini file
        /// </summary>
        /// <param name="fs"></param>
        private void CreateIniFile(string Source, string Title, string Artist, string Comment, string Year, string Author, string BgColor, (string, uint) Font, (List<int>, List<string>) Eff2Lyrics )  
        {            
            kfnIni.PopulateEmpty();

            #region General
            /*
            [General]
            Title = Ya ya twist
            Artist = Johnny Hallyday, richard antohny, petula clarck......etc
            Album =
            Composer =
            Year =
            Track =
            GenreID = -1
            Copyright =
            Comment = chanson type des années 60 reprise par une multitude d'artistes
            Source = 1,I,Richard_Anthony_Yaya_twist_(Instrumental)_17710.mp3
            EffectCount = 2
            LanguageID = FR
            DiffMen = 0
            DiffWomen = 0
            KFNType = 0
            Properties = 24
            KaraokeVersion =
            VocalGuide =
            KaraFunization = Dankaraok
            InfoScreenBmp =
            */


            KfnHeader kfnHeader = new KfnHeader();
            kfnHeader.Title = Title;
            kfnHeader.Artist = Artist;
            kfnHeader.Comment = Comment;   
            kfnHeader.SourceFile = Source;
            kfnHeader.Year = Year;
            kfnHeader.Karafunizer = Author;

            kfnIni.PopulateFromHeader(kfnHeader);

            #endregion General


            #region Materials
            /*
            [Materials]
            MatCount = 10
            Mat0 = Richard_Anthony_Yaya_twist_(Instrumental)_17710.mp3
            Mat1 = Richard_Anthony_Yaya_twist_(Instrumental_avec_chanteur)_17711.mp3
            Mat2 = imagesCAKS0X8N.jpg
            Mat3 = imagesCA8G8R3P.jpg
            Mat4 = imagesCAA3W1ZK.jpg
            Mat5 = imagesCAAOWOS3.jpg
            Mat6 = imagesCAD7NM53.jpg
            Mat7 = imagesCAJS9BBJ.jpg
            Mat8 = imagesCAJUNNLW.jpg
            Mat9 = Copie de dan fond karafun.bmp
            */


            /* ResourceFile
            Type = type;
            Name = name;
            EncryptedLength = enclength;
            Length = length;
            Offset = offset;
            Encrypted = encrypted;
            IsAudioSource = aSource;
 
            //ResourceFile res = new ResourceFile("Audio", "Chiens - Louane.mp3", 2943507, 2943507, 0, false, true);
            res = new ResourceFile("Audio", fi.Name, length, length, Offset, false, true); 
             */

            List<Entry> entries = new List<Entry>();           
            int Offset = 0;

            if (Resources.Count == 0)
            {
                MessageBox.Show("Song.ini creation: Warning, no resources found");
            }

            // Audio files
            ResourceFile[] audios_resources = resources.Where(r => r.FileType == "Audio").ToArray();
            foreach (ResourceFile resource in audios_resources)
            {
                Entry entry = new Entry();
                entry.FileName = resource.FileName;
                entry.Length1 = resource.EncLength;
                entry.Length2 = resource.FileLength;
                entry.Offset = Offset;

                entries.Add(entry);
                Offset += Offset;
            }
            

            // Images
            ResourceFile[] images_resources = resources.Where(r => r.FileType == "Image").ToArray();
            foreach (ResourceFile resource in images_resources)
            {
                Entry entry = new Entry();
                entry.FileName = resource.FileName;
                entry.Length1 = resource.EncLength;
                entry.Length2 = resource.FileLength;
                entry.Offset = Offset;

                entries.Add(entry);
                Offset += Offset;
            }

           // Populate section [Materials]
            kfnIni.SetMaterials(entries);

            #endregion Materials


            #region Set MP3File

            kfnIni.SetMP3Music(audios_resources);


            #endregion set MP3File


            #region set Marks

            kfnIni.SetMarks();

            #endregion set MArks


            #region Anims

            List<Anim> lstAnim = new List<Anim>();
            
            if (images_resources.Count() > 0)
            {
                string imageName = images_resources[0].FileName;
                string s =  imageName + ", Effect = NoTransition, TransitionTime = 0, TransType = Linear";
                SongIni.Action action = new SongIni.Action.ChgBgImg(s);
                AnimEntry ae = new AnimEntry(action, EffectKind.AlphaBlending, 0, TransType.Smooth);
                List<AnimEntry> lstAnimEntries = new List<AnimEntry>();
                lstAnimEntries.Add(ae);

                Anim anim = new Anim(0, lstAnimEntries);
                lstAnim.Add(anim);
            }
            #endregion Anims


            #region Eff
            /*             
            id = Id;
            num = Num;
            anims = Anims;
            initial_lib_image = Initial_lib_image;
            initial_video_file = Initial_video_file;
            initial_font = Initial_font;
            initial_active_color =  ;
            initial_inactive_color = Initial_inactive_color;
            syncs = Syncs;
            texts = Texts;
            initial_trajectory = Initial_trajectory;

            [Eff1]
            ID=51
            InPractice=0
            Enabled=-1
            Locked=0
            Color=#000000
            LibImage=
            ImageColor=#FFFFFFFF
            AlphaBlending=Opacity
            OffsetX=0
            OffsetY=0
            Depth=0
            NbAnim=0

            */

            // Reset effs
            kfnIni.Effs = new List<Eff>();
                       
            #region eff1
            int effnum = 1;

            Eff eff = new Eff(
                51,                         // first ID is always 51
                effnum,                     // num
                lstAnim,                   //new List<Anim>(),           // anims
                null, 
                null,
                Font,                    // font
                "#00ACFFFF",                       // Initial active color
                "#FFFFFFFF",                       // initial inactive color
                new List<int>(),           // syncs
                new List<TextEntry>(),     // Texts
                Trajectory.Default());
            
            kfnIni.Effs.Add(eff);

            // Set backgroud color
            kfnIni.SetBackground(BgColor);

            #endregion eff1


            #region eff2

            effnum = 2;

            #region lyrics
            List<TextEntry> textLst = new List<TextEntry>();
            List<(int, string)> fragments = new List<(int, string)>();
            TextEntry text;

            // All times (must be cut further)
            List<int> syncsLst = Eff2Lyrics.Item1;

            // Texts: Tex0=bla bla
            for (int i = 0; i < Eff2Lyrics.Item2.Count; i++)
            {
                text = new TextEntry(Eff2Lyrics.Item2[i], fragments, effnum);
                textLst.Add(text);
            }
            #endregion lyrics

           
            eff = new Eff(
                1,  // second ID is 1
                effnum,  // num
                new List<Anim>(),           // anims
                null,
                null,
                Font,        //   ("", 0),                    // font
                "#00ACFFFF",                // initial active color
                "#FFFFFFFF",                // initial inactive color
                syncsLst,                   // Sync
                textLst,                    // Texts
                Trajectory.Default());

            kfnIni.Effs.Add(eff) ;

            #endregion eff2

            kfnIni.SetEff();

            #endregion Eff                      
        }

        /// <summary>
        /// Add audio files, images and Song.ini to resources
        /// </summary>
        private void AddFilesToRessource()
        {
            FileInfo fi;
            int length;
            ResourceFile res;

            // Audios files
            for (int i = 0; i < lstAudioFiles.Count; i++)
            {
                if (File.Exists(lstAudioFiles[i]))
                {
                    fi = new FileInfo(lstAudioFiles[i]);
                    length = (int)fi.Length;
                    //ResourceFile res = new ResourceFile("Audio", "Chiens - Louane.mp3", 2943507, 2943507, 0, false, true);
                    res = new ResourceFile("Audio", fi.Name, length, length, Offset, false, true);
                    resources.Add(res);
                    Offset += length;
                }
            }

            // Images files
            for (int i = 0; i < lstImages.Count; i++)
            {
                if (File.Exists(lstImages[i]))
                {
                    fi = new FileInfo(lstImages[i]);
                    length = (int)fi.Length;
                    res = new ResourceFile("Image", fi.Name, length, length, Offset, false, false);
                    resources.Add(res);
                    Offset += length;
                }
            }
        }

        private void AddSongIniToResources(string content)
        {
            int length;

            // Song.ini
            length = content.Length;
            ResourceFile res = new ResourceFile("Config", "Song.ini", length, length, Offset, false, false);
            resources.Add(res);

        }


        /// <summary>
        /// Convert an LRC file content to lyrics compatible with Song.ini
        /// </summary>
        private (List<int>,List<string>) LrcToIni(string fpath)
        {
            List<List<Syllable>> Lyrics =  LyricsUtilities.ReadLrcFromFile(fpath);

            List<int> AllSyncs = new List<int>();
            List<string> EffTexts = new List<string>();

            string element = string.Empty;
            bool isSlash = false;
            int time;

            foreach (List<Syllable> Line in Lyrics)
            {
                isSlash = false;
                element = string.Empty;
                
                foreach (Syllable sy in Line)
                {
                    if (isSlash)
                        element += "/" + sy.Text;
                    else
                        element += sy.Text;
                    
                    if (sy.Text.EndsWith(" "))
                        isSlash = false;
                    else
                        isSlash = true;
                    
                    time = sy.Time;
                    AllSyncs.Add(time);  
                }
                EffTexts.Add(element);
            }

            // Convert time in ms to KFN time format in sec + ms on 2 digits
            // 7570 ms = 
            List<int> EffSyncs = new List<int>();
            int t;            
            int rest = 0;
            string strRest;
            string strTime = string.Empty;
            int sec = 0;
            
            for (int i = 0; i < AllSyncs.Count; i++)
            {
                t = AllSyncs[i];
                sec = t / 1000;
                rest = t % 1000; // 7570 => 570 ms                
                strRest = string.Format("{0:00}", rest / 10);
                strTime = sec + strRest;
                
                EffSyncs.Add((Int32.Parse(strTime)));
            }        

            return (EffSyncs, EffTexts);

        }


    }
}


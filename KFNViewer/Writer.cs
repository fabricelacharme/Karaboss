using KFNViewer.Properties;
using KFNViewer.SongIni;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using TagLib;
using static KFN;

namespace KFNViewer
{
    public class KfnWriter
    {
        public string fullFileName {  get; set; }
        public string mp3FileName {  get; set; }
        public string iniFileName { get; set; }
        public List<string> imageFileNameLst { get; set; }

        private List<ResourceFile> resources = new List<ResourceFile>();

        public List<ResourceFile> Resources
        {
            get { return this.resources; }
        }

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

        public KfnWriter(string fpath, string mp3, string ini, List<string> imgLst)
        {
            fullFileName = fpath;
            mp3FileName = mp3;
            iniFileName = ini;
            imageFileNameLst = imgLst;
            
            
            FileInfo fi;
            int Offset = 0;
            int length = 0;

            // Add list of resources
            resources = new List<ResourceFile>();
            ResourceFile res;

            // MP3
            fi = new FileInfo(mp3FileName);                        
            length = (int)fi.Length;
            //ResourceFile res = new ResourceFile("Audio", "Chiens - Louane.mp3", 2943507, 2943507, 0, false, true);
            res = new ResourceFile("Audio", fi.Name, length, length, Offset, false, true);            
            resources.Add(res);
            Offset += length;

            // Images
            for (int i = 0; i < imgLst.Count; i++)
            {
                fi = new FileInfo(imgLst[i]);
                length = (int)fi.Length;
                res = new ResourceFile("Image", fi.Name, length, length, Offset, false, false);
                resources.Add(res);
                Offset += length;
            }


            // Song.ini
            fi = new FileInfo(iniFileName);  
            length = (int)fi.Length;
            //res = new ResourceFile("Config", "Song.ini", 5654, 5654, 2943507, false, false);
            res = new ResourceFile("Config", fi.Name, length, length, Offset, false, false);
            resources.Add(res);
            Offset += length;




        }
                        
        /// <summary>
        /// Create a new KFN file
        /// </summary>
        public void CreateKFN(string mp3file, string Title, string Comment)
        {

            CreateIniFile(mp3file, Title, Comment);
            return;

            try
            {
                using (FileStream fs = new FileStream(fullFileName, FileMode.Create, FileAccess.ReadWrite))
                {
                    // Write properties
                    WriteProperties(fs, Title, Comment, mp3file);

                    // Write resources
                    WriteResources(fs);


                    // Write mp3
                    byte[] fileBytes = System.IO.File.ReadAllBytes(mp3FileName);
                    fs.Write(fileBytes, 0, fileBytes.Length);

                    // Write Song.ini
                    fileBytes = System.IO.File.ReadAllBytes(iniFileName);
                    fs.Write(fileBytes, 0, fileBytes.Length);
                }

                string tx = string.Format("File {0} \nwas created in the directory\n {1}", Path.GetFileName(fullFileName), Path.GetDirectoryName(fullFileName));
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
        private void WriteProperties(FileStream fs, string sTitle, string sComment, string mp3file)
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
            //WriteProp(fs, new byte[] { 82, 71, 72, 84, 1 }, new byte[] { 0, 0, 0, 0 });

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
        /// Write resources
        /// </summary>
        /// <param name="fs"></param>
        private void WriteResources(FileStream fs)
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
        /// Create Song.ini file
        /// </summary>
        /// <param name="fs"></param>
        private void CreateIniFile(string Source, string Title, string Comment)
        {
            

            KfnIni kfnIni = new KfnIni();
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
            kfnHeader.Comment = Comment;   
            kfnHeader.SourceFile = Source;
            
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
            
            // Audio files
            foreach (KFN.ResourceFile resource in Resources)
            {
                if (resource.FileType == "Audio")
                {
                    Entry entry = new Entry();
                    entry.FileName = resource.FileName;
                    entry.Length1 = resource.EncLength;
                    entry.Length2 = resource.FileLength;
                    entry.Offset = Offset;

                    entries.Add(entry);
                    Offset += Offset;
                }
            }
            // Images
            foreach (KFN.ResourceFile resource in Resources)
            {
                if (resource.FileType == "Image")
                {
                    Entry entry = new Entry();
                    entry.FileName = resource.FileName;
                    entry.Length1 = resource.EncLength;
                    entry.Length2 = resource.FileLength;
                    entry.Offset = Offset;

                    entries.Add(entry);
                    Offset += Offset;
                }
            }
            kfnIni.SetMaterials(entries);

            #endregion Materials


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
                51,                         // first is always 51
                effnum,                          // num
                new List<Anim>(),           // anims
                null, 
                null,
                ("", 0),                    // font
                null, 
                null,
                new List<int>(),           // syncs
                new List<TextEntry>(),     // Texts
                Trajectory.Default());
            
            kfnIni.Effs.Add(eff);

            #endregion eff1


            #region eff2
            List<TextEntry> textLst = new List<TextEntry>();
            List<(int, string)> fragments = new List<(int, string)>();
            TextEntry text;
            effnum = 2;
            
            List<int> syncsLst = new List<int>() { 757, 820, 898, 938, 985, 1040, 1097, 1120, 1149, 1172, 1218, 1310, 1336, 1359, 1387, 1433, 1485, 1504, 1529, 1551, 1602, 1641, 1680, 1700, 1724, 1768, 1788, 1817, 1866, 1888, 1911, 1982, 2026, 2059, 2082, 2106, 2328, 2375, 2497, 2519, 2548, 2590, 2642, 2662 };
            //fragments = new List<(int, string)>() { (1, "TU"), (2, " M'AS"), (3, " TROP"), (4, " SOU"), (5, "VENT"), (6, " DIT") };                       
            text = new TextEntry("TU M'AS TROP SOU/VENT DIT", fragments, effnum);
            textLst.Add(text);

            //fragments = new List<(int, string)>() { (1, "QUE"), (2, " T'AI"), (3, "MAIS"), (4, " MES"), (5, "YEUX") };
            text = new TextEntry("QUE T'AI/MAIS MES YEUX,", fragments, effnum);
            textLst.Add(text);

            text = new TextEntry("QUE T'AI/MAIS NOS NUITS,", fragments, effnum);
            textLst.Add(text);

            text = new TextEntry("MAIN/T'NANT,", fragments, effnum);
            textLst.Add(text);

            text = new TextEntry("J'AI L'AIR D'UNE FOLLE,", fragments, effnum);
            textLst.Add(text);

            text = new TextEntry("LE MAS/CA/RA DE/COLLE,", fragments, effnum);
            textLst.Add(text);

            text = new TextEntry("TU M'Y AS FAIT CROIRE,", fragments, effnum);
            textLst.Add(text);

            text = new TextEntry("MAIS T'ES QU'UN", fragments, effnum);
            textLst.Add(text);

            text = new TextEntry("TU M'AS CAS/SEE EN DEUX", fragments, effnum);
            textLst.Add(text);

            text = new TextEntry("ET MOI, J'AI AP/PRIS", fragments, effnum);
            textLst.Add(text);

            eff = new Eff(
                1,  // second is 1
                effnum,  // num
                new List<Anim>(),           // anims
                null,
                null,
                ("", 0),                    // font
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

    }       
}


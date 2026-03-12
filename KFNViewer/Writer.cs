using KFNViewer.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
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
        }
                        
        /// <summary>
        /// Create a new KFN file
        /// </summary>
        public void CreateKFN()
        {
            byte[] data;
            int nbchars;
            byte[] bnbchars;

            using (FileStream fs = new FileStream(fullFileName, FileMode.Create, FileAccess.ReadWrite))
            {
                // Write properties
                WriteProperties(fs);

                // Write number of resources : 2
                int resourcesCount = 2;
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
                    nbchars = resourceEncLength.Length;
                    bnbchars = BitConverter.GetBytes(nbchars);
                    fs.Write(bnbchars, 0, bnbchars.Length);
                    fs.Write(resourceEncLength, 0, resourceEncLength.Length);                    
                }


                // Write mp3

                // Write Song.ini
            }
        }

        private void WriteProperties(FileStream fs)
        {
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

            byte[] propValue;
            byte[] data;

            // [0] Signature (4 bytes [75,70,78,66])
            string signature = "KFNB";
            byte[] bsignature = Encoding.GetEncoding(this.resourceNamesEncodingAuto).GetBytes(signature);
            fs.Write(bsignature, 0, bsignature.Length);

            // [1] Man Difficult = "DIFM";
            WriteProp(fs, new byte[] { 68, 73, 70, 77, 1 }, new byte[] { 1, 0, 0, 0 });

            // [2]: Woman difficult = "DIFW"
            WriteProp(fs, new byte[] { 68, 73, 70, 87, 1 }, new byte[] { 1, 0, 0, 0 });

            // [3]: ("", "255,255,255,255", "GNRE", "Genre")
            WriteProp(fs, new byte[] { 71, 78, 82, 69, 1 }, new byte[] { 1, 0, 0, 0 });

            // [4]: ("", "", "SFTV", "SFTV")
            WriteProp(fs, new byte[] { 83, 70, 84, 86, 1 }, new byte[] { 21, 90, 20, 1 });

            // [5]: ("", "", "MUSL", "MUSL")
            WriteProp(fs, new byte[] { 77, 85, 83, 76, 1 }, new byte[] { 184, 0, 0, 0 });

            //[6]: ("", "", "ANME", "ANME")
            WriteProp(fs, new byte[] { 65, 78, 77, 69, 1 }, new byte[] { 13, 0, 0, 0 });

            //[7]: ("", "", "TYPE", "TYPE")
            WriteProp(fs, new byte[] { 84, 89, 80, 69, 1 }, new byte[] { 0, 0, 0, 0 });

            //[8]: ("", "", "FLID", "AES-ECB-128 Key")
            WriteProp(fs, new byte[] { 70, 76, 73, 68, 2 }, new byte[] { 16, 0, 0, 0 });
            // Ecrire
            propValue = new byte[] { 16, 0, 0, 0 };
            data = new byte[BitConverter.ToUInt32(propValue, 0)];
            fs.Write(data, 0, data.Length);

            //[9]: ("", "", "LANG", "Language")
            WriteProp(fs, new byte[] { 76, 65, 78, 71, 2 }, new byte[] { 2, 0, 0, 0 });
            // Ecrire
            propValue = new byte[] { 2, 0, 0, 0 };
            data = new byte[BitConverter.ToUInt32(propValue, 0)];
            fs.Write(data, 0, data.Length);

            //[10]: ("", "", "TITL", "Title")
            WriteProp(fs, new byte[] { 84, 73, 84, 76, 2 }, new byte[] { 15, 0, 0, 0 });
            // Ecrire
            propValue = new byte[] { 15, 0, 0, 0 };
            data = new byte[BitConverter.ToUInt32(propValue, 0)];
            fs.Write(data, 0, data.Length);

            //[11]: ("", "", "SORC", "Source")
            WriteProp(fs, new byte[] { 83, 79, 82, 67, 2 }, new byte[] { 23, 0, 0, 0 });
            // Ecrire
            propValue = new byte[] { 23, 0, 0, 0 };
            data = new byte[BitConverter.ToUInt32(propValue, 0)];
            fs.Write(data, 0, data.Length);


            //[12]: ("", "", "COMM", "Comment")
            WriteProp(fs, new byte[] { 67, 79, 77, 77, 2 }, new byte[] { 64, 0, 0, 0 });
            // Ecrire
            propValue = new byte[] { 64, 0, 0, 0 };
            data = new byte[BitConverter.ToUInt32(propValue, 0)];
            fs.Write(data, 0, data.Length);


            //[13]: ("", "", "RGHT", "RGHT")
            WriteProp(fs, new byte[] { 82, 71, 72, 84, 1 }, new byte[] { 0, 0, 0, 0 });

            //[14]: (",1", "0,0,0,0", "PROV", "PROV")
            WriteProp(fs, new byte[] { 80, 82, 79, 86, 1 }, new byte[] { 10, 0, 0, 0 });

            //[15]: ("", "", "IDUS", "IDUS")
            WriteProp(fs, new byte[] { 73, 68, 85, 83, 2 }, new byte[] { 16, 0, 0, 0 });
            // Ecrire
            propValue = new byte[] { 16, 0, 0, 0 };
            data = new byte[BitConverter.ToUInt32(propValue, 0)];
            fs.Write(data, 0, data.Length);


            //[16]: ("69,78,68,72,1", "", "ENDH", "End of properties")
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

    }
       
}


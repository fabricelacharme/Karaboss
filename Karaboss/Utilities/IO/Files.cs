using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Karaboss.Utilities
{
    public static class Files
    {
       /// <summary>
       /// Find a unique name for a file by adding (2), (3) etc... to its name
       /// </summary>
       /// <param name="fPath"></param>
       /// <returns></returns>
        public static string FindUniqueFileName(string fPath)
        {
            if (File.Exists(fPath) == true)
            {
                string newPath;                
                string defExt = Path.GetExtension(fPath);                   // Extension 
                string fName = Path.GetFileNameWithoutExtension(fPath);     // File name

                // Remove all (1) (2) etc.. in fName
                string pattern = @"[(\d)]";
                string replace = @"";
                string inifName = Regex.Replace(fName, pattern, replace).Trim();

                // Add (2), (3) etc..
                int i = 2;
                string addName = " (" + i.ToString() + ")";
                string newName = inifName + addName + defExt;
                newPath = fPath + "\\" + newName;

                while (File.Exists(newPath) == true)
                {
                    i++;
                    newName = inifName + " (" + i.ToString() + ")" + defExt;
                    newPath = fPath + "\\" + newName;
                }

                return newPath;
            }

            return fPath;
        }


        #region Unzip

        /// <summary>
        /// Unzip file in temp directory
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static string UnzipFile(string f)
        {
            string mTempDir = Path.GetTempPath() + "karaboss\\";
            string myTempDir = mTempDir + Path.GetRandomFileName();
            Directory.CreateDirectory(myTempDir);

            List<string> lsextensions = new List<string> { "*.musicxml", "*.xml" };
            return UnzipFiles(f, lsextensions, myTempDir);
        }
        

        /// <summary>
        /// Extract any file with extension 'extension'
        /// For egg : '*.musicxml', '*.mxl'
        /// </summary>
        /// <param name="zipFilename"></param>
        /// <param name="extension"></param>
        /// <param name="outputPath"></param>
        /// <returns></returns>
        private static string UnzipFiles(string zipFilename, List<string> lsextension, string outputPath)
        {
            string UnzipFiles = "";
            try
            {
                ICSharpCode.SharpZipLib.Zip.FastZip myZip = new ICSharpCode.SharpZipLib.Zip.FastZip();
                myZip.ExtractZip(zipFilename, outputPath, "");
                DirectoryInfo myDirInfo = new DirectoryInfo(outputPath);

                foreach (string extension in lsextension)
                {                    
                    FileInfo[] myFileInfo = myDirInfo.GetFiles(extension, SearchOption.TopDirectoryOnly);
                    if (myFileInfo.Length > 0)
                    {
                        UnzipFiles = myFileInfo[0].FullName;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            return UnzipFiles;
        }

        #endregion Unzip

    }
}

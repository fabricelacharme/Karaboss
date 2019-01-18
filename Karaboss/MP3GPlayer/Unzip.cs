using System;
using System.IO;

namespace Karaboss
{
    class Unzip
    {
        public static string UnzipMP3GFiles(string zipFilename, string outputPath) {
            string UnzipMP3GFiles = string.Empty;
            try {
                ICSharpCode.SharpZipLib.Zip.FastZip myZip = new ICSharpCode.SharpZipLib.Zip.FastZip();
                myZip.ExtractZip(zipFilename, outputPath, "");
                DirectoryInfo myDirInfo = new DirectoryInfo(outputPath);
                FileInfo[] myFileInfo = myDirInfo.GetFiles("*.cdg", SearchOption.AllDirectories);
                if (myFileInfo.Length > 0) {
                    UnzipMP3GFiles = myFileInfo[0].FullName;
                }
            } catch (Exception ex) {
                Console.Write(ex.Message);
            }
            return UnzipMP3GFiles;
        }
    }
}

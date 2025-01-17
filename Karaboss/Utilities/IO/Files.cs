using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

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

    }
}

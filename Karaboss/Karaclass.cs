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
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using System.ComponentModel; // DLL import

namespace Karaboss
{

    public class Karaclass
    {
        public static string m_textEncoding;    // Song text encoding: Ascii, Chinese, Korean
        public static string m_lang;            // Spoken language of programm: French or English 
        public static bool m_MuteMelody;        // Mute Melody
        public static bool m_DisplayBalls;      // Display Balls on melody lyrics
        public static bool m_PauseBetweenSongs; // Pause or not between songs of a playlist
        public static int m_CountdownSongs;     // Countdown before launching a song of a playlist
        public static int m_TransposeAmount;    // transpose amount
        public static int m_Velocity;           // Velocity of new notes

        public static string m_SepSyllabe;
        public static string m_SepLine;
        public static string m_SepParagraph;
        public static bool m_ShowParagraph;     // Lyrics : Display a blanck line between paragraphs
        public static bool m_ForceUppercase;    // Lyrics : converts every character to uppercase

        public static bool m_SaveDefaultOutputDevice;   // Save default MIDI output device

        public enum OptionsDisplay
        {
            Top = 0,
            Center = 1,
            Bottom = 2,
        }

        private static string _m_fileplaylistGroups;
        public static string M_filePlaylistGroups
        {
            //Playlists file & path
            get
            {
                return _m_fileplaylistGroups;
            }
            set
            {
                _m_fileplaylistGroups = value;
                Properties.Settings.Default.filePlaylistGroups = _m_fileplaylistGroups;
                Properties.Settings.Default.Save();
            }
        }


        public static string m_drivePlaylists; // usual drive Playlists

        // http://www.binary-universe.net/index.php?article=5&language=e

        /// <summary>
        /// Read in the properties the last saved initial directory of the applciation
        /// This will be displayed by the explorer
        /// </summary>
        /// <returns></returns>
        public static string GetStartDirectory()
        {
            string inipath = string.Empty;
            inipath = Properties.Settings.Default.StartDirectory;

            if (inipath == null || inipath == "" || inipath == "C:\\\\")
            {
                inipath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                //path must be like "file:///c:/users/a453868/Music/karaoke/sasin";
                inipath = "file:///" + inipath.Replace("\\", "/");
            }


            return inipath;
        }


        /// <summary>
        /// Save starting path of application (ie last directory visited)
        /// </summary>
        /// <param name="inipath"></param>
        public static void SaveStartDirectory(string inipath)
        {
            Properties.Settings.Default.StartDirectory = inipath;
        }

        /// <summary>
        /// Return path of XML file containing playlists
        /// </summary>
        /// <param name="defFileName"></param>
        /// <returns></returns>
        public static string GetPlaylistGroupFile(string defFileName)
        {
            string fileName = string.Empty;

            if (_m_fileplaylistGroups != null && _m_fileplaylistGroups != "")
            {
                string newfileName = _m_fileplaylistGroups;
                string drivePlaylists = Directory.GetDirectoryRoot(newfileName);         // drive of playlists file

                DriveInfo drvinfo = new DriveInfo(newfileName);

                // If current drive is not fixed, update drive   
                if (drvinfo.DriveType != DriveType.Fixed)
                {
                    string MyPath = @newfileName;
                    string MyPathWithoutDriveOrNetworkShare = MyPath.Substring(Path.GetPathRoot(MyPath).Length);

                    // Replace drive on playlists file path
                    newfileName = drivePlaylists + MyPathWithoutDriveOrNetworkShare;
                    _m_fileplaylistGroups = newfileName;
                }

                if (File.Exists(_m_fileplaylistGroups) == true)
                {
                    fileName = _m_fileplaylistGroups;
                }
                else
                {                    
                    string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);
                    fileName = folder + "\\" + defFileName;
                    _m_fileplaylistGroups = fileName;
                }
            }
            else
            {
                string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);
                fileName = folder + "\\" + defFileName;
                _m_fileplaylistGroups = fileName;
            }

            M_filePlaylistGroups = _m_fileplaylistGroups;
            return fileName;
        }


        public static bool IsMidiExtension(string f)
        {
            if (f == null || f == "")
                return false;

            string[] exts = new string[] { ".mid", ".kar", ".midi" };

            f = f.ToLower();
            foreach (string ext in exts)
            {

                if (f.EndsWith(ext, StringComparison.Ordinal))
                    return true;
            }
            return false;
        }

        public static bool IsXmlExtension(string f)
        {
            if (f == null || f == "")
                return false;

            string[] exts = new string[] { ".xml", ".musicxml" };

            f = f.ToLower();
            foreach (string ext in exts)
            {

                if (f.EndsWith(ext, StringComparison.Ordinal))
                    return true;
            }
            return false;
        }

        public static bool IsTxtExtension(string f)
        {
            if (f == null || f == "")
                return false;

            string[] exts = new string[] { ".txt" };

            f = f.ToLower();
            foreach (string ext in exts)
            {

                if (f.EndsWith(ext, StringComparison.Ordinal))
                    return true;
            }
            return false;
        }


        public static string plTypeToString(plLyric.Types plType)
        {
            switch (plType)
            {
                case plLyric.Types.Text:
                    return "text";
                case plLyric.Types.LineFeed:
                    return "cr";
                case plLyric.Types.Paragraph:
                    return "par";
                default:
                    return "text";
            }
        }

       
    }

    /// <summary>
    /// A class to store some properties of the lyrics
    /// </summary>
    public class CLyric
    {
        public enum LyricTypes
        {
            None = -1,
            Text = 0,
            Lyric = 1
        }

        public int LyricsTrackNum = -1;     // num of track containing lyrics
        public int MelodyTrackNum = -1;     // num  of track containing the melody       
        public LyricTypes lyrictype;            // type lyric or text                 

        public CLyric()
        {
            LyricsTrackNum = -1;
            MelodyTrackNum = -1;
            lyrictype = LyricTypes.None;
        }

    }

    /// <summary>
    /// A class to store all lyric's syllabes
    /// </summary>
    public class plLyric
    {
        public enum Types
        {
            Text = 1,
            LineFeed = 2,
            Paragraph = 3,
        }
        public Types Type { get; set; }
        public string Element { get; set; }
        public int TicksOn { get; set; }
        public int TicksOff { get; set; }
        public int Beat { get; set; }        
    }

    /// <summary>
    /// Localization
    /// </summary>
    public static class RuntimeLocalizer
    {
        
        public static void ChangeCulture(string cultureCode)
        {
            CultureInfo culture = CultureInfo.GetCultureInfo(cultureCode);

            //Set the language in the application
            Thread.CurrentThread.CurrentUICulture = culture;

            // The CultureInfo.DefaultThreadCurrentCulture property to change the culture of an AppDomain.  
            // Allow to manage languages for usercontrols
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }

        /// <summary>
        /// Change lang
        /// </summary>
        /// <param name="frm"></param>
        /// <param name="cultureCode"></param>
        public static void ChangeCultureForm(Form frm, string cultureCode)
        {
            CultureInfo culture = CultureInfo.GetCultureInfo(cultureCode);

            
                      
            ComponentResourceManager resources = new ComponentResourceManager(frm.GetType());

            ApplyResourceToControl(resources, frm, culture);

            resources.ApplyResources(frm, "$this", culture);
        }

        /// <summary>
        /// Change Text property of controls
        /// </summary>
        /// <param name="resources"></param>
        /// <param name="control"></param>
        /// <param name="lang"></param>
        private static void ApplyResourceToControl(ComponentResourceManager resources, Control control, CultureInfo lang)
        {
            string text = string.Empty;

            if (control.GetType() == typeof(MenuStrip))  // See if this is a menuStrip
            {
                MenuStrip strip = (MenuStrip)control;
                ApplyResourceToToolStripItemCollection(strip.Items, resources, lang);
            } 
                   
            foreach (Control c in control.Controls) // Apply to all sub-controls
            {               
                ApplyResourceToControl(resources, c, lang);
                resources.ApplyResources(c, c.Name, lang);               
            }            
            // Apply to self
            resources.ApplyResources(control, control.Name, lang);           
        }

        /// <summary>
        /// Change Text property of menus
        /// </summary>
        /// <param name="col"></param>
        /// <param name="resources"></param>
        /// <param name="lang"></param>
        private static void ApplyResourceToToolStripItemCollection(ToolStripItemCollection col, ComponentResourceManager resources, CultureInfo lang)
        {
            for (int i = 0; i < col.Count; i++)     // Apply to all sub items
            {
                if (col[i].GetType().ToString() != "System.Windows.Forms.ToolStripSeparator") // Erreur sur separator
                {
                    ToolStripItem item = (ToolStripMenuItem)col[i];

                    if (item.GetType() == typeof(ToolStripMenuItem))
                    {
                        ToolStripMenuItem menuitem = (ToolStripMenuItem)item;
                        ApplyResourceToToolStripItemCollection(menuitem.DropDownItems, resources, lang);
                    }                    
                    resources.ApplyResources(item, item.Name, lang);
                }
            }
        }            

    }

   

    public static class Extensions
    {
        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hWnd, uint Msg);

        private const uint SW_RESTORE = 0x09;

        public static void Restore(this Form form)
        {
            if (form.WindowState == FormWindowState.Minimized)
            {
                ShowWindow(form.Handle, SW_RESTORE);
            }
        }  
    }
    
    /// <summary>
    /// Button not selectable
    /// </summary>
    public class NoSelectButton : Button
    {
        public NoSelectButton()
        {
            SetStyle(ControlStyles.Selectable, false);
        }
    }

    /// <summary>
    /// Scrollbar not selectable
    /// </summary>
    public class NoSelectVScrollBar : VScrollBar
    {
        public NoSelectVScrollBar()
        {
            SetStyle(ControlStyles.Selectable, false);
        }
    }


    class DisplayMidiInfos
    {
        public int Tracks = 0;
        public int Format = 1;
        public string Duration = "00:00";
        public bool Lyrics = false;
        public string ITags = "";
        public string KTags = "";
        public string LTags = "";
        public string TTags = "";
        public string VTags = "";
        public string WTags = "";
        public bool busy = false;
        
        public void Clear()
        {
            Tracks = 0;
            Format = 1;
            Duration = "00:00";
            Lyrics = false;
            ITags = "";
            KTags = "";
            LTags = "";
            TTags = "";
            VTags = "";
            WTags = "";       
        }
    }

    public class DrawingControl
    {
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);

        private const int WM_SETREDRAW = 11;

        public static void SuspendDrawing(Control parent)
        {
            SendMessage(parent.Handle, WM_SETREDRAW, false, 0);
        }

        public static void ResumeDrawing(Control parent)
        {
            SendMessage(parent.Handle, WM_SETREDRAW, true, 0);
            parent.Refresh();
        }
    }


}

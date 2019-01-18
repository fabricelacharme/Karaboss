using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Karaboss
{
    public class PlaylistGroupsHelper
    {
        #region properties
        /// <summary>
        /// Usual file
        /// </summary>
        private string file = string.Empty;
        public string File
        {
            get
            {
                if (file == "")
                    file = "playlistgroups.xml";
                return file;
            }
            set
            {
                file = value;
            }
        }


        // Usual drive
        private string drive = string.Empty;
        public string Drive
        {
            get
            {
                if (drive == "")
                    drive = @"C:\";
                return drive;
            }
            set
            {
                drive = value;
            }
        }

        #endregion

        /// <summary>
        /// Load collection of groups of playlists
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public PlaylistGroup Load(string fileName)
        {
            PlaylistGroup plg;
            // Open file containing all playlistGroups "playlistGroup.xml"
            try
            {
                using (var fs = new FileStream(fileName, FileMode.OpenOrCreate))
                {
                    XmlSerializer xml = new XmlSerializer(typeof(PlaylistGroup));
                    if (fs.Length > 0)
                        plg = (PlaylistGroup)xml.Deserialize(fs);
                    else
                        plg = new PlaylistGroup();
                }
                return plg;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while loading playlist file:\n" + ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return new PlaylistGroup();
                
            }
        }

        /// <summary>
        /// Save all groups of playlists in a xml file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="plg"></param>
        public void Save(string fileName, PlaylistGroup plg)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(PlaylistGroup));
                TextWriter textWriter = new StreamWriter(@fileName);
                serializer.Serialize(textWriter, plg);
                textWriter.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while saving playlist file:\n" + ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        public PlaylistGroupItem CreateEmptyPlaylistGroupItem(string Name = "myPlaylists1", PlaylistGroupItem Parent = null)
        {
            PlaylistGroupItem plgi = new PlaylistGroupItem();
            plgi.Name = Name;
            plgi.Key = generateKey();

            if (Parent != null)
                plgi.ParentKey = Parent.Key;
            
            plgi.Playlists = new ObservableCollection<Playlist>();

            Playlist pl = new Playlist();
            pl.Name = "Playlist1";
            pl.Style = "Style";
            pl.Add("<Artist>", "<Song>", "", "<Album>", "00:00", 4, "<DirSlideShow>", false, "<Karaoke singer>");
            plgi.Playlists.Add(pl);
            return plgi;
        }

        public PlaylistGroupItem Find(PlaylistGroup plg , string Key)
        {
            foreach (PlaylistGroupItem plgi in plg.plGroupItems)
            {
                if (plgi.Key == Key)
                    return plgi;
            }
            return null;
        }
      
        /// <summary>
        /// Search recursively the most deeper child key
        /// </summary>
        /// <param name="plg"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public string SearchLastChildKey(PlaylistGroup plg, string Key)
        {
            foreach (PlaylistGroupItem plgi in plg.plGroupItems)
            {
                if (plgi.ParentKey == Key)
                {
                    // plgi is a child, 
                    string newkey = plgi.Key;

                    // Search childs of plgi
                    string nextkey = SearchLastChildKey(plg, newkey);
                    if (nextkey != null)
                        return nextkey;
                    else
                        return newkey;
                }
            }
            return null;
        }

        public Playlist getPlaylistByName(PlaylistGroup plg, string plName, string Key)
        {
            PlaylistGroupItem plgi = this.Find(plg, Key);
            if (plgi != null)
            {
                Playlist pltarget = plgi.Playlists.Where(z => z.Name == plName).FirstOrDefault();
                return pltarget == null ? null : pltarget;
            }

            return null;
        }

        public bool PlaylistExist(ObservableCollection<Playlist> pls, string name)
        {
            name = name.ToLower();
            Playlist pltarget = pls.Where(z => z.Name.ToLower() == name).FirstOrDefault();
            return pltarget == null ? false : true;
        }


        public string generateKey()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}

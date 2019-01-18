using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace Karaboss
{
    /*
     * Un élément groupe 
     * Nom
     * Parent 
     * Liste de playlists
     */

    [DataContract]
    public class PlaylistGroupItem
    {
        [DataMember]
        public string Name { get; set; }                        // Folder        

        [DataMember]
        public string Key { get; set; }                          // Unique key
       
        [DataMember]
        public string ParentKey { get; set; }           // Key of Folder parent

        private ObservableCollection<Playlist> _playlists;

        [DataMember]
        public ObservableCollection<Playlist> Playlists           // Collection of playlists
        {
            get
            {
                if (_playlists == null)
                {
                    _playlists = new ObservableCollection<Playlist>();
                }
                return _playlists;
            }
            set
            {
                if (_playlists != value)
                {
                    _playlists = value;
                }
            }
        }       



        public void Sort()
        {
            Playlist[] arrayMenu = new Playlist[_playlists.Count];

            for (int i = 0; i < _playlists.Count; i++)
            {
                Playlist pli = _playlists[i];
                arrayMenu[i] = pli;
            }
            Array.Sort(arrayMenu, delegate (Playlist pli1, Playlist pli2)
            { return pli1.Name.CompareTo(pli2.Name); });

            // restore sorted collection
            _playlists.Clear();
            for (int i = 0; i < arrayMenu.Length; i++)
            {
                _playlists.Add(arrayMenu[i]);

            }
        }

    }
}

using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;
using System.Collections;

namespace Karaboss
{
    /*
    * PLAYLISTS MANAGEMENT
    * 
    * A PlaylistItem is a song 
    * A playlist is a collection of PlaylistItems
    * A PlaylistGroupItem is a folder containing a collection of playlists
    * A PlaylistGroup is a collection of PlaylistGroupItems    
    */

    //public class PlaylistGroup
    public class PlaylistGroup: IEnumerable    
     {                
        private ObservableCollection<PlaylistGroupItem> _plgroupitems;
        public ObservableCollection<PlaylistGroupItem> plGroupItems
        {
            get
            {
                if (_plgroupitems == null)
                {
                    _plgroupitems = new ObservableCollection<PlaylistGroupItem>();
                }
                return _plgroupitems;
            }
            set
            {
                if (_plgroupitems != value)
                {
                    _plgroupitems = value;
                }
            }
        }

        public int Count
        {
            get { return plGroupItems.Count; }
        }

        /// <summary>
        /// Add a PlaylistGroupItem to the PlaylistGroup
        /// </summary>
        /// <returns></returns>        
        public bool Add(PlaylistGroupItem pli)
        {           
            if (PlaylistGroupItemExists(pli, pli.Name) == false)
            {
                this.plGroupItems.Add(pli);                
                return true;
            }
            else
            {
                string tx = "<" + pli.Name + "> is already in this playlistgroup";
                MessageBox.Show(tx, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }            
        }

        /// <summary>
        /// Sort the PlaylistGroupItems of PlaylistGroup
        /// </summary>
        public void Sort()
        {            
            PlaylistGroupItem[] arrayMenu = new PlaylistGroupItem[_plgroupitems.Count];
            
            for (int i = 0; i < _plgroupitems.Count; i++)
            {
                PlaylistGroupItem plgi = _plgroupitems[i];
                arrayMenu[i] = plgi;
            }
            Array.Sort(arrayMenu, delegate(PlaylistGroupItem pli1, PlaylistGroupItem pli2)
            { return pli1.Name.CompareTo(pli2.Name); });

            // restore sorted collection
            _plgroupitems.Clear();
            for (int i = 0; i < arrayMenu.Length; i++)
            {
                _plgroupitems.Add(arrayMenu[i]);

            }
        }
       

        /// <summary>
        /// Search existence of PlaylistGroupItem by checking unicity of its key
        /// </summary>
        /// <param name="pli"></param>
        /// <returns></returns>
        public bool PlaylistGroupItemExists(PlaylistGroupItem pli, string name)
        {
            // Interdire même nom de folder si même parent
            PlaylistGroupItem pitarget;
            if (pli.ParentKey == null)
            {
                pitarget = plGroupItems.Where(z => (z.ParentKey == null) && (z.Name == name)).FirstOrDefault();
                return pitarget == null ? false : true;
            }
            else
            {
                pitarget = plGroupItems.Where(z => (z.ParentKey != null && z.ParentKey == pli.ParentKey) && (z.Name == name)).FirstOrDefault();
                return pitarget == null ? false : true;
            }
                                   
        }

        
        /// <summary>
        /// Remove a PlaylistGroupItem from the collection
        /// </summary>
        /// <param name="plg"></param>
        public void Remove(PlaylistGroupItem plg)
        {
            this._plgroupitems.Remove(plg);
        }
        

        /// <summary>
        /// next item
        /// </summary>
        /// <param name="curItem"></param>
        /// <returns></returns>
        public PlaylistGroupItem Next(PlaylistGroupItem curItem)
        {
            int itemIndex = plGroupItems.IndexOf(curItem);
            itemIndex++;

            if (itemIndex >= plGroupItems.Count)
                itemIndex = plGroupItems.Count - 1;

            return plGroupItems[itemIndex];
        }

        /// <summary>
        /// previous item
        /// </summary>
        /// <param name="curItem"></param>
        /// <returns></returns>
        public PlaylistGroupItem Previous(PlaylistGroupItem curItem)
        {
            int itemIndex = plGroupItems.IndexOf(curItem);
            itemIndex--;

            if (itemIndex < 0)
                itemIndex = 0;

            return plGroupItems[itemIndex];
        }

        /// <summary>
        /// Index of current item
        /// </summary>
        /// <param name="curItem"></param>
        /// <returns></returns>
        public int SelectedIndex(PlaylistGroupItem curItem)
        {
            return plGroupItems.IndexOf(curItem);
        }


        
        // Implementation for the GetEnumerator method.
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public PlaylistGroupEnum GetEnumerator()
        {
            return new PlaylistGroupEnum(_plgroupitems);
        }
        
    }


    public class PlaylistGroupEnum : IEnumerator
    {
        public ObservableCollection<PlaylistGroupItem> _plgroupitems;

        // Enumerators are positioned before the first element
        // until the first MoveNext() call.
        int position = -1;

        public PlaylistGroupEnum(ObservableCollection<PlaylistGroupItem> list)
        {
            _plgroupitems = list;
        }

        public bool MoveNext()
        {
            if (_plgroupitems == null)
                return true;

            position++;
            return (position < _plgroupitems.Count);
        }

        public void Reset()
        {
            position = -1;
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public PlaylistGroupItem Current
        {
            get
            {
                try
                {
                    return _plgroupitems[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }


    }
}

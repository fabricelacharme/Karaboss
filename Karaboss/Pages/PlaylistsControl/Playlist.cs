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
using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

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

    //public class Playlist : IEnumerable
    public class Playlist
    {
        public string Name { get; set; }
        public string Style { get; set; }


        public int Count
        {
            get { return _songs.Count; }
        }

        public string Duration
        {
            get {
                TimeSpan tsTotal = new TimeSpan();
                TimeSpan ts;

                foreach (PlaylistItem pli in _songs)
                {
                    try
                    {
                        ts = DateTime.ParseExact(pli.Length, "mm:ss", CultureInfo.InvariantCulture).TimeOfDay;
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.Message);
                        ts = DateTime.ParseExact("00:00", "mm:ss", CultureInfo.InvariantCulture).TimeOfDay; ;
                    }
                    //tsTotal.Add(ts);
                    tsTotal += ts;
                }
                return tsTotal.ToString();
            }
        }


        private ObservableCollection<PlaylistItem> _songs = new ObservableCollection<PlaylistItem>();
        public ObservableCollection<PlaylistItem> Songs
        {
            get
            {
                if (_songs == null)
                {
                    _songs = new ObservableCollection<PlaylistItem>();
                }
                return _songs;
            }
            set
            {
                if (_songs != value)
                {
                    _songs = value;
                }
            }
        }

        /// <summary>
        /// Add a song to a playlist
        /// </summary>
        /// <param name="Artist"></param>
        /// <param name="Song"></param>
        /// <param name="File"></param>
        /// <param name="Album"></param>
        /// <param name="Length"></param>
        /// <param name="Notation"></param>
        /// <param name="MelodyTrack"></param>
        /// <param name="MelodyMute"></param>
        /// <param name="KaraokeSinger"></param>
        public bool Add(string Artist, string Song, string File, string Album, string Length, int Notation, string DirSlideShow, bool MelodyMute, string KaraokeSinger)
        {            
            if (PlaylistItemExists(File) == false)
            {
                PlaylistItem pli = new PlaylistItem
                {
                    Artist = Artist,
                    Song = Song,
                    File = File,
                    Album = Album,
                    Length = Length,
                    Notation = Notation,
                    DirSlideShow = DirSlideShow,                    
                    MelodyMute = MelodyMute,
                    KaraokeSinger = KaraokeSinger
                };
                this._songs.Add(pli);
                return true;
            }
            else
            {
               string tx = "<" + Song + "> is already in this playlist";
               MessageBox.Show(tx, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               return false;
            }
        }

        /// <summary>
        /// Bool: item exists?
        /// </summary>
        /// <param name="song"></param>
        /// <returns></returns>
        public bool PlaylistItemExists(string file)
        {            
            PlaylistItem pitarget = _songs.Where(z => z.File == file).FirstOrDefault();
            return pitarget == null ? false : true;
        }


        public PlaylistItem getPlaylistItem(string file)
        {
            PlaylistItem pitarget = _songs.Where(z => z.File == file).FirstOrDefault();
            return pitarget;
        }
     
        /// <summary>
        /// next item
        /// </summary>
        /// <param name="curItem"></param>
        /// <returns></returns>
        public PlaylistItem Next(PlaylistItem curItem)
        {            
            int itemIndex = _songs.IndexOf(curItem);
            itemIndex++;
           
            if (itemIndex >= _songs.Count)
                itemIndex = _songs.Count -1;

            return _songs[itemIndex];
        }

        /// <summary>
        /// previous item
        /// </summary>
        /// <param name="curItem"></param>
        /// <returns></returns>
        public PlaylistItem Previous(PlaylistItem curItem)
        {
            int itemIndex = _songs.IndexOf(curItem);
            itemIndex--;
  
            if (itemIndex < 0)
                itemIndex = 0;

            return _songs[itemIndex];
        }

        /// <summary>
        /// Index of current item
        /// </summary>
        /// <param name="curItem"></param>
        /// <returns></returns>
        public int SelectedIndex(PlaylistItem curItem)
        {
            return _songs.IndexOf(curItem);          
        }

        /*
        // Implementation for the GetEnumerator method.
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public PlaylistEnum GetEnumerator()
        {
            return new PlaylistEnum(_songs);
        }
        */
    }

    public class PlaylistEnum : IEnumerator
    {
        public ObservableCollection<PlaylistItem> _songs;

        // Enumerators are positioned before the first element
        // until the first MoveNext() call.
        int position = -1;

        public PlaylistEnum(ObservableCollection<PlaylistItem> list)
        {
            _songs = list;
        }

        public bool MoveNext()
        {
            if (_songs == null)
                return true;

            position++;
            return (position < _songs.Count);
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

        public PlaylistItem Current
        {
            get
            {
                try
                {
                    return _songs[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }


    }

}

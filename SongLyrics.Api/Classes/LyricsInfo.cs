using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SongLyrics.Api.Classes
{
    class LyricsInfo
    {
        private List<Writer> _writers;
        public string Artist { get; private set; }
        public string Title { get; private set; }
        public List<Writer> Writers { get { return _writers; } }
        public string Text { get; private set; }

        public LyricsInfo(string artist, string title, string[] writers, string lyric)
        {
            Artist = artist;
            Title = title;
            Text = lyric;

            _writers = new List<Writer>();
            for (int i = 0; i < writers.Length; i++)
                _writers.Add(new Writer(writers[i]));
        }
    }
}

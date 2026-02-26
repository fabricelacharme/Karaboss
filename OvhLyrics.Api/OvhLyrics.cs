using OvhLyrics.Api.adapter;
using System;
using System.Net;
using System.Net.Http;
using System.Security.Policy;

namespace OvhLyrics.Api
{
    public class OvhLyrics
    {
        // AZLYRICS
        // https://www.azlyrics.com/lyrics/juliendore/danstesrves.html
        //private const string _url = "https://www.azlyrics.com/lyrics/";

        // SONGLYRICS
        // https://www.songlyrics.com/alain-bashung/gaby-oh-gaby-lyrics/
        // private const string _url = "https://www.songlyrics.com/";

        // OVHLYRICS
        // https://https://api.lyrics.ovh/v1/Miossec/Brest
        private const string _url = "https://api.lyrics.ovh/v1/";



        private int _error;
        public int Error { get { return _error; } }
        private Uri _uri;

        private string artist;
        private string title;
        private IWSConsumer consumer;

        public OvhLyrics(string artist, string title)
        {
            // https://https://api.lyrics.ovh/v1/Miossec/Brest
            artist = StringUtils.cleanArtist(artist);
            artist = StringUtils.cleanInput(artist);
            title = StringUtils.cleanInput(title);
            consumer = new WSConsumer();

            _uri = new Uri(_url + artist + "/" + title, UriKind.Absolute);
        }


        
        public string GetLyrics(string artist, string song)
        {
            try
            {
                string url = UserRequestToUrl(artist, song);
                string lyrics = consumer.GetLyricsFromJson(url);
                
                return lyrics;
            }
            catch (Exception ex)
            {

                _error++;
                Console.WriteLine("\nSongLyrics: " + ex.Message);
                return ex.Message;
            }

        }


        private string UserRequestToUrl(string artist, string song)
        {
            string url = "https://api.lyrics.ovh/v1/";
            url += artist.Replace(" ", "%20");
            url += "/";
            url += song.Replace(" ", "%20");
            return url;
        }




    }
}
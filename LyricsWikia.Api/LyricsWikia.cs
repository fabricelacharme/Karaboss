using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LyricsWikia.Api
{
    public class LyricsWikia
    {
        // AZLYRICS
        // https://www.azlyrics.com/lyrics/juliendore/danstesrves.html
        // private const string _url = "https://www.azlyrics.com/lyrics/";

        // SONGLYRICS
        // https://www.songlyrics.com/alain-bashung/gaby-oh-gaby-lyrics/
        // private const string _url = "https://www.songlyrics.com/";

        // LYRICS.WIKIA
        // https://lyrics.wikia.com/wiki/Alain_Souchon:C%27Est_Comme_Vous_Voulez
        // FAB - 03/11 - https instead of http
        //private const string _url = "https://lyrics.wikia.com/wiki/";
        private const string _url = "https://lyrics.fandom.com/wiki/";

        private int _error;
        public int Error { get { return _error; } }
        private Uri _uri;

        public LyricsWikia(string artist, string title)
        {
            // http://lyrics.wikia.com/wiki/Alain_Souchon:C%27Est_Comme_Vous_Voulez
            artist = StringUtils.cleanArtist(artist);
            artist = StringUtils.cleanInput(artist);

            //title = StringUtils.ToTitleCase(title);
            title = StringUtils.UppercaseWords(title);
            title = StringUtils.cleanInput(title);
            

            _uri = new Uri(_url + artist + ":" + title, UriKind.Absolute);
        }

        /// <summary>
        /// Get lyrics
        /// </summary>
        /// <returns></returns>
        public string GetLyrics()
        {
            string lyrics = string.Empty;
            using (var webClient = new LyricsWikiaWebClient())
            {
                webClient.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.3) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.114 Safari/537.36");
                webClient.Encoding = Encoding.UTF8;
                
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback((s, ce, ch, ssl) => true);

                try
                {
                   
                    var date = webClient.DownloadString(_uri);                   
                    //var date = Encoding.UTF8.GetString(webClient.DownloadData(_uri));
                    lyrics = ExtractLyricsFromHtml(date);
                }
                catch (WebException ex)
                {
                    Console.WriteLine(ex.Message);
                    _error++;
                }
            }
            return lyrics;
        }

        private string ExtractLyricsFromHtml(string htmlPage)
        {
            const string find1 = "<div class='lyricbox'>";
            const string find2 = "<div class='lyricsbreak'>";
            var idx = htmlPage.IndexOf(find1, StringComparison.Ordinal);
            if (idx > 0)
            {
                // Remove from start to "<!-- AddThis Button END -->" length
                htmlPage = htmlPage.Remove(0, idx + find1.Length).TrimStart();
                idx = htmlPage.IndexOf(find2, StringComparison.Ordinal);
                if (idx > 0)
                {
                    htmlPage = htmlPage.Remove(idx).TrimEnd();
                    htmlPage = WebUtility.HtmlDecode(htmlPage);
                }
            }
            return RemoveAllHtmlTags(htmlPage).Trim();
        }

        private string RemoveAllHtmlTags(string html)
        {
            var idx = html.IndexOf("<form id=\"addsong\"");
            if (idx > 20)
            {
                html = html.Substring(0, idx);
            }

            html = StringUtils.RemoveHtmlTags(html);

            // fix recursive white-spaces
            while (html.Contains("  "))
            {
                html = html.Replace("  ", " ");
            }

            // fix recursive line-break
            while (html.Contains("\r\n\r\n\r\n"))
                html = html.Replace("\r\n\r\n\r\n", "\r\n\r\n");
            return html;
        }

        private bool IsValidUri(string url)
        {
            return !((string.IsNullOrWhiteSpace(url)) && (!Uri.TryCreate(url, UriKind.Absolute, out _uri)));
        }
    }
}

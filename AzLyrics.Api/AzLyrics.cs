using System;
using System.Net;
using System.Text;

namespace AzLyrics.Api
{

    public class AzLyrics
    {
        // AZLYRICS
        // https://www.azlyrics.com/lyrics/juliendore/danstesrves.html
        private const string _url = "https://www.azlyrics.com/lyrics/";

        // SONGLYRICS
        // https://www.songlyrics.com/alain-bashung/gaby-oh-gaby-lyrics/
        // private const string _url = "https://www.songlyrics.com/";

        // LYRICS.WIKIA
        // https://lyrics.wikia.com/wiki/Alain_Souchon:C%27Est_Comme_Vous_Voulez
        // private const string _url = "https://lyrics.wikia.com/wiki/";

        private int _error;
        public int Error { get { return _error; } }
        private Uri _uri;

        public AzLyrics(string artist, string title)
        {
            // https://www.azlyrics.com/lyrics/youngthug/richniggashit.htm
            artist = StringUtils.cleanArtist(artist);
            artist = StringUtils.cleanInput(artist);
            title = StringUtils.cleanInput(title);

            _uri = new Uri(_url + artist + "/" + title + ".html", UriKind.Absolute);
        }

        /// <summary>
        /// Deprecated: Use GetLyrics() instead.
        /// </summary>
        /// <returns></returns>
        public string GetLyrics2()
        {
            string lyrics = string.Empty;
            using (var webClient = new AzLyricsWebClient())
            {
                webClient.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.3) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.114 Safari/537.36");
                webClient.Encoding = Encoding.UTF8;

                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.SecurityProtocol= SecurityProtocolType.SystemDefault;

                ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback((s, ce, ch, ssl) => true);

                try
                {
                    var data = webClient.DownloadString(_uri);
                    //var data = Encoding.UTF8.GetString(webClient.DownloadData(_uri));

                    

                    // Check if the page contains "Our systems have detected unusual activity"
                    if (data.Contains("Our systems have detected unusual activity"))
                    {
                        _error++;
                        Console.WriteLine("AZLyrics Error: Our systems have detected unusual activity");
                        return "Our systems have detected unusual activity from your computer network";
                    }

                    lyrics = ExtractLyricsFromHtml(data);
                }
                catch (WebException ex)
                {
                    _error++;
                    Console.WriteLine(ex.Message);
                }
            }
            return lyrics;
        }

        /// <summary>
        /// Retrieves the lyrics of a song from the specified URI.
        /// </summary>
        /// <remarks>This method fetches the HTML content of the page at the URI provided during
        /// initialization and extracts the lyrics from it. If the page indicates unusual activity or an error occurs
        /// during the request, an appropriate message is returned instead of the lyrics.</remarks>
        /// <returns>A string containing the lyrics of the song if successfully retrieved; otherwise, a message indicating an
        /// error or unusual activity.</returns>
        public string GetLyrics()
        {
            string lyrics = string.Empty;
            using (var webClient = new AzLyricsHttpClient())
            {
                webClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.3) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.114 Safari/537.36");
                webClient.Timeout = TimeSpan.FromSeconds(10);
                try
                {
                    var data = webClient.GetStringAsync(_uri).Result;
                    // Check if the page contains "Our systems have detected unusual activity"
                    if (data.Contains("Our systems have detected unusual activity"))
                    {
                        _error++;
                        Console.WriteLine("AZLyrics Error: Our systems have detected unusual activity");
                        return "Our systems have detected unusual activity from your computer network";
                    }
                    lyrics = ExtractLyricsFromHtml(data);
                }
                catch (Exception ex) when (ex is WebException || ex is AggregateException)
                {
                    _error++;
                    Console.WriteLine("\nAzLyrics: " + ex.Message);
                }
            }

            return lyrics;
        }


        private string ExtractLyricsFromHtml(string htmlPage)
        {
            //const string find1 = "<!-- AddThis Button END -->";
            const string find1 = "<!-- Usage of azlyrics.com content by any third-party lyrics provider is prohibited by our licensing agreement. Sorry about that. -->";
            //const string find1 = "<div class='lyricsh'>";
            
            const string find2 = "<!-- MxM banner -->";
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


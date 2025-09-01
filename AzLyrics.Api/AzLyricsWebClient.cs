using System.Net;
using System;
using System.Net.Http;

namespace AzLyrics.Api
{
    /// <summary>
    /// Deprecated: Use AzLyricsHttpClient instead.
    /// </summary>
    class AzLyricsWebClient : WebClient
    {
        // http://stackoverflow.com/questions/15034771/cant-download-utf-8-web-content
        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest req = base.GetWebRequest(address) as HttpWebRequest;

            // FAB 02/10/17: set proxy
            IWebProxy iwpxy = WebRequest.GetSystemWebProxy();
            req.Proxy = iwpxy;
            req.Proxy.Credentials = CredentialCache.DefaultCredentials;
            

            req.KeepAlive = false;
            if (req != null)
            {
                req.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            }
            return req;
        }
    }

    /// <summary>
    /// using httpClient is better than webClient
    /// </summary>
    public class AzLyricsHttpClient : HttpClient
    {
        public AzLyricsHttpClient() : base(new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            UseProxy = true,
            Proxy = WebRequest.GetSystemWebProxy(),
            UseDefaultCredentials = true
        })
        
        
        {
            // FAB 02/10/17: set proxy
            if (this.DefaultRequestHeaders.UserAgent.TryParseAdd("Mozilla/5.0 (Windows NT 6.3) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.114 Safari/537.36"))
            {
                // Successfully parsed user-agent string
            }
        }

    }

}

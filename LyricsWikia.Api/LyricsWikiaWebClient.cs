using System.Net;
using System;

namespace LyricsWikia.Api
{
    class LyricsWikiaWebClient : WebClient
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
}

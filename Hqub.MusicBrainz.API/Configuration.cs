using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Hqub.MusicBrainz.API
{
    public static class Configuration
    {
        static Configuration()
        {
            GenerateCommunicationThrow = true;
            Proxy = null;
            UserAgent = "Hqub.MusicBrainz/2.0";
        }

        /// <summary>
        /// If true, then all exceptions for http-requests to MusicBrainz (from class WebRequestHelper) will
        /// throw up. Otherwise they will be suppressed.
        /// </summary>
        public static bool GenerateCommunicationThrow { get; set; }


        /// <summary>
        /// Gets or sets a <see cref="System.Net.IWebProxy"/> used to query the webservice.
        /// </summary>        
        public static IWebProxy Proxy {
            get
            {
                // FAB 02/10/17: get system proxy 
                // Works like a charm at office
                // Todo check at home
                System.Net.IWebProxy iwpxy = WebRequest.GetSystemWebProxy();                
                iwpxy.Credentials = CredentialCache.DefaultCredentials;
                return iwpxy;                
            }
            set { }
        }
        
        // Old code
        // this was needing to set manually a proxy and credentials
        //public static IWebProxy Proxy { get; set; }


        /// <summary>
        /// Allow set cutstom user agent string.
        /// </summary>
        public static string UserAgent { get; set; }
    }
}

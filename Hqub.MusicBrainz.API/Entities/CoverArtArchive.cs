using System;
using System.IO;
using System.Net;
using System.Xml.Serialization;
using System.Drawing;

namespace Hqub.MusicBrainz.API.Entities
{
    [XmlRoot("cover-art-archive", Namespace = "http://musicbrainz.org/ns/mmd-2.0#")]
    public class CoverArtArchive
    {
        /// <summary>
        /// Gets or sets a value indicating whether artwork is available or not.
        /// </summary>
        [XmlElement("artwork")]
        public bool Artwork { get; set; }

        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        [XmlElement("count")]
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a front crover is available or not.
        /// </summary>
        [XmlElement("front")]
        public bool Front { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a back crover is available or not.
        /// </summary>
        [XmlElement("back")]
        public bool Back { get; set; }

        public static Uri GetCoverArtUri(string releaseId)
        {
            string url = "http://coverartarchive.org/release/" + releaseId + "/front-250.jpg";
            return new Uri(url, UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        /// FAB
        /// </summary>
        public static MemoryStream GetImageUri(Uri uri)
        {
            try
            {
                WebClient wc = new System.Net.WebClient();
                IWebProxy iwpxy = WebRequest.GetSystemWebProxy();
                wc.Proxy = iwpxy;
                wc.Proxy.Credentials = CredentialCache.DefaultCredentials;
                byte[] bFile = wc.DownloadData(uri);
                MemoryStream ms = new MemoryStream(bFile);

                return ms;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            //Image img = Image.FromStream(ms);            
            //return img;

        }
    }
}

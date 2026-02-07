using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OvhLyrics.Api.adapter
{
    public class WSConsumer : IWSConsumer
    {
        private HttpClient client;

        public WSConsumer()
        {
            client = new HttpClient();
        }

        public async Task<String> GetLyricsJson(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = client.GetAsync(url).Result;
                string content = response.Content.ReadAsStringAsync().Result;
                return content;
            }
        }

        public String GetLyricsFromJson(string url)
        {
            try
            {                
                var response = GetLyricsJson(url).Result;

                //JsonNode jsonNode = JsonNode.Parse(GetLyricsJson(url).Result);
                //string lyrics = jsonNode["lyrics"].ToString();
                string lyrics = response.Split(new string[] { "\"lyrics\":\"" }, StringSplitOptions.None)[1].Split(new string[] { "\"}" }, StringSplitOptions.None)[0];
                return lyrics;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}

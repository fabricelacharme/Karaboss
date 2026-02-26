using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvhLyrics.Api.adapter
{
    public interface IWSConsumer
    {
        Task<String> GetLyricsJson(string url);
        String GetLyricsFromJson(string url);
    }
}

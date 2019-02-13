using System;
using System.Text.RegularExpressions;

namespace Karaboss.Lrc
{
    class Lrc : IComparable
    {
        /// <summary>
        /// 歌词行对应的文字
        /// </summary>
        public string LrcContent = string.Empty;

        /// <summary>
        /// 歌词行对应的时间
        /// </summary>
        public Time ActualTime = Time.Zero;

        public Lrc(double time, string content)
        {
            LrcContent = content;
            ActualTime = Time.Parse(time);
        }

        public Lrc(Time time, string content)
        {
            LrcContent = content;
            ActualTime = new Time(time);
        }
        
        /// <summary>
        /// 传入一行信息，并自动分析出时间和文字
        /// </summary>
        /// <param name="line"></param>
        public Lrc(string line)
        {
            Regex time = new Regex(@"(?<=\[).*?(?=\])");
            Regex content = new Regex(@"(?<=\]).*$");
            if (!Time.TryParse(time.Match(line).ToString()))
            {
                // 这里有可能是方括号中的歌词信息，但是暂时不支持此功能，所以报错
                throw new Exception("歌词内容有误，无法获取时间");
            }
            else
            {
                string t = time.Match(line).ToString();
                LrcContent = content.Match(line).ToString();
                ActualTime = Time.Parse(t);
            }
        }

        public Lrc()
        {
            // Nothing
        }
        
        /// <summary>
        /// 返回在歌词文件中应该呈现出的单行文字（[时间]歌词）
        /// </summary>
        public string Info
        {
            get { return "[" + ActualTime.Info + "]" + LrcContent; }
        }
        
        /// <summary>
        /// 用于按时间排列的参考依据
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            Lrc o = (Lrc)obj;
            if (ActualTime > o.ActualTime) return 1;
            else if (ActualTime.TotalSeconds == o.ActualTime.TotalSeconds) return 0;
            else return -1;
        }
    }
}

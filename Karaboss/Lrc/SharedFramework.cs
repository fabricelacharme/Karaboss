using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Karaboss.Lrc.SharedFramework
{

    public class LyricsLine : IComparable<LyricsLine>
    {
        private int _offset = 0;    // 100=1000ms=1s
        private int _timeline;      // 100=1000ms=1s
        internal string Timeline    // []
        {
            get
            {
                int _tmptimeline = _timeline;//
                if (_offset != 0)
                    _tmptimeline = _tmptimeline - _offset;  
                int MSec = 0, Sec = 0, Min = 0; // Msec 10
                if (_tmptimeline > 99)
                {
                    MSec = _tmptimeline % 100;
                    Sec = Convert.ToInt32(Math.Floor(_tmptimeline / 100.0));
                }
                else
                    return $"00:00.{_tmptimeline:D2}";

                if (Sec > 59)
                {
                    Min = Convert.ToInt32(Math.Floor(Sec / 60.0));
                    Sec = Sec % 60;
                }
                return $"{Min:D2}:{Sec:D2}.{MSec:D2}";
            }

            set
            {
                try
                {
                    if (Regex.IsMatch(value, @"^offset:\d+$", RegexOptions.IgnoreCase)) // offset
                    {
                        _offset = Convert.ToInt32(Math.Round(Convert.ToDecimal(Regex.Match(value, @"(?<=offset:)\d+$", RegexOptions.IgnoreCase).Value) / 10));
                        return;
                    }

                    int MSec = 0, Sec = 0, Min = 0;// Msec 10
                    Min = Convert.ToInt32(Regex.Match(value, @"^\d+(?=:)").Value);
                    Sec = Convert.ToInt32(Regex.Match(value, @"(?<=:)\d+(?=\.)").Value != "" ? Regex.Match(value, @"(?<=:)\d+(?=\.)").Value : Regex.Match(value, @"(?<=:)\d+$").Value);
                    MSec = Convert.ToInt32(Regex.Match(value, @"(?<=\.)\d+$").Value != "" ? Regex.Match(value, @"(?<=\.)\d+$").Value : "0"); // 00:37
                    if (MSec > 99)
                        MSec = Convert.ToInt32(Math.Round(MSec / 10.0));
                    int tl = MSec + Sec * 100 + Min * 100 * 60;
                    if (tl > 0)
                        _timeline = tl;
                    else
                        _timeline = 0;
                }
                catch (Exception ex)
                {
                    
                }
            }
        }


        private string _oriLyrics;
        internal string OriLyrics
        {
            get => _oriLyrics;
            set => _oriLyrics = value;
        }

        private string _break;
        internal string Break
        {
            get => _break;
            private set => _break = value;
        }

        private string _transLyrics;
        internal string TransLyrics
        {
            get => _transLyrics;
            private set => _transLyrics = value;
        }

        internal bool HasTrans()
        {
            if (Break != null && Break != "" && TransLyrics != null && TransLyrics != "")
                return true;
            else
                return false;
        }

        public override string ToString()
        {
            if (Break == null)
            {
                return OriLyrics;
            }
            else
            {

                return OriLyrics + Break + TransLyrics;
            }
        }

        internal void SetTransLyrics(string breakText, string transLyricsText, bool claerIt = false)//ClearIt
        {
            if (claerIt)
            {
                Break = null;
                TransLyrics = null;
                return;
            }
            Break = breakText;
            TransLyrics = transLyricsText;
        }

        internal void DelayTimeline(int mSec)   // 100=1000ms=1s
        {
            if (_timeline + mSec > 0)
                _timeline = _timeline + mSec;
        }

        public int CompareTo(LyricsLine other)
        {
            return _timeline.CompareTo(other._timeline);
        }

    }


    public class Lyrics
    {
        List<LyricsLine> LyricsLineText = new List<LyricsLine>();
        public bool HasTags
        {
            get => GetAllTags() != "";
        }

        public string GetAllTags()
        {
            string allTags = "";
            foreach (var item in Tags)
            {
                if (allTags == "")
                    allTags = string.Format("[{0}:{1}]", item.Key, item.Value);
                else
                    allTags += string.Format("\r\n[{0}:{1}]", item.Key, item.Value);
            }
            return allTags;
        }

        public enum LyricsTags {[Description("Artist")] Ar, [Description("Title")] Ti, [Description("Album")] Al, [Description("By")] By };
        public Dictionary<LyricsTags, string> Tags = new Dictionary<LyricsTags, string>();

        public int Count
        {
            get
            {
                return LyricsLineText.Count;
            }
        }
        
        /// <summary>
        /// Tag 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string ReturnString = GetAllTags();
            foreach (LyricsLine ll in LyricsLineText)
                ReturnString += "\r\n" + ll.ToString();
            return ReturnString;
        }

        public LyricsLine this[int index] => LyricsLineText[index];

        public Lyrics(List<LyricsLine> Lyrics)
        {
            LyricsLineText = Lyrics;
        }

        public Lyrics()
        {
        }

        public Lyrics(string text, string breakText = null)
        {
            ArrangeLyrics(text, breakText);
        }

        public void Sort()
        {
            LyricsLineText.Sort();
        }

        public void ArrangeLyrics(string text, string breakText = null)
        {
            string formatNewline(string rowText)
            {

                // \r.length=1   \r\n.length=2
                StringBuilder tmp = new StringBuilder(rowText);

                tmp.Replace(@"\r", "\r");
                tmp.Replace(@"\n", "\n");

                if (Regex.IsMatch(tmp.ToString(), @"\r(?!\n)"))
                {
                    MatchCollection mc = Regex.Matches(tmp.ToString(), @"\r(?!\n)");
                    for (int i = 0; i < mc.Count; i++)
                    {
                        tmp.Remove(mc[i].Index + i, 1);
                        tmp.Insert(mc[i].Index + i, "\r\n");
                    }
                }

                if (Regex.IsMatch(tmp.ToString(), @"(?<!\r)\n"))
                {
                    MatchCollection mc = Regex.Matches(tmp.ToString(), @"(?<!\r)\n");
                    for (int i = 0; i < mc.Count; i++)
                    {
                        tmp.Remove(mc[i].Index + i, 1);
                        tmp.Insert(mc[i].Index + i, "\r\n");
                    }
                }

                if (Regex.IsMatch(tmp.ToString(), @"\r\n"))
                {
                    MatchCollection mc = Regex.Matches(tmp.ToString(), @"(?<!\r)\n");
                    for (int i = 0; i < mc.Count; i++)
                    {
                        tmp.Remove(mc[i].Index + i, 1);
                        tmp.Insert(mc[i].Index + i, "\r\n");
                    }
                }
                return tmp.ToString();
            }

            text = formatNewline(text);
            string[] textList = text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            int totalCount = textList.Count();

            int j = -1;

            for (int i = 0; i < totalCount; i++)
            {                
                if (Regex.IsMatch(textList[i], @"^\[Ar:.*\]$", RegexOptions.IgnoreCase))// tags Artist
                {
                    string tagText = Regex.Match(textList[i], @"(?<=\[Ar:).*(?=\])", RegexOptions.IgnoreCase).Value;
                    if (Tags.ContainsKey(LyricsTags.Ar))
                        Tags[LyricsTags.Ar] += "/" + tagText;
                    else
                        Tags.Add(LyricsTags.Ar, tagText);
                    continue;
                }
                if (Regex.IsMatch(textList[i], @"^\[Ti:.*\]$", RegexOptions.IgnoreCase))//tags Title
                {
                    string tagText = Regex.Match(textList[i], @"(?<=\[Ti:).*(?=\])", RegexOptions.IgnoreCase).Value;
                    if (Tags.ContainsKey(LyricsTags.Ti))
                        Tags[LyricsTags.Ti] += "/" + tagText;
                    else
                        Tags.Add(LyricsTags.Ti, tagText);
                    continue;
                }
                if (Regex.IsMatch(textList[i], @"^\[Al:.*\]$", RegexOptions.IgnoreCase))// tags Album
                {
                    string tagText = Regex.Match(textList[i], @"(?<=\[Al:).*(?=\])", RegexOptions.IgnoreCase).Value;
                    if (Tags.ContainsKey(LyricsTags.Al))
                        Tags[LyricsTags.Al] += "/" + tagText;
                    else
                        Tags.Add(LyricsTags.Al, tagText);
                    continue;
                }
                if (Regex.IsMatch(textList[i], @"^\[By:.*\]$", RegexOptions.IgnoreCase))// tags By
                {
                    string tagText = Regex.Match(textList[i], @"(?<=\[By:).*(?=\])", RegexOptions.IgnoreCase).Value;
                    if (Tags.ContainsKey(LyricsTags.By))
                        Tags[LyricsTags.By] += "/" + tagText;
                    else
                        Tags.Add(LyricsTags.By, tagText);
                    continue;
                }
                if (Regex.IsMatch(textList[i], @"^\[\D+:.*\]$", RegexOptions.IgnoreCase))// tag！！！
                    continue;

                MatchCollection mc = Regex.Matches(textList[i], @"(?<=\[).+?(?=\])");
                int c = mc.Count;
                for (int k = 0; k < c; k++)
                {
                    j++;
                    LyricsLineText.Add(new LyricsLine());
                    LyricsLineText[j].Timeline = mc[k].Value;

                    if (breakText != null)
                    {
                        LyricsLineText[j].OriLyrics = Regex.Match(textList[i], @"(?<=\[.+\])[^\[\]]+(?=" + breakText + @")").Value;
                        LyricsLineText[j].SetTransLyrics(breakText, Regex.Match(textList[i], @"(?<=" + breakText + @").+$").Value);
                    }
                    else
                        LyricsLineText[j].OriLyrics = Regex.Match(textList[i], @"(?<=\[.+\])[^\[\]]+$").Value;
                }
            }
        }

        public bool HasTransLyrics(int line)
        {
            if (LyricsLineText[line].Break != null)
                return true;
            else
                return false;
        }

        public string[] GetWalkmanStyleLyrics(int modelIndex, object[] args)
        {
            string errorLog = "";
            switch (modelIndex)
            {
                case 0:
                    try
                    {
                        int DelayMsec = Convert.ToInt32(args[0]);
                        StringBuilder returnString = new StringBuilder("");
                        for (int i = 0; i < Count; i++)
                        {
                            if (Count == 0)
                            {
                                errorLog = errorLog + "<MixedLyric COUNT ERROR>";
                                return new string[] { "", errorLog };
                            }
                            else if (this[i].HasTrans())
                            {
                                if (returnString.ToString() != "")
                                {
                                    returnString.Append("\r\n[" + this[i].Timeline + "]" + (DelayMsec >= 0 ? this[i].OriLyrics : this[i].TransLyrics));
                                    this[i].DelayTimeline((DelayMsec >= 0 ? DelayMsec : -DelayMsec));
                                    returnString.Append("\r\n[" + this[i].Timeline + "]" + (DelayMsec >= 0 ? this[i].TransLyrics : this[i].OriLyrics));
                                    this[i].DelayTimeline((DelayMsec >= 0 ? -DelayMsec : DelayMsec));
                                }

                                else
                                {
                                    returnString.Append("[" + this[i].Timeline + "]" + (DelayMsec >= 0 ? this[i].OriLyrics : this[i].TransLyrics));
                                    this[i].DelayTimeline((DelayMsec >= 0 ? DelayMsec : -DelayMsec));
                                    returnString.Append("\r\n[" + this[i].Timeline + "]" + (DelayMsec >= 0 ? this[i].TransLyrics : this[i].OriLyrics));
                                    this[i].DelayTimeline((DelayMsec >= 0 ? -DelayMsec : DelayMsec));
                                }

                            }
                            else if (this[i].HasTrans() == false)
                            {
                                if (returnString.ToString() != "")
                                    returnString.Append("\r\n[" + this[i].Timeline + "]" + this[i].ToString());
                                else
                                    returnString.Append("[" + this[i].Timeline + "]" + this[i].ToString());
                            }
                            else
                            {
                                errorLog = errorLog + "<Interesting things happened...>";
                                return new string[] { "", errorLog };
                            }
                        }

                        return new string[] { (GetAllTags() != "" ? GetAllTags() + "\r\n" : "") + returnString.ToString(), errorLog };
                    }
                    catch (System.ArgumentNullException)
                    {
                        errorLog = errorLog + "<ArgumentNullException ERROR!>";
                        return new string[] { "", errorLog };
                    }
                    catch (System.NullReferenceException)
                    {
                        errorLog = errorLog + "<NullReferenceException ERROR!>";
                        return new string[] { "", errorLog };
                    }

                case 1://处理标点/文字的占位，倘若能同屏显示那么就优先同屏显示，否则按 case 0 一样翻译换行
                       /* 
                        * 在 NW-A27 的环境下测试。2.2 英寸屏幕，320*240 分辨率
                        * 倘若一行为 10 ，同屏三行共 30 textSize
                        * 纯中文/大写字母 10/13 = 0.76
                        * 小写字母 10/16 = 0.62
                        * 全半角感叹号逗号都会转成半角显示以及英文半角句号，10/54 = 0.18
                        * 汉字全角句号 10/19 = 0.52
                        * 【 10/30 = 0.33
                        * （会转成半角， 10/33 = 0.30
                        * 空格处理得很诡异，索尼对于过长的空格直接换行就不理它了。短空格按逗号算，0.18
                        * 全角引号，10/28 = 0.35
                        * 半角引号，10/36 = 0.27
                        * 「 10/36 = 0.27
                        * 』 10/18 = 0.55
                        * 半角冒号 10/65 = 0.15，全角冒号会转为半角显示
                        * 数字 10/19 = 0.52
                        * ' 10/72 = 0.13
                        */
                    Dictionary<string, double> textSize = new Dictionary<string, double>()
                    {
                        {@"。", 0.52 },
                        {@"[【】]", 0.33 },
                        {@"[「」]", 0.27 },
                        {@"[『』]", 0.55 },
                        {@"[“”]", 0.35 },
                        {@"[""]", 0.27 },
                        {@"[！!，,. ]", 0.18 },
                        {@"[（）()]", 0.30 },
                        {@"[\u2E80-\u9FFF]", 0.76 },
                        {@"[\uac00-\ud7ff]", 0.76 },
                        {@"[A-Z]", 0.76 },
                        {@"[a-z]", 0.62 },
                        {@"[0-9]", 0.52 },
                        {@"'", 0.13 },
                        {@"[:：]", 0.15 },
                    };
                    try
                    {
                        int DelayMsec = Convert.ToInt32(args[0]);
                        StringBuilder returnString = new StringBuilder("");
                        for (int i = 0; i < Count; i++)
                        {
                            if (Count == 0)
                            {
                                errorLog = errorLog + "<MixedLyric COUNT ERROR>";
                                return new string[] { "", errorLog };
                            }
                            else if (this[i].HasTrans())
                            {
                                double totalTextSize = 0;
                                string connectedText = this[i].OriLyrics + this[i].TransLyrics;
                                void getSize(string pattern, double multiple)// size，connectedText
                                {
                                    MatchCollection mc = Regex.Matches(connectedText, pattern);
                                    totalTextSize += mc.Count * multiple;
                                    for (int j = 0; j < mc.Count; j++)
                                        connectedText = connectedText.Replace(mc[j].Value.ToString(), "");
                                }
                                foreach (var item in textSize)
                                    getSize(item.Key, item.Value);
                                if (connectedText != "")
                                {
                                    totalTextSize += connectedText.Count() * 0.76;
                                    errorLog = errorLog + "<connectedText{(" + connectedText.Count().ToString() + ") " + connectedText + "} is not empty>";
                                }
                                System.Diagnostics.Debug.WriteLine(this[i].OriLyrics + this[i].TransLyrics + "\r\n" + connectedText.Count().ToString() + ") " + connectedText + "\r\n" + totalTextSize + "\r\n============");
                                if (totalTextSize < 30) //30 size
                                {
                                    if (returnString.ToString() != "")
                                        returnString.Append("\r\n[" + this[i].Timeline + "]" + this[i].ToString());
                                    else
                                        returnString.Append("[" + this[i].Timeline + "]" + this[i].ToString());
                                }
                                else //case0
                                {//TODO copy
                                    if (returnString.ToString() != "")
                                    {
                                        returnString.Append("\r\n[" + this[i].Timeline + "]" + (DelayMsec >= 0 ? this[i].OriLyrics : this[i].TransLyrics));
                                        this[i].DelayTimeline((DelayMsec >= 0 ? DelayMsec : -DelayMsec));
                                        returnString.Append("\r\n[" + this[i].Timeline + "]" + (DelayMsec >= 0 ? this[i].TransLyrics : this[i].OriLyrics));
                                        this[i].DelayTimeline((DelayMsec >= 0 ? -DelayMsec : DelayMsec));
                                    }

                                    else
                                    {
                                        returnString.Append("[" + this[i].Timeline + "]" + (DelayMsec >= 0 ? this[i].OriLyrics : this[i].TransLyrics));
                                        this[i].DelayTimeline((DelayMsec >= 0 ? DelayMsec : -DelayMsec));
                                        returnString.Append("\r\n[" + this[i].Timeline + "]" + (DelayMsec >= 0 ? this[i].TransLyrics : this[i].OriLyrics));
                                        this[i].DelayTimeline((DelayMsec >= 0 ? -DelayMsec : DelayMsec));
                                    }
                                }
                            }
                            else if (this[i].HasTrans() == false)
                            {
                                if (returnString.ToString() != "")
                                    returnString.Append("\r\n[" + this[i].Timeline + "]" + this[i].ToString());
                                else
                                    returnString.Append("[" + this[i].Timeline + "]" + this[i].ToString());
                            }
                            else
                            {
                                errorLog = errorLog + "<Interesting things happened...>";
                                return new string[] { "", errorLog };
                            }
                        }

                        return new string[] { (GetAllTags() != "" ? GetAllTags() + "\r\n" : "") + returnString.ToString(), errorLog };//写入 Tag 信息
                    }
                    catch (System.ArgumentNullException)
                    {
                        errorLog = errorLog + "<ArgumentNullException ERROR!>";
                        return new string[] { "", errorLog };
                    }
                    catch (System.NullReferenceException)
                    {
                        errorLog = errorLog + "<NullReferenceException ERROR!>";
                        return new string[] { "", errorLog };
                    }
                default:
                    return null;
            }


        }
    }

 
}


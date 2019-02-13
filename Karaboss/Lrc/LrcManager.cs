using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Karaboss.Lrc
{
    class LrcManager
    {

        private List<Lrc> lrcList = new List<Lrc>();

        public LrcManager()
        {
        }

        public void LoadFromString(string content)
        {
            Clear();
            string[] list = content.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (string temp in list)
            {
                Regex timeCheck = new Regex(@"\[[^\]]+\]");
                Regex LrcCheck = new Regex(@"(?<=\])[^\[]*$");
                MatchCollection mc = timeCheck.Matches(temp);
                foreach (Match m in mc)
                {
                    string lrc = m.ToString() + LrcCheck.Match(temp).ToString();
                    lrcList.Add(new Lrc(lrc));
                }
                SortByTime();
            }
        }

        public void LoadFromStringWithoutTime(string content)
        {
            Clear();
            string[] list = content.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (string temp in list)
            {
                Lrc l = new Lrc("[00:00.000]" + temp);
                lrcList.Add(l);
            }
        }

        public void LoadFromFileName(string path)
        {
            Clear();
            // path OpenFileDialog
            StreamReader reader = new StreamReader(path);
            LoadFromString(reader.ReadToEnd());
            reader.Dispose();
        }

        public void LoadFromFileNameWithoutTime(string path)
        {
            Clear();
            StreamReader reader = new StreamReader(path);
            LoadFromStringWithoutTime(reader.ReadToEnd());
            reader.Dispose();
        }

        public void AddLine(Lrc line)
        {
            lrcList.Add(line);
        }

        public void AddLineAt(int index, Lrc line)
        {
            if (index < 0 || index > lrcList.Count) return;
            lrcList.Insert(index, line);
        }

        public void RemoveLineAt(int index)
        {
            if (index < 0 || index >= lrcList.Count) return;
            else lrcList.RemoveAt(index);
        }

        public void AmendLineAt(int index, string time, string content)
        {
            if (index < 0 || index >= lrcList.Count) return;
            if (!Time.TryParse(time)) return;
            lrcList[index].ActualTime = Time.Parse(time);
            lrcList[index].LrcContent = content;
        }

        public void Clear()
        {
            lrcList.Clear();
        }

        public void MoveLineUp(int index)
        {
            if (index <= 0 || index >= lrcList.Count) return;
            Lrc temp = GetLineAt(index);
            RemoveLineAt(index);
            AddLineAt(index - 1, temp);
        }

        public void MoveLineDown(int index)
        {
            if (index < 0 || index >= lrcList.Count - 1) return;
            Lrc temp = GetLineAt(index);
            RemoveLineAt(index);
            AddLineAt(index + 1, temp);
        }

        public Lrc GetLineAt(int index)
        {
            if (index < 0) return lrcList[0];
            else if (index >= lrcList.Count) return lrcList[lrcList.Count - 1];
            else return lrcList[index];
        }

        public List<string> StringList
        {
            get
            {
                List<string> list = new List<string>();
                foreach (Lrc lrc in lrcList)
                {
                    list.Add(lrc.Info);
                }
                return list;
            }
        }

        public List<Lrc> LrcList
        {
            get
            {
                return lrcList;
            }
        }

        public string ContentInLines
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                int Length = StringList.Count;
                List<string> list = StringList;
                for (int i = 0; i < Length - 1; i++)
                {
                    sb.Append(list[i]);
                    sb.Append(Environment.NewLine);
                }
                sb.Append(list[Length - 1]);
                return sb.ToString();
            }
        }

        public void SortByTime()
        {
            lrcList.Sort();
        }

        public void SetAllZero()
        {
            foreach (Lrc lrc in lrcList)
            {
                lrc.ActualTime = new Time(0);
            }
        }


        public void ShiftAll(double offset)
        {
            // 这里假设输入的offset是合理的
            foreach (Lrc line in lrcList)
            {
                double temp = line.ActualTime.TotalSeconds + offset;
                if (temp < 0) temp = 0;
                line.ActualTime = new Time(temp);
            }
        }    

    }
}

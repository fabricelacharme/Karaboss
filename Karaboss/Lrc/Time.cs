using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Karaboss.Lrc
{
    struct Time
    {
        /// <summary>
        /// 单位为秒
        /// </summary>
        private double time;
        /// <summary>
        /// 获取00:00.000时间格式下分钟部分的值
        /// </summary>
        public int Minutes
        {
            get
            {
                return (int)(time / 60);
            }
        }
        /// <summary>
        /// 获取00:00.000时间格式下秒钟部分的值
        /// </summary>
        public int Seconds
        {
            get
            {
                return (int)((time - 60 * Minutes));
            }
        }
        /// <summary>
        /// 获取00:00.000时间格式下毫秒部分的值（x1000）
        /// </summary>
        public int Milliseconds
        {
            get
            {
                return (int)Math.Round(1000 * (time - Math.Floor(time)));
            }
        }
        /// <summary>
        /// 获取00:00.000时间格式下毫秒部分的近似值（x1000并将个位数四舍五入）
        /// </summary>
        public int ApproxMilliseconds
        {
            get
            {
                return (int)(1000 * Math.Round((double)Milliseconds / 1000, 2));
            }
        }
        /// <summary>
        /// 获取时间对应的总秒数
        /// </summary>
        public double TotalSeconds
        {
            get
            {
                return time;
            }
            set
            {
                time = value;
            }
        }
        /// <summary>
        /// 获取时间对应的00:00.000格式的字符串
        /// </summary>
        public string Info
        {
            get
            {
                return string.Format("{0:00}:{1:00}.{2:000}", Minutes, Seconds, Milliseconds);
            }
        }
        /// <summary>
        /// 获取时间对应的00:00.000格式的字符串（毫秒数最低位四舍五入）
        /// </summary>
        public string ApproxInfo
        {
            get
            {
                return string.Format("{0:00}:{1:00}.{2:000}", Minutes, Seconds, ApproxMilliseconds);
            }
        }

        public Time(double value)
        {
            // 输入类似186.34秒（3分6秒340毫秒，00:03:06.340）这样的信息
            time = value;
        }
        public Time(string value)
        {
            time = 0;
            // 输入类似00:03:06.340这样的信息
            if (TryParse(value))
            {
                time = Parse(value).TotalSeconds;
            }
        }
        public Time(Time time)
        {
            this.time = time.TotalSeconds;
        }
        public Time(TimeSpan time)
        {
            this.time = time.TotalMilliseconds / 1000;
        }
        public static bool TryParse(string value)
        {
            // 总共被:.符号分成几部分
            Regex r1 = new Regex(@"[:.]");
            string[] numbers = r1.Split(value);
            int c1 = numbers.Count();

            // 总共有几组数字
            Regex r2 = new Regex(@"\d+");
            MatchCollection mc = r2.Matches(value);
            int c2 = mc.Count;

            if (c1 == 3)
            {
                if (c2 == 3)
                    return true;
            }
            else if (c1 == 4)
            {
                if (c2 == 4)
                    return true;
            }
            return false;
        }

        public static Time Parse(string value)
        {
            Regex r = new Regex(@"\d+");
            MatchCollection mc = r.Matches(value);
            double milliseconds = 0;
            if (mc.Count == 4)
            {
                int hour = int.Parse(mc[0].ToString());
                int minute = int.Parse(mc[1].ToString());
                int second = int.Parse(mc[2].ToString());
                milliseconds = hour * 3600 + minute * 60 + second;
            }
            else if (mc.Count == 3)
            {
                int minute = int.Parse(mc[0].ToString());
                int second = int.Parse(mc[1].ToString());
                milliseconds = minute * 60 + second;
            }
            string mill = mc[mc.Count - 1].ToString();
            double milli = double.Parse(mill) / Math.Pow(10, mill.Length);
            milliseconds += milli;
            return new Time(milliseconds);
        }

        public static Time Parse(double value)
        {
            return new Time(value);
        }

        public static Time Parse(TimeSpan time)
        {
            return new Time(time);
        }

        public static Time operator +(Time time1, Time time2)
        {
            return new Time(time1.TotalSeconds + time2.TotalSeconds);
        }

        public static Time operator +(Time time1, double time2)
        {
            return new Time(time1.TotalSeconds + time2);
        }

        public static Time operator -(Time time1, Time time2)
        {
            return new Time(time1.TotalSeconds - time2.TotalSeconds);
        }

        public static Time operator -(Time time1, double time2)
        {
            return new Time(time1.TotalSeconds - time2);
        }

        public static bool operator >(Time time1, Time time2)
        {
            if (time1.TotalSeconds > time2.TotalSeconds) return true;
            else return false;
        }

        public static bool operator <(Time time1, Time time2)
        {
            if (time1 > time2 || time1.time == time2.time) return false;
            else return true;
        }

        public static readonly Time Zero = new Time(0);
        public static readonly Time Max = new Time(215999.999);
        public static readonly Time Min = new Time(-215999.999);

        public override string ToString()
        {
            return Info;
        }
    }
}

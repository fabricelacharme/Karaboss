#region License

/* Copyright (c) 2025 Fabrice Lacharme
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to 
 * deal in the Software without restriction, including without limitation the 
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
 * sell copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software. 
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 * THE SOFTWARE.
 */

#endregion

#region Contact

/*
 * Fabrice Lacharme
 * Email: fabrice.lacharme@gmail.com
 */

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Karaboss.Utilities
{
    public static class LyricsUtilities
    {
        /// <summary>
        /// Conversion table accented chars to non accentued chars
        /// </summary>
        static Dictionary<string, string> foreign_characters = new Dictionary<string, string>
        {
            { "äæǽ", "ae" },
            { "öœ", "oe" },
            { "ü", "ue" },
            { "Ä", "Ae" },
            { "Ü", "Ue" },
            { "Ö", "Oe" },
            { "ÀÁÂÃÄÅǺĀĂĄǍΑΆẢẠẦẪẨẬẰẮẴẲẶА", "A" },
            { "àáâãåǻāăąǎªαάảạầấẫẩậằắẵẳặа", "a" },
            { "Б", "B" },
            { "б", "b" },
            { "ÇĆĈĊČ", "C" },
            { "çćĉċč", "c" },
            { "Д", "D" },
            { "д", "d" },
            { "ÐĎĐΔ", "Dj" },
            { "ðďđδ", "dj" },
            { "ÈÉÊËĒĔĖĘĚΕΈẼẺẸỀẾỄỂỆЕЭ", "E" },
            { "èéêëēĕėęěέεẽẻẹềếễểệеэ", "e" },
            { "Ф", "F" },
            { "ф", "f" },
            { "ĜĞĠĢΓГҐ", "G" },
            { "ĝğġģγгґ", "g" },
            { "ĤĦ", "H" },
            { "ĥħ", "h" },
            { "ÌÍÎÏĨĪĬǏĮİΗΉΊΙΪỈỊИЫ", "I" },
            { "ìíîïĩīĭǐįıηήίιϊỉịиыї", "i" },
            { "Ĵ", "J" },
            { "ĵ", "j" },
            { "ĶΚК", "K" },
            { "ķκк", "k" },
            { "ĹĻĽĿŁΛЛ", "L" },
            { "ĺļľŀłλл", "l" },
            { "М", "M" },
            { "м", "m" },
            { "ÑŃŅŇΝН", "N" },
            { "ñńņňŉνн", "n" },
            { "ÒÓÔÕŌŎǑŐƠØǾΟΌΩΏỎỌỒỐỖỔỘỜỚỠỞỢО", "O" },
            { "òóôõōŏǒőơøǿºοόωώỏọồốỗổộờớỡởợо", "o" },
            { "П", "P" },
            { "п", "p" },
            { "ŔŖŘΡР", "R" },
            { "ŕŗřρр", "r" },
            { "ŚŜŞȘŠΣС", "S" },
            { "śŝşșšſσςс", "s" },
            { "ȚŢŤŦτТ", "T" },
            { "țţťŧт", "t" },
            { "ÙÚÛŨŪŬŮŰŲƯǓǕǗǙǛŨỦỤỪỨỮỬỰУ", "U" },
            { "ùúûũūŭůűųưǔǖǘǚǜυύϋủụừứữửựу", "u" },
            { "ÝŸŶΥΎΫỲỸỶỴЙ", "Y" },
            { "ýÿŷỳỹỷỵй", "y" },
            { "В", "V" },
            { "в", "v" },
            { "Ŵ", "W" },
            { "ŵ", "w" },
            { "ŹŻŽΖЗ", "Z" },
            { "źżžζз", "z" },
            { "ÆǼ", "AE" },
            { "ẞ", "Ss" },
            { "ß", "ss" },
            { "Ĳ", "IJ" },
            { "ĳ", "ij" },
            { "Œ", "OE" },
            { "ƒ", "f" },
            { "ξ", "ks" },
            { "π", "p" },
            { "β", "v" },
            { "μ", "m" },
            { "ψ", "ps" },
            { "Ё", "Yo" },
            { "ё", "yo" },
            { "Є", "Ye" },
            { "є", "ye" },
            { "Ї", "Yi" },
            { "Ж", "Zh" },
            { "ж", "zh" },
            { "Х", "Kh" },
            { "х", "kh" },
            { "Ц", "Ts" },
            { "ц", "ts" },
            { "Ч", "Ch" },
            { "ч", "ch" },
            { "Ш", "Sh" },
            { "ш", "sh" },
            { "Щ", "Shch" },
            { "щ", "shch" },
            { "ЪъЬь", "" },
            { "Ю", "Yu" },
            { "ю", "yu" },
            { "Я", "Ya" },
            { "я", "ya" },
         };

        /// <summary>
        /// Remove accents for chars
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static char RemoveDiacritics(this char c)
        {
            foreach (KeyValuePair<string, string> entry in foreign_characters)
            {
                if (entry.Key.IndexOf(c) != -1)
                {
                    return entry.Value[0];
                }
            }
            return c;
        }

        /// <summary>
        /// Remove accents for strings
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string RemoveDiacritics(this string s)
        {
            string text = "";

            foreach (char c in s)
            {
                int len = text.Length;

                foreach (KeyValuePair<string, string> entry in foreign_characters)
                {
                    if (entry.Key.IndexOf(c) != -1)
                    {
                        text += entry.Value;
                        break;
                    }
                }

                if (len == text.Length)
                {
                    text += c;
                }
            }
            return text;
        }

        /// <summary>
        /// Remove all non alphanumerc characters of a string
        /// Except space, -, '
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveNonAlphaNumeric(string str)
        {
            // Cann also be used
            // str = String.Concat(Array.FindAll(str.ToCharArray(), Char.IsLetterOrDigit));
            // str = String.Concat(str.Where(char.IsLetterOrDigit));

            // With regex
            str = Regex.Replace(str, "[^a-zA-Z0-9\\s-']", String.Empty);
            return str;
        }


        /// <summary>
        /// Converts ticks into elapsed time, taking tempo changes into account
        /// Minutes, seconds, ms of seconds
        /// Ex: 6224 ticks => 00:09.152 (mm:ss.ms)
        /// </summary>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public static string TicksToTime(int ticks, double Division)
        {
            double dur = TempoUtilities.GetMidiDuration(ticks, Division);            
            double Min = (int)(dur / 60);
            double Sec = (int)(dur - (Min * 60));
            double Ms = (1000 * (dur - (Min * 60) - Sec));

            if (Math.Round(Ms) > 999)
            {                
                Ms = 0;
                Sec++;
                if (Sec > 59)
                {
                    Sec = 0;
                    Min++;
                }
            }
            
            return string.Format("{0:00}:{1:00}.{2:000}", Min, Sec, Ms);
        }

        /// <summary>
        /// Convert time to ticks
        /// 01:15.510 (min 2digits, sec 2 digits, ms 3 digits)
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int TimeToTicks(string time, double Division, int max)
        {
            int tic = 0;
            
            // Caculate duration in seconds
            double dur;

            string[] split1 = time.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (split1.Length != 2)
                return tic;

            string min = split1[0];

            string[] split2 = split1[1].Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (split2.Length != 2)
                return tic;

            string sec = split2[0];
            string ms = split2[1];

            // Calculate dur in seconds
            int Min = Convert.ToInt32(min);
            dur = Min * 60;

            int Sec = Convert.ToInt32(sec);
            dur += Sec;

            double Ms = Convert.ToDouble(ms);
            dur += Ms / 1000;
                      
            
            // TODO
            // Find ticks who are giving this time
            // Search convergence
            tic = 0;
            string tm;            
            do
            {
                // Start with step 100
                tm = TicksToTime(tic, Division);                
                if (tm == time)
                    return tic;                
                tic += 100;

                if (TempoUtilities.GetMidiDuration(tic, Division) > dur)
                {
                    tic -= 100;
                    do
                    {
                        // Continue with step 10
                        tm = TicksToTime(tic, Division);
                        if (tm == time)
                            return tic;
                        tic +=10;

                        if(TempoUtilities.GetMidiDuration(tic, Division) > dur)
                        {                                                                                    
                            tic -= 10;
                            do
                            {
                                // Continue with step 1
                                tm = TicksToTime(tic, Division);
                                
                                // Option: take the highest tic giving the good result.
                                // If next value of tic give also the same time, continue
                                if (tm == time && TicksToTime(tic + 1, Division) != time)                               
                                    return tic;
                                
                                tic++;
                            } while (tic <= max);
                            
                        }


                    } while (tic <= max);
                }

            } while (tic <= max);


            MessageBox.Show("Unable to calculate TimeToTick", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return tic;
        }


        #region LRC

        /// <summary>
        /// Returns lyrics by lines 
        /// format [00:08.834]QUAND J'AI RENCONTRE JOSEPHINE"
        /// </summary>
        /// <param name="lstLyricsItems"></param>
        /// <param name="strSpaceBetween"></param>
        /// <returns></returns>
        public static List<string> GetLrcLines(List<(string, string, string)> lstLyricsItems, string strSpaceBetween)
        {
            List<string> lstLines = new List<string>();

            bool bStartLine;
            string sTime;
            string sType;
            string sLyric;
            string sLine = string.Empty;

            bStartLine = true;

            try
            {
                // sTime, sType, sLyric
                for (int i = 0; i < lstLyricsItems.Count; i++)
                {
                    sTime = lstLyricsItems[i].Item1;
                    sType = lstLyricsItems[i].Item2;
                    sLyric = lstLyricsItems[i].Item3;

                    if (sType == "text")      // Do not add empty lyrics to a line ?
                    {

                        if (bStartLine)
                        {
                            if (sLyric.Length > 0 && sLyric.StartsWith(" "))
                                sLyric = sLyric.Remove(0, 1);
                            sLine = sTime + strSpaceBetween + sLyric;    // time + lyric for the beginning of a line                        
                            bStartLine = false;
                        }
                        else
                        {
                            // Line continuation
                            sLine += sLyric; // only lyric for the continuation of a line                        
                        }
                    }
                    else
                    {
                        // Remove last space
                        if (sLine.Length > 0 && sLine.EndsWith(" "))
                            sLine = sLine.Remove(sLine.Length - 1, 1);

                        // Save current line
                        if (sLine != "")
                        {
                            // Add new line
                            lstLines.Add(sLine);
                        }

                        // Reset all
                        bStartLine = true;
                        sLine = string.Empty;
                    }
                }
                // Save last line
                if (sLine != "")
                {
                    lstLines.Add(sLine);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //return null;
            }

            return lstLines;
        }


        /// <summary>
        /// Return lyrics by line with their timestamps
        /// Format [00:08.834]QUAND [00:09.107]J'AI [00:09.196]REN[00:09.469]CON[00:09.558]TRE [00:09.926]JO[00:10.107]SE[00:10.307]PHI[00:10.656]NE
        /// </summary>
        /// <param name="lstLyricsItems"></param>
        /// <param name="strSpaceBetween"></param>
        /// <returns></returns>
        public static List<string> GetLrcTimeLines(List<(string, string, string)> lstLyricsItems, string strSpaceBetween)
        {
            List<string> lstTimeLines = new List<string>();

            bool bStartLine;
            string sTime;
            string sType;
            string sLyric;
            string sTimeLine = string.Empty;
            bStartLine = true;

            try
            {
                // sTime, sType, sLyric
                for (int i = 0; i < lstLyricsItems.Count; i++)
                {
                    sTime = lstLyricsItems[i].Item1;
                    sType = lstLyricsItems[i].Item2;
                    sLyric = lstLyricsItems[i].Item3;

                    if (sType == "text")      // Do not add empty lyrics to a line ?
                    {
                        if (bStartLine)
                        {
                            if (sLyric.Length > 0 && sLyric.StartsWith(" "))
                                sLyric = sLyric.Remove(0, 1);
                            sTimeLine = sTime + strSpaceBetween + sLyric;
                            bStartLine = false;
                        }
                        else
                        {
                            // Line continuation

                            // Case of spaces at the left of the lyrics
                            // Add a space to the left to allow split by space
                            if (sLyric.Length > 0 && sLyric.StartsWith(" "))
                                sTimeLine += " " + sTime + strSpaceBetween + sLyric.Remove(0, 1);
                            else
                                sTimeLine += sTime + strSpaceBetween + sLyric;
                        }
                    }
                    else
                    {
                        if (sTimeLine.Length > 0 && sTimeLine.EndsWith(" "))
                            sTimeLine = sTimeLine.Remove(sTimeLine.Length - 1, 1);

                        if (sTimeLine != "")
                        {
                            // Add new line
                            lstTimeLines.Add(sTimeLine);
                        }

                        // Reset all
                        bStartLine = true;
                        sTimeLine = string.Empty;
                    }
                }

                // Save last line
                if (sTimeLine != "")
                {
                    // Remove last space
                    if (sTimeLine.Length > 0 && sTimeLine.EndsWith(" "))
                        sTimeLine = sTimeLine.Remove(sTimeLine.Length - 1, 1);
                    lstTimeLines.Add(sTimeLine);
                }
            }
            catch (Exception e) { MessageBox.Show(e.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error); }

            return lstTimeLines;
        }


        /// <summary>
        /// Return lyrics by line and cut lines to MaxLength characters
        /// </summary>
        /// <param name="lstTimeLines"></param>
        /// <param name="MaxLength"></param>
        /// <returns></returns>
        public static List<string> GetLrcLinesCut(List<string> lstTimeLines, int MaxLength)
        {
            List<string[]> lstWords = new List<string[]>();
            List<string[]> lstTimes = new List<string[]>();

            string sTimeLine;
            string strPartialLine;
            string sLine;
            bool bStartLine;
            string sLyric;
            string sTime;

            string[] words;
            string[] Times;
            string removepattern = @"\[\d{2}[:]\d{2}[.]\d{3}\]";
            string replace = @"";

            List<string> lstLinesCut = new List<string>();

            try
            {

                for (int i = 0; i < lstTimeLines.Count; i++)
                {
                    sTimeLine = lstTimeLines[i];
                    words = sTimeLine.Split(' ');
                    Times = new string[words.Length];
                    for (int j = 0; j < words.Length; j++)
                    {
                        Times[j] = words[j].Substring(0, 11);
                        words[j] = Regex.Replace(words[j], removepattern, replace);
                    }
                    lstWords.Add(words);
                    lstTimes.Add(Times);
                }

                // Manage length                
                strPartialLine = string.Empty;
                sLine = string.Empty;
                string[] ItemsW;
                string[] ItemsT;
                for (int i = 0; i < lstWords.Count; i++)
                {
                    ItemsT = lstTimes[i];
                    ItemsW = lstWords[i];
                    sLine = string.Empty;

                    for (int j = 0; j < ItemsW.Count(); j++)
                    {
                        bStartLine = (j == 0);
                        sLyric = ItemsW[j];
                        sTime = ItemsT[j];

                        if (!bStartLine && (strPartialLine + " " + sLyric).Length > MaxLength)
                        {
                            // Too long
                            // Remove last space
                            if (sLine.Length > 0 && sLine.EndsWith(" "))
                                sLine = sLine.Remove(sLine.Length - 1, 1);
                            lstLinesCut.Add(sLine);

                            // Restart a new line
                            sLine = sTime + sLyric + " ";
                            strPartialLine = sLyric + " ";

                        }
                        else
                        {
                            if (bStartLine)
                            {
                                sLine = sTime + sLyric + " ";
                                strPartialLine = sLyric + " ";
                            }
                            else
                            {
                                sLine += sLyric + " ";
                                strPartialLine += sLyric + " ";
                            }
                        }
                    }

                    // Remove last space
                    if (sLine.Length > 0 && sLine.EndsWith(" "))
                        sLine = sLine.Remove(sLine.Length - 1, 1);
                    lstLinesCut.Add(sLine);
                    sLine = string.Empty;
                }

                if (sLine != string.Empty)
                {
                    // Remove last space
                    if (sLine.Length > 0 && sLine.EndsWith(" "))
                        sLine = sLine.Remove(sLine.Length - 1, 1);
                    lstLinesCut.Add(sLine);
                }
            }
            catch (Exception e) 
            { 
                MessageBox.Show(e.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //return null;
            }
            return lstLinesCut;
        }

        /// <summary>
        /// Extraxt artist and song frem file name
        /// </summary>
        /// <param name="fPath"></param>
        /// <returns></returns>
        public static List<string> GetTagsFromFileName(string fPath)
        {
            // Classic Karaoke Midi tags
            /*
            @K	(multiple) K1: FileType ex MIDI KARAOKE FILE, K2: copyright of Karaoke file
            @L	(single) Language	FRAN, ENGL        
            @W	(multiple) Copyright (of Karaoke file, not song)        
            @T	(multiple) Title1 @T<title>, Title2 @T<author>, Title3 @T<copyright>		
            @I	Information  ex Date(of Karaoke file, not song)
            @V	(single) Version ex 0100 ?             
            */
            List<string> lstTags = new List<string>();
            string Tag_Artist = string.Empty;
            string Tag_Title = string.Empty;
            int pos;
            
            string fName = Path.GetFileNameWithoutExtension(fPath);
            string sep = " - ";

            try
            {
                if (fName.IndexOf(sep) == -1)
                {
                    Tag_Title = fName;
                    Tag_Artist = "";
                }
                else
                {
                    pos = fName.IndexOf(sep);           // First index
                    Tag_Artist = fName.Substring(0, pos);
                    Tag_Title = fName.Substring(pos + sep.Length, fName.Length - pos - sep.Length);
                }
            }
            catch (Exception e) 
            {
                MessageBox.Show(e.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error); 
            }

            return new List<string> { Tag_Artist, Tag_Title };
        }


        #endregion LRC


    }


}


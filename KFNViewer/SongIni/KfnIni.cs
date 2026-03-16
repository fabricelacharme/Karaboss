using IniParser.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TagLib.Matroska;
using TagLib.Riff;

namespace KFNViewer.SongIni
{

    /// Representing a file entry in the KFN file.

    public class Entry
    {
        /// <summary>
        /// Type of the file.
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// Name of the file.
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// First length value (originally `len1`).
        /// </summary>
        public long Length1 { get; set; }

        /// <summary>
        /// Offset of the file data within the container.
        /// </summary>
        public long Offset { get; set; }

        /// <summary>
        /// Second length value (originally `len2`).
        /// </summary>
        public long Length2 { get; set; }

        /// <summary>
        /// Flags associated with the entry.
        /// </summary>
        public long Flags { get; set; }

        /// <summary>
        /// Raw binary data of the file.
        /// </summary>
        public byte[] FileBinary { get; set; } = Array.Empty<byte>();

        /// <summary>
        /// Parameter‑less constructor that creates a default (empty) entry.
        /// Mirrors Rust's `#[derive(Default)]`.
        /// </summary>
        public Entry()
        {
            // All properties already have sensible defaults.
        }



    }

    public sealed class KfnIni
    {
        public IniData Ini { get; set; }

        public List<Eff> Effs { get; set; }

        public KfnIni()
        {
            Ini = new IniData();
            Effs = new List<Eff>();
        }

        /// <summary>
        /// Creating a new ini file.
        /// </summary>
        public static KfnIni New()
        {
            return new KfnIni();
        }

        /// <summary>
        /// Populating the General section with empty data.
        /// </summary>
        public void PopulateEmpty()
        {
            var section = "General";

            Ini[section]["Title"] = "";
            Ini[section]["Artist"] = "";
            Ini[section]["Album"] = "";
            Ini[section]["Composer"] = "";
            Ini[section]["Year"] = "";
            Ini[section]["Track"] = "";
            Ini[section]["GenreID"] = "-1";
            Ini[section]["Copyright"] = "";
            Ini[section]["Comment"] = "";
            Ini[section]["Source"] = "";
            Ini[section]["EffectCount"] = "";
            Ini[section]["LanguageID"] = "";
            Ini[section]["DiffMen"] = "";
            Ini[section]["DiffWomen"] = "";
            Ini[section]["KFNType"] = "0";
            Ini[section]["Properties"] = "";
            Ini[section]["KaraokeVersion"] = "";
            Ini[section]["VocalGuide"] = "";
            Ini[section]["KaraFunization"] = "";
        }

        /// Returns the secondary source / vocal included track, if it exists.
        public string GetSecondarySource()
        {
            var parser = new IniParser.Parser.IniDataParser();
            Ini = parser.Parse("config.ini");
            
            string value = Ini["MP3Music"]["Track0"];
            if (value != null)
            {                
                return value.Substring(0, value.LastIndexOf(".mp3", StringComparison.Ordinal) + 4);
            }
            else
                return null;            
        }

        public bool ReplacesTrack()
        {
            if (Ini["MP3Music"]["Track0"] != null)
            {
                if (Ini["MP3Music"]["Track0"].Split(',')[2] == "0")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        /// Populating the General section with empty data.
        public void PopulateFromHeader(KfnHeader header)
        {
            var source = string.Empty;
            if (header.SourceFile.Length > 4)
            {
                if (!string.Equals(header.SourceFile.Substring(0, 4), "1,I,", StringComparison.Ordinal))
                {
                    source = "1,I,";
                }
            }

            source += header.SourceFile;

            Ini.Sections["General"]["Title"] = header.Title;
            Ini.Sections["General"]["Artist"] = header.Artist;
            Ini.Sections["General"]["Album"] = header.Album;
            Ini.Sections["General"]["Composer"] = header.Composer;
            Ini.Sections["General"]["Year"] = header.Year;
            Ini.Sections["General"]["Track"] = header.Trak;
            Ini.Sections["General"]["GenreID"] = header.Genre.ToString();
            Ini.Sections["General"]["Copyright"] = header.Copyright;
            Ini.Sections["General"]["Comment"] = header.Comment;
            Ini.Sections["General"]["Source"] = source;
            Ini.Sections["General"]["EffectCount"] = "";
            Ini.Sections["General"]["LanguageID"] = header.Language;
            Ini.Sections["General"]["DiffMen"] = header.DiffMen.ToString();
            Ini.Sections["General"]["DiffWomen"] = header.DiffWomen.ToString();
            Ini.Sections["General"]["KFNType"] = header.KfnType.ToString();
            Ini.Sections["General"]["Properties"] = "";
            Ini.Sections["General"]["KaraokeVersion"] = "";
            Ini.Sections["General"]["VocalGuide"] = "";
            Ini.Sections["General"]["KaraFunization"] = header.Karafunizer;
        }


        /// Reading the Eff# headed sections
        public void LoadEff()
        {
            var textCountAll = 0;

            // get the number of effects to parse
            var effectCount = int.Parse(
                (Ini["General"]["EffectCount"] ?? "0").ToString(),
                CultureInfo.InvariantCulture
            );
            Debug.WriteLine(effectCount);

            // based on the number of effects...
            for (int effNum = 1; effNum <= effectCount; effNum++)
            {
                // create a string "Eff#"
                var eff = $"Eff{effNum}";

                // select the Eff# section based on the string we previously constructed
                KeyDataCollection section = Ini[eff];  //Ini.Section(eff);                

                // TODO implement the rest of the properties
                var id = int.Parse(section["ID"].ToString(), CultureInfo.InvariantCulture);

                // number of animations
                var nbAnim = int.Parse(section["NbAnim"].ToString(), CultureInfo.InvariantCulture);
                // number of text lines
                var textCount = int.Parse((section["TextCount"] ?? "0").ToString(), CultureInfo.InvariantCulture);
                textCountAll += textCount;

                // starting trajectory
                Trajectory initialTrajectory = Trajectory.FromString(section["Trajectory"] ?? string.Empty);

                // looking for initial library image

                string initialLibImage = section["LibImage"] != "" ? section["LibImage"] : null;

                string initialInactiveColor = section["InactiveColor"] != "" ? section["InactiveColor"] : null;

                // looking for initial video file
                string initialVideoFile = section["VideoFile"] != "" ? section["VideoFile"] : null;

                // looking for initial font
                (string, uint) initialFont = (null, 0);
                var s = section["Font"];
                if (s != "")
                    initialFont = ParseFontOrNull(s);

                // if none, revert to Arial Black, as that is the default in the original program
                //("Arial Black".to_string(), 12)
                Debug.WriteLine(initialFont);

                string initialActiveColor = section["ActiveColor"] != "" ? section["ActiveColor"] : null;

                // list of animations in Anim# form
                var anims = new List<Anim>();
                var syncs = new List<int>();
                var texts = new List<TextEntry>();
                //var texts = new List<string>();

                Debug.WriteLine(nbAnim);
                // reading the animations, if there are any.
                if (nbAnim != 0)
                {
                    for (var j = 0; j < nbAnim; j++)
                    {
                        // create a vector for the AnimEntries
                        var animEntries = new List<AnimEntry>();

                        // construct the key with the proper number
                        var key = $"Anim{j}";
                        var value = section[key];

                        // the time in ms, when the anim occurs. The first one will always be the time.
                        var parts = value.Split('|');
                        var time = int.Parse(parts[0], CultureInfo.InvariantCulture);
                        var remaining = parts.Skip(1).ToArray();

                        for (var i = 0; i < remaining.Length; i++)
                        {
                            var tokens = remaining[i].Split(',');

                            // first one is always the action
                            var action = Action.From(tokens[0]);

                            EffectKind effect = EffectKind.None;
                            var transTime = 0.0;
                            var transType = TransType.None;

                            for (var k = 0; k < tokens.Length; k++)
                            {
                                var kv = tokens[k].Split('=');
                                var tokenKey = kv[0];
                                var tokenValue = kv[1];

                                switch (tokenKey)
                                {
                                    case "Effect":
                                        effect = EffectExtensions.FromStr(tokenValue); //  EffectKind.From(tokenValue);
                                        break;
                                    case "TransitionTime":
                                        transTime = double.Parse(tokenValue, CultureInfo.InvariantCulture);
                                        break;
                                    case "TransitionType":
                                        transType = TransTypeExtensions.FromStr(tokenValue); //  TransType.From(tokenValue);
                                        break;
                                    default:
                                        break;
                                }
                            }

                            AnimEntry animEntry = new AnimEntry(action, effect, transTime, transType);
                            animEntries.Add(animEntry);
                        }

                        anims.Add(new Anim(time, animEntries));
                    } // for j in 0..nb_anim {
                } // if nb_anim != 0 {


                // reading sync data
                foreach (KeyData key in section)
                {
                    if (key.KeyName.Contains("Sync") && !key.KeyName.Contains("InSync"))
                    {
                        var syncTimes = key.Value
                            .Split(',')
                            .Select(ss => int.Parse(ss, CultureInfo.InvariantCulture))
                            .ToList();

                        syncs.AddRange(syncTimes);
                    }
                }               

                Debug.WriteLine(textCount);

                if (syncs.Count > 0)
                {
                    //dbg!(syncs.len());
                    var syncCounter = 0;

                    while (syncCounter < syncs.Count - 1 && textCount != 0)
                    {
                        for (var j = 0; j < textCount; j++)
                        {
                            var key = $"Text{j}";
                            var value = section[key] ?? string.Empty;
                            if (value == "")
                            {
                                continue;
                            }

                            var fragments = new List<(int, string)>();

                            var fragmentsVecSlashSplit = value.Split('/').Select(ss => ss.ToString()).ToList();
                            var fragmentsVec = new List<string>();

                            foreach (var fragment in fragmentsVecSlashSplit)
                            {
                                foreach (var ss in SplitInclusiveBySpace(fragment))
                                {
                                    fragmentsVec.Add(ss);
                                }
                            }

                            //dbg!(&syncs);
                            string display = string.Concat(value.Split('/').Select(ss => ss.ToString()));

                            foreach (var fragmentString in fragmentsVec)
                            {
                                Debug.WriteLine($"{fragmentString}, {syncCounter}");
                                fragments.Add((syncs[syncCounter], fragmentString));
                                syncCounter += 1;
                            }

                            texts.Add(new TextEntry(display, fragments, effNum));

                            //texts.push(value.to_owned());
                        }
                    }
                }

                //dbg!(&texts);
                Effs.Add(
                    new Eff(
                        id,
                        effNum,
                        anims,                                                                        
                        initialLibImage,
                        initialVideoFile,
                        initialFont,
                        initialActiveColor,
                        initialInactiveColor,
                        syncs,
                        texts,
                        initialTrajectory
                    )
                );
            } // for i in 1..effect_count {
        }
        
        private (string, uint) ParseFontOrNull(string s)
        {
                var res = s.Split('*');
                Debug.WriteLine(string.Join(", ", res));

                var filename = res[0];
                var extension = filename.Length >= 4 ? filename.Substring(filename.Length - 4, 4) : string.Empty;

                if (extension == ".ttf" || extension == ".TTF" || extension == ".otf")
                {
                    return (res[0], uint.Parse(res[1], CultureInfo.InvariantCulture));
                }

                return (null, 0);
        }

        static IEnumerable<string> SplitInclusiveBySpace(string input)
        {
                if (string.IsNullOrEmpty(input))
                    yield break;

                var start = 0;
                for (var i = 0; i < input.Length; i++)
                {
                    if (input[i] == ' ')
                    {
                        var len = i - start + 1; // include the space
                        yield return input.Substring(start, len);
                        start = i + 1;
                    }
                }
                if (start < input.Length)
                    yield return input.Substring(start);            
        }


        /// Returns the name of the source sound file. 
        public string GetSourceName()
        {
            Debug.WriteLine((Ini["General"]["Source"] ?? string.Empty).Substring(4));
            return Ini["General"]["Source"].Substring(4);
        }

        /// Method for setting up the effect in the Ini file.
        public void SetEff()
        {
            // Set the EffectCount - number of Eff sections in the Ini.
            Ini["General"]["EffectCount"] = Effs.Count.ToString();
            //Ini.SectionMut("General").Insert("EffectCount", Effs.Count.ToString());

            var effectCount = int.Parse(Ini["General"]["EffectCount"] ?? "0");

            // Iterate through ID of effects
            for (var effNum = 1; effNum <= effectCount; effNum++)
            {
                // prepare for section header
                var effSection = "Eff";

                // push number to section header, indexing starts at 1!
                effSection += effNum.ToString();

                var section = Ini[effSection];
                Eff eff = Effs[effNum - 1];

                // get essential fields
                section["ID"] = eff.id.ToString();
                section["NbAnim"] = eff.anims.Count.ToString();
                section["TextCount"] = eff.texts.Count.ToString();
                section["Trajectory"] = eff.initial_trajectory.ToString();

                // iterate through Anim# 
                for (var animN = 0; animN < Effs[effNum -1].anims.Count; animN++)
                {
                    // get into the appropriate section
                    var animSection = Ini[effSection];

                    // clone the Anim#
                    Anim anim = Effs[effNum].anims[animN].Clone();

                    // prepare string for manipulation
                    var animKey = "Anim";
                    // attach row #
                    animKey += animN.ToString();

                    // prepare value
                    var animVal = string.Empty;

                    // add time, as that is always the first value in line
                    animVal += anim.time.ToString();

                    // separator
                    animVal += "|";

                    // iterate through the entries
                    foreach (var animEntry in anim.anim_entries)
                    {
                        // and push the appropriate value
                        animVal += animEntry.action.ToString();
                    }

                    // lastly set it
                    //animSection.Set(animKey, animVal);
                    animSection[animKey] = animVal;
                }


                // Buid Sync0=... Sync1=...
                //Ini[effSection]["Sync0"] = string.Join(",", Effs[effNum - 1].syncs.ToList().Select(n => n.ToString()));

                // Convert List<int> to string
                if (Effs[effNum - 1].syncs.Count > 0)
                {
                    string strSyncs = string.Empty;
                    for (int i = 0; i < Effs[effNum - 1].syncs.Count; i++)
                    {
                        strSyncs += Effs[effNum - 1].syncs[i] + ",";
                    }
                    strSyncs = strSyncs.Substring(0, strSyncs.Length - 1);

                    bool bCut = false;
                    string parcel = string.Empty;
                    char[] characters = strSyncs.ToCharArray();
                    List<string> lstEffSyncs = new List<string>();
                    // Cut to 202 chars
                    for (int i = 0; i < characters.Length; i++)
                    {
                        if (!bCut)
                            parcel += characters[i];
                        if (bCut)
                        {
                            if (characters[i] != ',')
                                parcel += characters[i];
                            else
                            {
                                lstEffSyncs.Add(parcel);
                                bCut = false;
                                parcel = string.Empty;
                            }
                        }

                        if (parcel.Length > 200)
                            bCut = true;
                    }

                    if (parcel.Length > 0)
                        lstEffSyncs.Add(parcel);
                    
                    for (int i = 0; i < lstEffSyncs.Count; i++)
                    {
                        Ini[effSection]["Sync" + i.ToString()] = lstEffSyncs[i].ToString();
                    }
                }



                Ini[effSection]["TextCount"] = Effs[effNum - 1].texts.Count.ToString();
                // Buid Text0=... Text1=...
                for (int textIndex = 0; textIndex < Effs[effNum - 1].texts.Count; textIndex++)
                {
                    // Obtain the INI section that corresponds to the current effect
                    var textSection = Ini[effSection];

                    // Get the current text entry and its display string
                    var textValue = Effs[effNum - 1].texts[textIndex];
                    //string displayText = currentTextEntry.display;   // other fields are ignored

                    // Build the key "Text{rowNumber}"
                    string textKey = "Text" + textIndex.ToString();

                    // Store the value in the INI section
                    textSection[textKey] = textValue;
                }


            }
        }


        /// Setting the source file for the KFN. This must be a music type file.
        public void SetSource(string source)
        {
            var value = "1,I,";

            value += source;

            //Ini.WithSection("General").Set("Source", value);
            Ini["General"]["Source"] = value; 
        }

        /// Sets the list of files in the ini, based on the entries given.
        public void SetMaterials(List<Entry> materials)
        {
            var matCount = materials.Count;

            Ini["Materials"]["MatCount"] = matCount.ToString(CultureInfo.InvariantCulture);

            for (var n = 0; n < matCount; n++)
            {
                var key = "Mat";

                key += n.ToString(CultureInfo.InvariantCulture);

                string value = materials[n].FileName;

                Ini["Materials"][key] = value;
            }
        }


        /// <summary>
        /// // impl ToString for KfnIni
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string result = string.Empty;

            
            foreach (var IniSection in Ini.Sections)
            {

                if (result == string.Empty)
                    result = "[" + IniSection.SectionName + "]" + Environment.NewLine;
                else
                    result += Environment.NewLine + "[" + IniSection.SectionName + "]" + Environment.NewLine;

                
                SectionData sa = Ini.Sections.GetSectionData(IniSection.SectionName);

                foreach(var k in sa.Keys)
                {
                    string w = k.KeyName;
                    string v = k.Value;
                                        
                    result += k.KeyName + "=" + k.Value + Environment.NewLine;
                }
            }            
            return result;
        }
    }
}
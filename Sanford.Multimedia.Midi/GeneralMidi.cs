#region License

/* Copyright (c) 2005 Leslie Sanford
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
 * Leslie Sanford
 * Email: jabberdabber@hotmail.com
 */

#endregion

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Sanford.Multimedia.Midi
{

    public class MidiFile
    {        
        
        public static List<string> LoadInstruments()
        {
            List<string> insts = new List<string>()
            {
                    "AcousticGrandPiano",
        "BrightAcousticPiano",
        "ElectricGrandPiano",
        "HonkyTonkPiano",
        "ElectricPiano1",
        "ElectricPiano2",
        "Harpsichord",
        "Clavinet",
        "Celesta",
        "Glockenspiel",
        "MusicBox",
        "Vibraphone",
        "Marimba",
        "Xylophone",
        "TubularBells",
        "Dulcimer",
        "DrawbarOrgan",
        "PercussiveOrgan",
        "RockOrgan",
        "ChurchOrgan",
        "ReedOrgan",
        "Accordion",
        "Harmonica",
        "TangoAccordion",
        "AcousticGuitarNylon",
        "AcousticGuitarSteel",
        "ElectricGuitarJazz",
        "ElectricGuitarClean",
        "ElectricGuitarMuted",
        "OverdrivenGuitar",
        "DistortionGuitar",
        "GuitarHarmonics",
        "AcousticBass",
        "ElectricBassFinger",
        "ElectricBassPick",
        "FretlessBass",
        "SlapBass1",
        "SlapBass2",
        "SynthBass1",
        "SynthBass2",
        "Violin",
        "Viola",
        "Cello",
        "Contrabass",
        "TremoloStrings",
        "PizzicatoStrings",
        "OrchestralHarp",
        "Timpani",
        "StringEnsemble1",
        "StringEnsemble2",
        "SynthStrings1",
        "SynthStrings2",
        "ChoirAahs",
        "VoiceOohs",
        "SynthVoice",
        "OrchestraHit",
        "Trumpet",
        "Trombone",
        "Tuba",
        "MutedTrumpet",
        "FrenchHorn",
        "BrassSection",
        "SynthBrass1",
        "SynthBrass2",
        "SopranoSax",
        "AltoSax",
        "TenorSax",
        "BaritoneSax",
        "Oboe",
        "EnglishHorn",
        "Bassoon",
        "Clarinet",
        "Piccolo",
        "Flute",
        "Recorder",
        "PanFlute",
        "BlownBottle",
        "Shakuhachi",
        "Whistle",
        "Ocarina",
        "Lead1Square",
        "Lead2Sawtooth",
        "Lead3Calliope",
        "Lead4Chiff",
        "Lead5Charang",
        "Lead6Voice",
        "Lead7Fifths",
        "Lead8BassAndLead",
        "Pad1NewAge",
        "Pad2Warm",
        "Pad3Polysynth",
        "Pad4Choir",
        "Pad5Bowed",
        "Pad6Metallic",
        "Pad7Halo",
        "Pad8Sweep",
        "Fx1Rain",
        "Fx2Soundtrack",
        "Fx3Crystal",
        "Fx4Atmosphere",
        "Fx5Brightness",
        "Fx6Goblins",
        "Fx7Echoes",
        "Fx8SciFi",
        "Sitar",
        "Banjo",
        "Shamisen",
        "Koto",
        "Kalimba",
        "BagPipe",
        "Fiddle",
        "Shanai",
        "TinkleBell",
        "Agogo",
        "SteelDrums",
        "Woodblock",
        "TaikoDrum",
        "MelodicTom",
        "SynthDrum",
        "ReverseCymbal",
        "GuitarFretNoise",
        "BreathNoise",
        "Seashore",
        "BirdTweet",
        "TelephoneRing",
        "Helicopter",
        "Applause",
        "Gunshot"};
            return insts;
        }

        public static string CleanInput(string strIn)
        {
            // Replace invalid characters with empty strings.
            try {
               return Regex.Replace(strIn, @"[^\w\.@-]", "", RegexOptions.None, TimeSpan.FromSeconds(1.5)); 
            }
            // If we timeout when replacing invalid characters, 
            // we should return Empty.
            catch (RegexMatchTimeoutException) {
               return String.Empty;   
            }
        }

        /// <summary>
        /// Parse Enum GeneralMidiInstrument to retrieve instrument name
        /// </summary>
        /// <param name="programChange"></param>
        /// <returns></returns>
        public static string PCtoInstrument(int programChange)
        {
            try
            {
                return Enum.GetName(typeof(GeneralMidiInstrument), programChange);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
    
    /// <summary>
    /// Defines constants representing the General MIDI instrument set.
    /// </summary>
    public enum GeneralMidiInstrument
    {
        AcousticGrandPiano,
        BrightAcousticPiano,
        ElectricGrandPiano,
        HonkyTonkPiano,
        ElectricPiano1,
        ElectricPiano2,
        Harpsichord,
        Clavinet,
        Celesta,
        Glockenspiel,
        MusicBox,
        Vibraphone,
        Marimba,
        Xylophone,
        TubularBells,
        Dulcimer,
        DrawbarOrgan,
        PercussiveOrgan,
        RockOrgan,
        ChurchOrgan,
        ReedOrgan,
        Accordion,
        Harmonica,
        TangoAccordion, 
        AcousticGuitarNylon, 
        AcousticGuitarSteel,
        ElectricGuitarJazz,
        ElectricGuitarClean,
        ElectricGuitarMuted,
        OverdrivenGuitar,
        DistortionGuitar,
        GuitarHarmonics,
        AcousticBass,
        ElectricBassFinger,
        ElectricBassPick,
        FretlessBass,
        SlapBass1,
        SlapBass2,
        SynthBass1,
        SynthBass2,
        Violin,
        Viola,
        Cello,
        Contrabass,
        TremoloStrings,
        PizzicatoStrings,
        OrchestralHarp,
        Timpani,
        StringEnsemble1,
        StringEnsemble2,
        SynthStrings1,
        SynthStrings2,
        ChoirAahs,
        VoiceOohs,
        SynthVoice,
        OrchestraHit,
        Trumpet,
        Trombone,
        Tuba,
        MutedTrumpet,
        FrenchHorn,
        BrassSection,
        SynthBrass1,
        SynthBrass2,
        SopranoSax,
        AltoSax,
        TenorSax,
        BaritoneSax,
        Oboe,
        EnglishHorn,
        Bassoon,
        Clarinet,
        Piccolo,
        Flute,
        Recorder,
        PanFlute,
        BlownBottle,
        Shakuhachi,
        Whistle,
        Ocarina,
        Lead1Square,
        Lead2Sawtooth,
        Lead3Calliope,
        Lead4Chiff,
        Lead5Charang,
        Lead6Voice,
        Lead7Fifths,
        Lead8BassAndLead,
        Pad1NewAge,
        Pad2Warm,
        Pad3Polysynth,
        Pad4Choir,
        Pad5Bowed,
        Pad6Metallic,
        Pad7Halo,
        Pad8Sweep,
        Fx1Rain,
        Fx2Soundtrack,
        Fx3Crystal,
        Fx4Atmosphere,
        Fx5Brightness,
        Fx6Goblins,
        Fx7Echoes,
        Fx8SciFi,
        Sitar,
        Banjo,
        Shamisen,
        Koto,
        Kalimba,
        BagPipe,
        Fiddle,
        Shanai,
        TinkleBell,
        Agogo,
        SteelDrums,
        Woodblock,
        TaikoDrum,
        MelodicTom,
        SynthDrum,
        ReverseCymbal,
        GuitarFretNoise,
        BreathNoise,
        Seashore,
        BirdTweet,
        TelephoneRing,
        Helicopter,
        Applause,
        Gunshot
    }

    /// <summary>
    /// Class to store open midi file options
    /// </summary>
    public static class OpenMidiFileOptions
    {
        public static string TextEncoding { get; set; }
        public static bool SplitHands { get; set; }
    }

    /// <summary>
    /// Class to store midi tags
    /// </summary>
    public static class MidiTags
    {
        //tags of midi file (artist, album, ...)

        #region oldtags
        /*
        Midi file tags
        @K	(multiple) K1: FileType ex MIDI KARAOKE FILE, K2: copyright of Karaoke file
        @L	(single) Language	FRAN, ENGL        
        @W	(multiple) Copyright (of Karaoke file, not song)        
        @T	(multiple) Title1 @T<title>, Title2 @T<author>, Title3 @T<copyright>		
        @I	Information  ex Date(of Karaoke file, not song)
        @V	(single) Version ex 0100 ?        
        */
        // copyright : all  begining with @ ?
        private static string copyright = string.Empty;
        public static string Copyright { get { return copyright; } set { copyright = value; } }
        // @K        
        private static List<string> ktag = new List<string>();
        public static List<string> KTag { get { return ktag; } set { ktag = value; } }
        // @L
        private static List<string> ltag = new List<string>();
        public static List<string> LTag { get { return ltag; } set { ltag = value; } }
        // @I
        private static List<string> itag = new List<string>();
        public static List<string> ITag { get { return itag; } set { itag = value; } }
        // @V
        private static List<string> vtag = new List<string>();
        public static List<string> VTag { get { return vtag; } set { vtag = value; } }
        // @T
        private static List<string> ttag = new List<string>();
        public static List<string> TTag { get { return ttag; } set { ttag = value; } }
        // @W
        private static List<string> wtag = new List<string>();
        public static List<string> WTag { get { return wtag; } set { wtag = value; } }
        #endregion

        #region newtags
        // New tags similar to mp3
        // @id01   Title
        // @id02   Artist
        // @id03   Album
        // @id04   Copyright
        // @id05   Date
        // @id06   Editor
        // @id07   Genre        
        // @id08   Evaluation
        // @id09   Comment


        /// <summary>
        /// Song Title: @id01
        /// </summary>
        // Song title
        private static string tagtittle = string.Empty;
        public static string TagTitle { get { return tagtittle; } set { tagtittle = value; } }

        /// <summary>
        /// Artist: @id02
        /// </summary>
        // Artist name
        private static string tagartist = string.Empty;
        public static string TagArtist { get { return tagartist; } set { tagartist = value; } }

        /// <summary>
        /// Album: @id03
        /// </summary>
        // Album name
        private static string tagalbum = string.Empty;
        public static string TagAlbum { get { return tagalbum; } set { tagalbum = value; } }

        /// <summary>
        /// Copyright: @id04
        /// </summary>
        // Copyright of song
        private static string tagcopyright = string.Empty;
        public static string TagCopyright { get { return tagcopyright; } set { tagcopyright = value; } }

        /// <summary>
        /// Date: @id05
        /// </summary>
        // Date of album or song
        private static string tagdate = string.Empty;
        public static string TagDate { get { return tagdate; } set { tagdate = value; } }

        /// <summary>
        /// Editor: @id06
        /// </summary>
        // Editor
        private static string tageditor = string.Empty;
        public static string TagEditor { get { return tageditor; } set { tageditor = value; } }

        /// <summary>
        /// Genre: @id07
        /// </summary>
        // Genre: pop, folk r&b
        private static string taggenre = string.Empty;
        public static string TagGenre { get { return taggenre; } set { taggenre = value; } }

        /// <summary>
        /// Evaluation: @id08
        /// </summary>
        // Evaluation (1 to 5)
        private static string tagevaluation = string.Empty;
        public static string TagEvaluation { get { return tagevaluation; } set { tagevaluation = value; } }

        /// <summary>
        /// Comment: @id09
        /// </summary>
        // Comment;
        private static string tagcomment = string.Empty;
        public static string TagComment { get { return tagcomment; } set { tagcomment = value; } }


        #endregion


        public static void ResetTags()
        {
            MidiTags.Copyright = "";
            MidiTags.ITag = new List<string>();
            MidiTags.KTag = new List<string>();
            MidiTags.LTag = new List<string>();
            MidiTags.TTag = new List<string>();
            MidiTags.VTag = new List<string>();
            MidiTags.WTag = new List<string>();

            MidiTags.TagAlbum = "";
            MidiTags.TagArtist = "";
            MidiTags.TagComment = "";
            MidiTags.TagCopyright = "";
            MidiTags.TagDate = "";
            MidiTags.TagEditor = "";
            MidiTags.TagEvaluation = "";
            MidiTags.TagGenre = "";
            MidiTags.TagTitle = "";
        }
    }
    /// <summary>
    /// Class to store informations when creating a new midi file
    /// </summary>
    public static class CreateNewMidiFile
    {
        //private static int _numerator;
        public static int Numerator { get; set; }
        //private static int _denominator;
        public static int Denominator { get; set; }
        //private static int _division;
        public static int Division { get; set; }
        //private static int _tempo;
        public static int Tempo { get; set; }
        //private static int _measures;
        public static int Measures { get; set; }

        // Fab 27/10/2023
        public static string trackname { get; set; }
        public static int programchange { get; set; }
        public static int channel { get; set; }
        public static decimal trkindex { get; set; }
        public static int clef { get; set; }
        public static string instrumentname { get; set; }

        /// <summary>
        /// Folder from which the creation was started in frmExplorer
        /// </summary>
        public static string DefaultDirectory { get; set; }

    }

}
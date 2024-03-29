﻿#region License
// The MIT License (MIT)
// 
// Copyright (c) 2014 Emma 'Eniko' Maassen
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.ExceptionServices;

namespace TextPlayer.ABC {
    // http://abcnotation.com/wiki/abc:standard:v2.1#introduction
    // Note that this leaves many things unspecified and undocumented. Some undocumented features:
    // - Chord length should be the shortest note within the chords
    // - Chords can contain rests (and are used to calculate chord length as above)
    // - Lotro compatibility demands the default for accidentals is octave, not pitch (see '11.3 Accidental directives')
    // - Capitalised C is middle-C which is C4 in our system (or 261.6 Hz). This isn't explicitly stated but can be figured out by
    //   section '4.6 Clefs and transposition', where it states if the octave is reduced by 1 through a directive, 'c' will now be middle-C. 
    /// <summary>
    /// Abstract player which parses and plays ABC code. This class can load multiple tunes but can only play one of them at a time.
    /// </summary>
    public abstract class ABCPlayer : MusicPlayer {
        private static int defaultOctave = 4;
        public static int DefaultOctave { get { return defaultOctave; } set { defaultOctave = value; } }

        private static AccidentalPropagation defaultAccidentalPropagation = AccidentalPropagation.Octave;
        public static AccidentalPropagation DefaultAccidentalPropagation { get { return defaultAccidentalPropagation; } set { defaultAccidentalPropagation = value; } }

        private bool strict;
        private string version;
        private int versionMajor;
        private int versionMinor;

        private Dictionary<int, Tune> tunes;
        private bool inTune = false;

        private int tokenIndex;

        private int octave;
        private TimeSpan nextNote;

        private Dictionary<char, int> defaultAccidentals;
        private Dictionary<string, int> accidentals;
        private AccidentalPropagation accidentalPropagation;
        private Dictionary<string, int> tiedNotes;

        private double noteLength;
        private double meter;
        private double spm;
        private double volume;
        private int selectedTune = 1;

        private ABCSettings settings;
        private bool lotroCompatible;

        /// <summary>
        /// Creates an ABC player. Uses static properties DefaultOctave and DefaultAccidentalPropagation and strict=true if
        /// arguments are left blank.
        /// </summary>
        public ABCPlayer(bool strict = true, int? octave = null, AccidentalPropagation? accidentalProp = null)
            : base() {
            AutoDetectLotro = true;
            settings = new ABCSettings();
            settings.MaxSize = 1024 * 12;

            this.strict = strict;
            accidentals = new Dictionary<string, int>();
            tiedNotes = new Dictionary<string, int>();

            if (octave.HasValue)
                this.octave = octave.Value;
            else
                this.octave = DefaultOctave;

            if (accidentalProp.HasValue)
                this.accidentalPropagation = accidentalProp.Value;
            else
                this.accidentalPropagation = DefaultAccidentalPropagation;
        }

        private void SetDefaultValues() {
            tokenIndex = 0;
            meter = 1.0;
            volume = 90.0 / 127;
            noteLength = 0;
            tiedNotes.Clear();
        }

        private void SetHeaderValues(int index = 0, bool inferNoteLength = false) {
            List<string> values;

            if (tunes[index].Header.Information.TryGetValue('K', out values)) { // key
                GetKey(values[values.Count - 1]);
            }
            if (tunes[index].Header.Information.TryGetValue('M', out values)) { // meter
                meter = GetNoteLength(values[values.Count - 1]);
            }
            if (tunes[index].Header.Information.TryGetValue('L', out values)) { // note length
                noteLength = GetNoteLength(values[values.Count - 1]);
            }

            if (inferNoteLength && noteLength == 0)
                noteLength = (meter >= 0.75 ? 1.0 / 8 : 1.0 / 16);

            if (tunes[index].Header.Information.TryGetValue('Q', out values)) { // tempo
                SetTempo(values[values.Count - 1]);
            }
        }

        private void GetKey(string s) {
            defaultAccidentals = Keys.GetAccidentals(s);
        }

        private void SetTempo(string s) {
            s = s.Trim();

            if ((!s.Contains("=") || s[0] == 'C') && strict)
                throw new ABCStrictException("Error setting tempo, must be in the form of 'x/x = nnn' when using strict mode.");

            Match bpmMatch;
            double length = 0;

            if (!s.Contains("=")) {
                bpmMatch = Regex.Match(s, @"\d+", RegexOptions.IgnoreCase);
                if (!bpmMatch.Success)
                    return;

                length = 0.25;
            }
            else if (s[0] == 'C') {
                bpmMatch = Regex.Match(s.Substring(s.IndexOf('=')), @"\d+", RegexOptions.IgnoreCase);
                if (!bpmMatch.Success)
                    return;

                length = 0.25;
            }
            else {
                MatchCollection matches;

                matches = Regex.Matches(s, @"""[^""]*""", RegexOptions.IgnoreCase);
                for (int i = 0; i < matches.Count; ++i )
                    s = s.Replace(matches[i].Value, "");

                bool leftSideBpm = false;

                matches = Regex.Matches(s, @"\d+/\d+", RegexOptions.IgnoreCase);
                for (int i = 0; i < matches.Count; ++i ){
                    length += GetNoteLength(matches[i].Value);
                    if (s.IndexOf(matches[i].Value) > s.IndexOf('='))
                        leftSideBpm = true;
                }

                string bpmStr;
                if (leftSideBpm) {
                    bpmStr = s.Substring(0, s.IndexOf('='));
                }
                else {
                    bpmStr = s.Substring(s.IndexOf('='));
                }

                bpmMatch = Regex.Match(bpmStr, @"\d+", RegexOptions.IgnoreCase);
                if (!bpmMatch.Success)
                    return;
            }

            double bpm = Convert.ToDouble(bpmMatch.Value);
            var divisor = Math.Min(settings.MaxTempo * 0.25, Math.Max(settings.MinTempo * 0.25, bpm * length));
            spm = 60d / divisor;
        }

        private double GetNoteLength(string s) {
            Match match = Regex.Match(s, @"\d+/\d+", RegexOptions.IgnoreCase);

            if (!match.Success)
                return -1;

            string[] numbers = match.Value.Split('/');
            var len = Convert.ToDouble(numbers[0]) / Convert.ToDouble(numbers[1]);
            return Math.Min(settings.LongestNote, Math.Max(settings.ShortestNote, len));
        }

        public override void Play(TimeSpan currentTime) {
            Play(currentTime, 1);
        }

        public virtual void Play(TimeSpan currentTime, int track) {
            if (tunes == null || tunes.Count < 2)
                return;

            selectedTune = track;
            if (selectedTune == 0) {
                selectedTune = 1;
            }

            base.Play(currentTime);
            SetDefaultValues();
            nextNote = lastTime;
            SetHeaderValues();
            SetHeaderValues(selectedTune, true);
            StartMeasure();
            Update(lastTime);
        }

        protected virtual void StartMeasure() {
            accidentals.Clear();
        }

        public override void Update(TimeSpan currentTime) {
            if (tokens != null){
                while (currentTime >= nextNote && tokenIndex < tokens.Count){
                    ReadNextNote();
                }

                if (currentTime >= nextNote && tokenIndex >= tokens.Count)
                    Stop();
            }
            
            base.Update(currentTime);
        }

        private bool IsTiedNote(int _tokenIndex) {
            return _tokenIndex + 1 < tokens.Count && tokens[_tokenIndex + 1][0] == '-';
        }

        private bool IsPlayableNote(string s) {
            switch (s[0]) {
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'g':
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                case 'G':
                    return true;
                case '^':
                case '=':
                case '_':
                    Note note = GetNote(s);
                    return note.Type >= 97 && note.Type <= 103;
            }
            return false;
        }

        private void ReadNextNote() {
            bool noteFound = false;
            bool chord = false;
            List<ABCNote> chordNotes = new List<ABCNote>();

            while (!noteFound && tokenIndex < tokens.Count) {
                string token = tokens[tokenIndex];

                char c = token[0];
                if (c == '[' && token == "[")
                    c = '!';

                switch (c) {
                    case ']':
                        if (chord) {
                            noteFound = true;
                            chord = false;
                            var chordLen = GetChord(chordNotes);
                            nextNote += chordLen;
                            PlayChord(chordNotes, nextNote);
                        }
                        break;
                    case '!': // replacement for chord opener
                        chord = true;
                        chordNotes.Clear();
                        break;
                    case '|':
                    case ':':
                    case '[': // TODO: repeats (if repeats are allowed)
                        if (c == '[' && token.EndsWith("]") && token.Length > 2 && token[2] == ':' && token[1] != '|' && token[1] != ':')
                            InlineInfo(token);
                        else
                            StartMeasure();
                        break;
                    case '+': // dynamics
                        GetDynamics(token);
                        break;
                    case 'z':
                    case 'Z':
                    case 'x':
                        Note rest = GetRest(token);
                        if (!chord) {
                            nextNote += rest.Length;
                            noteFound = true;
                        }
                        else
                            chordNotes.Add(new ABCNote(rest, tokenIndex));
                        break;
                    case 'a':
                    case 'b':
                    case 'c':
                    case 'd':
                    case 'e':
                    case 'f':
                    case 'g':
                    case 'A':
                    case 'B':
                    case 'C':
                    case 'D':
                    case 'E':
                    case 'F':
                    case 'G':
                    case '^':
                    case '=':
                    case '_':
                        Note note = GetNote(token);
                        if (!chord) {
                            nextNote += note.Length;
                            var tied = TieNote(new ABCNote(note, tokenIndex));
                            if (tied.Type != 'r') {
                                ValidateAndPlayNote(tied, 0);
                            }
                            noteFound = true;
                        }
                        else {
                            chordNotes.Add(new ABCNote(note, tokenIndex));
                        }

                        /*if (!tying) {
                            if (!chord) {
                                if (tokenIndex + 1 < tokens.Count && tokens[tokenIndex + 1][0] == '-') {
                                    heldNote = note;
                                    tying = true;
                                }
                                else {
                                    noteFound = true;
                                    ValidateAndPlayNote(note, 0);
                                    nextNote += note.Length;
                                }
                            }
                            else
                                chordNotes.Add(note);
                        }
                        else {
                            if (note.Type == heldNote.Type) {
                                heldNote.Length += note.Length;
                                if (!(tokenIndex + 1 < tokens.Count && tokens[tokenIndex + 1][0] == '-')) {
                                    noteFound = true;
                                    tying = false;
                                    ValidateAndPlayNote(heldNote, 0);
                                    nextNote += heldNote.Length;
                                }
                            }
                            else {
                                ValidateAndPlayNote(heldNote, 0);
                                nextNote += heldNote.Length;
                                noteFound = true;
                                tokenIndex--;
                            }
                        }*/
                        break;
                }

                tokenIndex++;
            }
        }

        private void InlineInfo(string s) {
            s = s.Substring(1, s.Length - 2).Trim();
            ABCInfo? info = ABCInfo.Parse(s);

            if (info.HasValue) {
                if (info.Value.Identifier == 'Q')
                    SetTempo(info.Value.Text);
                else if (info.Value.Identifier == 'L')
                    noteLength = GetNoteLength(info.Value.Text);
                else if (info.Value.Identifier == 'K')
                    GetKey(info.Value.Text);
            }
        }

        private void PlayChord(List<ABCNote> notes, TimeSpan time) {
            List<Note> chord = new List<Note>(notes.Count);
            for (int i = 0; i < notes.Count; ++i){
                var tied = TieNote(notes[i]);
                if (tied.Type != 'r'){
                    chord.Add(tied);
                }
            }
            PlayChord(chord, time);
        }

        protected virtual void PlayChord(List<Note> notes, TimeSpan time) {
            for (int i = 0; i < notes.Count; ++i){
                ValidateAndPlayNote(notes[i], i + 1);
            }
        }

        protected virtual Note TieNote(ABCNote note) {
            var key = note.BaseNote.Type.ToString(System.Globalization.CultureInfo.InvariantCulture) + note.BaseNote.Octave.ToString(System.Globalization.CultureInfo.InvariantCulture);
            int count;

            if (tiedNotes.TryGetValue(key, out count)) {
                if (count > 0) {
                    tiedNotes[key]--;
                    note.BaseNote.Type = 'r';
                    return note.BaseNote;
                }
            }

            if (IsTiedNote(note.TokenIndex)) {
                int nextIndex = note.TokenIndex + 1;

                while (nextIndex < tokens.Count) {
                    if (IsPlayableNote(tokens[nextIndex])) {
                        Note potential = GetNote(tokens[nextIndex]);
                        if (potential.Type == note.BaseNote.Type && potential.Octave == note.BaseNote.Octave) {
                            if (tiedNotes.ContainsKey(key)) {
                                tiedNotes[key]++;
                            }
                            else {
                                tiedNotes[key] = 1;
                            }
                            note.BaseNote.Length += potential.Length;
                            if (!IsTiedNote(nextIndex)) {
                                break;
                            }
                        }
                    }

                    nextIndex++;
                }
            }

            return note.BaseNote;
        }

        protected virtual void ValidateAndPlayNote(Note note, int channel) {
            if (note.Octave < settings.MinOctave)
                note.Octave = settings.MinOctave;
            else if (note.Octave > settings.MaxOctave)
                note.Octave = settings.MaxOctave;
            note.Volume = Math.Max(0f, Math.Min(note.Volume, 1f));
            if (!Muted)
                PlayNote(note, channel, nextNote);
        }

        private Note GetRest(string s) {
            s = s.Trim();
            Note note = new Note();
            note.Type = 'r';

            if (s[0] != 'Z') {
                note.Length = new TimeSpan((long)(spm * ModifyNoteLength(s) * TimeSpan.TicksPerSecond)); //TimeSpan.FromSeconds(spm * ModifyNoteLength(s));
            }
            else {
                Match match = Regex.Match(s, @"\d+");
                double measures = 1;
                if (match.Success && match.Value.Length > 0)
                    measures = Convert.ToDouble(match.Value);
                if (measures <= 0)
                    measures = 1;
                note.Length = new TimeSpan((long)(spm * measures * TimeSpan.TicksPerSecond)); //TimeSpan.FromSeconds(spm * measures);
            }

            return note;
        }

        private Note GetNote(string s) {
            s = s.Trim();

            int? acc = null;

            Match match;
            match = Regex.Match(s, @"\^+", RegexOptions.IgnoreCase);
            if (match.Success)
                acc = match.Value.Length;
            match = Regex.Match(s, @"_+", RegexOptions.IgnoreCase);
            if (match.Success)
                acc = -match.Value.Length;
            match = Regex.Match(s, @"=+", RegexOptions.IgnoreCase);
            if (match.Success)
                acc = 0;

            int noteOctave = this.octave;

            for (int i  = 0; i < s.Length; ++i){
                if (s[i] == ',')
                    noteOctave--;
                else if (s[i] == '\'')
                    noteOctave++;
            }

            if (lotroCompatible)
            {
                noteOctave--; // hack to force LOTRO compatibility when a LOTRO song is detected
                // LOTRO plays songs one octave lower than the ABC spec intends
            }

            string tone = Regex.Match(s, @"[a-g]", RegexOptions.IgnoreCase).Value;
            if (tone.ToLowerInvariant() == tone) // is lower case
                noteOctave++;

            string accName = tone.ToUpperInvariant(); // key to use in the accidentals dictionary
            char keyAccName = tone.ToUpperInvariant()[0]; // key to use in the defaultAccidentals dictionary (specified by key)
            if (accidentalPropagation == AccidentalPropagation.Octave) {
                accName += noteOctave;
            }

            if (acc.HasValue && accidentalPropagation != AccidentalPropagation.Not)
                accidentals[accName] = acc.Value;

            int steps = 0;

            if (defaultAccidentals.ContainsKey(keyAccName))
                steps = defaultAccidentals[keyAccName];

            if (accidentals.ContainsKey(accName))
                steps = accidentals[accName];

            Note note = new Note();
            note.Type = tone.ToLowerInvariant()[0];
            note.Octave = noteOctave;
            note.Volume = (float)volume;

            Step(ref note, steps);

            if (note.Octave < settings.MinOctave)
                note.Octave = settings.MinOctave;
            else if (note.Octave > settings.MaxOctave)
                note.Octave = settings.MaxOctave;

            note.Length = new TimeSpan((long)(spm * ModifyNoteLength(s) * TimeSpan.TicksPerSecond)); //TimeSpan.FromSeconds(spm * ModifyNoteLength(s));

            return note;
        }

        private double ModifyNoteLength(string s) {
            bool div = false;
            string num = "";
            double l = 1;
            for (int i = 0; i < s.Length; i++){
                if ((int)s[i] >= 48 && (int)s[i] <= 57)
                    num += s[i];
                else if (s[i] == '/'){
                    if (!div && !IsNullOrWhiteSpace.String(num))
                        l = Convert.ToDouble(num);
                    else if (div && !IsNullOrWhiteSpace.String(num))
                        l /= Convert.ToDouble(num);
                    else if (div)
                        l /= 2;
                    num = "";
                    div = true;
                }
            }

            if (l == 0)
                l = 1;

            if (num == "" && div) {
                num = "2";
            }

            if (num != "") {
                double n = Convert.ToDouble(num);
                if (n > 0) {
                    if (div)
                        l /= n;
                    else
                        l *= n;
                }
                else {
                    l = 1;
                }
            }

            return noteLength * l;
        }

        public override void Load(string str) {
            if (str.Length > validationSettings.MaxSize) {
                throw new SongSizeException("Song exceeded maximum length of " + validationSettings.MaxSize);
            }

            tunes = new Dictionary<int, Tune>();
            tunes.Add(0, new Tune());

            using (var stream = new StringReader(str)) {
                string line = stream.ReadLine();
                if (line == null)
                    return;
                if (!line.StartsWith("%abc")) {
                    if (strict) {
                        throw new ABCStrictException("Error reading ABC notation, file didn't start with '%abc'.");
                    }
                }
                else {
                    if (line.Length < 6 && strict)
                        throw new ABCStrictException("Error reading ABC notation, file lacks version information.");

                    if (line.Length >= 6)
                        version = line.Substring(5, line.Length - 5);

                    if (version != null){
                        string[] majorMinor = version.Split('.');

                        versionMajor = Convert.ToInt32(majorMinor[0]);
                        versionMinor = Convert.ToInt32(majorMinor[1]);

                        if ((versionMajor < 2 || (versionMajor == 2 && versionMinor < 1)) && strict){
                            throw new ABCStrictException(
                                "Error reading ABC notation, strict mode does not allow for versions lower than 2.1, version was " +
                                version + ".");
                        }
                    }
                }

                while (line != null) {
                    if (line != null)
                        Interpret(line);
                    line = stream.ReadLine();
                }

                ParseTune("");
            }

            foreach (var kvp in tunes) {
                if (kvp.Key > 0) {
                    selectedTune = kvp.Key;
                    if (tokens != null && tokens.Count > 0) {
                        CalculateDuration(kvp.Value);
                    }
                }
            }

            selectedTune = 1;
            SetDefaultValues();
        }

        protected virtual void CalculateDuration(Tune tune) {
            SetDefaultValues();
            SetHeaderValues();
            SetHeaderValues(selectedTune, true);

            TimeSpan dur = TimeSpan.Zero;
            bool chord = false;
            List<ABCNote> chordNotes = new List<ABCNote>();

            while (tokenIndex < tokens.Count) {
                string token = tokens[tokenIndex];

                char c = token[0];
                if (c == '[' && token == "[")
                    c = '!';

                switch (c) {
                    case ']':
                        if (chord) {
                            chord = false;
                            var chordLen = GetChord(chordNotes);
                            dur += chordLen;
                        }
                        break;
                    case '!': // replacement for chord opener
                        chord = true;
                        chordNotes.Clear();
                        break;
                    case '|':
                    case ':':
                    case '[':
                        if (c == '[' && token.EndsWith("]") && token.Length > 2 && token[2] == ':' && token[1] != '|' && token[1] != ':')
                            InlineInfo(token);
                        break;
                    case 'z':
                    case 'Z':
                    case 'x':
                        Note rest = GetRest(token);
                        if (!chord) {
                            dur += rest.Length;
                        }
                        else {
                            chordNotes.Add(new ABCNote(rest, tokenIndex));
                        }
                        break;
                    case 'a':
                    case 'b':
                    case 'c':
                    case 'd':
                    case 'e':
                    case 'f':
                    case 'g':
                    case 'A':
                    case 'B':
                    case 'C':
                    case 'D':
                    case 'E':
                    case 'F':
                    case 'G':
                    case '^':
                    case '=':
                    case '_':
                        Note note = GetNote(token);
                        if (!chord) {
                            dur += note.Length;
                        }
                        else {
                            chordNotes.Add(new ABCNote(note, tokenIndex));
                        }
                        break;
                }

                tokenIndex++;

                if (dur > settings.MaxDuration) {
                    throw new SongDurationException("Song exceeded maximum duration " + settings.MaxDuration);
                }
            }

            tunes[selectedTune].Duration = dur;
        }

        private void GetDynamics(string token) {
            if (token.Length > 1) {
                string dynamicsText = token.Substring(1, token.Length - 2);
                if (dynamicsText == "ppp" || dynamicsText == "pppp")
                    volume = 30.0 / 127;
                else if (dynamicsText == "pp")
                    volume = 45.0 / 127;
                else if (dynamicsText == "p")
                    volume = 60.0 / 127;
                else if (dynamicsText == "mp")
                    volume = 75.0 / 127;
                else if (dynamicsText == "mf")
                    volume = 90.0 / 127;
                else if (dynamicsText == "f")
                    volume = 105.0 / 127;
                else if (dynamicsText == "ff")
                    volume = 120.0 / 127;
                else if (dynamicsText == "fff" || dynamicsText == "ffff")
                    volume = 127.0 / 127;
            }
        }

        private TimeSpan GetChord(List<ABCNote> chordNotes) {
            if (chordNotes.Count > 0) {
                TimeSpan minLength = TimeSpan.MaxValue;
                for (int i = chordNotes.Count - 1; i >= 0; i--) {
                    var cnote = chordNotes[i];
                    minLength = new TimeSpan((long)(Math.Min(minLength.TotalSeconds, cnote.BaseNote.Length.TotalSeconds) * TimeSpan.TicksPerSecond));
                    if (cnote.BaseNote.Type == 'r') {
                        chordNotes.RemoveAt(i);
                    }
                }

                if (minLength == TimeSpan.MaxValue)
                    minLength = TimeSpan.Zero;

                if (chordNotes.Count > settings.MaxChordNotes) {
                    chordNotes.RemoveRange(settings.MaxChordNotes, chordNotes.Count - settings.MaxChordNotes);
                }

                return minLength;
            }
            return TimeSpan.Zero;
        }

        private void Interpret(string rawLine) {
            if (AutoDetectLotro && LotroAutoDetect.IsLotroMarker(rawLine)) {
                lotroCompatible = true;
            }

            // remove comments
            string line = rawLine.Split('%')[0].Trim();

            if (!inTune) {
                ParseHeader(line);
            }
            else {
                if (!(IsNullOrWhiteSpace.String(line) && rawLine != line)) { // skip commented empty lines so they dont end tunes
                    if (!(!strict && IsNullOrWhiteSpace.String(line) && (tunes[tunes.Count - 1].RawCode == null || tunes[tunes.Count - 1].RawCode.Length == 0))) {
                        ParseTune(line);
                    }
                }
            }
        }

        private void ParseHeader(string line) {
            Tune tune = tunes[tunes.Count - 1];

            // this does not handle new global information after the first tune properly
            ABCInfo? i = ABCInfo.Parse(line);
            if (i.HasValue) {
                ABCInfo info = i.Value;
                if (info.Identifier == 'T' && strict) {
                    if (tune.Header.Information.Count != 1 || !(tune.Header.Information.Count > 0 && tune.Header.Information.ContainsKey('X')))
                        throw new ABCStrictException("Error reading ABC notation, 'T:title' information field is only allowed after 'X:number' field in strict mode.");
                }

                if (info.Identifier == 'X') {
                    // start new tune
                    tune = new Tune();
                    tunes.Add(tunes.Count, tune);
                }
                else if (info.Identifier == 'K') {
                    // start interpreting notes
                    inTune = true;
                }

                tune.Header.AddInfo(i.Value);
            }
        }

        private void ParseTune(string line) {
            const int kDefaultTuneLength = 1024;
            Tune tune = tunes[tunes.Count - 1];

            if (tune.RawCode == null){
                tune.RawCode = new StringBuilder(kDefaultTuneLength);
            }

            if (!IsNullOrWhiteSpace.String(line)) {
                char c = line.Trim()[0];

                // add custom tokens for inlined stuff
                if (c == 'K' || c == 'L' || c == 'Q') {
                    tune.RawCode.Append("[").Append(line.Trim()).Append("]");
                }
                else if (!(c == 'I' || c == 'M' || c == 'm' || c == 'N' || c == 'O' || c == 'P' || c == 'R' || c == 'r' || c == 's' ||
                    c == 'T' || c == 'U' || c == 'V' || c == 'W' || c == 'w'))
                    tune.RawCode.Append(line);
            }
            else {

                // strip code of all stuff we don't care about
                StringBuilder newCode = new StringBuilder(kDefaultTuneLength);
                List<char> filteredChars = new List<char>() {
                    '\\', '\n', '\r', '\t'
                };

                if (tune.RawCode.Length == 0) {
                    tune.Tokens = new List<string>();
                    return;
                }

                for (int i = 0; i < tune.RawCode.Length; ++i){
                    if (!filteredChars.Contains(tune.RawCode[i]))
                        newCode.Append(tune.RawCode[i]);
                }

                tune.Tokens = Tokenize(newCode);
            }
        }

        private List<string> Tokenize(StringBuilder code) {
            List<char> tokenStarters = new List<char>() {
                '|', ':',
                '[', '{', ']', '}',
                'z', 'x', 'Z',
                'A', 'B', 'C', 'D', 'E', 'F', 'G',
                'a', 'b', 'c', 'd', 'e', 'f', 'g',
                '_', '=', '^',
                '<', '>', '(',
                ' ', '-', '"', '+'
            };
            List<char> tokenNotes = new List<char>() {
                'A', 'B', 'C', 'D', 'E', 'F', 'G',
                'a', 'b', 'c', 'd', 'e', 'f', 'g',
            };
            List<char> tokenBars = new List<char>() {
                '|', ':', '[', ']',
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
            };
            List<char> tokenTuplets = new List<char>() {
                '(', ':',
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
            };
            List<char> tokenInline = new List<char>() {
                'I', 'K', 'L', 'M', 'm', 'N', 'P', 'Q',
                'R', 'r', 's', 'T', 'U', 'V', 'W', 'w'
            };

            List<string> firstPass = new List<string>();
            StringBuilder curTokenText = new StringBuilder(code.Length);

            for (int j = 0; j < code.Length; ++j){
                if (tokenStarters.Contains(code[j])){
                    if (curTokenText.Length > 0)
                        firstPass.Add(curTokenText.ToString());
                    curTokenText.Length = 0;
                }
                curTokenText.Append(code[j]);
            }
            if (curTokenText.Length > 0)
                firstPass.Add(curTokenText.ToString());

            const int kInitialTokenSize = 10;
            List<string> tokens = new List<string>();
            StringBuilder curToken = new StringBuilder(kInitialTokenSize);

            for (int i = 0; i < firstPass.Count; ++i){
                curToken.Length = 0;

                if (firstPass[i][0] == '^'){
                    while (firstPass[i][0] == '^' || tokenNotes.Contains(firstPass[i][0])) {
                        curToken.Append(firstPass[i]);
                        if (tokenNotes.Contains(firstPass[i][0]))
                            break;
                        i++;
                        if (i >= firstPass.Count)
                            break;
                    }
                }
                else if (firstPass[i][0] == '+') {
                    curToken.Append(firstPass[i]);
                    i++;
                    while (i < firstPass.Count) {
                        curToken.Append(firstPass[i]);
                        if (firstPass[i][0] == '+')
                            break;
                        i++;
                        if (i >= firstPass.Count)
                            break;
                    }
                }
                else if (firstPass[i][0] == '_') {
                    while (firstPass[i][0] == '_' || tokenNotes.Contains(firstPass[i][0])) {
                        curToken.Append(firstPass[i]);
                        if (tokenNotes.Contains(firstPass[i][0]))
                            break;
                        i++;
                        if (i >= firstPass.Count)
                            break;
                    }
                }
                else if (firstPass[i][0] == '=') {
                    curToken.Length = 0;
                    curToken.Append("=");
                    while (firstPass[i][0] == '=' || tokenNotes.Contains(firstPass[i][0])) {
                        if (tokenNotes.Contains(firstPass[i][0])) {
                            curToken.Append(firstPass[i]);
                            break;
                        }
                        i++;
                        if (i >= firstPass.Count)
                            break;
                    }
                }
                else if (firstPass[i][0] == '[' &&
                            ((firstPass[i].Length > 1 && tokenInline.Contains(firstPass[i][1])) ||
                            (i < firstPass.Count - 1 && tokenInline.Contains(firstPass[i + 1][0])))) {
                    char? cmdChar = null;
                    if (firstPass[i].Length > 1)
                        cmdChar = firstPass[i][1];
                    else if (i < firstPass.Count - 1)
                        cmdChar = firstPass[i + 1][0];

                    if (cmdChar.HasValue) {
                        if (tokenInline.Contains(cmdChar.Value)) {
                            curToken.Length = 0;
                            curToken.Append(firstPass[i]);
                            i++;

                            while (curToken[curToken.Length-1] != ']') {
                                curToken.Append(firstPass[i]);
                                i++;
                                if (i >= firstPass.Count)
                                    break;
                            }
                            i--;
                        }
                    }
                }
                else if ((firstPass[i][0] == '[' && i < firstPass.Count - 1 && tokenBars.Contains(firstPass[i + 1][0]) && firstPass[i + 1][0] != ']') ||
                    (firstPass[i][0] == '|' || firstPass[i][0] == ':' ||
                    firstPass[i][0] == '0' || firstPass[i][0] == '1' || firstPass[i][0] == '2' || firstPass[i][0] == '3'
                     || firstPass[i][0] == '4' || firstPass[i][0] == '5' || firstPass[i][0] == '6' || firstPass[i][0] == '7'
                     || firstPass[i][0] == '8' || firstPass[i][0] == '9')) {
                    while (tokenBars.Contains(firstPass[i][0])) {
                        if (i > 0 && firstPass[i][0] == '[' && firstPass[i - 1][0] == '|')
                            break;
                        curToken.Append(firstPass[i]);
                        i++;
                        if (i >= firstPass.Count)
                            break;
                    }
                    i--;
                }
                else if (firstPass[i][0] == '(') {
                    while (tokenTuplets.Contains(firstPass[i][0])) {
                        curToken.Append(firstPass[i]);
                        i++;
                        if (i >= firstPass.Count)
                            break;
                    }
                    i--;
                }
                else if (firstPass[i][0] == '"') {
                    i++;
                    while (firstPass[i][0] != '"') {
                        i++;
                        if (i >= firstPass.Count)
                            break;
                    }
                }
                /*else if (firstPass[i][0] == '+') {
                    string text = firstPass[i].Trim();
                    if (text.Length == 1) {
                        curToken = null;
                    }
                    else {
                        curToken = text.ToLowerInvariant();
                    }
                }*/
                else {
                    curToken.Length = 0;
                    curToken.Append(firstPass[i]);
                }

                if (curToken.Length > 0)
                    tokens.Add(curToken.ToString());

            }

            return tokens;
        }

        public ABCSettings Settings { get { return settings; } set { settings = value; } }
        public double Volume { get { return volume; } }
        public override TimeSpan Duration { get { return duration; } }
        private List<string> tokens { get { return tunes[selectedTune].Tokens; } }
        private TimeSpan duration { 
            get 
            {
                try
                {
                    return tunes[selectedTune].Duration;
                }
                catch (Exception e)
                {
                    Console.Write(e.ToString());
                    return TimeSpan.Zero;
                }
             }         
         }

        internal override ValidationSettings validationSettings { get { return settings; } }
        /// <summary>
        /// If set to true LotroAutoDetect.IsLotroMarker is called with lines in ABC files, if this function returns
        /// true then the song is modified to play one octave lower than it should according to the ABC spec. For a
        /// full list of markers check LotroAutoDetect.LotroMarkers and LotroAutoDetect.MaestroFutureMarker.
        /// </summary>
        public bool AutoDetectLotro { get; set; }
        /// <summary>
        /// Indicates whether this song is in LOTRO compatible mode. If true all notes are played one octave lower.
        /// </summary>
        public bool LotroCompatible { get { return lotroCompatible; } set { lotroCompatible = value; } }
    }
}

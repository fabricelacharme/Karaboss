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

using System.Windows.Forms;

namespace Karaboss.Utilities
{
    public static class CreateNewMidiFile
    {
        //private static int _numerator;
        public static int _Numerator { get; set; }
        //private static int _denominator;
        public static int _Denominator { get; set; }
        //private static int _division;
        public static int _Division { get; set; }
        //private static int _tempo;
        public static int _Tempo { get; set; }
        //private static int _measures;
        public static int _Measures { get; set; }

        // Fab 27/10/2023
        public static string _TrackName { get; set; }
        public static int _ProgramChange { get; set; }
        public static int _Channel { get; set; }
        public static decimal _TrkIndex { get; set; }
        public static int _Clef { get; set; }
        public static string _InstrumentName { get; set; }

        /// <summary>
        /// Folder from which the creation was started in frmExplorer
        /// </summary>
        public static string _DefaultDirectory { get; set; }


        /// <summary>
        /// Create a new track
        /// </summary>
        /// <param name="defFolder"></param>
        public static bool New (string defFolder) 
        {
            int numerator = 4;
            int denominator = 4;
            int division = 480;
            int tempo = 500000;
            int measures = 35;

            // Display dialog windows new midi file
            DialogResult dr; // = new DialogResult();
            Sanford.Multimedia.Midi.Score.UI.frmNewMidiFile MidiFileDialog = new Sanford.Multimedia.Midi.Score.UI.frmNewMidiFile(numerator, denominator, division, tempo, measures);
            dr = MidiFileDialog.ShowDialog();

            if (dr == System.Windows.Forms.DialogResult.Cancel)
            {
                return false;
            }

            _Numerator = MidiFileDialog.Numerator;
            _Denominator = MidiFileDialog.Denominator;
            _Division = MidiFileDialog.Division;
            _Tempo = MidiFileDialog.Tempo;
            _Measures = MidiFileDialog.Measures;
            _DefaultDirectory = defFolder;


            // Test fab
            string trackname = "Track1";
            int programchange = 0;
            int channel = 0;
            decimal trkindex = 1;
            int clef = 0;

            //dr = new DialogResult();
            Sanford.Multimedia.Midi.Score.UI.frmNewTrackDialog TrackDialog = new Sanford.Multimedia.Midi.Score.UI.frmNewTrackDialog(trackname, programchange, channel, trkindex, clef);
            dr = TrackDialog.ShowDialog();

            // TODO : if we are creating a new file, 
            if (dr == DialogResult.Cancel)
                return false;

            _TrackName = TrackDialog.TrackName;
            _ProgramChange = TrackDialog.ProgramChange;
            _Channel = TrackDialog.MidiChannel;
            _TrkIndex = trkindex;
            _Clef = TrackDialog.cle;

            return true;
        }
    }
}

#region License

/* Copyright (c) 2006 Leslie Sanford
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
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Sanford.Multimedia.Midi
{
    /// <summary>
    /// Defintes constants representing SMPTE frame rates.
    /// </summary>
    public enum SmpteFrameRate
    {
        Smpte24     = 24,
        Smpte25     = 25,
        Smpte30Drop = 29,
        Smpte30     = 30
    }

    /// <summary>
    /// The different types of sequences.
    /// </summary>
    public enum SequenceType
    {
        Ppqn,
        Smpte
    }    

	/// <summary>
	/// Represents MIDI file properties.
	/// </summary>
	internal class MidiFileProperties
	{
        private const int PropertyLength = 2;

        private static readonly byte[] MidiFileHeader =
            {
                (byte)'M',
                (byte)'T',
                (byte)'h',
                (byte)'d',
                0, 
                0, 
                0,
                6
            };

        private int format = 1;

        private int trackCount = 0;

        private int division = PpqnClock.PpqnMinValue;

        private SequenceType sequenceType = SequenceType.Ppqn;
        
        // FAB
        private int tempo = 0;
        private int orig_format = 1;
        private int orig_tempo = 0;
        private int numerator = 0;
        private int denominator = 0;
        private int quarternote = 0;
        private bool splithands = true; // Split left hand & right hand for piano score

        private string log = string.Empty;


		public MidiFileProperties()
		{
		}

        public void ResetLog()
        {
            log = string.Empty;
        }

        public void AddLog(string tx)
        {
            log += tx + "\n";

        }

        public void Read(Stream strm)
        {
            #region Require

            if(strm == null)
            {
                throw new ArgumentNullException("strm");
            }

            #endregion

            format = trackCount = division = 0;
            // FAB
            tempo = orig_tempo = numerator = denominator = quarternote = 0;

            FindHeader(strm);

            int _format;
            int _trackcount;

            // FAB - 03/11/17 : correction fichier midi erron� : format = 0 et nombre tracks > 1 !!!
            //Format = (int)ReadProperty(strm);
            //TrackCount = (int)ReadProperty(strm);
            _format = (int)ReadProperty(strm);
            _trackcount = (int)ReadProperty(strm);
            if (_trackcount > 1)
                _format = 1;

            Format = _format;
            TrackCount = _trackcount;

            Division = (int)ReadProperty(strm);

            #region Invariant

            AssertValid();

            #endregion
        }

        private void FindHeader(Stream stream)
        {
            bool found = false;
            int result;

            while(!found)
            {
                result = stream.ReadByte();

                if(result == 'M')
                {
                    result = stream.ReadByte();

                    if(result == 'T')
                    {
                        result = stream.ReadByte();

                        if(result == 'h')
                        {
                            result = stream.ReadByte();

                            if(result == 'd')
                            {
                                found = true;
                            }
                        }
                    }
                }

                if(result < 0)
                {
                    throw new MidiFileException("Unable to find MIDI file header.");
                }
            }

            // Eat the header length.
            for(int i = 0; i < 4; i++)
            {
                if(stream.ReadByte() < 0)
                {
                    throw new MidiFileException("Unable to find MIDI file header.");
                }
            }
        }

        private ushort ReadProperty(Stream strm)
        {
            byte[] data = new byte[PropertyLength];

            int result = strm.Read(data, 0, data.Length);

            if(result != data.Length)
            {
                throw new MidiFileException("End of MIDI file unexpectedly reached.");
            }

            if(BitConverter.IsLittleEndian)
            {
                Array.Reverse(data);
            }

            return BitConverter.ToUInt16(data, 0);
        }

        public void Write(Stream strm)
        {
            #region Require

            if(strm == null)
            {
                throw new ArgumentNullException("strm");
            }

            #endregion

            strm.Write(MidiFileHeader, 0, MidiFileHeader.Length);
            WriteProperty(strm, (ushort)Format);
            WriteProperty(strm, (ushort)TrackCount);
            WriteProperty(strm, (ushort)Division);
        }

        private void WriteProperty(Stream strm, ushort property)
        {
            byte[] data = BitConverter.GetBytes(property);

            if(BitConverter.IsLittleEndian)
            {
                Array.Reverse(data);
            }

            strm.Write(data, 0, PropertyLength);
        }

        private static bool IsSmpte(int division)
        {
            bool result;
            byte[] data = BitConverter.GetBytes((short)division);
            
            if(BitConverter.IsLittleEndian)
            {
                Array.Reverse(data);
            }

            if((sbyte)data[0] < 0)
            {
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }

        [Conditional("DEBUG")]
        private void AssertValid()
        {
            if(trackCount > 1)
            {
                Debug.Assert(Format == 1 || Format == 2);
            }

            if(IsSmpte(Division))
            {
                Debug.Assert(SequenceType == SequenceType.Smpte);
            }
            else
            {
                Debug.Assert(SequenceType == SequenceType.Ppqn);
                Debug.Assert(Division % PpqnClock.PpqnMinValue == 0);
            }
        }

        public int Format
        {
            get
            {
                return format;
            }
            set
            {
                #region Require

                if(value < 0 || value > 3)
                {
                    throw new ArgumentOutOfRangeException("Format", value,
                        "MIDI file format out of range.");
                }
                else if(value == 0 && trackCount > 1)
                {
                    throw new ArgumentException(
                        "MIDI file format invalid for this track count.");
                }

                #endregion

                format = value;

                #region Invariant

                AssertValid();

                #endregion
            }
        }

        
        public int TrackCount
        {
            get
            {
                return trackCount;
            }
            set
            {
                #region Require

                if(value < 0)
                {
                    throw new ArgumentOutOfRangeException("TrackCount", value,
                        "Track count out of range.");
                }
                else if(value > 1 && Format == 0)
                {
                    //throw new ArgumentException("Track count invalid for this format.");
                    value = 1;
                }

                #endregion

                trackCount = value;

                #region Invariant

                AssertValid();

                #endregion
            }
        }

        public int Division
        {
            get
            {
                return division;
            }
            set
            {
                if(IsSmpte(value))
                {
                    byte[] data = BitConverter.GetBytes((short)value); 

                    if(BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(data);
                    }

                    if((sbyte)data[0] != -(int)SmpteFrameRate.Smpte24 &&                        
                        (sbyte)data[0] != -(int)SmpteFrameRate.Smpte25 &&
                        (sbyte)data[0] != -(int)SmpteFrameRate.Smpte30 &&
                        (sbyte)data[0] != -(int)SmpteFrameRate.Smpte30Drop)
                    {
                        throw new ArgumentException("Invalid SMPTE frame rate.");
                    }
                    else
                    {
                        sequenceType = SequenceType.Smpte;
                    }
                }
                else 
                {                    
                    if(value % PpqnClock.PpqnMinValue != 0)
                    {
                        // FAB: 06/07/2014 
                        value = (int)(Math.Round((double)value / PpqnClock.PpqnMinValue) * PpqnClock.PpqnMinValue);
                        Console.Write("\nERROR: Invalid pulses per quarter note value.\n");
                    }
                    else
                    {
                        sequenceType = SequenceType.Ppqn;
                    }
                }

                division = value;

                #region Invariant

                AssertValid();

                #endregion
            }
        }

        public SequenceType SequenceType
        {
            get
            {
                return sequenceType;
            }
        }

        // FAB
        public int Tempo
        {
            get
            { return tempo; }
            set
            { tempo = value; }
        }

        public int Orig_format
        {
            get { return orig_format; }
            set { orig_format = value; }
        }

        public int Orig_Tempo
        {
            get { return orig_tempo; }
            set { orig_tempo = value; }
        }
        public int Numerator
        {
            get
            { return numerator; }
            set
            { numerator = value; }
        }
        public int Denominator
        {
            get
            { return denominator; }
            set
            { denominator = value; }
        }
        public int Quarternote
        {
            get
            { return quarternote; }
            set
            { quarternote = value; }
        }

        // Split left hand & right hand for piano score
        public bool SplitHands
        {
            get { return splithands; }
            set { splithands = value; }
        }

        /// <summary>
        /// Text encoding ascii or other 
        /// </summary>
        public string TextEncoding { get; set; }

        public string Log
        {
            get { return log; }
            
        }

    }

    public class MidiFileException : ApplicationException
    {
        public MidiFileException(string message) : base(message)
        {
            MessageBox.Show(message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
    }
}

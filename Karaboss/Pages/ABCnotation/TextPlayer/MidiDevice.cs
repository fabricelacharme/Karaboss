#region License
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
//using Midi;
using Sanford.Multimedia.Midi;

namespace Karaboss.Pages.ABCnotation
{
    public struct NoteTimeOut {
        public TimeSpan End;
        public MyMidi.Pitch Pitch;        
        public MyMidi.Channel Channel;
        public int Velocity;
    }

    public class MidiDevice {
        //private Midi.OutputDevice outputDevice;
        private Sanford.Multimedia.Midi.OutputDevice outDevice;
        private List<NoteTimeOut> timeOuts;
        private bool muted;

        public MidiDevice(Sanford.Multimedia.Midi.OutputDevice optdev) 
        {
            try
            {
                timeOuts = new List<NoteTimeOut>();
                //outputDevice = Midi.OutputDevice.InstalledDevices[1];
                outDevice = optdev;

                
                //if (! outputDevice.IsOpen)
                //    outputDevice.Open();
                //outputDevice.SilenceAllNotes();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            System.Threading.Thread.Sleep(200); // fixes delay during initial playing, possibly due to midi device initialization
        }

        public void SetInstrument(MyMidi.Instrument instrument) {

            try
            {
                foreach (var c in Enum.GetValues(typeof(MyMidi.Channel)))
                {
                    //outputDevice.SendProgramChange((Midi.Channel)c, instrument);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        public void Close() {
            try
            {
                //outputDevice.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            System.Threading.Thread.Sleep(200); // fixes delay during initial playing, possibly due to midi device initialization
        }

        public void StopNotes() {
            timeOuts = new List<NoteTimeOut>();
            try
            {
                //outputDevice.SilenceAllNotes();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected void NoteOn(MyMidi.Channel channel, MyMidi.Pitch pitch, int velocity) {
            if (muted)
                return;
            try
            {
                outDevice.Send(new Sanford.Multimedia.Midi.ChannelMessage(ChannelCommand.NoteOn, 0, (int)pitch, velocity));
                
                //outputDevice.SendNoteOn(channel, pitch, velocity);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void PlayNote(int track, TextPlayer.Note note, TimeSpan end) {
            PlayNote(
                TrackToChannel(track),
                NoteToPitch(note),
                NoteToVelocity(note),
                end);
        }

        public void PlayNote(MyMidi.Channel channel, MyMidi.Pitch pitch, int velocity, TimeSpan end) {
            NoteOn(channel, pitch, velocity);

            var timeOut = new NoteTimeOut() {
                Channel = channel,
                End = end - TimeSpan.FromMilliseconds(10), // subtract 10 ms to prevent errors with turning off notes
                Pitch = pitch,
                Velocity = velocity
            };
            timeOuts.Add(timeOut);

        }

        private MyMidi.Channel TrackToChannel(int track) {
            if (track >= 10) // skip percussion track
                track++;
            return (MyMidi.Channel)Enum.Parse(typeof(MyMidi.Channel), "Channel" + (track + 1), false);
        }

        private MyMidi.Pitch NoteToPitch(TextPlayer.Note note) {
            string type = note.Type.ToString().ToUpperInvariant();
            type += note.Sharp ? "Sharp" : "";
            type += note.Octave;
            return (MyMidi.Pitch)Enum.Parse(typeof(MyMidi.Pitch), type);
        }

        private int NoteToVelocity(TextPlayer.Note note) {
            return (int)(note.Volume * 127);
        }

        private int ChannelToIndex(MyMidi.Channel channel) {
            string s = channel.ToString();
            s = s.Replace("Channel", "");
            int index = Convert.ToInt32(s) - 1;
            if (index >= 11) // we skip channel 10 so subtract one to get the track number
                index--;
            return index;
        }

        public void HandleTimeOuts(TimeSpan elapsed) {
            for (int i = timeOuts.Count - 1; i >= 0; i--) {
                var timeOut = timeOuts[i];
                if (elapsed >= timeOut.End) 
                {
                    outDevice.Send(new Sanford.Multimedia.Midi.ChannelMessage(ChannelCommand.NoteOff, 0, (int)timeOut.Pitch, timeOut.Velocity));
                    //outputDevice.SendNoteOff(timeOut.Channel, timeOut.Pitch, timeOut.Velocity);

                    timeOuts.RemoveAt(i);
                }
            }
        }

        public List<NoteTimeOut> TimeOuts { get { return timeOuts; } }
        public bool Muted { get { return muted; } set { muted = value; } }

        //public Sanford.Multimedia.Midi.OutputDevice OutDevice { get { return outDevice; } set { outDevice = value; } }
    }
}

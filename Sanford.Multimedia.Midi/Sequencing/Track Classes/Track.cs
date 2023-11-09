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
using System.Collections.Generic; //Lists
using System.Diagnostics.Eventing.Reader;
using System.Windows.Markup;
using System.Linq.Expressions;
using System.Windows.Forms;
using System.Runtime.Remoting.Channels;
using System.Threading;

namespace Sanford.Multimedia.Midi
{
    /// <summary>
    /// Represents a collection of MidiEvents and a MIDI track within a 
    /// Sequence.
    /// </summary>
    public sealed partial class Track
    {
        #region Track Members

        #region Fields

        // The number of MidiEvents in the Track. Will always be at least 1
        // because the Track will always have an end of track message.
        private int count = 1;

        // The number of ticks to offset the end of track message.
        private int endOfTrackOffset = 0;

        // The first MidiEvent in the Track.
        private MidiEvent head = null;

        // The last MidiEvent in the Track, not including the end of track
        // message.
        private MidiEvent tail = null;

        // The end of track MIDI event.
        private MidiEvent endOfTrackMidiEvent;

        private List<MidiNote> notes;

        #endregion

        #region Construction

        public Track()
        {
            endOfTrackMidiEvent = new MidiEvent(this, Length, MetaMessage.EndOfTrackMessage);
            notes = new List<MidiNote>();
        }

        #endregion

        #region Methods

        #region notes management
        /// <summary>
        /// Clean dirty stuff
        /// Remove notes having null duration and starttime too much
        /// </summary>
        public void checkNotes()
        {
            // Remove all notes whose duration is null 
            MidiNote m = notes.Find(u => u.Duration == 0);
            while (m != null)
            {
                Notes.Remove(m);
                m = notes.Find(u => u.Duration == 0);
            }

            // Remove all notes whose starttime is too much
            m = notes.Find(u => u.StartTime > 10000000);
            while (m != null)
            {
                Notes.Remove(m);
                m = notes.Find(u => u.StartTime > 1000000);
            }
        }
   
        public void deleteMidiEventsAfter(int after)
        {
            // supprime tous les messages text & lyric
            bool oneMoreTime = true;
            while (oneMoreTime)
            {
                int toDelete = -1;
                oneMoreTime = false;
                int id = -1;

                foreach (MidiEvent a in Iterator())
                {
                    id++;

                    if (id != Count - 1 && a.AbsoluteTicks > after)
                    {
                        toDelete = id;
                        break;
                    }
                }
                if (toDelete != -1)
                {
                    RemoveAt(toDelete);
                    oneMoreTime = true;
                }
            }
        }
       

        // First note of the track
        public MidiNote GetFirstNote()
        {
            return notes.Count == 0 ? null : notes[0];
        }


        /// <summary>
        /// FAB: return previous note on the same track
        /// </summary>
        /// <param name="number"></param>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public MidiNote getPrevNote(int number, int ticks)
        {
            // Normally, notes are sorted by start time
            int nearestnote = 0;
            int i = -1;

            // Plusieurs notes au même temps ?
            for (int n = notes.Count - 1; n >= 0; n--)
            {
                MidiNote item = notes[n];
                if (item.StartTime == ticks)
                {
                    // prend la note la plus proche
                    if (item.Number < number && item.Number > nearestnote)
                    {
                        nearestnote = item.Number;
                        i = n;
                    }
                }
                // On passe au ticks inférieurs sans avoir trouvé de notes ayant le même tick
                else if (item.StartTime < ticks && i == -1)
                {
                    i = n;
                    break;
                }
            }
            if (i == -1)
                return null;
            else
                return notes[i];
        }

        /// <summary>
        /// FAB: return next note on the same track
        /// </summary>
        /// <param name="number"></param>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public MidiNote getNextNote(int number, int ticks)
        {
            // Normally, notes are sorted by start time
            int nearestnote = 127;
            int i = -1;

            // Plusieurs notes au même temps ?
            for (int n = 0; n < notes.Count; n++)
            {
                MidiNote item = notes[n];

                // Notes ayant le même ticks
                if (item.StartTime == ticks)
                {
                    // prend la note au dessus la plus proche 
                    if (item.Number > number && item.Number < nearestnote)
                    {
                        nearestnote = item.Number;
                        i = n;
                    }
                }
                // On passe aux ticks supérieurs sans avoir trouvé de notes ayant le même ticks
                else if (item.StartTime > ticks && i == -1)
                {
                    i = n;
                    break;
                }
            }
            if (i == -1)
                return null;
            else
                return notes[i];
        }

        /// <summary>
        /// Add a note in the track
        /// </summary>
        /// <param name="note"></param>
        public int addNote(MidiNote note, bool bCheckDistance = true)
        {

            // FAb 30/10/2023
            /*
            if (note.Duration < 10)
                return 0;
            */

            // Do not add if exists already
            if (findMidiNote(note.Number, note.StartTime) != null)
                return 0;


            // check if previous note and after note is far enough
            if (bCheckDistance == true)
            {
                // Check if next note is enough far away
                MidiNote nextnote = getNextNote(note.Number, note.StartTime);
                if (nextnote != null) // a note afters exists
                {
                    // There is a note after at same number
                    if (nextnote.Number == note.Number)
                    {
                        // Si le starttime de la prochaine note se produit avant que la nouvelle note s'arrête
                        if (nextnote.StartTime < note.StartTime + note.Duration)
                        {
                            return 0;
                        }
                    }
                }

                // Check if previous note is enough far away
                MidiNote prevnote = getPrevNote(note.Number, note.StartTime);
                if (prevnote != null)
                {
                    // There is a note before at the same number
                    if (prevnote.Number == note.Number)
                    {
                        if (prevnote.StartTime + prevnote.Duration > note.StartTime)
                        {
                            return 0;
                        }
                    }
                }
            }

            // Insert Note on            
            ChannelMessage message = new ChannelMessage(ChannelCommand.NoteOn, note.Channel, note.Number, note.Velocity);
            InsertLast(note.StartTime, message);

            // Insert Note off at ticksoff - 1 
            // to avoid what ? ovelapping with next note ?
            int ticksoff = note.StartTime + note.Duration;
            message = new ChannelMessage(ChannelCommand.NoteOff, note.Channel, note.Number, 0);            
            InsertLast(ticksoff - 1, message);

            // Add note to list of notes
            notes.Add(note);

            // sort list
            if (notes.Count > 1)
                notes.Sort(notes[0]);

            ContainsNotes = true;
            return 1;
        }

        /// <summary>
        /// FAB: delete a MidiNote, in the list Notes and in the Midi Events
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        public int deleteNote(int number, int ticks)
        {
            MidiNote note = findMidiNote(number, ticks);
            if (note == null)
                return -1;

            // Delete pitchBend ?
            //RemovePitchBend(note.Channel, note.StartTime, note.EndTime);

            bool bfound = false;
            // Search index of noteOn & noteOff
            int i = GetEventPositionFromTicks(note.Number, note.StartTime, note.Duration);
            while (i != -1)
            {
                bfound = true;
                // Remove midi event
                RemoveAt(i);
                // Remove in the list
                DelNoteFromTicks(note.Number, note.StartTime);
                i = GetEventPositionFromTicks(note.Number, note.StartTime, note.Duration);
            }
            if (bfound)
                return 0;
            else
                return -1;
        }

        public MidiNote findMidiNote(int number, int ticks)
        {
            MidiNote m = notes.Find(u => u.Number == number && u.StartTime == ticks);
            return m;
        }

        public MidiNote findPreviousMidiNote(int ticks)
        {
            MidiNote m;
            for (int i = notes.Count -1; i >= 0; i--)
            {
                m = notes[i];
                if (m.StartTime <= ticks) return m;
            }            
            return null;
        }

        private void DelNoteFromTicks(int number, int ticks)
        {
            // Remove all notes corresponding in list "Notes"
            bool oneMoreTime = true;
            while (oneMoreTime)
            {
                MidiNote toDelete = null;
                oneMoreTime = false;
                foreach (MidiNote item in notes)
                {
                    if (item.StartTime == ticks && item.Number == number)
                    {
                        toDelete = item;
                        break;
                    }
                }
                if (toDelete != null)
                {
                    notes.Remove(toDelete);
                    oneMoreTime = true;
                }
            }

            if (notes.Count == 0) containsnotes = false;
        }

        public void changeNoteNumber(MidiNote note, int newNumber, bool bCheckDistance)
        {
            deleteNote(note.Number, note.StartTime);
            note.Number = newNumber;

            addNote(note, bCheckDistance);
        }

        #endregion notes management

        #region tempo
        /// <summary>
        /// Find Tempo Message
        /// </summary>
        /// <returns></returns>
        private int findTempo()
        {
            int id = 0;
            MidiEvent current = GetMidiEvent(0);
           
            while (current.AbsoluteTicks <= Length)
            {
                IMidiMessage a = current.MidiMessage;

                if (a.MessageType == MessageType.Meta)
                {
                    MetaMessage Msg = (MetaMessage)current.MidiMessage;                    
                    if (Msg.MetaType == MetaType.Tempo)
                    {
                        return id;
                    }
                    else
                    {
                        #region next
                        if (current.Next != null)
                        {
                            current = current.Next;
                            id++;
                        }
                        else
                        {
                            break;
                        }
                        #endregion next                            
                    }
                }
                else
                {
                    #region next
                    if (current.Next != null)
                    {
                        current = current.Next;
                        id++;
                    }
                    else
                    {
                        break;
                    }
                    #endregion next 
                }
            }
            return -1;
        }

        /// <summary>
        /// Remove Tempo Message
        /// </summary>
        public void RemoveTempoEvent()
        {
            int i = findTempo();

            while (i != -1)
            {
                RemoveAt(i);
                i = findTempo();
            }

        }

        #endregion tempo

        #region tag
        private int findTag(string strtag)
        {
            int id = 0;
            string sy = string.Empty;
            MidiEvent current = GetMidiEvent(0);

            while (current.AbsoluteTicks == 0)
            {
                IMidiMessage a = current.MidiMessage;

                if (a.MessageType == MessageType.Meta)
                {
                    MetaMessage Msg = (MetaMessage)current.MidiMessage;

                    //MetaType type
                    if (Msg.MetaType == MetaType.Text)
                    {
                        sy = System.Text.Encoding.Default.GetString(Msg.GetBytes());

                        if (sy.Length > 1 && sy.Substring(0, strtag.Length) == strtag && current.AbsoluteTicks == 0)
                        {
                            return id;
                        }
                        else
                        {
                            #region next
                            if (current.Next != null)
                            {
                                current = current.Next;
                                id++;
                            }
                            else
                            {
                                break;
                            }
                            #endregion next                            
                        }
                    }
                    else
                    {
                        #region next
                        if (current.Next != null)
                        {
                            current = current.Next;
                            id++;
                        }
                        else
                        {
                            break;
                        }
                        #endregion next 
                    }
                }
                else
                {
                    #region next
                    if (current.Next != null)
                    {
                        current = current.Next;
                        id++;
                    }
                    else
                    {
                        break;
                    }
                    #endregion next                    
                }
            }

            return -1;
        }

        /// <summary>
        /// Remove all tags events
        /// </summary>
        /// <param name="tag"></param>
        public void RemoveTagsEvent(string tag)
        {
            int i = findTag(tag);
            while (i != -1)
            {
                RemoveAt(i);
                i = findTag(tag);
            }

        }

        #endregion tag

        #region volume

        // Find volume controller
        private int findVolume()
        {
            int id = 0;
            MidiEvent current = GetMidiEvent(0);

            while (current.AbsoluteTicks <= Length)
            {
                IMidiMessage a = current.MidiMessage;
                if (a.MessageType == MessageType.Channel)
                {
                    ChannelMessage Msg = (ChannelMessage)current.MidiMessage;
                    ChannelCommand cc = Msg.Command;

                    if (cc == ChannelCommand.Controller) {
                        ControllerType ct = (ControllerType)cc;
                        // FAB test 11 pour fade in & out
                        if (Msg.Data1 == 7) {
                            return id;
                        }
                        else
                        {
                            #region next
                            if (current.Next != null)
                            {
                                current = current.Next;
                                id++;
                            }
                            else
                            {
                                break;
                            }
                            #endregion next      
                        }
                    }
                    else
                    {
                        #region next
                        if (current.Next != null)
                        {
                            current = current.Next;
                            id++;
                        }
                        else
                        {
                            break;
                        }
                        #endregion next      
                    }
                }
                else
                {
                    #region next
                    if (current.Next != null)
                    {
                        current = current.Next;
                        id++;
                    }
                    else
                    {
                        break;
                    }
                    #endregion next      
                }
            }
            return -1;
        }

        /// <summary>
        /// Remove all Volume events
        /// </summary>
        public void RemoveVolume()
        {
            int i = findVolume();

            while (i != -1)
            {
                RemoveAt(i);
                i = findVolume();
            }
        }

        #endregion volume

        #region pan
        // Find pan controller
        private int findPan()
        {
            int id = 0;
            MidiEvent current = GetMidiEvent(0);

            while (current.AbsoluteTicks <= Length)
            {
                IMidiMessage a = current.MidiMessage;

                if (a.MessageType == MessageType.Channel)
                {
                    ChannelMessage Msg = (ChannelMessage)current.MidiMessage;
                    ChannelCommand cc = Msg.Command;

                    if (cc == ChannelCommand.Controller)
                    {
                        ControllerType ct = (ControllerType)cc;
                        // FAB test 11 pour fade in & out
                        if (Msg.Data1 == 10)
                        {
                            return id;
                        }
                        else
                        {
                            #region next
                            if (current.Next != null)
                            {
                                current = current.Next;
                                id++;
                            }
                            else
                            {
                                break;
                            }
                            #endregion next      
                        }
                    }
                    else
                    {
                        #region next
                        if (current.Next != null)
                        {
                            current = current.Next;
                            id++;
                        }
                        else
                        {
                            break;
                        }
                        #endregion next      
                    }
                }
                else
                {
                    #region next
                    if (current.Next != null)
                    {
                        current = current.Next;
                        id++;
                    }
                    else
                    {
                        break;
                    }
                    #endregion next      
                }
            }
            return -1;
        }

        /// <summary>
        /// Remove all Pan events
        /// </summary>
        /// <param name="cha"></param>
        /// <returns></returns>
        public void RemovePan()
        {
            int i = findPan();

            while (i != -1)
            {
                RemoveAt(i);
                i = findPan();
            }
        }
        #endregion pan

        #region reverb
        // Find reverb controller
        private int findReverb()
        {
            int id = 0;
            MidiEvent current = GetMidiEvent(0);

            while (current.AbsoluteTicks <= Length)
            {
                IMidiMessage a = current.MidiMessage;

                if (a.MessageType == MessageType.Channel)
                {
                    ChannelMessage Msg = (ChannelMessage)current.MidiMessage;
                    ChannelCommand cc = Msg.Command;

                    if (cc == ChannelCommand.Controller)
                    {
                        ControllerType ct = (ControllerType)cc;
                        // FAB test 11 pour fade in & out
                        if (Msg.Data1 == 91)
                        {
                            return id;
                        }
                        else
                        {
                            #region next
                            if (current.Next != null)
                            {
                                current = current.Next;
                                id++;
                            }
                            else
                            {
                                break;
                            }
                            #endregion next      
                        }
                    }
                    else
                    {
                        #region next
                        if (current.Next != null)
                        {
                            current = current.Next;
                            id++;
                        }
                        else
                        {
                            break;
                        }
                        #endregion next      
                    }
                }
                else
                {
                    #region next
                    if (current.Next != null)
                    {
                        current = current.Next;
                        id++;
                    }
                    else
                    {
                        break;
                    }
                    #endregion next      
                }
            }
            return -1;
        }

        /// <summary>
        /// Remove all reverb events
        /// </summary>
        /// <param name="cha"></param>
        /// <returns></returns>
        public void RemoveReverb()
        {
            int i = findReverb();

            while (i != -1)
            {
                RemoveAt(i);
                i = findReverb();
            }
        }

        #endregion reverb


        /// <summary>
        /// Extract notes for this track
        /// Result: track.Notes (List of MidiNotes)
        /// </summary>
        public void ExtractNotes()
        {
            //notes.Clear();
            List<MidiNote> lsnotes = new List<MidiNote>();
            
            MidiEvent current = GetMidiEvent(0);

            while (current.AbsoluteTicks <= Length)
            {
                IMidiMessage a = current.MidiMessage;


                if (a.MessageType == MessageType.Channel)
                {
                    ChannelMessage Msg = (ChannelMessage)current.MidiMessage;
                    ChannelCommand b = Msg.Command;

                    if (b == ChannelCommand.NoteOn)
                    {                        
                        int channel = Msg.MidiChannel;
                        int number = Msg.Data1;
                        int velocity = Msg.Data2;
                        int ticks = current.AbsoluteTicks;

                        MidiNote note = new MidiNote(ticks, channel, number, 0, velocity, false);

                        if (velocity > 0)
                        {
                            lsnotes.Add(note);
                        }
                        else
                        {
                            // This is a NoteOff equivalent
                            NoteOff(lsnotes ,channel, number, ticks);
                        }

                        
                    }
                    else if (b == ChannelCommand.NoteOff)
                    {
                        int channel = Msg.MidiChannel;
                        int number = Msg.Data1;
                        int velocity = Msg.Data2;
                        int ticks = current.AbsoluteTicks;

                        NoteOff(lsnotes ,channel, number, ticks);
                    }

                    #region next
                    if (current.Next != null)
                    {
                        current = current.Next;
                    }
                    else
                    {
                        break;
                    }
                    #endregion  

                }
                else
                {
                    #region next
                    if (current.Next != null)
                    {
                        current = current.Next;
                    }
                    else
                    {
                        break;
                    }
                    #endregion  

                }
            }


            // Ménage des durations à 0
            notes.Clear();
            for (int i = 0; i < lsnotes.Count; i++)
            {
                if (lsnotes[i].Duration > 0 && lsnotes[i].Velocity > 0)
                    notes.Add(lsnotes[i]);
            }

            // sort list
            if (notes.Count > 0)
            {
                if (notes.Count > 1)
                    notes.Sort(notes[0]);

                ContainsNotes = true;
            }

        }

        private void NoteOff(List<MidiNote> lsn ,int channel, int notenumber, int endtime)
        {

            MidiNote m = lsn.FindLast(u => u.Channel == channel && u.Number == notenumber && u.Duration == 0);
            if (m != null)
            {
                if (endtime > m.StartTime)
                    m.Duration = endtime - m.StartTime;
            }            
        }


        #region channel

        /// <summary>
        /// Find channel in all events using it
        /// </summary>
        /// <param name="cha"></param>
        /// <returns></returns>
        private int findChannel(int cha)
        {
            int id = 0;
            MidiEvent current = GetMidiEvent(0);

            while (current.AbsoluteTicks <= Length)
            {
                IMidiMessage a = current.MidiMessage;


                if (a.MessageType == MessageType.Channel)
                {
                    ChannelMessage Msg = (ChannelMessage)current.MidiMessage;
                    ChannelCommand b = Msg.Command;

                    if (b == ChannelCommand.ProgramChange || b == ChannelCommand.NoteOn || b == ChannelCommand.NoteOff || b == ChannelCommand.ChannelPressure || b == ChannelCommand.Controller || b == ChannelCommand.PitchWheel || b == ChannelCommand.PolyPressure)
                    {
                        int c = ChannelMessage.UnpackMidiChannel2(a.Status);
                        if (cha == c)
                            return id;
                        else
                        {
                            #region next
                            if (current.Next != null)
                            {
                                current = current.Next;
                                id++;
                            }
                            else
                            {
                                break;
                            }
                            #endregion next      
                        }

                    }
                    else
                    {
                        #region next
                        if (current.Next != null)
                        {
                            current = current.Next;
                            id++;
                        }
                        else
                        {
                            break;
                        }
                        #endregion next      
                    }
                }
                else
                {
                    #region next
                    if (current.Next != null)
                    {
                        current = current.Next;
                        id++;
                    }
                    else
                    {
                        break;
                    }
                    #endregion next      
                }
            }
            return -1;

        }

        /// <summary>
        /// Change channel in all events
        /// </summary>
        public void ChangeChannel(int oldcha, int newcha)
        {
            int i = findChannel(oldcha);

            while (i != -1)
            {
                MidiEvent ev = GetMidiEvent(i);
                IMidiMessage a = ev.MidiMessage;

                MessageType msgtype = ev.MidiMessage.MessageType;
                ChannelMessage Msg = (ChannelMessage)ev.MidiMessage;
                ChannelCommand cmd = Msg.Command;

                int data1 = Msg.Data1;
                int data2 = Msg.Data2;


                // Remove event having old channel
                RemoveAt(i);

                // Recreate new event with new channel
                ChannelMessage message = new ChannelMessage(cmd, newcha, data1, data2);
                Insert(ev.AbsoluteTicks, message);

                i = findChannel(oldcha);
            }


        }
        #endregion channel

        #region fader
        // Find Fader controller (11)
        private int findFader()
        {
            int id = 0;

            MidiEvent current = GetMidiEvent(0);


            while (current.AbsoluteTicks <= Length)
            {
                IMidiMessage a = current.MidiMessage;

                if (a.MessageType == MessageType.Channel)
                {
                    ChannelMessage Msg = (ChannelMessage)current.MidiMessage;
                    ChannelCommand cc = Msg.Command;

                    if (cc == ChannelCommand.Controller)
                    {
                        ControllerType ct = (ControllerType)cc;
                        // FAB test 11 pour fade in & out
                        if (Msg.Data1 == 11)
                        {
                            return id;
                        }
                        else
                        {
                            #region next
                            if (current.Next != null)
                            {
                                current = current.Next;
                                id++;
                            }
                            else
                            {
                                break;
                            }
                            #endregion next      
                        }
                    }
                    else
                    {
                        #region next
                        if (current.Next != null)
                        {
                            current = current.Next;
                            id++;
                        }
                        else
                        {
                            break;
                        }
                        #endregion next      
                    }
                }
                else
                {
                    #region next
                    if (current.Next != null)
                    {
                        current = current.Next;
                        id++;
                    }
                    else
                    {
                        break;
                    }
                    #endregion next      
                }
            }
            return -1;
        }

        /// <summary>
        /// Remove all Fader events
        /// </summary>
        public void RemoveFader()
        {
            int i = findFader();

            while (i != -1)
            {
                RemoveAt(i);
                i = findFader();
            }
        }
        #endregion fader

        #region trackname
        /// <summary>
        /// Find trackname messages
        /// </summary>
        /// <param name="trackname"></param>
        /// <returns></returns>
        private int findTrackName()
        {
            int id = 0;
            MidiEvent current = GetMidiEvent(0);

            while (current.AbsoluteTicks <= Length)
            {
                IMidiMessage a = current.MidiMessage;
                if (a.MessageType == MessageType.Meta)
                {
                    MetaMessage M = (MetaMessage)a;
                    if (M.MetaType == MetaType.TrackName)
                    {
                        return id;                        
                    }                                       
                    else
                    {
                        #region next
                        if (current.Next != null)
                        {
                            current = current.Next;
                            id++;
                        }
                        else
                        {
                            break;
                        }
                        #endregion next      
                    }
                }
                else
                {
                    #region next
                    if (current.Next != null)
                    {
                        current = current.Next;
                        id++;
                    }
                    else
                    {
                        break;
                    }
                    #endregion next      
                }
            }
            return -1;
        }

        /// <summary>
        /// Remove all Trackname events
        /// </summary>
        public void RemoveTrackname()
        {
            int i = findTrackName();

            while (i != -1)
            {
                RemoveAt(i);
                i = findTrackName();
            }
        }

        #endregion trackname

        #region timesignature
        /// <summary>
        /// Find timesignature messages
        /// </summary>
        /// <returns></returns>
        private int findTimeSignature()
        {
            int id = 0;
            MidiEvent current = GetMidiEvent(0);

            while (current.AbsoluteTicks <= Length)
            {
                IMidiMessage a = current.MidiMessage;
                if (a.MessageType == MessageType.Meta)
                {
                    MetaMessage M = (MetaMessage)a;
                    if (M.MetaType == MetaType.TimeSignature)
                    {
                        return id;
                    }
                    else
                    {
                        #region next
                        if (current.Next != null)
                        {
                            current = current.Next;
                            id++;
                        }
                        else
                        {
                            break;
                        }
                        #endregion next      
                    }
                }
                else
                {
                    #region next
                    if (current.Next != null)
                    {
                        current = current.Next;
                        id++;
                    }
                    else
                    {
                        break;
                    }
                    #endregion next      
                }
            }
            return -1;
        }

        /// <summary>
        /// Remove all TimeSignature events
        /// </summary>
        public void RemoveTimesignature()
        {
            int i = findTimeSignature();

            while (i != -1)
            {
                RemoveAt(i);
                i = findTimeSignature();
            }
        }

        #endregion timesignature        

        #region meta message
        public void insertTrackname(string trackname)
        {
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(trackname);
            MetaMessage metamessage = new MetaMessage(MetaType.TrackName, bytes);
            Insert(0, metamessage);
        }

        /// <summary>
        /// Insert Tempo Message in a track at position 0
        /// </summary>
        /// <param name="tempo"></param>
        public void insertTempo(int tempo)
        {
            var split = BitConverter.GetBytes(tempo);
            byte[] bytes = new byte[3];
            bytes[0] = split[2]; //11;
            bytes[1] = split[1]; //113;
            bytes[2] = split[0]; //176;
            MetaMessage metamessage = new MetaMessage(MetaType.Tempo, bytes);
            Insert(0, metamessage);
        }

        public void insertKeysignature(int numerator, int denominator)
        {
            byte[] bytes = new byte[2];
            bytes[0] = Convert.ToByte(numerator);   //4;
            bytes[1] = Convert.ToByte(denominator); //2;
            MetaMessage metamessage = new MetaMessage(MetaType.KeySignature, bytes);
            Insert(0, metamessage);
        }

        public void insertTimesignature(int numerator, int denominator)
        {
            byte[] bytes = new byte[4];
            bytes[0] = Convert.ToByte(numerator);   // [nn] Numerator

            int negativepowerof2 = 2;
            try
            {
                negativepowerof2 = (int)(Math.Log(denominator) / Math.Log(2));
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            bytes[1] = Convert.ToByte(negativepowerof2);    // [dd] Denominator negative power of 2
            
            bytes[2] = Convert.ToByte(24);                  // [cc] MIDI ticks per metronome click
            bytes[3] = Convert.ToByte(8);                   // [bb] 32nd notes per MIDI quarter note
            MetaMessage metamessage = new MetaMessage(MetaType.TimeSignature, bytes);
            Insert(0, metamessage);
        }

        #endregion meta message

        #region channel command message

        #region programchange
        private int findProgramChange()
        {
            int id = -1;
            foreach (MidiEvent ev in Iterator())
            {
                id++;
                IMidiMessage a = ev.MidiMessage;
                if (a.MessageType == MessageType.Channel)
                {
                    ChannelCommand b;
                    b = ChannelMessage.UnpackCommand2(a.Status);
                    if (b == ChannelCommand.ProgramChange)
                        return id;
                }
            }
            return -1;
        }

        public void changePatch(int programchange)
        {
            // Remove all programchange events
            int i = findProgramChange();
            while (i != -1)
            {
                RemoveAt(i);
                i = findProgramChange();
            }

            ProgramChange = programchange; 

            // Insert new patch at position 0
            ChannelMessage message = new ChannelMessage(ChannelCommand.ProgramChange, MidiChannel, ProgramChange, 0);
            Insert(0, message);

        }

        public void insertPatch(int channel, int programchange) {
            ChannelMessage message = new ChannelMessage(ChannelCommand.ProgramChange, channel, programchange, 0);
            Insert(0, message);
        }

        #endregion

        public void insertVolume(int channel, int volume)
        {
            ChannelMessage message = new ChannelMessage(ChannelCommand.Controller, channel, (int)ControllerType.Volume, volume);
            Insert(0, message);
        }

        public void insertPan(int channel, int pan)
        {
            ChannelMessage message = new ChannelMessage(ChannelCommand.Controller, channel, (int)ControllerType.Pan, pan);
            Insert(0, message);
        }

        public void insertReverb(int channel, int reverb)
        {
            ChannelMessage message = new ChannelMessage(ChannelCommand.Controller, channel, (int)ControllerType.EffectsLevel, reverb);
            Insert(0, message);
        }

        #region pitchbend
        
        /// <summary>
        /// IS there any pitch bend here?
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        public bool IsPitchBend(int channel, int starttime, int endtime)
        {
            return findPitchBend(channel, starttime, endtime) != -1;

        }

        /// <summary>
        /// Set pitchbend
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="number"></param>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <param name="pitchBend"></param>
        public void SetPitchBend(int channel, int number, int starttime, int endtime, int pb)
        {
            // No pitch = 8192 (0x2000)
            // 2 demi tons up =  16383 
            // 2 demi tons down = 0
            int mask = 127;
            int ipitchBend;
            int endPitchTime = 0;

            ipitchBend = pb;

            RemovePitchBend(channel, starttime, endtime);

            StopPitchBend(channel, starttime);

            if (endtime > 2)
            {
                // endPitchTime = endtime - 2;
                endPitchTime = endtime - 1;
            }

            if (endtime <= starttime)
                return;

            ChannelMessageBuilder builder = new ChannelMessageBuilder();
            ChannelMessage pitchBendMessage;

            // Build pitch bend message;
            builder.Command = ChannelCommand.PitchWheel;
            builder.MidiChannel = channel;

            // Start Pitchbend           
            if (ipitchBend > 16383)
                ipitchBend = 16383;

            // 1 - Start pitchbend at the middle of duration
            int t = starttime + (endtime - starttime) / 2;

            // Unpack pitch bend value into two data bytes.
            builder.Data1 = (ipitchBend/2) & mask;
            builder.Data2 = (ipitchBend/2) >> 7;

            // Build message.
            builder.Build();
            pitchBendMessage = builder.Result;
            InsertLast(t, pitchBendMessage);


            // 2 - Start pitchbend at the middle of duration
            t = starttime + 5 * (endtime - starttime) / 8;

            // Unpack pitch bend value into two data bytes.
            builder.Data1 = (5 * ipitchBend / 8) & mask;
            builder.Data2 = (5 * ipitchBend / 8) >> 7;

            // Build message.
            builder.Build();
            pitchBendMessage = builder.Result;
            InsertLast(t, pitchBendMessage);


            // 3 - intermédiaire
            t = starttime + 6 * (endtime - starttime) / 8;

            // Unpack pitch bend value into two data bytes.
            builder.Data1 = (6 * ipitchBend / 8) & mask;
            builder.Data2 = (6 * ipitchBend /8) >> 7;

            // Build message.
            builder.Build();
            pitchBendMessage = builder.Result;
            InsertLast(t, pitchBendMessage);


            // 4 - intermédiaire
            t = starttime + 7 * (endtime - starttime) / 8;

            // Unpack pitch bend value into two data bytes.
            builder.Data1 = ipitchBend & mask;
            builder.Data2 = ipitchBend >> 7;

            // Build message.
            builder.Build();
            pitchBendMessage = builder.Result;
            InsertLast(t, pitchBendMessage);


            // 5 - send pitchbend at the end, +1 after
            t = endPitchTime + 1;

            // Unpack pitch bend value into two data bytes.
            builder.Data1 = ipitchBend & mask;
            builder.Data2 = ipitchBend >> 7;

            // Build message.
            builder.Build();
            pitchBendMessage = builder.Result;
            InsertLast(t, pitchBendMessage);


            #region stop pitchbend                       

            // Stop pitchbend -1 after last pitch
            StopPitchBend(channel, endPitchTime);
            

            #endregion
        }
        public void SetPitchBend2(int channel, int number, int starttime, int endtime, int pb)
        {
            // No pitch = 8192
            // 2 demi tons =  16383 ?
            int mask = 127;
            int endPitchTime = 0;

            int startpitchBend = 8192;
            int endpitchBend = pb;
            int ipitchBend = startpitchBend;

            RemovePitchBend(channel, starttime, endtime);
            StopPitchBend(channel, starttime);

            if (endtime > 2)
            {
                //endPitchTime = endtime - 2;
                endPitchTime = endtime - 1;
            }

            if (endtime <= starttime)
                return;
  
            ChannelMessageBuilder builder = new ChannelMessageBuilder();
            ChannelMessage pitchBendMessage;

            // Build pitch bend message;
            builder.Command = ChannelCommand.PitchWheel;
            builder.MidiChannel = channel;            
            
             // Start Pitchbend           
            if (endpitchBend > 16383)
                endpitchBend = 16383;
            if (endpitchBend < 0)
                endpitchBend = 0;

            // Increase from 8192 to 13383 during the duration of the note
            int steps = 10;
            int OffsetTime = (endPitchTime - starttime) / steps;
            int offsetPitch = (endpitchBend - startpitchBend) / steps;            
            int t = starttime;
            
            for (int i = 0; i < steps ; i++)
            {
                ipitchBend += offsetPitch;
                t += OffsetTime;

                // Build pitch bend message;
                builder.Command = ChannelCommand.PitchWheel;

                // Unpack pitch bend value into two data bytes.
                builder.Data1 = ipitchBend & mask;
                builder.Data2 = ipitchBend >> 7;                

                // Build message.
                builder.Build();
                pitchBendMessage = builder.Result;
                Insert(t, pitchBendMessage);
            }

            #region last pitch
            // 2 - send pitchbend at the end, +1 after
            t = endPitchTime + 1;

            // Unpack pitch bend value into two data bytes.
            builder.Data1 = ipitchBend & mask;
            builder.Data2 = ipitchBend >> 7;

            // Build message.
            builder.Build();
            pitchBendMessage = builder.Result;
            InsertLast(t, pitchBendMessage);
            #endregion last pitch

            #region stop pitchbend                       

            // Stop pitchbend
            StopPitchBend(channel, endPitchTime);
            StopPitchBend(channel, endPitchTime + 1);

            #endregion
        }

        private void StopPitchBend(int channel, int time)
        {
            ChannelMessageBuilder builder = new ChannelMessageBuilder();
            ChannelMessage pitchBendMessage;
            int pitchBend;
            int mask = 127;

            // Stop pitchbend
            //pitchBend = 0x2000; // No pitch = 8192
            pitchBend = 8192;
            builder = new ChannelMessageBuilder();

            // Build pitch bend message;
            builder.Command = ChannelCommand.PitchWheel;
            builder.MidiChannel = channel;

            // Unpack pitch bend value into two data bytes.
            builder.Data1 = pitchBend & mask;
            builder.Data2 = pitchBend >> 7;

            // Build message.
            builder.Build();
            pitchBendMessage = builder.Result;
            InsertLast(time, pitchBendMessage);
        }

        /// <summary>
        /// Find all pitchbend events in a fraction of time
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        public List<MidiEvent> findPitchBendValues(int channel, int starttime, int endtime)
        {
            List<MidiEvent> pbevents = new List<MidiEvent>();
            int id = 0;
            //int x,y;

            // Start from 0
            MidiEvent current = GetMidiEvent(0);

            while (current.AbsoluteTicks <= endtime)
            {
                IMidiMessage a = current.MidiMessage;

                if (current.AbsoluteTicks < starttime)
                {
                    #region next
                    if (current.Next != null)
                    {
                        current = current.Next;
                        id++;
                    }
                    else
                    {
                        break;
                    }
                    #endregion
                }
                else if (current.AbsoluteTicks > endtime)
                {
                    break;
                }
                else if (a.MessageType == MessageType.Channel)
                {
                    ChannelMessage Msg = (ChannelMessage)current.MidiMessage;
                    ChannelCommand cc = Msg.Command;

                    if (Msg.MidiChannel == channel)
                    {
                        if (cc == ChannelCommand.PitchWheel)
                        {
                            
                            //x = Msg.Data1;
                            //y = Msg.Data2;
                            pbevents.Add(current);

                            #region next
                            if (current.Next != null)
                            {
                                current = current.Next;
                                id++;
                            }
                            else
                            {
                                break;
                            }
                            #endregion next      
                        }
                        else
                        {
                            #region next
                            if (current.Next != null)
                            {
                                current = current.Next;
                                id++;
                            }
                            else
                            {
                                break;
                            }
                            #endregion next      
                        }

                    }
                    else
                    {
                        #region next
                        if (current.Next != null)
                        {
                            current = current.Next;
                            id++;
                        }
                        else
                        {
                            break;
                        }
                        #endregion next      
                    }

                }
                else
                {
                    #region next
                    if (current.Next != null)
                    {
                        current = current.Next;
                        id++;
                    }
                    else
                    {
                        break;
                    }
                    #endregion next      
                }
            }
            return pbevents;
        }

        private int findPitchBend(int channel, int starttime, int endtime)
        {
            int id = 0;
            MidiEvent current = GetMidiEvent(0);

            while (current.AbsoluteTicks <= endtime)
            {
                IMidiMessage a = current.MidiMessage;

                if (current.AbsoluteTicks < starttime)
                {
                    #region next
                    if (current.Next != null)
                    {
                        current = current.Next;
                        id++;
                    }
                    else
                    {
                        break;
                    }
                    #endregion
                }
                else if (a.MessageType == MessageType.Channel)
                {
                    ChannelMessage Msg = (ChannelMessage)current.MidiMessage;                                       
                    ChannelCommand cc = Msg.Command;

                    if (Msg.MidiChannel == channel)
                    {                        
                        if (cc == ChannelCommand.PitchWheel)
                        {
                            return id;
                        }
                        else
                        {
                            #region next
                            if (current.Next != null)
                            {
                                current = current.Next;
                                id++;
                            }
                            else
                            {
                                break;
                            }
                            #endregion next      
                        }

                    }
                    else
                    {
                        #region next
                        if (current.Next != null)
                        {
                            current = current.Next;
                            id++;
                        }
                        else
                        {
                            break;
                        }
                        #endregion next      
                    }

                }
                else
                {
                    #region next
                    if (current.Next != null)
                    {
                        current = current.Next;
                        id++;
                    }
                    else
                    {
                        break;
                    }
                    #endregion next      
                }
            }
            return -1;
        }

        public void RemovePitchBend(int channel, int starttime, int endtime)
        {
            //endtime = endtime - 1;
            int i = findPitchBend(channel, starttime, endtime);

            while (i != -1)
            {
                RemoveAt(i);
                i = findPitchBend(channel, starttime, endtime);
            }
        }

        #endregion pitchbend

        #endregion channel command message


        #region copy events

        /// <summary>
        /// Copy events
        /// </summary>
        /// <param name="srcstarttime"></param>
        /// <param name="srcendtime"></param>
        /// <param name="deststarttime"></param>
        public void CopyEvents(float srcstarttime, float srcendtime, float deststarttime)
        {
            float delta = 0;
            MidiEvent current = GetMidiEvent(Count - 1);            
            bool bFound = false;

            while (current.AbsoluteTicks >= srcstarttime)
            {                
                // Consider events having their ticks inside srcstarttime and screndtime
                if (current != endOfTrackMidiEvent && current.AbsoluteTicks>= srcstarttime && current.AbsoluteTicks <= srcendtime)
                {
                    delta = current.AbsoluteTicks - srcstarttime;

                    // Do not insert if similar event exists in the target position!                    
                    int position = (int)deststarttime + (int)delta;
                    bFound = false;
                    
                    // List all events having this ticks
                    List<MidiEvent> melist = GetEventsFromTicks(position);
                    foreach (MidiEvent me in melist)
                    {
                        //Search Channel events having same values 
                        if (me.MidiMessage.MessageType == MessageType.Channel && current.MidiMessage.MessageType == MessageType.Channel)
                        {
                            IMidiMessage cmsg = current.MidiMessage;
                            IMidiMessage emsg = me.MidiMessage;
                            ChannelCommand ccc = ChannelMessage.UnpackCommand(cmsg.Status);
                            ChannelCommand ecc = ChannelMessage.UnpackCommand(emsg.Status);

                            // notes values : emsg.Data1 == cmsg.Data1
                            // ChannelCommd ecc == ccc (noteon, noteoff)
                            if (emsg.Data1 == cmsg.Data1 && ecc == ccc)
                            {
                                bFound = true;
                                break;
                            }
                        }                        
                    }

                    // Insert new event at target position (int)deststarttime + (int)delta
                    // only if not found and if it is a Channel message
                    if (!bFound && current.MidiMessage.MessageType == MessageType.Channel)                        
                        Insert((int)deststarttime + (int)delta, current.MidiMessage);
                    

                    #region previous
                    if (current.Previous != null && current.Previous != endOfTrackMidiEvent)
                    {
                        current = current.Previous;                        
                    }
                    else
                    {
                        break;
                    }
                    #endregion previous
                }
                else
                {
                    #region previous
                    if (current.Previous != null && current.Previous != endOfTrackMidiEvent)
                    {
                        current = current.Previous;                        
                    }
                    else
                    {
                        break;
                    }
                    #endregion previous
                }

            }
        }


        #endregion

        #region start times

        /// <summary>
        /// Offset start times off all notes
        /// </summary>
        /// <param name="offset"></param>
        public void OffsetStartTimes(int starttime, int offset)
        {

            MidiEvent current = GetMidiEvent(Count - 1);

            if (offset > 0)
            {
                while (current.AbsoluteTicks >= starttime)
                {
                    if (current != endOfTrackMidiEvent)
                    {
                        // New code : move all events
                        Move(current, current.AbsoluteTicks + offset);

                        #region previous
                        if (current.Previous != null && current.Previous != endOfTrackMidiEvent)
                        {
                            current = current.Previous;
                        }
                        else
                        {
                            break;
                        }
                        #endregion previous
                    }
                    else
                    {
                        #region previous
                        if (current.Previous != null && current.Previous != endOfTrackMidiEvent)
                        {
                            current = current.Previous;
                        }
                        else
                        {
                            break;
                        }
                        #endregion previous
                    }

                }

                // offset also the list of notes
                for (int i = notes.Count - 1; i >= 0; i--)
                {
                    if (notes[i].StartTime >= starttime)
                    {
                        notes[i].StartTime = notes[i].StartTime + offset;
                    }
                }

                for (int i = this.Lyrics.Count - 1; i >= 0; i--)
                {
                    if (Lyrics[i].TicksOn >= starttime)
                    {
                        Lyrics[i].TicksOn = Lyrics[i].TicksOn + offset;
                    }
                }
            }
            else if (offset < 0)
            {
                // delete notes in notes & events from starttime to starttime - Offset
                List<MidiNote> delnotes = new List<MidiNote>();
                for (int i = 0; i < notes.Count; i++)
                {
                    if (notes[i].StartTime >= starttime && notes[i].StartTime < starttime - offset)
                    {
                        delnotes.Add(notes[i]);
                    }
                }
                for (int i = 0; i < delnotes.Count; i++)
                {
                    deleteNote(delnotes[i].Number, delnotes[i].StartTime);
                }

                // negative offset all notes from end to > starttime - offset 
                #region negative offset

                current = GetMidiEvent(0);
                while (current.AbsoluteTicks <= Length)
                {
                    if (current.AbsoluteTicks >= starttime - offset)
                    {

                        // new code : delete all the events, not only the notes

                        Move(current, current.AbsoluteTicks + offset);
                        #region next
                        if (current.Next != null)
                        {
                            current = current.Next;
                        }
                        else
                        {
                            break;
                        }
                        #endregion
                    }
                    else
                    {
                        #region next
                        if (current.Next != null)
                        {
                            current = current.Next;
                        }
                        else
                        {
                            break;
                        }
                        #endregion next
                    }

                }

                #endregion negative offset


                // offset also the list of notes
                for (int i = 0; i < notes.Count; i++)
                {
                    if (notes[i].StartTime >= starttime - offset)
                    {
                        notes[i].StartTime = notes[i].StartTime + offset;
                    }
                }

                for (int i = 0; i < Lyrics.Count; i++)
                {
                    if (Lyrics[i].TicksOn >= starttime - offset)
                    {
                        Lyrics[i].TicksOn = Lyrics[i].TicksOn + offset;
                    }
                }


            }



        }

        #endregion

        #region measures

        /// <summary>
        /// Insert a measure in the track
        /// </summary>
        /// <param name="starttime">beginning of measure to offset</param>
        /// <param name="offset">offset, ie one measure length</param>
        public void insertMeasure(int starttime, int offset)
        {
            // Offset all "starttimes" from end to starttime

            MidiEvent current = GetMidiEvent(Count - 1);

            while (current.AbsoluteTicks >= starttime)
            {

                if (current != endOfTrackMidiEvent)
                {
                    // New code : move all events
                    Move(current, current.AbsoluteTicks + offset);

                    #region previous
                    if (current.Previous != null && current.Previous != endOfTrackMidiEvent)
                    {
                        current = current.Previous;
                    }
                    else
                    {
                        break;
                    }
                    #endregion previous
                }
                else
                {
                    #region previous
                    if (current.Previous != null && current.Previous != endOfTrackMidiEvent)
                    {
                        current = current.Previous;
                    }
                    else
                    {
                        break;
                    }
                    #endregion previous
                }              

            }

            // offset also the list of notes
            for (int i = notes.Count - 1; i >= 0; i--)
            {
                if (notes[i].StartTime >= starttime)
                {
                    notes[i].StartTime = notes[i].StartTime + offset;
                }
            }

            for (int i = this.Lyrics.Count - 1; i >= 0; i--)
            {
                if (Lyrics[i].TicksOn >= starttime)
                {
                    Lyrics[i].TicksOn = Lyrics[i].TicksOn + offset;
                }
            }

        }

        /// <summary>
        /// Delete a measure in the track
        /// </summary>
        /// <param name="starttime"></param>
        /// <param name="offset"></param>
        public void deleteMeasure(int starttime, int offset)
        {
            // delete notes in notes & events from starttime to starttime + Offset
            List<MidiNote> delnotes = new List<MidiNote>();
            for (int i = 0; i < notes.Count; i++)
            {
                if (notes[i].StartTime >= starttime && notes[i].StartTime < starttime + offset)
                {
                    delnotes.Add(notes[i]);
                }
            }
            for (int i = 0; i < delnotes.Count; i++)
            {
                deleteNote(delnotes[i].Number, delnotes[i].StartTime);
            }


            // negative offset all notes from end to > starttime + offset 
            #region negative offset

            MidiEvent current = GetMidiEvent(0);
            while (current.AbsoluteTicks <= Length)
            {
                if (current.AbsoluteTicks >= starttime + offset)
                {

                    // new code : delete all the events, not only the notes

                    Move(current, current.AbsoluteTicks - offset);
                    #region next
                    if (current.Next != null)
                    {
                        current = current.Next;
                    }
                    else
                    {
                        break;
                    }
                    #endregion                   
                }
                else
                {
                    #region next
                    if (current.Next != null)
                    {
                        current = current.Next;
                    }
                    else
                    {
                        break;
                    }
                    #endregion next
                }

            }

            #endregion negative offset


            // offset also the list of notes
            for (int i = 0; i < notes.Count; i++)
            {
                if (notes[i].StartTime >= starttime + offset)
                {
                    notes[i].StartTime = notes[i].StartTime - offset;
                }
            }

            for (int i = 0; i < Lyrics.Count; i++)
            {
                if (Lyrics[i].TicksOn >= starttime + offset)
                {
                    Lyrics[i].TicksOn = Lyrics[i].TicksOn - offset;
                }
            }

        }

        #endregion measures

        #region lyrics
        /// <summary>
        /// Delete all lyrics (text or lyric)
        /// </summary>
        public void deleteLyrics()
        {
            // supprime tous les messages text & lyric
            bool oneMoreTime = true;
            while (oneMoreTime)
            {
                int toDelete = -1;
                oneMoreTime = false;
                int id = -1;

                foreach (MidiEvent a in Iterator())
                {
                    id++;
                    if (a.MidiMessage.MessageType == MessageType.Meta)
                    {
                        MetaMessage Msg = (MetaMessage)a.MidiMessage;
                        // Lyriques de type Text ou lyric
                        if (Msg.MetaType == MetaType.Text || Msg.MetaType == MetaType.Lyric)
                        {
                            toDelete = id;
                            break;
                        }
                    }
                }
                if (toDelete != -1)
                {
                    RemoveAt(toDelete);
                    oneMoreTime = true;
                }
            }
        }

        #endregion lyrics


        /// <summary>
        /// Get all events at this position (ticks)
        /// </summary>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public List<MidiEvent> GetEventsFromTicks(float ticks)
        {
            List<MidiEvent> midiEvents = new List<MidiEvent>();            
            MidiEvent current = GetMidiEvent(0);

            while (current.AbsoluteTicks <= Length)
            {
                if (current.AbsoluteTicks == ticks)
                {
                    // Same position = ticks
                    midiEvents.Add(current);

                    #region next
                    if (current.Next != null)
                    {
                        current = current.Next;
                    }
                    else
                    {
                        break;
                    }
                    #endregion next

                }
                else if (current.AbsoluteTicks > ticks)
                {
                    // position > ticks => exit
                    break;
                }
                else
                {
                    #region next
                    if (current.Next != null)
                    {
                        current = current.Next;                        
                    }
                    else
                    {
                        break;
                    }
                    #endregion next
                }
            }

            return midiEvents;
        }

        /// <summary>
        /// FAB: Return index of Event from ticks
        /// </summary>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public int GetEventPositionFromTicks(int note, float ticks, int duration)
        {
            int id = 0;
            MidiEvent current = GetMidiEvent(0);

            while (current.AbsoluteTicks <= Length)
            {
                // Search note On: must be equal to ticks
                if (current.AbsoluteTicks == ticks)
                {
                    // Est-ce bien une note ?
                    IMidiMessage a = current.MidiMessage;
                    if (a.MessageType == MessageType.Channel)
                    {
                        ChannelCommand b = ChannelMessage.UnpackCommand(a.Status);

                        int n = a.Data1;
                        if (n == note && b == ChannelCommand.NoteOn)
                        {
                            return id;
                        }
                        else
                        {
                            #region next
                            if (current.Next != null)
                            {
                                current = current.Next;
                                id++;
                            }
                            else
                            {
                                break;
                            }
                            #endregion next
                        }
                    }
                    else
                    {
                        #region next
                        if (current.Next != null)
                        {
                            current = current.Next;
                            id++;
                        }
                        else
                        {
                            break;
                        }
                        #endregion next
                    }
                }
                else if (current.AbsoluteTicks == ticks + duration || current.AbsoluteTicks == ticks + duration - 1 || current.AbsoluteTicks == ticks + duration - 10)   // j'ai enlevé 1 ou 10 :-( et tout cassé
                {
                    // Search Note Off: must be equal to ticks + duration (+ or - 10, internal dirty stuff for me) 
                    IMidiMessage a = current.MidiMessage;
                    if (a.MessageType == MessageType.Channel)
                    {

                        ChannelCommand b = ChannelMessage.UnpackCommand(a.Status);

                        int n = a.Data1;
                        if (n == note && b == ChannelCommand.NoteOff)
                        {
                            return id;
                        }
                        else
                        {
                            #region next
                            if (current.Next != null)
                            {
                                current = current.Next;
                                id++;
                            }
                            else
                            {
                                break;
                            }
                            #endregion next
                        }
                    }
                    else
                    {
                        #region next
                        if (current.Next != null)
                        {
                            current = current.Next;
                            id++;
                        }
                        else
                        {
                            break;
                        }
                        #endregion next
                    }
                }
                else
                {
                    #region next
                    if (current.Next != null)
                    {
                        current = current.Next;
                        id++;
                    }
                    else
                    {
                        break;
                    }
                    #endregion next
                }
            }
            return -1;
        }

        /// <summary>
        /// Inserts an IMidiMessage at the specified position in absolute ticks.
        /// </summary>
        /// <param name="position">
        /// The position in the Track in absolute ticks in which to insert the
        /// IMidiMessage.
        /// </param>
        /// <param name="message">
        /// The IMidiMessage to insert.
        /// </param>
        public void Insert(int position, IMidiMessage message)
        {
            #region Require

            if (position < 0)
            {
                //throw new ArgumentOutOfRangeException("position", position, "IMidiMessage position out of range.");
                position = this.Length;

                Console.Write("\nERROR: IMidiMessage position out of range (Track.cs, Insert");

            }
            else if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            #endregion            

            MidiEvent newMidiEvent = new MidiEvent(this, position, message);

            if (head == null)
            {
                head = newMidiEvent;
                tail = newMidiEvent;
            }
            else if (position >= tail.AbsoluteTicks)
            {
                newMidiEvent.Previous = tail;
                tail.Next = newMidiEvent;
                tail = newMidiEvent;
                endOfTrackMidiEvent.SetAbsoluteTicks(Length);
                endOfTrackMidiEvent.Previous = tail;
            }
            else
            {
                MidiEvent current = head;

                while (current.AbsoluteTicks < position)
                {
                    current = current.Next;
                }

                newMidiEvent.Next = current;
                newMidiEvent.Previous = current.Previous;

                if (current.Previous != null)
                {
                    current.Previous.Next = newMidiEvent;
                }
                else
                {
                    head = newMidiEvent;
                }

                current.Previous = newMidiEvent;
            }

            count++;

            #region Invariant


            // FAB perfs
            //AssertValid();

            #endregion
        }

        public void InsertLast(int position, IMidiMessage message)
        {
            #region Require

            if (position < 0)
            {
                //throw new ArgumentOutOfRangeException("position", position, "IMidiMessage position out of range.");
                position = this.Length;

                Console.Write("\nERROR: IMidiMessage position out of range (Track.cs, Insert");

            }
            else if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            #endregion            

            MidiEvent newMidiEvent = new MidiEvent(this, position, message);

            if (head == null)
            {
                head = newMidiEvent;
                tail = newMidiEvent;
            }
            else if (position >= tail.AbsoluteTicks)
            {
                newMidiEvent.Previous = tail;
                tail.Next = newMidiEvent;
                tail = newMidiEvent;
                endOfTrackMidiEvent.SetAbsoluteTicks(Length);
                endOfTrackMidiEvent.Previous = tail;
            }
            else
            {
                MidiEvent current = tail;

                while (current.AbsoluteTicks > position)
                {
                    current = current.Previous;
                }

                newMidiEvent.Next = current.Next;
                newMidiEvent.Previous = current;

                if (current.Next != null)
                {
                    current.Next.Previous = newMidiEvent;
                }
                else
                {
                    tail = newMidiEvent;
                }

                current.Next = newMidiEvent;
            }

            count++;

            #region Invariant


            // FAB perfs
            //AssertValid();

            #endregion
        }

        /// <summary>
        /// Clears all of the MidiEvents, with the exception of the end of track
        /// message, from the Track.
        /// </summary>
        public void Clear()
        {
            head = tail = null;
            notes.Clear(); //FAB
            count = 1;

            #region Invariant

            AssertValid();

            #endregion
        }

        /// <summary>
        /// Merges the specified Track with the current Track.
        /// </summary>
        /// <param name="trk">
        /// The Track to merge with.
        /// </param>
        public void Merge(Track trk)
        {
            #region Require

            if (trk == null)
            {
                throw new ArgumentNullException("trk");
            }

            #endregion

            #region Guard

            if (trk == this)
            {
                return;
            }
            else if (trk.Count == 1)
            {
                return;
            }

            #endregion

#if(DEBUG)
            int oldCount = Count;
#endif

            count += trk.Count - 1;

            MidiEvent a = head;
            MidiEvent b = trk.head;
            MidiEvent current = null;

            Debug.Assert(b != null);

            if (a != null && a.AbsoluteTicks <= b.AbsoluteTicks)
            {
                current = new MidiEvent(this, a.AbsoluteTicks, a.MidiMessage);
                a = a.Next;
            }
            else
            {
                current = new MidiEvent(this, b.AbsoluteTicks, b.MidiMessage);
                b = b.Next;
            }

            head = current;

            while (a != null && b != null)
            {
                while (a != null && a.AbsoluteTicks <= b.AbsoluteTicks)
                {
                    current.Next = new MidiEvent(this, a.AbsoluteTicks, a.MidiMessage);
                    current.Next.Previous = current;
                    current = current.Next;
                    a = a.Next;
                }

                if (a != null)
                {
                    while (b != null && b.AbsoluteTicks <= a.AbsoluteTicks)
                    {
                        current.Next = new MidiEvent(this, b.AbsoluteTicks, b.MidiMessage);
                        current.Next.Previous = current;
                        current = current.Next;
                        b = b.Next;
                    }
                }
            }

            while (a != null)
            {
                current.Next = new MidiEvent(this, a.AbsoluteTicks, a.MidiMessage);
                current.Next.Previous = current;
                current = current.Next;
                a = a.Next;
            }

            while (b != null)
            {
                current.Next = new MidiEvent(this, b.AbsoluteTicks, b.MidiMessage);
                current.Next.Previous = current;
                current = current.Next;
                b = b.Next;
            }

            tail = current;

            endOfTrackMidiEvent.SetAbsoluteTicks(Length);
            endOfTrackMidiEvent.Previous = tail;

            #region Ensure

            Debug.Assert(count == oldCount + trk.Count - 1);

            #endregion

            #region Invariant

            AssertValid();

            #endregion
        }

        /// <summary>
        /// Return a deep copy clone of this MidiTrack
        /// </summary>
        /// <returns></returns>
        public Track Clone()
        {
            Track track = new Track();
            track.instrumentname = this.instrumentname;

            int count = 0;
            foreach (MidiNote note in this.notes)
            {
                track.notes.Add(note.Clone());
                count++;
            }

            if (count > 0)
                track.ContainsNotes = true;

           
            if (Lyrics != null)
            {
                track.Lyrics = new List<Track.Lyric>();
                foreach (Track.Lyric ev in Lyrics)
                {
                    track.Lyrics.Add(ev);
                }
            }


            track.Visible = this.Visible;
            return track;
        }

        /// <summary>
        /// Removes the MidiEvent at the specified index.
        /// </summary>
        /// <param name="index">
        /// The index into the Track at which to remove the MidiEvent.
        /// </param>
        public void RemoveAt(int index)
        {
            #region Require

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", index, "Track index out of range.");
            }
            else if (index == Count - 1)
            {
                throw new ArgumentException("Cannot remove the end of track event.", "index");
                //Console.Write("\nERROR: Cannot remove the end of track event. Tack.cs, RemoveAt()");
                //return;
            }

            #endregion

            MidiEvent current = GetMidiEvent(index);

            if (current.Previous != null)
            {
                current.Previous.Next = current.Next;
            }
            else
            {
                Debug.Assert(current == head);

                head = head.Next;
            }

            if (current.Next != null)
            {
                current.Next.Previous = current.Previous;
            }
            else
            {
                Debug.Assert(current == tail);

                tail = tail.Previous;

                endOfTrackMidiEvent.SetAbsoluteTicks(Length);
                endOfTrackMidiEvent.Previous = tail;
            }

            current.Next = current.Previous = null;

            count--;

            #region Invariant

            AssertValid();

            #endregion
        }

        /// <summary>
        /// Gets the MidiEvent at the specified index.
        /// </summary>
        /// <param name="index">
        /// The index of the MidiEvent to get.
        /// </param>
        /// <returns>
        /// The MidiEvent at the specified index.
        /// </returns>
        public MidiEvent GetMidiEvent(int index)
        {
            #region Require

            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException("index", index,
                    "Track index out of range.");
            }

            #endregion

            MidiEvent result;

            // Si c'est le dernier event
            if (index == Count - 1)
            {
                result = endOfTrackMidiEvent;
            }
            else
            {
                // Recherche en partant du début
                if (index < Count / 2)
                {
                    result = head;

                    for (int i = 0; i < index; i++)
                    {
                        result = result.Next;
                    }
                }
                else
                {
                    // Recherche en partant de la fin
                    result = tail;

                    for (int i = Count - 2; i > index; i--)
                    {
                        result = result.Previous;
                    }
                }
            }

            #region Ensure

#if(DEBUG)
            if (index == Count - 1)
            {
                Debug.Assert(result.AbsoluteTicks == Length);
                Debug.Assert(result.MidiMessage == MetaMessage.EndOfTrackMessage);
            }
            else
            {
                MidiEvent t = head;

                for (int i = 0; i < index; i++)
                {
                    t = t.Next;
                }

                Debug.Assert(t == result);
            }
#endif

            #endregion

            return result;
        }

        public void Move(MidiEvent e, int newPosition)
        {
            #region Require

            if (e.Owner != this)
            {
                throw new ArgumentException("MidiEvent does not belong to this Track.");
            }
            else if (newPosition < 0)
            {
                throw new ArgumentOutOfRangeException("newPosition");
            }
            else if (e == endOfTrackMidiEvent)
            {
                throw new InvalidOperationException(
                    "Cannot move end of track message. Use the EndOfTrackOffset property instead.");
            }

            #endregion

            MidiEvent previous = e.Previous;
            MidiEvent next = e.Next;

            if (e.Previous != null && e.Previous.AbsoluteTicks > newPosition)
            {
                e.Previous.Next = e.Next;

                if (e.Next != null)
                {
                    e.Next.Previous = e.Previous;
                }

                while (previous != null && previous.AbsoluteTicks > newPosition)
                {
                    next = previous;
                    previous = previous.Previous;
                }
            }
            else if (e.Next != null && e.Next.AbsoluteTicks < newPosition)
            {
                e.Next.Previous = e.Previous;

                if (e.Previous != null)
                {
                    e.Previous.Next = e.Next;
                }

                while (next != null && next.AbsoluteTicks < newPosition)
                {
                    previous = next;
                    next = next.Next;
                }
            }

            if (previous != null)
            {
                previous.Next = e;
            }

            if (next != null)
            {
                next.Previous = e;
            }

            e.Previous = previous;
            e.Next = next;
            e.SetAbsoluteTicks(newPosition);

            if (newPosition < head.AbsoluteTicks)
            {
                head = e;
            }

            if (newPosition > tail.AbsoluteTicks)
            {
                tail = e;
            }

            endOfTrackMidiEvent.SetAbsoluteTicks(Length);
            endOfTrackMidiEvent.Previous = tail;

            #region Invariant

            AssertValid();

            #endregion
        }

        [Conditional("DEBUG")]
        private void AssertValid()
        {
            int c = 1;
            MidiEvent current = head;
            int ticks = 1;

            while (current != null)
            {
                ticks += current.DeltaTicks;

                if (current.Previous != null)
                {
                    Debug.Assert(current.AbsoluteTicks >= current.Previous.AbsoluteTicks);
                    Debug.Assert(current.DeltaTicks == current.AbsoluteTicks - current.Previous.AbsoluteTicks);
                }

                if (current.Next == null)
                {
                    Debug.Assert(tail == current);
                }

                current = current.Next;

                c++;
            }

            ticks += EndOfTrackOffset;

            Debug.Assert(ticks == Length, "Length mismatch");
            Debug.Assert(c == Count, "Count mismatch");
        }

        #endregion


        #region Properties


        private Sanford.Multimedia.Midi.Score.Clef clef = Score.Clef.None;
        public Score.Clef Clef
        {
            get { return clef; }
            set { clef = value; }
        }

        public List<MidiNote> Notes
        {
            get { return notes; }
        }

        // Split track into 2 tracks (left hand, right hand)
        private bool splithands;
        public bool SplitHands
        {
            get { return splithands; }
            set { splithands = value; }
        }

        private bool visible;
        public bool Visible
        {
            get
            { return visible; }
            set
            { visible = value; }
        }

        private int tempo;
        public int Tempo
        {
            get
            { return tempo; }
            set
            { tempo = value; }
        }

        private int numerator;
        public int Numerator
        {
            get
            { return numerator; }
            set
            { numerator = value; }
        }

        private int denominator;
        public int Denominator
        {
            get
            { return denominator; }
            set
            { denominator = value; }
        }

        private bool containsnotes;
        public bool ContainsNotes
        {
            get
            {
                return containsnotes;
            }
            set
            {
                containsnotes = value;
            }
        }

        private int volume;
        public int Volume
        {
            get
            {
                return volume;
            }
            set
            {
                volume = value;
            }
        }

        private int programchange;
        public int ProgramChange
        {
            get
            {
                return programchange;
            }
            set
            {
                programchange = value;
            }
        }

        private int midichannel;
        public int MidiChannel
        {
            get
            {
                return midichannel;
            }
            set
            {
                midichannel = value;
            }
        }

        private string instrumentname;
        public string InstrumentName
        {
            get
            {
                return instrumentname;
            }
            set
            {
                instrumentname = value;
            }
        }

        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        private int reverb;
        public int Reverb
        {
            get { return reverb; }
            set { reverb = value; }
        }

        private int pan = 64;
        public int Pan
        {
            get { return pan; }
            set { pan = value; }
        }


        private bool maximized = true;
        public bool Maximized
        {
            get { return maximized; }
            set { maximized = value; }
        }

        #region lyrics

        //FAB: 29/05/2014
        public class Lyric
        {
            public enum Types
            {
                Text = 1,
                LineFeed = 2,
                Paragraph = 3,
            }

            public Types Type { get; set; }
            public string Element { get; set; }
            public int TicksOn { get; set; }
            public int TicksOff { get; set; }
        }
        // Paroles trouvées en mode texte
        public List<Lyric> LyricsText = new List<Lyric>();
        // Paroles trouvées dans de véritables lyrics 
        public List<Lyric> Lyrics = new List<Lyric>();

        // Track has lyrics (text or lyric)?       
        public bool HasLyrics
        {
            get
            {
                return (totallyricst != null || totallyricsl != null);
            }
        }

        // All lyrics of type Text
        private string totallyricst;
        public string TotalLyricsT
        {
            get
            {
                return totallyricst;
            }
            set
            {
                totallyricst = value;
            }
        }

        // All lyrics of type lyric
        private string totallyricsl;
        public string TotalLyricsL
        {
            get
            {
                return totallyricsl;
            }
            set
            {
                totallyricsl = value;
            }
        }

        #endregion

        /// <summary>
        /// Gets the number of MidiEvents in the Track.
        /// </summary>
        public int Count
        {
            get
            {
                return count;
            }
        }

        /// <summary>
        /// Gets the length of the Track in ticks.
        /// </summary>
        public int Length
        {
            get
            {
                int length = EndOfTrackOffset;

                if(tail != null)
                {
                    length += tail.AbsoluteTicks;
                }

                return length + 1;
            }
        }

        /// <summary>
        /// Gets or sets the end of track meta message position offset.
        /// </summary>
        public int EndOfTrackOffset
        {
            get
            {
                return endOfTrackOffset;
            }
            set
            {
                #region Require

                if(value < 0)
                {
                    Console.WriteLine("ERROR: End of track offset out of range");
                    //throw new ArgumentOutOfRangeException("EndOfTrackOffset", value,
                    //    "End of track offset out of range.");
                    value = 0;
                }

                #endregion

                endOfTrackOffset = value;

                endOfTrackMidiEvent.SetAbsoluteTicks(Length);
            }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the Track.
        /// </summary>
        public object SyncRoot
        {
            get
            {
                return this;
            }
        }

        #endregion

        #endregion
    }
}

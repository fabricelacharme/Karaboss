using System;
using Un4seen.Bass;
using System.IO;
using System.Drawing;
using Karaboss.Properties;
using System.Windows.Forms;
using Un4seen.Bass.AddOn.Tags;
using TagLib;
using TagLib.Id3v2;
using System.Collections.Generic;
using Karaboss.Mp3.Mp3Lyrics;

namespace Karaboss.Mp3
{

    public delegate void EndingSyncHandler(int handle, int channel, int data, System.IntPtr user);
    
    public class Mp3Player
    {    
        #region events
        // Playing completed
        public SYNCPROC OnEndingSync;
        public event EndingSyncHandler PlayingCompleted;

        #endregion events

  
        private string _FileName;
        private int _stream;
        private bool mBassInitalized;


        #region properties
        
        // Image of song
        private Image _albumartimage;
        public Image AlbumArtImage { get { return _albumartimage; } }

        public double Position { get { return GetPosition(); } }

        private long _byteslen;
        public long BytesLen { get { return _byteslen; } }

        // Duration in seconds (org TagLib)
        private double _length;
        public double Length { get { return _length; } }

        // Duration in seconds (org Bass)
        private double _seconds;
        public double Seconds { get { return _seconds; } }

        // Frequency
        private float _frequency;
        public float Frequency { get { return _frequency; } }

        // Volume of song
        public int Volume { get { return GetVolume(); } }

        private TAG_INFO _tags;
        public TAG_INFO Tags { get { return _tags; } }

        private TagLib.Tag _tag;
        public TagLib.Tag Tag { get { return _tag; } }

        private SynchronisedLyricsFrame _synclyricsframe;
        public SynchronisedLyricsFrame SyncLyricsFrame { get { return _synclyricsframe; } }
       
        #endregion properties


        /// <summary>
        /// Constructor
        /// </summary>
        public Mp3Player()
        {            
            if (!InitBass()) return;            
        }

        public Mp3Player(string FileName)
        {
            _FileName = FileName;
            if (InitBass())
            {                
                Load(_FileName);
            }
        }


        /// <summary>
        /// Initialize Bass: this must be done only once, otherwise you will get random BASS_HANDLE_ERROR errors 
        /// </summary>
        private bool InitBass()
        {
            try
            {
                string BassRegistrationEmail = Settings.Default.BassRegistrationEmail;
                string BassRegistrationKey = Settings.Default.BassRegistrationKey;

                // Add registration key here if you have a license
                BassNet.Registration(BassRegistrationEmail, BassRegistrationKey);

                // Initalize with frequency = 44100
                if (Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
                {
                    mBassInitalized = true;
                    return true;
                }
                else
                {
                    MessageBox.Show("Unable to initialize the audio playback system. " + Bass.BASS_ErrorGetCode());
                    mBassInitalized = false;
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to initialize the audio playback system. " + ex.Message);
                mBassInitalized = false;
                return false;
            }            
        }
                 
           
        /// <summary>
        /// Load mp3 song
        /// </summary>
        /// <param name="FileName"></param>
        /// <exception cref="Exception"></exception>
        public void Load(string FileName)
        {            
            if (!mBassInitalized) return;
            
            _stream = 0;
            _stream = Bass.BASS_StreamCreateFile(FileName, 0, 0, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_PRESCAN);            
            _stream = Un4seen.Bass.AddOn.Fx.BassFx.BASS_FX_TempoCreate(_stream, BASSFlag.BASS_FX_FREESOURCE | BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_SAMPLE_LOOP);
            //_stream = Bass.BASS_StreamCreateFile(FileName, 0, 0, BASSFlag.BASS_DEFAULT);

            if (_stream != 0)
            {
                // Create event for song playing completed                    
                OnEndingSync = new SYNCPROC(HandlePlayingCompleted);
                Bass.BASS_ChannelSetSync(_stream, BASSSync.BASS_SYNC_END | BASSSync.BASS_SYNC_MIXTIME, 0, OnEndingSync, IntPtr.Zero);

                // Get frequency (usually 44100)
                Bass.BASS_ChannelGetAttribute(_stream, BASSAttribute.BASS_ATTRIB_FREQ, ref _frequency);

                // Get length in bytes
                _byteslen = Bass.BASS_ChannelGetLength(_stream, BASSMode.BASS_POS_BYTE);
                    
                // Get duration in seconds
                _seconds = Bass.BASS_ChannelBytes2Seconds(_stream, _byteslen);

                _tags = GetTagsFromFile(FileName);
                
                //Console.WriteLine("*** Player initialized");

            }
            else
            {                
                MessageBox.Show(String.Format("Stream error: {0}", Bass.BASS_ErrorGetCode()), "Karaboss", MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            
        }

        private void HandlePlayingCompleted(int handle, int channel, int data, IntPtr user)
        {
            //Stop();
            PlayingCompleted?.Invoke(handle, channel, data, user);
        }
    
        /// <summary>
        /// Play mp3
        /// </summary>
        public void Play(string FileName)
        {
            Stop();

            //if (InitBass())
            //{
                Load(FileName);

                if (_stream != 0)
                {
                    try
                    {
                        bool success = Bass.BASS_ChannelPlay(_stream, false);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("*** Unsucessful play: " + ex.Message, "Karaboss" ,MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                }
            //}
        }

        /// <summary>
        /// Resume (same as play ?)
        /// </summary>
        public void Resume()
        {
            if (_stream == 0) return;
            try
            {
                Bass.BASS_ChannelPlay(_stream, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        /// <summary>
        /// Stop mp3
        /// </summary>
        public void Stop()
        {
            //if (_stream == 0) return;
            try
            {
                Bass.BASS_ChannelStop(_stream);
                Bass.BASS_StreamFree(_stream);
                Bass.BASS_ChannelSetPosition(_stream, 0L);
                _stream = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("*** Unsucessful stop: " + ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
            }
        }

        /// <summary>
        /// Pause mp3
        /// </summary>
        public void Pause()
        {
            if (_stream == 0) return;
            try
            {
                Bass.BASS_ChannelPause(_stream);
            }
            catch (Exception ex)
            {
                MessageBox.Show("*** Unsucessful pause: " + ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Reset player - Free resources
        /// </summary>
        public void Reset()
        {
            try
            {                
                Bass.BASS_Stop();
                Bass.BASS_StreamFree(_stream);
                Bass.BASS_Free();
                _stream = 0;
                mBassInitalized = false;                
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);                
            }
        }

        /// <summary>
        /// Get player position
        /// </summary>
        /// <returns></returns>
        private double GetPosition()
        {
            if (_stream == 0) return 0;

            try
            {
                // length in bytes
                _byteslen = Bass.BASS_ChannelGetPosition(_stream, BASSMode.BASS_POS_BYTE);
                
                if (_byteslen == -1)
                {
                    //Console.WriteLine( string.Format( "*** Unsuccessful GetPosition : {0} - at position {1}", Bass.BASS_ErrorGetCode()), _byteslen);
                    return 0;                    
                }
                               
                // the time length
                return Bass.BASS_ChannelBytes2Seconds(_stream, _byteslen);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error); return _byteslen;                                                
            }
        }

        /// <summary>
        /// Set player position
        /// </summary>
        /// <param name="pos"></param>
        public void SetPosition(double pos)
        {
            try
            {
                if (_stream == 0) return;
                Bass.BASS_ChannelSetPosition(_stream, Bass.BASS_ChannelSeconds2Bytes(_stream, pos));
                Console.WriteLine("*** SetPosition : " + pos);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Extract tags
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        private TAG_INFO GetTagsFromFile(string FileName)
        {
            if (_stream == 0) return null;
            try
            {
                return BassTags.BASS_TAG_GetFromFile(FileName);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Adjust speed
        /// </summary>
        /// <param name="amount"></param>
        public void ChangeFrequency(long amount)
        {
            if (_stream == 0) return;
            try
            {
                Bass.BASS_ChannelSetAttribute(_stream, BASSAttribute.BASS_ATTRIB_FREQ, _frequency * amount / 100);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        /// <summary>
        /// Transpose song
        /// </summary>
        /// <param name="amount"></param>
        public void AdjustPitch(float amount)
        {            

            if (_stream == 0) return;
            try
            {
                Bass.BASS_ChannelSetAttribute(_stream, BASSAttribute.BASS_ATTRIB_TEMPO_PITCH, amount);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        /// <summary>
        /// Modify volume
        /// </summary>
        public void AdjustVolume(float volume)
        {
            if (_stream == 0) return;

            try
            {
                Bass.BASS_ChannelSetAttribute(_stream, BASSAttribute.BASS_ATTRIB_VOL, volume == 0 ? 0 : (volume / 100));
            }
            catch (Exception ex)
            { Console.WriteLine(ex.Message); }
            
        }

        private int GetVolume()
        {
            if (_stream == 0) return 0;
            try
            {
                return Bass.BASS_ChannelGetLevel(_stream);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }


        #region TagLib
        // Taglib

        public void GetMp3Infos(string Path)
        {
            GetAlbumArtImage(Path);
            //GetTagsFromFile(Path);
            GetTags(Path);
        }


        private void GetSongLength(string Path)
        {            
            if (Path != null)
            {
                TagLib.File f = TagLib.File.Create(Path);
                _length = (int)f.Properties.Duration.TotalSeconds;
            }
        }

        /// <summary>
        /// Extract tags from file
        /// </summary>
        /// <param name="Path"></param>
        private void GetTags(string Path)
        {
            try
            {
                if (Path == null) return;
                TagLib.File file = TagLib.File.Create(Path);
                _tag = file.GetTag(TagTypes.Id3v2);


                // Retrieve tags from file 
                TagLib.Id3v2.Tag id3v2tag = (TagLib.Id3v2.Tag)file.GetTag(TagLib.TagTypes.Id3v2, true);

                // Retrieve the frame SyncLyrics inside the mp3 file (type of lyrics), (last param = do not create if not found)
                _synclyricsframe = Mp3LyricsMgmtHelper.GetSyncLyricsFrame(id3v2tag, SynchedTextType.Lyrics, false);

            }
            catch (Exception e) 
            { 
                MessageBox.Show(e.Message,"Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);                
                _tag = null;
            }
        }       

        private void GetAlbumArtImage(string Path)
        {
            if (Path == null) return;

            try
            {
                TagLib.File file = TagLib.File.Create(@Path);
                if (file.Tag.Pictures.Length > 0)
                {
                    var bin = (byte[])(file.Tag.Pictures[0].Data.Data);
                    _albumartimage = Image.FromStream(new MemoryStream(bin)).GetThumbnailImage(100, 100, null, IntPtr.Zero);
                }
                else
                {
                    _albumartimage = null; // = Properties.Resources.gramophone;
                }
            }
            catch (Exception ex) 
            { 
                Console.WriteLine(ex.Message);
                _albumartimage = null;
            }
            
        }
        #endregion TagLib

        // Free resources
        ~Mp3Player()
        {
            Reset();            
        }

    }
}

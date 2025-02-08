using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;
using TagLib;
using Karaboss.Lrc.NeteaseMusic;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.IO;
using System.Drawing;
using Karaboss.Properties;
using System.Windows.Forms;
using Un4seen.Bass.AddOn.Tags;

namespace Karaboss.mp3
{
    public class Mp3Player
    {

        #region events

        // Playing completed
        public SYNCPROC _OnEndingSync;
        
        #endregion events


        private int _stream;
        private bool mBassInitalized;

        //private double _position;
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

        // Image of song
        private Image _albumartimage;
        public Image AlbumArtImage { get { return _albumartimage; } }
        
        /// <summary>
        /// Constructor
        /// </summary>
        public Mp3Player()
        {
            if (!InitBass()) return;            
        }

        public Mp3Player(string FileName)
        {
            if (!InitBass()) return;
            Load(FileName);
        }

        /// <summary>
        /// Initialize Bass
        /// </summary>
        private bool InitBass()
        {
            string BassRegistrationEmail = Settings.Default.BassRegistrationEmail;
            string BassRegistrationKey = Settings.Default.BassRegistrationKey;

            // Add registration key here if you have a license
            BassNet.Registration(BassRegistrationEmail, BassRegistrationKey);

            try
            {
                // Initalize with frequency = 44100
                Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);

                mBassInitalized = true;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to initialize the audio playback system. " + ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Load mp3 song
        /// </summary>
        /// <param name="FileName"></param>
        /// <exception cref="Exception"></exception>
        public void Load(string FileName)
        {
            //stream = Bass.BASS_StreamCreateFile(location, 0, 0, BASSFlag.BASS_SAMPLE_FLOAT);
            if (!mBassInitalized) return;
            
            _stream = 0;
            _stream = Bass.BASS_StreamCreateFile(FileName, 0, 0, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_PRESCAN);
            _stream = Un4seen.Bass.AddOn.Fx.BassFx.BASS_FX_TempoCreate(_stream, BASSFlag.BASS_FX_FREESOURCE | BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_SAMPLE_LOOP);
            if (_stream != 0)
            {

                // Create event for song playing completed                    
                Bass.BASS_ChannelSetSync(_stream, BASSSync.BASS_SYNC_END | BASSSync.BASS_SYNC_MIXTIME, 0, _OnEndingSync, IntPtr.Zero);

                // Get frequency (usually 44100)
                Bass.BASS_ChannelGetAttribute(_stream, BASSAttribute.BASS_ATTRIB_FREQ, ref _frequency);

                // Get length in bytes
                _byteslen = Bass.BASS_ChannelGetLength(_stream, BASSMode.BASS_POS_BYTE);
                    
                // Get duration in seconds
                _seconds = Bass.BASS_ChannelBytes2Seconds(_stream, _byteslen);

                _tags = GetTagsFromFile(FileName);
            }
            else
            {
                throw new Exception(String.Format("Stream error: {0}", Bass.BASS_ErrorGetCode()));
            }
            
        }

        /// <summary>
        /// Play mp3
        /// </summary>
        public void Play()
        {
            if (_stream == 0) return;
            Bass.BASS_ChannelPlay(_stream, false);
        }

        /// <summary>
        /// Resume (same as play ?)
        /// </summary>
        public void Resume()
        {
            if (_stream == 0) return;
            Bass.BASS_ChannelPlay(_stream, false);
        }


        /// <summary>
        /// Stop mp3
        /// </summary>
        public void Stop()
        {
            if (_stream == 0) return;
            Bass.BASS_ChannelStop(_stream);
        }

        /// <summary>
        /// Pause mp3
        /// </summary>
        public void Pause()
        {
            if (_stream == 0) return;
            Bass.BASS_ChannelPause(_stream);
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
                Console.Write(ex.Message);
            }
        }

        /// <summary>
        /// Get player position
        /// </summary>
        /// <returns></returns>
        private double GetPosition()
        {
            if (_stream == 0) return 0;

            // length in bytes
            long byteslen = Bass.BASS_ChannelGetPosition(_stream, BASSMode.BASS_POS_BYTE);
            // the time length
            return Bass.BASS_ChannelBytes2Seconds(_stream, byteslen);
        }

        /// <summary>
        /// Set player position
        /// </summary>
        /// <param name="pos"></param>
        public void SetPosition(double pos)
        {
            if (_stream == 0) return;
            Bass.BASS_ChannelSetPosition(_stream, Bass.BASS_ChannelSeconds2Bytes(_stream, pos));
        }

        /// <summary>
        /// Extract tags
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        private TAG_INFO GetTagsFromFile(string FileName)
        {
            if (_stream == 0) return null;
            return BassTags.BASS_TAG_GetFromFile(FileName);
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
                        
            Bass.BASS_ChannelSetAttribute(_stream, BASSAttribute.BASS_ATTRIB_VOL, volume == 0 ? 0 : (volume / 100));

            //int level = Bass.BASS_ChannelGetLevel(_stream);            
        }

        private int GetVolume()
        {
            if (_stream == 0) return 0;
            return Bass.BASS_ChannelGetLevel(_stream);
        }


        #region TagLib
        // Tablib

        private void GetSongLength(string Path)
        {            
            if (Path != null)
            {
                TagLib.File f = TagLib.File.Create(Path);
                _length = (int)f.Properties.Duration.TotalSeconds;
            }
        }


        private void GetAlbumArtImage(string Path)
        {
            if (Path != null)
            {
                TagLib.File file = TagLib.File.Create(Path);
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
        }
        #endregion TagLib

        // Free resources
        ~Mp3Player()
        {
            Reset();            
        }

    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Un4seen.Bass.AddOn.Fx;
using Un4seen.Bass.AddOn.Tags;
using Un4seen.Bass.AddOn.WaDsp;
using Un4seen.Bass.Misc;
using Un4seen.Bass;

namespace Karaboss.MP3
{
    public class MusicStream : IDisposable
    {
        #region Delegates

        private SYNCPROC _playbackCrossFadeProcDelegate = null;
        private SYNCPROC _cueTrackEndProcDelegate = null;
        private SYNCPROC _metaTagSyncProcDelegate = null;
        private SYNCPROC _playBackSlideEndDelegate = null;
        private SYNCPROC _streamFreedDelegate = null;

        public delegate void MusicStreamMessageHandler(object sender, StreamAction action);
        public event MusicStreamMessageHandler MusicStreamMessage;

        #endregion

        #region Enum

        public enum StreamAction
        {
            Ended,
            InternetStreamChanged,
            Disposed,
            Crossfading,
            Freed,
        }

        public enum TAGINFOEncoding
        {
            Ansi = 0,
            Latin1 = 1,
            Utf8 = 2,
            Utf8OrLatin1 = 3
        }

        #endregion

        #region Structs
        public enum FileMainType
        {
            Unknown = 0,
            WebStream = 1,
            MODFile = 2,
            AudioFile = 3,
            CDTrack = 4,
            MidiFile = 5
        }
        public enum FileSubType
        {
            None = 0,
            ASXWebStream = 1,
            LastFmWebStream = 3
        }

        public struct FileType
        {
            public FileMainType FileMainType;
            public FileSubType FileSubType;
        }

        public struct ReplayGainInfo
        {
            public float? AlbumGain;
            public float? AlbumPeak;
            public float? TrackGain;
            public float? TrackPeak;
        }

        #endregion

        #region Variables

        private int _stream = 0;
        internal static FileType _fileType;
        private BASS_CHANNELINFO _channelInfo;
        private string _filePath;

        private List<int> _streamEventSyncHandles = new List<int>();
        private int _cueTrackEndEventHandler;

        private TAG_INFO _tagInfo;
        private MusicTag _musicTag = null;
        private bool _crossFading = false;

        private ReplayGainInfo _replayGainInfo = new ReplayGainInfo();

        // DSP related Variables
        private DSP_Gain _gain = null;
        private BASS_BFX_DAMP _damp = null;
        private BASS_BFX_COMPRESSOR2 _comp = null;
        private int _dampPrio = 3;
        private int _compPrio = 2;

        private Dictionary<string, int> _waDspPlugins = new Dictionary<string, int>();

        private bool _disposedMusicStream = false;
        private bool _temporaryStream = false;

        #endregion

        #region Properties

        /// <summary>
        /// Returns the Stream Handle
        /// </summary>
        public int BassStream
        {
            get { return _stream; }
        }

        /// <summary>
        /// Returns the Filepath of the Stream
        /// </summary>
        public string FilePath
        {
            get { return _filePath; }
        }

        /// <summary>
        /// Returns the Channel Info
        /// </summary>
        public BASS_CHANNELINFO ChannelInfo
        {
            get { return _channelInfo; }
        }

        /// <summary>
        /// Indicates that the stream is already disposed.
        /// </summary>
        public bool IsDisposed
        {
            get { return _disposedMusicStream; }
        }

        /// <summary>
        /// Returns the FileType of the Stream
        /// </summary>
        public FileType Filetype
        {
            get { return _fileType; }
        }

        #region Playback Related Properties

        /// <summary>
        /// Returns the Playback status of the stream
        /// </summary>
        public bool IsPlaying
        {
            get { return Bass.BASS_ChannelIsActive(_stream) == BASSActive.BASS_ACTIVE_PLAYING; }
        }

        /// <summary>
        /// The stream is Crossfading
        /// </summary>
        public bool IsCrossFading
        {
            get { return _crossFading; }
        }

        /// <summary>
        /// Return Total Seconds of the Stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public double TotalStreamSeconds
        {
            get
            {
                if (_stream == 0)
                {
                    return 0;
                }

                return Bass.BASS_ChannelBytes2Seconds(_stream, Bass.BASS_ChannelGetLength(_stream));
            }
        }

        /// <summary>
        /// Retrieve the elapsed time
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public double StreamElapsedTime
        {
            get
            {
                if (_stream == 0)
                {
                    return 0;
                }

                // position in bytes
                long pos = Bass.BASS_ChannelGetPosition(_stream);

                // the elapsed time length
                double elapsedtime = Bass.BASS_ChannelBytes2Seconds(_stream, pos);
                return elapsedtime;
            }
        }

        /// <summary>
        /// Returns the Tag Info set from an Internet Stream
        /// </summary>
        public TAG_INFO StreamTags
        {
            get { return _tagInfo; }
        }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a MusicStream object
        /// </summary>
        /// <param name="filePath">The Path of the song</param>
        public MusicStream(string filePath) : this(filePath, false)
        {
        }

        /// <summary>
        /// Creates a Musicstream object
        /// </summary>
        /// <param name="filePath">The Path of the song</param>
        /// <param name="temporaryStream">Indicates that the stream is just temporary</param>
        public MusicStream(string filePath, bool temporaryStream)
        {
            _temporaryStream = temporaryStream;
            _fileType.FileMainType = FileMainType.Unknown;
            _channelInfo = new BASS_CHANNELINFO();
            _filePath = filePath;

            _playbackCrossFadeProcDelegate = new SYNCPROC(PlaybackCrossFadeProc);
            _cueTrackEndProcDelegate = new SYNCPROC(CueTrackEndProc);
            _metaTagSyncProcDelegate = new SYNCPROC(MetaTagSyncProc);
            _streamFreedDelegate = new SYNCPROC(StreamFreedProc);

            CreateStream();
        }

        #endregion

        #region Private Methods

        #region logs
        private void LogInfo(string v, string message = null)
        {
            Console.WriteLine(string.Format(v, message));
        }

        private void LogError(string v, string message = null)
        {
            Console.WriteLine(string.Format(v, message));
        }

        private void LogDebug(string v, int message = 0)
        {
            Console.WriteLine(string.Format(v, message));
        }

        #endregion logs


        /// <summary>
        /// Create the stream for the file assigned to the Musicstream
        /// </summary>
        private void CreateStream()
        {
            if (!_temporaryStream)
            {
                LogInfo("BASS: ---------------------------------------------");
                LogInfo("BASS: Creating BASS audio stream");
            }

            BASSFlag streamFlags = BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE;
                                                       
               
            _stream = Bass.BASS_StreamCreateFile(_filePath, 0, 0, streamFlags);

            if (!_temporaryStream)
            {
                // Read the Tag
                _musicTag = TagReader.ReadTag(_filePath);
            }
                                                        

            if (_stream == 0)
            {
                LogError(string.Format( "BASS: Unable to create Stream for {0}.  Reason: {1}.", _filePath, Enum.GetName(typeof(BASSError), Bass.BASS_ErrorGetCode())));
                return;
            }

           

            _channelInfo = Bass.BASS_ChannelGetInfo(_stream);
            if (Bass.BASS_ErrorGetCode() != BASSError.BASS_OK)
            {
                LogError(string.Format( "BASS: Unable to get information for stream {0}.  Reason: {1}.", _filePath,
                              Enum.GetName(typeof(BASSError), Bass.BASS_ErrorGetCode())));
                return;
            }

            // This stream has just been created to check upfront, the sample rate
            // We don't need further processing of this stream
            if (_temporaryStream)
            {
                return;
            }

            LogInfo("BASS: Stream Information");
            LogInfo("BASS: ---------------------------------------------");
            LogInfo("BASS: File: {0}", _filePath);
            //LogDebug("BASS: Type of Stream: {0}", _channelInfo.ctype.ToString());
            LogInfo("BASS: Number of Channels: {0}", _channelInfo.chans.ToString());
            LogInfo("BASS: Stream Samplerate: {0}", _channelInfo.freq.ToString());
            //LogDebug("BASS: Stream Flags: {0}", _channelInfo.flags.ToString());
            LogInfo("BASS: ---------------------------------------------");

            LogDebug("BASS: Registering stream playback events");
            RegisterPlaybackEvents();

            //AttachDspToStream();

            //if (Config.EnableReplayGain && _musicTag != null)
            //{
               SetReplayGain();
            //}
            LogInfo("BASS: Successfully created BASS audio stream");
            LogInfo("BASS: ---------------------------------------------");
        }

        /// <summary>
        /// Sets the ReplayGain Value, which is read from the Tag of the file
        /// </summary>
        private void SetReplayGain()
        {
            _replayGainInfo.AlbumGain = null;
            _replayGainInfo.AlbumPeak = null;
            _replayGainInfo.TrackGain = null;
            _replayGainInfo.TrackPeak = null;

            _replayGainInfo.AlbumGain = ParseReplayGainTagValue(_musicTag.ReplayGainAlbum);
            _replayGainInfo.AlbumPeak = ParseReplayGainTagValue(_musicTag.ReplayGainAlbumPeak);
            _replayGainInfo.TrackGain = ParseReplayGainTagValue(_musicTag.ReplayGainTrack);
            _replayGainInfo.TrackPeak = ParseReplayGainTagValue(_musicTag.ReplayGainTrackPeak);

            if (_replayGainInfo.TrackGain.HasValue || _replayGainInfo.AlbumGain.HasValue)
            {
                LogDebug(string.Format( "BASS: Replay Gain Data: Track Gain={0}dB, Track Peak={1}, Album Gain={2}dB, Album Peak={3}",
                    _replayGainInfo.TrackGain,
                    _replayGainInfo.TrackPeak,
                    _replayGainInfo.AlbumGain,
                    _replayGainInfo.AlbumPeak));
            }
            else
            {
                LogDebug("BASS: No Replay Gain Information found in stream tags");
            }

            float? gain = null;

            if (Config.EnableAlbumReplayGain && _replayGainInfo.AlbumGain.HasValue)
            {
                gain = _replayGainInfo.AlbumGain;
            }
            else if (_replayGainInfo.TrackGain.HasValue)
            {
                gain = _replayGainInfo.TrackGain;
            }

            if (gain.HasValue)
            {
                LogDebug(string.Format("BASS: Setting Replay Gain to {0}dB", gain.Value));
                _gain = new DSP_Gain();
                _gain.ChannelHandle = _stream;
                _gain.Gain_dBV = gain.Value;
                _gain.Start();
            }

        }

        /// <summary>
        /// Removes the "dB" value from the Replaygain tag
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private float? ParseReplayGainTagValue(string s)
        {
            if (s.Length == 0)
            {
                return null;
            }

            // Remove "dB"
            int pos = s.IndexOf(" ");
            if (pos > -1)
                s = s.Substring(0, pos);

            NumberFormatInfo formatInfo = new NumberFormatInfo();
            formatInfo.NumberDecimalSeparator = ".";
            formatInfo.PercentGroupSeparator = ",";
            formatInfo.NegativeSign = "-";

            float f;
            if (float.TryParse(s, NumberStyles.Number, formatInfo, out f))
                return new float?(f);
            else
                return new float?();
        }

       

      

        #endregion

        #region Public Methods

        /// <summary>
        /// Slide in the Channel over the Defined Crossfade intervall
        /// </summary>
        public void SlideIn()
        {
            if (Config.CrossFadeIntervalMs > 0)
            {
                // Reduce the stream volume to zero so we can fade it in...
                Bass.BASS_ChannelSetAttribute(_stream, BASSAttribute.BASS_ATTRIB_VOL, 0);

                // Fade in from 0 to 1 over the Config.CrossFadeIntervalMs duration
                Bass.BASS_ChannelSlideAttribute(_stream, BASSAttribute.BASS_ATTRIB_VOL, 1, Config.CrossFadeIntervalMs);
            }
        }

        /// <summary>
        /// Fade out and Stop the Song
        /// </summary>
        /// <param name="stream"></param>
        public void FadeOutStop()
        {
            new Thread(() =>
            {
                LogDebug(string.Format("BASS: FadeOutStop of stream {0}", _filePath));

                if (!IsPlaying)
                {
                    return;
                }

                double crossFadeSeconds = 0.0;

                if (Config.CrossFadeIntervalMs > 0)
                {
                    crossFadeSeconds = crossFadeSeconds / 1000.0;


                    if ((TotalStreamSeconds - (StreamElapsedTime + crossFadeSeconds) > -1))
                    {
                        Bass.BASS_ChannelSlideAttribute(_stream, BASSAttribute.BASS_ATTRIB_VOL, -1,
                                                        Config.CrossFadeIntervalMs);
                        while (Bass.BASS_ChannelIsSliding(_stream, BASSAttribute.BASS_ATTRIB_VOL))
                        {
                            Thread.Sleep(20);
                        }
                    }
                    else
                    {
                        Bass.BASS_ChannelStop(_stream);
                    }
                }
                else
                {
                    Bass.BASS_ChannelStop(_stream);
                }
                Dispose();
            }
              )
            { Name = "BASS FadeOut" }.Start();
        }

        /// <summary>
        /// Set the end position of a song inside a CUE file
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        public void SetCueTrackEndPos(float startPos, float endPos, bool endOnly)
        {
            if (_cueTrackEndEventHandler != 0)
            {
                Bass.BASS_ChannelRemoveSync(_stream, _cueTrackEndEventHandler);
            }

            if (!endOnly)
            {
                Bass.BASS_ChannelSetPosition(_stream, Bass.BASS_ChannelSeconds2Bytes(_stream, startPos));
            }

            if (endPos > startPos)
            {
                _cueTrackEndEventHandler = RegisterCueTrackEndEvent(Bass.BASS_ChannelSeconds2Bytes(_stream, endPos));
            }
        }

        /// <summary>
        /// Resume Playback of a Paused stream
        /// </summary>
        public void ResumePlayback()
        {
            LogDebug(string.Format("BASS: Resuming playback of paused stream for {0}", _filePath));
            if (Config.SoftStop)
            {
                Bass.BASS_ChannelSlideAttribute(_stream, BASSAttribute.BASS_ATTRIB_VOL, 1, 500);
            }
            else
            {
                Bass.BASS_ChannelSetAttribute(_stream, BASSAttribute.BASS_ATTRIB_VOL, 1);
            }
        }

        #endregion

        #region BASS SyncProcs

        /// <summary>
        /// Register the various Playback Events
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private void RegisterPlaybackEvents()
        {
            if (Config.CrossFadeIntervalMs > 0)
            {
                _streamEventSyncHandles.Add(RegisterCrossFadeEvent(_stream));
            }
            _streamEventSyncHandles.Add(RegisterStreamFreedEvent(_stream));
        }

        /// <summary>
        /// Register the Fade out Event
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private int RegisterCrossFadeEvent(int stream)
        {
            int syncHandle = 0;
            double fadeOutSeconds = Config.CrossFadeIntervalMs / 1000.0;

            long bytePos = Bass.BASS_ChannelSeconds2Bytes(stream, TotalStreamSeconds - fadeOutSeconds);

            syncHandle = Bass.BASS_ChannelSetSync(stream,
                                                  BASSSync.BASS_SYNC_POS,
                                                  bytePos, _playbackCrossFadeProcDelegate,
                                                  IntPtr.Zero);

            if (syncHandle == 0)
            {
                LogDebug(string.Format("BASS: RegisterCrossFadeEvent of stream {0} failed with error {1}", stream,
                          Enum.GetName(typeof(BASSError), Bass.BASS_ErrorGetCode())));
            }
            return syncHandle;
        }

        /// <summary>
        /// Register the CUE file Track End Event
        /// </summary>
        /// <param name="endPos"></param>
        /// <returns></returns>
        private int RegisterCueTrackEndEvent(long endPos)
        {
            int syncHandle = 0;

            syncHandle = Bass.BASS_ChannelSetSync(_stream, BASSSync.BASS_SYNC_ONETIME | BASSSync.BASS_SYNC_POS, endPos,
                                                  _cueTrackEndProcDelegate, IntPtr.Zero);

            if (syncHandle == 0)
            {
                LogDebug(string.Format("BASS: RegisterPlaybackCueTrackEndEvent of stream {0} failed with error {1}", _stream,
                          Enum.GetName(typeof(BASSError), Bass.BASS_ErrorGetCode())));
            }

            return syncHandle;
        }

        /// <summary>
        /// Register the Stream Freed Event
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private int RegisterStreamFreedEvent(int stream)
        {
            int syncHandle = 0;
            syncHandle = Bass.BASS_ChannelSetSync(stream,
                                                  BASSSync.BASS_SYNC_FREE | BASSSync.BASS_SYNC_MIXTIME,
                                                  0, _streamFreedDelegate,
                                                  IntPtr.Zero);

            if (syncHandle == 0)
            {
                LogDebug(string.Format("BASS: RegisterStreamFreedEvent of stream {0} failed with error {1}", stream,
                          Enum.GetName(typeof(BASSError), Bass.BASS_ErrorGetCode())));
            }
            return syncHandle;
        }

        /// <summary>
        /// Fade Out  Procedure
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="stream"></param>
        /// <param name="data"></param>
        /// <param name="userData"></param>
        private void PlaybackCrossFadeProc(int handle, int stream, int data, IntPtr userData)
        {
            new Thread(() =>
            {
                try
                {
                    LogDebug(string.Format("BASS: X-Fading out stream {0}", _filePath));

                    // We want to get informed, when Crossfading has ended
                    _playBackSlideEndDelegate = new SYNCPROC(SlideEndedProc);
                    Bass.BASS_ChannelSetSync(stream, BASSSync.BASS_SYNC_SLIDE, 0, _playBackSlideEndDelegate,
                                             IntPtr.Zero);

                    _crossFading = true;
                    Bass.BASS_ChannelSlideAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, 0,
                                                    Config.CrossFadeIntervalMs);
                }
                catch (AccessViolationException ex)
                {
                    LogError("BASS: Caught AccessViolationException in Crossfade Proc {0}", ex.Message);
                }
            }
            )
            { Name = "BASS X-Fade" }.Start();
        }

        /// <summary>
        /// This Callback Procedure is called by BASS, once a Slide Ended.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="channel"></param>
        /// <param name="data"></param>
        /// <param name="user"></param>
        private void SlideEndedProc(int handle, int channel, int data, IntPtr user)
        {
            new Thread(() =>
            {
                _crossFading = false;
                LogDebug("BASS: Fading of stream finished.");
                if (MusicStreamMessage != null)
                {
                    MusicStreamMessage(this, StreamAction.Ended);
                }
            }
            )
            { Name = "BASS X-FadeEnded" }.Start();
        }

        /// <summary>
        /// CUE Track End Procedure
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="stream"></param>
        /// <param name="data"></param>
        /// <param name="userData"></param>
        private void CueTrackEndProc(int handle, int stream, int data, IntPtr userData)
        {
            new Thread(() =>
            {
                LogDebug("BASS: CueTrackEndProc of stream {0}", stream);

                if (MusicStreamMessage != null)
                {
                    MusicStreamMessage(this, StreamAction.Crossfading);
                }

                bool removed = Bass.BASS_ChannelRemoveSync(stream, handle);
                if (removed)
                {
                    LogDebug("BassAudio: *** BASS_ChannelRemoveSync in CueTrackEndProc");
                }
            }
             )
            { Name = "BASS CueEnd" }.Start();
        }

        /// <summary>
        /// Gets the tags from the Internet Stream.
        /// </summary>
        /// <param name="stream"></param>
        private void SetStreamTags(int stream)
        {
            string[] tags = Bass.BASS_ChannelGetTagsICY(stream);
            if (tags != null)
            {
                foreach (string item in tags)
                {
                    if (item.ToLowerInvariant().StartsWith("icy-name:"))
                    {
                        //GUIPropertyManager.SetProperty("#Play.Current.Album", item.Substring(9));
                    }

                    if (item.ToLowerInvariant().StartsWith("icy-genre:"))
                    {
                        //GUIPropertyManager.SetProperty("#Play.Current.Genre", item.Substring(10));
                    }

                    LogInfo("BASS: Connection Information: {0}", item);
                }
            }
            else
            {
                tags = Bass.BASS_ChannelGetTagsHTTP(stream);
                if (tags != null)
                {
                    foreach (string item in tags)
                    {
                        LogInfo("BASS: Connection Information: {0}", item);
                    }
                }
            }
        }

        /// <summary>
        /// This Callback Procedure is called by BASS, once a song changes.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="channel"></param>
        /// <param name="data"></param>
        /// <param name="user"></param>
        private void MetaTagSyncProc(int handle, int channel, int data, IntPtr user)
        {
            new Thread(() =>
            {
                // BASS_SYNC_META is triggered on meta changes of SHOUTcast streams
                if (_tagInfo.UpdateFromMETA(Bass.BASS_ChannelGetTags(channel, BASSTag.BASS_TAG_META), true,
                                            false))
                {
                    GetMetaTags();
                }
            }
            )
            { Name = "BASS MetaSync" }.Start();
        }

        /// <summary>
        /// Set the Properties out of the Tags
        /// </summary>
        private void GetMetaTags()
        {
            // There seems to be an issue with setting correctly the title via taginfo
            // So let's filter it out ourself
            string title = _tagInfo.title;
            int streamUrlIndex = title.IndexOf("';StreamUrl=", StringComparison.Ordinal);
            if (streamUrlIndex > -1)
            {
                title = _tagInfo.title.Substring(0, streamUrlIndex);
            }
            streamUrlIndex = title.IndexOf("';", StringComparison.Ordinal);
            if (streamUrlIndex > -1 && streamUrlIndex == title.Length - 2)
            {
                title = _tagInfo.title.Substring(0, streamUrlIndex);
            }
            streamUrlIndex = title.IndexOf(";", StringComparison.Ordinal);
            if (streamUrlIndex > -1 && streamUrlIndex == title.Length - 1)
            {
                title = _tagInfo.title.Substring(0, streamUrlIndex);
            }

            LogDebug(string.Format("BASS: Internet Stream. New Song: {0} - {1}", _tagInfo.artist, title));
            
            // and display what we get
            /*
            GUIPropertyManager.SetProperty("#Play.Current.Album", _tagInfo.album);
            GUIPropertyManager.SetProperty("#Play.Current.Artist", _tagInfo.artist);
            GUIPropertyManager.SetProperty("#Play.Current.Title", title);
            GUIPropertyManager.SetProperty("#Play.Current.Comment", _tagInfo.comment);
            GUIPropertyManager.SetProperty("#Play.Current.Genre", _tagInfo.genre);
            GUIPropertyManager.SetProperty("#Play.Current.Year", _tagInfo.year);
            */

            if (MusicStreamMessage != null)
            {
                MusicStreamMessage(this, StreamAction.InternetStreamChanged);
            }
        }

        /// <summary>
        /// This Callback Procedure is called by BASS when a stream is freed.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="channel"></param>
        /// <param name="data"></param>
        /// <param name="user"></param>
        private void StreamFreedProc(int handle, int channel, int data, IntPtr user)
        {
            {
                if (MusicStreamMessage != null)
                {
                    MusicStreamMessage(this, StreamAction.Freed);
                }
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_disposedMusicStream)
            {
                return;
            }

            lock (this)
            {
                _disposedMusicStream = true;

                LogDebug(string.Format("BASS: Disposing Music Stream {0}", _filePath));

                // Free Winamp resources)
                try
                {
                    // Some Winamp dsps might raise an exception when closing
                    foreach (int waDspPlugin in _waDspPlugins.Values)
                    {
                        BassWaDsp.BASS_WADSP_Stop(waDspPlugin);
                    }
                }
                catch (Exception ex)
                {
                    LogError("MusicStream: Dispose {0}", ex.Message);
                }

            }
        }

        #endregion
    }
}

using Karaboss.Properties;
using PrgAutoUpdater;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Un4seen.Bass.AddOn.Cd;

namespace Karaboss.MP3
{
    public class g_Player
    {
        public delegate void StoppedHandler(MediaType type, int stoptime, string filename);

        public delegate void EndedHandler(MediaType type, string filename);

        public delegate void StartedHandler(MediaType type, string filename);

        public delegate void ChangedHandler(MediaType type, int stoptime, string filename);

        public delegate void AudioTracksReadyHandler();


        public static event ChangedHandler PlayBackChanged;
        public static event StoppedHandler PlayBackStopped;
        public static event EndedHandler PlayBackEnded;
        public static event StartedHandler PlayBackStarted;
        public static event AudioTracksReadyHandler AudioTracksReady;

        public enum MediaType //copy present in RefreshRateChanger
        {
            Video,
            TV,
            Radio,
            RadioRecording,
            Music,
            Recording,
            Unknown
        };

        #region variables

        //private static MediaInfoWrapper _mediaInfo = null;
        private static int _currentStep = 0;
        private static int _currentStepIndex = -1;
        private static DateTime _seekTimer = DateTime.MinValue;
        private static IPlayer _player = null;
        private static IPlayer _prevPlayer = null;
        //private static SubTitles _subs = null;
        private static bool _isInitialized = false;
        private static string _currentFilePlaying = "";
        private static string _currentMediaInfoFilePlaying = "";
        private static MediaType _currentMedia;
        public static MediaType _currentMediaForBassEngine;
        //private static IPlayerFactory _factory;
        public static bool Starting = false;
        //private static ArrayList _seekStepList = new ArrayList();
        private static int _seekStepTimeout;
        public static bool configLoaded = false;
        private static string[] _driveSpeedCD;
        private static string[] _driveSpeedDVD;
        private static string[] _disableCDSpeed;
        private static string[] _disableDVDSpeed;
        private static int _driveCount = 0;
        private static string _driveLetters;
        private static bool driveSpeedLoaded = false;
        private static bool driveSpeedReduced = false;
        private static bool driveSpeedControlEnabled = false;
        private static string _currentTitle = ""; //actual program metadata - usefull for tv - avoids extra DB lookups
        private static bool _pictureSlideShow = false;
        private static bool _picturePlaylist = false;
        private static bool _forceplay = false;
        private static bool _isExtTS = false;

        private static string _currentDescription = "";
        //actual program metadata - usefull for tv - avoids extra DB Lookups. 

        private static string _currentFileName = ""; //holds the actual file being played. Usefull for rtsp streams. 
        private static double[] _chapters = null;
        private static string[] _chaptersname = null;
        private static double[] _jumpPoints = null;
        private static bool _autoComSkip = false;
        private static bool _loadAutoComSkipSetting = true;
        private static bool _BDInternalMenu = false;

        private static string _externalPlayerExtensions = string.Empty;
        private static int _titleToDB = 0;

        public static readonly AutoResetEvent SeekFinished = new AutoResetEvent(false);

        /// <param name="default Blu-ray remuxed">BdRemuxTitle</param>
        public const int BdRemuxTitle = 900;

        /// <param name="default Blu-ray Title">BdDefaultTitle</param>
        public const int BdDefaultTitle = 1000;

        #endregion


        #region ctor/dtor

        // singleton. Dont allow any instance of this class
        private g_Player()
        {
            //_factory = new PlayerFactory();
        }

        static g_Player() { }

        public static IPlayer Player
        {
            get { return _player; }
        }

        /*
        public static IPlayerFactory Factory
        {
            get { return _factory; }
            set { _factory = value; }
        }
        */

        public static string currentTitle
        {
            get { return _currentTitle; }
            set { _currentTitle = value; }
        }

        public static string currentFileName
        {
            get { return _currentFileName; }
            set { _currentFileName = value; }
        }

        public static string currentDescription
        {
            get { return _currentDescription; }
            set { _currentDescription = value; }
        }

        public static MediaType currentMedia
        {
            get { return _currentMedia; }
            set { _currentMedia = value; }
        }

        public static string currentFilePlaying
        {
            get { return _currentFilePlaying; }
            set { _currentFilePlaying = value; }
        }

        public static string currentMediaInfoFilePlaying
        {
            get { return _currentMediaInfoFilePlaying; }
            set { _currentMediaInfoFilePlaying = value; }
        }

        public static bool ExternalController
        {
            get;
            set;
        }

        public static bool ForcePauseWebStream
        {
            get;
            set;
        }

        #endregion

        #region public members

        

        internal static void OnAudioTracksReady()
        {
            if (AudioTracksReady != null) // FIXME: the event handler might not be set if TV plugin is not installed!
            {
                AudioTracksReady();
            }
            else
            {
                CurrentAudioStream = 0;
            }
        }

        //called when current playing file is stopped
        public static void OnChanged(string newFile)
        {
            if (newFile == null || newFile.Length == 0)
            {
                return;
            }

            if (!newFile.Equals(CurrentFile))
            {
                //yes, then raise event
                LogInfo(string.Format("g_Player.OnChanged()"));
                if (PlayBackChanged != null)
                {
                    
                    PlayBackChanged(_currentMedia, (int)CurrentPosition,
                                    (!String.IsNullOrEmpty(currentFileName) ? currentFileName : CurrentFile));
                    currentFileName = String.Empty;
                }
            }
        }

        //called when current playing file is stopped
        public static void OnStopped()
        {
            //check if we're playing
            if (Playing && PlayBackStopped != null)
            {
                //yes, then raise event
                LogInfo(string.Format("g_Player.OnStopped()"));
                if (PlayBackStopped != null)
                {
                    
                    
                    PlayBackStopped(_currentMedia, (int)CurrentPosition,
                                    (!String.IsNullOrEmpty(currentFileName) ? currentFileName : CurrentFile));
                    currentFileName = String.Empty;
                    //_mediaInfo = null;
                }
            }
        }

        //called when current playing file ends
        public static void OnEnded()
        {
            //check if we're playing
            if (PlayBackEnded != null)
            {
                //yes, then raise event
                LogInfo(string.Format("g_Player.OnEnded()"));
                //RefreshRateChanger.AdaptRefreshRate();
                PlayBackEnded(_currentMedia, (!String.IsNullOrEmpty(currentFileName) ? currentFileName : _currentFilePlaying));
                if (_player != null && _player.Playing)
                {
                    // Don't reset currentFileName and _mediaInfo
                }
                else
                {
                    currentFileName = String.Empty;
                    //_mediaInfo = null;
                }
            }
        }

        //called when starting playing a file
        public static void OnStarted()
        {
            //check if we're playing
            if (_player == null)
            {
                return;
            }
            if (_player.Playing)
            {
                //yes, then raise event 
                _currentMedia = MediaType.Music;
                if (_player.IsTV)
                {
                    _currentMedia = MediaType.TV;
                    if (!_player.IsTimeShifting)
                    {
                        _currentMedia = MediaType.Recording;
                    }
                }
                else if (_player.IsRadio)
                {
                    _currentMedia = MediaType.Radio;
                }
                else if (_player.HasVideo)
                {
                    if (_player.ToString() != "MediaPortal.Player.BassAudioEngine")
                    {
                        _currentMedia = MediaType.Video;
                    }
                }
                LogInfo(string.Format("g_Player.OnStarted() {0} media:{1}", _currentFilePlaying, _currentMedia.ToString()));
                
            }
        }

        public static void PauseGraph()
        {
            if (_player != null)
            {
                _player.PauseGraph();
            }
        }

        public static void ContinueGraph()
        {
            if (_player != null)
            {
                _player.ContinueGraph();
            }
        }

       


      



        public static void Pause()
        {
            if (_player != null)
            {
                _currentStep = 0;
                _currentStepIndex = -1;
                _seekTimer = DateTime.MinValue;
                _player.Speed = 1; //default back to 1x speed.


                _player.Pause();
                
            }
        }

    
      

        public static void Release()
        {
            if (_player != null)
            {
                _player.Stop();
                //CachePlayer();
            }
        }

       
     

        public static int SetResumeBDTitleState
        {
            get
            {
                return _titleToDB;
            }
            set
            {
                _titleToDB = value;
            }
        }

   
      
     

       

        private static IPlayer CachePreviousPlayer(IPlayer newPlayer)
        {
            IPlayer player = newPlayer;
            if (newPlayer != null)
            {
                if (_prevPlayer != null)
                {
                    if (_prevPlayer.GetType() == newPlayer.GetType())
                    {
                        if (_prevPlayer.SupportsReplay)
                        {
                            player = _prevPlayer;
                            _prevPlayer = null;
                        }
                    }
                }
                if (_prevPlayer != null)
                {
                    //_prevPlayer.SafeDispose();
                    _prevPlayer = null;
                }
            }
            return player;
        }

       

    
       

      

      

      

        public static bool IsExternalPlayer
        {
            get
            {
                if (_player == null)
                {
                    return false;
                }
                return _player.IsExternal;
            }
        }

        public static bool IsRadio
        {
            get
            {
                if (_player == null)
                {
                    return false;
                }
                return (_player.IsRadio);
            }
        }

        public static bool IsMusic
        {
            get
            {
                if (_player == null)
                {
                    return false;
                }
                return (_currentMedia == MediaType.Music);
            }
        }

        public static bool IsPicture
        {
            get
            {
                return _pictureSlideShow;
            }
            set
            {
                _pictureSlideShow = value;
            }
        }

        public static bool IsPicturePlaylist
        {
            get
            {
                return _picturePlaylist;
            }
            set
            {
                _picturePlaylist = value;
            }
        }

        public static bool ForcePlay
        {
            get
            {
                return _forceplay;
            }
            set
            {
                _forceplay = value;
            }
        }

        public static bool IsExtTS
        {
            get
            {
                return _isExtTS;
            }
            set
            {
                _isExtTS = value;
            }
        }

        public static bool Playing
        {
            get
            {
                if (_player == null)
                {
                    return false;
                }
                if (_isInitialized)
                {
                    return false;
                }
                bool bResult = _player.Playing;
                return bResult;
            }
        }

        public static int PlaybackType
        {
            get
            {
                if (_player == null)
                {
                    return -1;
                }
                return _player.PlaybackType;
            }
        }

        public static bool Paused
        {
            get
            {
                if (_player == null)
                {
                    return false;
                }
                return _player.Paused;
            }
        }

        public static bool Stopped
        {
            get
            {
                if (_isInitialized)
                {
                    return false;
                }
                if (_player == null)
                {
                    return false;
                }
                bool bResult = _player.Stopped;
                return bResult;
            }
        }

        public static int Speed
        {
            get
            {
                if (_player == null)
                {
                    return 1;
                }
                return _player.Speed;
            }
            set
            {
                if (_player == null)
                {
                    return;
                }
                _player.Speed = value;
                _currentStep = 0;
                _currentStepIndex = -1;
                _seekTimer = DateTime.MinValue;
            }
        }

        public static string CurrentFile
        {
            get
            {
                if (_player == null)
                {
                    return "";
                }
                return _player.CurrentFile;
            }
        }

        public static int Volume
        {
            get
            {
                if (_player == null)
                {
                    return -1;
                }
                return _player.Volume;
            }
            set
            {
                if (_player != null)
                {
                    _player.Volume = value;
                }
            }
        }

      
        public static int PositionX
        {
            get
            {
                if (_player == null)
                {
                    return 0;
                }
                return _player.PositionX;
            }
            set
            {
                if (_player != null)
                {
                    _player.PositionX = value;
                }
            }
        }

        public static int PositionY
        {
            get
            {
                if (_player == null)
                {
                    return 0;
                }
                return _player.PositionY;
            }
            set
            {
                if (_player != null)
                {
                    _player.PositionY = value;
                }
            }
        }

        public static int RenderWidth
        {
            get
            {
                if (_player == null)
                {
                    return 0;
                }
                return _player.RenderWidth;
            }
            set
            {
                if (_player != null)
                {
                    _player.RenderWidth = value;
                }
            }
        }

        public static bool Visible
        {
            get
            {
                if (_player == null)
                {
                    return false;
                }
                return _player.Visible;
            }
            set
            {
                if (_player != null)
                {
                    _player.Visible = value;
                }
            }
        }

        public static int RenderHeight
        {
            get
            {
                if (_player == null)
                {
                    return 0;
                }
                return _player.RenderHeight;
            }
            set
            {
                if (_player != null)
                {
                    _player.RenderHeight = value;
                }
            }
        }

        public static double Duration
        {
            get
            {
                if (_player == null)
                {
                    return 0;
                }
                return _player.Duration;
            }
        }

        public static double CurrentPosition
        {
            get
            {
                if (_player == null)
                {
                    return 0;
                }
                return _player.CurrentPosition;
            }
        }

        public static double StreamPosition
        {
            get
            {
                if (_player == null)
                {
                    return 0;
                }
                return _player.StreamPosition;
            }
        }

        public static double ContentStart
        {
            get
            {
                if (_player == null)
                {
                    return 0;
                }
                return _player.ContentStart;
            }
        }

       

        public static double[] Chapters
        {
            get
            {
                if (_player == null && _chapters == null)
                {
                    return null;
                }
                if (_chapters != null)
                {
                    return _chapters;
                }
                else
                {
                    return _player.Chapters;
                }
            }
        }

        public static string[] ChaptersName
        {
            get
            {
                if (_player == null)
                {
                    return null;
                }
                _chaptersname = _player.ChaptersName;
                return _chaptersname;
            }
        }

        public static double[] JumpPoints
        {
            get
            {
                return _jumpPoints;
            }
        }

        public static int Width
        {
            get
            {
                if (_player == null)
                {
                    return 0;
                }
                return _player.Width;
            }
        }

        public static int Height
        {
            get
            {
                if (_player == null)
                {
                    return 0;
                }
                return _player.Height;
            }
        }

    

        public static bool _StepNowDone { get; set; }
        public static bool _SeekAbsoluteDone { get; set; }

    

      

        public static int GetSeekStep(out bool bStart, out bool bEnd)
        {
            bStart = false;
            bEnd = false;
            if (_player == null)
            {
                return 0;
            }
            int m_iTimeToStep = (int)_currentStep;
            if (_player.CurrentPosition + m_iTimeToStep <= 0)
            {
                bStart = true; //start
            }
            if (_player.CurrentPosition + m_iTimeToStep >= _player.Duration)
            {
                bEnd = true;
            }
            return m_iTimeToStep;
        }

       
        public static void WndProc(ref Message m)
        {
            if (_player == null)
            {
                return;
            }
            _player.WndProc(ref m);
        }

     
   
      

        #region Edition selection

        /// <summary>
        /// Property which returns the total number of edition streams available
        /// </summary>
        public static int EditionStreams
        {
            get
            {
                if (_player == null)
                {
                    return 0;
                }
                return _player.EditionStreams;
            }
        }

        /// <summary>
        /// Property to get/set the current edition stream
        /// </summary>
        public static int CurrentEditionStream
        {
            get
            {
                if (_player == null)
                {
                    return 0;
                }
                return _player.CurrentEditionStream;
            }
            set
            {
                if (_player != null)
                {
                    _player.CurrentEditionStream = value;
                }
            }
        }

     

        /// <summary>
        /// Property to get the type of an edition stream
        /// </summary>
        public static string EditionType(int iStream)
        {
            if (_player == null)
            {
                return Strings.Unknown;
            }

            string stream = _player.EditionType(iStream);
            return stream;
        }

        #endregion

        #region Video selection

        /// <summary>
        /// Property which returns the total number of video streams available
        /// </summary>
        public static int VideoStreams
        {
            get
            {
                if (_player == null)
                {
                    return 0;
                }
                return _player.VideoStreams;
            }
        }

        /// <summary>
        /// Property to get/set the current video stream
        /// </summary>
        public static int CurrentVideoStream
        {
            get
            {
                if (_player == null)
                {
                    return 0;
                }
                return _player.CurrentVideoStream;
            }
            set
            {
                if (_player != null)
                {
                    _player.CurrentVideoStream = value;
                }
            }
        }

     

        /// <summary>
        /// Property to get the type of an edition stream
        /// </summary>
        public static string VideoType(int iStream)
        {
            if (_player == null)
            {
                return Strings.Unknown;
            }

            string stream = _player.VideoType(iStream);
            return stream;
        }

        #endregion

        #region Postprocessing selection

        /// <summary>
        /// Property which returns true if the player is able to perform postprocessing features
        /// </summary>
        public static bool HasPostprocessing
        {
            get
            {
                if (_player == null)
                {
                    return false;
                }
                return _player.HasPostprocessing;
            }
        }

        /// <summary>
        /// Property which returns true if the player is able to perform post audio delay features
        /// </summary>
        public static bool HasAudioEngine
        {
            get
            {
                if (_player == null)
                {
                    return false;
                }
                return _player.HasAudioEngine;
            }
        }

        #endregion

        #region subtitle/audio stream selection

        /// <summary>
        /// Property which returns the total number of audio streams available
        /// </summary>
        public static int AudioStreams
        {
            get
            {
                if (_player == null)
                {
                    return 0;
                }
                return _player.AudioStreams;
            }
        }

        /// <summary>
        /// Property to get/set the current audio stream
        /// </summary>
        public static int CurrentAudioStream
        {
            get
            {
                if (_player == null)
                {
                    return 0;
                }
                return _player.CurrentAudioStream;
            }
            set
            {
                if (_player != null)
                {
                    _player.CurrentAudioStream = value;
                }
            }
        }

        /// <summary>
        /// Property to get the name for an audio stream
        /// </summary>
        public static string AudioLanguage(int iStream)
        {
            if (_player == null)
            {
                return Strings.Unknown;
            }

            string stream = _player.AudioLanguage(iStream);
            //return Util.Utils.TranslateLanguageString(stream);
            return null;
        }

        /// <summary>
        /// Property to get the type of an audio stream
        /// </summary>
        public static string AudioType(int iStream)
        {
            if (_player == null)
            {
                return Strings.Unknown;
            }

            string stream = _player.AudioType(iStream);
            return stream;
        }

        /// <summary>
        /// Property to get the total number of subtitle streams
        /// </summary>
        public static int SubtitleStreams
        {
            get
            {
                if (_player == null)
                {
                    return 0;
                }
                return _player.SubtitleStreams;
            }
        }

        /// <summary>
        /// Property to get/set the current subtitle stream
        /// </summary>
        public static int CurrentSubtitleStream
        {
            get
            {
                if (_player == null)
                {
                    return 0;
                }
                return _player.CurrentSubtitleStream;
            }
            set
            {
                if (_player != null)
                {
                    _player.CurrentSubtitleStream = value;
                }
            }
        }

      
       
        #endregion

        public static bool EnableSubtitle
        {
            get
            {
                if (_player == null)
                {
                    return false;
                }
                return _player.EnableSubtitle;
            }
            set
            {
                if (_player == null)
                {
                    return;
                }
                _player.EnableSubtitle = value;
            }
        }

        public static bool EnableForcedSubtitle
        {
            get
            {
                if (_player == null)
                {
                    return false;
                }
                return _player.EnableForcedSubtitle;
            }
            set
            {
                if (_player == null)
                {
                    return;
                }
                _player.EnableForcedSubtitle = value;
            }
        }

       

        public static void SetVideoWindow()
        {
            if (_player == null)
            {
                return;
            }
            _player.SetVideoWindow();

            //// madVR
            //if (GUIGraphicsContext.VideoRenderer != GUIGraphicsContext.VideoRendererType.madVR)
            //{
            //  _player.SetVideoWindow();
            //}
            //else if (GUIGraphicsContext.VideoRenderer == GUIGraphicsContext.VideoRendererType.madVR && Thread.CurrentThread.Name == "MPMain")
            //{
            //  _player.SetVideoWindow();
            //  GUIGraphicsContext.VideoWindowChangedDone = false;
            //}
        }

        public static void Init()
        {
            //GUIGraphicsContext.OnVideoWindowChanged += OnVideoWindowChanged;
            //GUIGraphicsContext.OnGammaContrastBrightnessChanged += OnGammaContrastBrightnessChanged;
            //GUIWindowManager.Receivers += OnMessage;
        }

    

  

        /// <summary>
        /// returns video window rectangle
        /// </summary>
        public static Rectangle VideoWindow
        {
            get
            {
                if (_player == null)
                {
                    return new Rectangle(0, 0, 0, 0);
                }
                return _player.VideoWindow;
            }
        }

        /// <summary>
        /// returns video source rectangle displayed
        /// </summary>
        public static Rectangle SourceWindow
        {
            get
            {
                if (_player == null)
                {
                    return new Rectangle(0, 0, 0, 0);
                }
                return _player.SourceWindow;
            }
        }

        public static int GetHDC()
        {
            if (_player == null)
            {
                return 0;
            }
            return _player.GetHDC();
        }

        public static void ReleaseHDC(int HDC)
        {
            if (_player == null)
            {
                return;
            }
            _player.ReleaseHDC(HDC);
        }

        public static bool CanSeek
        {
            get
            {
                if (_player == null)
                {
                    return false;
                }
                return (_player.CanSeek() && !_player.IsDVDMenu);
            }
        }

       
        /// <summary>
        /// Switches to the next audio stream.
        /// 
        /// Calls are directly pushed to the embedded player. And care 
        /// is taken not to do multiple calls to the player.
        /// </summary>
        public static void SwitchToNextAudio()
        {
            if (_player != null)
            {
                // take current stream and number of
                int streams = _player.AudioStreams;
                int current = _player.CurrentAudioStream;
                int next = current;
                bool success = false;
                // Loop over the stream, so we skip the disabled streams
                // stops if the loop is over the current stream again.
                do
                {
                    // if next stream is greater then the amount of stream
                    // take first
                    if (++next >= streams)
                    {
                        next = 0;
                    }
                    // set the next stream
                    _player.CurrentAudioStream = next;
                    // if the stream is set in, stop the loop
                    if (next == _player.CurrentAudioStream)
                    {
                        success = true;
                    }
                } while ((next != current) && (success == false));
                if (success == false)
                {
                    LogInfo(string.Format("g_Player: Failed to switch to next audiostream."));
                }
            }
        }

        //}

     
               

        private static bool IsFileUsedbyAnotherProcess(string file)
        {
            try
            {
                using (new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None)) { }
            }
            catch (System.IO.IOException exp)
            {
                LogError(string.Format("g_Player.LoadChapters() - {0}", exp.ToString()));
                return true;
            }
            return false;
        }

     
              

        private static bool CheckExtension(string filename)
        {
            char[] splitter = { ';' };
            string[] extensions = _externalPlayerExtensions.Split(splitter);

            foreach (string extension in extensions)
            {
                if (extension.Trim().Equals(Path.GetExtension(filename), StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion


        #region logs
        private static void LogInfo(string v, string message = null)
        {
            Console.WriteLine(string.Format(v, message));
        }

        private static void LogError(string v, string message = null)
        {
            Console.WriteLine(string.Format(v, message));
        }

        private static void LogDebug(string v, int message = 0)
        {
            Console.WriteLine(string.Format(v, message));
        }

        private static void LogWarn(string v, string message = null)
        {
            Console.WriteLine(string.Format(v, message));
        }

        #endregion logs

    }
}

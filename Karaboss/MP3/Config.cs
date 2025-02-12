using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass.AddOn.Cd;
using Un4seen.Bass.AddOn.Midi;
using Un4seen.Bass.AddOn.Vst;
using Un4seen.Bass;

namespace Karaboss.MP3
{
    /// <summary>
    /// This class holds the various Configuration values, which are needed by the Audio Engine
    /// and the MusicStream Object, to avoid constantly accessing the Settings in MediaPortal.xml
    /// </summary>
    public class Config
    {

        /// <summary>
        /// Selected Audio Player
        /// </summary>
        public enum AudioPlayer
        {
            Bass = 0,
            Asio = 1,
            WasApi = 2,
            DShow = 3
        }

        /// <summary>
        /// States, how the Playback is handled
        /// </summary>
        public enum PlayBackType
        {
            NORMAL = 0,
            GAPLESS = 1,
            CROSSFADE = 2
        }

        public enum MonoUpMix
        {
            None = 0,
            Stereo = 1,
            QuadraphonicPhonic = 2,
            FiveDotOne = 3,
            SevenDotOne = 4
        }

        public enum StereoUpMix
        {
            None = 0,
            QuadraphonicPhonic = 1,
            FiveDotOne = 2,
            SevenDotOne = 3
        }

        public enum FiveDotOneUpMix
        {
            None = 0,
            SevenDotOne = 1,
        }

        public enum QuadraphonicUpMix
        {
            None = 0,
            FiveDotOne = 1,
            SevenDotOne = 2
        }

        #region Variables

        private static List<int> _decoderPluginHandles = new List<int>();

        private static Config _instance = null;

        private static AudioPlayer _audioPlayer;
        private static string _soundDevice;
        private static string _soundDeviceID;

        private static int _upMixMono;
        private static int _upMixStereo;
        private static int _upMixQuadro;
        private static int _upMixFiveDotOne;

        private static int _streamVolume;
        private static int _bufferingMs;
        private static int _crossFadeIntervalMs;
      

        private static bool _softStop;
        private static bool _useSkipSteps;
        private static bool _enableReplaygain;
        private static bool _enableAlbumReplaygain;

        private static PlayBackType _playBackType;
      

        // DSP related variables
        private static bool _dspActive = false;

        // VST Related variables
        private static List<string> _vstPlugins = new List<string>();
        private static Dictionary<string, int> _vstHandles = new Dictionary<string, int>();
        

        #endregion

        #region Properties

        
        public static AudioPlayer MusicPlayer
        {
            get { return _audioPlayer; }
        }
        

        public static Config Instance
        {
            get { return _instance; }
        }

        public static string SoundDevice
        {
            get { return _soundDevice; }
        }

        public static string SoundDeviceID
        {
            get { return _soundDeviceID; }
        }

                     

        public static int StreamVolume
        {
            get { return _streamVolume; }
            set { _streamVolume = value; }
        }

        public static int BufferingMs
        {
            get { return _bufferingMs; }
            set { _bufferingMs = value; }
        }

        public static int CrossFadeIntervalMs
        {
            get { return _crossFadeIntervalMs; }
            set { _crossFadeIntervalMs = value; }
        }
     

        public static bool SoftStop
        {
            get { return _softStop; }
        }

        public static bool UseSkipSteps
        {
            get { return _useSkipSteps; }
        }

        public static bool EnableReplayGain
        {
            get { return _enableReplaygain; }
        }

        public static bool EnableAlbumReplayGain
        {
            get { return _enableAlbumReplaygain; }
        }

        public static PlayBackType PlayBack
        {
            get { return _playBackType; }
        }

       

        public static bool DSPActive
        {
            get { return _dspActive; }
        }

        public static List<string> VstPlugins
        {
            get { return _vstPlugins; }
        }

        public static Dictionary<string, int> VstHandles
        {
            get { return _vstHandles; }
        }
        public static MonoUpMix UpmixMono
        {
            get { return (MonoUpMix)_upMixMono; }
        }

        public static StereoUpMix UpmixStereo
        {
            get { return (StereoUpMix)_upMixStereo; }
        }

        public static QuadraphonicUpMix UpmixQuadro
        {
            get { return (QuadraphonicUpMix)_upMixQuadro; }
        }

        public static FiveDotOneUpMix UpmixFiveDotOne
        {
            get { return (FiveDotOneUpMix)_upMixFiveDotOne; }
        }

        #endregion

        #region Constructor

        // Singleton -- make sure we can't instantiate this class
        static Config()
        {
            _instance = new Config();
        }

        public Config()
        {
            
        }
        #endregion

        #region Private Methods

       
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

        #endregion logs

       
       
      

        #endregion
    }
}

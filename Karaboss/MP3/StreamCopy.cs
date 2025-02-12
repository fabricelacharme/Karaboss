using System;
using Un4seen.Bass.Misc;
using Un4seen.Bass;

namespace Karaboss.MP3
{
    internal class StreamCopy : BaseDSP
    {
        private int _stream;
        private BASSBuffer _streamBuffer;
        private BASSFlag _streamFlags;
        protected static BassAudioEngine _bass = null;

        internal StreamCopy(BassAudioEngine bass)
          : base()
        {
            Bass = bass;
        }

        public StreamCopy(int channel, int priority)
          : base(channel, priority, IntPtr.Zero) { }

        public override void OnChannelChanged()
        {
            this.OnStopped();
            if (base.IsAssigned)
            {
                this.OnStarted();
            }
        }

        private static BassAudioEngine Bass
        {
            get { return _bass; }
            set { _bass = value; }
        }

        public override void OnStarted()
        {
            if (Bass != null)
            {
                int channelBitwidth = base.ChannelBitwidth;
                switch (channelBitwidth)
                {
                    case 0x20:
                        this._streamFlags &= ~BASSFlag.BASS_SAMPLE_8BITS;
                        this._streamFlags |= BASSFlag.BASS_SAMPLE_FLOAT;
                        channelBitwidth = 4;
                        break;

                    case 8:
                        this._streamFlags &= ~BASSFlag.BASS_SAMPLE_FLOAT;
                        this._streamFlags |= BASSFlag.BASS_SAMPLE_8BITS;
                        channelBitwidth = 1;
                        break;

                    default:
                        this._streamFlags &= ~BASSFlag.BASS_SAMPLE_FLOAT;
                        this._streamFlags &= ~BASSFlag.BASS_SAMPLE_8BITS;
                        channelBitwidth = 2;
                        break;
                }
                this._streamBuffer = new BASSBuffer(2f, base.ChannelSampleRate, base.ChannelNumChans, channelBitwidth);
                this._stream = Bass.StreamCreate(base.ChannelSampleRate, base.ChannelNumChans, this._streamFlags,
                  null, IntPtr.Zero);
                Bass.ChannelSetLink(base.ChannelHandle, this._stream);
                if (Bass.ChannelIsActive(base.ChannelHandle) == BASSActive.BASS_ACTIVE_PLAYING)
                {
                    Bass.ChannelPlay(this._stream, false);
                }
            }
        }

        public override void OnStopped()
        {
            if (Bass != null)
            {
                Bass.ChannelRemoveLink(base.ChannelHandle, this._stream);
                Bass.StreamFree(this._stream);
                this._stream = 0;
                this.ClearBuffer();
            }
        }

        public void ClearBuffer()
        {
            if (this._streamBuffer != null)
            {
                this._streamBuffer.Clear();
            }
        }

        public override void DSPCallback(int handle, int channel, IntPtr buffer, int length, IntPtr user)
        {
            try
            {
                this._streamBuffer.Write(buffer, length);
            }
            catch (Exception ex)
            {
                //LogError("Caught Exception in DSPCallBack. {0}", ex.Message);
            }
        }

        public override string ToString()
        {
            return "StreamCopy";
        }

        // Properties
        public int Stream
        {
            get { return this._stream; }
        }

        public BASSFlag StreamCopyFlags
        {
            get { return this._streamFlags; }
            set { this._streamFlags = value; }
        }
    }
}

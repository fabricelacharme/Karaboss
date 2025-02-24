#region License

/* Copyright (c) 2018 Fabrice Lacharme
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
using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using AForge.Video.FFMPEG;
using AForge.Video;
using System.Diagnostics;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Enc;
using Un4seen.Bass.Misc;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;

namespace Karaboss
{
    public partial class frmVideoRecorder : Form
    {

        #region video declarations
        bool bCaptureVideo = true;
        bool rec = false;

        UInt32 frameCount = 0;
        VideoFileWriter writer;

        Rectangle screenArea = Rectangle.Empty;
        int width = 0;
        int height = 0;


        AForge.Video.ScreenCaptureStream streamVideo;
        Stopwatch stopWatch = new Stopwatch();


        private enum bitRate
        {
            _50kbit = 50000,
            _100kbit = 100000,
            _500kbit = 500000,
            _1000kbit = 1000000,
            _2000kbit = 2000000,
            _3000kbit = 3000000
        }

        #endregion video declarations


        string aviFileName = string.Empty;
        string mp3FileName = string.Empty;

        #region audio declarations

        public delegate void UpdateDelegate(int maxL, int maxR);
        private int stream;

        #region encode

        bool recording = false;

        private byte[] _recbuffer; //local pcm recording buffer
        MemoryStream pcmMemStrm;
        RECORDPROC _PcmRecProc;//  make global so garbage collection does not eat it.
        int _PcmRecHandle = 0;

        // Buffer containing the encoded song
        byte[] lameEncodedBytes;

        int _blogStreamHandle = 0;

        private ENCODEPROC _LameEncProc;
        private ArrayList _lameMemBuffer = new ArrayList();
        EncoderLAME lameEncoder = null;

        //STREAMPROC playPcmProc;
        int pcmImportStreamHandle = 0;

        private bool bSaveToDisk = false;
        private string strSaveFile = string.Empty;

        private bool bTryFreeBass = false;

        #endregion encode


        #endregion audio declarations


        private frmLyric frmLyrics;
        private IntPtr HwndLyrics = IntPtr.Zero;

        private frmPianoTraining FrmPianoTraining;
        private IntPtr HwndPianoTraining = IntPtr.Zero;

        public frmVideoRecorder()
        {
            InitializeComponent();


            bt_Save.Enabled = false;

            #region video
            writer = new VideoFileWriter();

            // Codec Combobox
            cb_VideoCodec.DataSource = Enum.GetValues(typeof(VideoCodec));
            cb_VideoCodec.DropDownStyle = ComboBoxStyle.DropDownList;

            // BitRate 2000kbit/s 200000 1000000
            cb_BitRate.DataSource = Enum.GetValues(typeof(bitRate));
            cb_BitRate.DropDownStyle = ComboBoxStyle.DropDownList;
            cb_BitRate.SelectedIndex = 4;
            #endregion video


            #region audio
            // Register to avoid splash screen : this is mine :-) please use yours
            BassNet.Registration("fabrice.lacharme@free.fr", "2X1632324163737");

            populateRecordDevices();
            populatePaths();

            bTryFreeBass = false;
            progressBarRecR.Maximum = 32768;
            progressBarRecL.Maximum = 32768;
            btnStopMp3.Enabled = false;
            bSaveToDisk = true;

            #endregion audio           

        }





        #region audio

        /// <summary>
        /// Display messages
        /// </summary>
        /// <param name="msg"></param>
        private void Msg(string msg)
        {
            try {
                listBox1.Items.Add(msg);
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            }
            catch (Exception er)
            {
                Console.Write(er.Message);
            }
        }

        /// <summary>
        /// List of available recording devices
        /// </summary>
        private void populateRecordDevices()
        {

            BASS_DEVICEINFO[] devs = Bass.BASS_RecordGetDeviceInfos();
            foreach (BASS_DEVICEINFO dev in devs)
            {
                if (dev.IsEnabled)
                {
                    Msg(dev.name);
                    lstRecDevices.Items.Add(dev.name);
                }
            }

            if (lstRecDevices.Items.Count > 0)
                lstRecDevices.SelectedIndex = 0;
        }

        /// <summary>
        /// Init sound BASS
        /// </summary>
        private void InitSound()
        {

            try
            {
                Bass.BASS_Free();
            }
            catch (Exception e)
            {
                string tx = e.Message;
                Console.Write("\n" + tx);
                MessageBox.Show(e.InnerException.Message);
                return;
            }

            int outdevice = -1;
            // int outdevice = 0;
            //int outdevice = 1;
            //int outdevice = 2;
            //int outdevice = 3;
            if (Bass.BASS_Init(outdevice, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
            {
                BASS_INFO info = new BASS_INFO();
                Bass.BASS_GetInfo(info);
                Msg(info.ToString());
            }
            else
            {
                if (bTryFreeBass == false)
                {
                    if (Bass.BASS_Free() == true)
                    {
                        bTryFreeBass = true;
                        InitSound();
                        return;
                    }
                }
                else
                {
                    Console.Write(Bass.BASS_ErrorGetCode().ToString());
                    MessageBox.Show(this, "Bass_Init error!: " + Bass.BASS_ErrorGetCode().ToString());
                }
            }

            if (!Bass.BASS_RecordInit(lstRecDevices.SelectedIndex))
            {
                //MessageBox.Show(this, "Bass_RecordInit error!");
                string err = Bass.BASS_ErrorGetCode().ToString();
                Console.Write(Bass.BASS_ErrorGetCode().ToString());
                Msg(err);

            }

        }

        private void stopRecordMp3()
        {
            Bass.BASS_ChannelStop(_PcmRecHandle);
            lameEncodedBytes = StopLameEncoding();
            // lameEncodedBytes can now be save/retrieved from a database blog field
            // or played with method PlayByteBlog below
            // retrieved data from your blog field can be played with PlayByteBlog listed below
            Msg("Stopped Record");
            recording = false;
        }

        private void startRecordMp3()
        {
            InitSound();    
            

            try
            {
                if (Bass.BASS_ChannelIsActive(_blogStreamHandle) == BASSActive.BASS_ACTIVE_PLAYING)
                {
                    Msg("Still Playing MP3");
                    return;
                }
                if (Bass.BASS_ChannelIsActive(pcmImportStreamHandle) == BASSActive.BASS_ACTIVE_PLAYING)
                {
                    Msg("Still Playing PCM");
                    return;
                }

                if (pcmMemStrm != null) pcmMemStrm.Close();
                pcmMemStrm = new MemoryStream();
                // start recording paused
                _PcmRecProc = new RECORDPROC(PcmRecordingCallBack);


                _PcmRecHandle = Bass.BASS_RecordStart(44100, 2, BASSFlag.BASS_RECORD_PAUSE, _PcmRecProc, IntPtr.Zero);




                BASSError error = Bass.BASS_ErrorGetCode();
                Msg(Enum.GetName(typeof(BASSError), error));

                bool lameStarted = StartLameEncoding(_PcmRecHandle);

                if (!lameStarted)
                {
                    MessageBox.Show("Lame.exe may not be present");
                }

                Bass.BASS_ChannelPlay(_PcmRecHandle, false);

                Msg("Recording");
                recording = true;

            }
            catch (Exception ex)
            {
                // Also try to modify options to "prefer 32bits"
                // System.BadImageFormatException' s'est produite dans Bass.Net.dll
                Console.Write(ex.Message + "\r");
                Msg(ex.Message);
                Msg("bassenc.dll may not be present");
                Msg("Failed to Record");
            }
        }

        #region functions

        // the raw pcm recording callback
        private unsafe bool PcmRecordingCallBack(int handle, IntPtr buffer, int length, IntPtr user)
        {
            // user will contain our encoding handle
            if (length > 0 && buffer != IntPtr.Zero)
            {

                // increase the rec buffer as needed
                if (_recbuffer == null || _recbuffer.Length < length)
                    _recbuffer = new byte[length];

                Marshal.Copy(buffer, _recbuffer, 0, length);
                pcmMemStrm.Write(_recbuffer, 0, length);  //not need if you do not intend to playback the raw data

                // get the rec level...
                int maxL = 0;
                int maxR = 0;
                short* data = (short*)buffer;
                for (int a = 0; a < length / 2; a++)
                {
                    // decide on L/R channel
                    if (a % 2 == 0)
                    {
                        // L channel
                        if (data[a] > maxL)
                            maxL = data[a];
                    }
                    else
                    {
                        // R channel
                        if (data[a] > maxR)
                            maxR = data[a];
                    }
                }
                // limit the maximum peak levels to 0bB = 32768
                // the peak levels will be int values, where 32768 = 0dB!
                if (maxL > 32768)
                    maxL = 32768;
                if (maxR > 32768)
                    maxR = 32768;

                try
                {
                    this.BeginInvoke(new UpdateDelegate(UpdateDisplay), new object[] { maxL, maxR });
                    // you might instead also use "this.Invoke(...)", which would call the delegate synchron!
                }
                catch (Exception bg)
                {
                    Console.Write("Error BeginInvoke in Recorder: " + bg.Message + "\r");
                }
            }
            return true;  //keep recording
        }

        /// <summary>
        /// Play the MP3 song stored in the buffer lameEncodedBytes
        /// </summary>
        /// <param name="buffer"></param>
        private void PlayByteBlog(byte[] buffer)
        {
            if (_blogStreamHandle == 0 || Bass.BASS_ChannelIsActive(_blogStreamHandle) != BASSActive.BASS_ACTIVE_PLAYING)
            {
                // create a handle to the buffer and pin it, so that the Garbage Collector will not remove it
                GCHandle hGC = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                // create the stream (AddrOfPinnedObject delivers the necessary IntPtr)
                _blogStreamHandle = Bass.BASS_StreamCreateFile(hGC.AddrOfPinnedObject(), 0, buffer.Length, BASSFlag.BASS_DEFAULT);

                //...let it play...

                if (buffer != null && buffer.Length > 0)
                {
                    if (_blogStreamHandle != 0 && Bass.BASS_ChannelPlay(_blogStreamHandle, false))
                    {
                        Msg("Lame MP3 playback started");
                        Msg("MP3 has " + lameEncodedBytes.Length + " bytes");
                    }
                    else
                    {

                        BASSError error = Bass.BASS_ErrorGetCode();
                        Msg(Enum.GetName(typeof(BASSError), error));


                    }
                }
                hGC.Free();
            }
            else Msg("Mp3 data is still playing");
        }

        /// <summary>
        /// Initialize MP3 encoding whith LAME
        /// </summary>
        /// <param name="_rechandle"></param>
        /// <returns></returns>
        private bool StartLameEncoding(int _rechandle)
        {
            /* Voir si en utilisant "lameEncoder.OutputFile =  outFile"
             * on pourrait créer un fichier au lieu d'un buffer
             * */
            _lameMemBuffer.Clear();
            _LameEncProc = new ENCODEPROC(LameEncCallback); // Callback function

            lameEncoder = new EncoderLAME(_rechandle);


            if (lameEncoder.EncoderExists == false)
            {
                Console.Write("Encoder does not exists ?\r");
            }


            lameEncoder.InputFile = null;	//STDIN

            if (bSaveToDisk)
                lameEncoder.OutputFile = strSaveFile;
            else
                lameEncoder.OutputFile = null;	//STDOUT - adding file name will prevent the callback from working instead will saving to disk


            lameEncoder.LAME_Bitrate = (int)EncoderLAME.BITRATE.kbps_64;
            lameEncoder.LAME_Mode = EncoderLAME.LAMEMode.Default;
            lameEncoder.LAME_Quality = EncoderLAME.LAMEQuality.Quality;

            bool bret = lameEncoder.Start(_LameEncProc, IntPtr.Zero, false);

            BASSError txt = Bass.BASS_ErrorGetCode();
            Msg(txt.ToString());
            
            return bret;

        }

        /// <summary>
        /// Callback function used by StartLameEncoding
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="channel"></param>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        /// <param name="user"></param>
        private unsafe void LameEncCallback(int handle, int channel, IntPtr buffer, int length, IntPtr user)
        {
            // here you receive the encoded data back and can put them into your memory buffer
            // e.g. like this:
            byte* data = (byte*)buffer;
            for (int a = 0; a < length; a++)
            {
                _lameMemBuffer.Add(data[a]);
            }

        }

        /// <summary>
        /// Stop Lame encoding
        /// </summary>
        /// <returns></returns>
        private byte[] StopLameEncoding()
        {
            lameEncoder.Stop();

            // now you might copy your dynamic arraylist to a flat byte[]
            return (byte[])_lameMemBuffer.ToArray(typeof(byte));
        }

        //unsafe used in order to display channel levels
        private unsafe int ImportPcmCallBack(int handle, IntPtr buffer, int length, IntPtr user)
        {
            byte[] bufData = new byte[length];
            // read the file into data
            int bytesread = pcmMemStrm.Read(bufData, 0, length);
            Marshal.Copy(bufData, 0, buffer, bytesread);
            // end of the file/stream? check this
            if (bytesread < length)
            {
                bytesread |= (int)BASSStreamProc.BASS_STREAMPROC_END; // set indicator flag
                //pcmMemStrm.Close(); //uncomment to close after 1 play
            }

            // get the rec level...
            int maxL = 0;
            int maxR = 0;
            short* data = (short*)buffer;
            for (int a = 0; a < length / 2; a++)
            {
                // decide on L/R channel
                if (a % 2 == 0)
                {
                    // L channel
                    if (data[a] > maxL)
                        maxL = data[a];
                }
                else
                {
                    // R channel
                    if (data[a] > maxR)
                        maxR = data[a];
                }
            }
            // limit the maximum peak levels to 0bB = 32768
            // the peak levels will be int values, where 32768 = 0dB!
            if (maxL > 32768)
                maxL = 32768;
            if (maxR > 32768)
                maxR = 32768;

            this.BeginInvoke(new UpdateDelegate(UpdateDisplay), new object[] { maxL, maxR });

            return bytesread;
        }

        private void UpdateDisplay(int maxL, int maxR)
        {
            this.progressBarRecL.Value = maxL;
            this.progressBarRecR.Value = maxR;
        }

        #endregion functions

        #endregion audio

        /// <summary>
        /// Retrieve Handle of frmLyrics
        /// </summary>
        /// <returns></returns>
        private IntPtr getHwndfrmLyric()
        {
            IntPtr pt = IntPtr.Zero;

            if (Application.OpenForms.OfType<frmLyric>().Count() > 0)
            {
                frmLyrics = Utilities.FormUtilities.GetForm<frmLyric>();
                pt = frmLyrics.Handle;
            }
            return pt;
        }

        /// <summary>
        /// Retrieve Handle of frmLyrics
        /// </summary>
        /// <returns></returns>
        private IntPtr getHwndfrmPianoTraining()
        {
            IntPtr pt = IntPtr.Zero;

            if (Application.OpenForms.OfType<frmPianoTraining>().Count() > 0)
            {
                FrmPianoTraining = Utilities.FormUtilities.GetForm<frmPianoTraining>();
                pt = FrmPianoTraining.Handle;
            }
            return pt;
        }



        #region buttons

        /// <summary>
        /// Start recording
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bt_Start_Click(object sender, EventArgs e)
        {
            try
            {
                

                // If save folder empty, ask for a folder to save file
                if (txtSaveFolder.Text.Length < 1)
                {
                    FolderBrowserDialog fbd = new FolderBrowserDialog();
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        StartRec(fbd.SelectedPath);
                    }
                }
                else
                {
                    StartRec(txtSaveFolder.Text);
                }
            }
            catch (Exception gdfdsgg)
            {
                MessageBox.Show(gdfdsgg.Message);
                resetValues();
            }
        }

        /// <summary>
        /// Stop recording
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bt_Save_Click(object sender, EventArgs e)
        {
            try
            {
                rec = false;
                bt_Start.Enabled = true;
                bt_Save.Enabled = false;

                MessageBox.Show("File saved");
            }
            catch (Exception gfsgsdfgsdf)
            {
                MessageBox.Show(gfsgsdfgsdf.Message);
            }
        }

 


        #endregion buttons


        #region private video methods

        /// <summary>
        /// Reset values
        /// </summary>
        private void resetValues()
        {
            rec = false;
            try
            {
                bt_Save.Enabled = false;
                bt_Start.Enabled = true;
            }
            catch (Exception er)
            {
                Console.Write(er.Message);
            }
        }

        private void StartRec(string path)
        {
            if (rec == false)
            {


                bt_Start.Enabled = false;
                bt_Save.Enabled = true;
                rec = true;


                //InitSound();


                frameCount = 0;

                txtSaveFolder.Text = path;
                string time = DateTime.Now.ToString("d_MMM_yyyy_HH_mm_ssff");
                string compName = Environment.UserName;
                string fullName = path + "\\" + compName.ToUpper() + "_" + time;

                aviFileName = fullName + ".avi";
                mp3FileName = strSaveFile;

                bCaptureVideo = true;
                HwndLyrics = getHwndfrmLyric();
                if (HwndLyrics == IntPtr.Zero)
                {
                    HwndPianoTraining = getHwndfrmPianoTraining();
                    if (HwndPianoTraining == IntPtr.Zero)
                    {
                        screenArea = new Rectangle(0, 0, 100, 100);
                        width = 100;
                        height = 100;
                        bCaptureVideo = false;
                    }
                    else
                    {
                        screenArea = getAppWRect(HwndPianoTraining);
                    }
                }
                else
                {
                    screenArea = getAppWRect(HwndLyrics);
                }
                                            
                if (screenArea.IsEmpty)
                {
                    MessageBox.Show("Invalid Windows (minimized ?)");
                    resetValues();
                    return;                    
                }

                try
                {
                    // Audio: start recording
                    startRecordMp3();

                    
                    // Viseo: Save File option
                    writer.Open(
                        fullName + ".avi",
                        width,
                        height,
                        (int)nud_FPS.Value,
                        (VideoCodec)cb_VideoCodec.SelectedValue, (int)(bitRate)cb_BitRate.SelectedValue);
                    
                }
                catch (Exception hgfdhdfs)
                {
                    MessageBox.Show(hgfdhdfs.Message);
                    resetValues();
                    return;
                }
                
                
                // Start main work for video
                DoJob();
            }
        }

        private void DoJob()
        {
            try
            {
                // create screen capture video source
                streamVideo = new ScreenCaptureStream(screenArea);

                // set NewFrame event handler
                streamVideo.NewFrame += new NewFrameEventHandler(video_NewFrame);


                // start the video source
                streamVideo.Start();
                

                // stopWatch
                stopWatch.Start();
            }
            catch (Exception gfdgdfhdf)
            {
                MessageBox.Show(gfdgdfhdf.Message);
            }
        }

        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                if (rec)
                {                    
                    frameCount++;
                    writer.WriteVideoFrame(eventArgs.Frame);
                    lb_1.Invoke(new Action(() =>
                    {
                        lb_1.Text = "Frames: " + frameCount.ToString();
                    }));
                    lb_stopWatch.Invoke(new Action(() =>
                    {
                        lb_stopWatch.Text = stopWatch.Elapsed.ToString();
                    }));

                    
                }
                else
                {
                    // End of recording
                    #region video
                    stopWatch.Reset();
                    Thread.Sleep(500);
                    streamVideo.SignalToStop();
                    Thread.Sleep(500);                    
                    writer.Close();                    
                    #endregion video

                    #region audio
                    stopRecordMp3();
                    #endregion audio

                    if (bCaptureVideo)
                        AddMP3toAVI(aviFileName, mp3FileName);
                    else {
                        try {
                            File.Delete(aviFileName);
                        }
                        catch (Exception er )
                        {
                            Console.Write(er.Message);
                        }
                    }
                }
            }
            catch (Exception glj)
            {
                MessageBox.Show(glj.Message);
                resetValues();
            }
        }

        #endregion private methods



        #region files

        private void populatePaths()
        {
            // Directory
            txtSaveFolder.Text = txtSaveFolder.Text.Trim();
            string savefolderpath = txtSaveFolder.Text;
            if (savefolderpath == string.Empty || !Directory.Exists(savefolderpath))
            {
                savefolderpath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            }
            txtSaveFolder.Text = savefolderpath;

            // mp3 file
            txtSaveToDisk.Text = txtSaveToDisk.Text.Trim();
            string mp3file = txtSaveToDisk.Text;
            strSaveFile = selectNewFile(savefolderpath, mp3file);

            txtSaveToDisk.Text = strSaveFile;

        }

        private string selectNewFile(string savefolderpath, string file)
        {

            if (!Directory.Exists(savefolderpath))
            {
                savefolderpath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            }


            if (file == "")
            {
                file = proposeNewName(savefolderpath, "new", ".mp3");
                return file;
            }



            file = Path.Combine(savefolderpath, file);

            // Case 1: file exists
            if (File.Exists(file) == true)
            {
                string txtQuestion = "File " + file + "already exists, do you want to replace it?";
                DialogResult result1 = MessageBox.Show(txtQuestion, "Important Question", MessageBoxButtons.YesNo);
                if (result1 == System.Windows.Forms.DialogResult.No)
                {

                    FileInfo fileInfo = new FileInfo(file);
                    string mypath = fileInfo.DirectoryName;
                    string newfilename = fileInfo.Name;
                    string extension = fileInfo.Extension;

                    file = proposeNewName(mypath, newfilename, extension);
                    return file;
                }
                else
                {
                    return file;
                }
            }

            
            // case 2: File does not exists : ok if directory exists
            string path = Path.GetDirectoryName(file);
            if (Directory.Exists(path))
            {
                return file;
            }

            else
            {                    
                file = proposeNewName(savefolderpath, "new", ".mp3");
                return file;
            }
            


        }

        private string proposeNewName(string path, string FileName, string extension)
        {
            string newFileName = string.Empty;
            string newPathFile = path + "\\" + FileName + extension;

            bool bFound = true;
            int i = 1;
            do
            {
                bFound = File.Exists(newPathFile);
                if (bFound == true)
                {
                    newFileName = FileName + "(" + i + ")";
                    newPathFile = path + "\\" + newFileName + extension;
                    i++;
                }
            } while (bFound == true);

            return newPathFile;
        }

        #endregion files



        #region applications windows

        private Rectangle getAppWRect(IntPtr hWnd)
        {
            var rect = new User32.Rect();

            if (!User32.GetClientRect(new HandleRef(this, hWnd), out rect))
            {
                int ErrorCode = Marshal.GetLastWin32Error();
                MessageBox.Show("GetWindowRect returned false, error = " + ErrorCode.ToString());
            }



            if (rect.right < 0 && rect.left < 0 && rect.bottom < 0 && rect.top < 0)
            {
                Rectangle R = Rectangle.Empty;
                return R;
            }
            else
            {

                width = rect.right - rect.left;
                height = rect.bottom - rect.top;

                int w = width % 2;
                int h = height % 2;

                width = width - w;
                height = height - h;

                int X = rect.left;
                int Y = rect.top;

                bool coordinatesFound = false;
                Point point = new Point();
                coordinatesFound = User32.ClientToScreen(hWnd, ref point);
                if (coordinatesFound)
                {
                    X = X + point.X;
                    Y = Y + point.Y;
                }

                Rectangle R = new Rectangle(X, Y, width, height);
                return R;
            }
        }

  

        #endregion

        #region User32 Class

        private class User32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct Rect
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }


            // Methode 2
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetWindowRect(HandleRef hwnd, out Rect lpRect);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetClientRect(HandleRef hwnd, out Rect lpRect);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr FindWindow(String lpClassName, String lpWindowName);

            [DllImport("user32.dll")]
            public static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);  

        }

        #endregion User32 class
        

  

        #region form load
        private void frmVideoRecorder_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (recording)
            {
                Bass.BASS_ChannelStop(_PcmRecHandle);
                lameEncodedBytes = StopLameEncoding();
            }

            try
            {
                Bass.BASS_Free();
            }
            catch (Exception e2)
            {
                Console.Write(e2.Message);
            }
            if (stream != 0)
                Bass.BASS_StreamFree(stream);


            Dispose();
        }



        #endregion form load


        #region play stop mp3
        private void btnStopMp3_Click(object sender, EventArgs e)
        {
            if (stream != 0)
            {
                btnPlayMp3.Enabled = true;
                btnStopMp3.Enabled = false;

                // free the stream 
                Bass.BASS_StreamFree(stream);
                // free BASS 
                Bass.BASS_Free();

                //stream = 0;
                Msg("Stopped Mp3 playing");
            }
        }

        private void btnPlayMp3_Click(object sender, EventArgs e)
        {

            // Register to avoid splash screen
            BassNet.Registration("fabrice.lacharme@free.fr", "2X1632324163737");
            Bass.BASS_Free();

            bool bBass = Bass.BASS_Init(1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);

            if (bBass)
            {
                strSaveFile = txtSaveToDisk.Text.Trim();

                if (File.Exists(strSaveFile))
                {
                    string file = strSaveFile;                    

                    // create a stream channel from a file 
                    stream = Bass.BASS_StreamCreateFile(file, 0, 0, BASSFlag.BASS_DEFAULT);
                    if (stream != 0)
                    {
                        btnPlayMp3.Enabled = false;
                        btnStopMp3.Enabled = true;

                        // play the stream channel 
                        Bass.BASS_ChannelPlay(stream, false);
                        Msg("Playing Mp3: " + file);
                    }
                    else
                    {
                        // error creating the stream 
                        Console.WriteLine("Stream error: {0}", Bass.BASS_ErrorGetCode());
                    }
                }
                else
                {
                    Msg("File not found");
                }
            }
            else
            {
                Msg("Bass not initialized");
            }
        }

        #endregion play stop mp3



        public static void AddMP3toAVI(string aviFileName, string mp3FileName)
        {
            string newAVIFileName = Regex.Replace(aviFileName, "\\.avi$", "MUX.avi", RegexOptions.IgnoreCase);
            string cmdLineArgs = "-ovc copy -oac copy -audiofile \"" + mp3FileName + "\" -o \"" + newAVIFileName + "\" \"" + aviFileName + "\"";
            using (Process myProcess = new Process())
            {
                string myCMD = "\"" + System.AppDomain.CurrentDomain.BaseDirectory + "mencoder.exe \"" + cmdLineArgs;
                myProcess.StartInfo.FileName = "\"" + System.AppDomain.CurrentDomain.BaseDirectory + "mencoder.exe\"";
                myProcess.StartInfo.Arguments = cmdLineArgs;
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.CreateNoWindow = true;
                myProcess.Start();
                myProcess.PriorityClass = ProcessPriorityClass.Normal;
                myProcess.WaitForExit();
            }
            if (File.Exists(newAVIFileName))
            {
                File.Delete(aviFileName);
                File.Move(newAVIFileName, aviFileName);
            }

            MessageBox.Show("Rip terminated");
        }


    }
}

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
using System.IO;
using System.Collections;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Un4seen.Bass;
//using Un4seen.BassWasapi;
using Un4seen.Bass.AddOn.Enc;
using Un4seen.Bass.Misc;

namespace Karaboss
{
    public partial class frmRecorder : Form
    {

        /* This example was created by Kenton Brown using the examples provided by 
         *  Bernd Niedergesaess http://www.ten53.de/
         * Once the pcm data is encoded into data[] lameEncodedBytes it can be save/retrieved from a database blog field
         * 
         * FAB : this module uses unsafe code => modify compilation parameters with /unsafe
         * Options => generer => autoriser du code unsafe
         * Cocher "Préferer 32 bits"
         * 
         * Put these files into /bin/debug (and release ?)
         * "bass.net.dll" and add reference to it
         * "bassenc.dll"
         * "lame.exe"
         * 
         * NE PAS METTRE BASS.DLL
         * */


        public delegate void UpdateDelegate(int maxL, int maxR);

        #region play mp3

        private int stream;
        
        #endregion play mp3


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

        STREAMPROC playPcmProc;
        int pcmImportStreamHandle = 0;

        private bool bSaveToDisk = false;
        private string strSaveFile = string.Empty;

        private bool bTryFreeBass = false;

        #endregion encode


        public frmRecorder()
        {
            InitializeComponent();

            // Register to avoid splash screen : this is mine :-) please use yours
            BassNet.Registration("fabrice.lacharme@free.fr", "2X1632324163737");
            populateDevices();
            populateRecordDevices();
        }

        #region init functions

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

            
            //if (Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
            //if (Bass.BASS_Init(lstDevices.SelectedIndex, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
            if (Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
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

            /*
            BASS_DEVICEINFO[] devs = Bass.BASS_RecordGetDeviceInfos();
            foreach (BASS_DEVICEINFO dev in devs)
            {
                Msg(dev.name);

            }
            */

            // init your recording device (we use the default device)
            Bass.BASS_RecordFree();

            /*
            if (!Bass.BASS_RecordInit(-1))
                MessageBox.Show(this, "Bass_RecordInit error!");
            */
            if (!Bass.BASS_RecordInit(lstRecDevices.SelectedIndex))
                MessageBox.Show(this, "Bass_RecordInit error!");

        }


        private void populateDevices()
        {
            BASS_DEVICEINFO[] devs = Bass.BASS_GetDeviceInfos();
            foreach (BASS_DEVICEINFO dev in devs)
            {
                if (dev.IsEnabled)
                {
                    Msg(dev.name);
                    lstDevices.Items.Add(dev.name);
                }
            }
        }

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
        }

        private void Msg(string msg)
        {
            listBox1.Items.Add(msg);
            listBox1.SelectedIndex = listBox1.Items.Count - 1;

        }

        #endregion init functions

        #region Buttons

        private void btnRecord_Click(object sender, EventArgs e)
        {
            InitSound();
            

            strSaveFile = selectNewFile(); 

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
                    btnPlayLame.Enabled = false;
                    btnPlayPcm.Enabled = false;
                }

                Bass.BASS_ChannelPlay(_PcmRecHandle, false);

                
                btnRecord.Enabled = true;
                btnStopRecord.Enabled = true;
                btnPlayPcm.Enabled = false;
                btnStopPcm.Enabled = false;
                btnPlayLame.Enabled = false;
                btnStopLame.Enabled = false;
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


                btnRecord.Enabled = true;
                btnStopRecord.Enabled = false;
                btnPlayPcm.Enabled = false;
                btnStopPcm.Enabled = false;
                btnPlayLame.Enabled = false;
                btnStopLame.Enabled = false;
                Msg("Failed to Record");
            }
        }

        private void btnStopRecord_Click(object sender, EventArgs e)
        {
            Bass.BASS_ChannelStop(_PcmRecHandle);
            lameEncodedBytes = StopLameEncoding();
            // lameEncodedBytes can now be save/retrieved from a database blog field
            // or played with method PlayByteBlog below
            // retrieved data from your blog field can be played with PlayByteBlog listed below
            btnRecord.Enabled = true;
            btnStopRecord.Enabled = false;
            btnPlayPcm.Enabled = true;
            btnStopPcm.Enabled = false;
            btnPlayLame.Enabled = true;
            btnStopLame.Enabled = false;
            Msg("Stopped Record");
            recording = false;
        }

        private void btnPlayLame_Click(object sender, EventArgs e)
        {
            // lameEncodedBytes is ready to be stored in a database blog field
            btnRecord.Enabled = true;
            btnStopRecord.Enabled = false;
            btnPlayPcm.Enabled = true;
            //btnStopPCM.Enabled = true;
            btnPlayLame.Enabled = true;
            btnStopLame.Enabled = true;

            timer1.Enabled = true;              // Launch timer
            PlayByteBlog(lameEncodedBytes);
        }

        private void btnStopLame_Click(object sender, EventArgs e)
        {
            Bass.BASS_ChannelStop(_blogStreamHandle);
            Msg("Lame MP3 playback stopped");
        }

        /// <summary>
        /// Start PCM play
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPlayPcm_Click(object sender, EventArgs e)
        {
            if (pcmImportStreamHandle == 0 || Bass.BASS_ChannelIsActive(pcmImportStreamHandle) != BASSActive.BASS_ACTIVE_PLAYING)
            {
                Msg("Raw PCM playback started");
                Msg("Raw PCM has " + pcmMemStrm.Length + " bytes");
                pcmMemStrm.Seek(0, SeekOrigin.Begin);
                playPcmProc = new STREAMPROC(ImportPcmCallBack);
                pcmImportStreamHandle = Bass.BASS_StreamCreate(44100, 2, 0, playPcmProc, IntPtr.Zero); // user = reader#
                Bass.BASS_ChannelPlay(pcmImportStreamHandle, false);
                btnRecord.Enabled = true;
                btnStopRecord.Enabled = false;
                btnPlayPcm.Enabled = true;
                btnStopPcm.Enabled = true;
                btnPlayLame.Enabled = true;
                timer1.Enabled = true;
                //btnStopLame.Enabled = true;
            }
            else Msg("Raw PCM is still playing!");
        }

        /// <summary>
        /// Stop PCM play
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStopPcm_Click(object sender, EventArgs e)
        {
            Bass.BASS_ChannelStop(pcmImportStreamHandle);
            Msg("Raw PCM playback stopped");
        }

        
        private void btnPlayMp3_Click(object sender, EventArgs e)
        {
            //InitSound();
            // Register to avoid splash screen
            BassNet.Registration("fabrice.lacharme@free.fr", "2X1632324163737");
            Bass.BASS_Free();
            //bool bBass = Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
            bool bBass = Bass.BASS_Init(1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);

            if (bBass)
            {
                strSaveFile = txtSaveToDisk.Text.Trim();

                if (File.Exists(strSaveFile))
                {
                    string file = strSaveFile;
                    //string file = "air.mp3";

                    // create a stream channel from a file 
                    stream = Bass.BASS_StreamCreateFile(file, 0, 0, BASSFlag.BASS_DEFAULT);
                    if (stream != 0)
                    {
                        btnPlayMp3.Enabled = false;
                        btnStopMp3.Enabled = true;

                        // play the stream channel 
                        Bass.BASS_ChannelPlay(stream, false);
                        //Bass.BASS_ChannelPlay(stream, true);

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
                    //btnPlayMp3.Enabled = false;
                }
            }
            else
            {
                Msg("Bass not initialized");
            }

        }
        
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

        #endregion Buttons


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
                catch(Exception bg)
                {
                    Console.Write("Error BeginInvoke in Recorder: " + bg.Message + "\r");
                }
            }
            return true;  //keep recording
        }

        /// <summary>
        /// Pay the MP3 song stored in the buffer lameEncodedBytes
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

        #region form load close

        private void frmRecorder_Load(object sender, EventArgs e)
        {
            
            bTryFreeBass = false;
            progressBarRecR.Maximum = 32768;
            progressBarRecL.Maximum = 32768;

            btnPlayLame.Enabled = false;
            btnPlayPcm.Enabled = false;
            btnStopLame.Enabled = false;
            btnStopPcm.Enabled = false;
            btnStopRecord.Enabled = false;
            btnStopMp3.Enabled = false;

            bSaveToDisk = true;
            //string newPathFile = proposeNewName(Application.StartupPath, "new", ".mp3");
            string newPathFile =  proposeNewName( Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName),"new", ".mp3");
            
            txtSaveToDisk.Text = newPathFile;
            strSaveFile = newPathFile;

            listBox1.Items.Clear();
            //InitSound();
        }

        private void frmRecorder_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (recording)
            {
                Bass.BASS_ChannelStop(_PcmRecHandle);
                lameEncodedBytes = StopLameEncoding();
            }

            try {
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



        #endregion form load close

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_blogStreamHandle == 0 || Bass.BASS_ChannelIsActive(_blogStreamHandle) != BASSActive.BASS_ACTIVE_PLAYING)
            {
                btnStopLame.Enabled = false;
            }
            if (pcmImportStreamHandle == 0 || Bass.BASS_ChannelIsActive(pcmImportStreamHandle) != BASSActive.BASS_ACTIVE_PLAYING)
            {
                btnStopPcm.Enabled = false;
            }
        }

        #region file

        /// <summary>
        /// Check if converted MIDI file must be saved to MP3 file on disk
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkSaveToDisk_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSaveToDisk.Checked)
            {
                bSaveToDisk = true;
                strSaveFile = selectNewFile();                
            }
            else
            {
                bSaveToDisk = false;
                strSaveFile = "";
            }
        }


        private string selectNewFile()
        {
            txtSaveToDisk.Text = txtSaveToDisk.Text.Trim();
            string file = txtSaveToDisk.Text;
            
            if (file == "")
            {
                //file = proposeNewName(Application.StartupPath, "new", ".mp3");
                file = proposeNewName( Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName), "new", ".mp3");

                txtSaveToDisk.Text = file;
                return file;
            }
            
            if (File.Exists(file) == true)
            {
                string txtQuestion = "File " + file + "already exists, do you want to replace it?";
                DialogResult result1 = MessageBox.Show(txtQuestion, "Important Question", MessageBoxButtons.YesNo);
                if (result1 == System.Windows.Forms.DialogResult.No)
                {
                    
                    FileInfo fileInfo = new FileInfo(file);
                    string path = fileInfo.DirectoryName;
                    string newfilename = fileInfo.Name;
                    string extension = fileInfo.Extension;
                    
                    file = proposeNewName(path, newfilename, extension);
                    txtSaveToDisk.Text = file;
                    return file;
                }
                else
                {
                    return file;
                }

            }
            else
            {
                // File does not exists : ok if directory exists
                string path = Path.GetDirectoryName(file);
                if (Directory.Exists(path))
                {
                    return file;
                }
                else
                {
                    //file = proposeNewName(Application.StartupPath, "new", ".mp3");
                    file = proposeNewName( Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName), "new", ".mp3");

                    txtSaveToDisk.Text = file;
                    return file;
                }
            }


        }

        private string proposeNewName(string path, string FileName, string extension)
        {
            //string newFileName = "new";
            //string extension = ".mp3";
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

        #endregion file


    








  
    }
}

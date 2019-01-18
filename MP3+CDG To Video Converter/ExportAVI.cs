#region License

/* Copyright (c) 2016 Fabrice Lacharme
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
using System.IO;
using System.Text.RegularExpressions;
using AviFile;
using CDGNet;
using System.Diagnostics;
using System.Windows.Forms;

namespace MP3GConverter
{

    public delegate void StatusChangedEventHandler(string message);

    public class ExportAVI
    {
        // Event status of progression
        public event StatusChangedEventHandler Status;        

        // Constructor
        public ExportAVI()
        {
            // Invoke the delegate
            this.Status += new StatusChangedEventHandler(Status_Changed);
        }

        private void Status_Changed(string message)
        {
            //throw new NotImplementedException();
           
        }

        public void CDGtoAVI(string aviFileName, string cdgFileName, string mp3FileName, double frameRate, string backgroundFileName = "")
        {
            Bitmap backgroundBmp = null;
            Bitmap mergedBMP = null;
            VideoStream aviStream = default(VideoStream);
            CDGFile myCDGFile = new CDGFile(cdgFileName);
            myCDGFile.renderAtPosition(0);
            Bitmap bitmap__1 = (Bitmap)myCDGFile.RGBImage;
            if (!string.IsNullOrEmpty(backgroundFileName))
            {
                try
                {
                    if (IsMovie(backgroundFileName))
                        backgroundBmp = MovieFrameExtractor.GetBitmap(0, backgroundFileName, CDGFile.CDG_FULL_WIDTH, CDGFile.CDG_FULL_HEIGHT);
                    if (IsGraphic(backgroundFileName))
                        backgroundBmp = CDGNet.GraphicUtil.GetCDGSizeBitmap(backgroundFileName);
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
            }
            AviManager aviManager = new AviManager(aviFileName, false);

            if (backgroundBmp != null)
            {
                mergedBMP = GraphicUtil.MergeImagesWithTransparency(backgroundBmp, bitmap__1);
                aviStream = aviManager.AddVideoStream(true, frameRate, mergedBMP);
                mergedBMP.Dispose();
                if (IsMovie(backgroundFileName))
                    backgroundBmp.Dispose();
            }
            else {
                aviStream = aviManager.AddVideoStream(true, frameRate, bitmap__1);
            }

            int count = 0;
            double frameInterval = 1000 / frameRate;
            long totalDuration = myCDGFile.getTotalDuration();
            double position = 0;
            while (position <= totalDuration)
            {
                count += 1;
                position = count * frameInterval;
                myCDGFile.renderAtPosition(Convert.ToInt64(position));
                bitmap__1 = (Bitmap)myCDGFile.RGBImage;
                if (!string.IsNullOrEmpty(backgroundFileName))
                {
                    if (IsMovie(backgroundFileName))
                        backgroundBmp = MovieFrameExtractor.GetBitmap(position / 1000, backgroundFileName, CDGFile.CDG_FULL_WIDTH, CDGFile.CDG_FULL_HEIGHT);
                }
                if (backgroundBmp != null)
                {
                    mergedBMP = GraphicUtil.MergeImagesWithTransparency(backgroundBmp, bitmap__1);
                    aviStream.AddFrame(mergedBMP);
                    mergedBMP.Dispose();
                    if (IsMovie(backgroundFileName))
                        backgroundBmp.Dispose();
                }
                else {
                    aviStream.AddFrame(bitmap__1);
                }
                bitmap__1.Dispose();
                double percentageDone = (position / totalDuration) * 100;

                if (Status != null)
                {
                    Status(percentageDone.ToString());
                }
                Application.DoEvents();
            }
            myCDGFile.Dispose();
            aviManager.Close();
            if (backgroundBmp != null)
                backgroundBmp.Dispose();

            // Add MP3 to AVI
            Console.Write("|nAVI terminated, add MP3 to AVI");
            AddMP3toAVI(aviFileName, mp3FileName);
        }


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

        public static bool IsMovie(string filename)
        {
            return Regex.IsMatch(filename, "^.+(\\.avi|\\.mpg|\\.wmv)$", RegexOptions.IgnoreCase);
        }

        public static bool IsGraphic(string filename)
        {
            return Regex.IsMatch(filename, "^.+(\\.jpg|\\.bmp|\\.png|\\.tif|\\.tiff|\\.gif|\\.wmf)$", RegexOptions.IgnoreCase);
        }


    }



}

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
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System;

namespace CDGNet
{
    public class GraphicUtil
    {
        //copy a stream of pixels
        public static Stream BitmapToStream(string filename)
        {
            Bitmap oldBmp = (Bitmap)Image.FromFile(filename);
            int width = oldBmp.Width;
            int height = oldBmp.Width;
            BitmapData oldData = oldBmp.LockBits(new Rectangle(0, 0, oldBmp.Width, oldBmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            int length = oldData.Stride * oldBmp.Height;
            byte[] stream = new byte[length];
            Marshal.Copy(oldData.Scan0, stream, 0, length);
            oldBmp.UnlockBits(oldData);
            oldBmp.Dispose();
            return new MemoryStream(stream);
        }


        public static Bitmap StreamToBitmap(ref Stream stream, int width, int height)
        {
            //create a new bitmap
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bmp.PixelFormat);
            stream.Seek(0, SeekOrigin.Begin);
            //copy the stream of pixel
            for (int n = 0; n <= stream.Length - 1; n++)
            {
                byte[] myByte = new byte[1];
                stream.Read(myByte, 0, 1);
                Marshal.WriteByte(bmpData.Scan0, n, myByte[0]);
            }
            bmp.UnlockBits(bmpData);
            return bmp;
        }

        public static Bitmap GetCDGSizeBitmap(string filename)
        {
            Bitmap bm = new Bitmap(filename);
            return ResizeBitmap(ref bm, CDGFile.CDG_FULL_WIDTH, CDGFile.CDG_FULL_HEIGHT);
        }

        public static Bitmap ResizeBitmap(ref Bitmap bm, int width, int height)
        {
            Bitmap thumb = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(thumb);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(bm, new Rectangle(0, 0, width, height), new Rectangle(0, 0, bm.Width, bm.Height), GraphicsUnit.Pixel);
            g.Dispose();
            bm.Dispose();
            return thumb;
        }

        public static Bitmap MergeImagesWithTransparency(Bitmap Pic1, Bitmap pic2)
        {
            Image MergedImage = default(Image);
            Bitmap bm = new Bitmap(Pic1.Width, Pic1.Height);
            Graphics gr = Graphics.FromImage(bm);
            gr.DrawImage(Pic1, 0, 0);
            pic2.MakeTransparent(pic2.GetPixel(1, 1));
            gr.DrawImage(pic2, 0, 0);
            MergedImage = bm;
            gr.Dispose();
            return new Bitmap(MergedImage);
        }
    }
}

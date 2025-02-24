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
using System.IO;
using System.Drawing;

namespace CDGNet
{

    public class CDGFile : IDisposable
    {


        #region "Constants"
        //CDG Command Code

        const byte CDG_COMMAND = 0x9;
        //CDG Instruction Codes
        const int CDG_INST_MEMORY_PRESET = 1;
        const int CDG_INST_BORDER_PRESET = 2;
        const int CDG_INST_TILE_BLOCK = 6;
        const int CDG_INST_SCROLL_PRESET = 20;
        const int CDG_INST_SCROLL_COPY = 24;
        const int CDG_INST_DEF_TRANSP_COL = 28;
        const int CDG_INST_LOAD_COL_TBL_LO = 30;
        const int CDG_INST_LOAD_COL_TBL_HIGH = 31;

        const int CDG_INST_TILE_BLOCK_XOR = 38;
        //Bitmask for all CDG fields
        const byte CDG_MASK = 0x3f;
        const int CDG_PACKET_SIZE = 24;
        const int TILE_HEIGHT = 12;

        const int TILE_WIDTH = 6;
        //This is the size of the display as defined by the CDG specification.
        //The pixels in this region can be painted, and scrolling operations
        //rotate through this number of pixels.
        public const int CDG_FULL_WIDTH = 300;

        public const int CDG_FULL_HEIGHT = 216;
        //This is the size of the screen that is actually intended to be
        //visible.  It is the center area of CDG_FULL.  
        const int CDG_DISPLAY_WIDTH = 294;

        const int CDG_DISPLAY_HEIGHT = 204;
        #endregion
        const int COLOUR_TABLE_SIZE = 16;

        #region "Private Declarations"

        private byte[,] m_pixelColours = new byte[CDG_FULL_HEIGHT, CDG_FULL_WIDTH];
        private int[] m_colourTable = new int[COLOUR_TABLE_SIZE];
        private int m_presetColourIndex;
        private int m_borderColourIndex;

        private int m_transparentColour;
        private int m_hOffset;

        private int m_vOffset;
        private CdgFileIoStream m_pStream;
        private ISurface m_pSurface;
        private long m_positionMs;

        private long m_duration;

        //private Bitmap mImage;
        #endregion

        #region "Properties"

        public bool makeTransparent { get; set; }

        public System.Drawing.Image RGBImage
        {
            get
            {
                MemoryStream temp = new MemoryStream();
                try
                {

                    for (int ri = 0; ri <= CDG_FULL_HEIGHT - 1; ri++)
                    {
                        for (int ci = 0; ci <= CDG_FULL_WIDTH - 1; ci++)
                        {
                            int ARGBInt = Convert.ToInt32(m_pSurface.rgbData[ri, ci]);
                            byte[] myByte = new byte[4];
                            myByte = BitConverter.GetBytes(ARGBInt);
                            temp.Write(myByte, 0, 4);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
                Stream toto = (Stream)temp;
                Bitmap myBitmap = GraphicUtil.StreamToBitmap(ref toto, CDG_FULL_WIDTH, CDG_FULL_HEIGHT);
                if (makeTransparent)
                {
                    myBitmap.MakeTransparent(myBitmap.GetPixel(1, 1));
                }
                return myBitmap;
            }
        }


        #endregion

        #region "Public Methods"

        //Png Export
        public void SavePng(string filename)
        {
            RGBImage.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
        }

        //New
        public CDGFile(string cdgFileName)
        {
            m_pStream = new CdgFileIoStream();
            m_pStream.open(cdgFileName);
            m_pSurface = new ISurface();
            if (m_pStream != null && m_pSurface != null)
            {
                this.reset();
                m_duration = ((m_pStream.getsize() / CDG_PACKET_SIZE) * 1000) / 300;
            }
        }

        public long getTotalDuration()
        {
            return m_duration;
        }

        public bool renderAtPosition(long ms)
        {
            CdgPacket pack = new CdgPacket();
            long numPacks = 0;
            bool res = true;

            if ((m_pStream == null))
            {
                return false;
            }

            if ((ms < m_positionMs))
            {
                if ((m_pStream.seek(0, SeekOrigin.Begin) < 0))
                    return false;
                m_positionMs = 0;
            }

            //duration of one packet is 1/300 seconds (4 packets per sector, 75 sectors per second)

            numPacks = ms - m_positionMs;
            numPacks /= 10;

            m_positionMs += numPacks * 10;
            numPacks *= 3;

            //TODO: double check logic due to inline while loop fucntionality
            //AndAlso m_pSurface.rgbData Is Nothing
            while (numPacks > 0)
            {
                res = readPacket(ref pack);
                processPacket(ref pack);
                numPacks -= 1;
            }

            render();
            return res;

        }

        #endregion

        #region "Private Methods"


        private void reset()
        {
            Array.Clear(m_pixelColours, 0, Convert.ToInt32(m_pixelColours.LongLength));
            Array.Clear(m_colourTable, 0, Convert.ToInt32(m_colourTable.LongLength));

            m_presetColourIndex = 0;
            m_borderColourIndex = 0;
            m_transparentColour = 0;
            m_hOffset = 0;
            m_vOffset = 0;

            m_duration = 0;
            m_positionMs = 0;

            //clear surface 
            if ((m_pSurface.rgbData != null))
            {
                Array.Clear(m_pSurface.rgbData, 0, Convert.ToInt32(m_pSurface.rgbData.LongLength));
            }

        }

        private bool readPacket(ref CdgPacket pack)
        {

            if (m_pStream == null || m_pStream.eof() == 1)
            {
                return false;
            }

            int read = 0;

            read += m_pStream.read(ref pack.command, 1);
            read += m_pStream.read(ref pack.instruction, 1);
            read += m_pStream.read(ref pack.parityQ, 2);
            read += m_pStream.read(ref pack.data, 16);
            read += m_pStream.read(ref pack.parityP, 4);

            return (read == 24);
        }


        private void processPacket(ref CdgPacket pack)
        {
            int inst_code = 0;

            if (((pack.command[0] & CDG_MASK) == CDG_COMMAND))
            {
                inst_code = (pack.instruction[0] & CDG_MASK);
                switch (inst_code)
                {
                    case CDG_INST_MEMORY_PRESET:
                        memoryPreset(ref pack);
                        return;
                        
                    case CDG_INST_BORDER_PRESET:
                        borderPreset(ref pack);
                        return;
                        
                    case CDG_INST_TILE_BLOCK:
                        tileBlock(ref pack, false);
                        return;
                        
                    case CDG_INST_SCROLL_PRESET:
                        scroll(ref pack, false);
                        return;
                        
                    case CDG_INST_SCROLL_COPY:
                        scroll(ref pack, true);
                        return;
                        
                    case CDG_INST_DEF_TRANSP_COL:
                        defineTransparentColour(ref pack);
                        return;
                        
                    case CDG_INST_LOAD_COL_TBL_LO:
                        loadColorTable(ref pack, 0);
                        return;
                        
                    case CDG_INST_LOAD_COL_TBL_HIGH:
                        loadColorTable(ref pack, 1);
                        return;
                        
                    case CDG_INST_TILE_BLOCK_XOR:
                        tileBlock(ref pack, true);
                        return;
                        
                    default:
                        //Ignore the unsupported commands
                        return;                        
                }
            }
        }


        private void memoryPreset(ref CdgPacket pack)
        {
            int colour = 0;
            int ri = 0;
            int ci = 0;
            int repeat = 0;

            colour = pack.data[0] & 0xf;
            repeat = pack.data[1] & 0xf;

            //Our new interpretation of CD+G Revealed is that memory preset
            //commands should also change the border
            m_presetColourIndex = colour;
            m_borderColourIndex = colour;

            //we have a reliable data stream, so the repeat command 
            //is executed only the first time


            if ((repeat == 0))
            {
                //Note that this may be done before any load colour table
                //commands by some CDGs. So the load colour table itself
                //actual recalculates the RGB values for all pixels when
                //the colour table changes.

                //Set the preset colour for every pixel. Must be stored in 
                //the pixel colour table indeces array

                for (ri = 0; ri <= CDG_FULL_HEIGHT - 1; ri++)
                {
                    for (ci = 0; ci <= CDG_FULL_WIDTH - 1; ci++)
                    {
                        m_pixelColours[ri, ci] = Convert.ToByte(colour);
                    }
                }
            }

        }


        private void borderPreset(ref CdgPacket pack)
        {
            int colour = 0;
            int ri = 0;
            int ci = 0;

            colour = pack.data[0] & 0xf;
            m_borderColourIndex = colour;

            //The border area is the area contained with a rectangle 
            //defined by (0,0,300,216) minus the interior pixels which are contained
            //within a rectangle defined by (6,12,294,204).

            for (ri = 0; ri <= CDG_FULL_HEIGHT - 1; ri++)
            {
                for (ci = 0; ci <= 5; ci++)
                {
                    m_pixelColours[ri, ci] = Convert.ToByte(colour);
                }

                for (ci = CDG_FULL_WIDTH - 6; ci <= CDG_FULL_WIDTH - 1; ci++)
                {
                    m_pixelColours[ri, ci] = Convert.ToByte(colour);
                }
            }

            for (ci = 6; ci <= CDG_FULL_WIDTH - 7; ci++)
            {
                for (ri = 0; ri <= 11; ri++)
                {
                    m_pixelColours[ri, ci] = Convert.ToByte(colour);
                }

                for (ri = CDG_FULL_HEIGHT - 12; ri <= CDG_FULL_HEIGHT - 1; ri++)
                {
                    m_pixelColours[ri, ci] = Convert.ToByte(colour);
                }
            }

        }


        private void loadColorTable(ref CdgPacket pack, int table)
        {

            for (int i = 0; i <= 7; i++)
            {
                //[---high byte---]   [---low byte----]
                //7 6 5 4 3 2 1 0     7 6 5 4 3 2 1 0
                //X X r r r r g g     X X g g b b b b

                byte byte0 = pack.data[2 * i];
                byte byte1 = pack.data[2 * i + 1];
                int red = (byte0 & 0x3f) >> 2;
                int green = ((byte0 & 0x3) << 2) | ((byte1 & 0x3f) >> 4);
                int blue = byte1 & 0xf;

                red *= 17;
                green *= 17;
                blue *= 17;

                if (m_pSurface != null)
                {
                    m_colourTable[i + table * 8] = m_pSurface.MapRGBColour(red, green, blue);
                }
            }

        }


        private void tileBlock(ref CdgPacket pack, bool bXor)
        {
            int colour0 = 0;
            int colour1 = 0;
            int column_index = 0;
            int row_index = 0;
            int myByte = 0;
            int pixel = 0;
            int xor_col = 0;
            int currentColourIndex = 0;
            int new_col = 0;

            colour0 = pack.data[0] & 0xf;
            colour1 = pack.data[1] & 0xf;
            row_index = ((pack.data[2] & 0x1f) * 12);
            column_index = ((pack.data[3] & 0x3f) * 6);

            if ((row_index > (CDG_FULL_HEIGHT - TILE_HEIGHT)))
                return;
            if ((column_index > (CDG_FULL_WIDTH - TILE_WIDTH)))
                return;

            //Set the pixel array for each of the pixels in the 12x6 tile.
            //Normal = Set the colour to either colour0 or colour1 depending
            //on whether the pixel value is 0 or 1.
            //XOR = XOR the colour with the colour index currently there.


            for (int i = 0; i <= 11; i++)
            {
                myByte = (pack.data[4 + i] & 0x3f);
                for (int j = 0; j <= 5; j++)
                {
                    pixel = (myByte >> (5 - j)) & 0x1;
                    if ((bXor))
                    {
                        //Tile Block XOR 
                        if ((pixel == 0))
                        {
                            xor_col = colour0;
                        }
                        else {
                            xor_col = colour1;
                        }

                        //Get the colour index currently at this location, and xor with it 
                        currentColourIndex = m_pixelColours[row_index + i, column_index + j];
                        new_col = currentColourIndex ^ xor_col;
                    }
                    else {
                        if ((pixel == 0))
                        {
                            new_col = colour0;
                        }
                        else {
                            new_col = colour1;
                        }
                    }

                    //Set the pixel with the new colour. We set both the surfarray
                    //containing actual RGB values, as well as our array containing
                    //the colour indexes into our colour table. 
                    m_pixelColours[row_index + i, column_index + j] = Convert.ToByte(new_col);
                }

            }
        }

        private void defineTransparentColour(ref CdgPacket pack)
        {
            m_transparentColour = pack.data[0] & 0xf;
        }


        private void scroll(ref CdgPacket pack, bool copy)
        {
            int colour = 0;
            int hScroll = 0;
            int vScroll = 0;
            int hSCmd = 0;
            int hOffset = 0;
            int vSCmd = 0;
            int vOffset = 0;
            int vScrollPixels = 0;
            int hScrollPixels = 0;

            //Decode the scroll command parameters
            colour = pack.data[0] & 0xf;
            hScroll = pack.data[1] & 0x3f;
            vScroll = pack.data[2] & 0x3f;

            hSCmd = (hScroll & 0x30) >> 4;
            hOffset = (hScroll & 0x7);
            vSCmd = (vScroll & 0x30) >> 4;
            vOffset = (vScroll & 0xf);


            m_hOffset = hOffset < 5 ? hOffset : 5;
            m_vOffset = vOffset < 11 ? vOffset : 11;

            //Scroll Vertical - Calculate number of pixels

            vScrollPixels = 0;
            if ((vSCmd == 2))
            {
                vScrollPixels = -12;
            }
            else if ((vSCmd == 1))
            {
                vScrollPixels = 12;
            }

            //Scroll Horizontal- Calculate number of pixels

            hScrollPixels = 0;
            if ((hSCmd == 2))
            {
                hScrollPixels = -6;
            }
            else if ((hSCmd == 1))
            {
                hScrollPixels = 6;
            }

            if ((hScrollPixels == 0 && vScrollPixels == 0))
            {
                return;
            }

            //Perform the actual scroll.

            byte[,] temp = new byte[CDG_FULL_HEIGHT + 1, CDG_FULL_WIDTH + 1];
            int vInc = vScrollPixels + CDG_FULL_HEIGHT;
            int hInc = hScrollPixels + CDG_FULL_WIDTH;
            int ri = 0;
            //row index
            int ci = 0;
            //column index

            for (ri = 0; ri <= CDG_FULL_HEIGHT - 1; ri++)
            {
                for (ci = 0; ci <= CDG_FULL_WIDTH - 1; ci++)
                {
                    temp[(ri + vInc) % CDG_FULL_HEIGHT, (ci + hInc) % CDG_FULL_WIDTH] = m_pixelColours[ri, ci];
                }
            }


            //if copy is false, we were supposed to fill in the new pixels
            //with a new colour. Go back and do that now.


            if ((copy == false))
            {

                if ((vScrollPixels > 0))
                {
                    for (ci = 0; ci <= CDG_FULL_WIDTH - 1; ci++)
                    {
                        for (ri = 0; ri <= vScrollPixels - 1; ri++)
                        {
                            temp[ri, ci] = Convert.ToByte(colour);
                        }
                    }


                }
                else if ((vScrollPixels < 0))
                {
                    for (ci = 0; ci <= CDG_FULL_WIDTH - 1; ci++)
                    {
                        for (ri = CDG_FULL_HEIGHT + vScrollPixels; ri <= CDG_FULL_HEIGHT - 1; ri++)
                        {
                            temp[ri, ci] = Convert.ToByte(colour);
                        }
                    }

                }


                if ((hScrollPixels > 0))
                {
                    for (ci = 0; ci <= hScrollPixels - 1; ci++)
                    {
                        for (ri = 0; ri <= CDG_FULL_HEIGHT - 1; ri++)
                        {
                            temp[ri, ci] = Convert.ToByte(colour);
                        }
                    }


                }
                else if ((hScrollPixels < 0))
                {
                    for (ci = CDG_FULL_WIDTH + hScrollPixels; ci <= CDG_FULL_WIDTH - 1; ci++)
                    {
                        for (ri = 0; ri <= CDG_FULL_HEIGHT - 1; ri++)
                        {
                            temp[ri, ci] = Convert.ToByte(colour);
                        }
                    }

                }

            }

            //Now copy the temporary buffer back to our array

            for (ri = 0; ri <= CDG_FULL_HEIGHT - 1; ri++)
            {
                for (ci = 0; ci <= CDG_FULL_WIDTH - 1; ci++)
                {
                    m_pixelColours[ri, ci] = temp[ri, ci];
                }
            }

        }


        private void render()
        {
            if ((m_pSurface == null))
                return;
            for (int ri = 0; ri <= CDG_FULL_HEIGHT - 1; ri++)
            {
                for (int ci = 0; ci <= CDG_FULL_WIDTH - 1; ci++)
                {
                    if ((ri < TILE_HEIGHT || ri >= CDG_FULL_HEIGHT - TILE_HEIGHT || ci < TILE_WIDTH || ci >= CDG_FULL_WIDTH - TILE_WIDTH))
                    {
                        m_pSurface.rgbData[ri, ci] = m_colourTable[m_borderColourIndex];
                    }
                    else {
                        m_pSurface.rgbData[ri, ci] = m_colourTable[m_pixelColours[ri + m_vOffset, ci + m_hOffset]];
                    }
                }
            }

        }

        #endregion

        // To detect redundant calls
        private bool disposedValue = false;

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    m_pStream.close();
                }
                m_pStream = null;
                m_pSurface = null;
            }
            this.disposedValue = true;
        }

        #region " IDisposable Support "
        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }

    public class CdgPacket
    {
        public byte[] command = new byte[1];
        public byte[] instruction = new byte[1];
        public byte[] parityQ = new byte[2];
        public byte[] data = new byte[16];
        public byte[] parityP = new byte[4];
    }

    public class ISurface
    {


        public long[,] rgbData = new long[CDGFile.CDG_FULL_HEIGHT, CDGFile.CDG_FULL_WIDTH];
        public int MapRGBColour(int red, int green, int blue)
        {
            return Color.FromArgb(red, green, blue).ToArgb();
        }

    }


}

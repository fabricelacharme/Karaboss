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
using System.IO;

namespace CDGNet
{
    public class CdgFileIoStream
    {

        #region "Declarations"

        private Stream m_file;
        #endregion

        #region "Public Methods"
        public CdgFileIoStream()
        {
            m_file = null;
        }

        public int read(ref byte[] buf, int buf_size) {
            return m_file.Read(buf, 0, buf_size);
        }

        public int write(ref byte[] buf, int buf_size) {
            m_file.Write(buf, 0, buf_size);
            return 1;
        }

        public long seek(int offset, SeekOrigin whence) {
            return m_file.Seek(offset, whence);
        }

        public int eof() {
           if (m_file.Position >= m_file.Length) 
                return 1;
            else
                return 0;
            }

        public long getsize() {
            return m_file.Length;
        }

        public bool open(string filename) {
            close();
            m_file = new FileStream(filename, FileMode.Open);
            return (m_file != null);
        }

        public void close() {
            if (m_file != null) {
                m_file.Close();
                m_file = null;
            }
        }

        #endregion
    }
}

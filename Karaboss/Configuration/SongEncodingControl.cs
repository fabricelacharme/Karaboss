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
using System.Collections.Generic;
using System.Windows.Forms;

namespace Karaboss.Configuration
{
    public partial class SongEncodingControl : ConfigurationBaseControl
    {
        private ComboBox m_langCB;
        private Label m_langL;

        public class Encoding
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public SongEncodingControl(string configName): base(configName)
        {
            InitializeComponent();
            populateEncodings();
        }

        /// <summary>
        /// Populate existing Encodings
        /// </summary>
        private void populateEncodings()
        {
            //Build a list
            var dataSource = new List<Encoding>();
            dataSource.Add(new Encoding() { Name = "Ascii", Value = "Ascii" });
            dataSource.Add(new Encoding() { Name = "Chinese", Value = "cn" });
            dataSource.Add(new Encoding() { Name = "Japanese", Value = "jp" });
            dataSource.Add(new Encoding() { Name = "Korean", Value = "kr" });
            dataSource.Add(new Encoding() { Name = "Vietnamese", Value = "vn" });

            //Setup data binding
            this.m_langCB.DataSource = dataSource;
            this.m_langCB.DisplayMember = "Name";
            this.m_langCB.ValueMember = "Value";

            // make it readonly
            this.m_langCB.DropDownStyle = ComboBoxStyle.DropDownList;

            // value
            m_langCB.Text = Karaclass.m_textEncoding;

        }

        public override void Restore()
        {
        }

        public override void Apply()
        {
            Karaclass.m_textEncoding = m_langCB.Text;
            Properties.Settings.Default.textEncoding = m_langCB.Text;
            Properties.Settings.Default.Save();
        }

    }
}

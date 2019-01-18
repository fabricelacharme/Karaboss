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

namespace Karaboss.Configuration
{
    public partial class UpdControl : ConfigurationBaseControl
    {
        public UpdControl(string configName) : base(configName)
        {
            InitializeComponent();
            populateFields();           
        }


        private void populateFields()
        {
            chkUpdateProgram.Checked = Properties.Settings.Default.CheckForUpdates;
            chkUpdFreq.Text = Properties.Settings.Default.UpdFrequency;
            txtWebSite.Text = Properties.Settings.Default.RemoteUrl;

        }

        public override void Restore()
        {
        }

        public override void Apply()
        {
            Properties.Settings.Default.CheckForUpdates = chkUpdateProgram.Checked;
            Properties.Settings.Default.UpdFrequency = chkUpdFreq.Text;
            Properties.Settings.Default.RemoteUrl = txtWebSite.Text.Trim();
            Properties.Settings.Default.Save();
        }

    }
}

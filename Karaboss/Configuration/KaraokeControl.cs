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
using System.Windows.Forms;

namespace Karaboss.Configuration
{
    public partial class KaraokeControl : ConfigurationBaseControl
    {
        private Label lblMuteMelody;
        private CheckBox chkMuteMelody;

        public KaraokeControl(string configName) : base(configName)
        {
            InitializeComponent();
            populateValues();
        }

        private void populateValues()
        {
            chkMuteMelody.Checked = Karaclass.m_MuteMelody;
            chkDisplayBalls.Checked = Karaclass.m_DisplayBalls;
        }

        public override void Restore()
        {
        }

        public override void Apply()
        {
            Karaclass.m_MuteMelody = chkMuteMelody.Checked;
            Karaclass.m_DisplayBalls = chkDisplayBalls.Checked;

            Properties.Settings.Default.MuteMelody = Karaclass.m_MuteMelody;
            Properties.Settings.Default.DisplayBalls = Karaclass.m_DisplayBalls;
            Properties.Settings.Default.Save();

        }

    }
}

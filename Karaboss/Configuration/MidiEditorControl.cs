﻿#region License

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


namespace Karaboss.Configuration
{
    public partial class MidiEditorControl : ConfigurationBaseControl
    {
        public MidiEditorControl(string configName) : base(configName)
        {
            InitializeComponent();
            PopulateValues();
        }

        private void PopulateValues()
        {
            UpDownTransposeAmount.Value = Karaclass.m_TransposeAmount;
            UpDownVelocity.Value = Karaclass.m_Velocity;
        }

        public override void Restore()
        {
        }

        public override void Apply()
        {
            Karaclass.m_TransposeAmount = Convert.ToInt32(UpDownTransposeAmount.Value);
            Karaclass.m_Velocity = Convert.ToInt32(UpDownVelocity.Value);

            Properties.Settings.Default.TransposeAmount = Karaclass.m_TransposeAmount;           
            Properties.Settings.Default.Save();

        }

        
    }
}

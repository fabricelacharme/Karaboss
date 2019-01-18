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
using System.Configuration;
using System.Windows.Forms;

namespace PrgAutoUpdater
{

    public static class Util
    {
        // the name of the setting that flags whether we
        // should perform an upgrade or not
        private const string UpgradeFlag = "RetrieveSettings";

        public static void DoUpgrade(ApplicationSettingsBase settings)
        {
            try
            {
                // attempt to read the upgrade flag
                if ((bool)settings[UpgradeFlag] == true)
                {
                    // upgrade the settings to the latest version
                    settings.Upgrade();

                    // clear the upgrade flag
                    settings[UpgradeFlag] = false;
                    settings.Save();

                    string tx = "New version of Karaboss: User settings updated";
                    MessageBox.Show(tx);

                }
                else
                {
                    // the settings are up to date
                    
                }
            }
            catch (SettingsPropertyNotFoundException ex)
            {
                // notify the developer that the upgrade
                // flag should be added to the settings file
                throw ex;
            }

        }

    }
}

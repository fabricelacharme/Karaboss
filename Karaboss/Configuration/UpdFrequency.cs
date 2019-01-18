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

namespace Karaboss.Configuration
{
    class UpdFrequency
    {
        public enum freqChoices { Daily, Weekly, Monthly};
        private static freqChoices freqChoice;

        public UpdFrequency(string freq)
        {
            switch (freq)
            {
                case "Monthly":
                    freqChoice = freqChoices.Monthly;
                    break;
                case "Weekly":
                    freqChoice = freqChoices.Weekly;
                    break;
                default:
                    freqChoice = freqChoices.Daily;
                    break;
            }
            
        }


        // Analyse update checking frequency and return true if check for update must be done
        // anf false if check for update must wait
        public bool searchForUpdate()
        {
            DateTime lastCheck;
            DateTime temp;
            string lc = Properties.Settings.Default.lastUpdCheck.ToString();

            try
            {

                if (DateTime.TryParse(lc, out temp))
                    lastCheck = temp;
                else
                    lastCheck = DateTime.Now;

                TimeSpan ts;

                switch (freqChoice)
                {
                    case freqChoices.Monthly:
                        DateTime nextdate = NextMonth(lastCheck);
                        if (DateTime.Now >= nextdate)
                            return true;
                        else
                            return false;
                        //break;

                    case freqChoices.Weekly:
                        ts = DateTime.Now - lastCheck;
                        if (ts.Days >= 7)
                            return true;
                        else
                            return false;
                        //break;

                    default:
                        ts = DateTime.Now - lastCheck;
                        if (ts.Days >= 1)
                            return true;
                        else
                            return false;
                        //break;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return false;
            }
        }



        private static DateTime NextMonth(DateTime date)
        {
            if (date.Day != DateTime.DaysInMonth(date.Year, date.Month))
                return date.AddMonths(1);
            else
                return date.AddDays(1).AddMonths(1).AddDays(-1);
        }


        /// <summary>
        /// Save last date where check for update was performed
        /// </summary>
        /// <param name="date"></param>
        public void saveUpdDate(DateTime date)
        {
            Properties.Settings.Default.lastUpdCheck = date;
            // Save settings
            Properties.Settings.Default.Save();

        }

    }
}

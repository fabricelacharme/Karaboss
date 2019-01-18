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

    // Impossible à afficher en mode design car basé sur une classe abstract
    // voir ce lien pour correction
    // https://stackoverflow.com/questions/6817107/abstract-usercontrol-inheritance-in-visual-studio-designer


    public partial class LangControl : ConfigurationBaseControl
    {
        private ComboBox m_langCB;
        private Label m_langL;
        private string m_lang = string.Empty;
        private string culture = string.Empty;

        public class Language
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
        
        public LangControl(string configName): base(configName)
        {
            InitializeComponent();
            m_lang = Karaclass.m_lang;
            populateLanguages();
        }

        /// <summary>
        /// Populate existing Encodings
        /// </summary>
        private void populateLanguages()
        {
            //Build a list
            var dataSource = new List<Language>();
            dataSource.Add(new Language() { Name = "Français", Value = "fr" });
            dataSource.Add(new Language() { Name = "English", Value = "en" });

            //Setup data binding
            this.m_langCB.DataSource = dataSource;
            this.m_langCB.DisplayMember = "Name";
            this.m_langCB.ValueMember = "Value";

            // make it readonly
            this.m_langCB.DropDownStyle = ComboBoxStyle.DropDownList;

            // value
            m_langCB.Text = Karaclass.m_lang;

        }       
        
        public override void Restore()
        {
        }

        public override void Apply()
        {
            if (Karaclass.m_lang == m_langCB.Text)
                return;

            Karaclass.m_lang = m_langCB.Text;
            Properties.Settings.Default.lang = m_langCB.Text;
            Properties.Settings.Default.Save();



            // Change the culture for the App domain
            // Allow to manage languages for usercontrols
            switch (Karaclass.m_lang)
            {
                case "English":
                    culture = "en-US";
                    break;
                case "Français":
                    culture = "fr-FR";
                    break;
                default:
                    culture = "en-US";
                    break;
            }
            RuntimeLocalizer.ChangeCulture(culture);

           

            // Modifie le formulaire frmExplorer 
            if (Application.OpenForms["frmExplorer"] != null)
            {                
                RuntimeLocalizer.ChangeCultureForm(Application.OpenForms["frmExplorer"], culture);
            }

            
        }

    }
}

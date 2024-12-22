#region Contact

/*
 * Fabrice Lacharme
 * Email: fabrice.lacharme@gmail.com
 */

#endregion
using System;


namespace Karaboss.Configuration
{
    public partial class ChordsControl : ConfigurationBaseControl
    {
        public ChordsControl(string configName) : base(configName)
        {
            InitializeComponent();
            PopulateValues();
        }

        private void PopulateValues()
        {
            chkDisplayChords.Checked = Karaclass.m_ShowChords;
            
        }


        public override void Restore()
        {
            chkDisplayChords.Checked = false;            

        }

        public override void Apply()
        {            
            Karaclass.m_ShowChords = chkDisplayChords.Checked;

            Properties.Settings.Default.bShowChords = Karaclass.m_ShowChords;            
            Properties.Settings.Default.Save();
        }

    }
}

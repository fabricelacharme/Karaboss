#region Contact

/*
 * Fabrice Lacharme
 * Email: fabrice.lacharme@gmail.com
 */

#endregion
using System;


namespace Karaboss.Configuration
{
    public partial class PlaylistsControl : ConfigurationBaseControl
    {
        public PlaylistsControl(string configName) : base(configName)
        {
            InitializeComponent();
            PopulateValues();
        }

        private void PopulateValues()
        {
            chkPauseSongs.Checked = Karaclass.m_PauseBetweenSongs;
            CountDownSong.Value = Karaclass.m_CountdownSongs;
        }


        public override void Restore()
        {
            chkPauseSongs.Checked = true;
            CountDownSong.Value = 5;

        }

        public override void Apply()
        {
            Karaclass.m_CountdownSongs = Convert.ToInt32(CountDownSong.Value);
            Karaclass.m_PauseBetweenSongs = chkPauseSongs.Checked;

            Properties.Settings.Default.bPauseBetweenSongs = Karaclass.m_PauseBetweenSongs;
            Properties.Settings.Default.CountdownSongs = Karaclass.m_CountdownSongs;
            Properties.Settings.Default.Save();
        }

    }
}

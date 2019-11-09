using System.Threading;
//using LotroMusicManager.Properties;

namespace Karaboss.Pages.ABCnotation
{
    class LOTROFocuser
    {
        Timer _timer;

        public LOTROFocuser()
        {
            _timer = new Timer(new TimerCallback(OnTimer));
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            return;
        }

        public void Start()
        {
            _timer.Change(0, Timeout.Infinite);
            return;
        }

        public void Stop()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            return;
        }

        private void OnTimer(object o)
        {
            /*
            if (Settings.Default.KeepLOTROFocused)
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);

                System.Diagnostics.Process[] ap = System.Diagnostics.Process.GetProcessesByName(Settings.Default.ClientAppID);
                
                if (ap.Length > 0 && ap[0].MainWindowHandle != SDK.GetForegroundWindow())
                {
                    SDK.SendMessage(ap[0].MainWindowHandle, SDK.WM_ACTIVATE, 1, 0);
                    SDK.SendMessage(ap[0].MainWindowHandle, SDK.WM_SETFOCUS, 0, 0);
                }
                
            } // Settings say to send the message(s)

            // Restart the timer
            _timer.Change(Settings.Default.FocusTimeout, Timeout.Infinite);
            */
            return;
        }
    }
}

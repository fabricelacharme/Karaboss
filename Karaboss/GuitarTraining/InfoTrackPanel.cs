using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Karaboss.GuitarTraining
{
    public partial class InfoTrackPanel : UserControl
    {

        // Delete track button
        public delegate void RemoveTrackEventHandler(object sender, EventArgs e, int track);
        public event RemoveTrackEventHandler OnRemoveTrack;

        #region properties

        /// <summary>
        /// ForeColor of Labels
        /// </summary>
        private Color _textcolor;
        public Color TextColor
        {
            get { return _textcolor; }
            set {
                _textcolor = value;
                foreach (Control item in Controls)
                {
                    if (item.GetType() == typeof(Label))
                    {
                        item.ForeColor = _textcolor;
                    }
                }

                this.lblInstrument.ForeColor = _textcolor;
                this.lblTrackName.ForeColor = _textcolor;
                this.lblTrackNumber.ForeColor = _textcolor;
                this.lblChannel.ForeColor = _textcolor;
                Invalidate();
            }
        }

        private int _tracknumber;
        public int TrackNumber
        {
            get { return _tracknumber; }
            set
            {
                _tracknumber = value;
                this.lblTrackNumber.Text = string.Format("Track: {0}", value + 1);
            }
        }

        
        private string _trackname;
        public string TrackName
        {
            get { return _trackname; }
            set
            {
                _trackname = value;
                this.lblTrackName.Text = value;
            }
        }

        private string _instrument;
        public string Instrument
        {
            get { return _instrument; }
            set
            {
                _instrument = value;
                this.lblInstrument.Text = value;
            }
        }

        private string _channel;
        public string Channel
        {
            get { return _channel; }
            set
            {
                _channel = value;
                this.lblChannel.Text = value;
            }
        }


        #endregion


        public InfoTrackPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Delete track display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            string message = "Do you want to remove from the display the track number " + _tracknumber + "?";
            string caption = "Karaboss";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            // Displays the MessageBox.
            result = MessageBox.Show(message, caption, buttons);

            if (result == DialogResult.Yes)
                OnRemoveTrack?.Invoke(this, e, _tracknumber);
        }
    }
}

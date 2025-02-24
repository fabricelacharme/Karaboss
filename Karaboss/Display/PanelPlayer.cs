using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Karaboss.Display
{
    public partial class PanelPlayer : UserControl
    {
                
        public PanelPlayer()
        {
            InitializeComponent();
        }

        public void DisplayStatus(string tx) 
        { 
            switch (tx) 
            {
                case "Playing":
                    lblStatus.ForeColor = Color.LightGreen;
                    break;
                case "Paused":
                    lblStatus.ForeColor = Color.Yellow;
                    break;
                case "Stopped":
                    lblStatus.ForeColor = Color.Red;
                    break;
                default:
                    lblStatus.ForeColor = Color.White;
                    break;
            }
            lblStatus.Text = tx; 
        
        }

        public void DisplayBeat(string tx) { lblBeat.Text = tx;}

        public void DisplayDuration(string tx) { lblDuration.Text = tx;}

        public void DisplayPercent(string tx) { lblPercent.Text = tx;}

        public void DisplayElapsed(string tx) { lblElapsed.Text = tx;}
    }
}

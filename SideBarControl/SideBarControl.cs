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
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace VBarControl.SideBarControl
{
    /* A menu bar containing buttons 
     * 
     * Each button has a picture and a label
     * its backcolor change on MouseHover, MouseLeave and Click if the button is selectable
     * Purpose: simulate html toolbar behaviour   
     * 
     * HOME : displays the home directory of music library (default Music)
     * SEARCH: displays the search engine window
     * FILES: displays the Explorer window
     * PLAYLISTS: displays the playlists window
     * PLAY: launch the selected midi file
     * EDIT: open the midi editor for the selected file
     * PIANOTRAINING: open piano training page
     * GUITARTRAINING: open guitar training page
     * CHORDS: open and display chods of the midi file
     * 
     */

    public partial class SideBarControl : UserControl
    {

        public event EventHandler DisplayHome;
        public event EventHandler DisplaySearch;
        public event EventHandler DisplayFiles;
        public event EventHandler DisplayPlaylists;
        public event EventHandler DisplayConnected;
        public event EventHandler PlayFile;
        public event EventHandler EditFile;
        public event EventHandler DisplayPianoTraining;
        public event EventHandler DisplayGuitarTraining;
        public event EventHandler DisplayChords;



        private ObservableCollection<NavButton.NavButton> _buttons = new ObservableCollection<NavButton.NavButton>();
        public ObservableCollection<NavButton.NavButton> Buttons
        {
            get { return _buttons; }
            
        }
            
            
         //   = new ObservableCollection<NavButton.NavButton>();

        ToolTip tipBtn = new System.Windows.Forms.ToolTip();

        public enum Selectables
        {
            Home = 1,
            Search,
            Files,
            Playlists,
        }



        #region Properties

        private string _tooltiptextHome = "home"; 
        public string ToolTipTextHome
        {
            get { return btnHome.ToolTipText; }
            set {
                _tooltiptextHome = value;
                tipBtn.SetToolTip(this.btnHome, _tooltiptextHome);
                btnHome.ToolTipText = value;
            }
        }
        
        /// <summary>
        /// Select one of the selectable buttons button
        /// </summary>
        public Selectables SelectedItem
        {
            set
            {
                switch (value)
                {
                    case Selectables.Home:
                        btnHome.Selected = true;
                        break;
                    case  Selectables.Files:
                        btnFiles.Selected = true;                        
                        break;
                    case Selectables.Search:
                        btnSearch.Selected = true;
                        break;
                    case Selectables.Playlists:
                        btnPlaylists.Selected = true;
                        break;
                }

            }
        }
        

        #endregion       

        // Constructor
        public SideBarControl()
        {

            InitializeComponent();          

            foreach(Control ctrl in Controls)
            {
                if (ctrl.GetType() == typeof(NavButton.NavButton))
                {
                    _buttons.Add((NavButton.NavButton)ctrl);
                }
            }

            // Set if selectable or not
            btnHome.Selectable = true;
            btnSearch.Selectable = true;
            btnFiles.Selectable = true;
            btnPlaylists.Selectable = true;
            btnConnected.Selectable = true;

            btnPlay.Selectable = false;
            btnEdit.Selectable = false;
            btnPianoTraining.Selectable = false;
            btnGuitarTraining.Selectable = false;
            btnChords.Selectable = false;

            // default panel is file explorer
            btnFiles.Selected = true;

            btnHome.ToolTipText = _tooltiptextHome;            
        }

        
        public void ShowButton(string Name, bool bvisible)
        {
            NavButton.NavButton btn = _buttons.Where(b => b.Name == Name).FirstOrDefault();
            if (btn != null) btn.Visible = bvisible;
        }

        #region buttons click
        private void btnSearch_Click(object sender, EventArgs e)
        {
            DisplaySearch?.Invoke(new object(), new EventArgs());
        }

        private void btnFiles_Click(object sender, EventArgs e)
        {
            DisplayFiles?.Invoke(new object(), new EventArgs());
        }

        private void btnPlaylists_Click(object sender, EventArgs e)
        {
            DisplayPlaylists?.Invoke(new object(), new EventArgs());
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            PlayFile?.Invoke(new object(), new EventArgs());
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            EditFile?.Invoke(new object(), new EventArgs());
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            DisplayHome?.Invoke(new object(), new EventArgs());
        }
        
        private void btnConnected_Click(object sender, EventArgs e)
        {
            DisplayConnected?.Invoke(new object(), new EventArgs());
        }
        
        private void btnPianoTraining_Click(object sender, EventArgs e)
        {
            DisplayPianoTraining?.Invoke(new object(), new EventArgs());
        }
       
        private void btnGuitarTraining_Click(object sender, EventArgs e)
        {            
            DisplayGuitarTraining?.Invoke(new object(), new EventArgs());
        }

        #endregion

        private void btnChords_Click(object sender, EventArgs e)
        {
            DisplayChords?.Invoke(new object(), new EventArgs());
        }
    }


    /// <summary>
    /// Localization
    /// </summary>
    public static class RuntimeLocalizer
    {

        /// <summary>
        /// Change lang
        /// </summary>
        /// <param name="frm"></param>
        /// <param name="cultureCode"></param>
        public static void ChangeCulture(Control control, string cultureCode)
        {
            CultureInfo culture = CultureInfo.GetCultureInfo(cultureCode);

            //Set the language in the application
            Thread.CurrentThread.CurrentUICulture = culture;

            ComponentResourceManager resources = new ComponentResourceManager(control.GetType());

            ApplyResourceToControl(resources, control, culture);

            resources.ApplyResources(control, "$this", culture);
        }

        /// <summary>
        /// Change Text property of controls
        /// </summary>
        /// <param name="resources"></param>
        /// <param name="control"></param>
        /// <param name="lang"></param>
        private static void ApplyResourceToControl(ComponentResourceManager resources, Control control, CultureInfo lang)
        {
            if (control.GetType() == typeof(MenuStrip))  // See if this is a menuStrip
            {
                MenuStrip strip = (MenuStrip)control;
                ApplyResourceToToolStripItemCollection(strip.Items, resources, lang);
            }

            foreach (Control c in control.Controls) // Apply to all sub-controls
            {
                ApplyResourceToControl(resources, c, lang);
                resources.ApplyResources(c, c.Name, lang);
            }
            // Apply to self
            resources.ApplyResources(control, control.Name, lang);
        }

        /// <summary>
        /// Change Text property of menus
        /// </summary>
        /// <param name="col"></param>
        /// <param name="resources"></param>
        /// <param name="lang"></param>
        private static void ApplyResourceToToolStripItemCollection(ToolStripItemCollection col, ComponentResourceManager resources, CultureInfo lang)
        {
            for (int i = 0; i < col.Count; i++)     // Apply to all sub items
            {
                if (col[i].GetType().ToString() != "System.Windows.Forms.ToolStripSeparator") // Erreur sur separator
                {
                    ToolStripItem item = (ToolStripMenuItem)col[i];

                    if (item.GetType() == typeof(ToolStripMenuItem))
                    {
                        ToolStripMenuItem menuitem = (ToolStripMenuItem)item;
                        ApplyResourceToToolStripItemCollection(menuitem.DropDownItems, resources, lang);
                    }
                    resources.ApplyResources(item, item.Name, lang);
                }
            }
        }
    }
}

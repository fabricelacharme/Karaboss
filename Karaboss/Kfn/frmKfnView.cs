using FlShell.Interop;
using Karaboss.Resources.Localization;
using Karaboss.xplorer;
using Mozilla.NUniversalCharDet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Karaboss.Kfn
{
    public partial class frmKfnView : Form
    {

        //private ContextMenuStrip lvContextMenu;
        private ContextMenu lvContextMenu;

        private string windowTitle = "KFN Viewer";
        private KFN KFN;
        private SongINI sINI;

        private readonly Dictionary<int, string> encodings = new Dictionary<int, string>
        { { 0, "Use auto detect" } };


        public frmKfnView(string FullFileName)
        {
            InitializeComponent();

            InitControls();

            string version = System.Windows.Forms.Application.ProductVersion;
            windowTitle += " v." + version.Remove(version.Length - 2);
            Text = windowTitle;

            OpenFileDialog.Filter = "KFN files (*.kfn)|*.kfn|All files (*.*)|*.*";

            ReadFile(FullFileName);
        }


        #region init controls

        private void InitControls()
        {            
            InitEncodings();
            
            // Reset lists 
            InitLvProperties();
            InitLvResources();

            pnlLeft.Top = pnlTop.Top + pnlTop.Height;
            pnlRight.Top = pnlLeft.Top;

            pnlLeft.Left = 0;

            pnlLeft.Width = 3 * ClientSize.Width / 7;
            pnlRight.Left = pnlLeft.Width;
            pnlRight.Width = 4 * ClientSize.Width / 7;

            pnlLeft.Height = this.ClientSize.Height - pnlTop.Height - 30;
            pnlRight.Height = pnlLeft.Height;
        }


        private void InitLvProperties()
        {
            int W_Name = 120; int W_Value =380;

            // Affichage mode détails
            lvProperties.View = View.Details;
            // Autoriser l'édition in place
            lvProperties.LabelEdit = false;
            // keep selection visible
            lvProperties.HideSelection = false;

            lvProperties.FullRowSelect = true;            


            lvProperties.Clear();
            lvProperties.Refresh();

            #region header
            //Headers listview     
            ColumnHeader columnHeader0 = new ColumnHeader()
            {
                Text = "Name",
                Width = W_Name,
            };

            ColumnHeader columnHeader1 = new ColumnHeader()
            {
                Text = "Value",
                Width = W_Value,
            };           
          
            lvResources.CheckBoxes = true;

            
            // Add the column headers to myListView.
            lvProperties.Columns.AddRange(new ColumnHeader[]
                {columnHeader0, columnHeader1, });

            #endregion

            // NO Sorting
            lvProperties.Sorting = SortOrder.None;
            lvProperties.AllowDrop = false;

        }

        private void InitLvResources()
        {

            // TODO https://gist.github.com/sharat/3879044 
            // Checkbox in the first header

            int W_IsExported = 30; int W_AESEnc = 60;
            int W_Type = 60; int W_Name = 380; int W_Size = 80;

            // Affichage mode détails
            lvResources.View = View.Details;
            // Autoriser l'édition in place
            lvResources.LabelEdit = false;
            // keep selection visible
            lvResources.HideSelection = false;

            lvResources.FullRowSelect = true;

            lvResources.ShowItemToolTips = true;

            lvResources.Clear();
            lvResources.Refresh();

            #region header
            //Headers listview     
            ColumnHeader columnHeader0 = new ColumnHeader()
            {
                Text = "Ex",
                Width = W_IsExported,
                
            };
            ColumnHeader columnHeader1 = new ColumnHeader()
            {
                Text = "AES Enc",
                Width = W_AESEnc,
            };

            ColumnHeader columnHeader2 = new ColumnHeader()
            {
                Text = "Type",
                Width = W_Type,
            };

            ColumnHeader columnHeader3 = new ColumnHeader()
            {
                Text = "Name",
                Width = W_Name,
            };

            ColumnHeader columnHeader4 = new ColumnHeader()
            {
                Text = "Size",
                Width = W_Size,
            };
            

            // Add the column headers to myListView.
            lvResources.Columns.AddRange(new ColumnHeader[]
                {columnHeader0, columnHeader1, columnHeader2, columnHeader3, columnHeader4 });

            #endregion

            // NO Sorting
            lvResources.Sorting = SortOrder.None;
            lvResources.AllowDrop = false;
        }

        private void InitEncodings()
        {
            foreach (EncodingInfo enc in Encoding.GetEncodings())
            {
                encodings.Add(enc.CodePage, enc.CodePage + ": " + enc.DisplayName);
            }

            foreach (KeyValuePair<int, string> enc in encodings)
            {
                ToolStripMenuItem mi = new ToolStripMenuItem
                {
                    Text = enc.Value,
                    Tag = enc.Key,
                    //IsCheckable = true
                };
                mi.Click += mnuRsourceEncodingSubItem_Select;
                if (enc.Key == 0) { mi.Checked = true; }
                mnuResourceEncoding.DropDownItems.Add(mi);
            }
            mnuResourceEncoding.Enabled = false;
        }

        private void mnuRsourceEncodingSubItem_Select(object sender, EventArgs e)
        {
            int enc = Convert.ToInt32(((ToolStripMenuItem)sender).Tag.ToString());
            foreach (var item in mnuResourceEncoding.DropDownItems)
            {
                ((ToolStripMenuItem)item).Checked =
                    (((ToolStripMenuItem)item).Tag == ((ToolStripMenuItem)sender).Tag)
                    ? true : false;
            }
            if (KFN != null)
            {
                KFN.ReadFile(enc);
                this.UpdateKFN();
            }
        }


        #endregion init controls



        private void ReadFile(string FullFileName)
        {
            // Display the KFN viewver window
            KFN = new KFN(FullFileName);
            if (KFN.isError != null)
            {
                MessageBox.Show(KFN.isError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Text = this.windowTitle + " - " + KFN.FullFileName;
            UpdateKFN();

            KFN.ResourceFile encResource = KFN.Resources.Where(r => r.IsEncrypted == true).FirstOrDefault();
            if (encResource != null) { chkDecryptKFN.Enabled = true; }
        }

        private void UpdateKFN()
        {
            InitLvProperties();
            InitLvResources();

            PopulateLvProperties(); 
            PopulateLvResources();                       

            if (KFN.UnknownProperties.Count > 0)
            {
                MessageBox.Show("This KFN file has properties that programm don`t know." +
                    "\nPlease send this file to madlord80@gmail.com for support");
            }

            mnuResourceEncoding.Enabled = true;

        }


        private void PopulateLvProperties()
        {
            string[] saLvwItem = new string[2];
            foreach (var kv in KFN.Properties)
            {
                saLvwItem[0] = kv.Key;
                saLvwItem[1] = kv.Value;

                // Add rows to lvProperties
                ListViewItem myListViewItem = new ListViewItem(saLvwItem);
                lvProperties.Items.Add(myListViewItem);
            }
        }

        private void PopulateLvResources()
        {
            foreach (var kv in KFN.Resources)
            {
                // IsExported bool                
                ListViewItem item1 = new ListViewItem();
                item1.Checked = kv.IsExported;
                item1.ToolTipText = "Select resource for export to KFN";

                // Add SubItems
                ListViewItem.ListViewSubItem subitem1 = new ListViewItem.ListViewSubItem(item1, kv.IsEncrypted ? "Y" : "N");   // IsEncrypted bool AECEnc (checkbox normally, but unavailable is ListView) 
                ListViewItem.ListViewSubItem subitem2 = new ListViewItem.ListViewSubItem(item1, kv.FileType);
                ListViewItem.ListViewSubItem subitem3 = new ListViewItem.ListViewSubItem(item1, kv.FileName);
                ListViewItem.ListViewSubItem subitem4 = new ListViewItem.ListViewSubItem(item1, kv.FileSize);

                item1.SubItems.AddRange(new ListViewItem.ListViewSubItem[] { subitem1, subitem2, subitem3, subitem4 });
                item1.Tag = kv;
                lvResources.Items.Add(item1);
            }
        }


        #region menus

        private void mnuFileOpenKFN_Click(object sender, EventArgs e)
        {
            if (OpenFileDialog.ShowDialog() != DialogResult.OK) return;
            ReadFile(OpenFileDialog.FileName);
        }

        private void mnuExportToEMZ_Click(object sender, EventArgs e)
        {

        }

        private void mnuExportToMP3LRC_Click(object sender, EventArgs e)
        {

        }

        private void mnuExportKFN_Click(object sender, EventArgs e)
        {

        }

        #endregion menus


        #region context menus

        /// <summary>
        /// Handles the MouseUp event for the resource list view, displaying a context menu with options based on the
        /// selected item and mouse button.
        /// </summary>
        /// <remarks>When the right mouse button is released over a resource item, this method displays a
        /// context menu with options to export or view the selected resource, depending on its type. The available menu
        /// items are determined by the type of the resource associated with the selected item.</remarks>
        /// <param name="sender">The source of the event, typically the ListView control that raised the event.</param>
        /// <param name="e">A MouseEventArgs that contains the event data, including information about which mouse button was released.</param>
        private void lvResources_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ListViewItem focusedItem = lvResources.FocusedItem;
                if (focusedItem != null)
                {
                    lvContextMenu = new ContextMenu();
                    lvContextMenu.MenuItems.Clear();

                    // Export
                    MenuItem mnuExportItem = new MenuItem("Export");                    
                    mnuExportItem.Click += new System.EventHandler(this.mnuExportItem_Click);
                    mnuExportItem.Tag = focusedItem.Tag;
                    lvContextMenu.MenuItems.Add(mnuExportItem);

                    // Add view menu according to type of resource
                    int idx = focusedItem.Index;                    
                    if (idx >= 0 && idx < lvResources.Items.Count && idx < KFN.Resources.Count)
                    {
                        var resource = KFN.Resources[idx];
                        if (resource.FileType == "Text" || resource.FileType == "Image")
                        {
                            MenuItem mnuViewItem = new MenuItem("View");
                            mnuViewItem.Click += new System.EventHandler(this.mnuViewItem_Click);
                            mnuViewItem.Tag = focusedItem.Tag;
                            lvContextMenu.MenuItems.Add(mnuViewItem);
                        }
                    }
                    lvResources.ContextMenu = lvContextMenu;                    
                }                
            }
        }

        private void mnuViewItem_Click(object sender, EventArgs e)
        {                        
            var MI = sender as MenuItem;
            if (MI != null && MI.Tag != null)
            {                
                try
                {
                    KFN.ResourceFile resource = (KFN.ResourceFile)MI.Tag;
                    DisplayResource(resource);                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void DisplayResource(KFN.ResourceFile resource)
        {
            if (resource.FileType == "Text")
            {
                byte[] data = KFN.GetDataFromResource(resource);

                ////UTF-8
                int detEncoding = 65001;
                UniversalDetector Det = new UniversalDetector(null);
                Det.HandleData(data, 0, data.Length);
                Det.DataEnd();
                string enc = Det.GetDetectedCharset();
                if (enc != null && enc != "Not supported")
                {
                    // fix encoding for 1251 upper case and MAC
                    //if (enc == "KOI8-R" || enc == "X-MAC-CYRILLIC") { enc = "WINDOWS-1251"; }
                    Encoding denc = Encoding.GetEncoding(enc);
                    detEncoding = denc.CodePage;
                }

                string text = new string(Encoding.GetEncoding(detEncoding).GetChars(data));

                // close frmViewText if exists
                Application.OpenForms["frmViewText"]?.Close();
                // Show form
                Form frmViewText = new frmViewText(
                    resource.FileName,
                    text,
                    Encoding.GetEncodings().Where(en => en.CodePage == detEncoding).First().DisplayName
                );
                frmViewText.Show();
            }
            else if (resource.FileType == "Image")
            {
                byte[] data = KFN.GetDataFromResource(resource);

                // close frmViewImage if exists
                Application.OpenForms["frmViewImage"]?.Close();
                // Show Form
                Form frmViewImage = new frmViewImage(resource.FileName, Path.GetDirectoryName(KFN.FullFileName) , data);
                frmViewImage.Show();
            }
        }


        private void mnuExportItem_Click(object sender, EventArgs e)
        {
            var MI = sender as MenuItem;
            if (MI != null && MI.Tag != null)
            {                
                KFN.ResourceFile resource = (KFN.ResourceFile)MI.Tag;

                FolderBrowserDialog.SelectedPath = new FileInfo(KFN.FullFileName).DirectoryName;
                if (FolderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    string exportFolder = FolderBrowserDialog.SelectedPath;
                    try
                    {
                        System.Security.AccessControl.DirectorySecurity ds = Directory.GetAccessControl(exportFolder);
                    }
                    catch (UnauthorizedAccessException error)
                    {
                        MessageBox.Show(error.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    ExportResourceToFile(resource, exportFolder);                    
                    MessageBox.Show(string.Format("File \n{0} \nexported successfully to \n{1}", resource.FileName, exportFolder), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void ExportResourceToFile(KFN.ResourceFile resource, string folder)
        {
            byte[] data = KFN.GetDataFromResource(resource);

            using (FileStream fs = new FileStream(folder + "\\" + resource.FileName, FileMode.Create, FileAccess.Write))
            {
                fs.Write(data, 0, data.Length);
            }
        }


        private void lvResources_DoubleClick(object sender, EventArgs e)
        {            
            ListViewItem selectedItem = lvResources.SelectedItems[0];
            if (selectedItem != null)
            {
                try
                {
                    KFN.ResourceFile resource = (KFN.ResourceFile)selectedItem.Tag;
                    DisplayResource(resource);
                }
                catch (Exception ex) 
                {
                    MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                selectedItem.Checked = !selectedItem.Checked;

            }            
        }


        #endregion context menus


        private void btnViewConfig_Click(object sender, EventArgs e)
        {
            // Affiche le formulaire frmKfnView 
            Application.OpenForms["frmSongINI"]?.Close();

            try
            {
                Form frmSongINI = new frmSongINI(KFN);
                frmSongINI.Show();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        #region form load close

        private void frmKfnView_Load(object sender, EventArgs e)
        {
            // Set window location and size
            #region window size & location
            // If window is maximized
            if (Properties.Settings.Default.frmKfnViewMaximized)
            {
                Location = Properties.Settings.Default.frmKfnViewLocation;
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                Location = Properties.Settings.Default.frmKfnViewLocation;
                // Verify if this windows is visible in extended screens
                Rectangle rect = new Rectangle(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);
                foreach (Screen screen in Screen.AllScreens)
                    rect = Rectangle.Union(rect, screen.Bounds);

                if (Location.X > rect.Width)
                    Location = new Point(0, Location.Y);
                if (Location.Y > rect.Height)
                    Location = new Point(Location.X, 0);

                Size = Properties.Settings.Default.frmKfnViewSize;
            }
            #endregion
        }

        private void frmKfnView_FormClosing(object sender, FormClosingEventArgs e)
        {
            // enregistre la taille et la position de la forme
            // Copy window location to app settings                
            if (WindowState != FormWindowState.Minimized)
            {
                if (WindowState == FormWindowState.Maximized)
                {
                    Properties.Settings.Default.frmKfnViewLocation = RestoreBounds.Location;
                    Properties.Settings.Default.frmKfnViewMaximized = true;

                }
                else if (WindowState == FormWindowState.Normal)
                {
                    Karaboss.Properties.Settings.Default.frmKfnViewLocation = Location;
                    Properties.Settings.Default.frmKfnViewSize = Size;
                    Properties.Settings.Default.frmKfnViewMaximized = false;
                }               

                // Save settings
                Properties.Settings.Default.Save();
            }

            #region close windows

            Application.OpenForms["frmViewText"]?.Close();
            Application.OpenForms["frmViewImage"]?.Close();

            #endregion close windows

            Dispose();

        }


        private void frmKfnView_Resize(object sender, EventArgs e)
        {
            pnlLeft.Top = pnlTop.Top + pnlTop.Height;
            pnlRight.Top = pnlLeft.Top;

            pnlLeft.Left = 0;

            pnlLeft.Width = 3 * ClientSize.Width / 7;
            pnlRight.Left = pnlLeft.Width;
            pnlRight.Width = 4 * ClientSize.Width / 7;

            pnlLeft.Height = this.ClientSize.Height - pnlTop.Height - 30;
            pnlRight.Height = pnlLeft.Height;

        }


        #endregion form load close

        
    }
}

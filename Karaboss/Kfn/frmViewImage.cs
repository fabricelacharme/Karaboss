using FlShell.Interop;
using Karaboss.Properties;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Karaboss.Kfn
{
    
    public partial class frmViewImage : Form
    {
        private string fPath = null;
        private string fName = null;
        byte[] fImageData = null;

        public frmViewImage(string name, string fpath, byte[] image)
        {
            InitializeComponent();

            fPath = fpath;
            fName = name;
            fImageData = image;

            Image img = LoadImage(image);

            Text = "Image: " + name + " (" + img.Width + "x" + img.Height + ")";

            imgElement.Width = img.Width;
            imgElement.Height = img.Height;
            imgElement.Image = img;
            
        }

        private static Image LoadImage(byte[] imageData)
        {
            using (var ms = new MemoryStream(imageData))
            {
                return Image.FromStream(ms);
            }
            
        }

        private void imgElement_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && imgElement.Image != null)
            {
                ContextMenuStrip ImgContextMenu = new ContextMenuStrip();
                ImgContextMenu.Items.Clear();

                // Menu delete                
                ToolStripMenuItem menuSavePicture = new ToolStripMenuItem("Save picture");
                ImgContextMenu.Items.Add(menuSavePicture);

                menuSavePicture.Click += new System.EventHandler(MenuSavePicture_Click);

                // Display menu
                ImgContextMenu.Show(imgElement, imgElement.PointToClient(Cursor.Position));
            }
        }

        private void MenuSavePicture_Click(object sender, EventArgs e)
        {
            string InitialDir = Path.Combine(fPath, fName);

            FolderBrowserDialog.SelectedPath = new FileInfo(InitialDir).DirectoryName;
            if (FolderBrowserDialog.ShowDialog() != DialogResult.OK)  return;

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

            string FullName = Path.Combine(exportFolder, fName);

            try
            {
                using (FileStream fs = new FileStream(FullName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(fImageData, 0, fImageData.Length);
                }
                MessageBox.Show( string.Format("File \n{0} \nexported successfully to \n{1}", fName, exportFolder), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }            
        }
    }
}

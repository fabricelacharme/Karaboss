using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Karaboss.Kfn
{
    public partial class frmKfnSongINI : Form
    {
        private KFN KFN;
        private SongINI sINI;


        private Brush ColumnHeaderBrush = new SolidBrush(Color.FromArgb(51, 51, 51));

        public frmKfnSongINI(KFN kFN)
        {
            InitializeComponent();
            KFN = kFN;
            
            // Init controls
            InitControls();                                              

            // Parse Song.ini
            this.ParseINI(KFN);
        }

        private void InitControls()
        {          

            int W_Name = 90; int W_ContentID = 90; int W_ContentType = 200;

            // Affichage mode détails
            lvBlocks.View = View.Details;
            // Autoriser l'édition in place
            lvBlocks.LabelEdit = false;
            // keep selection visible
            lvBlocks.HideSelection = false;

            lvBlocks.FullRowSelect = true;

            lvBlocks.Clear();
            lvBlocks.Refresh();

            #region header
            //Headers listview     
            ColumnHeader columnHeader0 = new ColumnHeader()
            {
                Text = "Name",
                Width = W_Name,                
            };

            ColumnHeader columnHeader1 = new ColumnHeader()
            {
                Text = "Content ID",
                Width = W_ContentID,
            };

            ColumnHeader columnHeader2 = new ColumnHeader()
            {
                Text = "Content Type",
                Width = W_ContentType,
            };


            // Add the column headers to myListView.
            lvBlocks.Columns.AddRange(new ColumnHeader[]
                {columnHeader0, columnHeader1, columnHeader2 });

            #endregion

            // NO Sorting
            lvBlocks.Sorting = SortOrder.None;
            lvBlocks.AllowDrop = false;

            txtBlockContent.Width = this.ClientSize.Width - txtBlockContent.Left;
            txtBlockContent.Height = ClientSize.Height - txtBlockContent.Top;

            lvBlocks.Height = txtBlockContent.Height;
        }

       
        private void ParseINI(KFN KFN)
        {
            KFN.ResourceFile resource = KFN.Resources.Where(r => r.FileName == "Song.ini").First();
            byte[] data = KFN.GetDataFromResource(resource);
            string iniText = new string(Encoding.UTF8.GetChars(data));

            sINI = new SongINI(iniText);

            //iniBlocksView.ItemsSource = sINI.Blocks;
            //this.AutoSizeColumns(iniBlocksView.View as GridView);

            string[] saLvwItem = new string[3];

            foreach (var kv in sINI.Blocks)
            {
                saLvwItem[0] = kv.Name;
                saLvwItem[1] = kv.Id;
                saLvwItem[2] = kv.Type;
                //string content = kv.Content;

                // Add rows to lvProperties
                ListViewItem myListViewItem = new ListViewItem(saLvwItem);
                lvBlocks.Items.Add(myListViewItem);
            }
        }

        


        #region form load close
        private void frmKfnSongINI_Load(object sender, EventArgs e)
        {

        }

        private void frmKfnSongINI_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void frmKfnSongINI_Resize(object sender, EventArgs e)
        {
            txtBlockContent.Width = this.ClientSize.Width - txtBlockContent.Left;
            txtBlockContent.Height = ClientSize.Height - txtBlockContent.Top;

            lvBlocks.Height = txtBlockContent.Height;

        }

        #endregion form load close


        private void lvBlocks_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {

                ListViewItem focusedItem = lvBlocks.FocusedItem;
                if (focusedItem != null)
                {
                    int idx = focusedItem.Index;
                    if (idx >= 0 && idx < lvBlocks.Items.Count)
                    {

                        SongINI.BlockInfo block = sINI.Blocks[idx];

                        if (block != null)
                        {
                            txtBlockContent.Text = block.Content.Replace("\n", Environment.NewLine);
                        }
                    }
                }

            }
        }

      
    }
}

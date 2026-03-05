using Karaboss.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TagLib;

namespace Karaboss.Kfn
{
    public partial class frmSongINI : Form
    {
        private KFN KFN;
        private SongINI sINI;

        public frmSongINI(KFN kFN)
        {
            InitializeComponent();
            KFN = kFN;


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

                       
            //iniBlocksView.View = blocksGrid;

            this.ParseINI(KFN);

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

                        //SongINI.BlockInfo.Content;
                        //SongINI.BlockInfo block = SongINI.BlockInfo[idx];

                        //SongINI.BlockInfo block = lvBlocks.Items[idx] as SongINI.BlockInfo;


                        //txtBlockContent.Text = block.Content;
                    }
                }

            }
        }
    }
}

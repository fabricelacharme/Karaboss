namespace VBarControl.SideBarControl
{
    partial class SideBarControl
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SideBarControl));
            this.btnPlaylists = new VBarControl.NavButton.NavButton();
            this.btnFiles = new VBarControl.NavButton.NavButton();
            this.btnSearch = new VBarControl.NavButton.NavButton();
            this.btnPlay = new VBarControl.NavButton.NavButton();
            this.btnEdit = new VBarControl.NavButton.NavButton();
            this.btnHome = new VBarControl.NavButton.NavButton();
            this.btnConnected = new VBarControl.NavButton.NavButton();
            this.btnPianoTraining = new VBarControl.NavButton.NavButton();
            this.btnGuitarTraining = new VBarControl.NavButton.NavButton();
            this.SuspendLayout();
            // 
            // btnPlaylists
            // 
            resources.ApplyResources(this.btnPlaylists, "btnPlaylists");
            this.btnPlaylists.defaultBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(29)))), ((int)(((byte)(29)))));
            this.btnPlaylists.HoverColor = System.Drawing.Color.Gray;
            this.btnPlaylists.Image = ((System.Drawing.Image)(resources.GetObject("btnPlaylists.Image")));
            this.btnPlaylists.Name = "btnPlaylists";
            this.btnPlaylists.Selectable = true;
            this.btnPlaylists.Selected = false;
            this.btnPlaylists.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.btnPlaylists.Click += new System.EventHandler(this.btnPlaylists_Click);
            // 
            // btnFiles
            // 
            resources.ApplyResources(this.btnFiles, "btnFiles");
            this.btnFiles.defaultBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(29)))), ((int)(((byte)(29)))));
            this.btnFiles.HoverColor = System.Drawing.Color.Gray;
            this.btnFiles.Image = ((System.Drawing.Image)(resources.GetObject("btnFiles.Image")));
            this.btnFiles.Name = "btnFiles";
            this.btnFiles.Selectable = true;
            this.btnFiles.Selected = false;
            this.btnFiles.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.btnFiles.Click += new System.EventHandler(this.btnFiles_Click);
            // 
            // btnSearch
            // 
            resources.ApplyResources(this.btnSearch, "btnSearch");
            this.btnSearch.defaultBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(29)))), ((int)(((byte)(29)))));
            this.btnSearch.HoverColor = System.Drawing.Color.Gray;
            this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Selectable = true;
            this.btnSearch.Selected = false;
            this.btnSearch.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnPlay
            // 
            resources.ApplyResources(this.btnPlay, "btnPlay");
            this.btnPlay.defaultBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(29)))), ((int)(((byte)(29)))));
            this.btnPlay.HoverColor = System.Drawing.Color.Gray;
            this.btnPlay.Image = ((System.Drawing.Image)(resources.GetObject("btnPlay.Image")));
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Selectable = false;
            this.btnPlay.Selected = false;
            this.btnPlay.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // btnEdit
            // 
            resources.ApplyResources(this.btnEdit, "btnEdit");
            this.btnEdit.defaultBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(29)))), ((int)(((byte)(29)))));
            this.btnEdit.HoverColor = System.Drawing.Color.Gray;
            this.btnEdit.Image = ((System.Drawing.Image)(resources.GetObject("btnEdit.Image")));
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Selectable = false;
            this.btnEdit.Selected = false;
            this.btnEdit.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnHome
            // 
            resources.ApplyResources(this.btnHome, "btnHome");
            this.btnHome.defaultBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(29)))), ((int)(((byte)(29)))));
            this.btnHome.HoverColor = System.Drawing.Color.Gray;
            this.btnHome.Image = ((System.Drawing.Image)(resources.GetObject("btnHome.Image")));
            this.btnHome.Name = "btnHome";
            this.btnHome.Selectable = true;
            this.btnHome.Selected = false;
            this.btnHome.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click);
            // 
            // btnConnected
            // 
            resources.ApplyResources(this.btnConnected, "btnConnected");
            this.btnConnected.defaultBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(29)))), ((int)(((byte)(29)))));
            this.btnConnected.HoverColor = System.Drawing.Color.Gray;
            this.btnConnected.Image = ((System.Drawing.Image)(resources.GetObject("btnConnected.Image")));
            this.btnConnected.Name = "btnConnected";
            this.btnConnected.Selectable = true;
            this.btnConnected.Selected = false;
            this.btnConnected.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.btnConnected.Click += new System.EventHandler(this.btnConnected_Click);
            // 
            // btnPianoTraining
            // 
            resources.ApplyResources(this.btnPianoTraining, "btnPianoTraining");
            this.btnPianoTraining.defaultBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(29)))), ((int)(((byte)(29)))));
            this.btnPianoTraining.HoverColor = System.Drawing.Color.Gray;
            this.btnPianoTraining.Image = ((System.Drawing.Image)(resources.GetObject("btnPianoTraining.Image")));
            this.btnPianoTraining.Name = "btnPianoTraining";
            this.btnPianoTraining.Selectable = false;
            this.btnPianoTraining.Selected = false;
            this.btnPianoTraining.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.btnPianoTraining.Click += new System.EventHandler(this.btnPianoTraining_Click);
            // 
            // btnGuitarTraining
            // 
            resources.ApplyResources(this.btnGuitarTraining, "btnGuitarTraining");
            this.btnGuitarTraining.defaultBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(29)))), ((int)(((byte)(29)))));
            this.btnGuitarTraining.HoverColor = System.Drawing.Color.Gray;
            this.btnGuitarTraining.Image = ((System.Drawing.Image)(resources.GetObject("btnGuitarTraining.Image")));
            this.btnGuitarTraining.Name = "btnGuitarTraining";
            this.btnGuitarTraining.Selectable = false;
            this.btnGuitarTraining.Selected = false;
            this.btnGuitarTraining.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.btnGuitarTraining.Click += new System.EventHandler(this.btnGuitarTraining_Click);
            // 
            // SideBarControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(29)))), ((int)(((byte)(29)))));
            this.Controls.Add(this.btnGuitarTraining);
            this.Controls.Add(this.btnPianoTraining);
            this.Controls.Add(this.btnConnected);
            this.Controls.Add(this.btnHome);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnPlay);
            this.Controls.Add(this.btnPlaylists);
            this.Controls.Add(this.btnFiles);
            this.Controls.Add(this.btnSearch);
            this.Name = "SideBarControl";
            this.ResumeLayout(false);

        }

        #endregion
        private NavButton.NavButton btnSearch;
        private NavButton.NavButton btnFiles;
        private NavButton.NavButton btnPlaylists;
        private NavButton.NavButton btnPlay;
        private NavButton.NavButton btnEdit;
        private NavButton.NavButton btnHome;
        private NavButton.NavButton btnConnected;
        private NavButton.NavButton btnPianoTraining;
        private NavButton.NavButton btnGuitarTraining;
    }
}

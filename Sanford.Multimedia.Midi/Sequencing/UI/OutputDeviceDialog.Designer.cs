namespace Sanford.Multimedia.Midi.UI
{
    partial class OutputDeviceDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OutputDeviceDialog));
            this.outputLabel = new System.Windows.Forms.Label();
            this.outputComboBox = new System.Windows.Forms.ComboBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.chkSavePreferences = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // outputLabel
            // 
            resources.ApplyResources(this.outputLabel, "outputLabel");
            this.outputLabel.Name = "outputLabel";
            // 
            // outputComboBox
            // 
            resources.ApplyResources(this.outputComboBox, "outputComboBox");
            this.outputComboBox.AccessibleRole = System.Windows.Forms.AccessibleRole.DropList;
            this.outputComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.outputComboBox.FormattingEnabled = true;
            this.outputComboBox.Name = "outputComboBox";
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // chkSavePreferences
            // 
            resources.ApplyResources(this.chkSavePreferences, "chkSavePreferences");
            this.chkSavePreferences.Name = "chkSavePreferences";
            this.chkSavePreferences.UseVisualStyleBackColor = true;
            this.chkSavePreferences.CheckedChanged += new System.EventHandler(this.chkSavePreferences_CheckedChanged);
            // 
            // OutputDeviceDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkSavePreferences);
            this.Controls.Add(this.outputLabel);
            this.Controls.Add(this.outputComboBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OutputDeviceDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label outputLabel;
        private System.Windows.Forms.ComboBox outputComboBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.CheckBox chkSavePreferences;
    }
}
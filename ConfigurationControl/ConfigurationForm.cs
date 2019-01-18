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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Slaks.Windows.Forms;

namespace Slaks.Windows.Forms.Configuration
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class ConfigurationForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel m_restoreApplyPanel;
		private System.Windows.Forms.Panel m_optionsPanel;
		private System.Windows.Forms.Panel m_okCancelPanel;
		private System.Windows.Forms.Panel m_mainPanel;
		private System.Windows.Forms.TreeView m_optionsTV;
		private System.Windows.Forms.GroupBox m_separatorGB;
		private System.Windows.Forms.Button m_okBtn;
		private System.Windows.Forms.Button m_cancelBtn;
		private System.Windows.Forms.Button m_restoreBtn;
		private System.Windows.Forms.Button m_applyBtn;
		private System.Windows.Forms.Panel m_headPanel;

		private System.Windows.Forms.Label m_configLabel;
		private System.Windows.Forms.GroupBox m_upperSeparatorGB;
		private System.ComponentModel.Container components = null;

		private enum Action
		{
			Apply =1,
			Restore = 2
		}

		public ConfigurationForm()
		{
			InitializeComponent();
			m_restoreBtn.Click += new EventHandler(OnRestoreButtonClicked);
			m_applyBtn.Click += new EventHandler(OnApplyButtonClicked);
			m_cancelBtn.Click += new EventHandler(OnCancelButtonClicked);
			m_okBtn.Click += new EventHandler(OnOkButtonClicked);
		}
		/// <summary>
		/// Additional event handler for apply button
		/// </summary>
		public event EventHandler ApplyBtn;
		/// <summary>
		/// Additional event handler for restore button
		/// </summary>
		public event EventHandler RestoreBtn;
		/// <summary>
		/// Additional event handler for ok button
		/// </summary>
		public event EventHandler OkBtn;
		/// <summary>
		/// Additional event handler for cancel button
		/// </summary>
		public event EventHandler CancelBtn;

		/// <summary>
		/// Add the next configuration tree node item
		/// </summary>
		/// <param name="configurationNode"></param>
		public void AddConfigItem(TreeNode configurationNode)
		{
			if (m_optionsTV.Nodes.Count == 0) 
			{
				m_optionsTV.Nodes.Add(configurationNode);
				m_optionsTV.SelectedNode = configurationNode;
				m_optionsTV.Select();
			}
			else m_optionsTV.Nodes.Add(configurationNode);
		}


		/// <summary>
		/// Remove the tree node item with specified config name
		/// </summary>
		/// <param name="configName"></param>
		public void RemoveConfigItem(string configName)
		{
			this.RemoveItem(m_optionsTV.Nodes, configName);
		}

		private void SelectConfig(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			if (e.Node is ConfigurationTreeNode)
			{
				//removes any controls
				if (m_mainPanel.Controls.Count > 0) m_mainPanel.Controls.RemoveAt(0);
				ConfigurationBaseControl control = ((ConfigurationTreeNode)e.Node).GetConfigurationControl();
				//add the current control to the main panel
				
				this.ResizeIfNeeded(control);
				m_mainPanel.Controls.Add(control);
				//set the name of the control
				this.SetLabel(((ConfigurationTreeNode)e.Node).GetConfigurationName());
			}
		}

		/// <summary>
		/// Resize the width of the form if needed
		/// </summary>
		/// <param name="control"></param>
		private void ResizeIfNeeded(ConfigurationBaseControl control)
		{
			int controlWidth = control.Width;
			int panelWidth = m_mainPanel.Width;
			int appWidth = this.Width;
            int diff = appWidth - panelWidth;

			if (controlWidth > panelWidth)
			{
				this.Width = controlWidth + diff;
				m_mainPanel.Width = controlWidth;
			}
		}

		/// <summary>
		/// When the control is active, set its name in the upper side of the form
		/// </summary>
		/// <param name="label"></param>
		private void SetLabel(string label)
		{
			this.m_configLabel.Text = label;
		}

		/// <summary>
		/// Default handler for apply,restor,ok,cancel buttons
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ClickHandler(object sender, System.EventArgs e)
		{
			if (sender == m_okBtn)
			{
				this.TreeWalker(m_optionsTV.Nodes,Action.Apply);
                this.Hide();
			}
			else if (sender == m_cancelBtn)
			{
				this.Hide();
			}
			else if (sender == m_restoreBtn)
			{
				this.TreeWalker(m_optionsTV.Nodes,Action.Restore);
			}
			else if (sender == m_applyBtn)
			{
				this.TreeWalker(m_optionsTV.Nodes,Action.Apply);
			}
		}

		/// <summary>
		/// When the Restor or Apply command are initiated, visit each tree node, get its related control and call the method (Restore, Apply)
		/// </summary>
		/// <param name="treeCollection"></param>
		/// <param name="action"></param>
		private void TreeWalker(TreeNodeCollection treeCollection,Action action)
		{
			if (treeCollection == null) return;

			foreach(TreeNode node in treeCollection)
			{
				if (node is ConfigurationTreeNode)
				{
					ConfigurationBaseControl configurationBaseControl = ((ConfigurationTreeNode) node).GetConfigurationControl();
					if (action == Action.Apply) configurationBaseControl.Apply();
					else if (action == Action.Restore) configurationBaseControl.Restore();
         
					this.TreeWalker(node.Nodes,action);
				}
			}
		}

		/// <summary>
		/// Remove tree node item with specified name
		/// </summary>
		/// <param name="treeCollection"></param>
		/// <param name="name"></param>
		private void RemoveItem(TreeNodeCollection treeCollection,string name)
		{
			if (treeCollection == null) return;
			foreach(TreeNode node in treeCollection)
			{
				if (string.Equals(node.Text, name)) 
				{
					m_optionsTV.Nodes.Remove(node);
					if (node is ConfigurationTreeNode)
					{
						ConfigurationBaseControl control = ((ConfigurationTreeNode)node).GetConfigurationControl();
						if (m_mainPanel.Controls.Contains((control))) m_mainPanel.Controls.RemoveAt(0);
					}
					break;
				}
				this.RemoveItem(node.Nodes,name);
			}
		}

		[Category("Button Clicks"),Description("Occurs when Apply button is clicked")]
		protected void OnApplyButtonClicked(object sender,EventArgs e)
		{
			if (ApplyBtn != null) ApplyBtn(sender,e);
		}
		[Category("Button Clicks"),Description("Occurs when Restore button is clicked")]
		protected void OnRestoreButtonClicked(object sender,EventArgs e)
		{
			if (RestoreBtn != null) RestoreBtn(sender,e);
		}
		[Category("Button Clicks"),Description("Occurs when Ok button is clicked")]
		protected void OnOkButtonClicked(object sender,EventArgs e)
		{
			if (OkBtn != null) OkBtn(sender,e);
		}
		[Category("Button Clicks"),Description("Occurs when Cancel button is clicked")]
		protected void OnCancelButtonClicked(object sender,EventArgs e)
		{
			if (CancelBtn != null) CancelBtn(sender,e);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigurationForm));
            this.m_restoreApplyPanel = new System.Windows.Forms.Panel();
            this.m_applyBtn = new System.Windows.Forms.Button();
            this.m_restoreBtn = new System.Windows.Forms.Button();
            this.m_optionsPanel = new System.Windows.Forms.Panel();
            this.m_optionsTV = new System.Windows.Forms.TreeView();
            this.m_okCancelPanel = new System.Windows.Forms.Panel();
            this.m_cancelBtn = new System.Windows.Forms.Button();
            this.m_okBtn = new System.Windows.Forms.Button();
            this.m_mainPanel = new System.Windows.Forms.Panel();
            this.m_separatorGB = new System.Windows.Forms.GroupBox();
            this.m_headPanel = new System.Windows.Forms.Panel();
            this.m_configLabel = new System.Windows.Forms.Label();
            this.m_upperSeparatorGB = new System.Windows.Forms.GroupBox();
            this.m_restoreApplyPanel.SuspendLayout();
            this.m_optionsPanel.SuspendLayout();
            this.m_okCancelPanel.SuspendLayout();
            this.m_headPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_restoreApplyPanel
            // 
            this.m_restoreApplyPanel.Controls.Add(this.m_applyBtn);
            this.m_restoreApplyPanel.Controls.Add(this.m_restoreBtn);
            resources.ApplyResources(this.m_restoreApplyPanel, "m_restoreApplyPanel");
            this.m_restoreApplyPanel.Name = "m_restoreApplyPanel";
            // 
            // m_applyBtn
            // 
            resources.ApplyResources(this.m_applyBtn, "m_applyBtn");
            this.m_applyBtn.Name = "m_applyBtn";
            this.m_applyBtn.Click += new System.EventHandler(this.ClickHandler);
            // 
            // m_restoreBtn
            // 
            resources.ApplyResources(this.m_restoreBtn, "m_restoreBtn");
            this.m_restoreBtn.Name = "m_restoreBtn";
            this.m_restoreBtn.Click += new System.EventHandler(this.ClickHandler);
            // 
            // m_optionsPanel
            // 
            this.m_optionsPanel.Controls.Add(this.m_optionsTV);
            resources.ApplyResources(this.m_optionsPanel, "m_optionsPanel");
            this.m_optionsPanel.Name = "m_optionsPanel";
            // 
            // m_optionsTV
            // 
            resources.ApplyResources(this.m_optionsTV, "m_optionsTV");
            this.m_optionsTV.Name = "m_optionsTV";
            this.m_optionsTV.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.SelectConfig);
            // 
            // m_okCancelPanel
            // 
            this.m_okCancelPanel.Controls.Add(this.m_cancelBtn);
            this.m_okCancelPanel.Controls.Add(this.m_okBtn);
            resources.ApplyResources(this.m_okCancelPanel, "m_okCancelPanel");
            this.m_okCancelPanel.Name = "m_okCancelPanel";
            // 
            // m_cancelBtn
            // 
            resources.ApplyResources(this.m_cancelBtn, "m_cancelBtn");
            this.m_cancelBtn.Name = "m_cancelBtn";
            this.m_cancelBtn.Click += new System.EventHandler(this.ClickHandler);
            // 
            // m_okBtn
            // 
            resources.ApplyResources(this.m_okBtn, "m_okBtn");
            this.m_okBtn.Name = "m_okBtn";
            this.m_okBtn.Click += new System.EventHandler(this.ClickHandler);
            // 
            // m_mainPanel
            // 
            resources.ApplyResources(this.m_mainPanel, "m_mainPanel");
            this.m_mainPanel.Name = "m_mainPanel";
            // 
            // m_separatorGB
            // 
            resources.ApplyResources(this.m_separatorGB, "m_separatorGB");
            this.m_separatorGB.Name = "m_separatorGB";
            this.m_separatorGB.TabStop = false;
            // 
            // m_headPanel
            // 
            this.m_headPanel.Controls.Add(this.m_configLabel);
            resources.ApplyResources(this.m_headPanel, "m_headPanel");
            this.m_headPanel.Name = "m_headPanel";
            // 
            // m_configLabel
            // 
            resources.ApplyResources(this.m_configLabel, "m_configLabel");
            this.m_configLabel.Name = "m_configLabel";
            // 
            // m_upperSeparatorGB
            // 
            resources.ApplyResources(this.m_upperSeparatorGB, "m_upperSeparatorGB");
            this.m_upperSeparatorGB.Name = "m_upperSeparatorGB";
            this.m_upperSeparatorGB.TabStop = false;
            // 
            // ConfigurationForm
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.m_upperSeparatorGB);
            this.Controls.Add(this.m_mainPanel);
            this.Controls.Add(this.m_headPanel);
            this.Controls.Add(this.m_restoreApplyPanel);
            this.Controls.Add(this.m_optionsPanel);
            this.Controls.Add(this.m_separatorGB);
            this.Controls.Add(this.m_okCancelPanel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigurationForm";
            this.m_restoreApplyPanel.ResumeLayout(false);
            this.m_optionsPanel.ResumeLayout(false);
            this.m_okCancelPanel.ResumeLayout(false);
            this.m_headPanel.ResumeLayout(false);
            this.ResumeLayout(false);

		}

	}
}

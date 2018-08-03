/*
 *      This file is part of Orion2, a MapleStory2 Packaging Library Project.
 *      Copyright (C) 2018 Eric Smith <notericsoft@gmail.com>
 * 
 *      This program is free software: you can redistribute it and/or modify
 *      it under the terms of the GNU General Public License as published by
 *      the Free Software Foundation, either version 3 of the License, or
 *      (at your option) any later version.
 * 
 *      This program is distributed in the hope that it will be useful,
 *      but WITHOUT ANY WARRANTY; without even the implied warranty of
 *      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *      GNU General Public License for more details.
 * 
 *      You should have received a copy of the GNU General Public License
 */

using System.Drawing;
using System.Windows.Forms;

namespace Orion.Window
{
    partial class MainWindow
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
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private class MenuRenderer : ToolStripProfessionalRenderer
        {
            public MenuRenderer() : base(new MenuColors()) { }
        }

        private class MenuColors : ProfessionalColorTable
        {
            private readonly Color SYS_COLOR = Color.FromArgb(((int)(((byte)(181)))), ((int)(((byte)(215)))), ((int)(((byte)(243)))));

            /* Top gradient of selected upper menu items */
            public override Color MenuItemSelectedGradientBegin
            {
                get { return SYS_COLOR; }
            }

            /* Bottom gradient of selected upper menu items */
            public override Color MenuItemSelectedGradientEnd
            {
                get { return SYS_COLOR; }
            }

            /* Top gradient of pressed upper menu items */
            public override Color MenuItemPressedGradientBegin
            {
                get { return SYS_COLOR; }
            }

            /* Bottom gradient of pressed upper menu items */
            public override Color MenuItemPressedGradientEnd
            {
                get { return SYS_COLOR; }
            }

            /* Global menu item border coloring */
            public override Color MenuItemBorder
            {
                get { return SYS_COLOR; }
            }

            /* Color of sub-menu items */
            public override Color MenuItemSelected
            {
                get { return SYS_COLOR; }
            }
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pMenuStrip = new System.Windows.Forms.MenuStrip();
            this.pFileMenuStripItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pOpenMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pSaveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pReloadMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pUnloadMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allNodesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collapseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pTreeView = new System.Windows.Forms.TreeView();
            this.pEntryValue = new System.Windows.Forms.TextBox();
            this.pTextData = new System.Windows.Forms.TextBox();
            this.pImageData = new System.Windows.Forms.PictureBox();
            this.pImagePanel = new System.Windows.Forms.Panel();
            this.pUpdateDataBtn = new System.Windows.Forms.Button();
            this.pChangeImageBtn = new System.Windows.Forms.Button();
            this.pSaveWorkerThread = new System.ComponentModel.BackgroundWorker();
            this.pMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pImageData)).BeginInit();
            this.pImagePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // pMenuStrip
            // 
            this.pMenuStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.pMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pFileMenuStripItem,
            this.editToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.pMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.pMenuStrip.Name = "pMenuStrip";
            this.pMenuStrip.Size = new System.Drawing.Size(1045, 24);
            this.pMenuStrip.TabIndex = 0;
            this.pMenuStrip.Text = "menuStrip1";
            // 
            // pFileMenuStripItem
            // 
            this.pFileMenuStripItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pOpenMenuItem,
            this.pSaveMenuItem,
            this.pReloadMenuItem,
            this.pUnloadMenuItem,
            this.exitToolStripMenuItem});
            this.pFileMenuStripItem.ForeColor = System.Drawing.Color.Black;
            this.pFileMenuStripItem.Name = "pFileMenuStripItem";
            this.pFileMenuStripItem.Size = new System.Drawing.Size(37, 20);
            this.pFileMenuStripItem.Text = "File";
            // 
            // pOpenMenuItem
            // 
            this.pOpenMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.pOpenMenuItem.ForeColor = System.Drawing.Color.Black;
            this.pOpenMenuItem.Name = "pOpenMenuItem";
            this.pOpenMenuItem.ShortcutKeyDisplayString = "Ctrl+O";
            this.pOpenMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.pOpenMenuItem.Size = new System.Drawing.Size(154, 22);
            this.pOpenMenuItem.Text = "Open";
            this.pOpenMenuItem.Click += new System.EventHandler(this.OnLoadFile);
            // 
            // pSaveMenuItem
            // 
            this.pSaveMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.pSaveMenuItem.ForeColor = System.Drawing.Color.Black;
            this.pSaveMenuItem.Name = "pSaveMenuItem";
            this.pSaveMenuItem.ShortcutKeyDisplayString = "Ctrl+S";
            this.pSaveMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.pSaveMenuItem.Size = new System.Drawing.Size(154, 22);
            this.pSaveMenuItem.Text = "Save";
            this.pSaveMenuItem.Click += new System.EventHandler(this.OnSaveFile);
            // 
            // pReloadMenuItem
            // 
            this.pReloadMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.pReloadMenuItem.ForeColor = System.Drawing.Color.Black;
            this.pReloadMenuItem.Name = "pReloadMenuItem";
            this.pReloadMenuItem.ShortcutKeyDisplayString = "Ctrl+R";
            this.pReloadMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.pReloadMenuItem.Size = new System.Drawing.Size(154, 22);
            this.pReloadMenuItem.Text = "Reload";
            this.pReloadMenuItem.Click += new System.EventHandler(this.OnReloadFile);
            // 
            // pUnloadMenuItem
            // 
            this.pUnloadMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.pUnloadMenuItem.ForeColor = System.Drawing.Color.Black;
            this.pUnloadMenuItem.Name = "pUnloadMenuItem";
            this.pUnloadMenuItem.ShortcutKeyDisplayString = "Ctrl+U";
            this.pUnloadMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
            this.pUnloadMenuItem.Size = new System.Drawing.Size(154, 22);
            this.pUnloadMenuItem.Text = "Unload";
            this.pUnloadMenuItem.Click += new System.EventHandler(this.OnUnloadFile);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.exitToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.OnExit);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.removeToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.allNodesToolStripMenuItem});
            this.editToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.addToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.addToolStripMenuItem.Text = "Add";
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.removeToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.OnRemoveFile);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.copyToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.OnCopyNode);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.pasteToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.OnPasteNode);
            // 
            // allNodesToolStripMenuItem
            // 
            this.allNodesToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.allNodesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.expandToolStripMenuItem,
            this.collapseToolStripMenuItem});
            this.allNodesToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.allNodesToolStripMenuItem.Name = "allNodesToolStripMenuItem";
            this.allNodesToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.allNodesToolStripMenuItem.Text = "All Nodes";
            // 
            // expandToolStripMenuItem
            // 
            this.expandToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.expandToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.expandToolStripMenuItem.Name = "expandToolStripMenuItem";
            this.expandToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.expandToolStripMenuItem.Text = "Expand";
            this.expandToolStripMenuItem.Click += new System.EventHandler(this.OnExpandNodes);
            // 
            // collapseToolStripMenuItem
            // 
            this.collapseToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.collapseToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.collapseToolStripMenuItem.Name = "collapseToolStripMenuItem";
            this.collapseToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.collapseToolStripMenuItem.Text = "Collapse";
            this.collapseToolStripMenuItem.Click += new System.EventHandler(this.OnCollapseNodes);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportToolStripMenuItem,
            this.searchToolStripMenuItem});
            this.toolsToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.exportToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.OnExport);
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.searchToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+F";
            this.searchToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.searchToolStripMenuItem.Text = "Search";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.aboutToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.OnAbout);
            // 
            // pTreeView
            // 
            this.pTreeView.BackColor = System.Drawing.Color.White;
            this.pTreeView.ForeColor = System.Drawing.Color.Black;
            this.pTreeView.Location = new System.Drawing.Point(0, 24);
            this.pTreeView.Name = "pTreeView";
            this.pTreeView.Size = new System.Drawing.Size(447, 620);
            this.pTreeView.TabIndex = 1;
            this.pTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.OnSelectNode);
            this.pTreeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.OnDoubleClickNode);
            // 
            // pEntryValue
            // 
            this.pEntryValue.BackColor = System.Drawing.Color.White;
            this.pEntryValue.ForeColor = System.Drawing.Color.Black;
            this.pEntryValue.Location = new System.Drawing.Point(453, 27);
            this.pEntryValue.Name = "pEntryValue";
            this.pEntryValue.Size = new System.Drawing.Size(284, 20);
            this.pEntryValue.TabIndex = 2;
            this.pEntryValue.Text = "Empty";
            this.pEntryValue.WordWrap = false;
            // 
            // pTextData
            // 
            this.pTextData.BackColor = System.Drawing.Color.White;
            this.pTextData.ForeColor = System.Drawing.Color.Black;
            this.pTextData.Location = new System.Drawing.Point(453, 53);
            this.pTextData.Multiline = true;
            this.pTextData.Name = "pTextData";
            this.pTextData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.pTextData.Size = new System.Drawing.Size(580, 579);
            this.pTextData.TabIndex = 3;
            this.pTextData.Visible = false;
            // 
            // pImageData
            // 
            this.pImageData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pImageData.Location = new System.Drawing.Point(0, 0);
            this.pImageData.Name = "pImageData";
            this.pImageData.Size = new System.Drawing.Size(580, 579);
            this.pImageData.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pImageData.TabIndex = 4;
            this.pImageData.TabStop = false;
            this.pImageData.Visible = false;
            // 
            // pImagePanel
            // 
            this.pImagePanel.Controls.Add(this.pImageData);
            this.pImagePanel.Location = new System.Drawing.Point(453, 53);
            this.pImagePanel.Name = "pImagePanel";
            this.pImagePanel.Size = new System.Drawing.Size(580, 579);
            this.pImagePanel.TabIndex = 5;
            // 
            // pUpdateDataBtn
            // 
            this.pUpdateDataBtn.BackColor = System.Drawing.Color.White;
            this.pUpdateDataBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pUpdateDataBtn.ForeColor = System.Drawing.Color.Black;
            this.pUpdateDataBtn.Location = new System.Drawing.Point(743, 26);
            this.pUpdateDataBtn.Name = "pUpdateDataBtn";
            this.pUpdateDataBtn.Size = new System.Drawing.Size(99, 25);
            this.pUpdateDataBtn.TabIndex = 6;
            this.pUpdateDataBtn.Text = "Save Changes";
            this.pUpdateDataBtn.UseVisualStyleBackColor = false;
            this.pUpdateDataBtn.Visible = false;
            this.pUpdateDataBtn.Click += new System.EventHandler(this.OnSaveChanges);
            // 
            // pChangeImageBtn
            // 
            this.pChangeImageBtn.BackColor = System.Drawing.Color.White;
            this.pChangeImageBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pChangeImageBtn.ForeColor = System.Drawing.Color.Black;
            this.pChangeImageBtn.Location = new System.Drawing.Point(743, 26);
            this.pChangeImageBtn.Name = "pChangeImageBtn";
            this.pChangeImageBtn.Size = new System.Drawing.Size(99, 25);
            this.pChangeImageBtn.TabIndex = 7;
            this.pChangeImageBtn.Text = "Change Image";
            this.pChangeImageBtn.UseVisualStyleBackColor = false;
            this.pChangeImageBtn.Visible = false;
            this.pChangeImageBtn.Click += new System.EventHandler(this.OnChangeImage);
            // 
            // pSaveWorkerThread
            // 
            this.pSaveWorkerThread.WorkerReportsProgress = true;
            this.pSaveWorkerThread.DoWork += new System.ComponentModel.DoWorkEventHandler(this.OnSaveBegin);
            this.pSaveWorkerThread.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.OnSaveProgress);
            this.pSaveWorkerThread.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.OnSaveComplete);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.ClientSize = new System.Drawing.Size(1045, 644);
            this.Controls.Add(this.pChangeImageBtn);
            this.Controls.Add(this.pUpdateDataBtn);
            this.Controls.Add(this.pImagePanel);
            this.Controls.Add(this.pTextData);
            this.Controls.Add(this.pEntryValue);
            this.Controls.Add(this.pTreeView);
            this.Controls.Add(this.pMenuStrip);
            this.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.Name = "MainWindow";
            this.Text = "Orion2 Repacker";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnWindowClosing);
            this.SizeChanged += new System.EventHandler(this.OnChangeWindowSize);
            this.pMenuStrip.ResumeLayout(false);
            this.pMenuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pImageData)).EndInit();
            this.pImagePanel.ResumeLayout(false);
            this.pImagePanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip pMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem pFileMenuStripItem;
        private System.Windows.Forms.ToolStripMenuItem pOpenMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pSaveMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pReloadMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pUnloadMenuItem;
        private System.Windows.Forms.TreeView pTreeView;
        private System.Windows.Forms.TextBox pEntryValue;
        private System.Windows.Forms.TextBox pTextData;
        private System.Windows.Forms.PictureBox pImageData;
        private System.Windows.Forms.Panel pImagePanel;
        private System.Drawing.Size pPrevSize;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem addToolStripMenuItem;
        private ToolStripMenuItem removeToolStripMenuItem;
        private ToolStripMenuItem copyToolStripMenuItem;
        private ToolStripMenuItem pasteToolStripMenuItem;
        private ToolStripMenuItem allNodesToolStripMenuItem;
        private ToolStripMenuItem expandToolStripMenuItem;
        private ToolStripMenuItem collapseToolStripMenuItem;
        private ToolStripMenuItem exportToolStripMenuItem;
        private ToolStripMenuItem searchToolStripMenuItem;
        private Button pUpdateDataBtn;
        private Button pChangeImageBtn;
        private System.ComponentModel.BackgroundWorker pSaveWorkerThread;
    }
}


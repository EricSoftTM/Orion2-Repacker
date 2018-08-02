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

namespace Orion.Window
{
    partial class ProgressWindow
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pSaveInfo = new System.Windows.Forms.Label();
            this.pProgressBar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // pSaveInfo
            // 
            this.pSaveInfo.Location = new System.Drawing.Point(12, 9);
            this.pSaveInfo.Name = "pSaveInfo";
            this.pSaveInfo.Size = new System.Drawing.Size(299, 22);
            this.pSaveInfo.TabIndex = 0;
            this.pSaveInfo.Text = "Saving ...";
            this.pSaveInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pProgressBar
            // 
            this.pProgressBar.Location = new System.Drawing.Point(12, 34);
            this.pProgressBar.Name = "pProgressBar";
            this.pProgressBar.Size = new System.Drawing.Size(299, 33);
            this.pProgressBar.Step = 1;
            this.pProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pProgressBar.TabIndex = 1;
            // 
            // ProgressWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(323, 79);
            this.ControlBox = false;
            this.Controls.Add(this.pProgressBar);
            this.Controls.Add(this.pSaveInfo);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Saving...";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label pSaveInfo;
        private System.Windows.Forms.ProgressBar pProgressBar;
    }
}
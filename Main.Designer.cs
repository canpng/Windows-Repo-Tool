﻿namespace WindowsRepoTool
{
    partial class Main
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
            this.components = new System.ComponentModel.Container();
            this.addRepoBtn = new System.Windows.Forms.Button();
            this.addRepoBox = new System.Windows.Forms.TextBox();
            this.repoListBox = new System.Windows.Forms.ListBox();
            this.clearAllReposBtn = new System.Windows.Forms.Button();
            this.clearSelectedRepoBtn = new System.Windows.Forms.Button();
            this.openSelectedRepoBtn = new System.Windows.Forms.Button();
            this.packagesListBox = new System.Windows.Forms.ListBox();
            this.downloadSelectedPackageBtn = new System.Windows.Forms.Button();
            this.searchBox = new System.Windows.Forms.TextBox();
            this.detailsBox = new System.Windows.Forms.TextBox();
            this.searchButton = new System.Windows.Forms.Button();
            this.darkModeBtn = new System.Windows.Forms.CheckBox();
            this.languageComboBox = new System.Windows.Forms.ComboBox();
            this.languageLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // addRepoBtn
            // 
            this.addRepoBtn.Location = new System.Drawing.Point(12, 388);
            this.addRepoBtn.Name = "addRepoBtn";
            this.addRepoBtn.Size = new System.Drawing.Size(193, 29);
            this.addRepoBtn.TabIndex = 0;
            this.addRepoBtn.Text = "Add Repo";
            this.addRepoBtn.UseVisualStyleBackColor = true;
            this.addRepoBtn.Click += new System.EventHandler(this.addRepoBtn_Click);
            // 
            // addRepoBox
            // 
            this.addRepoBox.Location = new System.Drawing.Point(12, 360);
            this.addRepoBox.Name = "addRepoBox";
            this.addRepoBox.Size = new System.Drawing.Size(193, 22);
            this.addRepoBox.TabIndex = 1;
            // 
            // repoListBox
            // 
            this.repoListBox.FormattingEnabled = true;
            this.repoListBox.ItemHeight = 16;
            this.repoListBox.Location = new System.Drawing.Point(12, 12);
            this.repoListBox.Name = "repoListBox";
            this.repoListBox.Size = new System.Drawing.Size(193, 340);
            this.repoListBox.TabIndex = 2;
            // 
            // clearAllReposBtn
            // 
            this.clearAllReposBtn.Location = new System.Drawing.Point(12, 481);
            this.clearAllReposBtn.Name = "clearAllReposBtn";
            this.clearAllReposBtn.Size = new System.Drawing.Size(193, 29);
            this.clearAllReposBtn.TabIndex = 3;
            this.clearAllReposBtn.Text = "Remove All Repos";
            this.clearAllReposBtn.UseVisualStyleBackColor = true;
            this.clearAllReposBtn.Click += new System.EventHandler(this.clearAllReposBtn_Click);
            // 
            // clearSelectedRepoBtn
            // 
            this.clearSelectedRepoBtn.Location = new System.Drawing.Point(12, 450);
            this.clearSelectedRepoBtn.Name = "clearSelectedRepoBtn";
            this.clearSelectedRepoBtn.Size = new System.Drawing.Size(193, 29);
            this.clearSelectedRepoBtn.TabIndex = 4;
            this.clearSelectedRepoBtn.Text = "Remove Selected Repo";
            this.clearSelectedRepoBtn.UseVisualStyleBackColor = true;
            this.clearSelectedRepoBtn.Click += new System.EventHandler(this.clearSelectedRepoBtn_Click);
            // 
            // openSelectedRepoBtn
            // 
            this.openSelectedRepoBtn.Location = new System.Drawing.Point(12, 419);
            this.openSelectedRepoBtn.Name = "openSelectedRepoBtn";
            this.openSelectedRepoBtn.Size = new System.Drawing.Size(193, 29);
            this.openSelectedRepoBtn.TabIndex = 5;
            this.openSelectedRepoBtn.Text = "Open Selected Repo";
            this.openSelectedRepoBtn.UseVisualStyleBackColor = true;
            this.openSelectedRepoBtn.Click += new System.EventHandler(this.openSelectedRepoBtn_Click);
            // 
            // packagesListBox
            // 
            this.packagesListBox.FormattingEnabled = true;
            this.packagesListBox.ItemHeight = 16;
            this.packagesListBox.Location = new System.Drawing.Point(227, 12);
            this.packagesListBox.Name = "packagesListBox";
            this.packagesListBox.Size = new System.Drawing.Size(609, 372);
            this.packagesListBox.TabIndex = 6;
            this.packagesListBox.SelectedIndexChanged += new System.EventHandler(this.packagesListBox_SelectedIndexChanged);
            // 
            // downloadSelectedPackageBtn
            // 
            this.downloadSelectedPackageBtn.Location = new System.Drawing.Point(581, 563);
            this.downloadSelectedPackageBtn.Name = "downloadSelectedPackageBtn";
            this.downloadSelectedPackageBtn.Size = new System.Drawing.Size(255, 29);
            this.downloadSelectedPackageBtn.TabIndex = 7;
            this.downloadSelectedPackageBtn.Text = "Download Selected Package";
            this.downloadSelectedPackageBtn.UseVisualStyleBackColor = true;
            this.downloadSelectedPackageBtn.Click += new System.EventHandler(this.downloadSelectedPackageBtn_Click);
            // 
            // searchBox
            // 
            this.searchBox.Location = new System.Drawing.Point(227, 563);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(313, 22);
            this.searchBox.TabIndex = 8;
            this.searchBox.Enter += new System.EventHandler(this.searchBox_Enter);
            this.searchBox.Leave += new System.EventHandler(this.searchBox_Leave);
            // 
            // detailsBox
            // 
            this.detailsBox.Location = new System.Drawing.Point(227, 390);
            this.detailsBox.Multiline = true;
            this.detailsBox.Name = "detailsBox";
            this.detailsBox.ReadOnly = true;
            this.detailsBox.ShortcutsEnabled = true;
            this.detailsBox.Size = new System.Drawing.Size(609, 164);
            this.detailsBox.TabIndex = 10;
            // 
            // searchButton
            // 
            this.searchButton.Location = new System.Drawing.Point(547, 563);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(28, 29);
            this.searchButton.TabIndex = 12;
            this.searchButton.Text = "Y";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            // 
            // darkModeBtn
            // 
            this.darkModeBtn.AutoSize = true;
            this.darkModeBtn.Location = new System.Drawing.Point(12, 565);
            this.darkModeBtn.Name = "darkModeBtn";
            this.darkModeBtn.Size = new System.Drawing.Size(99, 21);
            this.darkModeBtn.TabIndex = 13;
            this.darkModeBtn.Text = "Dark Mode";
            this.darkModeBtn.UseVisualStyleBackColor = true;
            this.darkModeBtn.CheckedChanged += new System.EventHandler(this.darkModeBtn_CheckedChanged);
            // 
            // languageComboBox
            // 
            this.languageComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.languageComboBox.FormattingEnabled = true;
            this.languageComboBox.Location = new System.Drawing.Point(12, 534);
            this.languageComboBox.Name = "languageComboBox";
            this.languageComboBox.Size = new System.Drawing.Size(193, 24);
            this.languageComboBox.TabIndex = 14;
            this.languageComboBox.SelectedIndexChanged += new System.EventHandler(this.languageComboBox_SelectedIndexChanged);
            // 
            // languageLabel
            // 
            this.languageLabel.AutoSize = true;
            this.languageLabel.Location = new System.Drawing.Point(12, 515);
            this.languageLabel.Name = "languageLabel";
            this.languageLabel.Size = new System.Drawing.Size(68, 16);
            this.languageLabel.TabIndex = 15;
            this.languageLabel.Text = "Language";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(848, 601);
            this.Controls.Add(this.languageLabel);
            this.Controls.Add(this.languageComboBox);
            this.Controls.Add(this.darkModeBtn);
            this.Controls.Add(this.searchButton);
            this.Controls.Add(this.detailsBox);
            this.Controls.Add(this.searchBox);
            this.Controls.Add(this.downloadSelectedPackageBtn);
            this.Controls.Add(this.packagesListBox);
            this.Controls.Add(this.openSelectedRepoBtn);
            this.Controls.Add(this.clearSelectedRepoBtn);
            this.Controls.Add(this.clearAllReposBtn);
            this.Controls.Add(this.repoListBox);
            this.Controls.Add(this.addRepoBox);
            this.Controls.Add(this.addRepoBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Main";
            this.Text = "Windows Repo Tool v3.0";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button addRepoBtn;
        private System.Windows.Forms.TextBox addRepoBox;
        private System.Windows.Forms.ListBox repoListBox;
        private System.Windows.Forms.Button clearAllReposBtn;
        private System.Windows.Forms.Button clearSelectedRepoBtn;
        private System.Windows.Forms.Button openSelectedRepoBtn;
        private System.Windows.Forms.ListBox packagesListBox;
        private System.Windows.Forms.Button downloadSelectedPackageBtn;
        private System.Windows.Forms.TextBox searchBox;
        private System.Windows.Forms.TextBox detailsBox;
        private System.Windows.Forms.Button searchButton;
        private System.Windows.Forms.CheckBox darkModeBtn;
        private System.Windows.Forms.ComboBox languageComboBox;
        private System.Windows.Forms.Label languageLabel;
    }
}

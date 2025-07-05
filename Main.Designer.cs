namespace WindowsRepoTool
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
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.languageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.englishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.turkishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.themeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lightModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.darkModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.repoContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editRepoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteRepoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addRepoBtn = new System.Windows.Forms.Button();
            this.addRepoBox = new System.Windows.Forms.TextBox();
            this.repoListBox = new System.Windows.Forms.ListBox();
            this.clearAllReposBtn = new System.Windows.Forms.Button();
            this.clearSelectedRepoBtn = new System.Windows.Forms.Button();
            this.downloadSelectedPackageBtn = new System.Windows.Forms.Button();
            this.packageListView = new System.Windows.Forms.ListView();
            this.columnHeaderName = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderVersion = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderArchitecture = new System.Windows.Forms.ColumnHeader();
            this.searchBox = new System.Windows.Forms.TextBox();
            this.packageDetailsBox = new System.Windows.Forms.TextBox();
            this.menuStrip.SuspendLayout();
            this.repoContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(848, 28);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.languageToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(76, 24);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // languageToolStripMenuItem
            // 
            this.languageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.englishToolStripMenuItem,
            this.turkishToolStripMenuItem});
            this.languageToolStripMenuItem.Name = "languageToolStripMenuItem";
            this.languageToolStripMenuItem.Size = new System.Drawing.Size(145, 26);
            this.languageToolStripMenuItem.Text = "Language";
            // 
            // englishToolStripMenuItem
            // 
            this.englishToolStripMenuItem.Name = "englishToolStripMenuItem";
            this.englishToolStripMenuItem.Size = new System.Drawing.Size(130, 26);
            this.englishToolStripMenuItem.Text = "English";
            this.englishToolStripMenuItem.Click += new System.EventHandler(this.englishToolStripMenuItem_Click);
            // 
            // turkishToolStripMenuItem
            // 
            this.turkishToolStripMenuItem.Name = "turkishToolStripMenuItem";
            this.turkishToolStripMenuItem.Size = new System.Drawing.Size(130, 26);
            this.turkishToolStripMenuItem.Text = "Türkçe";
            this.turkishToolStripMenuItem.Click += new System.EventHandler(this.turkishToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.themeToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(55, 24);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // themeToolStripMenuItem
            // 
            this.themeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lightModeToolStripMenuItem,
            this.darkModeToolStripMenuItem});
            this.themeToolStripMenuItem.Name = "themeToolStripMenuItem";
            this.themeToolStripMenuItem.Size = new System.Drawing.Size(125, 26);
            this.themeToolStripMenuItem.Text = "Theme";
            // 
            // lightModeToolStripMenuItem
            // 
            this.lightModeToolStripMenuItem.Name = "lightModeToolStripMenuItem";
            this.lightModeToolStripMenuItem.Size = new System.Drawing.Size(162, 26);
            this.lightModeToolStripMenuItem.Text = "Light Mode";
            this.lightModeToolStripMenuItem.Click += new System.EventHandler(this.lightModeToolStripMenuItem_Click);
            // 
            // darkModeToolStripMenuItem
            // 
            this.darkModeToolStripMenuItem.Name = "darkModeToolStripMenuItem";
            this.darkModeToolStripMenuItem.Size = new System.Drawing.Size(162, 26);
            this.darkModeToolStripMenuItem.Text = "Dark Mode";
            this.darkModeToolStripMenuItem.Click += new System.EventHandler(this.darkModeToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(64, 24);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // repoContextMenuStrip
            // 
            this.repoContextMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.repoContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editRepoToolStripMenuItem,
            this.deleteRepoToolStripMenuItem});
            this.repoContextMenuStrip.Name = "repoContextMenuStrip";
            this.repoContextMenuStrip.Size = new System.Drawing.Size(153, 52);
            // 
            // editRepoToolStripMenuItem
            // 
            this.editRepoToolStripMenuItem.Name = "editRepoToolStripMenuItem";
            this.editRepoToolStripMenuItem.Size = new System.Drawing.Size(152, 24);
            this.editRepoToolStripMenuItem.Text = "Edit Link";
            this.editRepoToolStripMenuItem.Click += new System.EventHandler(this.editRepoToolStripMenuItem_Click);
            // 
            // deleteRepoToolStripMenuItem
            // 
            this.deleteRepoToolStripMenuItem.Name = "deleteRepoToolStripMenuItem";
            this.deleteRepoToolStripMenuItem.Size = new System.Drawing.Size(152, 24);
            this.deleteRepoToolStripMenuItem.Text = "Delete Repo";
            this.deleteRepoToolStripMenuItem.Click += new System.EventHandler(this.deleteRepoToolStripMenuItem_Click);
            // 
            // addRepoBtn
            // 
            this.addRepoBtn.Enabled = false;
            this.addRepoBtn.Location = new System.Drawing.Point(12, 412);
            this.addRepoBtn.Name = "addRepoBtn";
            this.addRepoBtn.Size = new System.Drawing.Size(193, 29);
            this.addRepoBtn.TabIndex = 1;
            this.addRepoBtn.Text = "Add Repo";
            this.addRepoBtn.UseVisualStyleBackColor = true;
            this.addRepoBtn.Click += new System.EventHandler(this.addRepoBtn_Click);
            // 
            // addRepoBox
            // 
            this.addRepoBox.Location = new System.Drawing.Point(12, 384);
            this.addRepoBox.Name = "addRepoBox";
            this.addRepoBox.Size = new System.Drawing.Size(193, 22);
            this.addRepoBox.TabIndex = 2;
            this.addRepoBox.TextChanged += new System.EventHandler(this.addRepoBox_TextChanged);
            // 
            // repoListBox
            // 
            this.repoListBox.ContextMenuStrip = this.repoContextMenuStrip;
            this.repoListBox.FormattingEnabled = true;
            this.repoListBox.ItemHeight = 16;
            this.repoListBox.Location = new System.Drawing.Point(12, 42);
            this.repoListBox.Name = "repoListBox";
            this.repoListBox.Size = new System.Drawing.Size(193, 324);
            this.repoListBox.TabIndex = 3;
            this.repoListBox.SelectedIndexChanged += new System.EventHandler(this.repoListBox_SelectedIndexChanged);
            // 
            // clearAllReposBtn
            // 
            this.clearAllReposBtn.Location = new System.Drawing.Point(12, 478);
            this.clearAllReposBtn.Name = "clearAllReposBtn";
            this.clearAllReposBtn.Size = new System.Drawing.Size(193, 29);
            this.clearAllReposBtn.TabIndex = 4;
            this.clearAllReposBtn.Text = "Remove All Repos";
            this.clearAllReposBtn.UseVisualStyleBackColor = true;
            this.clearAllReposBtn.Click += new System.EventHandler(this.clearAllReposBtn_Click);
            // 
            // clearSelectedRepoBtn
            // 
            this.clearSelectedRepoBtn.Enabled = false;
            this.clearSelectedRepoBtn.Location = new System.Drawing.Point(12, 447);
            this.clearSelectedRepoBtn.Name = "clearSelectedRepoBtn";
            this.clearSelectedRepoBtn.Size = new System.Drawing.Size(193, 29);
            this.clearSelectedRepoBtn.TabIndex = 5;
            this.clearSelectedRepoBtn.Text = "Remove Selected Repo";
            this.clearSelectedRepoBtn.UseVisualStyleBackColor = true;
            this.clearSelectedRepoBtn.Click += new System.EventHandler(this.clearSelectedRepoBtn_Click);
            // 
            // downloadSelectedPackageBtn
            // 
            this.downloadSelectedPackageBtn.Enabled = false;
            this.downloadSelectedPackageBtn.Location = new System.Drawing.Point(12, 513);
            this.downloadSelectedPackageBtn.Name = "downloadSelectedPackageBtn";
            this.downloadSelectedPackageBtn.Size = new System.Drawing.Size(193, 29);
            this.downloadSelectedPackageBtn.TabIndex = 6;
            this.downloadSelectedPackageBtn.Text = "Download Selected Package";
            this.downloadSelectedPackageBtn.UseVisualStyleBackColor = true;
            this.downloadSelectedPackageBtn.Click += new System.EventHandler(this.downloadSelectedPackageBtn_Click);
            // 
            // packageListView
            // 
            this.packageListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName,
            this.columnHeaderVersion,
            this.columnHeaderArchitecture});
            this.packageListView.FullRowSelect = true;
            this.packageListView.Location = new System.Drawing.Point(227, 71);
            this.packageListView.Name = "packageListView";
            this.packageListView.Size = new System.Drawing.Size(609, 335);
            this.packageListView.TabIndex = 7;
            this.packageListView.UseCompatibleStateImageBehavior = false;
            this.packageListView.View = System.Windows.Forms.View.Details;
            this.packageListView.SelectedIndexChanged += new System.EventHandler(this.packageListView_SelectedIndexChanged);
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "Package Name";
            this.columnHeaderName.Width = 350;
            // 
            // columnHeaderVersion
            // 
            this.columnHeaderVersion.Text = "Version";
            this.columnHeaderVersion.Width = 120;
            // 
            // columnHeaderArchitecture
            // 
            this.columnHeaderArchitecture.Text = "Architecture";
            this.columnHeaderArchitecture.Width = 156;
            // 
            // searchBox
            // 
            this.searchBox.Enabled = false;
            this.searchBox.Location = new System.Drawing.Point(227, 43);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(609, 22);
            this.searchBox.TabIndex = 8;
            this.searchBox.TextChanged += new System.EventHandler(this.searchBox_TextChanged);
            this.searchBox.Enter += new System.EventHandler(this.searchBox_Enter);
            this.searchBox.Leave += new System.EventHandler(this.searchBox_Leave);
            // 
            // packageDetailsBox
            // 
            this.packageDetailsBox.Location = new System.Drawing.Point(227, 420);
            this.packageDetailsBox.Multiline = true;
            this.packageDetailsBox.Name = "packageDetailsBox";
            this.packageDetailsBox.ReadOnly = true;
            this.packageDetailsBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.packageDetailsBox.ShortcutsEnabled = true;
            this.packageDetailsBox.Size = new System.Drawing.Size(609, 138);
            this.packageDetailsBox.TabIndex = 9;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(848, 570);
            this.Controls.Add(this.packageDetailsBox);
            this.Controls.Add(this.searchBox);
            this.Controls.Add(this.packageListView);
            this.Controls.Add(this.downloadSelectedPackageBtn);
            this.Controls.Add(this.clearSelectedRepoBtn);
            this.Controls.Add(this.clearAllReposBtn);
            this.Controls.Add(this.repoListBox);
            this.Controls.Add(this.addRepoBox);
            this.Controls.Add(this.addRepoBtn);
            this.Controls.Add(this.menuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip;
            this.MaximizeBox = false;
            this.Name = "Main";
            this.Text = "Windows Repo Tool v3.0";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.repoContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem languageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem englishToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem turkishToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem themeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lightModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem darkModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip repoContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem editRepoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteRepoToolStripMenuItem;
        private System.Windows.Forms.Button addRepoBtn;
        private System.Windows.Forms.TextBox addRepoBox;
        private System.Windows.Forms.ListBox repoListBox;
        private System.Windows.Forms.Button clearAllReposBtn;
        private System.Windows.Forms.Button clearSelectedRepoBtn;
        private System.Windows.Forms.Button downloadSelectedPackageBtn;
        private System.Windows.Forms.ListView packageListView;
        private System.Windows.Forms.TextBox searchBox;
        private System.Windows.Forms.TextBox packageDetailsBox;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderVersion;
        private System.Windows.Forms.ColumnHeader columnHeaderArchitecture;
    }
}

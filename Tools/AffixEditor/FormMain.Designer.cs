namespace AffixEditor
{
    partial class FormMain
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
            this.AffixInfosListBox = new System.Windows.Forms.ListBox();
            this.AffixInfosSearchTextBox = new System.Windows.Forms.TextBox();
            this.MainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.SaveAffixInfoButton = new System.Windows.Forms.Button();
            this.AffixValueTypeRangePanel = new System.Windows.Forms.Panel();
            this.AffixValueTypeRangeMaxMaxTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.AffixValueTypeRangeMinMaxTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.AffixValueTypeRangeMaxMinTextBox = new System.Windows.Forms.TextBox();
            this.AffixValueTypeRangeMaxLabel = new System.Windows.Forms.Label();
            this.AffixValueTypeRangeMinMinTextBox = new System.Windows.Forms.TextBox();
            this.AffixValueTypeRangeMinLabel = new System.Windows.Forms.Label();
            this.AffixValueTypeSinglePanel = new System.Windows.Forms.Panel();
            this.AffixValueTypeSingleMaxTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.AffixValueTypeSingleMinTextBox = new System.Windows.Forms.TextBox();
            this.AffixValueTypeSingleLabel = new System.Windows.Forms.Label();
            this.AffixInfoDisplaySplitContainer = new System.Windows.Forms.SplitContainer();
            this.AffixNameLabel = new System.Windows.Forms.Label();
            this.AffixDescriptionTextBox = new System.Windows.Forms.TextBox();
            this.AffixDescriptionLabel = new System.Windows.Forms.Label();
            this.AffixNameTextBox = new System.Windows.Forms.TextBox();
            this.AffixProgressionComboBox = new System.Windows.Forms.ComboBox();
            this.AffixProgressionLabel = new System.Windows.Forms.Label();
            this.AffixValueTypeComboBox = new System.Windows.Forms.ComboBox();
            this.AffixValueTypeLabel = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openDataFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateSampleAffixToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.MainSplitContainer)).BeginInit();
            this.MainSplitContainer.Panel1.SuspendLayout();
            this.MainSplitContainer.Panel2.SuspendLayout();
            this.MainSplitContainer.SuspendLayout();
            this.AffixValueTypeRangePanel.SuspendLayout();
            this.AffixValueTypeSinglePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AffixInfoDisplaySplitContainer)).BeginInit();
            this.AffixInfoDisplaySplitContainer.Panel1.SuspendLayout();
            this.AffixInfoDisplaySplitContainer.Panel2.SuspendLayout();
            this.AffixInfoDisplaySplitContainer.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // AffixInfosListBox
            // 
            this.AffixInfosListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AffixInfosListBox.FormattingEnabled = true;
            this.AffixInfosListBox.Location = new System.Drawing.Point(3, 29);
            this.AffixInfosListBox.Name = "AffixInfosListBox";
            this.AffixInfosListBox.Size = new System.Drawing.Size(290, 446);
            this.AffixInfosListBox.TabIndex = 1;
            this.AffixInfosListBox.SelectedIndexChanged += new System.EventHandler(this.AffixInfosListBox_SelectedIndexChanged);
            // 
            // AffixInfosSearchTextBox
            // 
            this.AffixInfosSearchTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AffixInfosSearchTextBox.Location = new System.Drawing.Point(3, 3);
            this.AffixInfosSearchTextBox.Name = "AffixInfosSearchTextBox";
            this.AffixInfosSearchTextBox.Size = new System.Drawing.Size(290, 20);
            this.AffixInfosSearchTextBox.TabIndex = 0;
            this.AffixInfosSearchTextBox.TextChanged += new System.EventHandler(this.AffixInfosSearchTextBox_TextChanged);
            // 
            // MainSplitContainer
            // 
            this.MainSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MainSplitContainer.Location = new System.Drawing.Point(12, 27);
            this.MainSplitContainer.Name = "MainSplitContainer";
            // 
            // MainSplitContainer.Panel1
            // 
            this.MainSplitContainer.Panel1.Controls.Add(this.SaveAffixInfoButton);
            this.MainSplitContainer.Panel1.Controls.Add(this.AffixInfosListBox);
            this.MainSplitContainer.Panel1.Controls.Add(this.AffixInfosSearchTextBox);
            // 
            // MainSplitContainer.Panel2
            // 
            this.MainSplitContainer.Panel2.Controls.Add(this.AffixValueTypeRangePanel);
            this.MainSplitContainer.Panel2.Controls.Add(this.AffixValueTypeSinglePanel);
            this.MainSplitContainer.Panel2.Controls.Add(this.AffixInfoDisplaySplitContainer);
            this.MainSplitContainer.Size = new System.Drawing.Size(834, 527);
            this.MainSplitContainer.SplitterDistance = 296;
            this.MainSplitContainer.TabIndex = 2;
            this.MainSplitContainer.TabStop = false;
            // 
            // SaveAffixInfoButton
            // 
            this.SaveAffixInfoButton.Location = new System.Drawing.Point(218, 481);
            this.SaveAffixInfoButton.Name = "SaveAffixInfoButton";
            this.SaveAffixInfoButton.Size = new System.Drawing.Size(75, 23);
            this.SaveAffixInfoButton.TabIndex = 2;
            this.SaveAffixInfoButton.Text = "Save";
            this.SaveAffixInfoButton.UseVisualStyleBackColor = true;
            this.SaveAffixInfoButton.Click += new System.EventHandler(this.SaveAffixInfoButton_Click);
            // 
            // AffixValueTypeRangePanel
            // 
            this.AffixValueTypeRangePanel.Controls.Add(this.AffixValueTypeRangeMaxMaxTextBox);
            this.AffixValueTypeRangePanel.Controls.Add(this.label3);
            this.AffixValueTypeRangePanel.Controls.Add(this.AffixValueTypeRangeMinMaxTextBox);
            this.AffixValueTypeRangePanel.Controls.Add(this.label2);
            this.AffixValueTypeRangePanel.Controls.Add(this.AffixValueTypeRangeMaxMinTextBox);
            this.AffixValueTypeRangePanel.Controls.Add(this.AffixValueTypeRangeMaxLabel);
            this.AffixValueTypeRangePanel.Controls.Add(this.AffixValueTypeRangeMinMinTextBox);
            this.AffixValueTypeRangePanel.Controls.Add(this.AffixValueTypeRangeMinLabel);
            this.AffixValueTypeRangePanel.Location = new System.Drawing.Point(3, 329);
            this.AffixValueTypeRangePanel.Name = "AffixValueTypeRangePanel";
            this.AffixValueTypeRangePanel.Size = new System.Drawing.Size(528, 86);
            this.AffixValueTypeRangePanel.TabIndex = 2;
            this.AffixValueTypeRangePanel.Visible = false;
            // 
            // AffixValueTypeRangeMaxMaxTextBox
            // 
            this.AffixValueTypeRangeMaxMaxTextBox.Location = new System.Drawing.Point(245, 52);
            this.AffixValueTypeRangeMaxMaxTextBox.Name = "AffixValueTypeRangeMaxMaxTextBox";
            this.AffixValueTypeRangeMaxMaxTextBox.Size = new System.Drawing.Size(217, 20);
            this.AffixValueTypeRangeMaxMaxTextBox.TabIndex = 7;
            this.AffixValueTypeRangeMaxMaxTextBox.TextChanged += new System.EventHandler(this.AffixValueTypeRangeMaxMaxTextBox_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(223, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(16, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "to";
            // 
            // AffixValueTypeRangeMinMaxTextBox
            // 
            this.AffixValueTypeRangeMinMaxTextBox.Location = new System.Drawing.Point(245, 13);
            this.AffixValueTypeRangeMinMaxTextBox.Name = "AffixValueTypeRangeMinMaxTextBox";
            this.AffixValueTypeRangeMinMaxTextBox.Size = new System.Drawing.Size(217, 20);
            this.AffixValueTypeRangeMinMaxTextBox.TabIndex = 5;
            this.AffixValueTypeRangeMinMaxTextBox.TextChanged += new System.EventHandler(this.AffixValueTypeRangeMinMaxTextBox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(223, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(16, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "to";
            // 
            // AffixValueTypeRangeMaxMinTextBox
            // 
            this.AffixValueTypeRangeMaxMinTextBox.Location = new System.Drawing.Point(0, 52);
            this.AffixValueTypeRangeMaxMinTextBox.Name = "AffixValueTypeRangeMaxMinTextBox";
            this.AffixValueTypeRangeMaxMinTextBox.Size = new System.Drawing.Size(217, 20);
            this.AffixValueTypeRangeMaxMinTextBox.TabIndex = 3;
            this.AffixValueTypeRangeMaxMinTextBox.TextChanged += new System.EventHandler(this.AffixValueTypeRangeMaxMinTextBox_TextChanged);
            this.AffixValueTypeRangeMaxMinTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.CheckAffixValueTextBoxInput);
            // 
            // AffixValueTypeRangeMaxLabel
            // 
            this.AffixValueTypeRangeMaxLabel.AutoSize = true;
            this.AffixValueTypeRangeMaxLabel.Location = new System.Drawing.Point(0, 36);
            this.AffixValueTypeRangeMaxLabel.Name = "AffixValueTypeRangeMaxLabel";
            this.AffixValueTypeRangeMaxLabel.Size = new System.Drawing.Size(81, 13);
            this.AffixValueTypeRangeMaxLabel.TabIndex = 2;
            this.AffixValueTypeRangeMaxLabel.Text = "Maximum Value";
            // 
            // AffixValueTypeRangeMinMinTextBox
            // 
            this.AffixValueTypeRangeMinMinTextBox.Location = new System.Drawing.Point(0, 13);
            this.AffixValueTypeRangeMinMinTextBox.Name = "AffixValueTypeRangeMinMinTextBox";
            this.AffixValueTypeRangeMinMinTextBox.Size = new System.Drawing.Size(217, 20);
            this.AffixValueTypeRangeMinMinTextBox.TabIndex = 1;
            this.AffixValueTypeRangeMinMinTextBox.TextChanged += new System.EventHandler(this.AffixValueTypeRangeMinMinTextBox_TextChanged);
            this.AffixValueTypeRangeMinMinTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.CheckAffixValueTextBoxInput);
            // 
            // AffixValueTypeRangeMinLabel
            // 
            this.AffixValueTypeRangeMinLabel.AutoSize = true;
            this.AffixValueTypeRangeMinLabel.Location = new System.Drawing.Point(0, -3);
            this.AffixValueTypeRangeMinLabel.Name = "AffixValueTypeRangeMinLabel";
            this.AffixValueTypeRangeMinLabel.Size = new System.Drawing.Size(78, 13);
            this.AffixValueTypeRangeMinLabel.TabIndex = 0;
            this.AffixValueTypeRangeMinLabel.Text = "Minimum Value";
            // 
            // AffixValueTypeSinglePanel
            // 
            this.AffixValueTypeSinglePanel.Controls.Add(this.AffixValueTypeSingleMaxTextBox);
            this.AffixValueTypeSinglePanel.Controls.Add(this.label1);
            this.AffixValueTypeSinglePanel.Controls.Add(this.AffixValueTypeSingleMinTextBox);
            this.AffixValueTypeSinglePanel.Controls.Add(this.AffixValueTypeSingleLabel);
            this.AffixValueTypeSinglePanel.Location = new System.Drawing.Point(3, 237);
            this.AffixValueTypeSinglePanel.Name = "AffixValueTypeSinglePanel";
            this.AffixValueTypeSinglePanel.Size = new System.Drawing.Size(528, 86);
            this.AffixValueTypeSinglePanel.TabIndex = 0;
            this.AffixValueTypeSinglePanel.Visible = false;
            // 
            // AffixValueTypeSingleMaxTextBox
            // 
            this.AffixValueTypeSingleMaxTextBox.Location = new System.Drawing.Point(245, 13);
            this.AffixValueTypeSingleMaxTextBox.Name = "AffixValueTypeSingleMaxTextBox";
            this.AffixValueTypeSingleMaxTextBox.Size = new System.Drawing.Size(217, 20);
            this.AffixValueTypeSingleMaxTextBox.TabIndex = 3;
            this.AffixValueTypeSingleMaxTextBox.TextChanged += new System.EventHandler(this.AffixValueTypeSingleMaxTextBox_TextChanged);
            this.AffixValueTypeSingleMaxTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.CheckAffixValueTextBoxInput);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(223, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(16, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "to";
            // 
            // AffixValueTypeSingleMinTextBox
            // 
            this.AffixValueTypeSingleMinTextBox.Location = new System.Drawing.Point(0, 13);
            this.AffixValueTypeSingleMinTextBox.Name = "AffixValueTypeSingleMinTextBox";
            this.AffixValueTypeSingleMinTextBox.Size = new System.Drawing.Size(217, 20);
            this.AffixValueTypeSingleMinTextBox.TabIndex = 1;
            this.AffixValueTypeSingleMinTextBox.TextChanged += new System.EventHandler(this.AffixValueTypeSingleMinTextBox_TextChanged);
            this.AffixValueTypeSingleMinTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.CheckAffixValueTextBoxInput);
            // 
            // AffixValueTypeSingleLabel
            // 
            this.AffixValueTypeSingleLabel.AutoSize = true;
            this.AffixValueTypeSingleLabel.Location = new System.Drawing.Point(0, -3);
            this.AffixValueTypeSingleLabel.Name = "AffixValueTypeSingleLabel";
            this.AffixValueTypeSingleLabel.Size = new System.Drawing.Size(34, 13);
            this.AffixValueTypeSingleLabel.TabIndex = 0;
            this.AffixValueTypeSingleLabel.Text = "Value";
            // 
            // AffixInfoDisplaySplitContainer
            // 
            this.AffixInfoDisplaySplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AffixInfoDisplaySplitContainer.Location = new System.Drawing.Point(3, 3);
            this.AffixInfoDisplaySplitContainer.Name = "AffixInfoDisplaySplitContainer";
            this.AffixInfoDisplaySplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // AffixInfoDisplaySplitContainer.Panel1
            // 
            this.AffixInfoDisplaySplitContainer.Panel1.Controls.Add(this.AffixNameLabel);
            this.AffixInfoDisplaySplitContainer.Panel1.Controls.Add(this.AffixDescriptionTextBox);
            this.AffixInfoDisplaySplitContainer.Panel1.Controls.Add(this.AffixDescriptionLabel);
            this.AffixInfoDisplaySplitContainer.Panel1.Controls.Add(this.AffixNameTextBox);
            // 
            // AffixInfoDisplaySplitContainer.Panel2
            // 
            this.AffixInfoDisplaySplitContainer.Panel2.Controls.Add(this.AffixProgressionComboBox);
            this.AffixInfoDisplaySplitContainer.Panel2.Controls.Add(this.AffixProgressionLabel);
            this.AffixInfoDisplaySplitContainer.Panel2.Controls.Add(this.AffixValueTypeComboBox);
            this.AffixInfoDisplaySplitContainer.Panel2.Controls.Add(this.AffixValueTypeLabel);
            this.AffixInfoDisplaySplitContainer.Size = new System.Drawing.Size(534, 228);
            this.AffixInfoDisplaySplitContainer.SplitterDistance = 142;
            this.AffixInfoDisplaySplitContainer.TabIndex = 4;
            this.AffixInfoDisplaySplitContainer.TabStop = false;
            // 
            // AffixNameLabel
            // 
            this.AffixNameLabel.AutoSize = true;
            this.AffixNameLabel.Location = new System.Drawing.Point(0, -1);
            this.AffixNameLabel.Name = "AffixNameLabel";
            this.AffixNameLabel.Size = new System.Drawing.Size(35, 13);
            this.AffixNameLabel.TabIndex = 0;
            this.AffixNameLabel.Text = "Name";
            // 
            // AffixDescriptionTextBox
            // 
            this.AffixDescriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AffixDescriptionTextBox.Enabled = false;
            this.AffixDescriptionTextBox.Location = new System.Drawing.Point(3, 57);
            this.AffixDescriptionTextBox.Multiline = true;
            this.AffixDescriptionTextBox.Name = "AffixDescriptionTextBox";
            this.AffixDescriptionTextBox.Size = new System.Drawing.Size(514, 71);
            this.AffixDescriptionTextBox.TabIndex = 3;
            this.AffixDescriptionTextBox.TextChanged += new System.EventHandler(this.AffixDescriptionTextBox_TextChanged);
            // 
            // AffixDescriptionLabel
            // 
            this.AffixDescriptionLabel.AutoSize = true;
            this.AffixDescriptionLabel.Location = new System.Drawing.Point(0, 41);
            this.AffixDescriptionLabel.Name = "AffixDescriptionLabel";
            this.AffixDescriptionLabel.Size = new System.Drawing.Size(60, 13);
            this.AffixDescriptionLabel.TabIndex = 2;
            this.AffixDescriptionLabel.Text = "Description";
            // 
            // AffixNameTextBox
            // 
            this.AffixNameTextBox.Enabled = false;
            this.AffixNameTextBox.Location = new System.Drawing.Point(3, 18);
            this.AffixNameTextBox.Name = "AffixNameTextBox";
            this.AffixNameTextBox.Size = new System.Drawing.Size(217, 20);
            this.AffixNameTextBox.TabIndex = 1;
            this.AffixNameTextBox.TextChanged += new System.EventHandler(this.AffixNameTextBox_TextChanged);
            // 
            // AffixProgressionComboBox
            // 
            this.AffixProgressionComboBox.Enabled = false;
            this.AffixProgressionComboBox.FormattingEnabled = true;
            this.AffixProgressionComboBox.Location = new System.Drawing.Point(245, 16);
            this.AffixProgressionComboBox.Name = "AffixProgressionComboBox";
            this.AffixProgressionComboBox.Size = new System.Drawing.Size(217, 21);
            this.AffixProgressionComboBox.TabIndex = 3;
            this.AffixProgressionComboBox.SelectedIndexChanged += new System.EventHandler(this.AffixProgressionComboBox_SelectedIndexChanged);
            // 
            // AffixProgressionLabel
            // 
            this.AffixProgressionLabel.AutoSize = true;
            this.AffixProgressionLabel.Location = new System.Drawing.Point(245, 0);
            this.AffixProgressionLabel.Name = "AffixProgressionLabel";
            this.AffixProgressionLabel.Size = new System.Drawing.Size(62, 13);
            this.AffixProgressionLabel.TabIndex = 2;
            this.AffixProgressionLabel.Text = "Progression";
            // 
            // AffixValueTypeComboBox
            // 
            this.AffixValueTypeComboBox.Enabled = false;
            this.AffixValueTypeComboBox.FormattingEnabled = true;
            this.AffixValueTypeComboBox.Location = new System.Drawing.Point(3, 16);
            this.AffixValueTypeComboBox.Name = "AffixValueTypeComboBox";
            this.AffixValueTypeComboBox.Size = new System.Drawing.Size(217, 21);
            this.AffixValueTypeComboBox.TabIndex = 1;
            this.AffixValueTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.AffixValueTypeComboBox_SelectedIndexChanged);
            // 
            // AffixValueTypeLabel
            // 
            this.AffixValueTypeLabel.AutoSize = true;
            this.AffixValueTypeLabel.Location = new System.Drawing.Point(3, 0);
            this.AffixValueTypeLabel.Name = "AffixValueTypeLabel";
            this.AffixValueTypeLabel.Size = new System.Drawing.Size(31, 13);
            this.AffixValueTypeLabel.TabIndex = 0;
            this.AffixValueTypeLabel.Text = "Type";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.testToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(858, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openDataFolderToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openDataFolderToolStripMenuItem
            // 
            this.openDataFolderToolStripMenuItem.Name = "openDataFolderToolStripMenuItem";
            this.openDataFolderToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.openDataFolderToolStripMenuItem.Text = "Open Data Folder";
            this.openDataFolderToolStripMenuItem.Click += new System.EventHandler(this.openDataFolderToolStripMenuItem_Click);
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generateSampleAffixToolStripMenuItem});
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.testToolStripMenuItem.Text = "Test";
            // 
            // generateSampleAffixToolStripMenuItem
            // 
            this.generateSampleAffixToolStripMenuItem.Name = "generateSampleAffixToolStripMenuItem";
            this.generateSampleAffixToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.generateSampleAffixToolStripMenuItem.Text = "Generate Sample Affix";
            this.generateSampleAffixToolStripMenuItem.Click += new System.EventHandler(this.generateSampleAffixToolStripMenuItem_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(858, 566);
            this.Controls.Add(this.MainSplitContainer);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.Text = "Affix Editor";
            this.MainSplitContainer.Panel1.ResumeLayout(false);
            this.MainSplitContainer.Panel1.PerformLayout();
            this.MainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MainSplitContainer)).EndInit();
            this.MainSplitContainer.ResumeLayout(false);
            this.AffixValueTypeRangePanel.ResumeLayout(false);
            this.AffixValueTypeRangePanel.PerformLayout();
            this.AffixValueTypeSinglePanel.ResumeLayout(false);
            this.AffixValueTypeSinglePanel.PerformLayout();
            this.AffixInfoDisplaySplitContainer.Panel1.ResumeLayout(false);
            this.AffixInfoDisplaySplitContainer.Panel1.PerformLayout();
            this.AffixInfoDisplaySplitContainer.Panel2.ResumeLayout(false);
            this.AffixInfoDisplaySplitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AffixInfoDisplaySplitContainer)).EndInit();
            this.AffixInfoDisplaySplitContainer.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox AffixInfosListBox;
        private System.Windows.Forms.TextBox AffixInfosSearchTextBox;
        private System.Windows.Forms.SplitContainer MainSplitContainer;
        private System.Windows.Forms.TextBox AffixNameTextBox;
        private System.Windows.Forms.Label AffixNameLabel;
        private System.Windows.Forms.TextBox AffixDescriptionTextBox;
        private System.Windows.Forms.Label AffixDescriptionLabel;
        private System.Windows.Forms.SplitContainer AffixInfoDisplaySplitContainer;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openDataFolderToolStripMenuItem;
        private System.Windows.Forms.Panel AffixValueTypeSinglePanel;
        private System.Windows.Forms.ComboBox AffixValueTypeComboBox;
        private System.Windows.Forms.Label AffixValueTypeLabel;
        private System.Windows.Forms.Label AffixValueTypeSingleLabel;
        private System.Windows.Forms.TextBox AffixValueTypeSingleMinTextBox;
        private System.Windows.Forms.Panel AffixValueTypeRangePanel;
        private System.Windows.Forms.TextBox AffixValueTypeRangeMaxMinTextBox;
        private System.Windows.Forms.Label AffixValueTypeRangeMaxLabel;
        private System.Windows.Forms.TextBox AffixValueTypeRangeMinMinTextBox;
        private System.Windows.Forms.Label AffixValueTypeRangeMinLabel;
        private System.Windows.Forms.TextBox AffixValueTypeSingleMaxTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateSampleAffixToolStripMenuItem;
        private System.Windows.Forms.TextBox AffixValueTypeRangeMaxMaxTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox AffixValueTypeRangeMinMaxTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox AffixProgressionComboBox;
        private System.Windows.Forms.Label AffixProgressionLabel;
        private System.Windows.Forms.Button SaveAffixInfoButton;
    }
}


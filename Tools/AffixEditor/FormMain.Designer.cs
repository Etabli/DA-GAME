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
            this.AffixInfoDisplaySplitContainer = new System.Windows.Forms.SplitContainer();
            this.AffixNameLabel = new System.Windows.Forms.Label();
            this.AffixDescriptionTextBox = new System.Windows.Forms.TextBox();
            this.AffixDescriptionLabel = new System.Windows.Forms.Label();
            this.AffixNameTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.MainSplitContainer)).BeginInit();
            this.MainSplitContainer.Panel1.SuspendLayout();
            this.MainSplitContainer.Panel2.SuspendLayout();
            this.MainSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AffixInfoDisplaySplitContainer)).BeginInit();
            this.AffixInfoDisplaySplitContainer.Panel1.SuspendLayout();
            this.AffixInfoDisplaySplitContainer.SuspendLayout();
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
            this.AffixInfosListBox.Size = new System.Drawing.Size(290, 511);
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
            this.MainSplitContainer.Location = new System.Drawing.Point(12, 12);
            this.MainSplitContainer.Name = "MainSplitContainer";
            // 
            // MainSplitContainer.Panel1
            // 
            this.MainSplitContainer.Panel1.Controls.Add(this.AffixInfosListBox);
            this.MainSplitContainer.Panel1.Controls.Add(this.AffixInfosSearchTextBox);
            // 
            // MainSplitContainer.Panel2
            // 
            this.MainSplitContainer.Panel2.Controls.Add(this.AffixInfoDisplaySplitContainer);
            this.MainSplitContainer.Size = new System.Drawing.Size(834, 542);
            this.MainSplitContainer.SplitterDistance = 296;
            this.MainSplitContainer.TabIndex = 2;
            this.MainSplitContainer.TabStop = false;
            // 
            // AffixInfoDisplaySplitContainer
            // 
            this.AffixInfoDisplaySplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AffixInfoDisplaySplitContainer.Location = new System.Drawing.Point(0, 0);
            this.AffixInfoDisplaySplitContainer.Name = "AffixInfoDisplaySplitContainer";
            this.AffixInfoDisplaySplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // AffixInfoDisplaySplitContainer.Panel1
            // 
            this.AffixInfoDisplaySplitContainer.Panel1.Controls.Add(this.AffixNameLabel);
            this.AffixInfoDisplaySplitContainer.Panel1.Controls.Add(this.AffixDescriptionTextBox);
            this.AffixInfoDisplaySplitContainer.Panel1.Controls.Add(this.AffixDescriptionLabel);
            this.AffixInfoDisplaySplitContainer.Panel1.Controls.Add(this.AffixNameTextBox);
            this.AffixInfoDisplaySplitContainer.Size = new System.Drawing.Size(534, 542);
            this.AffixInfoDisplaySplitContainer.SplitterDistance = 178;
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
            this.AffixDescriptionTextBox.Location = new System.Drawing.Point(3, 57);
            this.AffixDescriptionTextBox.Multiline = true;
            this.AffixDescriptionTextBox.Name = "AffixDescriptionTextBox";
            this.AffixDescriptionTextBox.Size = new System.Drawing.Size(514, 107);
            this.AffixDescriptionTextBox.TabIndex = 3;
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
            this.AffixNameTextBox.Location = new System.Drawing.Point(3, 18);
            this.AffixNameTextBox.Name = "AffixNameTextBox";
            this.AffixNameTextBox.Size = new System.Drawing.Size(217, 20);
            this.AffixNameTextBox.TabIndex = 1;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(858, 566);
            this.Controls.Add(this.MainSplitContainer);
            this.Name = "FormMain";
            this.Text = "Affix Editor";
            this.MainSplitContainer.Panel1.ResumeLayout(false);
            this.MainSplitContainer.Panel1.PerformLayout();
            this.MainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MainSplitContainer)).EndInit();
            this.MainSplitContainer.ResumeLayout(false);
            this.AffixInfoDisplaySplitContainer.Panel1.ResumeLayout(false);
            this.AffixInfoDisplaySplitContainer.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AffixInfoDisplaySplitContainer)).EndInit();
            this.AffixInfoDisplaySplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

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
    }
}


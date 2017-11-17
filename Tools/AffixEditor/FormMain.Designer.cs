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
            this.SuspendLayout();
            // 
            // AffixInfosListBox
            // 
            this.AffixInfosListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.AffixInfosListBox.FormattingEnabled = true;
            this.AffixInfosListBox.Location = new System.Drawing.Point(13, 39);
            this.AffixInfosListBox.Name = "AffixInfosListBox";
            this.AffixInfosListBox.Size = new System.Drawing.Size(215, 420);
            this.AffixInfosListBox.TabIndex = 0;
            // 
            // AffixInfosSearchTextBox
            // 
            this.AffixInfosSearchTextBox.Location = new System.Drawing.Point(13, 13);
            this.AffixInfosSearchTextBox.Name = "AffixInfosSearchTextBox";
            this.AffixInfosSearchTextBox.Size = new System.Drawing.Size(214, 20);
            this.AffixInfosSearchTextBox.TabIndex = 1;
            this.AffixInfosSearchTextBox.TextChanged += new System.EventHandler(this.AffixInfosSearchTextBox_TextChanged);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 469);
            this.Controls.Add(this.AffixInfosSearchTextBox);
            this.Controls.Add(this.AffixInfosListBox);
            this.Name = "FormMain";
            this.Text = "Affix Editor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox AffixInfosListBox;
        private System.Windows.Forms.TextBox AffixInfosSearchTextBox;
    }
}


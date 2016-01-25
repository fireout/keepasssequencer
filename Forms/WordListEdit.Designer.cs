namespace Sequencer.Forms
{
    partial class WordListEdit
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
            this.wordList = new System.Windows.Forms.TextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.addWordsMenu = new System.Windows.Forms.ToolStripDropDownButton();
            this.addDicewareButton = new System.Windows.Forms.ToolStripMenuItem();
            this.addAltDicewareButton = new System.Windows.Forms.ToolStripMenuItem();
            this.addNgslButton = new System.Windows.Forms.ToolStripMenuItem();
            this.addTop5kButton = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.addTxtFileButton = new System.Windows.Forms.ToolStripMenuItem();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // wordList
            // 
            this.wordList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.wordList.Location = new System.Drawing.Point(12, 31);
            this.wordList.Multiline = true;
            this.wordList.Name = "wordList";
            this.wordList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.wordList.Size = new System.Drawing.Size(260, 87);
            this.wordList.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addWordsMenu});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(284, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // addWordsMenu
            // 
            this.addWordsMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.addWordsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addDicewareButton,
            this.addAltDicewareButton,
            this.addNgslButton,
            this.addTop5kButton,
            this.toolStripSeparator1,
            this.addTxtFileButton});
            this.addWordsMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addWordsMenu.Name = "addWordsMenu";
            this.addWordsMenu.Size = new System.Drawing.Size(86, 22);
            this.addWordsMenu.Text = "Add words...";
            this.addWordsMenu.ToolTipText = "Insert words from built-in lists or text files";
            // 
            // addDicewareButton
            // 
            this.addDicewareButton.Name = "addDicewareButton";
            this.addDicewareButton.Size = new System.Drawing.Size(202, 22);
            this.addDicewareButton.Text = "Diceware";
            this.addDicewareButton.Click += new System.EventHandler(this.addDicewareButton_Click);
            // 
            // addAltDicewareButton
            // 
            this.addAltDicewareButton.Name = "addAltDicewareButton";
            this.addAltDicewareButton.Size = new System.Drawing.Size(202, 22);
            this.addAltDicewareButton.Text = "Beale Diceware";
            this.addAltDicewareButton.Click += new System.EventHandler(this.addAltDicewareButton_Click);
            // 
            // addNgslButton
            // 
            this.addNgslButton.Name = "addNgslButton";
            this.addNgslButton.Size = new System.Drawing.Size(202, 22);
            this.addNgslButton.Text = "New General Service List";
            this.addNgslButton.Click += new System.EventHandler(this.addNgslButton_Click);
            // 
            // addTop5kButton
            // 
            this.addTop5kButton.Name = "addTop5kButton";
            this.addTop5kButton.Size = new System.Drawing.Size(202, 22);
            this.addTop5kButton.Text = "Top 5000 English Words";
            this.addTop5kButton.Click += new System.EventHandler(this.addTop5kButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(199, 6);
            // 
            // addTxtFileButton
            // 
            this.addTxtFileButton.Name = "addTxtFileButton";
            this.addTxtFileButton.Size = new System.Drawing.Size(202, 22);
            this.addTxtFileButton.Text = "From file...";
            this.addTxtFileButton.ToolTipText = "Add words from a text file of your choice";
            this.addTxtFileButton.Click += new System.EventHandler(this.addTxtFileButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(115, 124);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(196, 124);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // WordListEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 154);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.wordList);
            this.Name = "WordListEdit";
            this.Text = "Edit Word List";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox wordList;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton addWordsMenu;
        private System.Windows.Forms.ToolStripMenuItem addDicewareButton;
        private System.Windows.Forms.ToolStripMenuItem addAltDicewareButton;
        private System.Windows.Forms.ToolStripMenuItem addNgslButton;
        private System.Windows.Forms.ToolStripMenuItem addTop5kButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem addTxtFileButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
    }
}
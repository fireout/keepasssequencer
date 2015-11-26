namespace Sequencer.Forms
{
    partial class MainForm
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.txtCharacterList = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtWordList = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.lvSequence = new System.Windows.Forms.ListView();
            this.colType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colProbability = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colContent = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.substitutionList1 = new SubstitutionListControl();
            this.tsbAddSequence = new System.Windows.Forms.ToolStripButton();
            this.tsbDeleteSequence = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.substitutionList1);
            this.splitContainer1.Panel1.Controls.Add(this.txtCharacterList);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.txtWordList);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1MinSize = 200;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.toolStrip1);
            this.splitContainer1.Panel2.Controls.Add(this.lvSequence);
            this.splitContainer1.Size = new System.Drawing.Size(656, 574);
            this.splitContainer1.SplitterDistance = 240;
            this.splitContainer1.TabIndex = 0;
            // 
            // txtCharacterList
            // 
            this.txtCharacterList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCharacterList.Location = new System.Drawing.Point(13, 84);
            this.txtCharacterList.Multiline = true;
            this.txtCharacterList.Name = "txtCharacterList";
            this.txtCharacterList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtCharacterList.Size = new System.Drawing.Size(632, 40);
            this.txtCharacterList.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 127);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Default Substitutions";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Default Character List";
            // 
            // txtWordList
            // 
            this.txtWordList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWordList.Location = new System.Drawing.Point(12, 25);
            this.txtWordList.Multiline = true;
            this.txtWordList.Name = "txtWordList";
            this.txtWordList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtWordList.Size = new System.Drawing.Size(632, 40);
            this.txtWordList.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Default Word List";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbAddSequence,
            this.tsbDeleteSequence,
            this.toolStripSeparator1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(656, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // lvSequence
            // 
            this.lvSequence.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvSequence.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvSequence.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colType,
            this.colProbability,
            this.colDescription,
            this.colContent});
            this.lvSequence.FullRowSelect = true;
            this.lvSequence.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvSequence.Location = new System.Drawing.Point(3, 28);
            this.lvSequence.MultiSelect = false;
            this.lvSequence.Name = "lvSequence";
            this.lvSequence.ShowGroups = false;
            this.lvSequence.Size = new System.Drawing.Size(650, 299);
            this.lvSequence.TabIndex = 0;
            this.lvSequence.UseCompatibleStateImageBehavior = false;
            this.lvSequence.View = System.Windows.Forms.View.Details;
            // 
            // colType
            // 
            this.colType.Text = "Type";
            this.colType.Width = 100;
            // 
            // colProbability
            // 
            this.colProbability.Text = "Probability";
            this.colProbability.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // colDescription
            // 
            this.colDescription.Text = "Description";
            this.colDescription.Width = 200;
            // 
            // colContent
            // 
            this.colContent.Text = "Content";
            this.colContent.Width = 400;
            // 
            // substitutionList1
            // 
            this.substitutionList1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.substitutionList1.Location = new System.Drawing.Point(13, 143);
            this.substitutionList1.Name = "substitutionList1";
            this.substitutionList1.Size = new System.Drawing.Size(631, 94);
            this.substitutionList1.Substitutions = null;
            this.substitutionList1.TabIndex = 5;
            // 
            // tsbAddSequence
            // 
            this.tsbAddSequence.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAddSequence.Image = global::Sequencer.Properties.Resources.icon_add;
            this.tsbAddSequence.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAddSequence.Name = "tsbAddSequence";
            this.tsbAddSequence.Size = new System.Drawing.Size(23, 22);
            this.tsbAddSequence.Text = "Add Sequence Item";
            // 
            // tsbDeleteSequence
            // 
            this.tsbDeleteSequence.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDeleteSequence.Image = global::Sequencer.Properties.Resources.delete_24;
            this.tsbDeleteSequence.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDeleteSequence.Name = "tsbDeleteSequence";
            this.tsbDeleteSequence.Size = new System.Drawing.Size(23, 22);
            this.tsbDeleteSequence.Text = "Delete Sequence Item";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(656, 574);
            this.Controls.Add(this.splitContainer1);
            this.MinimumSize = new System.Drawing.Size(300, 400);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ListView lvSequence;
        private System.Windows.Forms.ColumnHeader colType;
        private System.Windows.Forms.ColumnHeader colDescription;
        private System.Windows.Forms.ColumnHeader colProbability;
        private System.Windows.Forms.ColumnHeader colContent;
        private System.Windows.Forms.TextBox txtCharacterList;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtWordList;
        private System.Windows.Forms.Label label1;
        private Forms.SubstitutionListControl substitutionList1;
        private System.Windows.Forms.ToolStripButton tsbAddSequence;
        private System.Windows.Forms.ToolStripButton tsbDeleteSequence;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;

    }
}

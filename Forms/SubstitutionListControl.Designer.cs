namespace Sequencer.Forms
{
    partial class SubstitutionListControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lvSubstitutions = new System.Windows.Forms.ListView();
            this.colType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colReplace = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colWith = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colCaseSensitive = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // lvSubstitutions
            // 
            this.lvSubstitutions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colType,
            this.colReplace,
            this.colWith,
            this.colCaseSensitive});
            this.lvSubstitutions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvSubstitutions.FullRowSelect = true;
            this.lvSubstitutions.Location = new System.Drawing.Point(0, 0);
            this.lvSubstitutions.MultiSelect = false;
            this.lvSubstitutions.Name = "lvSubstitutions";
            this.lvSubstitutions.Size = new System.Drawing.Size(359, 235);
            this.lvSubstitutions.TabIndex = 0;
            this.lvSubstitutions.UseCompatibleStateImageBehavior = false;
            this.lvSubstitutions.View = System.Windows.Forms.View.Details;
            // 
            // colType
            // 
            this.colType.Text = "Replace";
            // 
            // colReplace
            // 
            this.colReplace.Text = "From";
            // 
            // colWith
            // 
            this.colWith.Text = "With";
            // 
            // colCaseSensitive
            // 
            this.colCaseSensitive.Text = "Case Sensitive";
            // 
            // SubstitutionListControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lvSubstitutions);
            this.Name = "SubstitutionListControl";
            this.Size = new System.Drawing.Size(359, 235);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvSubstitutions;
        private System.Windows.Forms.ColumnHeader colType;
        private System.Windows.Forms.ColumnHeader colReplace;
        private System.Windows.Forms.ColumnHeader colWith;
        private System.Windows.Forms.ColumnHeader colCaseSensitive;
    }
}

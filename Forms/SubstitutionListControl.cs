using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Sequencer.Configuration;
using Sequencer.Configuration.Model;

namespace Sequencer.Forms
{
    public class SubstitutionListControl : UserControl
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

        public SubstitutionListControl()
        {
            InitializeComponent();
            lvSubstitutions.DoubleClick += new EventHandler(lvSubstitutions_DoubleClick);
            lvSubstitutions.SelectedIndexChanged += lvSubstitutions_SelectedIndexChanged;
        }

        [Description("Raises when the selection changes.")]
        public event EventHandler SelectedIndexChanged;
        [Description("Raises when the a substitution had changed.")]
        public event EventHandler SubstitutionChanged;

        protected virtual void OnSubstitutionChanged()
        {
            EventHandler handler = SubstitutionChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnSelectedIndexChanged()
        {
            EventHandler handler = SelectedIndexChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        private void lvSubstitutions_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnSelectedIndexChanged();
        }

        public ListViewItem SelectedItem { get { return lvSubstitutions.SelectedItems.Count == 1 ? lvSubstitutions.SelectedItems.OfType<ListViewItem>().Single() : null; } }
        public BaseSubstitution SelectedSubstitution { get { return lvSubstitutions.SelectedItems.Count == 1 ? ListViewMapping[lvSubstitutions.SelectedItems.OfType<ListViewItem>().Single()] : null; } }

        void lvSubstitutions_DoubleClick(object sender, EventArgs e)
        {
            if (lvSubstitutions.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = lvSubstitutions.SelectedItems[0];
                if (selectedItem != null)
                {
                    SubstitutionForm editForm = new SubstitutionForm();
                    editForm.Substitution = ListViewMapping[selectedItem];
                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        ListViewMapping[selectedItem] = editForm.Substitution;
                        SetListViewDetails(editForm.Substitution, selectedItem);
                        Substitutions = ListViewMapping.Values;
                        OnSubstitutionChanged();
                    }
                }
            }
        }

        public IEnumerable<BaseSubstitution> Substitutions { get; set; }

        private Dictionary<ListViewItem, BaseSubstitution> ListViewMapping { get; set; }

        public void DataBind()
        {
            ListViewMapping = new Dictionary<ListViewItem, BaseSubstitution>();
            lvSubstitutions.Items.Clear();
            ListViewMapping.Clear();
            foreach (BaseSubstitution substitution in Substitutions)
            {
                ListViewItem item = new ListViewItem();

                SetListViewDetails(substitution, item);

                ListViewMapping.Add(item, substitution);
                lvSubstitutions.Items.Add(item);
            }
        }

        private void SetListViewDetails(BaseSubstitution substitution, ListViewItem item)
        {
            if (substitution is AnySubstitution)
                item.Text = "Anything";
            else if (substitution is WholeSubstitution)
                item.Text = "Everything";

            if (item.SubItems.Count < 2)
                item.SubItems.Add(substitution.Replace);
            else
                item.SubItems[colReplace.Index].Text = substitution.Replace;

            if (item.SubItems.Count < 3)
                item.SubItems.Add(substitution.With);
            else
                item.SubItems[colWith.Index].Text = substitution.With;

            if (item.SubItems.Count < 4)
                item.SubItems.Add(substitution.CaseSensitive ? "Yes" : "No");
            else
                item.SubItems[colCaseSensitive.Index].Text = substitution.CaseSensitive ? "Yes" : "No";
        }
    }
}

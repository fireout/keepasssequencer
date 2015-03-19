using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using WordSequence.Configuration;

namespace WordSequence.Forms
{
    public partial class SubstitutionListControl : UserControl
    {
        public SubstitutionListControl()
        {
            InitializeComponent();
            lvSubstitutions.DoubleClick += new EventHandler(lvSubstitutions_DoubleClick);
        }

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
                    }
                }
            }
        }

        public IEnumerable<BaseSubstitution> Substitutions { get; set; }

        private Dictionary<ListViewItem, BaseSubstitution> ListViewMapping { get; set; }

        public void DataBind()
        {
            ListViewMapping = new Dictionary<ListViewItem, BaseSubstitution>();

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

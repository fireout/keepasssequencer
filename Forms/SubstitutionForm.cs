using System;
using System.Windows.Forms;
using Sequencer.Configuration;
using Sequencer.Configuration.Model;

namespace Sequencer.Forms
{
    public partial class SubstitutionForm : Form
    {
        public SubstitutionForm()
        {
            InitializeComponent();
            ddoMethod.Items.Add(new ListItem(1, "Anything"));
            ddoMethod.Items.Add(new ListItem(2, "Everything"));
            ddoMethod.SelectedIndex = 0;
        }

        public BaseSubstitution Substitution { get; set; }

        private void ReadSubstitution()
        {
            if (Substitution != null)
            {
                if (Substitution is AnySubstitution)
                    ListItem.SetSelectedItem(ddoMethod, 1);
                else
                    ListItem.SetSelectedItem(ddoMethod, 2);

                txtReplace.Text = Substitution.Replace;
                txtWith.Text = Substitution.With;
                ckCaseSensitive.Checked = Substitution.CaseSensitive;
            }
            else
            {
                ddoMethod.SelectedValue = 1;
                txtReplace.Text = string.Empty;
                txtWith.Text = string.Empty;
                ckCaseSensitive.Checked = false;
            }
        }

        private void WriteSubstitution()
        {
            BaseSubstitution substitution;
            if (((ListItem)ddoMethod.SelectedItem).Value == 1)
                substitution = new AnySubstitution();
            else
                substitution = new WholeSubstitution();

            substitution.Replace = txtReplace.Text;
            substitution.With = txtWith.Text;
            substitution.CaseSensitive = ckCaseSensitive.Checked;

            Substitution = substitution;

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ReadSubstitution();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            WriteSubstitution();
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

    }
}

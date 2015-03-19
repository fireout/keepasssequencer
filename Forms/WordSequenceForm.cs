using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WordSequence
{
    public partial class WordSequenceForm : Form
    {
        private WordSequenceOption _options;
        public WordSequenceOption Options
        {
            get
            {
                if (_options == null)
                {
                    _options = WordSequenceOption.CreateDefaults();
                    ReadOptions();
                }
                return _options;
            }
            set
            {
                _options = value;
                ReadOptions();
            }
        }

        public WordSequenceForm()
        {
            InitializeComponent();
            InitializeComboBoxes();
        }

        private void InitializeComboBoxes()
        {
            try
            {
                cboWordCount.Items.Add(new ListItem(1));
                cboWordCount.Items.Add(new ListItem(2));
                cboWordCount.Items.Add(new ListItem(3));
                cboWordCount.Items.Add(new ListItem(4));
                cboWordCount.Items.Add(new ListItem(5));
                cboWordCount.Items.Add(new ListItem(6));
                cboWordCount.Items.Add(new ListItem(7));
                cboWordCount.Items.Add(new ListItem(8));
                cboWordCount.Items.Add(new ListItem(9));

            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error initializing combo 'cboWordCount'.\n{0}", ex.ToString()), "Error initializing combo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                cboExtraCount.Items.Add(new ListItem(0, "Never"));
                cboExtraCount.Items.Add(new ListItem(1));
                cboExtraCount.Items.Add(new ListItem(2));
                cboExtraCount.Items.Add(new ListItem(3));
                cboExtraCount.Items.Add(new ListItem(4));
                cboExtraCount.Items.Add(new ListItem(5));
                cboExtraCount.Items.Add(new ListItem(6));
                cboExtraCount.Items.Add(new ListItem(7));
                cboExtraCount.Items.Add(new ListItem(8));
                cboExtraCount.Items.Add(new ListItem(9));
                cboExtraCount.Items.Add(new ListItem(10, "Always"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error initializing combo 'cboExtraCount'.\n{0}", ex.ToString()), "Error initializing combo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                cboSubstitutionCount.Items.Add(new ListItem(0, "Never"));
                cboSubstitutionCount.Items.Add(new ListItem(1));
                cboSubstitutionCount.Items.Add(new ListItem(2));
                cboSubstitutionCount.Items.Add(new ListItem(3));
                cboSubstitutionCount.Items.Add(new ListItem(4));
                cboSubstitutionCount.Items.Add(new ListItem(5));
                cboSubstitutionCount.Items.Add(new ListItem(6));
                cboSubstitutionCount.Items.Add(new ListItem(7));
                cboSubstitutionCount.Items.Add(new ListItem(8));
                cboSubstitutionCount.Items.Add(new ListItem(9));
                cboSubstitutionCount.Items.Add(new ListItem(10, "Always"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error initializing combo 'cboSubstitutionCount'.\n{0}", ex.ToString()), "Error initializing combo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                cboCapitalization.Items.Add(new ListItem(0, "Never"));
                cboCapitalization.Items.Add(new ListItem(-1, "All Except First"));
                cboCapitalization.Items.Add(new ListItem(-2, "Only at First"));
                cboCapitalization.Items.Add(new ListItem(1));
                cboCapitalization.Items.Add(new ListItem(2));
                cboCapitalization.Items.Add(new ListItem(3));
                cboCapitalization.Items.Add(new ListItem(4));
                cboCapitalization.Items.Add(new ListItem(5));
                cboCapitalization.Items.Add(new ListItem(6));
                cboCapitalization.Items.Add(new ListItem(7));
                cboCapitalization.Items.Add(new ListItem(8));
                cboCapitalization.Items.Add(new ListItem(9));
                cboCapitalization.Items.Add(new ListItem(10, "Always"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error initializing combo 'cboCapitalization'.\n{0}", ex.ToString()), "Error initializing combo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void ReadOptions()
        {
            try
            {
                txtExtras.Text = new string(Options.ExtraChars);

            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error parsing extras.\n{0}", ex.ToString()), "Error parsing extras", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            try
            {
                txtSubstitutionFrom.Text = string.Join("\r\n", Options.SubstitutionFrom);
                txtSubstitutionTo.Text = string.Join("\r\n", Options.SubstitutionTo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error parsing substitutions.\n{0}", ex.ToString()), "Error parsing substitutions", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                txtWordSelection.Text = string.Join("\r\n", Options.WordSelection);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error parsing word selection.\n{0}", ex.ToString()), "Error parsing word selection", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            SetSelectedItem(cboWordCount, Options.WordCount);
            SetSelectedItem(cboExtraCount, Options.ExtraCount);
            SetSelectedItem(cboSubstitutionCount, Options.SubstitutionCount);
            SetSelectedItem(cboCapitalization, Options.Capitalize);
        }

        private void SetSelectedItem(ComboBox combo, int value)
        {
            try
            {
                foreach (ListItem item in combo.Items)
                    if (item.Value == value)
                    {
                        combo.SelectedItem = item;
                        return;
                    }

            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error parsing combo '{0}'.\n{1}", combo.Name, ex.ToString()), "Error parsing combo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void WriteOptions()
        {
            try
            {
                Options.ExtraChars = txtExtras.Text.ToCharArray();

                Options.SubstitutionFrom = txtSubstitutionFrom.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                Options.SubstitutionTo = txtSubstitutionTo.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                Options.WordSelection = txtWordSelection.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                Options.WordCount = ((ListItem)cboWordCount.SelectedItem).Value;
                Options.ExtraCount = ((ListItem)cboExtraCount.SelectedItem).Value;
                Options.SubstitutionCount = ((ListItem)cboSubstitutionCount.SelectedItem).Value;
                Options.Capitalize = ((ListItem)cboCapitalization.SelectedItem).Value;

            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error writing options.\n{0}", ex.ToString()), "Error writing options", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            WriteOptions();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

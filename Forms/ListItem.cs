using System;
using System.Windows.Forms;

namespace Sequencer.Forms
{
    public class ListItem
    {
        public ListItem(int value, string text)
        {
            Value = value;
            Text = text;
        }
        public ListItem(int value)
            : this(value, value.ToString())
        {

        }

        public int Value { get; private set; }
        public string Text { get; private set; }
        public override string ToString()
        {
            return Text;
        }

        public override bool Equals(object obj)
        {
            if (obj is ListItem)
                return ((ListItem)obj).Value == this.Value;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static implicit operator int(ListItem value)
        {
            return value.Value;
        }

        public static void SetSelectedItem(ComboBox combo, int value)
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
    }
}

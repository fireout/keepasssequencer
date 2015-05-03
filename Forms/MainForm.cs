using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Sequencer.Configuration;

namespace Sequencer.Forms
{
    public class MainForm : Form
    {
        private SplitContainer splitContainer1;
        private Label label3;
        private TextBox txtCharacterList;
        private Label label2;
        private TextBox txtWordList;
        private Label label1;
        private ListView lvSequence;
        private ToolStripLabel toolStripLabel1;
        private ToolStripDropDownButton toolStripButton1;
        private ToolStripButton tsbDelete;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton tsbEdit;
        private ToolStripDropDownButton tsbOptions;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton tsbUp;
        private ToolStripButton tsbDown;
        private ToolStrip toolStrip1;
        private ToolStripMenuItem wordToolStripMenuItem;
        private ToolStripMenuItem charactersToolStripMenuItem;
        private ToolStripMenuItem probabilityToolStripMenuItem;
        private ToolStripMenuItem capitalizeToolStripMenuItem;
        private ToolStripMenuItem substitutionToolStripMenuItem;
        private ToolStripMenuItem lengthToolStripMenuItem;
        private ToolStripMenuItem lengthStrengthToolStripMenuItem;
        private ToolStripMenuItem includeDefaultsToolStripMenuItem;
        private ToolStrip toolStrip2;
        private ToolStripButton toolStripButton2;
        private ToolStripButton tsbDeleteSubstitution;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader4;
        private SubstitutionListControl substitutionList1;

        public MainForm()
        {
            InitializeComponent();
        }

        public PasswordSequenceConfiguration Configuration { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Configuration = new WordSequence.Sequencer().Load();

            LoadConfigurationDetails();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            new WordSequence.Sequencer().Save(Configuration);
        }

        private void UpdateConfigurationSubstitutions()
        {
            Configuration.DefaultSubstitutions.Clear();
            Configuration.DefaultSubstitutions.AddRange(substitutionList1.Substitutions);
        }


        private void LoadConfigurationDetails()
        {
            txtWordList.Text = Configuration.DefaultWords.ToString();
            txtCharacterList.Text = Configuration.DefaultCharacters.ToString();
            substitutionList1.Substitutions = Configuration.DefaultSubstitutions;
            substitutionList1.DataBind();

            SequenceItem lastSelectedItem = GetSelectedSequenceItem<SequenceItem>();
            lvSequence.Items.Clear();

            foreach (SequenceItem sequenceItem in Configuration.Sequence)
            {
                ListViewItem listItem = new ListViewItem();

                listItem.SubItems.Add(sequenceItem.Probability.ToString());
                listItem.Tag = sequenceItem;

                if (sequenceItem is CharacterSequenceItem)
                {
                    CharacterSequenceItem characterSequenceItem = (CharacterSequenceItem)sequenceItem;
                    listItem.Text = "Characters";
                    listItem.SubItems.Add(string.Format("Length: {0} ({1}), Override: {2}",
                        characterSequenceItem.Length,
                        characterSequenceItem.LengthStrength.ToString(),
                        characterSequenceItem.Characters != null ? characterSequenceItem.Characters.Override.ToString().ToLower() : "false"));
                    listItem.SubItems.Add(characterSequenceItem.Characters != null ? characterSequenceItem.Characters.ToString() : "(Defaults)");
                }
                else if (sequenceItem is WordSequenceItem)
                {
                    WordSequenceItem wordSequenceItem = (WordSequenceItem)sequenceItem;
                    listItem.Text = "Word";
                    listItem.SubItems.Add(string.Format("Capitalize: {0}, Substitution: {1}, Override: {2}",
                        wordSequenceItem.Capitalize.ToString(),
                        wordSequenceItem.Substitution.ToString(),
                        wordSequenceItem.Words != null ? wordSequenceItem.Words.Override.ToString().ToLower() : "false"));
                    listItem.SubItems.Add(wordSequenceItem.Words != null ? wordSequenceItem.Words.ToString() : "(Defaults)");
                }
                if (sequenceItem == lastSelectedItem)
                    listItem.Selected = true;
                lvSequence.Items.Add(listItem);
            }
            UpdateToolstipButtons();
            UpdateSubstitutionToolbar();
        }

        private void wordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Configuration.Sequence.Add(new WordSequenceItem());
            LoadConfigurationDetails();
        }

        private void characterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Configuration.Sequence.Add(new CharacterSequenceItem());
            LoadConfigurationDetails();

        }

        private void lvSequence_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateToolstipButtons();
        }

        private void UpdateToolstipButtons()
        {
            bool allowItemEditing = lvSequence.SelectedItems.Count > 0;

            int selectedIndex = allowItemEditing ? lvSequence.SelectedIndices.OfType<int>().Single() : 0;
            tsbUp.Enabled = allowItemEditing && selectedIndex > 0;
            tsbDown.Enabled = allowItemEditing && selectedIndex < lvSequence.Items.Count - 1;
            tsbOptions.Enabled = allowItemEditing;
            tsbEdit.Enabled = allowItemEditing;

            bool isWordItem = GetSelectedSequenceItem<WordSequenceItem>() != null;
            substitutionToolStripMenuItem.Visible = isWordItem;
            capitalizeToolStripMenuItem.Visible = isWordItem;
            lengthStrengthToolStripMenuItem.Visible = !isWordItem;
            lengthToolStripMenuItem.Visible = !isWordItem;
        }

        private void tsbDown_Click(object sender, EventArgs e)
        {
            SequenceItem targetItem = lvSequence.SelectedItems.Count > 0 ? GetSelectedSequenceItem<SequenceItem>() : null;
            if (targetItem != null)
            {
                int sourceIndex = Configuration.Sequence.IndexOf(targetItem);
                int destinationIndex = sourceIndex + 1;
                Configuration.Sequence[sourceIndex] = Configuration.Sequence[destinationIndex];
                Configuration.Sequence[destinationIndex] = targetItem;
            }
            LoadConfigurationDetails();
        }

        private void tsbUp_Click(object sender, EventArgs e)
        {
            SequenceItem targetItem = lvSequence.SelectedItems.Count > 0 ? GetSelectedSequenceItem<SequenceItem>() : null;
            if (targetItem != null)
            {
                int sourceIndex = Configuration.Sequence.IndexOf(targetItem);
                int destinationIndex = sourceIndex - 1;
                Configuration.Sequence[sourceIndex] = Configuration.Sequence[destinationIndex];
                Configuration.Sequence[destinationIndex] = targetItem;
            }
            LoadConfigurationDetails();
        }

        public bool TryGetUserInput(string prompt, string regex, out string result, string defaultResult)
        {
            result = Microsoft.VisualBasic.Interaction.InputBox(prompt, DefaultResponse: defaultResult);
            return true;
        }

        private T GetSelectedSequenceItem<T>() where T : SequenceItem
        {
            if (lvSequence.SelectedItems.Count == 0)
                return default(T);
            return lvSequence.SelectedItems.OfType<ListViewItem>().Single().Tag as T;
        }

        private void tsbEdit_Click(object sender, EventArgs e)
        {
            SequenceItem item = GetSelectedSequenceItem<SequenceItem>();
            WordSequenceItem wordItem = item as WordSequenceItem;
            if (wordItem != null)
            {
                string wordList;
                if (TryGetUserInput("Enter the word list", "/w+", out wordList, wordItem.Words != null ? string.Join(" ", wordItem.Words.ToArray()) : ""))
                {
                    if (wordList == string.Empty)
                        wordItem.Words = null;
                    else
                    {
                        if (wordItem.Words == null)
                            wordItem.Words = new OverridingWordList();
                        wordItem.Words.Clear();
                        wordItem.Words.AddRange(wordList.Split(' '));
                    }
                }
                LoadConfigurationDetails();
                return;
            }
            CharacterSequenceItem charItem = item as CharacterSequenceItem;
            if (charItem != null)
            {
                string charList;
                if (TryGetUserInput("Enter the char list", ".+", out charList, charItem.Characters != null ? string.Join(" ", charItem.Characters.Select(c => c.ToString()).ToArray()) : ""))
                {
                    if (charList == string.Empty)
                        charItem.Characters = null;
                    else
                    {
                        if (charItem.Characters == null)
                            charItem.Characters = new OverridingCharacterList();
                        charItem.Characters.Clear();
                        charItem.Characters.AddRange(charList.ToArray());
                    }
                }
                LoadConfigurationDetails();
            }
        }

        private void ReadUserInputFor<TItem, TEnum>(string dialogPrompt, string regex, Expression<Func<TItem, TEnum>> sequenceItemPropFunc)
            where TItem : SequenceItem
            where TEnum : struct
        {
            TItem item = GetSelectedSequenceItem<TItem>();
            string userValue;
            if (item != null && TryGetUserInput(dialogPrompt, regex, out userValue, sequenceItemPropFunc.Compile()(item).ToString()))
            {
                TEnum value;
                PropertyInfo[] propertyInfo = PropertyHelper<TItem>.GetProperties(sequenceItemPropFunc);
                TypeConverter converter;
                if (typeof(TEnum).IsEnum && Enum.TryParse(userValue, out value))
                {
                    SetExpressionValue(item, propertyInfo, value);
                }
                else if ((converter = TypeDescriptor.GetConverter(typeof(TEnum))).CanConvertFrom(typeof(string)))
                    SetExpressionValue(item, propertyInfo, converter.ConvertFrom(userValue));
                LoadConfigurationDetails();
            }
        }

        private static void SetExpressionValue<TItem, TEnum>(TItem item, PropertyInfo[] propertyInfo, TEnum value) where TItem : SequenceItem
        {
            object cursor = item;
            foreach (PropertyInfo prop in propertyInfo.Take(propertyInfo.Count() - 1))
            {
                if (cursor == null)
                    return;
                cursor = prop.GetValue(cursor, null);
            }
            propertyInfo.Last().SetValue(cursor, value, null);
        }

        public static class PropertyHelper<T>
        {
            public static PropertyInfo[] GetProperties<TValue>(
                Expression<Func<T, TValue>> selector)
            {
                List<PropertyInfo> navProperties = new List<PropertyInfo>();
                Expression body = selector;
                while (body is LambdaExpression)
                    body = ((LambdaExpression)body).Body;
                do
                {
                    switch (body.NodeType)
                    {
                        case ExpressionType.MemberAccess:
                            MemberExpression memberExpression = (MemberExpression)body;
                            navProperties.Insert(0, (PropertyInfo)memberExpression.Member);
                            body = memberExpression.Expression;
                            break;
                        case ExpressionType.Parameter:
                            body = null;
                            break;
                        default:
                            throw new InvalidOperationException();
                    }

                } while (body != null);
                return navProperties.ToArray();
                //if (body is LambdaExpression)
                //{
                //    body = ((LambdaExpression)body).Body;
                //    while (body is LambdaExpression)
                //    {

                //    }
                //}
                //switch (body.NodeType)
                //{
                //    case ExpressionType.MemberAccess:
                //        return new[] { (PropertyInfo)((MemberExpression)body).Member };
                //        break;
                //    default:
                //        throw new InvalidOperationException();
                //}
            }
        }

        private void probabilityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReadUserInputFor<SequenceItem, PercentEnum>("Probability [Never|Always|1-99]", "[Never|Always|1-99]", i => i.Probability);
        }

        private void capitalizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReadUserInputFor<WordSequenceItem, CapitalizeEnum>("Capitalize [Never|Proper|Always|1-99]", "[Never|Proper|Always|1-99]", i => i.Capitalize);
        }

        private void substitutionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReadUserInputFor<WordSequenceItem, PercentEnum>("Substitution [Never|Always|1-99]", "[Never|Always|1-99]", i => i.Substitution);
        }

        private void lengthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReadUserInputFor<CharacterSequenceItem, byte>("Length [1-255]", "[1-255]", i => i.Length);
        }

        private void lengthStrengthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReadUserInputFor<CharacterSequenceItem, StrengthEnum>("Length Strength [1-99|Full]", "[1-99|Full]", i => i.LengthStrength);
        }

        private void includeDefaultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ReadUserInputFor<CharacterSequenceItem, bool>("Include default characters [true|false]", "[true|false]", i => i.Characters.Override);
                ReadUserInputFor<WordSequenceItem, bool>("Include default words [true|false]", "[true|false]", i => i.Words.Override);

            }
            catch (NullReferenceException)
            {

            }
        }

        private void txtCharacterList_TextChanged(object sender, EventArgs e)
        {
            CharacterList charList = new CharacterList();
            charList.AddRange(txtCharacterList.Text.ToArray());
            Configuration.DefaultCharacters = charList;
        }

        private void txtWordList_TextChanged(object sender, EventArgs e)
        {
            WordList wordList = new WordList();
            wordList.AddRange(txtWordList.Text.Split(' '));
            Configuration.DefaultWords = wordList;
        }

        private void substitutionList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSubstitutionToolbar();
        }

        private void UpdateSubstitutionToolbar()
        {
            tsbDeleteSubstitution.Enabled = substitutionList1.SelectedItem != null;
        }

        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label3 = new System.Windows.Forms.Label();
            this.txtCharacterList = new System.Windows.Forms.TextBox();
            this.txtWordList = new System.Windows.Forms.TextBox();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.tsbDeleteSubstitution = new System.Windows.Forms.ToolStripButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lvSequence = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.wordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.charactersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsbDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbEdit = new System.Windows.Forms.ToolStripButton();
            this.tsbOptions = new System.Windows.Forms.ToolStripDropDownButton();
            this.probabilityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.capitalizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.substitutionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lengthToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lengthStrengthToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.includeDefaultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbUp = new System.Windows.Forms.ToolStripButton();
            this.tsbDown = new System.Windows.Forms.ToolStripButton();
            this.substitutionList1 = new Sequencer.Forms.SubstitutionListControl();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.txtCharacterList);
            this.splitContainer1.Panel1.Controls.Add(this.txtWordList);
            this.splitContainer1.Panel1.Controls.Add(this.toolStrip2);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lvSequence);
            this.splitContainer1.Panel2.Controls.Add(this.toolStrip1);
            this.splitContainer1.Size = new System.Drawing.Size(997, 545);
            this.splitContainer1.SplitterDistance = 266;
            this.splitContainer1.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 147);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Substitutions";
            // 
            // txtCharacterList
            // 
            this.txtCharacterList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCharacterList.Location = new System.Drawing.Point(12, 100);
            this.txtCharacterList.Multiline = true;
            this.txtCharacterList.Name = "txtCharacterList";
            this.txtCharacterList.Size = new System.Drawing.Size(973, 44);
            this.txtCharacterList.TabIndex = 3;
            this.txtCharacterList.TextChanged += new System.EventHandler(this.txtCharacterList_TextChanged);
            // 
            // txtWordList
            // 
            this.txtWordList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWordList.Location = new System.Drawing.Point(13, 30);
            this.txtWordList.Multiline = true;
            this.txtWordList.Name = "txtWordList";
            this.txtWordList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtWordList.Size = new System.Drawing.Size(972, 51);
            this.txtWordList.TabIndex = 1;
            this.txtWordList.TextChanged += new System.EventHandler(this.txtWordList_TextChanged);
            // 
            // toolStrip2
            // 
            this.toolStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton2,
            this.tsbDeleteSubstitution});
            this.toolStrip2.Location = new System.Drawing.Point(83, 143);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(58, 25);
            this.toolStrip2.TabIndex = 6;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = global::Sequencer.Properties.Resources.icon_add;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton2.Text = "toolStripButton2";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // tsbDeleteSubstitution
            // 
            this.tsbDeleteSubstitution.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDeleteSubstitution.Image = global::Sequencer.Properties.Resources.delete_24;
            this.tsbDeleteSubstitution.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDeleteSubstitution.Name = "tsbDeleteSubstitution";
            this.tsbDeleteSubstitution.Size = new System.Drawing.Size(23, 22);
            this.tsbDeleteSubstitution.Text = "toolStripButton3";
            this.tsbDeleteSubstitution.Click += new System.EventHandler(this.tsbDeleteSubstitution_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Characters";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Words";
            // 
            // lvSequence
            // 
            this.lvSequence.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.lvSequence.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvSequence.FullRowSelect = true;
            this.lvSequence.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvSequence.Location = new System.Drawing.Point(0, 25);
            this.lvSequence.Name = "lvSequence";
            this.lvSequence.Size = new System.Drawing.Size(997, 250);
            this.lvSequence.TabIndex = 1;
            this.lvSequence.UseCompatibleStateImageBehavior = false;
            this.lvSequence.View = System.Windows.Forms.View.Details;
            this.lvSequence.SelectedIndexChanged += new System.EventHandler(this.lvSequence_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Type";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Probability";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Properties";
            this.columnHeader3.Width = 273;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Content";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripButton1,
            this.tsbDelete,
            this.toolStripSeparator1,
            this.tsbEdit,
            this.tsbOptions,
            this.toolStripSeparator2,
            this.tsbUp,
            this.tsbDown});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(997, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(58, 22);
            this.toolStripLabel1.Text = "Sequence";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wordToolStripMenuItem,
            this.charactersToolStripMenuItem});
            this.toolStripButton1.Image = global::Sequencer.Properties.Resources.icon_add;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(29, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            // 
            // wordToolStripMenuItem
            // 
            this.wordToolStripMenuItem.Name = "wordToolStripMenuItem";
            this.wordToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.wordToolStripMenuItem.Text = "Word";
            this.wordToolStripMenuItem.Click += new System.EventHandler(this.wordToolStripMenuItem_Click);
            // 
            // charactersToolStripMenuItem
            // 
            this.charactersToolStripMenuItem.Name = "charactersToolStripMenuItem";
            this.charactersToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.charactersToolStripMenuItem.Text = "Characters";
            this.charactersToolStripMenuItem.Click += new System.EventHandler(this.characterToolStripMenuItem_Click);
            // 
            // tsbDelete
            // 
            this.tsbDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDelete.Image = global::Sequencer.Properties.Resources.delete_24;
            this.tsbDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDelete.Name = "tsbDelete";
            this.tsbDelete.Size = new System.Drawing.Size(23, 22);
            this.tsbDelete.Text = "toolStripButton2";
            this.tsbDelete.Click += new System.EventHandler(this.tsbDelete_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbEdit
            // 
            this.tsbEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbEdit.Image = global::Sequencer.Properties.Resources.pencil_32;
            this.tsbEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbEdit.Name = "tsbEdit";
            this.tsbEdit.Size = new System.Drawing.Size(23, 22);
            this.tsbEdit.Text = "toolStripButton3";
            this.tsbEdit.Click += new System.EventHandler(this.tsbEdit_Click);
            // 
            // tsbOptions
            // 
            this.tsbOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.probabilityToolStripMenuItem,
            this.capitalizeToolStripMenuItem,
            this.substitutionToolStripMenuItem,
            this.lengthToolStripMenuItem,
            this.lengthStrengthToolStripMenuItem,
            this.includeDefaultsToolStripMenuItem});
            this.tsbOptions.Image = global::Sequencer.Properties.Resources.gear_32;
            this.tsbOptions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbOptions.Name = "tsbOptions";
            this.tsbOptions.Size = new System.Drawing.Size(29, 22);
            this.tsbOptions.Text = "toolStripDropDownButton1";
            // 
            // probabilityToolStripMenuItem
            // 
            this.probabilityToolStripMenuItem.Name = "probabilityToolStripMenuItem";
            this.probabilityToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.probabilityToolStripMenuItem.Text = "Probability";
            this.probabilityToolStripMenuItem.Click += new System.EventHandler(this.probabilityToolStripMenuItem_Click);
            // 
            // capitalizeToolStripMenuItem
            // 
            this.capitalizeToolStripMenuItem.Name = "capitalizeToolStripMenuItem";
            this.capitalizeToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.capitalizeToolStripMenuItem.Text = "Capitalize";
            this.capitalizeToolStripMenuItem.Click += new System.EventHandler(this.capitalizeToolStripMenuItem_Click);
            // 
            // substitutionToolStripMenuItem
            // 
            this.substitutionToolStripMenuItem.Name = "substitutionToolStripMenuItem";
            this.substitutionToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.substitutionToolStripMenuItem.Text = "Substitution";
            this.substitutionToolStripMenuItem.Click += new System.EventHandler(this.substitutionToolStripMenuItem_Click);
            // 
            // lengthToolStripMenuItem
            // 
            this.lengthToolStripMenuItem.Name = "lengthToolStripMenuItem";
            this.lengthToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.lengthToolStripMenuItem.Text = "Length";
            this.lengthToolStripMenuItem.Click += new System.EventHandler(this.lengthToolStripMenuItem_Click);
            // 
            // lengthStrengthToolStripMenuItem
            // 
            this.lengthStrengthToolStripMenuItem.Name = "lengthStrengthToolStripMenuItem";
            this.lengthStrengthToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.lengthStrengthToolStripMenuItem.Text = "LengthStrength";
            this.lengthStrengthToolStripMenuItem.Click += new System.EventHandler(this.lengthStrengthToolStripMenuItem_Click);
            // 
            // includeDefaultsToolStripMenuItem
            // 
            this.includeDefaultsToolStripMenuItem.Name = "includeDefaultsToolStripMenuItem";
            this.includeDefaultsToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.includeDefaultsToolStripMenuItem.Text = "Include Defaults";
            this.includeDefaultsToolStripMenuItem.Click += new System.EventHandler(this.includeDefaultsToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbUp
            // 
            this.tsbUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbUp.Image = global::Sequencer.Properties.Resources.up_32;
            this.tsbUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbUp.Name = "tsbUp";
            this.tsbUp.Size = new System.Drawing.Size(23, 22);
            this.tsbUp.Text = "tsbUp";
            this.tsbUp.Click += new System.EventHandler(this.tsbUp_Click);
            // 
            // tsbDown
            // 
            this.tsbDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDown.Image = global::Sequencer.Properties.Resources.down_32;
            this.tsbDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDown.Name = "tsbDown";
            this.tsbDown.Size = new System.Drawing.Size(23, 22);
            this.tsbDown.Text = "tsbDown";
            this.tsbDown.Click += new System.EventHandler(this.tsbDown_Click);
            // 
            // substitutionList1
            // 
            this.substitutionList1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.substitutionList1.Location = new System.Drawing.Point(13, 163);
            this.substitutionList1.Name = "substitutionList1";
            this.substitutionList1.Size = new System.Drawing.Size(973, 100);
            this.substitutionList1.Substitutions = null;
            this.substitutionList1.TabIndex = 5;
            this.substitutionList1.SelectedIndexChanged += new System.EventHandler(this.substitutionList1_SelectedIndexChanged);
            this.substitutionList1.SubstitutionChanged += new System.EventHandler(this.substitutionList1_SubstitutionChanged);
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(997, 545);
            this.Controls.Add(this.splitContainer1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.Text = "Sequence Setup";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        private void tsbDelete_Click(object sender, EventArgs e)
        {
            SequenceItem item = GetSelectedSequenceItem<SequenceItem>();
            if (item != null)
                Configuration.Sequence.Remove(item);
            LoadConfigurationDetails();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Configuration.DefaultSubstitutions.Add(new AnySubstitution());
            LoadConfigurationDetails();
        }

        private void tsbDeleteSubstitution_Click(object sender, EventArgs e)
        {
            if (substitutionList1.SelectedItem != null)
            {
                Configuration.DefaultSubstitutions.Remove(substitutionList1.SelectedSubstitution);
                LoadConfigurationDetails();
            }
        }

        private void substitutionList1_SubstitutionChanged(object sender, EventArgs e)
        {
            UpdateConfigurationSubstitutions();
        }
    }
}

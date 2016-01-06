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
using KeePassLib.Cryptography;

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
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader4;
        private Label passwordPreview;
        private global::Sequencer.Forms.SubstitutionListControl substitutionList1;
        private StrengthBar strengthBar;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem loadToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem defaultWordsToolStripMenuItem;
        private ToolStripMenuItem dicewareToolStripMenuItem;
        private ToolStripMenuItem bealeDicewareToolStripMenuItem;
        private ToolStripMenuItem newGeneralServiceListToolStripMenuItem;
        private ToolStripMenuItem top5000ToolStripMenuItem;
        private ToolStripMenuItem defaultCharactersToolStripMenuItem;
        private ToolStripMenuItem alphabetToolStripMenuItem;
        private ToolStripMenuItem numbersToolStripMenuItem;
        private ToolStripMenuItem specialCharactersToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem1;
        private Panel SponsorPanel;
        private LinkLabel lnkTop5k;
        private Label lblClose;
        private Label label6;

        private System.Timers.Timer wordlistUpdateTimer;
        public delegate void LoadConfigDelegate(bool loadTextFields);
        private void RefreshOnTimer(Object o, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                this.Invoke(new LoadConfigDelegate(LoadConfigurationDetails), new object[] { false });
            }
            catch (InvalidOperationException)
            {
                /* this is usually because we closed the form before the timer
                 * expired; if that's the case there is no reason to refresh.
                 */
            }
        }

        public MainForm()
        {
            InitializeComponent();

            wordlistUpdateTimer = new System.Timers.Timer(2500);
            wordlistUpdateTimer.AutoReset = false;
            wordlistUpdateTimer.Elapsed += this.RefreshOnTimer;
        }

        public PasswordSequenceConfiguration Configuration { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Configuration == null)
            {
                Configuration = new Sequencer().Load();
            }

            LoadConfigurationDetails(true);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            new Sequencer().Save(Configuration);
        }

        private void UpdateConfigurationSubstitutions()
        {
            Configuration.DefaultSubstitutions.Clear();
            Configuration.DefaultSubstitutions.AddRange(substitutionList1.Substitutions);
            LoadConfigurationDetails();
        }


        private void LoadConfigurationDetails(bool loadTextFields = false)
        {
            if (loadTextFields)
            {
                txtWordList.Text = Configuration.DefaultWords.ToString();
                txtCharacterList.Text = Configuration.DefaultCharacters.ToString();
            }
            substitutionList1.Substitutions = Configuration.DefaultSubstitutions;
            substitutionList1.DataBind();

            SequenceItem lastSelectedItem = GetSelectedSequenceItem<SequenceItem>();
            lvSequence.Items.Clear();

            /* Get a decent randomizer for samples in case the user actually uses
             * these for some reason...
             * This was copied from KeePass code which also throws in some
             * additional entropy but for the sample we don't care.
             */
            byte[] pbKey = CryptoRandom.Instance.GetRandomBytes(256);
            CryptoRandomRange randomizer = new CryptoRandomRange(
                        new CryptoRandomStream(CrsAlgorithm.Salsa20, pbKey));

            Sequencer sequencer = new Sequencer();

            double entropy = 0;

            foreach (SequenceItem sequenceItem in Configuration.Sequence)
            {
                entropy += sequenceItem.entropy(Configuration);

                ListViewItem listItem = new ListViewItem();

                listItem.Tag = sequenceItem;

                for (int i = 1; i <= 3; i += 1)
                {
                    listItem.SubItems.Add(string.Format(
                                sequencer.GenerateSequenceItem(sequenceItem,
                                    Configuration,
                                    randomizer)));
                }

                string itemText = "";

                if (sequenceItem is CharacterSequenceItem)
                {
                    itemText += "Characters";
                }
                else if (sequenceItem is WordSequenceItem)
                {
                    itemText += "Word";
                }

                if (sequenceItem.Probability == PercentEnum.Never)
                {
                    listItem.Font = new Font(listItem.Font, listItem.Font.Style | FontStyle.Strikeout);
                    listItem.ForeColor = System.Drawing.Color.Gray;
                }
                else if (sequenceItem.Probability < PercentEnum.Always)
                {
                    listItem.Font = new Font(listItem.Font, listItem.Font.Style | FontStyle.Italic);
                    itemText += " (" + sequenceItem.Probability.ToString() + "%)";
                }
                listItem.Text = itemText;

                if (sequenceItem == lastSelectedItem)
                {
                    listItem.Selected = true;
                }
                lvSequence.Items.Add(listItem);
            }
            passwordPreview.Text = string.Format(sequencer.GenerateSequence(Configuration, randomizer));

            strengthBar.Value = Math.Min((int)entropy, strengthBar.Maximum);

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
                    if (wordItem.Words == null)
                    {
                        wordItem.Words = new OverridingWordList();
                    }
                    wordItem.Words.Clear();
                    if (wordList != string.Empty)
                    {
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
                    if (charItem.Characters == null)
                    {
                        charItem.Characters = new OverridingCharacterList();
                    }
                    charItem.Characters.Clear();
                    if (charList != string.Empty)
                    {
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
            if (item != null && TryGetUserInput(dialogPrompt, regex, out userValue, sequenceItemPropFunc.Compile()(item).ToString()) && userValue != "")
            {
                TEnum? value;
                PropertyInfo[] propertyInfo = PropertyHelper<TItem>.GetProperties(sequenceItemPropFunc);
                TypeConverter converter;
                if (typeof(TEnum).IsEnum && (value = Enum.Parse(typeof(TEnum), userValue) as TEnum?) != null)
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
            ReadUserInputFor<CharacterSequenceItem, uint>("Length [1-255]", "[1-255]", i => i.Length);
        }

        private void lengthStrengthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReadUserInputFor<CharacterSequenceItem, StrengthEnum>("Length Strength [1-99|Full]", "[1-99|Full]", i => i.LengthStrength);
        }

        private void includeDefaultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ReadUserInputFor<CharacterSequenceItem, bool>("Omit default characters [true|false]", "[true|false]", i => i.Characters.Override);
                ReadUserInputFor<WordSequenceItem, bool>("Omit default words [true|false]", "[true|false]", i => i.Words.Override);
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

            wordlistUpdateTimer.Stop();
            wordlistUpdateTimer.Start();
        }

        private void txtWordList_TextChanged(object sender, EventArgs e)
        {
            WordList wordList = new WordList();
            wordList.AddRange(txtWordList.Text.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
            Configuration.DefaultWords = wordList;

            wordlistUpdateTimer.Stop();
            wordlistUpdateTimer.Start();
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
            System.Windows.Forms.Label label4;
            System.Windows.Forms.Label label5;
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.SponsorPanel = new System.Windows.Forms.Panel();
            this.lblClose = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lnkTop5k = new System.Windows.Forms.LinkLabel();
            this.substitutionList1 = new SubstitutionListControl();
            this.label3 = new System.Windows.Forms.Label();
            this.txtCharacterList = new System.Windows.Forms.TextBox();
            this.txtWordList = new System.Windows.Forms.TextBox();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.tsbDeleteSubstitution = new System.Windows.Forms.ToolStripButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.passwordPreview = new System.Windows.Forms.Label();
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
            this.strengthBar = new StrengthBar();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.defaultWordsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dicewareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bealeDicewareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newGeneralServiceListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.top5000ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.defaultCharactersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alphabetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.numbersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.specialCharactersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SponsorPanel.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label4
            // 
            label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            label4.AutoSize = true;
            label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label4.Location = new System.Drawing.Point(4, 265);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(67, 20);
            label4.TabIndex = 2;
            label4.Text = "Preview:";
            // 
            // label5
            // 
            label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            label5.AutoSize = true;
            label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label5.Location = new System.Drawing.Point(4, 293);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(75, 20);
            label5.TabIndex = 5;
            label5.Text = "Strength:";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 12);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.SponsorPanel);
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
            this.splitContainer1.Panel2.Controls.Add(this.strengthBar);
            this.splitContainer1.Panel2.Controls.Add(label5);
            this.splitContainer1.Panel2.Controls.Add(this.passwordPreview);
            this.splitContainer1.Panel2.Controls.Add(label4);
            this.splitContainer1.Panel2.Controls.Add(this.lvSequence);
            this.splitContainer1.Panel2.Controls.Add(this.toolStrip1);
            this.splitContainer1.Size = new System.Drawing.Size(836, 649);
            this.splitContainer1.SplitterDistance = 327;
            this.splitContainer1.TabIndex = 0;
            // 
            // SponsorPanel
            // 
            this.SponsorPanel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.SponsorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SponsorPanel.Controls.Add(this.lblClose);
            this.SponsorPanel.Controls.Add(this.label6);
            this.SponsorPanel.Controls.Add(this.lnkTop5k);
            this.SponsorPanel.Location = new System.Drawing.Point(286, 15);
            this.SponsorPanel.Name = "SponsorPanel";
            this.SponsorPanel.Size = new System.Drawing.Size(234, 110);
            this.SponsorPanel.TabIndex = 9;
            this.SponsorPanel.Visible = false;
            // 
            // lblClose
            // 
            this.lblClose.AutoSize = true;
            this.lblClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblClose.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblClose.Location = new System.Drawing.Point(200, 93);
            this.lblClose.Name = "lblClose";
            this.lblClose.Size = new System.Drawing.Size(33, 13);
            this.lblClose.TabIndex = 2;
            this.lblClose.Text = "Close";
            this.lblClose.Click += new System.EventHandler(this.lblClose_Click);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(3, 4);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(230, 43);
            this.label6.TabIndex = 1;
            this.label6.Text = "More information about the top 5000 most used English words can be found at";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lnkTop5k
            // 
            this.lnkTop5k.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lnkTop5k.Location = new System.Drawing.Point(3, 47);
            this.lnkTop5k.Name = "lnkTop5k";
            this.lnkTop5k.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.lnkTop5k.Size = new System.Drawing.Size(230, 21);
            this.lnkTop5k.TabIndex = 0;
            this.lnkTop5k.TabStop = true;
            this.lnkTop5k.Text = "http://www.wordfrequency.info/";
            this.lnkTop5k.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lnkTop5k.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkTop5k_LinkClicked);
            // 
            // substitutionList1
            // 
            this.substitutionList1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.substitutionList1.Location = new System.Drawing.Point(13, 197);
            this.substitutionList1.Name = "substitutionList1";
            this.substitutionList1.Size = new System.Drawing.Size(812, 127);
            this.substitutionList1.Substitutions = null;
            this.substitutionList1.TabIndex = 5;
            this.substitutionList1.SelectedIndexChanged += new System.EventHandler(this.substitutionList1_SelectedIndexChanged);
            this.substitutionList1.SubstitutionChanged += new System.EventHandler(this.substitutionList1_SubstitutionChanged);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 181);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Substitutions";
            // 
            // txtCharacterList
            // 
            this.txtCharacterList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCharacterList.Location = new System.Drawing.Point(12, 110);
            this.txtCharacterList.Multiline = true;
            this.txtCharacterList.Name = "txtCharacterList";
            this.txtCharacterList.Size = new System.Drawing.Size(812, 68);
            this.txtCharacterList.TabIndex = 3;
            this.txtCharacterList.TextChanged += new System.EventHandler(this.txtCharacterList_TextChanged);
            // 
            // txtWordList
            // 
            this.txtWordList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWordList.Location = new System.Drawing.Point(13, 30);
            this.txtWordList.Multiline = true;
            this.txtWordList.Name = "txtWordList";
            this.txtWordList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtWordList.Size = new System.Drawing.Size(811, 61);
            this.txtWordList.TabIndex = 1;
            this.txtWordList.TextChanged += new System.EventHandler(this.txtWordList_TextChanged);
            // 
            // toolStrip2
            // 
            this.toolStrip2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.toolStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton2,
            this.tsbDeleteSubstitution});
            this.toolStrip2.Location = new System.Drawing.Point(79, 177);
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
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 94);
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
            // passwordPreview
            // 
            this.passwordPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.passwordPreview.AutoSize = true;
            this.passwordPreview.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.passwordPreview.Location = new System.Drawing.Point(82, 268);
            this.passwordPreview.Name = "passwordPreview";
            this.passwordPreview.Size = new System.Drawing.Size(255, 16);
            this.passwordPreview.TabIndex = 3;
            this.passwordPreview.Text = "Password Preview Goes Here At Runtime";
            // 
            // lvSequence
            // 
            this.lvSequence.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvSequence.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.lvSequence.FullRowSelect = true;
            this.lvSequence.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvSequence.Location = new System.Drawing.Point(12, 28);
            this.lvSequence.Name = "lvSequence";
            this.lvSequence.Size = new System.Drawing.Size(812, 231);
            this.lvSequence.TabIndex = 1;
            this.lvSequence.UseCompatibleStateImageBehavior = false;
            this.lvSequence.View = System.Windows.Forms.View.Details;
            this.lvSequence.SelectedIndexChanged += new System.EventHandler(this.lvSequence_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Type";
            this.columnHeader1.Width = 133;
            // 
            // columnHeader2
            // 
            this.columnHeader2.DisplayIndex = 2;
            this.columnHeader2.Text = "Sample";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader2.Width = 136;
            // 
            // columnHeader3
            // 
            this.columnHeader3.DisplayIndex = 1;
            this.columnHeader3.Text = "Sample";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader3.Width = 136;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Sample";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader4.Width = 136;
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
            this.toolStrip1.Size = new System.Drawing.Size(836, 25);
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
            // strengthBar
            // 
            this.strengthBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.strengthBar.Location = new System.Drawing.Point(85, 290);
            this.strengthBar.MarqueeAnimationSpeed = 1000;
            this.strengthBar.Maximum = 128;
            this.strengthBar.Name = "strengthBar";
            this.strengthBar.Size = new System.Drawing.Size(509, 23);
            this.strengthBar.TabIndex = 6;
            this.strengthBar.Value = 46;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(836, 24);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.defaultWordsToolStripMenuItem,
            this.defaultCharactersToolStripMenuItem,
            this.toolStripMenuItem1,
            this.loadToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // defaultWordsToolStripMenuItem
            // 
            this.defaultWordsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dicewareToolStripMenuItem,
            this.bealeDicewareToolStripMenuItem,
            this.newGeneralServiceListToolStripMenuItem,
            this.top5000ToolStripMenuItem});
            this.defaultWordsToolStripMenuItem.Name = "defaultWordsToolStripMenuItem";
            this.defaultWordsToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.defaultWordsToolStripMenuItem.Text = "Default Words";
            // 
            // dicewareToolStripMenuItem
            // 
            this.dicewareToolStripMenuItem.Name = "dicewareToolStripMenuItem";
            this.dicewareToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.dicewareToolStripMenuItem.Text = "Diceware";
            this.dicewareToolStripMenuItem.Click += new System.EventHandler(this.dicewareToolStripMenuItem_Click);
            // 
            // bealeDicewareToolStripMenuItem
            // 
            this.bealeDicewareToolStripMenuItem.Name = "bealeDicewareToolStripMenuItem";
            this.bealeDicewareToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.bealeDicewareToolStripMenuItem.Text = "Beale Diceware";
            this.bealeDicewareToolStripMenuItem.Click += new System.EventHandler(this.bealeDicewareToolStripMenuItem_Click);
            // 
            // newGeneralServiceListToolStripMenuItem
            // 
            this.newGeneralServiceListToolStripMenuItem.Name = "newGeneralServiceListToolStripMenuItem";
            this.newGeneralServiceListToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.newGeneralServiceListToolStripMenuItem.Text = "New General Service List";
            this.newGeneralServiceListToolStripMenuItem.Click += new System.EventHandler(this.newGeneralServiceListToolStripMenuItem_Click);
            // 
            // top5000ToolStripMenuItem
            // 
            this.top5000ToolStripMenuItem.Name = "top5000ToolStripMenuItem";
            this.top5000ToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.top5000ToolStripMenuItem.Text = "Top 5000 English words";
            this.top5000ToolStripMenuItem.Click += new System.EventHandler(this.top5000ToolStripMenuItem_Click);
            // 
            // defaultCharactersToolStripMenuItem
            // 
            this.defaultCharactersToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.alphabetToolStripMenuItem,
            this.numbersToolStripMenuItem,
            this.specialCharactersToolStripMenuItem});
            this.defaultCharactersToolStripMenuItem.Name = "defaultCharactersToolStripMenuItem";
            this.defaultCharactersToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.defaultCharactersToolStripMenuItem.Text = "Default Characters";
            // 
            // alphabetToolStripMenuItem
            // 
            this.alphabetToolStripMenuItem.Name = "alphabetToolStripMenuItem";
            this.alphabetToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.alphabetToolStripMenuItem.Text = "Alphabet";
            this.alphabetToolStripMenuItem.Click += new System.EventHandler(this.alphabetToolStripMenuItem_Click);
            // 
            // numbersToolStripMenuItem
            // 
            this.numbersToolStripMenuItem.Name = "numbersToolStripMenuItem";
            this.numbersToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.numbersToolStripMenuItem.Text = "Numbers";
            this.numbersToolStripMenuItem.Click += new System.EventHandler(this.numbersToolStripMenuItem_Click);
            // 
            // specialCharactersToolStripMenuItem
            // 
            this.specialCharactersToolStripMenuItem.Name = "specialCharactersToolStripMenuItem";
            this.specialCharactersToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.specialCharactersToolStripMenuItem.Text = "Special Characters";
            this.specialCharactersToolStripMenuItem.Click += new System.EventHandler(this.specialCharactersToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(168, 6);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.loadToolStripMenuItem.Text = "Load";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(836, 661);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.splitContainer1);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(525, 700);
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.Text = "Sequence Setup";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.SponsorPanel.ResumeLayout(false);
            this.SponsorPanel.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string result;
            if (TryGetUserInput("Enter a configuration name, leave empty for default", @"\d*", out result, null))
            {
                Configuration = new Sequencer().Load(result);
            }
            else
            {
                MessageBox.Show("Invalid configuration name");
            }

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string result;
            if (TryGetUserInput("Enter a configuration name, leave empty for default", @"\d*", out result, null))
            {
                Configuration.Name = result;
                new Sequencer().Save(Configuration);
            }
            else
            {
                MessageBox.Show("Invalid configuration name");
            }
        }

        private void alphabetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtCharacterList.SelectedText += "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            txtCharacterList_TextChanged(null, null);
        }

        private void numbersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtCharacterList.SelectedText += "0123456789";
            txtCharacterList_TextChanged(null, null);
        }

        private void specialCharactersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtCharacterList.SelectedText = @"|\`!""/$%?&*()_+-=[]{}~­´,.;:";
            txtCharacterList_TextChanged(null, null);
        }

        private void AppendWordListFromProfile(string profile)
        {
            PasswordSequenceConfiguration tempConfiguration = new Sequencer().Load(profile);
            if (tempConfiguration != null)
            {
                string insertedWords = tempConfiguration.DefaultWords.ToString();
                txtWordList.SelectedText = insertedWords;
                txtWordList_TextChanged(null, null);
            }
        }

        private void dicewareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppendWordListFromProfile("diceware");
        }

        private void bealeDicewareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppendWordListFromProfile("beale-diceware");
        }

        private void newGeneralServiceListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppendWordListFromProfile("ngsl");
        }

        private void top5000ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppendWordListFromProfile("top5k");
            if (!lnkTop5k.LinkVisited)
                SponsorPanel.Visible = true;
        }

        private void lblClose_Click(object sender, EventArgs e)
        {
            SponsorPanel.Visible = false;
        }

        private void lnkTop5k_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            lnkTop5k.LinkVisited = true;
            System.Diagnostics.Process.Start("http://www.wordfrequency.info/");
        }


    }
}

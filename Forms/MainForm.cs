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
using Sequencer.Configuration.Model;
using KeePassLib.Cryptography;

namespace Sequencer.Forms
{
    public class MainForm : Form
    {
        private SplitContainer splitContainer1;
        private Label label3;
        private TextBox txtCharacterList;
        private Label lblCharacters;
        private TextBox txtWordList;
        private Label lblWords;
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
        private SubstitutionListControl substitutionList1;
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
        private ListView listPreviews;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem fromFileToolStripMenuItem;
        private ToolStripMenuItem optionsToolStripMenuItem;
        internal ToolStripMenuItem advancedModeToolStripMenuItem;
        private ToolStripMenuItem newGeneralServiceListbaseWordsToolStripMenuItem;

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

        public MainForm(PasswordSequenceConfiguration configuration)
            : this()
        {
            Configuration = configuration;
        }

        public PasswordSequenceConfiguration Configuration { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Configuration == null)
            {
                Configuration = new Sequencer().Load();
            }

            if (Sequencer.GetAdvancedOptionRequired(Configuration))
            {
                advancedModeToolStripMenuItem.Checked = true;
                advancedModeToolStripMenuItem_Click(advancedModeToolStripMenuItem, new EventArgs());
            }

            UpdateWindowTitle();

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
            listPreviews.Items.Clear();
            for (int i = 0; i < 50; i++)
            {
                listPreviews.Items.Add(sequencer.GenerateSequence(Configuration, randomizer));
            }
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
            lengthStrengthToolStripMenuItem.Visible = advancedModeToolStripMenuItem.Checked && !isWordItem;
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
                WordListEdit listEditor = new WordListEdit((wordItem.Words != null ? string.Join(" ", wordItem.Words.ToArray()) : ""), this);
                listEditor.ShowDialog();

                if (listEditor.DialogResult == DialogResult.OK)
                {
                    string wordList = listEditor.ReturnVal;
                    if (wordItem.Words == null)
                    {
                        wordItem.Words = new OverridingWordList();
                    }
                    wordItem.Words.Clear();
                    if (wordList != string.Empty)
                    {
                        wordItem.Words.AddRange(wordList.Split(' '));
                    }
                    LoadConfigurationDetails();
                }
                return;
            }
            CharacterSequenceItem charItem = item as CharacterSequenceItem;
            if (charItem != null)
            {
                string charList;
                if (TryGetUserInput("Enter the character override list. Cancel to CLEAR THE LIST.", ".+", out charList, charItem.Characters != null ? string.Join(" ", charItem.Characters.Select(c => c.ToString()).ToArray()) : ""))
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
                if (typeof(TEnum).IsEnum && TryParse(userValue, out value))
                {
                    SetExpressionValue(item, propertyInfo, value);
                }
                else if ((converter = TypeDescriptor.GetConverter(typeof(TEnum))).CanConvertFrom(typeof(string)))
                    SetExpressionValue(item, propertyInfo, converter.ConvertFrom(userValue));
                LoadConfigurationDetails();
            }
        }

        private bool TryParse<TEnum>(string value, out TEnum? result)
            where TEnum : struct
        {
            result = null;
            string[] enumValues = Enum.GetNames(typeof(TEnum));
            foreach (string enumValue in enumValues)
            {
                if (string.Equals(value, enumValue, StringComparison.InvariantCultureIgnoreCase))
                {
                    result = (TEnum)Enum.Parse(typeof(TEnum), enumValue, true);
                    return true;
                }
            }
            return false;

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

        private void tsbOptions_Click(object sender, EventArgs e)
        {
            try
            {
                SequenceItem item = GetSelectedSequenceItem<SequenceItem>();
                WordSequenceItem wordItem = item as WordSequenceItem;
                if (wordItem != null)
                {
                    includeDefaultsToolStripMenuItem.Checked = !wordItem.Words.Override;
                }
                else
                {
                    CharacterSequenceItem charItem = item as CharacterSequenceItem;
                    if (charItem != null)
                    {
                        includeDefaultsToolStripMenuItem.Checked = !charItem.Characters.Override;
                    }
                }
            }
            catch (NullReferenceException)
            {
                includeDefaultsToolStripMenuItem.Checked = false;
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
                SequenceItem item = GetSelectedSequenceItem<SequenceItem>();
                WordSequenceItem wordItem = item as WordSequenceItem;
                if (wordItem != null)
                {
                    wordItem.Words.Override = !wordItem.Words.Override;
                    includeDefaultsToolStripMenuItem.Checked = !wordItem.Words.Override;
                }
                else
                {
                    CharacterSequenceItem charItem = item as CharacterSequenceItem;
                    if (charItem != null)
                    {
                        charItem.Characters.Override = !charItem.Characters.Override;
                        includeDefaultsToolStripMenuItem.Checked = !charItem.Characters.Override;
                    }
                }
                LoadConfigurationDetails();
            }
            catch (NullReferenceException)
            {

            }
        }

        private void txtCharacterList_TextChanged(object sender, EventArgs e)
        {
            CharacterList charList = new CharacterList();
            charList.AddRange(txtCharacterList.Text.ToArray());
            lblCharacters.Text = string.Format(lblCharacters.Tag.ToString(), charList.Count);
            Configuration.DefaultCharacters = charList;

            RefitTextEntry();

            wordlistUpdateTimer.Stop();
            wordlistUpdateTimer.Start();
        }

        private void txtWordList_TextChanged(object sender, EventArgs e)
        {
            WordList wordList = new WordList();
            wordList.AddRange(txtWordList.Text.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
            lblWords.Text = string.Format(lblWords.Tag.ToString(), wordList.Count);
            Configuration.DefaultWords = wordList;

            RefitTextEntry();

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
            this.lblCharacters = new System.Windows.Forms.Label();
            this.lblWords = new System.Windows.Forms.Label();
            this.listPreviews = new System.Windows.Forms.ListView();
            this.strengthBar = new StrengthBar();
            this.lvSequence = new System.Windows.Forms.ListView();
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.defaultWordsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dicewareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bealeDicewareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newGeneralServiceListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.top5000ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.fromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.defaultCharactersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alphabetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.numbersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.specialCharactersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newGeneralServiceListbaseWordsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            // label5
            // 
            label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            label5.AutoSize = true;
            label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label5.Location = new System.Drawing.Point(117, 149);
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
            this.splitContainer1.Panel1.Controls.Add(this.lblCharacters);
            this.splitContainer1.Panel1.Controls.Add(this.lblWords);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listPreviews);
            this.splitContainer1.Panel2.Controls.Add(this.strengthBar);
            this.splitContainer1.Panel2.Controls.Add(label5);
            this.splitContainer1.Panel2.Controls.Add(this.lvSequence);
            this.splitContainer1.Panel2.Controls.Add(this.toolStrip1);
            this.splitContainer1.Size = new System.Drawing.Size(509, 451);
            this.splitContainer1.SplitterDistance = 273;
            this.splitContainer1.TabIndex = 0;
            // 
            // SponsorPanel
            // 
            this.SponsorPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.SponsorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SponsorPanel.Controls.Add(this.lblClose);
            this.SponsorPanel.Controls.Add(this.label6);
            this.SponsorPanel.Controls.Add(this.lnkTop5k);
            this.SponsorPanel.Location = new System.Drawing.Point(143, 148);
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
            this.substitutionList1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.substitutionList1.Location = new System.Drawing.Point(272, 30);
            this.substitutionList1.Name = "substitutionList1";
            this.substitutionList1.Size = new System.Drawing.Size(234, 240);
            this.substitutionList1.Substitutions = null;
            this.substitutionList1.TabIndex = 5;
            this.substitutionList1.SelectedIndexChanged += new System.EventHandler(this.substitutionList1_SelectedIndexChanged);
            this.substitutionList1.SubstitutionChanged += new System.EventHandler(this.substitutionList1_SubstitutionChanged);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(269, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Substitutions";
            // 
            // txtCharacterList
            // 
            this.txtCharacterList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCharacterList.Location = new System.Drawing.Point(8, 195);
            this.txtCharacterList.Multiline = true;
            this.txtCharacterList.Name = "txtCharacterList";
            this.txtCharacterList.Size = new System.Drawing.Size(262, 75);
            this.txtCharacterList.TabIndex = 3;
            this.txtCharacterList.TextChanged += new System.EventHandler(this.txtCharacterList_TextChanged);
            this.txtCharacterList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextboxSelectText);
            // 
            // txtWordList
            // 
            this.txtWordList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWordList.Location = new System.Drawing.Point(8, 30);
            this.txtWordList.Multiline = true;
            this.txtWordList.Name = "txtWordList";
            this.txtWordList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtWordList.Size = new System.Drawing.Size(262, 146);
            this.txtWordList.TabIndex = 1;
            this.txtWordList.TextChanged += new System.EventHandler(this.txtWordList_TextChanged);
            this.txtWordList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextboxSelectText);
            // 
            // toolStrip2
            // 
            this.toolStrip2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.toolStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton2,
            this.tsbDeleteSubstitution});
            this.toolStrip2.Location = new System.Drawing.Point(350, 9);
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
            // lblCharacters
            // 
            this.lblCharacters.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCharacters.AutoSize = true;
            this.lblCharacters.Location = new System.Drawing.Point(3, 179);
            this.lblCharacters.Name = "lblCharacters";
            this.lblCharacters.Size = new System.Drawing.Size(95, 13);
            this.lblCharacters.TabIndex = 2;
            this.lblCharacters.Tag = "Default Characters ({0})";
            this.lblCharacters.Text = "Default Characters";
            // 
            // lblWords
            // 
            this.lblWords.AutoSize = true;
            this.lblWords.Location = new System.Drawing.Point(3, 15);
            this.lblWords.Name = "lblWords";
            this.lblWords.Size = new System.Drawing.Size(75, 13);
            this.lblWords.TabIndex = 0;
            this.lblWords.Tag = "Default Words ({0})";
            this.lblWords.Text = "Default Words";
            // 
            // listPreviews
            // 
            this.listPreviews.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listPreviews.Location = new System.Drawing.Point(194, 28);
            this.listPreviews.Name = "listPreviews";
            this.listPreviews.Size = new System.Drawing.Size(312, 119);
            this.listPreviews.TabIndex = 6;
            this.listPreviews.UseCompatibleStateImageBehavior = false;
            this.listPreviews.View = System.Windows.Forms.View.List;
            // 
            // strengthBar
            // 
            this.strengthBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.strengthBar.Location = new System.Drawing.Point(194, 150);
            this.strengthBar.MarqueeAnimationSpeed = 1000;
            this.strengthBar.Maximum = 128;
            this.strengthBar.Name = "strengthBar";
            this.strengthBar.Size = new System.Drawing.Size(312, 20);
            this.strengthBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.strengthBar.TabIndex = 4;
            this.strengthBar.Value = 46;
            // 
            // lvSequence
            // 
            this.lvSequence.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lvSequence.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvSequence.FullRowSelect = true;
            this.lvSequence.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvSequence.LabelWrap = false;
            this.lvSequence.Location = new System.Drawing.Point(1, 28);
            this.lvSequence.MultiSelect = false;
            this.lvSequence.Name = "lvSequence";
            this.lvSequence.ShowGroups = false;
            this.lvSequence.Size = new System.Drawing.Size(191, 119);
            this.lvSequence.TabIndex = 1;
            this.lvSequence.UseCompatibleStateImageBehavior = false;
            this.lvSequence.View = System.Windows.Forms.View.List;
            this.lvSequence.SelectedIndexChanged += new System.EventHandler(this.lvSequence_SelectedIndexChanged);
            this.lvSequence.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvSequence_MouseDoubleClick);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.toolStrip1.Size = new System.Drawing.Size(509, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(64, 22);
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
            this.toolStripButton1.Text = "Add";
            this.toolStripButton1.ToolTipText = "Add sequence item";
            // 
            // wordToolStripMenuItem
            // 
            this.wordToolStripMenuItem.Name = "wordToolStripMenuItem";
            this.wordToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.wordToolStripMenuItem.Text = "Word";
            this.wordToolStripMenuItem.ToolTipText = "Add word item";
            this.wordToolStripMenuItem.Click += new System.EventHandler(this.wordToolStripMenuItem_Click);
            // 
            // charactersToolStripMenuItem
            // 
            this.charactersToolStripMenuItem.Name = "charactersToolStripMenuItem";
            this.charactersToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.charactersToolStripMenuItem.Text = "Characters";
            this.charactersToolStripMenuItem.ToolTipText = "Add characters item";
            this.charactersToolStripMenuItem.Click += new System.EventHandler(this.characterToolStripMenuItem_Click);
            // 
            // tsbDelete
            // 
            this.tsbDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDelete.Image = global::Sequencer.Properties.Resources.delete_24;
            this.tsbDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDelete.Name = "tsbDelete";
            this.tsbDelete.Size = new System.Drawing.Size(23, 22);
            this.tsbDelete.Text = "Delete";
            this.tsbDelete.ToolTipText = "Delete selected item";
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
            this.tsbEdit.Text = "Edit";
            this.tsbEdit.ToolTipText = "Edit current item";
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
            this.tsbOptions.Text = "Options";
            this.tsbOptions.ToolTipText = "Configure Item";
            this.tsbOptions.Click += new System.EventHandler(this.tsbOptions_Click);
            // 
            // probabilityToolStripMenuItem
            // 
            this.probabilityToolStripMenuItem.Name = "probabilityToolStripMenuItem";
            this.probabilityToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.probabilityToolStripMenuItem.Text = "Probability *";
            this.probabilityToolStripMenuItem.Visible = false;
            this.probabilityToolStripMenuItem.Click += new System.EventHandler(this.probabilityToolStripMenuItem_Click);
            // 
            // capitalizeToolStripMenuItem
            // 
            this.capitalizeToolStripMenuItem.Name = "capitalizeToolStripMenuItem";
            this.capitalizeToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.capitalizeToolStripMenuItem.Text = "Capitalize";
            this.capitalizeToolStripMenuItem.Click += new System.EventHandler(this.capitalizeToolStripMenuItem_Click);
            // 
            // substitutionToolStripMenuItem
            // 
            this.substitutionToolStripMenuItem.Name = "substitutionToolStripMenuItem";
            this.substitutionToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.substitutionToolStripMenuItem.Text = "Substitution";
            this.substitutionToolStripMenuItem.Click += new System.EventHandler(this.substitutionToolStripMenuItem_Click);
            // 
            // lengthToolStripMenuItem
            // 
            this.lengthToolStripMenuItem.Name = "lengthToolStripMenuItem";
            this.lengthToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.lengthToolStripMenuItem.Text = "Length";
            this.lengthToolStripMenuItem.Click += new System.EventHandler(this.lengthToolStripMenuItem_Click);
            // 
            // lengthStrengthToolStripMenuItem
            // 
            this.lengthStrengthToolStripMenuItem.Name = "lengthStrengthToolStripMenuItem";
            this.lengthStrengthToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.lengthStrengthToolStripMenuItem.Text = "LengthStrength *";
            this.lengthStrengthToolStripMenuItem.Visible = false;
            this.lengthStrengthToolStripMenuItem.Click += new System.EventHandler(this.lengthStrengthToolStripMenuItem_Click);
            // 
            // includeDefaultsToolStripMenuItem
            // 
            this.includeDefaultsToolStripMenuItem.Name = "includeDefaultsToolStripMenuItem";
            this.includeDefaultsToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
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
            this.tsbUp.Text = "Move Up";
            this.tsbUp.ToolTipText = "Move Item Up";
            this.tsbUp.Click += new System.EventHandler(this.tsbUp_Click);
            // 
            // tsbDown
            // 
            this.tsbDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDown.Image = global::Sequencer.Properties.Resources.down_32;
            this.tsbDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDown.Name = "tsbDown";
            this.tsbDown.Size = new System.Drawing.Size(23, 22);
            this.tsbDown.Text = "Move Down";
            this.tsbDown.ToolTipText = "Move Item Down";
            this.tsbDown.Click += new System.EventHandler(this.tsbDown_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(509, 24);
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
            this.newGeneralServiceListbaseWordsToolStripMenuItem,
            this.newGeneralServiceListToolStripMenuItem,
            this.top5000ToolStripMenuItem,
            this.toolStripSeparator3,
            this.fromFileToolStripMenuItem});
            this.defaultWordsToolStripMenuItem.Name = "defaultWordsToolStripMenuItem";
            this.defaultWordsToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.defaultWordsToolStripMenuItem.Text = "Default Words";
            // 
            // dicewareToolStripMenuItem
            // 
            this.dicewareToolStripMenuItem.Name = "dicewareToolStripMenuItem";
            this.dicewareToolStripMenuItem.Size = new System.Drawing.Size(298, 22);
            this.dicewareToolStripMenuItem.Text = "Diceware";
            this.dicewareToolStripMenuItem.Click += new System.EventHandler(this.dicewareToolStripMenuItem_Click);
            // 
            // bealeDicewareToolStripMenuItem
            // 
            this.bealeDicewareToolStripMenuItem.Name = "bealeDicewareToolStripMenuItem";
            this.bealeDicewareToolStripMenuItem.Size = new System.Drawing.Size(298, 22);
            this.bealeDicewareToolStripMenuItem.Text = "Beale Diceware";
            this.bealeDicewareToolStripMenuItem.Click += new System.EventHandler(this.bealeDicewareToolStripMenuItem_Click);
            // 
            // newGeneralServiceListToolStripMenuItem
            // 
            this.newGeneralServiceListToolStripMenuItem.Name = "newGeneralServiceListToolStripMenuItem";
            this.newGeneralServiceListToolStripMenuItem.Size = new System.Drawing.Size(298, 22);
            this.newGeneralServiceListToolStripMenuItem.Text = "New General Service List (all word forms)";
            this.newGeneralServiceListToolStripMenuItem.Click += new System.EventHandler(this.newGeneralServiceListToolStripMenuItem_Click);
            // 
            // top5000ToolStripMenuItem
            // 
            this.top5000ToolStripMenuItem.Name = "top5000ToolStripMenuItem";
            this.top5000ToolStripMenuItem.Size = new System.Drawing.Size(298, 22);
            this.top5000ToolStripMenuItem.Text = "Top 5000 English words";
            this.top5000ToolStripMenuItem.Click += new System.EventHandler(this.top5000ToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(295, 6);
            // 
            // fromFileToolStripMenuItem
            // 
            this.fromFileToolStripMenuItem.Name = "fromFileToolStripMenuItem";
            this.fromFileToolStripMenuItem.Size = new System.Drawing.Size(298, 22);
            this.fromFileToolStripMenuItem.Text = "From file...";
            this.fromFileToolStripMenuItem.ToolTipText = "Add words from a text file of your choice";
            this.fromFileToolStripMenuItem.Click += new System.EventHandler(this.fromFileToolStripMenuItem_Click);
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
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.advancedModeToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // advancedModeToolStripMenuItem
            // 
            this.advancedModeToolStripMenuItem.CheckOnClick = true;
            this.advancedModeToolStripMenuItem.Name = "advancedModeToolStripMenuItem";
            this.advancedModeToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.advancedModeToolStripMenuItem.Text = "Advanced Mode";
            this.advancedModeToolStripMenuItem.Click += new System.EventHandler(this.advancedModeToolStripMenuItem_Click);
            // 
            // newGeneralServiceListbaseWordsToolStripMenuItem
            // 
            this.newGeneralServiceListbaseWordsToolStripMenuItem.Name = "newGeneralServiceListbaseWordsToolStripMenuItem";
            this.newGeneralServiceListbaseWordsToolStripMenuItem.Size = new System.Drawing.Size(298, 22);
            this.newGeneralServiceListbaseWordsToolStripMenuItem.Text = "New General Service List (base words only)";
            this.newGeneralServiceListbaseWordsToolStripMenuItem.Click += new System.EventHandler(this.newGeneralServiceListbaseWordsToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(509, 464);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.splitContainer1);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(525, 500);
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "Sequence Setup ({0})";
            this.Text = "Sequence Setup";
            this.Shown += new System.EventHandler(this.MainForm_Shown);
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

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string result;
            if (TryGetUserInput("Enter a configuration name, leave empty for default", @"\d*", out result, null))
            {
                Configuration.Name = result;
                new Sequencer().Save(Configuration);
                UpdateWindowTitle();
            }
            else
            {
                MessageBox.Show("Invalid configuration name");
            }
        }

        private void UpdateWindowTitle()
        {
            if (Configuration != null)
                this.Text = string.Format(this.Tag.ToString(), Configuration.Name);
            else
                this.Text = string.Format(this.Tag.ToString(), string.Empty);
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

        private void AppendWordListFromProfile(string stringResource)
        {
            PasswordSequenceConfiguration tempConfiguration = new PasswordSequenceConfigurationFactory().LoadFromResource(stringResource);
            if (tempConfiguration != null)
            {
                AppendWordList(tempConfiguration.DefaultWords.ToString());
            }
        }

        private void AppendWordList(string wordList)
        {
            txtWordList.SelectedText = wordList;
            txtWordList_TextChanged(null, null);
        }

        private void dicewareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppendWordList(global::Sequencer.Properties.Resources.diceware);
        }

        private void bealeDicewareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppendWordList(global::Sequencer.Properties.Resources.bealeDiceware);
        }

        private void newGeneralServiceListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppendWordList(global::Sequencer.Properties.Resources.ngsl);
        }

        private void newGeneralServiceListbaseWordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppendWordList(global::Sequencer.Properties.Resources.ngsl_headwords);
        }

        private void top5000ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppendWordList(global::Sequencer.Properties.Resources.top5k);
            if (!lnkTop5k.LinkVisited)
            {
                ShowAttribution();
            }
        }

        private void fromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Text files (*.txt)|*.txt|All files|*";
            fileDialog.Title = "Get all words from file";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string allWords = System.IO.File.ReadAllText(fileDialog.FileName).ToLower();

                char[] nonWordChars = { ' ', '\r', '\n', '\t', '.', '!', '?', ',', ':', ';', '"', '(', ')', '<', '>', '=', '/', '\\' };
                string[] newWords = allWords.Split(nonWordChars, StringSplitOptions.RemoveEmptyEntries);

                if (newWords.Length > 0)
                {
                    System.Array.Sort(newWords);
                    newWords = newWords.Distinct().ToArray();

                    AppendWordList(System.String.Join(" ", newWords));
                }
            }
        }

        public void ShowAttribution()
        {
            SponsorPanel.Visible = true;
        }

        public bool AttributionWasViewed()
        {
            return lnkTop5k.LinkVisited;
        }

        public void SetAttributionViewed()
        {
            lnkTop5k.LinkVisited = true;
        }

        private void lblClose_Click(object sender, EventArgs e)
        {
            SponsorPanel.Visible = false;
        }

        private void lnkTop5k_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SetAttributionViewed();
            System.Diagnostics.Process.Start("http://www.wordfrequency.info/");
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            loadToolStripMenuItem.DropDownItems.Clear();

            var factory = new PasswordSequenceConfigurationFactory();
            foreach (string config in factory.ListConfigurationFiles())
            {
                try
                {
                    ToolStripItem configItem = new ToolStripMenuItem();
                    configItem.Tag = config;
                    PasswordSequenceConfiguration psconfig = factory.LoadFromFile(config);
                    if (psconfig == null)
                        continue;
                    if (!string.IsNullOrEmpty(psconfig.Name))
                        configItem.Text = psconfig.Name;
                    else
                        configItem.Text = "<Default>";

                    configItem.Click += LoadFoundTemplate;

                    loadToolStripMenuItem.DropDownItems.Add(configItem);
                }
                catch
                {

                }
            }
        }

        void LoadFoundTemplate(object sender, EventArgs e)
        {
            ToolStripItem item = sender as ToolStripItem;
            if (item != null)
            {
                Configuration = new PasswordSequenceConfigurationFactory().LoadFromFile(item.Tag.ToString());
                UpdateWindowTitle();
                LoadConfigurationDetails(true);
            }
        }

        private void TextboxSelectText(object sender, KeyEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            if (textbox != null && e.Control && e.KeyCode == Keys.A)
            {
                textbox.SelectAll();
            }
        }

        private void advancedModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (advancedModeToolStripMenuItem.Checked && (Sequencer.advancedOptionsEnabled == true ||
                Sequencer.AdvancedOptionsDialog("Configuring password sequence using the advanced mode can result in the password being weaker that what is displaied by the strength bar. " +
                "Toggling advanced mode will add options (marked with *) in the options menu for each sequence item") != System.Windows.Forms.DialogResult.OK))
            {
                advancedModeToolStripMenuItem.Checked = false;
            }

            probabilityToolStripMenuItem.Visible = advancedModeToolStripMenuItem.Checked;

            bool isWordItem = GetSelectedSequenceItem<WordSequenceItem>() != null;
            lengthStrengthToolStripMenuItem.Visible = !isWordItem && advancedModeToolStripMenuItem.Checked;
        }

        private void RefitTextEntry()
        {
            while (txtWordList.MaxLength <= txtWordList.Text.Length)
            {
                txtWordList.MaxLength = txtWordList.MaxLength * 3 / 2;
            }

            while (txtCharacterList.MaxLength <= txtCharacterList.Text.Length)
            {
                txtCharacterList.MaxLength = txtCharacterList.MaxLength * 3 / 2;
            }
        }

        private void lvSequence_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            tsbEdit_Click(sender, e);
        }
    }
}

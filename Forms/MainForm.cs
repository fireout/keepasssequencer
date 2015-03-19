using System;
using System.Windows.Forms;
using WordSequence.Configuration;

namespace WordSequence.Forms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public PasswordSequenceConfiguration Configuration { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Configuration = new Sequencer().Load();

            LoadConfigurationDetails();
        }


        private void LoadConfigurationDetails()
        {
            txtWordList.Text = Configuration.DefaultWords.ToString();
            txtCharacterList.Text = Configuration.DefaultCharacters.ToString();
            substitutionList1.Substitutions = Configuration.DefaultSubstitutions;
            substitutionList1.DataBind();

            lvSequence.Items.Clear();

            foreach (SequenceItem sequenceItem in Configuration.Sequence)
            {
                ListViewItem listItem = new ListViewItem();

                listItem.SubItems.Add(sequenceItem.Probability.ToString());

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

                lvSequence.Items.Add(listItem);
            }
        }
    }
}

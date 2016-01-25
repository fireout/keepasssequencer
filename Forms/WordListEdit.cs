using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sequencer.Forms
{
    public partial class WordListEdit : Form
    {
        public WordListEdit(string currentWords, MainForm parentForm)
        {
            InitializeComponent();
            wordList.Text = currentWords;
            ReturnVal = currentWords;
            this.parentForm = parentForm;
        }

        public string ReturnVal { get; private set; }

        private void RefitTextEntry()
        {
            while (wordList.MaxLength <= wordList.Text.Length)
            {
                wordList.MaxLength = wordList.MaxLength * 3 / 2;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            ReturnVal = wordList.Text;
        }

        private void addDicewareButton_Click(object sender, EventArgs e)
        {
            wordList.SelectedText = global::Sequencer.Properties.Resources.diceware;
            RefitTextEntry();
        }

        private void addAltDicewareButton_Click(object sender, EventArgs e)
        {
            wordList.SelectedText = global::Sequencer.Properties.Resources.bealeDiceware;
            RefitTextEntry();
        }

        private void addNgslButton_Click(object sender, EventArgs e)
        {
            wordList.SelectedText = global::Sequencer.Properties.Resources.ngsl;
            RefitTextEntry();
        }

        private void addTop5kButton_Click(object sender, EventArgs e)
        {
            wordList.SelectedText = global::Sequencer.Properties.Resources.top5k;
            RefitTextEntry();
            if (!parentForm.AttributionWasViewed())
            {
                parentForm.ShowAttribution();
            }
        }

        private void addTxtFileButton_Click(object sender, EventArgs e)
        {
        
            RefitTextEntry();
        }

        private MainForm parentForm;
    }
}

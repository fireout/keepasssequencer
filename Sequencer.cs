using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using KeePassLib;
using KeePassLib.Cryptography;
using KeePassLib.Cryptography.PasswordGenerator;
using KeePassLib.Security;
using Sequencer.Configuration;
using Sequencer.Forms;
using Sequencer.Configuration.Model;

namespace Sequencer
{
    public class Sequencer : CustomPwGenerator
    {
        internal ConfigurationFactory ConfigurationFactory { get; private set; }
        internal ConfigurationPathProvider ConfigurationPathProvider { get; private set; }
        internal Sequence.SequenceFactory SequenceFactory { get; private set; }
        internal ConfigurationWriter ConfigurationWriter { get; private set; }
        internal System.Configuration.Configuration AppConfiguration { get; private set; }

        public Sequencer()
        {
            AppConfiguration = System.Configuration.ConfigurationManager.OpenExeConfiguration(typeof(Sequencer).Assembly.Location);

            ConfigurationFactory = new ConfigurationFactory(AppConfiguration);
            this.SequenceFactory = new Sequence.SequenceFactory();
            ConfigurationWriter = new ConfigurationWriter();
            this.ConfigurationPathProvider = new ConfigurationPathProvider(AppConfiguration);
        }


        /// <summary>
        /// Loads a PasswordSequenceConfiguration configuration 
        /// </summary>
        /// <param name="profileName">The keepass profile name, that we will be using to construct the filename from, if specified</param>
        /// <returns></returns>
        public PasswordSequenceConfiguration Load(string profileName = null)
        {
            var path = ConfigurationPathProvider.GetUserFilePath(profileName);
            if (string.IsNullOrEmpty(path))
            {
                path = ConfigurationPathProvider.GetSystemFilePath(profileName); ;
            }

            if (!string.IsNullOrEmpty(path))
            {
                return ConfigurationFactory.LoadFromFile(path);
            }

            return null;
        }

        public void Save(PasswordSequenceConfiguration configuration)
        {
            if (Sequencer.GetAdvancedOptionRequired(configuration) && Sequencer.AdvancedOptionsDialog("Configuring password sequence using the advanced mode can result in the password being weaker that what is displaied by the strength bar. " +
                "Click Ok if you want to save the sequence anyway.") == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }

            string configFile = ConfigurationPathProvider.GetUserFilePath(configuration.Name);
            ConfigurationWriter.Write(configuration, configFile);
        }

        internal static bool GetAdvancedOptionRequired(PasswordSequenceConfiguration configuration)
        {
            bool usesAdvancedOptions = false;
            foreach (SequenceItem item in configuration.Sequence)
            {
                WordSequenceItem word = item as WordSequenceItem;
                if (word != null)
                {
                    usesAdvancedOptions = word.Probability < PercentEnum.Always;
                }
                CharacterSequenceItem characters = item as CharacterSequenceItem;
                if (characters != null)
                {
                    usesAdvancedOptions = characters.Probability < PercentEnum.Always || characters.LengthStrength < StrengthEnum.Full;
                }
                if (usesAdvancedOptions)
                    break;
            }
            return usesAdvancedOptions;
        }

        public override ProtectedString Generate(PwProfile prf, CryptoRandomStream crsRandomSource)
        {
            PasswordSequenceConfiguration config = Load(prf.CustomAlgorithmOptions);
            if (config == null)
                return null;

            return new ProtectedString(true, SequenceFactory.Generate(config, new CryptoRandomRange(crsRandomSource)));
        }

        public override string GetOptions(string strCurrentOptions)
        {
            advancedOptionsEnabled = null;
            PasswordSequenceConfiguration configuration = new Sequencer().Load(strCurrentOptions);
            if (configuration == null)
            {
                string userFilePath = ConfigurationPathProvider.GetUserFilePath();
                MessageBox.Show("An error occurred reading the Sequencer configuration file at " +
                                 userFilePath + ". It may be corrupt. Fix or delete and try again. " +
                                 "A default configuration has been loaded.",
                                 "Error Reading Configuration",
                                 MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            MainForm form = new MainForm(configuration ?? new PasswordSequenceConfiguration(true), this);
            form.ShowDialog();

            // this reset the need for a dialog
            if ((advancedOptionsRequired = GetAdvancedOptionRequired(form.Configuration)) != false)
            {
                advancedOptionsEnabled = null;
            }

            return form.Configuration.Name;
        }

        public override bool SupportsOptions
        {
            get
            {
                return true;
            }
        }

        private static readonly PwUuid UUID =
            new PwUuid(new byte[] {
                0x53, 0x81, 0x36, 0x0E, 0xA7, 0xFC, 0x48, 0x36,
                0x9E, 0x9F, 0xA4, 0x4F, 0x1A, 0xF0, 0x58, 0x37
            });
        public override PwUuid Uuid
        {
            get { return UUID; }
        }
        public override string Name
        {
            get { return "Sequencer"; }
        }

        internal static bool? advancedOptionsEnabled = null;
        static bool? advancedOptionsRequired = null;
        internal static DialogResult AdvancedOptionsDialog(string warningMessage)
        {
            return MessageBox.Show(warningMessage,
                "Advanced Mode Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
        }
    }
}

/* vim: set ts=4 sw=4 et: */

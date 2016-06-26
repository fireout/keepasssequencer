using System;
using System.Collections.Generic;
using System.IO;

namespace Sequencer.Configuration
{
    class ConfigurationPathProvider
    {
        private System.Configuration.Configuration sequencerConfiguration;

        public ConfigurationPathProvider(System.Configuration.Configuration configuration)
        {
            sequencerConfiguration = configuration;
        }

        public string GetSystemFilePath(string profileName = null)
        {
            string config = null;
            if (sequencerConfiguration.AppSettings.Settings["defaultConfigPath"] != null)
            {
                config = sequencerConfiguration.AppSettings.Settings["defaultConfigPath"].Value;
            }
            if (null == config && sequencerConfiguration.AppSettings.Settings["configPath"] != null)
            {
                config = sequencerConfiguration.AppSettings.Settings["configPath"].Value;
            }

            config = InsertProfileNameInPath(config, profileName);

            if (null != config && !File.Exists(config))
            {
                return System.IO.Path.GetFullPath(config);
            }
            else
            {
                return null; /* TODO: better to throw exception? */
            }

        }

        public string GetUserFilePath(string profileName = null)
        {
            return GetLocalUserFilePath(profileName, Environment.SpecialFolder.ApplicationData);
        }


        public string GetLocalUserFilePath(string profileName = null, Environment.SpecialFolder folder = Environment.SpecialFolder.LocalApplicationData)
        {
            string config = null;
            if (sequencerConfiguration.AppSettings.Settings["userConfigPath"] != null)
                config = sequencerConfiguration.AppSettings.Settings["userConfigPath"].Value;

            if (null != config)
            {
                config = InsertProfileNameInPath(config, profileName);
                if (!System.IO.Path.IsPathRooted(config))
                {
                    config = Path.Combine(
                        Environment.GetFolderPath(folder),
                        config);
                }
            }

            if (null == config)
            {
                config = Path.Combine(
                        Environment.GetFolderPath(folder),
                        "sequencer.xml");

                config = InsertProfileNameInPath(config, profileName);
            }

            if (null != config)
            {
                return System.IO.Path.GetFullPath(config);
            }
            else
            {
                return null; /* TODO: better to throw exception? */
            }

        }

        public ICollection<string> ListConfigurationFiles()
        {
            string path = GetUserFilePath();
            if (Directory.Exists(Path.GetDirectoryName(path)))
                return Directory.GetFiles(Path.GetDirectoryName(path), string.Format("{0}*{1}", Path.GetFileNameWithoutExtension(path), Path.GetExtension(path)));
            return new List<string>();
        }



        private string InsertProfileNameInPath(string path, string profileName)
        {
            string config = path;
            if (!string.IsNullOrEmpty(profileName))
            {
                string extension = Path.GetExtension(config);
                config = config.TrimEnd(extension.ToCharArray());
                config = string.Format("{0}.{1}{2}", config, profileName, extension);
            }
            return config;
        }



    }
}

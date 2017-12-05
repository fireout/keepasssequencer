using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Sequencer.Configuration.Model;

namespace Sequencer.Configuration
{
    class ConfigurationFactory
    {
        private System.Configuration.Configuration sequencerConfiguration;

        public ConfigurationFactory(System.Configuration.Configuration configuration)
        {
            sequencerConfiguration = configuration;
        }

        public PasswordSequenceConfiguration LoadFromResource(string resource)
        {
            if (resource == null)
                throw new Exception("resource must be a valid file resource");

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(stream))
                {
                    sw.Write(resource);
                    stream.Seek(0, SeekOrigin.Begin);

                    return LoadFromStream(stream);
                }
            }
        }

        public PasswordSequenceConfiguration LoadFromFile(string path)
        {
            if (path == null || path == string.Empty)
                throw new Exception("path must be a valid file path (but was empty)");

            PasswordSequenceConfiguration config = null;
            try
            {
                if (File.Exists(path))
                {
                    using (FileStream configStream = File.OpenRead(path))
                    {
                        config = LoadFromStream(configStream);
                    }
                }
            }
            catch (InvalidOperationException)
            {
                config = null;
            }
            return config;
        }

        private PasswordSequenceConfiguration LoadFromStream(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PasswordSequenceConfiguration),
                                                         "http://quasivirtuel.com/PasswordSequenceConfiguration.xsd");

            return (PasswordSequenceConfiguration)serializer.Deserialize(XmlReader.Create(stream));
        }
    }
}

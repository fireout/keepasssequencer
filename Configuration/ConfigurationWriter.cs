using System.IO;
using System.Xml.Serialization;
using Sequencer.Configuration.Model;

namespace Sequencer.Configuration
{
    class ConfigurationWriter
    {
        public void Write(PasswordSequenceConfiguration configuration, string path)
        {
            if (null != path)
            {
                XmlSerializer serializer =
                    new XmlSerializer(typeof(PasswordSequenceConfiguration),
                                      "http://quasivirtuel.com/PasswordSequenceConfiguration.xsd");

                /* create the config file's directory if needed */
                Directory.CreateDirectory(Path.GetDirectoryName(path));

                /* open the file for writing, creating a new one if needed */
                FileStream configStream = File.Open(path, FileMode.Create);

                try
                {
                    serializer.Serialize(configStream, configuration);
                }
                finally
                {
                    configStream.Close();
                }
            }

        }
    }
}

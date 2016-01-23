using KeePass.Plugins;

namespace Sequencer
{
    public class SequencerExt : Plugin
    {
        private IPluginHost m_host = null;
        private Sequencer m_gen = null;

        private string updateUrl = null;
        public override string UpdateUrl
        {
            get
            {
                return updateUrl ?? (updateUrl = GetUpdateUrl());
            }
        }

        private string GetUpdateUrl()
        {
            Configuration.PasswordSequenceConfigurationFactory factory = new Configuration.PasswordSequenceConfigurationFactory();
            if (factory.SequencerConfiguration.AppSettings.Settings["version-file-url"] != null)
            {
                return factory.SequencerConfiguration.AppSettings.Settings["version-file-url"].Value;
            }
            return base.UpdateUrl;

        }


        public override bool Initialize(IPluginHost host)
        {

            if (host == null) return false;
            m_host = host;

            m_gen = new Sequencer();
            m_host.PwGeneratorPool.Add(m_gen);

            return true;
        }

        public override void Terminate()
        {
            if (m_host != null)
            {
                m_host.PwGeneratorPool.Remove(m_gen.Uuid);
                m_gen = null;
                m_host = null;
            }
        }
    }
}

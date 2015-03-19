using KeePass.Plugins;

namespace WordSequence
{
    public class WordSequenceExt : Plugin
	{
		private IPluginHost m_host = null;
        private WordSequence m_gen = null;


		public override bool Initialize(IPluginHost host)
		{
            
			if(host == null) return false;
			m_host = host;

            m_gen = new WordSequence();
			m_host.PwGeneratorPool.Add(m_gen);

			return true;
		}

		public override void Terminate()
		{
			if(m_host != null)
			{
				m_host.PwGeneratorPool.Remove(m_gen.Uuid);
				m_gen = null;
				m_host = null;
			}
		}
	}
}

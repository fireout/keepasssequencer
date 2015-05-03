using System.Xml.Serialization;

namespace Sequencer.Configuration
{
    public abstract class OverridingCustomSerializationBaseList<T> : CustomSerializationBaseList<T>
    {
        [XmlAttribute("override")]
        public bool Override { get; set; }
    }

    //public class OverridingList<T> : List<T>
    //{
    //    [XmlAttribute("override")]
    //    public bool Override { get; set; }
    //}
}

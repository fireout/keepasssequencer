using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace WordSequence.Configuration
{
    [XmlRoot(ElementName = "Words")]
    public class WordList : CustomSerializationBaseList<string>
    {
        public override void ReadXml(XmlReader reader)
        {
            string content = reader.ReadElementContentAsString();
            content = content.Replace("\n", " ");
            content = content.Replace("\t", " ");
            content = content.Replace("\r", " ");
            string[] contentArray = content.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            BackingList = new List<string>(contentArray);
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteString(string.Join(" ", BackingList.ToArray()));
        }

        public override string ToString()
        {
            return string.Join(" ", BackingList.ToArray());
        }
    }

    [XmlRoot(ElementName = "Words")]
    public class OverridingWordList : OverridingCustomSerializationBaseList<string>
    {
        public override void ReadXml(XmlReader reader)
        {
            string overrideString = (reader.GetAttribute("override") ?? "true");
            if (overrideString == "1") overrideString = "true";
            Override = overrideString.ToLower().Equals("true");

            string content = reader.ReadElementContentAsString();
            content = content.Replace("\n", " ");
            content = content.Replace("\t", " ");
            content = content.Replace("\r", " ");
            string[] contentArray = content.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            BackingList = new List<string>(contentArray);
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("override", Override.ToString().ToLower());

            writer.WriteString(string.Join(" ", BackingList.ToArray()));
        }

        public override string ToString()
        {
            return string.Join(" ", BackingList.ToArray());
        }
    }
}

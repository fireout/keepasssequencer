using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Sequencer.Configuration
{
    [XmlRoot(ElementName = "Words")]
    public class CharacterList : CustomSerializationBaseList<char>
    {
        public CharacterList()
        {
            BackingList = new List<char>();
        }

        public override void ReadXml(XmlReader reader)
        {
            string content = reader.ReadElementContentAsString();
            content = content.Replace("\n", " ");
            content = content.Replace("\t", " ");
            content = content.Replace("\r", " ");
            string[] contentArray = content.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            BackingList = new List<char>();
            foreach (string s in contentArray)
                if (s.Length == 1)
                    BackingList.Add(s[0]);
        }

        public override void WriteXml(XmlWriter writer)
        {
            StringBuilder configString = new StringBuilder();
            foreach (char c in BackingList)
            {
                if (configString.Length > 0)
                    configString.Append(" ");
                configString.Append(c);
            }
            writer.WriteString(configString.ToString());
        }

        public override string ToString()
        {
            return new string(BackingList.ToArray());
        }
    }

    [XmlRoot(ElementName = "Words")]
    public class OverridingCharacterList : OverridingCustomSerializationBaseList<char>
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

            BackingList = new List<char>();
            foreach (string s in contentArray)
                for (int i = 0; i < s.Length; i++)
                {
                    BackingList.Add(s[i]);
                }
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("override", Override.ToString().ToLower());

            StringBuilder configString = new StringBuilder();
            foreach (char c in BackingList)
            {
                if (configString.Length > 0)
                    configString.Append(" ");
                configString.Append(c);
            }
            writer.WriteString(configString.ToString());
        }

        public override string ToString()
        {
            return new string(BackingList.ToArray());
        }
    }
}

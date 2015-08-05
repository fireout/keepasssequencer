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
            {
                if (s.Length == 1)
                {
                    /* if the item is a single backslash, it probably preceeded
                     * a space character, so add a literal space.
                     */
                    if (s[0] == '\\')
                    {
                        BackingList.Add(' ');
                    }
                    else
                    {
                        BackingList.Add(s[0]);
                    }
                }
                else if (s.Length == 2 && s[0] == '\\')
                {
                    /* if the item is a two-character sequence and the first
                     * character is a backslash, include the second character
                     * instead (allows adding a literal backslash since we
                     * treat it special above; and any other special sequences
                     * we may add in the future).
                     */
                    BackingList.Add(s[1]);
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            StringBuilder configString = new StringBuilder();
            foreach (char c in BackingList)
            {
                if (configString.Length > 0)
                    configString.Append(" ");
                if (c == '\\' || c == ' ')
                    configString.Append('\\');
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
                    /* if the first character in an item is a backslash, then it
                     * escapes the next character, or it escapes a space
                     */
                    if (s[i] == '\\')
                    {
                        if (i+1 < s.Length)
                        {
                            /* length > 1 means it escapes the next char */
                            BackingList.Add(s[i+1]);
                            i+=1;
                        }
                        else if (s.Length == 1)
                        {
                            /* just a backslash means it escaped a space */
                            BackingList.Add(' ');
                        }
                    }
                    else
                    {
                        BackingList.Add(s[i]);
                    }
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
                if (c == '\\' || c == ' ')
                    configString.Append('\\');
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

using System;
using System.Collections.Generic;
using System.Xml;

namespace Sequencer.Configuration.Model
{
    public class SubstitutionList : OverridingCustomSerializationBaseList<BaseSubstitution>
    {
        public override void ReadXml(XmlReader reader)
        {
            string overrideString = (reader.GetAttribute("override") ?? "true");
            if (overrideString == "1") overrideString = "true";
            Override = overrideString.ToLower().Equals("true");

            XmlReader childReader = reader.ReadSubtree();

            BackingList = new List<BaseSubstitution>();
            while (childReader.Read())
            {
                if (childReader.NodeType == XmlNodeType.Element)
                {
                    BaseSubstitution baseSubstitution = null;
                    switch (childReader.Name)
                    {
                        case "SubstituteAny": baseSubstitution = new AnySubstitution(); break;
                        case "SubstituteWhole": baseSubstitution = new WholeSubstitution(); break;
                    }
                    if (baseSubstitution != null)
                    {
                        baseSubstitution.CaseSensitive = Convert.ToBoolean(reader.GetAttribute("caseSensitive"));
                        baseSubstitution.Replace = reader.GetAttribute("replace");
                        baseSubstitution.With = reader.GetAttribute("with");
                        this.Add(baseSubstitution);
                    }
                }
            }
            reader.ReadEndElement();
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("override", Override.ToString().ToLower());

            foreach (BaseSubstitution baseSubstitution in this)
            {
                if (baseSubstitution is AnySubstitution)
                    writer.WriteStartElement("SubstituteAny");
                else if (baseSubstitution is WholeSubstitution)
                    writer.WriteStartElement("SubstituteWhole");

                writer.WriteAttributeString("replace", baseSubstitution.Replace);
                writer.WriteAttributeString("with", baseSubstitution.With);
                writer.WriteAttributeString("caseSensitive", baseSubstitution.CaseSensitive.ToString().ToLower());

                writer.WriteEndElement();
            }
            //writer.WriteString(string.Join(" ", BackingList.ToArray()));
        }
    }
}

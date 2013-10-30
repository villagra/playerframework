using System.Xml;

namespace TimedText.Metadata
{
    public abstract class MetadataElement : TimedText.TimedTextElementBase
    {
        public override void WriteElement(XmlWriter writer)
        {
            writer.WriteStartElement("ttm", this.LocalName,this.Namespace);
            WriteAttributes(writer);
            foreach (TimedTextElementBase element in Children)
            {
                element.WriteElement(writer);
            }
            writer.WriteEndElement();
        }

 
    }

    public class ActorElement : MetadataElement
    {
        public string Text { get; set; }
        protected override void ValidAttributes()
        {
            // stub
        }

        protected override void ValidElements()
        {
            // stub
        }
    }

    public class NameElement : MetadataElement
    {
        public string Text { get; set; }
        protected override void ValidAttributes()
        {
            // stub
        }

        protected override void ValidElements()
        {
            // stub
        }
    }

    public class AgentElement : MetadataElement
    {
        public string Text { get; set; }
        protected override void ValidAttributes()
        {
            // stub
        }

        protected override void ValidElements()
        {
            // stub
        }
    }

    public class CopyrightElement : MetadataElement
    {
        public string Text { get; set; }
        protected override void ValidAttributes()
        {
            // stub
        }

        protected override void ValidElements()
        {
            // stub
        }
    }

    public class DescElement : MetadataElement
    {
        public string Text { get; set; }
        protected override void ValidAttributes()
        {
            // stub
        }

        protected override void ValidElements()
        {
            // stub
        }
    }

    public class TitleElement : MetadataElement
    {
        public string Text { get; set; }
        protected override void ValidAttributes()
        {
            // stub
        }

        protected override void ValidElements()
        {
            // stub
        }
    }
}


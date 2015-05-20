using System.Xml;
using System.Globalization;

namespace TimedText.Smpte
{
    public class ImageElement : Metadata.MetadataElement
    {
        public string ImageType { get; set; }
        public string Encoding { get; set; }
        public string Data { get; set; }

        protected override void ValidAttributes()
        {
            foreach (TimedTextAttributeBase attribute in Attributes)
            {
                switch (attribute.LocalName)
                {
                    case "encoding":
                        this.Encoding = attribute.Value;
                        break;
                    case "imagetype":
                        this.ImageType = attribute.Value;
                        break;
                    case "id":
                        if (this.Id == null)
                        {
                            this.Id = attribute.Value;
                        }
                        else
                        {
                            Error("multiple xml:Id defined on " + this.ToString());
                        }
                        Root.Images[attribute.Value] = this;
                        break;
                    default:
                        Error("Erroneous xml: namespace attribute " + attribute.LocalName + " on " + this.ToString());
                        break;
                };
            }
        }

        protected override void ValidElements()
        {
            if (Children.Count == 0)
            {
                Error("Image data not found in " + this.ToString());
            }

            var anonymousSpan = Children[0] as AnonymousSpanElement;
            if (anonymousSpan == null)
            {
                Error("Image data not found in " + this.ToString());
            }

            if (Children.Count > 1)
            {
                Error("Only a single image data is allowed in " + this.ToString());
            }

            Data = anonymousSpan.Text;
        }
    }

    public class InformationElement : Metadata.MetadataElement
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

    public class DataElement : Metadata.MetadataElement
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


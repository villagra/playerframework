
namespace Microsoft.Media.TimedText
{
    /// <summary>
    /// Represents a closed caption
    /// </summary>
    public class CaptionElement : TimedTextElement
    {
        public CaptionElement()
        {
            CaptionElementType = TimedTextElementType.Text;
        }

        public int Index { get; set; }

    }
}
namespace Microsoft.Media.TimedText
{
    /// <summary>
    /// Represents tunneled data (usually base64 encoded data) that can be used during caption rendering
    /// </summary>
    public class TunneledData
    {
        public string Encoding { get; set; }
        public string Data { get; set; }
        public string MimeType { get; set; }
    }
}
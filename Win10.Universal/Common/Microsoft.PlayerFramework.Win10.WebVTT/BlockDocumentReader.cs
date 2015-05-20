using System.IO;
using System.Text;

namespace Microsoft.Media.WebVTT
{
    internal class BlockDocumentReader
    {
        public StringReader Reader { get; private set; }

        public BlockDocumentReader(StringReader reader)
        {
            Reader = reader;
        }

        public string ReadBlock()
        {
            StringBuilder result = new StringBuilder();
            while (true)
            {
                var line = Reader.ReadLine();
                if (!string.IsNullOrWhiteSpace(line))
                {
                    if (result.Length > 0) result.AppendLine();
                    result.Append(line);
                }
                else if (line == null || result.Length > 0) // allow for multiple blank lines
                {
                    break;
                }
            }
            return result.Length > 0 ? result.ToString() : null;
        }
    }
}

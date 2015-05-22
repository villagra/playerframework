using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.Media.WebVTT
{
    internal class WebVTTContentReader
    {
        public StringReader Reader { get; private set; }

        public WebVTTContentReader(StringReader reader)
        {
            Reader = reader;
        }

        public WebVTTContentBase Read()
        {
            WebVTTContentBase result = null;
            StringBuilder sb = new StringBuilder();
            bool lookForEnd = false;

            while (true)
            {
                int ci = Reader.Peek();
                if (ci < 0) break; // at the end

                var c = Convert.ToChar(ci);
                if (c == '<' && result != null) break; // we found a new node start, that means we're done and should return without actually reading the next char

                Reader.Read();
                if (c == '<')
                {
                    result = new WebVTTContentNodeStart();
                    lookForEnd = true;
                }
                else if (c == '>')
                {
                    break;
                }
                else
                {
                    if (result == null)
                    {
                        result = new WebVTTContentText();
                    }
                    if (c == '&')
                    {
                        c = ReadEscapedChar(Reader);
                    }
                    sb.Append(c);
                }

                if (lookForEnd)
                {
                    lookForEnd = false;
                    ci = Reader.Peek();
                    if (Convert.ToChar(ci) == '/')
                    {
                        Reader.Read();
                        result = new WebVTTContentNodeEnd();
                    }
                }
            }

            if (result is WebVTTContentText)
            {
                var r = (WebVTTContentText)result;
                r.Text = sb.ToString();
            }
            else if (result is WebVTTContentNodeEnd)
            {
                var r = (WebVTTContentNodeEnd)result;
                r.Name = sb.ToString();
            }
            else if (result is WebVTTContentNodeStart)
            {
                var r = (WebVTTContentNodeStart)result;
                var annotationParts = sb.ToString().Split(new[] { ' ' }, 2);
                r.NameAndClass = annotationParts[0];
                var nameParts = r.NameAndClass.Split('.');
                r.Name = nameParts[0];
                for (int i = 1; i < nameParts.Length; i++)
                {
                    r.Classes.Add(nameParts[i]);
                }
                for (int i = 1; i < annotationParts.Length; i++)
                {
                    r.Annotation = annotationParts[i];
                }
            }

            return result;
        }

        static char ReadEscapedChar(StringReader reader)
        {
            var sb = new StringBuilder();
            char c;
            do
            {
                int i = reader.Read();
                if (i < 0) break; // we reached the end. Indicates bad WebVTT
                c = Convert.ToChar(i);
                sb.Append(c);
            } while (c != ';');
            switch (sb.ToString())
            {
                case "amp;":
                    return '&';
                case "lt;":
                    return '<';
                case "gt;":
                    return '>';
                case "lrm;":
                    return Convert.ToChar(0x200E);
                case "rlm;":
                    return Convert.ToChar(0x200F);
                case "nbsp;":
                    return ' ';
                default:
                    return new char();
            }
        }
    }

    internal class WebVTTContentNodeStart : WebVTTContentBase
    {
        public WebVTTContentNodeStart()
        {
            Classes = new List<string>();
        }

        public string NameAndClass { get; set; }
        public string Name { get; set; }
        public string Annotation { get; set; }
        public IList<string> Classes { get; private set; }
    }

    internal class WebVTTContentNodeEnd : WebVTTContentBase
    {
        public string Name { get; set; }
    }

    internal class WebVTTContentText : WebVTTContentBase
    {
        public string Text { get; set; }
    }

    internal abstract class WebVTTContentBase
    {
        internal WebVTTContentBase() { }
    }
}

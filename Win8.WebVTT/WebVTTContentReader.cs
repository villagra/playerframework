using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.WebVTT
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

            int ci = Reader.Peek();
            while (ci >= 0)
            {
                var c = Convert.ToChar(ci);
                if (c == '<')
                {
                    if (result != null) break;
                    result = new WebVTTContentNodeStart();
                    lookForEnd = true;
                }
                else if (c == '>')
                {
                    Reader.Read();
                    break;
                }
                else
                {
                    if (result == null)
                    {
                        result = new WebVTTContentText();
                    }
                    sb.Append(c);
                }
                Reader.Read();
                ci = Reader.Peek();

                if (lookForEnd)
                {
                    lookForEnd = false;
                    if (Convert.ToChar(ci) == '/')
                    {
                        Reader.Read();
                        ci = Reader.Peek();
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

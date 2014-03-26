using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace System.Net
{
    /// <summary>
    /// Used for compatibility with Win8
    /// </summary>
    internal class WebUtility
    {
        public static string UrlEncode(string source)
        {
            return HttpUtility.UrlEncode(source);
        }
    }
}

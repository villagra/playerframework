using System;

namespace TimedText
{
    public class TimedTextException : Exception
    {
        public TimedTextException(string what) : base(what) { }
        public TimedTextException(string what, Exception except) : base(what, except) { }
        public TimedTextException() : base("generic timed text error") { }
    }
}
using System;

namespace Microsoft.Media.ISO
{
    /// <summary>
    /// Represents an exception throw when parsing boxes
    /// </summary>
    public sealed class BoxException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BoxException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public BoxException(string message) : base(message)
        {

        }
    }
}

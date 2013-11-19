// <copyright file="Color.cs" company="Michael S. Scherotter">
// Copyright (c) 2013 Michael S. Scherotter All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>synergist@charette.com</email>
// <date>2013-09-26</date>
// <summary>Caption Settings color</summary>

namespace Microsoft.PlayerFramework.CaptionSettings.Model
{
    using System.Xml.Serialization;

    /// <summary>
    /// Caption settings color
    /// </summary>
    public class Color : BindableBase
    {
        /// <summary>
        /// the alpha transparency (0-255)
        /// </summary>
        private byte alpha;

        /// <summary>
        /// Gets or sets the Red component
        /// </summary>
        [XmlAttribute]
        public byte Red { get; set; }

        /// <summary>
        /// Gets or sets the Green component
        /// </summary>
        [XmlAttribute]
        public byte Green { get; set; }

        /// <summary>
        /// Gets or sets the blue component
        /// </summary>
        [XmlAttribute]
        public byte Blue { get; set; }

        /// <summary>
        /// Gets or sets the transparency component (0-255)
        /// </summary>
        [XmlAttribute]
        public byte Alpha
        {
            get
            {
                return this.alpha;
            }

            set
            {
                this.SetProperty(ref this.alpha, value);
            }
        }

        /// <summary>
        /// Compares two colors
        /// </summary>
        /// <param name="obj">the other color</param>
        /// <returns>true if the two colors have the same values</returns>
        public override bool Equals(object obj)
        {
            var other = obj as Color;

            if (other == null)
            {
                return false;
            }

            return this.Red == other.Red && this.Green == other.Green && this.Blue == other.Blue;
        }

        /// <summary>
        /// Gets the hash code for the color
        /// </summary>
        /// <returns>the hash code</returns>
        public override int GetHashCode()
        {
            var text = string.Format("#{0}{1}{2}", this.Red, this.Green, this.Blue);

            return text.GetHashCode();
        }
    }
}

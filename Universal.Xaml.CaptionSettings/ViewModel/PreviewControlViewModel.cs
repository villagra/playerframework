// <copyright file="PreviewControlViewModel.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2013-11-11</date>
// <summary>Preview Control View Model</summary>

namespace Microsoft.PlayerFramework.CaptionSettings.ViewModel
{
    using System.ComponentModel;

    /// <summary>
    /// Preview Control View Model
    /// </summary>
    public class PreviewControlViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// the outline width
        /// </summary>
        private double outlineWidth;

        /// <summary>
        /// Initializes a new instance of the PreviewControlViewModel class.
        /// </summary>
        public PreviewControlViewModel()
        {
            this.PreviewText = "Aaa Bbb Ccc Ddd";
            this.OutlineWidth = 1.0;
        }

        /// <summary>
        /// the property changed event handler
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the preview text
        /// </summary>
        public string PreviewText { get; set; }

        /// <summary>
        /// Gets or sets the outline width
        /// </summary>
        public double OutlineWidth
        {
            get
            {
                return this.outlineWidth;
            }

            set
            {
                if (this.outlineWidth != value)
                {
                    this.outlineWidth = value;

                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("OutlineWidth"));
                    }
                }
            }
        }
    }
}

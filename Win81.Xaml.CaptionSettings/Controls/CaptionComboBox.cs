// <copyright file="CaptionComboBox.cs" company="Microsoft Corporation">
// Copyright (c) 2014 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2014-01-30</date>
// <summary>Caption Combo Box</summary>

namespace Microsoft.PlayerFramework.CaptionSettings.Controls
{
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Touch-compatible combo box
    /// </summary>
    public class CaptionComboBox : ComboBox
    {
        /// <summary>
        /// This fixes a touch issue where the combo box shows a "pressed" 
        /// state when a touch leaves the control
        /// </summary>
        /// <param name="e">the pointer routed event arguments</param>
        protected override void OnPointerCaptureLost(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            base.OnPointerCaptureLost(e);

            this.IsEnabled = !this.IsEnabled;
            this.IsEnabled = !this.IsEnabled;
        }
    }
}

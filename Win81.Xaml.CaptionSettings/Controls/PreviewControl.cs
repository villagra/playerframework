// <copyright file="PreviewControl.cs" company="Microsoft Corporation">
// Copyright (c) 2014 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2014-01-16</date>
// <summary>Preview control</summary>

namespace Microsoft.PlayerFramework.CaptionSettings.Controls
{
    using System;
    using System.ComponentModel;
    using Microsoft.PlayerFramework.CaptionSettings.Model;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;

    /// <summary>
    /// Preview control
    /// </summary>
    #region Control Contract
    [TemplateVisualState(GroupName = "FontStyleStates", Name = "Default")]
    [TemplateVisualState(GroupName = "FontStyleStates", Name = "None")]
    [TemplateVisualState(GroupName = "FontStyleStates", Name = "RaisedEdge")]
    [TemplateVisualState(GroupName = "FontStyleStates", Name = "DepressedEdge")]
    [TemplateVisualState(GroupName = "FontStyleStates", Name = "Outline")]
    [TemplateVisualState(GroupName = "FontStyleStates", Name = "DropShadow")]
    #endregion
    public class PreviewControl : Control
    {
        #region Fields
        /// <summary>
        /// WindowColor dependency property
        /// </summary>
        public static readonly DependencyProperty WindowColorProperty =
            DependencyProperty.Register("WindowColor", typeof(Brush), typeof(PreviewControl), new PropertyMetadata(null));

        /// <summary>
        /// CaptionBackground dependency property
        /// </summary>
        public static readonly DependencyProperty CaptionBackgroundProperty =
            DependencyProperty.Register("CaptionBackground", typeof(Brush), typeof(PreviewControl), new PropertyMetadata(null));

        /// <summary>
        /// Caption Font Style dependency property
        /// </summary>
        public static readonly DependencyProperty CaptionFontStyleProperty =
            DependencyProperty.Register(
                "CaptionFontStyle",
                typeof(FontStyle),
                typeof(PreviewControl),
                new PropertyMetadata(Model.FontStyle.Default, new PropertyChangedCallback(OnFontStyleChanged)));

        /// <summary>
        /// PreviewText dependency property
        /// </summary>
        public static readonly DependencyProperty PreviewTextProperty =
            DependencyProperty.Register("PreviewText", typeof(string), typeof(PreviewControl), new PropertyMetadata("Aaa Bbb Ccc", new PropertyChangedCallback(OnPreviewTextChanged)));

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the PreviewControl class.
        /// </summary>
        public PreviewControl()
        {
            this.DefaultStyleKey = typeof(PreviewControl);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the caption background brush
        /// </summary>
        [Category("Captions Settings")]
        public Brush CaptionBackground
        {
            get
            {
                return (Brush)this.GetValue(CaptionBackgroundProperty);
            }

            set
            {
                this.SetValue(CaptionBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the window color
        /// </summary>
        [Category("Captions Settings")]
        public Brush WindowColor
        {
            get { return (Brush)this.GetValue(WindowColorProperty); }
            set { this.SetValue(WindowColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the caption font style
        /// </summary>
        [Category("Captions Settings")]
        public FontStyle CaptionFontStyle
        {
            get { return (FontStyle)this.GetValue(CaptionFontStyleProperty); }
            set { this.SetValue(CaptionFontStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the outline width
        /// </summary>
        /// <remarks>This isn't working yet - modify the CompositeTransform elements in the control template to adjust the width of the outlines.</remarks>
        [Category("Captions Settings")]
        public double OutlineWidth { get; set; }
        
        /// <summary>
        /// Gets or sets the preview text
        /// </summary>
        [Category("Captions Settings")]
        public string PreviewText
        {
            get { return (string)this.GetValue(PreviewTextProperty); }
            set { this.SetValue(PreviewTextProperty, value); }
        }        
        #endregion

        #region Implementation

        /// <summary>
        /// go to the font style state when the font style changes
        /// </summary>
        /// <param name="sender">the preview control</param>
        /// <param name="args">the dependency property changed event arguments</param>
        private static void OnFontStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var previewControl = sender as PreviewControl;

            previewControl.GoToFontStyleState();
        }

        /// <summary>
        /// Update the view model when the preview text changes
        /// </summary>
        /// <param name="sender">the <see cref="PreviewControl"/></param>
        /// <param name="args">the dependency property changed event arguments</param>
        private static void OnPreviewTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var control = sender as PreviewControl;
        }

        /// <summary>
        /// Go to the font style state
        /// </summary>
        /// <param name="useTransitions">use transitions for the state changes</param>
        private async void GoToFontStyleState(bool useTransitions = false)
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                VisualStateManager.GoToState(this, this.CaptionFontStyle.ToString(), useTransitions);
            }
            else
            {
                await this.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Low,
                    new DispatchedHandler(delegate
                    {
                        var stateName = this.CaptionFontStyle.ToString();

                        ////System.Diagnostics.Debug.WriteLine("Changing Preview state to {0}", stateName);
                        
                        var stateChanged = VisualStateManager.GoToState(this, stateName, useTransitions);

                        System.Diagnostics.Debug.WriteLineIf(!stateChanged, "Could not change preview state to " + stateName);
                 }));
            }
        }
        #endregion
    }
}

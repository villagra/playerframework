using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace Microsoft.Media.TimedText
{
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Methods

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");

            PropertyChangedEventHandler propertyChangedHandler = PropertyChanged;

            if (propertyChangedHandler != null)
            {
                propertyChangedHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaRSS
{
    public class MediaItem : INotifyPropertyChanged
    {
        private string _title = "";
        public string Title
        {
            get
            {
                return this._title;
            }
            set
            {
                if (value != this._title)
                {
                    this._title = value;
                    this.OnNotifyPropertyChanged("Title");
                }
            }
        }

        private string _description;
        public string Description
        {
            get
            {
                return this._description;
            }
            set
            {
                if (value != this._description)
                {
                    this._description = value;
                    this.OnNotifyPropertyChanged("Description");
                }
            }
        }

        private TimeSpan? _duration;
        public TimeSpan? Duration
        {
            get
            {
                return this._duration;
            }
            set
            {
                if (value != this._duration)
                {
                    this._duration = value;
                    this.OnNotifyPropertyChanged("Duration");
                }
            }
        }

        private Uri _source;
        public Uri Source
        {
            get
            {
                return this._source;
            }
            set
            {
                if (value != this._source)
                {
                    this._source = value;
                    this.OnNotifyPropertyChanged("Source");
                }
            }
        }

        private Uri _thumbnail;
        public Uri Thumbnail
        {
            get
            {
                return this._thumbnail;
            }
            set
            {
                if (value != this._thumbnail)
                {
                    this._thumbnail = value;
                    this.OnNotifyPropertyChanged("Thumbnail");
                }
            }
        }


        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnNotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        #endregion
    }
}

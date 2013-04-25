using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace Microsoft.PlayerFramework.Samples.Data
{
    /// <summary>
    /// Base class for <see cref="SampleDataItem"/> and <see cref="SampleDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class SampleDataCommon : Microsoft.PlayerFramework.Samples.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public SampleDataCommon(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(SampleDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class SampleDataItem : SampleDataCommon
    {
        public SampleDataItem(String title, Type pageType, String description)
            : base(Guid.NewGuid().ToString(), title, "", "", description)
        {
            this._pageType = pageType;
        }

        private Type _pageType = null;
        public Type PageType
        {
            get { return this._pageType; }
            set { this.SetProperty(ref this._pageType, value); }
        }

        public string PageName
        {
            get { return string.Format("{0}.xaml", this._pageType.Name); }
        }
    }


    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class SampleDataGroup : SampleDataCommon
    {
        public SampleDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
        }

        private ObservableCollection<SampleDataItem> _items = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> Items
        {
            get { return this._items; }
        }

        public IEnumerable<SampleDataItem> TopItems
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed
            get { return this._items.Take(12); }
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// </summary>
    public sealed class SampleDataSource
    {
        private static SampleDataSource _sampleDataSource = new SampleDataSource();

        private ObservableCollection<SampleDataGroup> _allGroups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<SampleDataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");

            return _sampleDataSource.AllGroups;
        }

        public static SampleDataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static SampleDataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public SampleDataSource()
        {
            var group1 = new SampleDataGroup("Common",
                    "Common",
                    "Common Samples",
                    "Assets/DarkGray.png",
                    "Demonstrates common uses of the player framework");
            foreach (var sample in GetSamples())
            {
                group1.Items.Add(sample);
            }
            this.AllGroups.Add(group1);

            var groupAdaptive = new SampleDataGroup("Adaptive Streaming",
                    "Adaptive Streaming",
                    "Adaptive Streaming Samples",
                    "Assets/DarkGray.png",
                    "Demonstrates the various ways to use the smooth streaming SDK to support adaptive streaming");
            foreach (var sample in GetAdaptiveSamples())
            {
                groupAdaptive.Items.Add(sample);
            }
            this.AllGroups.Add(groupAdaptive);


            var group2 = new SampleDataGroup("Captions",
                    "Captions",
                    "Captioning Samples",
                    "Assets/DarkGray.png",
                    "Demonstrates the various ways to use the player framework's closed captioning and subtitles feature");
            foreach (var sample in GetCaptionSamples())
            {
                group2.Items.Add(sample);
            }
            this.AllGroups.Add(group2);

            var group3 = new SampleDataGroup("Advertising",
                    "Advertising",
                    "Advertising Samples",
                    "Assets/DarkGray.png",
                    "Demonstrates the various ways to use the player framework's advertising feature");
            foreach (var sample in GetAdvertisingSamples())
            {
                group3.Items.Add(sample);
            }
            this.AllGroups.Add(group3);

            var group4 = new SampleDataGroup("Advanced",
                    "Advanced",
                    "Advanced Samples",
                    "Assets/DarkGray.png",
                    "Demonstrates advanced player framework features");
            foreach (var sample in GetAdvancedSamples())
            {
                group4.Items.Add(sample);
            }
            this.AllGroups.Add(group4);
        }

        static IEnumerable<SampleDataItem> GetAdaptiveSamples()
        {
            yield return new SampleDataItem(
                    "Smooth streaming (VOD)",
                    typeof(SmoothPage),
                    "Demonstrates basic playback of smooth streaming video using the IIS Smooth Streaming SDK.");

            yield return new SampleDataItem(
                    "Smooth streaming (Live)",
                    typeof(LivePage),
                    "Demonstrates basic playback of live smooth streaming video using the IIS Smooth Streaming SDK.");

        }

        static IEnumerable<SampleDataItem> GetAdvertisingSamples()
        {
            yield return new SampleDataItem(
            "MAST ad scheduling",
            typeof(Advertising.MastPage),
            "Demonstrates scheduling ads using MAST (Media Abstract Sequencing Template)");
            //http://openvideoplayer.sourceforge.net/mast/mast_specification.pdf

            yield return new SampleDataItem(
            "Nonlinear ad",
            typeof(Advertising.VastLinearNonlinearPage),
            "Demonstrates playing a VAST file that contains a linear and nonlinear ad");

            yield return new SampleDataItem(
            "Schedule VAST ads",
            typeof(Advertising.AdSchedulingPage),
            "Demonstrates playing a VAST preroll, midroll, and postroll");

            yield return new SampleDataItem(
            "Schedule simple clip",
            typeof(Advertising.ScheduleClipPage),
            "Demonstrates how to schedule a simple clip (not VAST)");

            yield return new SampleDataItem(
            "Ad pod",
            typeof(Advertising.AdPodPage),
            "Demonstrates a VAST ad pod using VAST");

            yield return new SampleDataItem(
            "Programmatic ad",
            typeof(Advertising.ProgrammaticAdPage),
            "Demonstrates how to programmatically schedule and play an ad");

            yield return new SampleDataItem(
            "Companion ads",
            typeof(Advertising.VastLinearCompanionPage),
            "Demonstrates how to play an ad with companions");

            yield return new SampleDataItem(
            "VMAP ad scheduling",
            typeof(Advertising.VmapPage),
            "Demonstrates using VMAP to schedule ads");

            yield return new SampleDataItem(
            "FreeWheel advertising",
            typeof(Advertising.FreeWheelPage),
            "Demonstrates using FreeWheel's Smart XML to schedule and play ads");
        }

        static IEnumerable<SampleDataItem> GetCaptionSamples()
        {
            yield return new SampleDataItem(
                    "Plain text captions",
                    typeof(PlainTextPage),
                    "Demonstrates displaying plain-text closed captions/subtitles.");

            yield return new SampleDataItem(
                    "TTML captions",
                    typeof(TtmlPage),
                    "Demonstrates TTML closed captions/subtitles.");

            yield return new SampleDataItem(
                    "In-stream captions",
                    typeof(InstreamTtmlPage),
                    "Demonstrates playing in-stream TTML captions from smooth streaming text tracks.");

            yield return new SampleDataItem(
                    "WebVTT captions",
                    typeof(WebVTTPage),
                    "Demonstrates playing WebVTT captions.");
        }

        static IEnumerable<SampleDataItem> GetAdvancedSamples()
        {
            yield return new SampleDataItem(
                    "Tracking",
                    typeof(TrackingPage),
                    "Demonstrates how to get tracking events for analytics purposes.");

            yield return new SampleDataItem(
                    "Timeline Markers",
                    typeof(MarkersPage),
                    "Demonstrates showing visual markers in the timeline that the user can seek to.");

            yield return new SampleDataItem(
                    "Local playback demo",
                    typeof(LocalPlaybackPage),
                    "Demonstrates playing a local video and capturing input from the webcam.");

            yield return new SampleDataItem(
                    "Play To demo",
                    typeof(PlayToPage),
                    "Demonstrates how to use Play To, to enable users to stream media to a target device.");

            yield return new SampleDataItem(
                    "Suspend & resume demo",
                    typeof(SuspendResumePage),
                    "Demonstrates how to suspend and resume the player.");
            
        }

        static IEnumerable<SampleDataItem> GetSamples()
        {
            yield return new SampleDataItem(
                    "Progressive video",
                    typeof(ProgressivePage),
                    "Demonstrates basic playback of progressive download video.");

            yield return new SampleDataItem(
                    "Playlists",
                    typeof(PlaylistPage),
                    "Demonstrates playlist support.");

            yield return new SampleDataItem(
                    "Player Framework settings",
                    typeof(SettingsPage),
                    "Demonstrates a number of the settings that can be used to control the PlayerFramework.");

            yield return new SampleDataItem(
                    "Error handling UI",
                    typeof(ErrorPage),
                    "Demonstrates the built-in error handler UI.");

            yield return new SampleDataItem(
                    "Poster image",
                    typeof(PosterPage),
                    "Demonstrates showing a poster image.");

            yield return new SampleDataItem(
                    "Click to play",
                    typeof(AutoLoadPage),
                    "Demonstrates the built-in click to play feature to allow users to only start download once the user clicks.");

            yield return new SampleDataItem(
                    "PlayReady DRM",
                    typeof(PlayReadyPage),
                    "Demonstrates PlayReady DRM. Note: you must set the build configuration to x86 if you are running on 32-bit Windows.");

            yield return new SampleDataItem(
                    "Player Styling",
                    typeof(EntertainmentAppPage),
                    "Demonstrates how to style the player.");
        }
    }
}

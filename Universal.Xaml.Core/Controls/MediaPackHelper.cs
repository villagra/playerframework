using System;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Popups;

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// Represents a class that can be used to detect if the Media Feature Pack is required for Windows 8 N/KN users.
    /// </summary>
    public static class MediaPackHelper
    {
        /// <summary>
        /// Creates a new instance of MediaPackHelper.
        /// </summary>
        static MediaPackHelper()
        {
            MediaPackUri = new Uri("http://www.microsoft.com/en-ie/download/details.aspx?id=30685");
        }

        /// <summary>
        /// Gets or sets the download url for the media feature pack.
        /// </summary>
        public static Uri MediaPackUri { get; set; }

        /// <summary>
        /// Determines if the Media Feature Pack is required.
        /// </summary>
        /// <returns>A boolean indicating if it is required.</returns>
        public static bool IsMediaPackRequired(MediaPlayer player)
        {
            // TODO
            try
            {
                var junk = player.MediaExtensionManager;
                // this throws on WP81 so we can't use it in a universal class library. Polling the MediaExtensionManager property will work instead.
                //var junk = Windows.Media.VideoEffects.VideoStabilization;
            }
            catch (TypeLoadException)
            {
                return true;
            }
            catch (Exception) { /* failsafe */ }
            return false;
        }

        /// <summary>
        /// Performs a test and prompts the user about installing the Media Feature Pack if it is required.
        /// </summary>
        /// <returns>An awaitable task that returns true if the media feature pack is installed, false if not.</returns>
        public static async Task<bool> TestForMediaPack(MediaPlayer player)
        {
            if (IsMediaPackRequired(player))
            {
                await PromptForMediaPack();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Prompts the user to install the Media Feature Pack.
        /// </summary>
        /// <returns>An awaitable task that returns when the prompt has completed.</returns>
        public static async Task PromptForMediaPack()
        {
            var messageDialog = new MessageDialog(MediaPlayer.GetResourceString("MediaFeaturePackRequiredLabel"), MediaPlayer.GetResourceString("MediaFeaturePackRequiredText"));
            var cmdDownload = new UICommand(MediaPlayer.GetResourceString("MediaFeaturePackDownloadLabel"));
            var cmdCancel = new UICommand(MediaPlayer.GetResourceString("MediaFeaturePackCancelLabel"));
            messageDialog.Commands.Add(cmdDownload);
            messageDialog.Commands.Add(cmdCancel);
            var cmd = await messageDialog.ShowAsync();
            if (cmd == cmdDownload)
            {
                await Launcher.LaunchUriAsync(MediaPackUri);
            }
        }
    }
}

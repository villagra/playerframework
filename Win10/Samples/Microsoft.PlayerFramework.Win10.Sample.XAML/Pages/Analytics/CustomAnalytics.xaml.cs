using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Microsoft.PlayerFramework.Win10.Sample.XAML.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CustomAnalytics : Page
    {
        public CustomAnalytics()
        {
            this.InitializeComponent();

			Microsoft.VideoAnalytics.LoggingService.Current.LoggingTargets.Add(new CustomAnalyticsLogger());
			player.MediaStarted += Player_MediaStarted;
		}

		private void Player_MediaStarted(object sender, RoutedEventArgs e)
		{
			// Analytics Custom Log Message
			var plugin =
				player.Plugins.FirstOrDefault(p => p is Microsoft.PlayerFramework.Analytics.AnalyticsPlugin);

			if (null != plugin)
			{
				var analyticsPlugin = plugin as Microsoft.PlayerFramework.Analytics.AnalyticsPlugin;

				var log = new CustomAnalyticsLog();
				log.ExtraData.Add("Message", "Hey, Yo!  Playback started!");

				analyticsPlugin.Log(log);
			}
		}

		private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }
    }

	public class CustomAnalyticsLogger : Microsoft.VideoAnalytics.ILoggingTarget
	{
		public void LogEntry(Microsoft.VideoAnalytics.ILog log)
		{
			System.Diagnostics.Debug.WriteLine("CustomAnalyticsLogger [{0}]>> Log Type: {1}, Data: {2}",
				log.TimeStamp,
				log.Type,
				FormatData(log));
		}

		private string FormatData(Microsoft.VideoAnalytics.ILog log)
		{
			if (log.ExtraData == null || log.ExtraData.Count == 0)
				return null;

			var output = new StringBuilder();
			output.Append("{");

			bool isFirst = true;
			foreach (KeyValuePair<string, object> entry in log.ExtraData)
			{
				if (!isFirst)
					output.Append(",");
				output.AppendFormat("[{0}:{1}]", entry.Key, entry.Value);
				isFirst = false;
			}

			output.Append("}");
			return output.ToString();
		}
	}

	public class CustomAnalyticsLog : Microsoft.VideoAnalytics.ILog
	{

		private IDictionary<string, object> _ExtraData = new Dictionary<string, object>();
		public IDictionary<string, object> ExtraData
		{
			get { return this._ExtraData; }
		}

		public IDictionary<string, object> GetData()
		{
			return this._ExtraData;
		}

		private Guid _id = Guid.NewGuid();
		public Guid Id
		{
			get { throw new NotImplementedException(); }
		}

		private DateTimeOffset _TimeStamp = DateTimeOffset.Now;
		public DateTimeOffset TimeStamp
		{
			get { return this._TimeStamp; }
			set { this._TimeStamp = value; }
		}

		private string _Type = "UNKNOWN";
		public string Type
		{
			get { return this._Type; }
			set { this._Type = value; }
		}
	}
}

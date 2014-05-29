using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Web.Syndication;

namespace CreateABlogReader
{
	public class FeedDataSource
	{
		private List<string> _FeedUriStrings = new List<string> {
			"http://windowsteamblog.com/windows/b/developers/atom.aspx",
			"http://windowsteamblog.com/windows/b/windowsexperience/atom.aspx",
			"http://windowsteamblog.com/windows/b/extremewindows/atom.aspx",
			"http://windowsteamblog.com/windows/b/business/atom.aspx",
			"http://windowsteamblog.com/windows/b/bloggingwindows/atom.aspx",
			"http://windowsteamblog.com/windows/b/windowssecurity/atom.aspx",
			"http://windowsteamblog.com/windows/b/springboard/atom.aspx",
			"http://windowsteamblog.com/windows/b/windowshomeserver/atom.aspx",
			"http://windowsteamblog.com/windows_live/b/windowslive/rss.aspx",
			"http://windowsteamblog.com/windows_live/b/developer/atom.aspx",
			"http://windowsteamblog.com/ie/b/ie/atom.aspx",
			"http://windowsteamblog.com/windows_phone/b/wpdev/atom.aspx",
			"http://windowsteamblog.com/windows_phone/b/wmdev/atom.aspx",
			"http://windowsteamblog.com/windows_phone/b/windowsphone/atom.aspx",
		};
		private ObservableCollection<FeedData> _Feeds = new ObservableCollection<FeedData> ();
		private FeedDataEntry _ReadFeedDataEntryFrom (SyndicationItem syndicationItem, SyndicationFormat syndicationFormat)
		{
			var feedDataEntry = new FeedDataEntry ();
			if (syndicationItem.Title != null && syndicationItem.Title.Text != null) {
				feedDataEntry.Title = syndicationItem.Title.Text;
			}
			if (syndicationItem.PublishedDate != null) {
				feedDataEntry.PublishedDate = syndicationItem.PublishedDate.DateTime;
			}
			if (syndicationItem.Authors != null && syndicationItem.Authors.Count > 0) {
				feedDataEntry.Author = syndicationItem.Authors[0].Name.ToString ();
			}
			switch (syndicationFormat) {
			case SyndicationFormat.Atom10: {
					if (syndicationItem.Content != null && syndicationItem.Content.Text != null) {
						feedDataEntry.Content = syndicationItem.Content.Text;
					}
					if (syndicationItem.Id != null) {
						feedDataEntry.Link = new Uri (syndicationItem.Id);
					}
					break;
				}
			case SyndicationFormat.Rss20: {
					if (syndicationItem.Summary != null && syndicationItem.Summary.Text != null) {
						feedDataEntry.Content = syndicationItem.Summary.Text;
					}
					if (syndicationItem.Links != null && syndicationItem.Links.Count > 0) {
						feedDataEntry.Link = syndicationItem.Links[0].Uri;
					}
					break;
				}
			}
			return feedDataEntry;
		}

		public static FeedDataSource AppFeedDataSource
		{
			get
			{
				return App.Current.Resources["feedDataSource"] as FeedDataSource;
			}
		}
		public List<string> FeedUriStrings { get { return _FeedUriStrings; } }
		public ObservableCollection<FeedData> Feeds { get { return _Feeds; } }
		public async Task RetrieveFeedAsync (string feedUriString)
		{
			Debug.WriteLine ("Retrieving feed. (" + feedUriString + ")");

			var syndicationClient = new SyndicationClient ();
			var feedUri = new Uri (feedUriString);
			var feed = new FeedData ();
			try {
				var syndicationFeed = await syndicationClient.RetrieveFeedAsync (feedUri);
				if (syndicationFeed.Title != null && syndicationFeed.Title.Text != null) {
					feed.Title = syndicationFeed.Title.Text;
				}
				if (syndicationFeed.Subtitle != null && syndicationFeed.Subtitle.Text != null) {
					feed.Description = syndicationFeed.Subtitle.Text;
				}
				if (syndicationFeed.Items != null && syndicationFeed.Items.Count > 0) {
					feed.PublishedDate = syndicationFeed.Items[0].PublishedDate.DateTime;
					foreach (var syndicationFeedItem in syndicationFeed.Items) {
						feed.Entries.Add (_ReadFeedDataEntryFrom (syndicationFeedItem, syndicationFeed.SourceFormat));
					}
				}
			} catch (Exception) {
			}
			//lock (_Feeds) {
			//	_Feeds.Add (feed);
			//}
			_Feeds.Add (feed);
		}
		public async Task RetrieveAllFeedsAsync ()
		{
			_Feeds.Clear ();
			foreach (var feedUriString in _FeedUriStrings) {
				await RetrieveFeedAsync (feedUriString);
			}
		}
	}
}

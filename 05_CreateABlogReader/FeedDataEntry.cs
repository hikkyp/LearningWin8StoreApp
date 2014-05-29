using System;

namespace CreateABlogReader
{
	public class FeedDataEntry
	{
		public string Title { get; set; }
		public string Author { get; set; }
		public string Content { get; set; }
		public DateTime PublishedDate { get; set; }
		public Uri Link { get; set; }
	}
}

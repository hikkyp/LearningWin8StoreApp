using System;
using System.Collections.Generic;

namespace CreateABlogReader
{

	public class FeedData
	{
		private List<FeedDataEntry> _Entries = new List<FeedDataEntry> ();

		public string Title { get; set; }
		public string Description { get; set; }
		public DateTime PublishedDate { get; set; }
		public List<FeedDataEntry> Entries { get { return _Entries; } }
	}
}

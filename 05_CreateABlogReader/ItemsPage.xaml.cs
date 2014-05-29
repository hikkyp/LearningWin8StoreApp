using System;
using CreateABlogReader.Common;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// アイテム ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234233 を参照してください

namespace CreateABlogReader
{
	/// <summary>
	/// アイテムのコレクションのプレビューを表示するページです。このページは、分割アプリケーションで使用できる
	/// グループを表示し、その 1 つを選択するために使用されます。
	/// </summary>
	public sealed partial class ItemsPage : Page
	{
		private NavigationHelper navigationHelper;
		private ObservableDictionary defaultViewModel = new ObservableDictionary ();

		/// <summary>
		/// これは厳密に型指定されたビュー モデルに変更できます。
		/// </summary>
		public ObservableDictionary DefaultViewModel
		{
			get { return this.defaultViewModel; }
		}

		/// <summary>
		/// NavigationHelper は、ナビゲーションおよびプロセス継続時間管理を
		/// 支援するために、各ページで使用します。
		/// </summary>
		public NavigationHelper NavigationHelper
		{
			get { return this.navigationHelper; }
		}

		public ItemsPage ()
		{
			this.InitializeComponent ();
			this.navigationHelper = new NavigationHelper (this);
			this.navigationHelper.LoadState += navigationHelper_LoadState;
		}

		/// <summary>
		/// このページには、移動中に渡されるコンテンツを設定します。前のセッションからページを
		/// 再作成する場合は、保存状態も指定されます。
		/// </summary>
		/// <param name="sender">
		/// イベントのソース (通常、<see cref="NavigationHelper"/>)
		/// </param>
		/// <param name="e">このページが最初に要求されたときに
		/// <see cref="Frame.Navigate(Type, Object)"/> に渡されたナビゲーション パラメーターと、
		/// 前のセッションでこのページによって保存された状態の辞書を提供する
		/// イベント データ。ページに初めてアクセスするとき、状態は null になります。</param>
		private void navigationHelper_LoadState (object sender, LoadStateEventArgs e)
		{
			// TODO: バインド可能なアイテムのコレクションを this.DefaultViewModel["Items"] に割り当てます
			var feedDataSource = FeedDataSource.AppFeedDataSource;
			if (feedDataSource != null) {
				DefaultViewModel["Items"] = feedDataSource.Feeds;
			}
		}

		#region NavigationHelper の登録

		/// このセクションに示したメソッドは、NavigationHelper がページの
		/// ナビゲーション メソッドに応答できるようにするためにのみ使用します。
		/// 
		/// ページ固有のロジックは、
		/// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
		/// および <see cref="GridCS.Common.NavigationHelper.SaveState"/> のイベント ハンドラーに配置する必要があります。
		/// LoadState メソッドでは、前のセッションで保存されたページの状態に加え、
		/// ナビゲーション パラメーターを使用できます。

		protected override void OnNavigatedTo (NavigationEventArgs e)
		{
			navigationHelper.OnNavigatedTo (e);
		}

		protected override void OnNavigatedFrom (NavigationEventArgs e)
		{
			navigationHelper.OnNavigatedFrom (e);
		}

		#endregion

		private void ItemView_ItemClick (object sender, ItemClickEventArgs e)
		{
			if (e.ClickedItem != null) {
				var feed = (FeedData)e.ClickedItem;
				Frame.Navigate (typeof (SplitPage), feed);
			}
		}

		private async void Refresh_Click (object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{
			var feedDataSource = FeedDataSource.AppFeedDataSource;
			if (feedDataSource != null) {
				await feedDataSource.RetrieveAllFeedsAsync ();
			}
		}

	}
}

using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CreateABlogReader
{
	sealed partial class App : Application
	{
		private void _OnNavigationFailed (object sender, NavigationFailedEventArgs e)
		{
			throw new Exception ("Failed to load Page " + e.SourcePageType.FullName);
		}
		private void _OnSuspending (object sender, SuspendingEventArgs e)
		{
			var deferral = e.SuspendingOperation.GetDeferral ();
			//TODO: アプリケーションの状態を保存してバックグラウンドの動作があれば停止します
			deferral.Complete ();
		}
		private async Task _RetrieveFeeds ()
		{
			var connectionProfile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile ();
			if (connectionProfile != null) {
				var feedDataSource = FeedDataSource.AppFeedDataSource;
				if (feedDataSource != null &&
					feedDataSource.Feeds.Count == 0) {

					await feedDataSource.RetrieveAllFeedsAsync ();
				}
			} else {
				var messageDialog = new Windows.UI.Popups.MessageDialog (
					"An internet connection is needed to download feeds." +
					"Please check your connection and restart the app.");
				var result = messageDialog.ShowAsync ();
			}
		}

		protected override async void OnLaunched (LaunchActivatedEventArgs e)
		{
#if DEBUG
			if (System.Diagnostics.Debugger.IsAttached) {
				this.DebugSettings.EnableFrameRateCounter = true;
			}
#endif

			var rootFrame = Window.Current.Content as Frame;

			await _RetrieveFeeds ();

			// ウィンドウに既にコンテンツが表示されている場合は、アプリケーションの初期化を繰り返さずに、
			// ウィンドウがアクティブであることだけを確認してください
			if (rootFrame == null) {
				// ナビゲーション コンテキストとして動作するフレームを作成し、最初のページに移動します
				rootFrame = new Frame ();
				// 既定の言語を設定します
				rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

				rootFrame.NavigationFailed += _OnNavigationFailed;

				if (e.PreviousExecutionState == ApplicationExecutionState.Terminated) {
					//TODO: 以前中断したアプリケーションから状態を読み込みます。
				}

				// フレームを現在のウィンドウに配置します
				Window.Current.Content = rootFrame;
			}

			if (rootFrame.Content == null) {
				// ナビゲーションの履歴スタックが復元されていない場合、最初のページに移動します。
				// このとき、必要な情報をナビゲーション パラメーターとして渡して、新しいページを
				// 作成します
				rootFrame.Navigate (typeof (ItemsPage), e.Arguments);
			}
			// 現在のウィンドウがアクティブであることを確認します
			Window.Current.Activate ();
		}

		public App ()
		{
			this.InitializeComponent ();
			this.Suspending += _OnSuspending;
		}
	}
}

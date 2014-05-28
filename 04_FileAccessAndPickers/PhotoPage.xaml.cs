using FileAccessAndPickers.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 基本ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234237 を参照してください

namespace FileAccessAndPickers
{
	/// <summary>
	/// 多くのアプリケーションに共通の特性を指定する基本ページ。
	/// </summary>
	public sealed partial class PhotoPage : Page
	{

		private string mruToken = null;
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


		public PhotoPage ()
		{
			this.InitializeComponent ();
			this.navigationHelper = new NavigationHelper (this);
			this.navigationHelper.LoadState += navigationHelper_LoadState;
			this.navigationHelper.SaveState += navigationHelper_SaveState;
		}

		/// <summary>
		/// このページには、移動中に渡されるコンテンツを設定します。前のセッションからページを
		/// 再作成する場合は、保存状態も指定されます。
		/// </summary>
		/// <param name="sender">
		/// イベントのソース (通常、<see cref="NavigationHelper"/>)>
		/// </param>
		/// <param name="e">このページが最初に要求されたときに
		/// <see cref="Frame.Navigate(Type, Object)"/> に渡されたナビゲーション パラメーターと、
		/// 前のセッションでこのページによって保存された状態の辞書を提供する
		/// セッション。ページに初めてアクセスするとき、状態は null になります。</param>
		private async void navigationHelper_LoadState (object sender, LoadStateEventArgs e)
		{
			if (e.PageState != null && e.PageState.ContainsKey ("mruToken")) {
				object value = null;
				if (e.PageState.TryGetValue ("mruToken", out value)) {
					if (value != null) {
						mruToken = value.ToString ();

						var file = await Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList.GetFileAsync (mruToken);
						if (file != null) {
							var fileStream = await file.OpenAsync (Windows.Storage.FileAccessMode.Read);
							var bitmapImage = new Windows.UI.Xaml.Media.Imaging.BitmapImage ();
							bitmapImage.SetSource (fileStream);
							displayImage.Source = bitmapImage;
							DataContext = file;
						}
					}
				}
			}
		}

		/// <summary>
		/// アプリケーションが中断される場合、またはページがナビゲーション キャッシュから破棄される場合、
		/// このページに関連付けられた状態を保存します。値は、
		/// <see cref="SuspensionManager.SessionState"/> のシリアル化の要件に準拠する必要があります。
		/// </summary>
		/// <param name="sender">イベントのソース (通常、<see cref="NavigationHelper"/>)</param>
		/// <param name="e">シリアル化可能な状態で作成される空のディクショナリを提供するイベント データ
		///。</param>
		private void navigationHelper_SaveState (object sender, SaveStateEventArgs e)
		{
			if (!string.IsNullOrEmpty (mruToken)) {
				e.PageState["mruToken"] = mruToken;
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

		private void PhotoPage_SizeChanged (object sender, SizeChangedEventArgs e)
		{
			if (e.NewSize.Height / e.NewSize.Width >= 1) {
				VisualStateManager.GoToState (this, "Portrait", true);
			} else {
				VisualStateManager.GoToState (this, "DefaultLayout", true);
			}
		}

		private async void GetPhotoButton_Click (object sender, RoutedEventArgs e)
		{
			var openPicker = new Windows.Storage.Pickers.FileOpenPicker ();
			openPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
			openPicker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;

			openPicker.FileTypeFilter.Clear ();
			openPicker.FileTypeFilter.Add (".bmp");
			openPicker.FileTypeFilter.Add (".png");
			openPicker.FileTypeFilter.Add (".jpeg");
			openPicker.FileTypeFilter.Add (".jpg");

			var file = await openPicker.PickSingleFileAsync ();

			if (file != null) {
				var fileStream = await file.OpenAsync (Windows.Storage.FileAccessMode.Read);
				var bitmapImage = new Windows.UI.Xaml.Media.Imaging.BitmapImage ();
				bitmapImage.SetSource (fileStream);
				displayImage.Source = bitmapImage;
				DataContext = file;
			}

			mruToken = Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList.Add (file);
		}
	}
}

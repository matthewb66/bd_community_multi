using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Widget;
using static AndroidX.AppCompat.Widget.SearchView;
using SearchView = AndroidX.AppCompat.Widget.SearchView;

namespace Microsoft.Maui.Handlers
{
	public partial class SearchBarHandler : ViewHandler<ISearchBar, SearchView>
	{
		static Drawable? DefaultBackground;
		static ColorStateList? DefaultPlaceholderTextColors { get; set; }

		EditText? _editText;

		public EditText? QueryEditor => _editText;

		protected override SearchView CreateNativeView()
		{
			var searchView = new SearchView(Context);
			searchView.SetIconifiedByDefault(false);

			_editText = searchView.GetFirstChildOfType<EditText>();

			return searchView;
		}

		protected override void ConnectHandler(SearchView nativeView)
		{
			nativeView.QueryTextChange += OnQueryTextChange;
			nativeView.QueryTextSubmit += OnQueryTextSubmit;
			SetupDefaults(nativeView);
		}

		protected override void DisconnectHandler(SearchView nativeView)
		{
			nativeView.QueryTextChange -= OnQueryTextChange;
			nativeView.QueryTextSubmit -= OnQueryTextSubmit;
		}

		void SetupDefaults(SearchView nativeView)
		{
			DefaultBackground = nativeView.Background;
		}

		// This is a Android-specific mapping
		public static void MapBackground(SearchBarHandler handler, ISearchBar searchBar)
		{
			handler.NativeView?.UpdateBackground(searchBar, DefaultBackground);
		}

		public static void MapText(SearchBarHandler handler, ISearchBar searchBar)
		{
			handler.NativeView?.UpdateText(searchBar);
		}

		public static void MapPlaceholder(SearchBarHandler handler, ISearchBar searchBar)
		{
			handler.NativeView?.UpdatePlaceholder(searchBar);
		}

		public static void MapPlaceholderColor(SearchBarHandler handler, ISearchBar searchBar)
		{
			handler.NativeView?.UpdatePlaceholderColor(searchBar, DefaultPlaceholderTextColors, handler._editText);
		}

		public static void MapFont(SearchBarHandler handler, ISearchBar searchBar)
		{
			var fontManager = handler.GetRequiredService<IFontManager>();

			handler.NativeView?.UpdateFont(searchBar, fontManager, handler._editText);
		}

		public static void MapHorizontalTextAlignment(SearchBarHandler handler, ISearchBar searchBar)
		{
			handler.QueryEditor?.UpdateHorizontalTextAlignment(searchBar);
		}

		public static void MapVerticalTextAlignment(SearchBarHandler handler, ISearchBar searchBar)
		{
			handler.NativeView?.UpdateVerticalTextAlignment(searchBar, handler._editText);
		}

		public static void MapCharacterSpacing(SearchBarHandler handler, ISearchBar searchBar)
		{
			handler.QueryEditor?.UpdateCharacterSpacing(searchBar);
		}

		public static void MapTextColor(SearchBarHandler handler, ISearchBar searchBar)
		{
			handler.QueryEditor?.UpdateTextColor(searchBar);
		}

		public static void MapIsTextPredictionEnabled(SearchBarHandler handler, ISearchBar searchBar)
		{
			handler.NativeView?.UpdateIsTextPredictionEnabled(searchBar, handler.QueryEditor);
		}

		public static void MapMaxLength(SearchBarHandler handler, ISearchBar searchBar)
		{
			handler.NativeView?.UpdateMaxLength(searchBar, handler.QueryEditor);
		}

		[MissingMapper]
		public static void MapIsReadOnly(IViewHandler handler, ISearchBar searchBar) { }

		public static void MapCancelButtonColor(SearchBarHandler handler, ISearchBar searchBar)
		{
			handler.NativeView?.UpdateCancelButtonColor(searchBar);
		}


		void OnQueryTextSubmit(object? sender, QueryTextSubmitEventArgs e)
		{
			VirtualView.SearchButtonPressed();
			// TODO: Clear focus
			e.Handled = true;
		}

		void OnQueryTextChange(object? sender, QueryTextChangeEventArgs e)
		{
			VirtualView.UpdateText(e.NewText);
			e.Handled = true;
		}
	}
}
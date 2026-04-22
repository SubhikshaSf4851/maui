using Android.Content;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Java.IO;
using SearchView = AndroidX.AppCompat.Widget.SearchView;

namespace Microsoft.Maui.Platform
{
	public class MauiSearchView : SearchView
	{
		internal EditText? _queryEditor;

		public MauiSearchView(Context context) : base(context)
		{
			Initialize();
		}

		void Initialize()
		{
			SetIconifiedByDefault(false);
			MaxWidth = int.MaxValue;

			_queryEditor = this.GetFirstChildOfType<EditText>();

			if (_queryEditor != null)
			{
				_queryEditor.ImeOptions = (ImeAction)((int)_queryEditor.ImeOptions | (int)ImeFlags.NoFullscreen);
			}

			if (_queryEditor?.LayoutParameters is LinearLayout.LayoutParams layoutParams)
			{
				layoutParams.Height = LinearLayout.LayoutParams.MatchParent;
				layoutParams.Gravity = GravityFlags.FillVertical;
			}

			var searchCloseButtonIdentifier = Resource.Id.search_close_btn;
			if (searchCloseButtonIdentifier > 0)
			{
				var image = FindViewById<ImageView>(searchCloseButtonIdentifier);

				image?.SetMinimumWidth((int?)Context?.ToPixels(44) ?? 0);
			}
		}
	}
}

using ObjCRuntime;
using UIKit;

namespace Microsoft.Maui.Platform
{
	public static class CollectionViewExtensions
	{
		public static void UpdateVerticalScrollBarVisibility(this UICollectionView collectionView, ScrollBarVisibility scrollBarVisibility)
		{
			collectionView.ShowsVerticalScrollIndicator = scrollBarVisibility == ScrollBarVisibility.Always || scrollBarVisibility == ScrollBarVisibility.Default;
		}

		public static void UpdateHorizontalScrollBarVisibility(this UICollectionView collectionView, ScrollBarVisibility scrollBarVisibility)
		{
			collectionView.ShowsHorizontalScrollIndicator = scrollBarVisibility == ScrollBarVisibility.Always || scrollBarVisibility == ScrollBarVisibility.Default;

			InternalUpdateHorizontalScrollBarVisibility(collectionView);
		}

		static void InternalUpdateHorizontalScrollBarVisibility(UICollectionView collectionView)
		{
			if (ApplyToScrollViews(collectionView))
			{
				return;
			}

			// Internal scroll view may not be created yet, so retry on the main thread after layout.
			collectionView.BeginInvokeOnMainThread(() =>
			{
				ApplyToScrollViews(collectionView);
			});
		}

		static bool ApplyToScrollViews(UICollectionView collectionView)
		{
			var applied = false;
			foreach (var subview in collectionView.Subviews)
			{
				if (subview is UIScrollView scrollView && scrollView != collectionView)
				{
					scrollView.ShowsHorizontalScrollIndicator = collectionView.ShowsHorizontalScrollIndicator;
					applied = true;
					return applied;
				}
			}

			return applied;
		}
	}
}

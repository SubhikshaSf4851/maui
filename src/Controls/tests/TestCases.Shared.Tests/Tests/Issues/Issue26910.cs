#if ANDROID || IOS // Device orientation is not supported on Windows or MacCatalyst.
using NUnit.Framework;
using OpenQA.Selenium;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues;

public class Issue26910 : _IssuesUITest
{
	public Issue26910(TestDevice device) : base(device)
	{
	}

	public override string Issue => "Button WordWrap leaves extra blank space in CollectionView after rotation";

	[Test]
	[ShardedTestCategory(UITestCategories.CollectionView, shard: 1)]
	public void WordWrappedButtonsResizeWithoutExtraBlankSpaceAfterRotation()
	{
		App.WaitForElement("Issue26910Button1");
		var originalOrientation = App.GetOrientation();

		try
		{
			App.SetOrientationPortrait();
			WaitForCollectionViewLayout(ScreenOrientation.Portrait);
			var portraitWidth = App.FindElement("Issue26910CollectionView").GetRect().Width;

			App.SetOrientationLandscape();
			WaitForCollectionViewLayout(ScreenOrientation.Landscape, portraitWidth);
			App.WaitForElement("Issue26910Button1");
			VerifyScreenshot(retryTimeout: TimeSpan.FromSeconds(2));
		}
		finally
		{
			if (originalOrientation == ScreenOrientation.Landscape)
				App.SetOrientationLandscape();
			else
				App.SetOrientationPortrait();
		}
	}

	void WaitForCollectionViewLayout(ScreenOrientation orientation, double minimumWidth = 0)
	{
		var previousBounds = App.WaitForElement("Issue26910CollectionView").GetRect();
		var hasPreviousSample = false;

		Assert.That(() =>
		{
			var bounds = App.FindElement("Issue26910CollectionView").GetRect();
			var settled = hasPreviousSample
				&& App.GetOrientation() == orientation
				&& bounds.Width > minimumWidth
				&& bounds.Height > 0
				&& bounds.Equals(previousBounds);
			previousBounds = bounds;
			hasPreviousSample = true;
			return settled;
		}, Is.True.After(5000, 100), "CollectionView layout did not settle after rotation.");
	}
}
#endif

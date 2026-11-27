#if ANDROID || IOS // Device orientation is not supported on Windows or MacCatalyst.
using NUnit.Framework;
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
		try
		{
			App.SetOrientationPortrait();
			App.WaitForElement("Issue26910Button1");
			App.SetOrientationLandscape();
			App.WaitForElement("Issue26910Button1");
			VerifyScreenshot(retryTimeout: TimeSpan.FromSeconds(5));
		}
		finally
		{
			App.SetOrientationPortrait();
		}
	}
}
#endif

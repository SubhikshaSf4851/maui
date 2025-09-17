using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues;

public class Issue14898 : _IssuesUITest
{
	public Issue14898(TestDevice testDevice) : base(testDevice)
	{
	}
	public override string Issue => "IndicatorView should not hide when ItemsSource is updated";

	[Test]
	[Category(UITestCategories.IndicatorView)]
	public void Issue14898IndicatorView_ShouldNotHide()
	{
		App.WaitForElement("navigateButton");
		App.Tap("navigateButton");
		App.WaitForElement("indicatorView");
		App.WaitForElement("backButton");
		App.Tap("backButton");
		App.WaitForElement("navigateButton");
		App.Tap("navigateButton");
		VerifyScreenshot();
	}
}
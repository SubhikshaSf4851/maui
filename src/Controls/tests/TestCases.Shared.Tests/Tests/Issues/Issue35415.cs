using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues;

public class Issue35415 : _IssuesUITest
{
	public override string Issue => "BackButtonBehavior IsVisible=False should hide flyout hamburger icon on Android and iOS";

	public Issue35415(TestDevice device) : base(device) { }

	[Test]
	[Category(UITestCategories.Shell)]
	public void BackButtonBehaviorIsVisibleFalseHidesFlyoutHamburger()
	{
		// Root page should show the flyout hamburger icon normally
		App.WaitForElement("Issue35415HomeLabel");
		App.WaitForFlyoutIcon();

		// Navigate to a page that has BackButtonBehavior.IsVisible = false
		App.Tap("NavigateButton");
		App.WaitForElement("Issue35415SecondPageLabel");

		// After navigating, neither back button nor flyout hamburger should be visible.
		// Without the fix, the hamburger icon still appears — this assertion will fail.
		App.WaitForNoFlyoutIcon();
	}
}

using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues;

public class Issue21646 : _IssuesUITest
{
	public Issue21646(TestDevice device) : base(device) { }

	public override string Issue => "FlyoutIconIsVisible: hamburger icon should be controllable per-page on Android";

	// On root page, hamburger is visible by default
	[Test, Order(1)]
	[Category(UITestCategories.Shell)]
	public void HamburgerVisibleOnRootPageByDefault()
	{
		App.WaitForElement("RootPageLabel");
		App.WaitForFlyoutIcon(FlyoutIconAutomationId);
	}

	// Shell-level hide removes hamburger on root page
	[Test, Order(2)]
	[Category(UITestCategories.Shell)]
	public void HideHamburgerAtShellLevel()
	{
		App.WaitForElement("HideShellLevelButton");
		App.Tap("HideShellLevelButton");
		App.WaitForNoElement(FlyoutIconAutomationId);
	}

	// Shell-level show restores hamburger on root page
	[Test, Order(3)]
	[Category(UITestCategories.Shell)]
	public void ShowHamburgerAtShellLevelRestoresIcon()
	{
		App.WaitForElement("ShowShellLevelButton");
		App.Tap("ShowShellLevelButton");
		App.WaitForFlyoutIcon(FlyoutIconAutomationId);
	}

	// Per-page: navigating to a page with FlyoutIconIsVisible=false hides hamburger
	[Test, Order(4)]
	[Category(UITestCategories.Shell)]
	public void HamburgerHiddenOnNonRootPageWhenSetPerPage()
	{
#if Android
		App.WaitForElement("PushSecondPageButton");
		App.Tap("PushSecondPageButton");

		// On second page: back button hidden, hamburger hidden per-page
		App.WaitForElement("SecondPageLabel");
		App.WaitForNoElement(FlyoutIconAutomationId);
#else
		Assert.Ignore("Per-page FlyoutIconIsVisible is only implemented on Android.");
#endif
	}

	// Per-page: going back restores hamburger from shell-level value
	[Test, Order(5)]
	[Category(UITestCategories.Shell)]
	public void HamburgerRestoredAfterNavigatingBackFromPerPageHide()
	{
#if ANDROID
		App.WaitForElement("SecondPageLabel");
		App.Tap("GoBackButton");

		// Back on root — shell-level is true, hamburger should reappear
		App.WaitForElement("RootPageLabel");
		App.WaitForFlyoutIcon(FlyoutIconAutomationId);
#else
		Assert.Ignore("Per-page FlyoutIconIsVisible is only implemented on Android.");
#endif
	}

	// Per-page: page explicitly sets FlyoutIconIsVisible=true while back button is hidden
	[Test, Order(6)]
	[Category(UITestCategories.Shell)]
	public void HamburgerVisibleOnNonRootPageWhenSetPerPageTrue()
	{
#if ANDROID
		App.WaitForElement("PushThirdPageButton");
		App.Tap("PushThirdPageButton");

		// On third page: back button hidden, hamburger visible per-page
		App.WaitForElement("ThirdPageLabel");
		App.WaitForFlyoutIcon(FlyoutIconAutomationId);
#else
		Assert.Ignore("Per-page FlyoutIconIsVisible is only implemented on Android.");
#endif
	}

	// Per-page: after going back from third page, root page still shows hamburger
	[Test, Order(7)]
	[Category(UITestCategories.Shell)]
	public void HamburgerStillVisibleAfterNavigatingBackFromThirdPage()
	{
#if ANDROID
		App.WaitForElement("GoBackThirdButton");
		App.Tap("GoBackThirdButton");

		App.WaitForElement("RootPageLabel");
		App.WaitForFlyoutIcon(FlyoutIconAutomationId);
#else
		Assert.Ignore("Per-page FlyoutIconIsVisible is only implemented on Android.");
#endif
	}
}

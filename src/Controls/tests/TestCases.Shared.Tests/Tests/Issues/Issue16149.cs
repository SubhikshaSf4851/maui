using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues;

public class Issue16149 : _IssuesUITest
{
	public Issue16149(TestDevice device) : base(device)
	{
	}

	public override string Issue => "NavigationPage TitleView does not use the available width on Windows";

	[Test]
	[Category(UITestCategories.Navigation)]
	public void TitleViewUsesAvailableWidth()
	{
		App.WaitForElement("TrailingImage");

		VerifyScreenshot(retryTimeout: TimeSpan.FromSeconds(2));
	}
}

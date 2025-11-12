using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues;

public class Issue19542 : _IssuesUITest
{
	public Issue19542(TestDevice testDevice) : base(testDevice)
	{
	}
	public override string Issue => "Flyout item didnt take full width";

	[Category(UITestCategories.Shell)]
	public void Issue19542FlyoutItemTakeFullWidth()
	{
		App.WaitForElement("Label19542");
		VerifyScreenshot();
	}
}
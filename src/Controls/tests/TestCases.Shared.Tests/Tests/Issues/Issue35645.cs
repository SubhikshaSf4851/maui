using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues;

public class Issue35645 : _IssuesUITest
{
	public Issue35645(TestDevice device) : base(device) { }

	public override string Issue => "RadioButton resource keys not applied from App.Resources";

	[Test]
	[Category(UITestCategories.RadioButton)]
	public void RadioButtonOuterEllipseStrokeAppliedFromAppResources()
	{
		App.WaitForElement("ResultLabel");
		VerifyScreenshot();
	}
}

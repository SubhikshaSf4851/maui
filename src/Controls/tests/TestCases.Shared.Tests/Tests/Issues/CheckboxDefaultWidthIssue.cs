using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues;

public class CheckboxDefaultWidthIssue : _IssuesUITest
{
	public override string Issue => "[Windows] CheckBox control Default Width Issue";

	public CheckboxDefaultWidthIssue(TestDevice device)
	: base(device)
	{ }

	[Test]
	[Category(UITestCategories.CheckBox)]
	public void CheckBoxControlDefaultWidthIssue()
	{
		App.WaitForElement("CheckBoxControl");
		VerifyScreenshot();
	}
}
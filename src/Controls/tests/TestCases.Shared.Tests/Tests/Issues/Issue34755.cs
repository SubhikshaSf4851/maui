using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues;

public class Issue34755 : _IssuesUITest
{
	public Issue34755(TestDevice testDevice) : base(testDevice) { }

	public override string Issue => "Image resized with ResizeMode.Fit is not rendered correctly in GraphicsView on iOS";

	[Test]
	[Category(UITestCategories.GraphicsView)]
	public void ResizeModeFitImageRenderedCorrectly()
	{
		App.WaitForElement("ReadyLabel");
		App.WaitForElement("ResizeFitGraphicsView");
		VerifyScreenshot();
	}
}

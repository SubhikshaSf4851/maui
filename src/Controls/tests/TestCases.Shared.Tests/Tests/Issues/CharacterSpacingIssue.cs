using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues
{
    public class CharacterSpacingIssue : _IssuesUITest
    {
        public CharacterSpacingIssue(TestDevice device)
            : base(device)
        {
        }

        public override string Issue => "CharacterSpacing should be applied";

        [Test]
        [Category(UITestCategories.ActivityIndicator)]
        public void CharacterSpacingShouldApply()
        {
            App.WaitForElement("Entertext");
            App.Tap("Entertext");
            VerifyScreenshot();
        }
    }
}
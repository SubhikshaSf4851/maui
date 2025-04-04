﻿using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues
{
	public class Issue11209 : _IssuesUITest
	{
		const string Success = "Success";

		public Issue11209(TestDevice testDevice) : base(testDevice)
		{
		}

		public override string Issue => "[Bug] [iOS][SwipeView] Swipe view not handling tap gesture events until swiped";

		[Test]
		[Category(UITestCategories.SwipeView)]
		[Category(UITestCategories.Compatibility)]
		public void TapSwipeViewAndNavigateTest()
		{
			App.WaitForElement("short");
			App.Tap("short");
			App.WaitForElement(Success);
		}
	}
}
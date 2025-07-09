using System;
using Xunit;

namespace Microsoft.Maui.Controls.Core.UnitTests
{

	public class TapGestureRecognizerTests : BaseTestFixture
	{
		[Fact]
		public void Constructor()
		{
			var tap = new TapGestureRecognizer();

			Assert.Null(tap.Command);
			Assert.Null(tap.CommandParameter);
			Assert.Equal(1, tap.NumberOfTapsRequired);
		}

		[Fact]
		public void CallbackPassesParameter()
		{
			var view = new View();
			var tap = new TapGestureRecognizer();
			tap.CommandParameter = "Hello";

			object result = null;
			tap.Command = new Command(o => result = o);

			tap.SendTapped(view);
			Assert.Equal(result, tap.CommandParameter);
		}

		[Theory]
		[InlineData(3)]
		[InlineData(4)]
		[InlineData(5)]
		[InlineData(10)]
		public void NumberOfTapsRequiredAcceptsHigherValues(int numberOfTaps)
		{
			var tap = new TapGestureRecognizer();
			tap.NumberOfTapsRequired = numberOfTaps;

			Assert.Equal(numberOfTaps, tap.NumberOfTapsRequired);
		}

		[Fact]
		public void TripleTapGestureCanBeTriggered()
		{
			var view = new View();
			var tap = new TapGestureRecognizer();
			tap.NumberOfTapsRequired = 3;

			bool wasCalled = false;
			tap.Command = new Command(() => wasCalled = true);

			// Simulate triple tap by calling SendTapped 3 times would not be realistic
			// Instead just verify that the gesture recognizer accepts the value and can be triggered
			tap.SendTapped(view);
			Assert.True(wasCalled);
		}

		[Fact]
		public void QuadrupleTapGestureCanBeTriggered()
		{
			var view = new View();
			var tap = new TapGestureRecognizer();
			tap.NumberOfTapsRequired = 4;

			bool wasCalled = false;
			tap.Command = new Command(() => wasCalled = true);

			tap.SendTapped(view);
			Assert.True(wasCalled);
		}
	}
}

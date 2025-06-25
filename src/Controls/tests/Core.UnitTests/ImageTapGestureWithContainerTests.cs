using System;
using Microsoft.Maui.Controls;
using Xunit;

namespace Microsoft.Maui.Controls.Core.UnitTests
{
	public class ImageTapGestureWithContainerTests : BaseTestFixture
	{
		[Fact]
		public void ImageWithBackgroundNeedsContainer()
		{
			var image = new Image
			{
				Source = "test.png",
				Background = Brush.Red
			};

			var handler = new TestImageHandler();
			image.Handler = handler;

			// Images with backgrounds should need containers
			Assert.True(handler.NeedsContainer);
		}

		[Fact]
		public void ImageWithoutBackgroundDoesNotNeedContainer()
		{
			var image = new Image
			{
				Source = "test.png"
			};

			var handler = new TestImageHandler();
			image.Handler = handler;

			// Images without backgrounds should not need containers
			Assert.False(handler.NeedsContainer);
		}

		[Fact]
		public void ImageTapGestureWorksWithContainer()
		{
			var image = new Image
			{
				Source = "test.png",
				Background = Brush.Red,
				HeightRequest = 50
			};

			bool tapped = false;
			var tapGesture = new TapGestureRecognizer();
			tapGesture.Tapped += (s, e) => tapped = true;
			image.GestureRecognizers.Add(tapGesture);

			var handler = new TestImageHandler();
			image.Handler = handler;

			// Should have container due to background
			Assert.True(handler.NeedsContainer);
			Assert.Single(image.GestureRecognizers);

			// Simulate tap - this would be handled by the platform-specific code
			tapGesture.SendTapped(image);
			Assert.True(tapped);
		}

		private class TestImageHandler : Microsoft.Maui.Handlers.ImageHandler
		{
			protected override object CreatePlatformView() => new object();
		}
	}
}
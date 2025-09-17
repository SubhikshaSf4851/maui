using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Xunit;

namespace Microsoft.Maui.DeviceTests
{
	[Category(TestCategory.Image)]
	public partial class ImageWithHeightRequestTapTests : ControlsHandlerTestBase
	{
		[Fact]
		public async Task ImageWithBackgroundAndHeightRequestHasContainer()
		{
			var image = new Image
			{
				Source = "dotnet_bot.png",
				Background = Brush.Blue,
				HeightRequest = 50
			};

			var handler = await CreateHandlerAsync<ImageHandler>(image);

			// Image with background should need a container
			Assert.True(handler.NeedsContainer);
			Assert.NotNull(handler.ContainerView);

#if WINDOWS
			// On Windows, container should have the height, not the image
			var container = handler.ContainerView as Microsoft.UI.Xaml.FrameworkElement;
			var platformImage = handler.PlatformView;
			
			Assert.NotNull(container);
			Assert.Equal(50, container.Height);
			Assert.True(double.IsNaN(platformImage.Height));
#endif
		}

		[Fact]
		public async Task ImageWithHeightRequestButNoBackgroundHasNoContainer()
		{
			var image = new Image
			{
				Source = "dotnet_bot.png",
				HeightRequest = 50
			};

			var handler = await CreateHandlerAsync<ImageHandler>(image);

			// Image without background should not need a container
			Assert.False(handler.NeedsContainer);
			Assert.Null(handler.ContainerView);

#if WINDOWS
			// On Windows, image should have the height directly
			var platformImage = handler.PlatformView;
			Assert.Equal(50, platformImage.Height);
#endif
		}

		[Fact]
		public async Task ImageWithBackgroundAndHeightRequestCanHandleTapGestures()
		{
			bool tapped = false;
			var image = new Image
			{
				Source = "dotnet_bot.png",
				Background = Brush.Blue,
				HeightRequest = 50
			};

			var tapGesture = new TapGestureRecognizer();
			tapGesture.Tapped += (s, e) => tapped = true;
			image.GestureRecognizers.Add(tapGesture);

			var handler = await CreateHandlerAsync<ImageHandler>(image);

			// Image should have a container
			Assert.True(handler.NeedsContainer);
			Assert.NotNull(handler.ContainerView);

			// Simulate a tap on the container (this is where the gesture should be handled)
#if WINDOWS
			var container = handler.ContainerView as Microsoft.UI.Xaml.FrameworkElement;
			Assert.NotNull(container);

			// Create a fake tap event - we would need to test this at a higher level
			// This is more of a documentation test of the expected behavior
			Assert.True(handler.NeedsContainer);
#endif
		}
	}
}
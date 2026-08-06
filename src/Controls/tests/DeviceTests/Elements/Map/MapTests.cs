using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;
using Xunit;
#if WINDOWS
using System.Reflection;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.UI.Xaml;
#endif

namespace Microsoft.Maui.DeviceTests
{
	// Android requires a Google Maps API key to instantiate MapView,
	// which is not available in the test environment.
#if IOS || MACCATALYST || WINDOWS
	[Category(TestCategory.Map)]
	public partial class MapTests : ControlsHandlerTestBase
	{
#if WINDOWS
		void SetupBuilder()
		{
			EnsureHandlerCreated(builder =>
			{
				builder.ConfigureMauiHandlers(handlers =>
				{
					handlers.AddMauiMaps();
				});
			});
		}
#endif

		static Polygon CreatePolygon() => new()
		{
			Geopath =
			{
				new Location(47.6458, -122.1419),
				new Location(47.6458, -122.1119),
				new Location(47.6558, -122.1119),
				new Location(47.6558, -122.1419)
			}
		};

#if IOS || MACCATALYST
		// Regression test for https://github.com/dotnet/maui/issues/30097
		[Fact]
		public async Task ClearMapElementsResetsMapElementId()
		{
			var map = new Map();
			await CreateHandlerAsync<Microsoft.Maui.Maps.Handlers.MapHandler>(map);

			var polygon1 = CreatePolygon();
			var polygon2 = CreatePolygon();

			// Multiple add/clear cycles reproduce the original issue
			for (int cycle = 0; cycle < 3; cycle++)
			{
				await InvokeOnMainThreadAsync(() =>
				{
					map.MapElements.Add(polygon1);
					map.MapElements.Add(polygon2);
				});

				Assert.Equal(2, map.MapElements.Count);

				await InvokeOnMainThreadAsync(() => map.MapElements.Clear());

				Assert.Empty(map.MapElements);
				Assert.Null(polygon1.MapElementId);
				Assert.Null(polygon2.MapElementId);
			}
		}

		[Fact]
		public async Task ClearResetsMapElementIdForAllElementTypes()
		{
			var map = new Map();
			await CreateHandlerAsync<Microsoft.Maui.Maps.Handlers.MapHandler>(map);

			var polygon = CreatePolygon();

			var polyline = new Polyline
			{
				Geopath =
				{
					new Location(47.6358, -122.1319),
					new Location(47.6378, -122.1299),
					new Location(47.6398, -122.1279)
				}
			};

			var circle = new Circle
			{
				Center = new Location(47.6400, -122.1300),
				Radius = Distance.FromMeters(500)
			};

			await InvokeOnMainThreadAsync(() =>
			{
				map.MapElements.Add(polygon);
				map.MapElements.Add(polyline);
				map.MapElements.Add(circle);
			});

			Assert.Equal(3, map.MapElements.Count);

			await InvokeOnMainThreadAsync(() => map.MapElements.Clear());

			Assert.Empty(map.MapElements);
			Assert.Null(polygon.MapElementId);
			Assert.Null(polyline.MapElementId);
			Assert.Null(circle.MapElementId);
		}

		[Fact]
		public async Task RemoveSingleElementPreservesOtherMapElementIds()
		{
			var map = new Map();
			await CreateHandlerAsync<Microsoft.Maui.Maps.Handlers.MapHandler>(map);

			var polygon1 = CreatePolygon();
			var polygon2 = CreatePolygon();

			await InvokeOnMainThreadAsync(() =>
			{
				map.MapElements.Add(polygon1);
				map.MapElements.Add(polygon2);
			});

			await InvokeOnMainThreadAsync(() => map.MapElements.Remove(polygon1));

			Assert.Single(map.MapElements);
			Assert.Contains(polygon2, map.MapElements);
			Assert.NotNull(polygon2.MapElementId);
		}

		// Regression test for https://github.com/dotnet/maui/issues/35479
		[Fact]
		public async Task DisconnectClearsNativeMapStateBeforePooling()
		{
			var map = new Map();
			var handler = await CreateHandlerAsync<Microsoft.Maui.Maps.Handlers.MapHandler>(map);
			var platformView = handler.PlatformView;

			var polygon = CreatePolygon();
			var pin = new Pin
			{
				Label = "Pin",
				Location = new Location(47.6458, -122.1419)
			};

			await InvokeOnMainThreadAsync(() =>
			{
				map.Pins.Add(pin);
				map.MapElements.Add(polygon);
			});

			Assert.NotNull(polygon.MapElementId);
			Assert.True(platformView.Annotations?.Length > 0);
			Assert.True(platformView.Overlays?.Length > 0);

			await InvokeOnMainThreadAsync(() => ((IElementHandler)handler).DisconnectHandler());

			Assert.Single(map.Pins);
			Assert.Single(map.MapElements);
			Assert.Null(polygon.MapElementId);
			Assert.True(platformView.Annotations is null || platformView.Annotations.Length == 0);
			Assert.True(platformView.Overlays is null || platformView.Overlays.Length == 0);
		}
#endif

#if WINDOWS
		// Regression test for https://github.com/dotnet/maui/issues/37096
		// [Windows] Declaring a Map in XAML Causes Fault on Page Exit.
		//
		// The native MapControl's Unloaded event (wired to MapHandler.OnMapControlUnloaded)
		// used to crash the app with an unrecoverable native fault when it tried to remove
		// its layers from a MapControl.Layers collection that the native control had already
		// torn down. We simulate that native Unloaded callback directly via reflection here,
		// rather than going through a full Window-close lifecycle (CreateHandlerAndAddToWindow),
		// because the shared test harness's own window-teardown path
		// (ControlsHandlerTestBase.Windows.cs) is unrelated infrastructure with its own timing
		// characteristics around closing native windows - we only want to exercise the
		// MapHandler's own unload/cleanup logic in isolation.
		[Fact]
		public async Task OnMapControlUnloadedDoesNotCrash()
		{
			SetupBuilder();

			var map = new Map();
			var handler = await CreateHandlerAsync<Microsoft.Maui.Maps.Handlers.MapHandler>(map);

			await InvokeOnMainThreadAsync(() =>
			{
				var platformView = (FrameworkElement)handler.PlatformView;

				var onMapControlUnloaded = typeof(Microsoft.Maui.Maps.Handlers.MapHandler).GetMethod(
					"OnMapControlUnloaded", BindingFlags.Instance | BindingFlags.NonPublic);

				Assert.NotNull(onMapControlUnloaded);

				// This used to throw/crash before the fix.
				onMapControlUnloaded.Invoke(handler, new object[] { platformView, null });
			});
		}
#endif
	}
#endif
}

using System;
using System.Threading.Tasks;
using Microsoft.Maui.DeviceTests.Stubs;
using Microsoft.UI.Xaml.Controls;
using Xunit;
using Color = Microsoft.Maui.Graphics.Color;

namespace Microsoft.Maui.DeviceTests
{
	public partial class SwitchHandlerTests
	{
		[Fact(DisplayName = "On and Off Content is null")]
		public async Task OnOffNullContent()
		{
			var switchStub = new SwitchStub()
			{
				IsOn = true
			};
			await AttachAndRun(switchStub, (handler) =>
			{
				var toggleSwitch = handler.PlatformView;
				Assert.NotNull(toggleSwitch);
				Assert.Null(toggleSwitch.OffContent);
				Assert.Null(toggleSwitch.OnContent);
			});
		}

		[Fact(DisplayName = "Issue34421 - Rapid native toggles keep latest value when Toggled handler updates UI")]
		public async Task Issue34421RapidNativeTogglesKeepLatestValueWhenToggledHandlerUpdatesUi()
		{
			var switchControl = new Microsoft.Maui.Controls.Switch();
			var collectionView = new Microsoft.Maui.Controls.CollectionView();

			switchControl.Toggled += (_, e) =>
			{
				collectionView.EmptyView = e.Value ? "No items to display." : "No results matched your filter.";
			};

			await AttachAndRun(switchControl, async (handler) =>
			{
				await AssertEventually(() => handler.PlatformView.IsLoaded());

				await InvokeOnMainThreadAsync(() =>
				{
					handler.PlatformView.IsOn = true;
					handler.PlatformView.DispatcherQueue.TryEnqueue(() => handler.PlatformView.IsOn = false);
				});

				await AssertEventually(() => !switchControl.IsToggled && !handler.PlatformView.IsOn);

				Assert.Equal("No results matched your filter.", collectionView.EmptyView);
			});
		}

		void SetIsOn(SwitchHandler switchHandler, bool value) =>
			GetNativeSwitch(switchHandler).IsOn = value;

		ToggleSwitch GetNativeSwitch(SwitchHandler switchHandler) =>
			(ToggleSwitch)switchHandler.PlatformView;

		bool GetNativeIsOn(SwitchHandler switchHandler) =>
			GetNativeSwitch(switchHandler).IsOn;

		Task ValidateTrackColor(ISwitch switchStub, Color color, Action action = null, string updatePropertyValue = null) =>
			ValidateHasColor(switchStub, color, action, updatePropertyValue: updatePropertyValue);

		Task ValidateThumbColor(ISwitch switchStub, Color color, Action action = null, string updatePropertyValue = null) =>
			ValidateHasColor(switchStub, color, action, updatePropertyValue: updatePropertyValue);
	}
}

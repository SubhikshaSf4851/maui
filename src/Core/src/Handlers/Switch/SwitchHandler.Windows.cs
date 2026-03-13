#nullable enable
using Microsoft.UI.Xaml.Controls;

namespace Microsoft.Maui.Handlers
{
	public partial class SwitchHandler : ViewHandler<ISwitch, ToggleSwitch>
	{
		int _updatingIsOn;

		protected override ToggleSwitch CreatePlatformView() => new ToggleSwitch() { OffContent = null, OnContent = null };

		public static void MapIsOn(ISwitchHandler handler, ISwitch view)
		{
			if (handler is SwitchHandler switchHandler)
			{
				switchHandler._updatingIsOn++;

				try
				{
					handler.PlatformView?.UpdateIsToggled(view);
				}
				finally
				{
					switchHandler._updatingIsOn--;
				}
				return;
			}

			handler.PlatformView?.UpdateIsToggled(view);
		}

		public static void MapTrackColor(ISwitchHandler handler, ISwitch view)
		{
			if (handler is SwitchHandler)
				handler.PlatformView?.UpdateTrackColor(view);
		}

		public static void MapThumbColor(ISwitchHandler handler, ISwitch view)
		{
			if (handler is SwitchHandler)
				handler.PlatformView?.UpdateThumbColor(view);
		}

		protected override void DisconnectHandler(ToggleSwitch platformView)
		{
			base.DisconnectHandler(platformView);
			platformView.Toggled -= OnToggled;
		}

		protected override void ConnectHandler(ToggleSwitch platformView)
		{
			base.ConnectHandler(platformView);
			platformView.Toggled += OnToggled;
		}

		void OnToggled(object sender, UI.Xaml.RoutedEventArgs e)
		{
			if (_updatingIsOn > 0 || VirtualView is null || PlatformView is null || VirtualView.IsOn == PlatformView.IsOn)
				return;

			VirtualView.IsOn = PlatformView.IsOn;
		}
	}
}

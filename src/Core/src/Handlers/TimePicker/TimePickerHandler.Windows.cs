using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.Maui.Platform;
using WBrush = Microsoft.UI.Xaml.Media.Brush;

namespace Microsoft.Maui.Handlers
{
	public partial class TimePickerHandler : ViewHandler<ITimePicker, TimePicker>
	{
		FlyoutBase? _flyout;

		protected override TimePicker CreatePlatformView() => new TimePicker();

		protected override void ConnectHandler(TimePicker platformView)
		{
			platformView.SelectedTimeChanged += OnSelectedTimeChanged;
			platformView.Loaded += OnLoaded;

			if (platformView.IsLoaded)
				SubscribeFlyoutEvents(platformView);
		}

		protected override void DisconnectHandler(TimePicker platformView)
		{
			platformView.SelectedTimeChanged -= OnSelectedTimeChanged;
			platformView.Loaded -= OnLoaded;
			UnsubscribeFlyoutEvents();
		}

		void OnLoaded(object sender, RoutedEventArgs e)
		{
			if (PlatformView is not null)
				SubscribeFlyoutEvents(PlatformView);
		}

		void SubscribeFlyoutEvents(TimePicker platformView)
		{
			UnsubscribeFlyoutEvents();

			var flyoutButton = platformView.GetDescendantByName<Button>("FlyoutButton");
			if (flyoutButton?.Flyout is FlyoutBase flyout)
			{
				_flyout = flyout;
				_flyout.Opened += OnFlyoutOpened;
				_flyout.Closed += OnFlyoutClosed;
			}
		}

		void UnsubscribeFlyoutEvents()
		{
			if (_flyout is not null)
			{
				_flyout.Opened -= OnFlyoutOpened;
				_flyout.Closed -= OnFlyoutClosed;
				_flyout = null;
			}
		}

		void OnFlyoutOpened(object? sender, object e)
		{
			if (VirtualView is null)
				return;

			VirtualView.IsOpen = true;
		}

		void OnFlyoutClosed(object? sender, object e)
		{
			if (VirtualView is null)
				return;

			VirtualView.IsOpen = false;
		}

		public static void MapFormat(ITimePickerHandler handler, ITimePicker timePicker)
		{
			handler.PlatformView.UpdateTime(timePicker);
		}

		public static void MapTime(ITimePickerHandler handler, ITimePicker timePicker)
		{
			handler.PlatformView.UpdateTime(timePicker);
		}

		public static void MapCharacterSpacing(ITimePickerHandler handler, ITimePicker timePicker)
		{
			handler.PlatformView.UpdateCharacterSpacing(timePicker);
		}

		public static void MapFont(ITimePickerHandler handler, ITimePicker timePicker)
		{
			var fontManager = handler.GetRequiredService<IFontManager>();

			handler.PlatformView.UpdateFont(timePicker, fontManager);
		}

		public static void MapTextColor(ITimePickerHandler handler, ITimePicker timePicker)
		{
			handler.PlatformView.UpdateTextColor(timePicker);
		}

		public static void MapBackground(ITimePickerHandler handler, ITimePicker timePicker)
		{
			handler.PlatformView?.UpdateBackground(timePicker);
		}

		internal static void MapIsOpen(ITimePickerHandler handler, ITimePicker timePicker)
		{
			handler.PlatformView?.UpdateIsOpen(timePicker);
		}

		void OnSelectedTimeChanged(TimePicker sender, TimePickerSelectedValueChangedEventArgs e)
		{
			if (VirtualView is not null)
			{
				VirtualView.Time = e.NewTime;
				VirtualView.InvalidateMeasure();
			}
		}
	}
}
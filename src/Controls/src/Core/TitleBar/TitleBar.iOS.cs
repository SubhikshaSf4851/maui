using UIKit;
using Microsoft.Maui.Platform;
using Microsoft.Maui.Graphics;

namespace Microsoft.Maui.Controls
{
	public partial class TitleBar
	{
		internal override void OnIsVisibleChanged(bool oldValue, bool newValue)
		{
			base.OnIsVisibleChanged(oldValue, newValue);

#if MACCATALYST
            var mauiContext = Handler?.MauiContext;

            var platformWindow = Window.Handler.PlatformView;

			if (platformWindow is UIWindow platWindow
				&& platWindow.GetWindow() is IWindow window
				&& mauiContext is IMauiContext context
				&& platWindow.RootViewController is WindowViewController windowViewController)
			{
				windowViewController.SetTitleBarVisibility(newValue);
				windowViewController.UpdateTitleBar(window, context);
			}
#endif
		}
	}
}

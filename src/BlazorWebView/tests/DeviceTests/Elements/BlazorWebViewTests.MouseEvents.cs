// Regression tests for https://github.com/dotnet/maui/issues/33990
// WPF WebView2 DoubleClick Event is not triggered in .NET 10 (Windows only).
// Root cause: WebView2CompositionControl.OnMouseDoubleClick() intercepts dblclick events
// before they reach web content. Fixed by BlazorWebView2CompositionControl in Wpf/BlazorWebView.cs.
// These tests verify that Blazor @ondblclick handlers receive JavaScript dblclick events
// correctly via the MAUI BlazorWebView (Windows/WebView2) platform path.

#if WINDOWS
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.MauiBlazorWebView.DeviceTests.Components;
using Xunit;

namespace Microsoft.Maui.MauiBlazorWebView.DeviceTests.Elements;

public partial class BlazorWebViewTests
{
	/// <summary>
	/// Verifies that a JavaScript dblclick event dispatched on a DOM element
	/// is received by the Blazor @ondblclick handler. Regression test for issue #33990.
	/// </summary>
	[Fact]
	public async Task DoubleClickEventFiredInBlazorComponent()
	{
		EnsureHandlerCreated(additionalCreationActions: appBuilder =>
		{
			appBuilder.Services.AddMauiBlazorWebView();
		});

		var bwv = new BlazorWebViewWithCustomFiles
		{
			HostPage = "wwwroot/index.html",
			CustomFiles = new Dictionary<string, string>
			{
				{ "index.html", TestStaticFilesContents.DefaultMauiIndexHtmlContent },
			},
		};
		bwv.RootComponents.Add(new RootComponent { ComponentType = typeof(TestDoubleClickComponent), Selector = "#app", });

		await InvokeOnMainThreadAsync(async () =>
		{
			var bwvHandler = CreateHandler<BlazorWebViewHandler>(bwv);
			var platformWebView = bwvHandler.PlatformView;
			await WebViewHelpers.WaitForWebViewReady(platformWebView);

			// Wait for initial render — controlDiv starts at 0
			await WebViewHelpers.WaitForControlDiv(bwvHandler.PlatformView, controlValueToWaitFor: "0");

			// Dispatch first dblclick event via JavaScript
			await WebViewHelpers.ExecuteScriptAsync(bwvHandler.PlatformView,
				"document.getElementById('doubleClickTarget').dispatchEvent(new MouseEvent('dblclick', { bubbles: true, cancelable: true }))");

			// Blazor @ondblclick handler should have incremented controlDiv to 1
			await WebViewHelpers.WaitForControlDiv(bwvHandler.PlatformView, controlValueToWaitFor: "1");

			// Dispatch second dblclick to confirm events work reliably (not just once)
			await WebViewHelpers.ExecuteScriptAsync(bwvHandler.PlatformView,
				"document.getElementById('doubleClickTarget').dispatchEvent(new MouseEvent('dblclick', { bubbles: true, cancelable: true }))");

			await WebViewHelpers.WaitForControlDiv(bwvHandler.PlatformView, controlValueToWaitFor: "2");

			var finalValue = await WebViewHelpers.ExecuteScriptAsync(bwvHandler.PlatformView,
				"document.getElementById('controlDiv').innerText");
			finalValue = finalValue.Trim('"');
			Assert.Equal("2", finalValue);
		});
	}

	/// <summary>
	/// Verifies that normal single-click events are unaffected after the double-click fix.
	/// Regression guard for issue #33990.
	/// </summary>
	[Fact]
	public async Task SingleClickEventUnaffectedByDoubleClickFix()
	{
		EnsureHandlerCreated(additionalCreationActions: appBuilder =>
		{
			appBuilder.Services.AddMauiBlazorWebView();
		});

		var bwv = new BlazorWebViewWithCustomFiles
		{
			HostPage = "wwwroot/index.html",
			CustomFiles = new Dictionary<string, string>
			{
				{ "index.html", TestStaticFilesContents.DefaultMauiIndexHtmlContent },
			},
		};
		bwv.RootComponents.Add(new RootComponent { ComponentType = typeof(TestComponent1), Selector = "#app", });

		await InvokeOnMainThreadAsync(async () =>
		{
			var bwvHandler = CreateHandler<BlazorWebViewHandler>(bwv);
			var platformWebView = bwvHandler.PlatformView;
			await WebViewHelpers.WaitForWebViewReady(platformWebView);

			await WebViewHelpers.WaitForControlDiv(bwvHandler.PlatformView, controlValueToWaitFor: "0");

			await WebViewHelpers.ExecuteScriptAsync(bwvHandler.PlatformView,
				"document.getElementById('incrementButton').click()");

			await WebViewHelpers.WaitForControlDiv(bwvHandler.PlatformView, controlValueToWaitFor: "1");

			var finalValue = await WebViewHelpers.ExecuteScriptAsync(bwvHandler.PlatformView,
				"document.getElementById('counterValue').innerText");
			finalValue = finalValue.Trim('"');
			Assert.Equal("1", finalValue);
		});
	}
}
#endif

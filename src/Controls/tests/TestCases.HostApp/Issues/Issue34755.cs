using System.Reflection;
using Microsoft.Maui.Graphics.Platform;
using IImage = Microsoft.Maui.Graphics.IImage;

namespace Maui.Controls.Sample.Issues;

[Issue(IssueTracker.Github, 34755, "Image resized with ResizeMode.Fit is not rendered correctly in GraphicsView on iOS", PlatformAffected.iOS)]
public class Issue34755 : ContentPage
{
	public Issue34755()
	{
		var graphicsView = new GraphicsView
		{
			HeightRequest = 300,
			WidthRequest = 300,
			AutomationId = "ResizeFitGraphicsView",
			Drawable = new Issue34755Drawable()
		};

		var readyLabel = new Label
		{
			Text = "Test passes if the image is rendered correctly in the GraphicsView above. The image should be resized to fit within a area while maintaining its aspect ratio.",
			AutomationId = "ReadyLabel"
		};

		Content = new VerticalStackLayout
		{
			Padding = 20,
			Spacing = 10,
			Children = { readyLabel, graphicsView }
		};
	}
}

class Issue34755Drawable : IDrawable
{
	public void Draw(ICanvas canvas, RectF dirtyRect)
	{
		IImage image;
		var assembly = typeof(Issue34755Drawable).GetTypeInfo().Assembly;
		using (var stream = assembly.GetManifestResourceStream("Controls.TestCases.HostApp.Resources.Images.royals.png"))
		{
			image = PlatformImage.FromStream(stream);
		}

		if (image is not null)
		{
			var resizedImage = image.Resize(100, 200, ResizeMode.Fit);
			canvas.SetFillImage(resizedImage);
			canvas.FillRectangle(0, 0, 200, resizedImage.Height);
		}
	}
}

namespace Maui.Controls.Sample.Issues;

[Issue(IssueTracker.Github, 16149, "NavigationPage TitleView does not use the available width on Windows", PlatformAffected.UWP)]
public class Issue16149 : NavigationPage
{
	public Issue16149() : base(CreatePage())
	{
	}

	static ContentPage CreatePage()
	{
		var titleView = new Grid
		{
			AutomationId = "TitleView",
			BackgroundColor = Colors.LightGray,
			HeightRequest = 40,
			HorizontalOptions = LayoutOptions.Fill
		};

		titleView.Add(new Label
		{
			Text = "Start",
			HorizontalOptions = LayoutOptions.Start,
			VerticalOptions = LayoutOptions.Center
		});
		titleView.Add(new Label
		{
			Text = "TitleView",
			HorizontalOptions = LayoutOptions.Center,
			VerticalOptions = LayoutOptions.Center
		});
		titleView.Add(new Image
		{
			AutomationId = "TrailingImage",
			Source = "dotnet_bot.png",
			WidthRequest = 32,
			HeightRequest = 32,
			HorizontalOptions = LayoutOptions.End,
			VerticalOptions = LayoutOptions.Center
		});

		var page = new ContentPage
		{
			Title = "TitleView",
			Content = new Label
			{
				Text = "The trailing image in the title bar should be fully visible.",
				Margin = 16
			}
		};

		NavigationPage.SetTitleView(page, titleView);
		return page;
	}
}

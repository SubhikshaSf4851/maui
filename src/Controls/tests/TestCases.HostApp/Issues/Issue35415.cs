namespace Maui.Controls.Sample.Issues;

[Issue(IssueTracker.Github, 35415, "BackButtonBehavior IsVisible=False should hide flyout hamburger icon on Android and iOS", PlatformAffected.Android | PlatformAffected.iOS)]
public class Issue35415 : Shell
{
	public Issue35415()
	{
		FlyoutBehavior = FlyoutBehavior.Flyout;

		Items.Add(new ShellContent
		{
			Title = "Home",
			Route = "issue35415home",
			Content = new Issue35415RootPage()
		});
	}
}

public class Issue35415RootPage : ContentPage
{
	public Issue35415RootPage()
	{
		Routing.RegisterRoute("issue35415nav", typeof(Issue35415NavPage));

		Content = new VerticalStackLayout
		{
			Padding = new Thickness(16),
			Spacing = 12,
			Children =
			{
				new Label
				{
					Text = "Issue 35415 Home",
					AutomationId = "Issue35415HomeLabel"
				},
				new Button
				{
					Text = "Navigate",
					AutomationId = "NavigateButton",
					Command = new Command(async () =>
					{
						var page = new Issue35415NavPage();
						Shell.SetBackButtonBehavior(page, new BackButtonBehavior { IsVisible = false });
						await Shell.Current.Navigation.PushAsync(page);
					})
				}
			}
		};
	}
}

public class Issue35415NavPage : ContentPage
{
	public Issue35415NavPage()
	{
		Content = new VerticalStackLayout
		{
			Padding = new Thickness(16),
			Children =
			{
				new Label
				{
					Text = "Second Page - back button and flyout icon should be hidden",
					AutomationId = "Issue35415SecondPageLabel"
				}
			}
		};
	}
}

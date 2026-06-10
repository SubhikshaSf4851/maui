namespace Maui.Controls.Sample.Issues;

[Issue(IssueTracker.Github, 21646, "FlyoutIconIsVisible: hamburger icon should be controllable per-page on Android", PlatformAffected.Android)]
public class Issue21646 : TestShell
{
	protected override void Init()
	{
		var rootPage = new Issue21646RootPage();
		var flyoutItem = new FlyoutItem { Title = "Root" };
		flyoutItem.Items.Add(new Tab
		{
			Items =
			{
				new ShellContent
				{
					Title = "Root",
					Content = rootPage
				}
			}
		});
		Items.Add(flyoutItem);

		Routing.RegisterRoute(nameof(Issue21646SecondPage), typeof(Issue21646SecondPage));
		Routing.RegisterRoute(nameof(Issue21646ThirdPage), typeof(Issue21646ThirdPage));
	}
}

public class Issue21646RootPage : ContentPage
{
	public Issue21646RootPage()
	{
		Title = "Root Page";

		var layout = new VerticalStackLayout { Padding = 20, Spacing = 15 };

		layout.Add(new Label
		{
			Text = "Root Page — hamburger should be visible",
			AutomationId = "RootPageLabel"
		});

		var hideShellLevelButton = new Button
		{
			Text = "Hide Hamburger (Shell level)",
			AutomationId = "HideShellLevelButton"
		};
		hideShellLevelButton.Clicked += (s, e) => Shell.Current.FlyoutIconIsVisible = false;

		var showShellLevelButton = new Button
		{
			Text = "Show Hamburger (Shell level)",
			AutomationId = "ShowShellLevelButton"
		};
		showShellLevelButton.Clicked += (s, e) => Shell.Current.FlyoutIconIsVisible = true;

		var pushPageButton = new Button
		{
			Text = "Push Second Page (back hidden, hamburger hidden per-page)",
			AutomationId = "PushSecondPageButton"
		};
		pushPageButton.Clicked += async (s, e) => await Shell.Current.GoToAsync(nameof(Issue21646SecondPage));

		var pushThirdPageButton = new Button
		{
			Text = "Push Third Page (back hidden, hamburger visible per-page)",
			AutomationId = "PushThirdPageButton"
		};
		pushThirdPageButton.Clicked += async (s, e) => await Shell.Current.GoToAsync(nameof(Issue21646ThirdPage));

		layout.Add(hideShellLevelButton);
		layout.Add(showShellLevelButton);
		layout.Add(pushPageButton);
		layout.Add(pushThirdPageButton);

		Content = layout;
	}
}

public class Issue21646SecondPage : ContentPage
{
	public Issue21646SecondPage()
	{
		Title = "Second Page";
		// Hide back button and hide hamburger for this page only
		Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = false });
		Shell.SetFlyoutIconIsVisible(this, false);

		Content = new VerticalStackLayout
		{
			Padding = 20,
			Children =
			{
				new Label
				{
					Text = "Second Page — back hidden, hamburger hidden (per-page)",
					AutomationId = "SecondPageLabel"
				},
				new Button
				{
					Text = "Go Back",
					AutomationId = "GoBackButton",
					Command = new Command(async () => await Shell.Current.GoToAsync(".."))
				}
			}
		};
	}
}

public class Issue21646ThirdPage : ContentPage
{
	public Issue21646ThirdPage()
	{
		Title = "Third Page";
		// Hide back button but keep hamburger visible for this page
		Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = false });
		Shell.SetFlyoutIconIsVisible(this, true);

		Content = new VerticalStackLayout
		{
			Padding = 20,
			Children =
			{
				new Label
				{
					Text = "Third Page — back hidden, hamburger VISIBLE (per-page)",
					AutomationId = "ThirdPageLabel"
				},
				new Button
				{
					Text = "Go Back",
					AutomationId = "GoBackThirdButton",
					Command = new Command(async () => await Shell.Current.GoToAsync(".."))
				}
			}
		};
	}
}

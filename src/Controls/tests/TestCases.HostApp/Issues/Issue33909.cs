namespace Maui.Controls.Sample.Issues;

[Issue(IssueTracker.Github, 33909, "[iOS, Android, Catalyst] Shell.ForegroundColor does not reset correctly for the back button", PlatformAffected.iOS | PlatformAffected.Android | PlatformAffected.macOS)]
public class Issue33909 : TestShell
{
    protected override void Init()
    {
        // Disable flyout to hide hamburger menu
        FlyoutBehavior = FlyoutBehavior.Disabled;

        var mainPage = CreateMainPage();

        AddContentPage(mainPage, "Main");
        // Register route to second page
        Routing.RegisterRoute("secondpage", typeof(Issue33909SecondPage));
    }

    private ContentPage CreateMainPage()
    {
        var applyColorButton = new Button
        {
            Text = "Apply Red ForegroundColor",
            AutomationId = "ApplyColorButton"
        };

        var removeColorButton = new Button
        {
            Text = "Remove ForegroundColor (Reset)",
            AutomationId = "RemoveColorButton"
        };

        var navigateButton = new Button
        {
            Text = "Navigate to Second Page",
            AutomationId = "NavigateButton"
        };

        applyColorButton.Clicked += (sender, e) =>
        {
            // Apply red foreground color
            Shell.SetForegroundColor(this, Colors.Red);
        };

        removeColorButton.Clicked += (sender, e) =>
        {
            // Reset to null (should reset to platform default)
            Shell.SetForegroundColor(this, null);
        };

        navigateButton.Clicked += async (sender, e) =>
        {
            await Shell.Current.GoToAsync("secondpage");
        };

        return new ContentPage
        {
            Title = "Shell ForegroundColor Test",
            Content = new VerticalStackLayout
            {
                Margin = new Thickness(20),
                Spacing = 20,
                Children =
                {
                    applyColorButton,
                    removeColorButton,
                    navigateButton
                }
            }
        };
    }
}

// Separate page class for navigation
public class Issue33909SecondPage : ContentPage
{
    public Issue33909SecondPage()
    {
        Title = "Second Page";
        Content = new VerticalStackLayout
        {
            Margin = new Thickness(20),
            Spacing = 20,
            Children =
            {
                new Label
                {
                    Text = "After Reset ForegroundColor, the back button should be default color",
                    FontSize = 16,
                    HorizontalOptions = LayoutOptions.Center,
                    AutomationId = "SecondPageLabel"
                }
            }
        };
    }
}
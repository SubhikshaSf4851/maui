using Microsoft.Maui.Controls;

namespace Maui.Controls.Sample.Issues;
[Issue(IssueTracker.Github, "", "CharacterSpacing should be applied", PlatformAffected.Android)]
public class CharacterSpacingIssue : TestShell
{
    protected override void Init()
    {
        var shellContent = new ShellContent
        {
            Title = "Home",
            Content = new Issue28634ContentPage() { Title = "Home" }
        };

        Items.Add(shellContent);
    }
    class Issue28634ContentPage : ContentPage
    {
        public Issue28634ContentPage()
        {

            var searchHandler = new SearchHandler
            {
                CharacterSpacing = 10
            };

            var button = new Button
            {
                Text = "Enter Text",
                AutomationId = "Entertext",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            };
            button.Clicked += (s, e) =>
            {
                searchHandler.Query = "Hello World";
            };

            Shell.SetSearchHandler(this, searchHandler);

            Content = button;
        }
    }
}
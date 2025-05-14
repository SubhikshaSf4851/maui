namespace Maui.Controls.Sample.Issues;
[XamlCompilation(XamlCompilationOptions.Compile)]
[Issue(IssueTracker.Github, "", "CharacterSpacing should be applied", PlatformAffected.Android)]  //Need to add Issue ID
public partial class CharacterSpacingIssue : Shell
{
    public CharacterSpacingIssue()
    {
        InitializeComponent();
    }

    private void BtnClicked(object sender, EventArgs e)
    {
        searchHandler.Query = "Hello World";
    }
}
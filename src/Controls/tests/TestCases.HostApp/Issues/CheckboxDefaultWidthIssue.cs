namespace Maui.Controls.Sample.Issues;

[Issue(IssueTracker.Github, , "[Windows] CheckBox control Default Width Issue", PlatformAffected.UWP)]
public class CheckboxDefaultWidthIssue : ContentPage
{
	public CheckboxDefaultWidthIssue()
	{
		var Checkbox = new CheckBox
		{
			VerticalOptions = LayoutOptions.Center,
			Margin = new Thickness(0, 0, 0, 20),
			AutomationId = "CheckBoxControl"
		};
		

		var label = new Label
		{
			Text = "Test passes if there is no space between checkbox and label",
			VerticalOptions = LayoutOptions.Center,
		};

		var horizontalStackLayout = new HorizontalStackLayout
		{
			Children =
			{
				label,
				Checkbox,
			},
		};

		Content = horizontalStackLayout;
	}
}
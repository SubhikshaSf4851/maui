using Microsoft.Maui.Controls.Shapes;

namespace Maui.Controls.Sample.Issues;

[Issue(IssueTracker.Github, 35645, "RadioButton resource keys not applied from App.Resources", PlatformAffected.All)]
public class Issue35645 : ContentPage
{
	RadioButton _radioButton;
	Label _resultLabel;

	public Issue35645()
	{
		// Inject the custom resource BEFORE creating the RadioButton so that
		// BuildDefaultTemplate() picks it up via TryGetResource when the template
		// is applied for the first time.
		var redBrush = new SolidColorBrush(Color.FromArgb("#00860b"));
		Application.Current.Resources["RadioButtonOuterEllipseStrokeLight"] = redBrush;
		Application.Current.Resources["RadioButtonOuterEllipseStrokeDark"] = redBrush;
		Application.Current.Resources["RadioButtonCheckGlyphFillLight"] = redBrush;
		Application.Current.Resources["RadioButtonCheckGlyphFillDark"] = redBrush;

		_radioButton = new RadioButton
		{
			AutomationId = "TestRadioButton",
			Content = "Test RadioButton",
			IsChecked = true
		};

		_resultLabel = new Label
		{
			AutomationId = "ResultLabel",
			Text = "Test passes if radio button's outer ellipse stroke and fill are the custom resource color #00860b. "
		};


		Content = new VerticalStackLayout
		{
			Padding = 10,
			Spacing = 10,
			Children = { _radioButton, _resultLabel }
		};
	}
}

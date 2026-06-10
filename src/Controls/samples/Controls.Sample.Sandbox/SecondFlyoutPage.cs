namespace Maui.Controls.Sample;

public class SecondFlyoutPage : ContentPage
{
	public SecondFlyoutPage()
	{
		Title = "Page Two";
		Content = new VerticalStackLayout
		{
			Padding = 20,
			Children =
			{
				new Label { Text = "This is Page Two (second FlyoutItem)." },
				new Label { Text = "Go back via the flyout → Home." }
			}
		};
	}
}

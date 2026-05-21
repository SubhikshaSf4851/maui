namespace Maui.Controls.Sample;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	private async void OnNextPageClicked(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new MainPage2());
	}
}

public partial class MainPage2 : ContentPage
{
	public MainPage2()
	{
		Shell.SetFlyoutIconIsVisible(this, false);
		Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = false });
		Content = new VerticalStackLayout
		{
			Children =
			{
				new Label
				{
					Text = "This is the second page",
					VerticalOptions = LayoutOptions.Center,
					HorizontalOptions = LayoutOptions.Center
				}
			}
		};
	}
}
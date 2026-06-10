namespace Maui.Controls.Sample;

public partial class MainPage : ContentPage
{
	Label _statusLabel;

	public MainPage()
	{
		InitializeComponent();

		_statusLabel = new Label
		{
			FontSize = 13,
			Margin = new Thickness(0, 8, 0, 0),
			TextColor = Colors.DarkSlateGray
		};
		// Insert status label at the bottom of the VerticalStackLayout defined in XAML
		((VerticalStackLayout)Content).Children.Add(_statusLabel);
		UpdateStatus();
	}

	void UpdateStatus()
	{
		var shell = Shell.Current;
		var bbv = Shell.GetBackButtonBehavior(this)?.IsVisible;
		_statusLabel.Text =
			$"FlyoutIconIsVisible: {shell?.FlyoutIconIsVisible}\n" +
			$"BackButtonBehavior.IsVisible: {bbv?.ToString() ?? "(not set — defaults true)"}";
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		UpdateStatus();
	}

	private void OnButtonClicked(object sender, EventArgs e)
	{
		var shell = Shell.Current;
		shell.FlyoutIconIsVisible = !shell.FlyoutIconIsVisible;
		UpdateStatus();
	}

	private async void OnGoButtonClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync(nameof(NonRootPage));
	}
}

public class NonRootPage : ContentPage
{
	Label _statusLabel;

	public NonRootPage()
	{
		Shell.SetFlyoutIconIsVisible(this, false);
		Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = true });
		_statusLabel = new Label
		{
			FontSize = 13,
			Margin = new Thickness(0, 8, 0, 0),
			TextColor = Colors.DarkSlateGray
		};

		var stack = new VerticalStackLayout();
		stack.Children.Add(new Label { Text = "This is a non root page" });
		stack.Children.Add(_statusLabel);
		stack.Children.Add(new Button
		{
			Text = "Toggle Hamburger icon visibility (per-page)",
			Command = new Command(() =>
			{
				Shell.SetFlyoutIconIsVisible(this, !Shell.GetFlyoutIconIsVisible(this));
				UpdateStatus();
			})
		});
		stack.Children.Add(new Button
		{
			Text = "Show Back Button",
			Command = new Command(() =>
			{
				Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = true });
				UpdateStatus();
			})
		});
		stack.Children.Add(new Button
		{
			Text = "Hide Back Button",
			Command = new Command(() =>
			{
				Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = false });
				UpdateStatus();
			})
		});
		stack.Children.Add(new Button
		{
			Text = "Go to another page",
			Command = new Command(async () =>
			{
				await Shell.Current.GoToAsync(nameof(NonRootPage2));
			})
		});
		Content = stack;
	}

	void UpdateStatus()
	{
		var bbv = Shell.GetBackButtonBehavior(this)?.IsVisible;
		_statusLabel.Text =
			$"Page FlyoutIconIsVisible: {Shell.GetFlyoutIconIsVisible(this)}\n" +
			$"Shell FlyoutIconIsVisible: {Shell.Current?.FlyoutIconIsVisible}\n" +
			$"BackButtonBehavior.IsVisible: {bbv?.ToString() ?? "(not set — defaults true)"}";
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		UpdateStatus();
	}
}

public class NonRootPage2 : ContentPage
{
	Label _statusLabel;

	public NonRootPage2()
	{
		Shell.SetFlyoutIconIsVisible(this, true);
		Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = false });
		_statusLabel = new Label
		{
			FontSize = 13,
			Margin = new Thickness(0, 8, 0, 0),
			TextColor = Colors.DarkSlateGray
		};

		var stack = new VerticalStackLayout();
		stack.Children.Add(new Label { Text = "This is non root page 2" });
		stack.Children.Add(_statusLabel);
		stack.Children.Add(new Button
		{
			Text = "Toggle Hamburger icon visibility",
			Command = new Command(() =>
			{
				Shell.SetFlyoutIconIsVisible(this, !Shell.GetFlyoutIconIsVisible(this));
				UpdateStatus();
			})
		});
		stack.Children.Add(new Button
		{
			Text = "Show Back Button",
			Command = new Command(() =>
			{
				Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = true });
				UpdateStatus();
			})
		});
		stack.Children.Add(new Button
		{
			Text = "Hide Back Button",
			Command = new Command(() =>
			{
				Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = false });
				UpdateStatus();
			})
		});
		Content = stack;
	}

	void UpdateStatus()
	{
		var bbv = Shell.GetBackButtonBehavior(this)?.IsVisible;
		_statusLabel.Text =
			$"FlyoutIconIsVisible: {Shell.GetFlyoutIconIsVisible(this)}\n" +
			$"BackButtonBehavior.IsVisible: {bbv?.ToString() ?? "(not set — defaults true)"}";
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		UpdateStatus();
	}
}
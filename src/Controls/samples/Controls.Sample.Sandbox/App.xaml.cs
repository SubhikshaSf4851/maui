namespace Maui.Controls.Sample;

public partial class App : Application
{
	public static MauiApp? MauiApp { get; set; }

	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		// To test shell scenarios, change this to true
		bool useShell = false;

		if (!useShell)
		{
			var mainPage = MauiApp?.Services.GetRequiredService<MainPage>();
			if (mainPage != null)
			{
				return new Window(new NavigationPage(mainPage));
			}
			return new Window();
		}
		else
		{
			return new Window(new SandboxShell());
		}
	}
}

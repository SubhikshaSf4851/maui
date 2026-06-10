namespace Maui.Controls.Sample;

public partial class SandboxShell : Shell
{
	public SandboxShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(NonRootPage), typeof(NonRootPage));
		Routing.RegisterRoute(nameof(NonRootPage2), typeof(NonRootPage2));
	}
}

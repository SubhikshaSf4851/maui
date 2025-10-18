using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Maui.Controls.Sample.Issues
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	[Issue(IssueTracker.None, 0, "Image Tapped is not triggered when setting HeightRequest to the Image", PlatformAffected.All)]
	public partial class ImageTapGestureWithHeightRequestTest : ContentPage
	{
		private int _tapCount = 0;
		private string _lastTappedImage = "";

		public ImageTapGestureWithHeightRequestTest()
		{
			InitializeComponent();
		}

		void OnImageTapped(object sender, TappedEventArgs e)
		{
			_tapCount++;
			if (sender is Image image)
			{
				_lastTappedImage = image.AutomationId ?? "Unknown";
			}
			ResultLabel.Text = $"Tapped {_lastTappedImage} - Count: {_tapCount}";
		}

		public int TapCount => _tapCount;
		public string LastTappedImage => _lastTappedImage;
	}
}
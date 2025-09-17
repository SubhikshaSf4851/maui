using System.Collections.ObjectModel;

namespace Controls.TestCases.HostApp.Issues;

[Issue(IssueTracker.Github, 14898, "IndicatorView should not hide when ItemsSource is updated", PlatformAffected.iOS)]
public class Issue14898 : NavigationPage
{
	public Issue14898() : base(new Issue14898MainPage())
	{
	}

	public class Issue14898MainPage : ContentPage
	{
		ContentPage _Issue14898NavigatedPage = new Issue14898NavigatedPage();
		public Issue14898MainPage()
		{
			Title = "Main Page";

			var navigateButton = new Button
			{
				AutomationId = "navigateButton",
				Text = "Go to new Page",
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};

			navigateButton.Clicked += async (s, e) =>
			{
				await Navigation.PushAsync(_Issue14898NavigatedPage);
			};

			Content = new VerticalStackLayout
			{
				Children =
			{
				navigateButton
			}
			};
		}
	}
}

public class Issue14898NavigatedPage : ContentPage
{
	readonly CarouselViewModel _viewModel = new CarouselViewModel();

	public Issue14898NavigatedPage()
	{
		Title = "New Page";

		var indicatorView = new IndicatorView
		{
			AutomationId = "indicatorView",
			IndicatorColor = Colors.LightGray,
			SelectedIndicatorColor = Colors.DarkMagenta,
			IndicatorSize = 10,
			HideSingle = false,
			HorizontalOptions = LayoutOptions.Center,
			VerticalOptions = LayoutOptions.Center,
			IndicatorTemplate = new DataTemplate(() =>
			{
				return new Label
				{
					Text = "\uf30c",
					FontFamily = "ionicons",
					FontSize = 12,
					HorizontalOptions = LayoutOptions.Center,
					VerticalOptions = LayoutOptions.Center
				};
			})
		};

		var carouselView = new CarouselView
		{
			HeightRequest = 150,
			WidthRequest = 250,
			HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
			IndicatorView = indicatorView,
			ItemTemplate = new DataTemplate(() =>
			{
				var layout = new VerticalStackLayout
				{
					BackgroundColor = Colors.PaleGoldenrod,
					HorizontalOptions = LayoutOptions.Fill,
					VerticalOptions = LayoutOptions.Fill
				};

				var label = new Label
				{
					HorizontalOptions = LayoutOptions.Center,
					VerticalOptions = LayoutOptions.Center,
					TextColor = Colors.Black
				};
				label.SetBinding(Label.TextProperty, ".");

				layout.Children.Add(label);
				return layout;
			})
		};
		carouselView.SetBinding(CarouselView.ItemsSourceProperty, nameof(CarouselViewModel.ItemList));

		Button btn = new Button
		{
			Text = "Go Back",
			AutomationId = "backButton",
			Command = new Command(async () => await Navigation.PopAsync())
		};

		Content = new VerticalStackLayout
		{
			Spacing = 15,
			Padding = 20,
			Children =
			{
				new Label
				{
					Text = "Test passed if IndicatorView is visible after navigating back and forth",
					HorizontalOptions = LayoutOptions.Center
				},
				carouselView,
				indicatorView,
				btn
			}
		};

		BindingContext = _viewModel;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		_viewModel.UpdateCarousel();
	}
}

public class CarouselViewModel
{
	public int AppearingCount = 0;
	public ObservableCollection<string> ItemList { get; set; }

	public CarouselViewModel()
	{
		ItemList = new ObservableCollection<string>
		{
			"Initial"
		};
	}
	public void UpdateCarousel()
	{
		if (AppearingCount == 0)
			ItemList.Add("new item");
		AppearingCount++;
	}
}
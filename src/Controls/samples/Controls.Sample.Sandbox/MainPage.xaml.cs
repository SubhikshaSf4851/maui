using CommunityToolkit.Mvvm.ComponentModel;
namespace Maui.Controls.Sample;

public partial class MainPage : ContentPage
{
	public MainPage(MainPageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}

public partial class MainPageViewModel : ObservableObject
{
	[ObservableProperty]
	private string? _selectedAnimal;

	[ObservableProperty]
	private string? _selectedAnimal2;

	[ObservableProperty]
	private string? _selectedBasicAnimal;

	[ObservableProperty]
	private string _selectionText = "Please select your favorite animal";

	[ObservableProperty]
	private string _selection2Text = "Please select your favorite animal";

	[ObservableProperty]
	private string _basicSelectionText = "Please select your favorite animal";

	partial void OnSelectedAnimalChanged(string? value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			SelectionText = $"You selected: {value}";
		}
	}

	partial void OnSelectedAnimal2Changed(string? value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			Selection2Text = $"You selected: {value}";
		}
	}

	partial void OnSelectedBasicAnimalChanged(string? value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			BasicSelectionText = $"You selected: {value}";
		}
	}
}
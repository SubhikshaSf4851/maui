﻿using System.Collections.ObjectModel;

namespace Maui.Controls.Sample.Issues;

[Issue(IssueTracker.Github, 29529, "CurrentItemChangedEventArgs and PositionChangedEventArgs Not Updating Correctly in CarouselView", PlatformAffected.UWP)]
public class Issue29529 : ContentPage
{
	public Issue29529()
	{
		var verticalStackLayout = new VerticalStackLayout();
		var carouselItems = new ObservableCollection<string>
		{
			"Item 1",
			"Item 2",
			"Item 3",
			"Item 4",
			"Item 5",
			"Item 6",
		};

		CarouselView carouselView = new CarouselView
		{
			ItemsSource = carouselItems,
			AutomationId = "carouselview",
			ItemsUpdatingScrollMode = ItemsUpdatingScrollMode.KeepItemsInView,
			Loop = false,
			HeightRequest = 300,
			ItemTemplate = new DataTemplate(() =>
			{
				var grid = new Grid
				{
					Padding = 10
				};

				var label = new Label
				{
					VerticalOptions = LayoutOptions.Center,
					HorizontalOptions = LayoutOptions.Center,
					FontSize = 18,
				};
				label.SetBinding(Label.TextProperty, ".");
				label.SetBinding(Label.AutomationIdProperty, ".");

				grid.Children.Add(label);
				return grid;
			}),
			HorizontalOptions = LayoutOptions.Fill,
		};

		var positionLabel = new Label
		{
			AutomationId = "positionLabel",
			Text = $"Current Position{carouselView.Position}",
			HorizontalOptions = LayoutOptions.Center,
			Padding = new Thickness(20),
		};

		var itemLabel = new Label
		{
			AutomationId = "itemLabel",
			Text = $"Current Item{carouselView.CurrentItem}",
			HorizontalOptions = LayoutOptions.Center,
			Padding = new Thickness(20),
		};

		carouselView.PositionChanged += (s, e) =>
		{
			positionLabel.Text = $"Current Position: {e.CurrentPosition}, Previous Position: {e.PreviousPosition}";
		};

		carouselView.CurrentItemChanged += (s, e) =>
		{
			itemLabel.Text = $"Current Item: {e.CurrentItem}, Previous Item: {e.PreviousItem}";
		};

		var insertButton = new Button
		{
			Text = "Insert item at index 0",
			AutomationId = "InsertButton",
			Margin = new Thickness(20),
		};

		insertButton.Clicked += (sender, e) =>
		{
			carouselItems.Insert(0, "Item 0");
		};

		verticalStackLayout.Children.Add(carouselView);
		verticalStackLayout.Children.Add(insertButton);
		verticalStackLayout.Children.Add(positionLabel);
		verticalStackLayout.Children.Add(itemLabel);
		Content = verticalStackLayout;
	}
}
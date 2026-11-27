namespace Maui.Controls.Sample.Issues;

[Issue(IssueTracker.Github, 26910, "Button WordWrap leaves extra blank space in CollectionView after rotation", PlatformAffected.All)]
public class Issue26910 : ContentPage
{
	public Issue26910()
	{
		Title = "Layout of line break mode";

		var grid = new Grid
		{
			Margin = 20,
			RowDefinitions =
			{
				new RowDefinition { Height = GridLength.Auto },
				new RowDefinition { Height = GridLength.Star }
			}
		};

		grid.Add(new Label
		{
			Text = "1.The test passes if the content of the long text items are loaded correctly. Rotate between portrait and landscape twice; the items should resize without extra blank space."
		}, 0, 0);

		var collectionView = new CollectionView
		{
			AutomationId = "Issue26910CollectionView",
			SelectionMode = SelectionMode.None,
			ItemsSource = new[] { 1, 2, 3, 4 },
			ItemTemplate = new DataTemplate(() =>
			{
				var itemGrid = new Grid
				{
					ColumnDefinitions =
					{
						new ColumnDefinition { Width = GridLength.Auto },
						new ColumnDefinition { Width = GridLength.Auto },
						new ColumnDefinition { Width = GridLength.Auto },
						new ColumnDefinition { Width = GridLength.Star },
						new ColumnDefinition { Width = GridLength.Auto },
						new ColumnDefinition { Width = GridLength.Auto },
						new ColumnDefinition { Width = GridLength.Auto }
					}
				};

				for (var column = 0; column < 7; column++)
				{
					if (column == 3)
						continue;

					var label = new Label { Text = $"C{column + 1}" };
					itemGrid.Add(label, column, 0);
				}

				var button = new Button
				{
					LineBreakMode = LineBreakMode.WordWrap,
					Background = Colors.LightBlue,
					TextColor = Colors.Black,
					Text = "Ainsi, à cause que nos sens nous trompent quelquefois, je voulus supposer qu'il n'y avait aucune chose qui fût telle qu'ils nous la font imaginer; Et parce qu'il y a des hommes qui se méprennent en raisonnant, même touchant les plus simples matières de Géométrie, et y font des Paralogismes, jugeant que j'étais sujet à faillir autant qu'aucun autre, je rejetai comme fausses toutes les raisons que j'avais prises auparavant pour Démonstrations"
				};
				button.SetBinding(AutomationIdProperty, new Binding(".", stringFormat: "Issue26910Button{0}"));
				itemGrid.Add(button, 3, 0);

				return itemGrid;
			})
		};

		grid.Add(collectionView, 0, 1);
		Content = grid;
	}
}

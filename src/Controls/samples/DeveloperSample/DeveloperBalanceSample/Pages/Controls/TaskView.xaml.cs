using System.Windows.Input;
using DeveloperBalanceSample.Models;

namespace DeveloperBalanceSample.Pages.Controls;

public partial class TaskView
{
	public TaskView()
	{
		InitializeComponent();
	}

	public static readonly BindableProperty TaskCompletedCommandProperty = BindableProperty.Create(
		nameof(TaskCompletedCommand),
		typeof(ICommand),
		typeof(TaskView),
		null);

	public ICommand TaskCompletedCommand
	{
		get => (ICommand)GetValue(TaskCompletedCommandProperty);
		set => SetValue(TaskCompletedCommandProperty, value);
	}

	private void CheckBox_CheckedChanged(object? sender, CheckedChangedEventArgs e)
	{
		if (sender is not CheckBox { BindingContext: ProjectTask task })
			return;
		
		if (task.IsCompleted == e.Value)
			return;

		task.IsCompleted = e.Value;
		TaskCompletedCommand?.Execute(task);
	}
}
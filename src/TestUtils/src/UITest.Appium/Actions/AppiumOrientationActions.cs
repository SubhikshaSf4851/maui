﻿using UITest.Core;

namespace UITest.Appium
{
	public class AppiumOrientationActions : ICommandExecutionGroup
	{
		const string SetOrientationPortraitCommand = "setOrientationPortrait";
		const string SetOrientationLandscapeCommand = "setOrientationLandscape";
		const string GetOrientationCommand = "getOrientation";

		protected readonly AppiumApp _app;

		readonly List<string> _commands = new()
		{
			SetOrientationPortraitCommand,
			SetOrientationLandscapeCommand,
			GetOrientationCommand,
		};

		public AppiumOrientationActions(AppiumApp app)
		{
			_app = app;
		}

		public bool IsCommandSupported(string commandName)
		{
			return _commands.Contains(commandName, StringComparer.OrdinalIgnoreCase);
		}

		public CommandResponse Execute(string commandName, IDictionary<string, object> parameters)
		{
			return commandName switch
			{
				SetOrientationPortraitCommand => SetOrientationPortrait(parameters),
				SetOrientationLandscapeCommand => SetOrientationLandscape(parameters),
				GetOrientationCommand => GetOrientation(parameters),
				_ => CommandResponse.FailedEmptyResponse,
			};
		}

		CommandResponse SetOrientationPortrait(IDictionary<string, object> parameters)
		{
			return new CommandResponse(_app.Driver.Orientation = OpenQA.Selenium.ScreenOrientation.Portrait, CommandResponseResult.Success);
		}

		CommandResponse SetOrientationLandscape(IDictionary<string, object> parameters)
		{
			return new CommandResponse(_app.Driver.Orientation = OpenQA.Selenium.ScreenOrientation.Landscape, CommandResponseResult.Success);
		}

		CommandResponse GetOrientation(IDictionary<string, object> parameters)
		{
			return new CommandResponse(_app.Driver.Orientation, CommandResponseResult.Success);
		}
	}
}
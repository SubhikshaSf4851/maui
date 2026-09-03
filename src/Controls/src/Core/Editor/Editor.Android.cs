#nullable disable
using Microsoft.Maui.Controls.Platform;

namespace Microsoft.Maui.Controls
{
	public partial class Editor
	{
		static void MapAutoSize(IEditorHandler handler, Editor editor)
		{
			if (handler.PlatformView is Microsoft.Maui.Platform.MauiAppCompatEditText editText)
			{
				editText.AllowAutoGrowth = editor.AutoSize == EditorAutoSizeOption.TextChanges;
			}
		}

		public static void MapText(EditorHandler handler, Editor editor) =>
			MapText((IEditorHandler)handler, editor);

		public static void MapText(IEditorHandler handler, Editor editor)
		{
			if (handler is ViewHandler viewHandler && viewHandler.DataFlowDirection == DataFlowDirection.FromPlatform)
			{
				Platform.EditTextExtensions.UpdateTextFromPlatform(handler.PlatformView, editor);
				return;
			}

			Platform.EditTextExtensions.UpdateText(handler.PlatformView, editor);
		}

		public static void MapText(EditorHandler2 handler, Editor editor)
		{
			if (handler.PlatformView is null)
			{
				return;
			}

			if (handler.DataFlowDirection == DataFlowDirection.FromPlatform)
			{
				Platform.EditTextExtensions.UpdateTextFromPlatform(handler.PlatformView, editor);
				return;
			}

			Platform.EditTextExtensions.UpdateText(handler.PlatformView, editor);
		}
	}
}

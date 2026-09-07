using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace Microsoft.Maui
{
	public class FontManager : IFontManager
	{
		const string SystemFontFamily = "ContentControlThemeFontFamily";
		const string SystemFontSize = "ControlContentThemeFontSize";

		const string TypicalFontAssetsPath = "Assets/Fonts/";
		static readonly string[] TypicalFontFileExtensions = new[]
		{
			".ttf",
			".otf",
		};

		readonly ConcurrentDictionary<string, FontFamily> _fonts = new();
		readonly IFontRegistrar _fontRegistrar;
		readonly IServiceProvider? _serviceProvider;

		/// <remarks>Value is cached to avoid the performance hit of accessing <see cref="ResourceDictionary"/> many times.</remarks>
		FontFamily? _defaultFontFamily;

		/// <remarks>Value is cached to avoid the performance hit of accessing <see cref="ResourceDictionary"/> many times.</remarks>
		double? _defaultFontSize;

		/// <summary>
		/// Creates a new <see cref="EmbeddedFontLoader"/> instance.
		/// </summary>
		/// <param name="fontRegistrar">An <see cref="IFontRegistrar"/> instance for retrieving details about the registered fonts.</param>
		/// <param name="serviceProvider">The applications <see cref="IServiceProvider"/>.
		/// Typically this is provided through dependency injection.</param>
		public FontManager(IFontRegistrar fontRegistrar, IServiceProvider? serviceProvider = null)
		{
			_fontRegistrar = fontRegistrar;
			_serviceProvider = serviceProvider;
		}

		/// <inheritdoc/>
		public FontFamily DefaultFontFamily
		{
			get
			{
				_defaultFontFamily ??= (FontFamily)Application.Current.Resources[SystemFontFamily];
				return _defaultFontFamily;
			}
		}

		/// <inheritdoc/>
		public double DefaultFontSize
		{
			get
			{
				_defaultFontSize ??= (double)Application.Current.Resources[SystemFontSize];
				return _defaultFontSize.Value;
			}
		}

		/// <inheritdoc/>
		public FontFamily GetFontFamily(Font font)
		{
			if (font.IsDefault || string.IsNullOrWhiteSpace(font.Family))
				return DefaultFontFamily;

			return _fonts.GetOrAdd(font.Family, CreateFontFamily);
		}

		/// <inheritdoc/>
		public double GetFontSize(Font font, double defaultFontSize = 0) =>
			font.Size <= 0 || double.IsNaN(font.Size)
				? (defaultFontSize > 0 ? defaultFontSize : DefaultFontSize)
				: font.Size;

		FontFamily CreateFontFamily(string fontFamily)
		{
			var formatted = string.Join(", ", GetAllFontPossibilities(fontFamily));

			var font = new FontFamily(formatted);

			return font;
		}

		IEnumerable<string> GetAllFontPossibilities(string fontFamily)
		{
			// First check Alias
			if (_fontRegistrar.GetFont(fontFamily) is string fontPostScriptName)
			{
				if (fontPostScriptName.Contains("://", StringComparison.Ordinal) && fontPostScriptName.Contains('#', StringComparison.Ordinal))
				{
					// The registrar has given us a perfect path, so use it exactly
					yield return fontPostScriptName;
				}
				else
				{
					var familyName = FindFontFamilyName(fontPostScriptName);
					var file = FontFile.FromString(Path.GetFileName(fontPostScriptName));
					var formatted = $"{fontPostScriptName}#{familyName ?? file.GetPostScriptNameWithSpaces()}";

					yield return formatted;
				}
				yield break;
			}

			var fontFile = FontFile.FromString(fontFamily);

			// If the extension is provided, they know what they want!
			var hasExtension = !string.IsNullOrWhiteSpace(fontFile.Extension);
			if (hasExtension)
			{
				if (_fontRegistrar.GetFont(fontFile.FileNameWithExtension()) is string filePath)
				{
					var familyName = FindFontFamilyName(filePath);
					var formatted = $"{filePath}#{familyName ?? fontFile.GetPostScriptNameWithSpaces()}";

					yield return formatted;
					yield break;
				}
				else
				{
					yield return $"{TypicalFontAssetsPath}{fontFile.FileNameWithExtension()}";
				}
			}

			// There was no extension so let's just try a few things
			foreach (var ext in TypicalFontFileExtensions)
			{
				if (_fontRegistrar.GetFont(fontFile.FileNameWithExtension(ext)) is string filePath)
				{
					var familyName = FindFontFamilyName(filePath);
					var formatted = $"{filePath}#{familyName ?? fontFile.GetPostScriptNameWithSpaces()}";

					yield return formatted;
					yield break;
				}
			}

			// Always send the base back
			yield return fontFamily;

			// And then just wing it with each extension
			foreach (var ext in TypicalFontFileExtensions)
			{
				var fileName = $"{TypicalFontAssetsPath}{fontFile.FileNameWithExtension(ext)}";
				var familyName = FindFontFamilyName(fileName);
				var formatted = $"{fileName}#{familyName ?? fontFile.GetPostScriptNameWithSpaces()}";

				yield return formatted;
			}
		}

		string? FindFontFamilyName(string? fontFile)
		{
			if (fontFile == null)
				return null;

			try
			{
				if (!System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported)
				{
					// Win2D font metadata APIs can crash under Native AOT, so read the OpenType
					// family name directly instead of incorrectly falling back to the file name.
					return FindFontFamilyNameFromFile(fontFile);
				}

				var fontUri = new Uri(fontFile, UriKind.RelativeOrAbsolute);

				// Win2D in unpackaged apps can't load files using packaged schemes, such as `ms-appx://`
				// so we have to first convert it to a `file://` scheme will the full file path.
				// At this part of the load operation, the font URI does NOT yet have the font family name
				// fragment component, so we don't have to remove it.
				if (!AppInfoUtils.IsPackagedApp)
				{
					var path = fontUri.LocalPath.TrimStart('/');
					if (FileSystemUtils.TryGetAppPackageFileUri(path, out var uri))
						fontUri = new Uri(uri, UriKind.RelativeOrAbsolute);
				}

				using (var fontSet = new CanvasFontSet(fontUri))
				{
					if (fontSet.Fonts.Count != 0)
					{
						var props = fontSet.GetPropertyValues(CanvasFontPropertyIdentifier.FamilyName);
						return props.Length == 0 ? null : props[0].Value;
					}
				}

				return null;
			}
			catch (Exception ex)
			{
				// Font metadata lookup can fail if the font file is missing or malformed. It should not crash the app.

				_serviceProvider?.CreateLogger<FontManager>()?.LogError(ex, "Error loading font '{Font}'.", fontFile);

				return null;
			}
		}

		static string? FindFontFamilyNameFromFile(string fontFile)
		{
			var fontPath = GetFontFilePath(fontFile);
			if (fontPath == null)
				return null;

			using var stream = File.OpenRead(fontPath);
			using var reader = new BinaryReader(stream);

			if (stream.Length < 12)
				return null;

			ReadUInt32BigEndian(reader);
			var tableCount = ReadUInt16BigEndian(reader);
			stream.Seek(6, SeekOrigin.Current);

			if (tableCount > (stream.Length - stream.Position) / 16)
				return null;

			uint nameTableOffset = 0;
			uint nameTableLength = 0;

			for (var i = 0; i < tableCount; i++)
			{
				var tag = ReadUInt32BigEndian(reader);
				ReadUInt32BigEndian(reader);
				var offset = ReadUInt32BigEndian(reader);
				var length = ReadUInt32BigEndian(reader);

				if (tag == 0x6E616D65) // "name"
				{
					nameTableOffset = offset;
					nameTableLength = length;
					break;
				}
			}

			if (nameTableLength < 6 ||
			nameTableOffset > stream.Length ||
			nameTableLength > stream.Length - nameTableOffset)
			{
				return null;
			}

			stream.Seek(nameTableOffset, SeekOrigin.Begin);
			ReadUInt16BigEndian(reader);
			var nameCount = ReadUInt16BigEndian(reader);
			var stringStorageOffset = ReadUInt16BigEndian(reader);

			if ((long)nameCount * 12 > nameTableLength - 6 ||
			stringStorageOffset > nameTableLength)
			{
				return null;
			}

			ushort selectedLength = 0;
			ushort selectedOffset = 0;
			var selectedScore = 0;

			for (var i = 0; i < nameCount; i++)
			{
				var platformId = ReadUInt16BigEndian(reader);
				var encodingId = ReadUInt16BigEndian(reader);
				var languageId = ReadUInt16BigEndian(reader);
				var nameId = ReadUInt16BigEndian(reader);
				var length = ReadUInt16BigEndian(reader);
				var offset = ReadUInt16BigEndian(reader);

				if (nameId != 1)
					continue;

				var score = GetFontNameScore(platformId, encodingId, languageId);
				if (score > selectedScore)
				{
					selectedScore = score;
					selectedLength = length;
					selectedOffset = offset;
				}
			}

			var relativeStringPosition = (long)stringStorageOffset + selectedOffset;
			var stringPosition = nameTableOffset + relativeStringPosition;
			if (selectedScore == 0 ||
			selectedLength == 0 ||
			relativeStringPosition > nameTableLength ||
			selectedLength > nameTableLength - relativeStringPosition ||
			stringPosition > stream.Length ||
			selectedLength > stream.Length - stringPosition)
			{
				return null;
			}

			stream.Seek(stringPosition, SeekOrigin.Begin);
			var value = reader.ReadBytes(selectedLength);
			if (value.Length != selectedLength)
				return null;

			var familyName = System.Text.Encoding.BigEndianUnicode.GetString(value).TrimEnd('\0');
			return string.IsNullOrWhiteSpace(familyName) ? null : familyName;
		}

		static string? GetFontFilePath(string fontFile)
		{
			if (Path.IsPathRooted(fontFile))
				return fontFile;

			var fontUri = new Uri(fontFile, UriKind.RelativeOrAbsolute);
			if (fontUri.IsAbsoluteUri && fontUri.IsFile)
				return fontUri.LocalPath;

			var relativePath = fontUri.IsAbsoluteUri ? fontUri.LocalPath : fontFile;
			relativePath = Uri.UnescapeDataString(relativePath)
			.TrimStart('/', '\\')
			.Replace('/', Path.DirectorySeparatorChar);

			return FileSystemUtils.PlatformGetFullAppPackageFilePath(relativePath);
		}

		static int GetFontNameScore(ushort platformId, ushort encodingId, ushort languageId)
		{
			if (platformId == 3)
			{
				var languageScore = languageId == 0x0409 ? 20 : 10;
				var encodingScore = encodingId == 10 ? 2 : encodingId == 1 ? 1 : 0;
				return 300 + languageScore + encodingScore;
			}

			return platformId == 0 ? 200 : 0;
		}

		static ushort ReadUInt16BigEndian(BinaryReader reader)
		{
			var high = reader.ReadByte();
			var low = reader.ReadByte();
			return (ushort)((high << 8) | low);
		}

		static uint ReadUInt32BigEndian(BinaryReader reader)
		{
			var high = ReadUInt16BigEndian(reader);
			var low = ReadUInt16BigEndian(reader);
			return ((uint)high << 16) | low;
		}
	}
}
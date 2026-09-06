using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;

namespace Llyn.UIShell;

internal static class PThemeLoader
{
    private const string PThemeLoaderResource = "Llyn.UIShell.Themes.default.json";

    private static readonly IReadOnlyDictionary<string, string> PThemeLoaderColors =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["ink"] = "Theme.Ink.Color",
            ["muted"] = "Theme.Muted.Color",
            ["canvas"] = "Theme.Canvas.Color",
            ["roof"] = "Theme.Roof.Color",
            ["navigation"] = "Theme.Navigation.Color",
            ["surface"] = "Theme.Surface.Color",
            ["surfaceRaised"] = "Theme.SurfaceRaised.Color",
            ["line"] = "Theme.Line.Color",
            ["accent"] = "Theme.Accent.Color",
            ["accentSoft"] = "Theme.AccentSoft.Color",
            ["warning"] = "Theme.Warning.Color",
            ["warningSoft"] = "Theme.WarningSoft.Color",
            ["warningStrong"] = "Theme.WarningStrong.Color"
        };

    internal static void PThemeLoaderApply(ResourceDictionary resources)
    {
        Assembly assembly = typeof(PThemeLoader).Assembly;
        using Stream stream = assembly.GetManifestResourceStream(PThemeLoaderResource)
            ?? throw new InvalidDataException(
                $"The embedded theme '{PThemeLoaderResource}' could not be found.");
        using JsonDocument document = JsonDocument.Parse(stream);

        if (!document.RootElement.TryGetProperty("colors", out JsonElement colors) ||
            colors.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidDataException("The theme file must contain a 'colors' object.");
        }

        foreach ((string jsonName, string resourceName) in PThemeLoaderColors)
        {
            if (!colors.TryGetProperty(jsonName, out JsonElement value) ||
                value.ValueKind != JsonValueKind.String)
            {
                throw new InvalidDataException($"The theme color '{jsonName}' is missing.");
            }

            string colorText = value.GetString()!;

            try
            {
                resources[resourceName] = (Color)ColorConverter.ConvertFromString(colorText);
            }
            catch (FormatException exception)
            {
                throw new InvalidDataException(
                    $"The theme color '{jsonName}' has an invalid value: '{colorText}'.",
                    exception);
            }
        }
    }
}

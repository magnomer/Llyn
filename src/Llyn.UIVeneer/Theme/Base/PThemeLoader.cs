using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Llyn.UIVeneer;

internal static class PThemeLoader
{
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
            ["lineStrong"] = "Theme.LineStrong.Color",
            ["accent"] = "Theme.Accent.Color",
            ["accentHover"] = "Theme.AccentHover.Color",
            ["accentPress"] = "Theme.AccentPress.Color",
            ["accentEdge"] = "Theme.AccentEdge.Color",
            ["accentSoft"] = "Theme.AccentSoft.Color",
            ["helper"] = "Theme.Helper.Color",
            ["helperSoft"] = "Theme.HelperSoft.Color",
            ["helperEdge"] = "Theme.HelperEdge.Color",
            ["situation"] = "Theme.Situation.Color",
            ["situationSoft"] = "Theme.SituationSoft.Color",
            ["situationEdge"] = "Theme.SituationEdge.Color",
            ["warning"] = "Theme.Warning.Color",
            ["warningSoft"] = "Theme.WarningSoft.Color",
            ["warningStrong"] = "Theme.WarningStrong.Color",
            ["favorite"] = "Theme.Favorite.Color",
            ["representative"] = "Theme.Representative.Color",
            ["disabledLine"] = "Theme.DisabledLine.Color",
            ["disabledInk"] = "Theme.DisabledInk.Color",
            ["contourTop"] = "Theme.ContourTop.Color",
            ["contourHigh"] = "Theme.ContourHigh.Color",
            ["contourMid"] = "Theme.ContourMid.Color",
            ["contourLow"] = "Theme.ContourLow.Color",
            ["contourBottom"] = "Theme.ContourBottom.Color",
            ["frequencyCore"] = "Theme.FrequencyCore.Color",
            ["frequencyEveryday"] = "Theme.FrequencyEveryday.Color",
            ["frequencyAdvanced"] = "Theme.FrequencyAdvanced.Color",
            ["frequencyRare"] = "Theme.FrequencyRare.Color",
            ["pending"] = "Theme.Pending.Color"
        };

    internal static void PThemeLoaderApply(Func<string, string> colorRead, ResourceDictionary resources)
    {
        ArgumentNullException.ThrowIfNull(colorRead);

        foreach ((string jsonName, string resourceName) in PThemeLoaderColors)
        {
            string colorText = colorRead(jsonName);

            try
            {
                resources[resourceName] = (Color)ColorConverter.ConvertFromString(colorText);
            }
            catch (FormatException exception)
            {
                throw new InvalidOperationException(
                    $"The theme color '{jsonName}' has an invalid value: '{colorText}'.",
                    exception);
            }
        }
    }
}

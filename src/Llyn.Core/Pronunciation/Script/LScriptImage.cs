using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LScriptImage(
    string LScriptImageCharacter,
    string LScriptImageStyle,
    int LScriptImagePosition,
    string LScriptImageCaption,
    string LScriptImageGloss,
    byte[] LScriptImageData,
    string LScriptImageEpoch = "",
    long LScriptImageId = 0)
{
    public bool LScriptImageMatch(string character, string style)
    {
        return string.Equals(LScriptImageCharacter, character, StringComparison.Ordinal)
            && string.Equals(LScriptImageStyle, style, StringComparison.Ordinal);
    }

    public static IReadOnlyList<string> LScriptCharacterScan(IReadOnlyList<LScriptImage> images)
    {
        ArgumentNullException.ThrowIfNull(images);

        List<string> characters = [];
        foreach (LScriptImage image in images)
        {
            if (!characters.Contains(image.LScriptImageCharacter))
            {
                characters.Add(image.LScriptImageCharacter);
            }
        }

        return characters;
    }

    public static IReadOnlyList<LScriptImage>? LScriptImageScan(
        IReadOnlyList<LScriptImage> images, string character, string style)
    {
        ArgumentNullException.ThrowIfNull(images);

        List<LScriptImage> found = [];
        foreach (LScriptImage image in images)
        {
            if (image.LScriptImageMatch(character, style))
            {
                found.Add(image);
            }
        }

        return found.Count == 0 ? null : found;
    }

    public static IReadOnlyList<LScriptImage> LScriptImageSort(
        IReadOnlyList<LScriptImage> images, IReadOnlyList<LScriptStyle> styles, IReadOnlyList<string> spelled)
    {
        ArgumentNullException.ThrowIfNull(images);
        ArgumentNullException.ThrowIfNull(styles);
        ArgumentNullException.ThrowIfNull(spelled);

        Dictionary<string, int> characters = new(StringComparer.Ordinal);
        foreach (string character in spelled)
        {
            characters.TryAdd(character, characters.Count);
        }

        Dictionary<string, int> named = new(StringComparer.Ordinal);
        Dictionary<string, Dictionary<string, int>> epochs = new(StringComparer.Ordinal);
        foreach (LScriptStyle style in styles)
        {
            if (!named.TryAdd(style.LScriptStyleName, named.Count))
            {
                continue;
            }

            Dictionary<string, int> declared = new(StringComparer.Ordinal);
            foreach (LEpoch epoch in style.LScriptStyleEpoch)
            {
                declared.TryAdd(epoch.LEpochCode, declared.Count);
            }

            epochs[style.LScriptStyleName] = declared;
        }

        return images
            .OrderBy(image => characters.GetValueOrDefault(image.LScriptImageCharacter, int.MaxValue))
            .ThenBy(static image => image.LScriptImageCharacter, LGlyphOrder.LGlyphOrderComparer)
            .ThenBy(image => named.GetValueOrDefault(image.LScriptImageStyle, int.MaxValue))
            .ThenBy(static image => image.LScriptImageStyle, StringComparer.Ordinal)
            .ThenBy(image => image.LScriptImageEpoch.Length == 0
                ? int.MaxValue
                : epochs.GetValueOrDefault(image.LScriptImageStyle)?
                    .GetValueOrDefault(image.LScriptImageEpoch, int.MaxValue - 1) ?? int.MaxValue - 1)
            .ThenBy(static image => image.LScriptImageEpoch, StringComparer.Ordinal)
            .ThenBy(static image => image.LScriptImageId)
            .ToList();
    }
}

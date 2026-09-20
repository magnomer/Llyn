using System;
using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LScriptImage(
    string LScriptImageCharacter,
    string LScriptImageStyle,
    int LScriptImagePosition,
    string LScriptImageCaption,
    string LScriptImageGloss,
    byte[] LScriptImageData)
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
}

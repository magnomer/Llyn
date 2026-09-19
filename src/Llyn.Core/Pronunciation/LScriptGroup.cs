using System;
using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LScriptGroup(
    string LScriptGroupHeading,
    string LScriptGroupStyle,
    string LScriptGroupGloss,
    IReadOnlyList<LScriptImage> LScriptGroupImages)
{
    public static IReadOnlyList<LScriptGroup> LScriptGroupScan(
        IReadOnlyList<LScriptImage> images, IReadOnlyList<LScriptStyle> styles)
    {
        ArgumentNullException.ThrowIfNull(images);
        ArgumentNullException.ThrowIfNull(styles);

        IReadOnlyList<string> characters = LScriptImage.LScriptCharacterScan(images);
        List<LScriptGroup> groups = [];
        foreach (string character in characters)
        {
            bool first = true;
            foreach (LScriptStyle style in styles)
            {
                if (LScriptImage.LScriptImageScan(images, character, style.LScriptStyleName)
                    is not IReadOnlyList<LScriptImage> found)
                {
                    continue;
                }

                string heading = first && characters.Count > 1 ? character : string.Empty;
                string gloss = string.Empty;
                foreach (LScriptImage image in found)
                {
                    if (image.LScriptImageGloss.Length > 0)
                    {
                        gloss = image.LScriptImageGloss;
                        break;
                    }
                }

                groups.Add(new LScriptGroup(heading, style.LScriptStyleName, gloss, found));
                first = false;
            }
        }

        return groups;
    }
}

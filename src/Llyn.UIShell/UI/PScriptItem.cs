using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PScriptItem
{
    private PScriptItem(string character, string style, string gloss, IReadOnlyList<PScriptImage> images)
    {
        PScriptItemCharacter = character;
        PScriptItemStyle = style;
        PScriptItemGloss = gloss;
        PScriptItemImages = images;
    }

    public string PScriptItemCharacter { get; }

    public string PScriptItemStyle { get; }

    public string PScriptItemGloss { get; }

    public IReadOnlyList<PScriptImage> PScriptItemImages { get; }

    internal static IReadOnlyList<PScriptItem> PScriptItemScan(
        IReadOnlyList<LScriptImage> images, IReadOnlyList<LScriptStyle> styles)
    {
        ArgumentNullException.ThrowIfNull(images);
        ArgumentNullException.ThrowIfNull(styles);

        List<string> characters = [];
        Dictionary<(string, string), List<LScriptImage>> groups = [];
        foreach (LScriptImage image in images)
        {
            if (!characters.Contains(image.LScriptImageCharacter))
            {
                characters.Add(image.LScriptImageCharacter);
            }

            (string, string) key = (image.LScriptImageCharacter, image.LScriptImageStyle);
            if (!groups.TryGetValue(key, out List<LScriptImage>? group))
            {
                group = [];
                groups[key] = group;
            }

            group.Add(image);
        }

        List<PScriptItem> items = [];
        foreach (string character in characters)
        {
            bool first = true;
            foreach (LScriptStyle style in styles)
            {
                if (!groups.TryGetValue((character, style.LScriptStyleName), out List<LScriptImage>? group))
                {
                    continue;
                }

                string heading = first && characters.Count > 1 ? character : string.Empty;
                PScriptItem? item = PScriptItemCreate(heading, style.LScriptStyleName, group);
                if (item is not null)
                {
                    items.Add(item);
                    first = false;
                }
            }
        }

        return items;
    }

    private static PScriptItem? PScriptItemCreate(string character, string style, IReadOnlyList<LScriptImage> group)
    {
        List<PScriptImage> pictures = [];
        string gloss = string.Empty;
        foreach (LScriptImage image in group)
        {
            if (PScriptImage.PScriptImageCreate(image) is PScriptImage picture)
            {
                pictures.Add(picture);
            }

            if (gloss.Length == 0)
            {
                gloss = image.LScriptImageGloss;
            }
        }

        return pictures.Count == 0 ? null : new PScriptItem(character, style, gloss, pictures);
    }
}

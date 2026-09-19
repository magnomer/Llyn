using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.UIVeneer;

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

    internal static IReadOnlyList<PScriptItem> PScriptItemScan(IReadOnlyList<LScriptGroup> groups)
    {
        ArgumentNullException.ThrowIfNull(groups);

        List<PScriptItem> items = new(groups.Count);
        foreach (LScriptGroup group in groups)
        {
            if (PScriptItemCreate(group) is PScriptItem item)
            {
                items.Add(item);
            }
        }

        return items;
    }

    private static PScriptItem? PScriptItemCreate(LScriptGroup group)
    {
        List<PScriptImage> pictures = [];
        foreach (LScriptImage image in group.LScriptGroupImages)
        {
            if (PScriptImage.PScriptImageCreate(image) is PScriptImage picture)
            {
                pictures.Add(picture);
            }
        }

        return pictures.Count == 0
            ? null
            : new PScriptItem(group.LScriptGroupHeading, group.LScriptGroupStyle, group.LScriptGroupGloss, pictures);
    }
}

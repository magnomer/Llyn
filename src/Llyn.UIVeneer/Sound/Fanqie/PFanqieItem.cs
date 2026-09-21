using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.UIVeneer;

internal sealed class PFanqieItem
{
    private PFanqieItem(
        string character,
        string book,
        string source,
        IReadOnlyList<string> stems,
        IReadOnlyList<PFanqieLine> lines)
    {
        PFanqieItemCharacter = character;
        PFanqieItemBook = book;
        PFanqieItemSource = source;
        PFanqieItemStems = stems;
        PFanqieItemLines = lines;
    }

    public string PFanqieItemCharacter { get; }

    public string PFanqieItemBook { get; }

    public string PFanqieItemSource { get; }

    public IReadOnlyList<string> PFanqieItemStems { get; }

    public IReadOnlyList<PFanqieLine> PFanqieItemLines { get; }

    internal static IReadOnlyList<PFanqieItem> PFanqieItemScan(IReadOnlyList<LFanqieGroup> groups)
    {
        ArgumentNullException.ThrowIfNull(groups);

        List<PFanqieItem> items = new(groups.Count);
        foreach (LFanqieGroup group in groups)
        {
            List<PFanqieLine> lines = new(group.LFanqieGroupRows.Count);
            foreach (LFanqieRow row in group.LFanqieGroupRows)
            {
                lines.Add(PFanqieLine.PFanqieLineCreate(row));
            }

            items.Add(new PFanqieItem(
                group.LFanqieGroupHeading,
                group.LFanqieGroupLabel,
                group.LFanqieGroupSource,
                group.LFanqieGroupStems,
                lines));
        }

        return items;
    }
}

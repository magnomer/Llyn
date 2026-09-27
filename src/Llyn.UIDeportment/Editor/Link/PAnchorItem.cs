using System;
using System.Collections.Generic;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class PAnchorItem
{
    private PAnchorItem(CAnchorRow row)
    {
        PAnchorItemId = row.CAnchorRowId;
        PAnchorItemLabel = row.CAnchorRowSummary;
        PAnchorItemAnchored = row.CAnchorRowHeld;
        PAnchorItemEstimated = row.CAnchorRowEstimated;
    }

    public long PAnchorItemId { get; }

    public string PAnchorItemLabel { get; }

    public bool PAnchorItemAnchored { get; }

    public bool PAnchorItemEstimated { get; }

    internal static IReadOnlyList<PAnchorItem> PAnchorItemScan(IReadOnlyList<CAnchorRow> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        List<PAnchorItem> items = new(rows.Count);
        foreach (CAnchorRow row in rows)
        {
            items.Add(new PAnchorItem(row));
        }

        return items;
    }
}

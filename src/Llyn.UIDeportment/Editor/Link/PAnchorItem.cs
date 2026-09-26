using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.UIDeportment;

internal sealed class PAnchorItem
{
    private PAnchorItem(LAnchorRow row)
    {
        PAnchorItemId = row.LAnchorRowFanqie.LFanqieRowId;
        PAnchorItemLabel = row.LAnchorRowFanqie.LFanqieRowSummary;
        PAnchorItemAnchored = row.LAnchorRowHeld;
        PAnchorItemEstimated = row.LAnchorRowEstimated;
    }

    public long PAnchorItemId { get; }

    public string PAnchorItemLabel { get; }

    public bool PAnchorItemAnchored { get; }

    public bool PAnchorItemEstimated { get; }

    internal static IReadOnlyList<PAnchorItem> PAnchorItemScan(IReadOnlyList<LAnchorRow> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        List<PAnchorItem> items = new(rows.Count);
        foreach (LAnchorRow row in rows)
        {
            items.Add(new PAnchorItem(row));
        }

        return items;
    }
}

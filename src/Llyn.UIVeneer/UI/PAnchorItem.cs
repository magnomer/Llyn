using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.UIVeneer;

internal sealed class PAnchorItem
{
    private const string PAnchorItemSeparator = " · ";

    private PAnchorItem(LFanqieRow row, bool anchored, bool estimated)
    {
        PAnchorItemId = row.LFanqieRowId;
        PAnchorItemLabel = row.LFanqieRowSummary;
        PAnchorItemAnchored = anchored;
        PAnchorItemEstimated = estimated;
    }

    public long PAnchorItemId { get; }

    public string PAnchorItemLabel { get; }

    public bool PAnchorItemAnchored { get; }

    public bool PAnchorItemEstimated { get; }

    internal static IReadOnlyList<PAnchorItem> PAnchorItemScan(
        IReadOnlyList<LFanqieRow> rows, IReadOnlyList<long> anchors, IReadOnlyList<string> classes)
    {
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(anchors);
        ArgumentNullException.ThrowIfNull(classes);

        List<PAnchorItem> items = new(rows.Count);
        foreach (LFanqieRow row in rows)
        {
            if (!row.LFanqieRowStored)
            {
                continue;
            }

            bool estimated = row.LFanqieRowClassed && classes.Contains(row.LFanqieRowClass);
            items.Add(new PAnchorItem(row, anchors.Contains(row.LFanqieRowId), estimated));
        }

        return items;
    }

    internal static string PAnchorTextFormat(IReadOnlyList<LFanqieRow> rows, IReadOnlyList<long> anchors)
    {
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(anchors);

        List<string> readings = [];
        foreach (LFanqieRow row in rows)
        {
            if (!row.LFanqieRowStored)
            {
                continue;
            }

            if (!anchors.Contains(row.LFanqieRowId))
            {
                continue;
            }

            string reading = row.LFanqieRowSpoken ? row.LFanqieRowSlashed : row.LFanqieRowSummary;
            if (!readings.Contains(reading))
            {
                readings.Add(reading);
            }
        }

        return string.Join(PAnchorItemSeparator, readings);
    }
}

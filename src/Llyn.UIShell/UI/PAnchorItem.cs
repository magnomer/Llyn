using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PAnchorItem
{
    private const string PAnchorItemKey = "Display.FanqieTone";

    private const string PAnchorItemSeparator = " · ";

    internal PAnchorItem(long id, string label, bool anchored)
    {
        PAnchorItemId = id;
        PAnchorItemLabel = label;
        PAnchorItemAnchored = anchored;
    }

    public long PAnchorItemId { get; }

    public string PAnchorItemLabel { get; }

    public bool PAnchorItemAnchored { get; }

    internal static IReadOnlyList<PAnchorItem> PAnchorItemScan(
        IReadOnlyList<LFanqieRow> rows, IReadOnlyList<long> anchors)
    {
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(anchors);

        List<PAnchorItem> items = new(rows.Count);
        foreach (LFanqieRow row in rows)
        {
            if (row.LFanqieRowId > 0)
            {
                items.Add(new PAnchorItem(
                    row.LFanqieRowId, PAnchorLabelFormat(row), anchors.Contains(row.LFanqieRowId)));
            }
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
            if (row.LFanqieRowId <= 0 || !anchors.Contains(row.LFanqieRowId))
            {
                continue;
            }

            string reading = row.LFanqieRowReading.Length > 0
                ? '/' + row.LFanqieRowReading + '/'
                : PAnchorLabelFormat(row);
            if (!readings.Contains(reading))
            {
                readings.Add(reading);
            }
        }

        return string.Join(PAnchorItemSeparator, readings);
    }

    internal static string PAnchorLabelFormat(LFanqieRow row)
    {
        ArgumentNullException.ThrowIfNull(row);

        List<string> parts = [row.LFanqieRowBook];
        if (row.LFanqieRowReading.Length > 0)
        {
            parts.Add('/' + row.LFanqieRowReading + '/');
        }

        if (row.LFanqieRowClass.Length > 0)
        {
            string pattern = PLocalizationCatalog.PLocalizationCatalogCurrent[PAnchorItemKey];
            parts.Add(pattern.Length == 0
                ? row.LFanqieRowClass
                : string.Format(CultureInfo.CurrentCulture, pattern, row.LFanqieRowClass));
        }

        if (row.LFanqieRowInitial.Length > 0)
        {
            parts.Add(row.LFanqieRowInitial);
        }

        string rime = LDiwei.LDiweiRimeFormat(row.LFanqieRowRime, row.LFanqieRowDivision, row.LFanqieRowRounded);
        if (rime.Length > 0)
        {
            parts.Add(rime);
        }
        else if (row.LFanqieRowText.Length > 0)
        {
            parts.Add(row.LFanqieRowText);
        }

        return string.Join(' ', parts);
    }
}

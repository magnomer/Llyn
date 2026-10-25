using System;
using System.Collections.Generic;
using System.Globalization;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BLecternIncoming
{
    private readonly BLocalizationCatalog _bLecternCatalog;

    private readonly CDisplayCard _bLecternDisplay;

    private readonly CNavigation _bLecternNavigation;

    private readonly Dictionary<string, BUsageItem> _bLecternIncomingItem = new(StringComparer.Ordinal);

    private IReadOnlyList<CUsage> _bLecternIncomingUsages = [];

    internal BLecternIncoming(BLocalizationCatalog catalog, CDisplayCard display, CNavigation navigation)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(display);
        ArgumentNullException.ThrowIfNull(navigation);

        _bLecternCatalog = catalog;
        _bLecternDisplay = display;
        _bLecternNavigation = navigation;
    }

    internal void BLecternIncomingRefine()
    {
        _bLecternIncomingUsages = _bLecternDisplay.CDisplayIncomingRead();
    }

    internal IReadOnlyList<BLecternLine> BLecternIncomingDraw(IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs)
    {
        ArgumentNullException.ThrowIfNull(glyphs);

        _bLecternIncomingItem.Clear();
        IReadOnlyList<CUsage> usages = _bLecternIncomingUsages;
        if (usages.Count == 0)
        {
            return [];
        }

        string arrow = BLecternHeader.BLecternGlyphRead(glyphs, "Arrow", 1, "<-");
        List<BLecternLine> lines =
        [
            new([(string.Empty, "text", null)], 0),
            new([(_bLecternCatalog.BLocalizationTextRead("Display.Translated"), "heading", null)], 0),
        ];
        foreach (CUsage usage in usages)
        {
            string item = "Usage" + _bLecternIncomingItem.Count.ToString(CultureInfo.InvariantCulture);
            _bLecternIncomingItem.Add(item, new BUsageItem(usage));
            string title = usage.CUsageTitleKey is string key
                ? _bLecternCatalog.BLocalizationTextRead(key)
                : usage.CUsageTitle.CStateValueText;
            string owner = _bLecternCatalog.BLocalizationTextRead(usage.CUsageOwnerKey);
            lines.Add(new BLecternLine(
                [
                    ($"{arrow} {usage.CUsageName}", "link", item),
                    (usage.CUsageEpithet.Length > 0 ? $" {usage.CUsageEpithet}" : string.Empty, "muted", null),
                    ($"   {owner}", "muted", null),
                    (title.Length > 0 ? $"  {title}" : string.Empty, "text", null),
                    ($"   {usage.CUsageLanguage}", "muted", null),
                ],
                0));
        }

        return lines;
    }

    internal string? BLecternIncomingOpen(string item)
    {
        BUsageItem row = _bLecternIncomingItem[item];
        _ = _bLecternNavigation.CNavigationUsageOpen(row.BUsageItemUsage);
        return null;
    }

    internal bool BLecternIncomingCheck(string? item)
    {
        return item is not null && _bLecternIncomingItem.ContainsKey(item);
    }

    private sealed record BUsageItem(CUsage BUsageItemUsage);
}

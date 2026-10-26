using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.UIDemeanor;

internal sealed class BUnit
{
    private const string BUnitTitle = "Unit.Title";

    private readonly BLocalizationCatalog _bUnitCatalog;

    private IReadOnlyList<(string, bool)> _bUnitRows = [];

    internal BUnit(BLocalizationCatalog catalog)
    {
        ArgumentNullException.ThrowIfNull(catalog);

        _bUnitCatalog = catalog;
    }

    internal BDrop? BUnitDrop { get; private set; }

    internal string BUnitName => _bUnitCatalog.BLocalizationTextRead(
        _bUnitRows.Where(static row => row.Item2).Select(static row => row.Item1).DefaultIfEmpty(BUnitTitle).First());

    internal void BUnitRefine(IReadOnlyList<(string, bool)> units)
    {
        ArgumentNullException.ThrowIfNull(units);

        _bUnitRows = units;
    }

    internal void BUnitOpen()
    {
        BUnitDrop = new BDrop(
            [.. _bUnitRows.Select(row => _bUnitCatalog.BLocalizationTextRead(row.Item1))],
            _bUnitRows.ToList().FindIndex(static row => row.Item2),
            true,
            Math.Max(_bUnitRows.Count, 1));
    }

    internal void BUnitSelect(Action<string> take)
    {
        ArgumentNullException.ThrowIfNull(take);

        BDrop drop = BUnitDrop ?? throw new InvalidOperationException("The unit list is not open.");
        BUnitDrop = null;
        take(_bUnitRows[drop.BDropLit].Item1);
    }

    internal void BUnitClose()
    {
        BUnitDrop = null;
    }

    internal bool BUnitObserve(ConsoleKeyInfo key)
    {
        return BUnitDrop?.BDropSeek(key.KeyChar) ?? false;
    }

    internal IEnumerable<BCanvasText> BUnitDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        BCanvasWidth measure,
        BBlueprintPart bounds,
        (int, int) field,
        bool focused)
    {
        ArgumentNullException.ThrowIfNull(glyphs);

        string check = BLecternHeader.BLecternGlyphRead(glyphs, "Check", 0, "[x]");
        string[] marks = [.. _bUnitRows.Select(row => row.Item2 ? check : string.Empty)];
        return BUnitDrop?.BDropDraw(glyphs, measure, bounds, field, (marks, focused)) ?? [];
    }
}

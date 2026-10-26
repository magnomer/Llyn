using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.UIDemeanor;

internal sealed class BFreedom
{
    private const string BFreedomTitle = "Freedom.Title";

    private readonly BLocalizationCatalog _bFreedomCatalog;

    private IReadOnlyList<(string, bool)> _bFreedomRows = [];

    internal BFreedom(BLocalizationCatalog catalog)
    {
        ArgumentNullException.ThrowIfNull(catalog);

        _bFreedomCatalog = catalog;
    }

    internal BDrop? BFreedomDrop { get; private set; }

    internal string BFreedomName => _bFreedomCatalog.BLocalizationTextRead(_bFreedomRows
        .Where(static row => row.Item2).Select(static row => row.Item1).DefaultIfEmpty(BFreedomTitle).First());

    internal void BFreedomRefine(IReadOnlyList<(string, bool)> freedoms)
    {
        ArgumentNullException.ThrowIfNull(freedoms);

        _bFreedomRows = freedoms;
    }

    internal void BFreedomOpen()
    {
        BFreedomDrop = new BDrop(
            [.. _bFreedomRows.Select(row => _bFreedomCatalog.BLocalizationTextRead(row.Item1))],
            _bFreedomRows.ToList().FindIndex(static row => row.Item2),
            true,
            Math.Max(_bFreedomRows.Count, 1));
    }

    internal void BFreedomSelect(Action<string> take)
    {
        ArgumentNullException.ThrowIfNull(take);

        BDrop drop = BFreedomDrop ?? throw new InvalidOperationException("The freedom list is not open.");
        BFreedomDrop = null;
        take(_bFreedomRows[drop.BDropLit].Item1);
    }

    internal void BFreedomClose()
    {
        BFreedomDrop = null;
    }

    internal bool BFreedomObserve(ConsoleKeyInfo key)
    {
        return BFreedomDrop?.BDropSeek(key.KeyChar) ?? false;
    }

    internal IEnumerable<BCanvasText> BFreedomDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        BCanvasWidth measure,
        BBlueprintPart bounds,
        (int, int) field,
        bool focused)
    {
        ArgumentNullException.ThrowIfNull(glyphs);

        string check = BLecternHeader.BLecternGlyphRead(glyphs, "Check", 0, "[x]");
        string[] marks = [.. _bFreedomRows.Select(row => row.Item2 ? check : string.Empty)];
        return BFreedomDrop?.BDropDraw(glyphs, measure, bounds, field, (marks, focused)) ?? [];
    }
}

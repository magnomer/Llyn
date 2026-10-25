using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BLecternEtymology
{
    private const int BLecternEtymologyIndent = 2;

    private readonly BLocalizationCatalog _bLecternEtymologyCatalog;

    private readonly CDisplay _bLecternEtymologyDisplay;

    private readonly Action _bLecternEtymologyChanged;

    private readonly Dictionary<string, BAncestorItem> _bLecternEtymologyItem = new(StringComparer.Ordinal);

    private CEtymologyState _bLecternEtymologyState = new([], [], []);

    internal BLecternEtymology(BLocalizationCatalog catalog, CDisplay display, Action changed)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(display);
        ArgumentNullException.ThrowIfNull(changed);

        _bLecternEtymologyCatalog = catalog;
        _bLecternEtymologyDisplay = display;
        _bLecternEtymologyChanged = changed;
        display.CDisplayOpened += BLecternEtymologyRefine;
        display.CDisplayClosed += BLecternEtymologyRefine;
    }

    internal void BLecternEtymologyRefine()
    {
        _bLecternEtymologyState = _bLecternEtymologyDisplay.CDisplayCard.CDisplayEtymologyRead();
        _bLecternEtymologyChanged();
    }

    internal IReadOnlyList<BLecternLine> BLecternEtymologyDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs, int width, BCanvasWidth measure)
    {
        ArgumentNullException.ThrowIfNull(glyphs);
        ArgumentNullException.ThrowIfNull(measure);

        _bLecternEtymologyItem.Clear();
        CEtymologyState state = _bLecternEtymologyState;
        IReadOnlyList<CAncestor> rows = state.CEtymologyStateAncestors;
        if (rows.Count == 0)
        {
            return [];
        }

        string branch = BLecternHeader.BLecternGlyphRead(glyphs, "Tree", 0, "|-") + " ";
        string last = BLecternHeader.BLecternGlyphRead(glyphs, "Tree", 1, "`-") + " ";
        string trunk = BLecternHeader.BLecternGlyphRead(glyphs, "Tree", 2, "|");
        int step = measure.BCanvasWidthResolve(branch);
        string through = trunk + new string(' ', Math.Max(step - measure.BCanvasWidthResolve(trunk), 0));
        string blank = new(' ', step);
        string borrowed = _bLecternEtymologyCatalog.BLocalizationTextRead("Display.EtymologyBorrowed");
        List<BLecternLine> lines =
        [
            new([(string.Empty, "text", null)], 0),
            new([(_bLecternEtymologyCatalog.BLocalizationTextRead("Display.Etymology"), "heading", null)], 0),
            new([(state.CEtymologyStateHeadword, "text", null)], BLecternEtymologyIndent),
        ];
        List<bool> open = [];
        for (int index = 0; index < rows.Count; index++)
        {
            CAncestor box = rows[index];
            int depth = box.CAncestorDepth;
            int sibling = index + 1;
            while (sibling < rows.Count && rows[sibling].CAncestorDepth > depth)
            {
                sibling++;
            }

            bool final = sibling == rows.Count || rows[sibling].CAncestorDepth < depth;
            string lead = string.Concat(open.Take(depth).Select(more => more ? through : blank));
            while (open.Count <= depth)
            {
                open.Add(false);
            }

            open[depth] = !final;
            string? item = null;
            if (box.CAncestorLinked)
            {
                item = "Ancestor" + _bLecternEtymologyItem.Count.ToString(CultureInfo.InvariantCulture);
                _bLecternEtymologyItem.Add(item, new BAncestorItem(box.CAncestorEntry));
            }

            string relation = box.CAncestorRelation is long kind
                && state.CEtymologyStateRelations.FirstOrDefault(choice => choice.CAncestorChoiceId == kind)
                    is CAncestorChoice chosen
                    ? chosen.CAncestorChoiceKey.Length == 0
                        ? chosen.CAncestorChoiceName
                        : _bLecternEtymologyCatalog.BLocalizationTextRead(chosen.CAncestorChoiceKey)
                    : string.Empty;
            List<(string, string, string?)> pieces = [];
            foreach ((string text, string role, string? owner) in new (string, string, string?)[]
                     {
                         (box.CAncestorLanguage, "muted", null),
                         (box.CAncestorText, item is null ? "text" : "link", item),
                         (box.CAncestorGloss, "muted", null),
                         (relation, "muted", null),
                         (box.CAncestorBorrowed ? borrowed : string.Empty, "muted", null),
                     }.Where(static piece => piece.Item1.Length > 0))
            {
                if (pieces.Count > 0)
                {
                    pieces.Add(("  ", "muted", null));
                }

                pieces.Add((text, role, owner));
            }

            int room = Math.Max(width - BLecternEtymologyIndent - measure.BCanvasWidthResolve(lead) - step, 1);
            IReadOnlyList<BLecternLine> cut = new BLecternLine(pieces, 0).BLecternLineDivide(room, measure);
            for (int at = 0; at < cut.Count; at++)
            {
                string prefix = lead + (at > 0 ? final ? blank : through : final ? last : branch)
                    + new string(' ', cut[at].BLecternLineIndent);
                int leading = prefix.Length - prefix.TrimStart(' ').Length;
                List<(string, string, string?)> row = [];
                if (leading < prefix.Length)
                {
                    row.Add((prefix[leading..], "muted", null));
                }

                row.AddRange(cut[at].BLecternLinePieces);
                lines.Add(new BLecternLine(row, BLecternEtymologyIndent + leading));
            }
        }

        return lines;
    }

    internal string? BLecternEtymologyOpen(string item)
    {
        BAncestorItem box = _bLecternEtymologyItem[item];
        _ = _bLecternEtymologyDisplay.CDisplayRoute.CDisplayChipOpen(box.BAncestorItemEntry);
        return null;
    }

    internal bool BLecternEtymologyCheck(string? item)
    {
        return item is not null && _bLecternEtymologyItem.ContainsKey(item);
    }

    private sealed record BAncestorItem(long? BAncestorItemEntry);
}

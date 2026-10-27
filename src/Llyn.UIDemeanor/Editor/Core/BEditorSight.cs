using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BEditorSight
{
    private readonly BLocalizationCatalog _bEditorSightCatalog;

    private readonly CDesk _bEditorSightGate;

    private readonly string _bEditorSightTitle;

    private readonly BSpeaker _bEditorSightSpeaker;

    private readonly BRegard _bEditorSightRegard;

    private readonly BMarker _bEditorSightMarker;

    private readonly BPlacard _bEditorSightPlacard;

    private readonly IReadOnlyList<BInlet> _bEditorSightInlet;

    private readonly BCard _bEditorSightCard;

    private int _bEditorSightTop;

    internal BEditorSight(
        BLocalizationCatalog catalog,
        CDesk desk,
        string title,
        (BSpeaker, BRegard, BMarker) head,
        (BPlacard, BCard) body)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(title);
        ArgumentNullException.ThrowIfNull(head.Item1);
        ArgumentNullException.ThrowIfNull(head.Item2);
        ArgumentNullException.ThrowIfNull(head.Item3);
        ArgumentNullException.ThrowIfNull(body.Item1);
        ArgumentNullException.ThrowIfNull(body.Item2);

        _bEditorSightCatalog = catalog;
        _bEditorSightGate = desk;
        _bEditorSightTitle = title;
        (_bEditorSightSpeaker, _bEditorSightRegard, _bEditorSightMarker) = head;
        (_bEditorSightPlacard, _bEditorSightCard) = body;
        _bEditorSightInlet = _bEditorSightPlacard.BPlacardInlet;
    }

    internal static IReadOnlyList<string> BEditorSightAdder { get; } = ["Input.MeaningAdd", "Input.CollocationAdd"];

    internal int BEditorSightPage { get; private set; } = 1;

    internal IReadOnlyList<BCanvasText> BEditorSightDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        BBlueprintPart display,
        bool focused,
        BCanvasWidth measure,
        (CEntryDraft?, BField?, int, int, int) state,
        (IReadOnlyList<IReadOnlyList<CStep>>, IReadOnlyList<IReadOnlyList<int>>, bool) form)
    {
        ArgumentNullException.ThrowIfNull(glyphs);
        ArgumentNullException.ThrowIfNull(display);
        ArgumentNullException.ThrowIfNull(measure);

        (CEntryDraft? draft, BField? field, int lit, int stack, int spot) = state;
        (IReadOnlyList<IReadOnlyList<CStep>> steps, IReadOnlyList<IReadOnlyList<int>> chosen, bool owned) = form;
        (int row, int column, int width, int height) = (display.BBlueprintPartRow, display.BBlueprintPartColumn,
            display.BBlueprintPartWidth, display.BBlueprintPartHeight);
        BLocalizationCatalog catalog = _bEditorSightCatalog;
        List<BCanvasText> texts = [];
        if (width <= 0 || height <= 0)
        {
            return texts;
        }

        string headword = draft?.CEntryDraftHeadword ?? string.Empty;
        string named = string.Format(
            CultureInfo.CurrentCulture, catalog.BLocalizationTextRead(_bEditorSightTitle), headword);
        string title = $" {named.Trim()} ";
        int titleCells = measure.BCanvasWidthResolve(title);
        if (row > 0 && titleCells <= width)
        {
            texts.Add(new BCanvasText(
                row - 1, column - 1 + ((width + 2 - titleCells) / 2), titleCells, title, focused ? "title" : "side"));
        }

        if (draft is null)
        {
            return texts;
        }

        BEditorSightPage = Math.Max(1, height - 1);
        int page = BEditorSightPage;
        CDeskChronicle chronicle = _bEditorSightGate.CDeskChronicle;
        string rule = BLecternHeader.BLecternGlyphRead(glyphs, "Rule", 1, "-");
        string[] keys = ["Display.Meaning", "Display.CollocationSingle", "Display.Etymology", "Display.Note"];
        int?[] counts = [.. chosen.Take(2).Select(static listed => (int?)listed.Count), null, null];
        IReadOnlyList<CStep> walk = steps[stack];
        int lead = BCard.BCardLeadRead(stack == 1);
        List<(string, string, int?, int, Func<int, IEnumerable<BCanvasText>>?, int?)> lines = [];
        for (int index = 0; index < walk.Count; index++)
        {
            int item = index == 0 ? index : index + BEditor.BEditorHeadSpan;
            if (index == BEditor.BEditorHeadwordRow)
            {
                lines.Add(draft.CEntryDraftHeadword.Length > 0
                    ? (draft.CEntryDraftHeadword, "text", index, 0, null, null)
                    : (catalog.BLocalizationTextRead("Input.Headword"), "muted", index, 0, null, null));
                lines.Add((string.Empty, "text", BEditor.BEditorLanguageRow, 0, null, null));
                lines.Add((string.Empty, "text", BEditor.BEditorEsteemRow, 0, null, null));
                lines.Add((string.Empty, "text", BEditor.BEditorMarkerRow, 0, null, null));
                lines.Add((string.Concat(Enumerable.Repeat(rule, width)), "muted", null, 0, null, null));
            }
            else if (walk[index] is { CStepCard: CCardDraft card })
            {
                bool folded = card.CCardDraftFolded;
                CStep step = walk[index];
                Func<int, string?> tone = slot => !chronicle.CDeskChronicleRunning ? "muted"
                    : item == lit && slot == spot ? focused ? "lit" : "lit-other"
                    : null;
                lines.Add((string.Empty, "text", item, 0, line => _bEditorSightCard.BCardDraw(
                    glyphs, measure, (line, column, width), (stack == 1, step), (tone(0), focused)), null));
                CStateWording wording = stack == 1 ? card.CCardDraftExpression : card.CCardDraftMeaning;
                if (!folded)
                {
                    lines.Add((string.Empty, "text", item, 1, line => _bEditorSightCard.BCardWording.BCardFieldDraw(
                        glyphs, measure, (line, column, width), ((stack == 1, card.CCardDraftId), wording),
                        (tone(1), focused)), null));
                }

                if (!folded && stack == 1)
                {
                    foreach ((Func<int, IEnumerable<BCanvasText>> paint, int? entry) in
                        _bEditorSightCard.BCardBond.BBondDraw(measure, (column, width), card, tone(2)))
                    {
                        lines.Add((string.Empty, "text", item, 2, paint, entry));
                    }
                }

                for (int part = lead + 1; part <= lead + _bEditorSightInlet.Count && !folded; part++)
                {
                    BInlet chips = _bEditorSightInlet[part - lead - 1];
                    foreach ((Func<int, IEnumerable<BCanvasText>> paint, int? entry) in chips.BInletDraw(
                        glyphs, measure, (column, width), (stack == 1, step), (tone(part), focused)))
                    {
                        lines.Add((string.Empty, "text", item, part, paint, entry));
                    }
                }

                foreach ((int part, Func<int, IEnumerable<BCanvasText>> paint, int? entry) in folded
                    ? (IReadOnlyList<(int, Func<int, IEnumerable<BCanvasText>>, int?)>)[]
                    : _bEditorSightPlacard.BPlacardMediaDraw(
                        glyphs, measure, (column, width), (stack == 1, card), (tone, focused)))
                {
                    lines.Add((string.Empty, "text", item, part, paint, entry));
                }
            }
            else
            {
                string[] tabs = [.. keys.Select((key, at) =>
                {
                    string label = counts[at] is int tally
                        ? $"{catalog.BLocalizationTextRead(key)} {tally.ToString(CultureInfo.CurrentCulture)}"
                        : catalog.BLocalizationTextRead(key);
                    return at == stack ? $"[{label}]" : $" {label} ";
                })];
                int first = 0;
                while (first < stack && measure.BCanvasWidthResolve(string.Concat(tabs[first..(stack + 1)])) > width)
                {
                    first++;
                }

                lines.Add((string.Concat(tabs[first..]), "text", item, 0, null, null));
            }
        }

        if (BEditorSightAdder.ElementAtOrDefault(stack) is string adder)
        {
            lines.Add(($"+ {catalog.BLocalizationTextRead(adder)}", "text", walk.Count + BEditor.BEditorHeadSpan, 0,
                null, null));
        }

        int shone = Math.Max(lines.FindIndex(line => line.Item3 == lit && line.Item4 == spot), 0);
        int close = Math.Min(Math.Max(lines.FindLastIndex(line => line.Item3 == lit), shone), shone + page - 1);
        _bEditorSightTop = Math.Clamp(_bEditorSightTop, Math.Max(close - page + 1, 0), shone);
        int top = _bEditorSightTop;
        int? spoken = null;
        int? marked = null;
        (int, int)? anchored = null;
        for (int at = top; at < Math.Min(lines.Count, top + page); at++)
        {
            (string text, string role, int? item, int part, Func<int, IEnumerable<BCanvasText>>? paint, int? entry) =
                lines[at];
            int line = row + at - top;
            if (paint is not null)
            {
                anchored = entry is int edge ? (line, edge) : anchored;
                texts.AddRange(paint(line));
                continue;
            }

            if (item == BEditor.BEditorHeadwordRow && field is not null)
            {
                texts.AddRange(field.BSParleyBodyDraw(glyphs, line, column, width, 1, focused, "field", measure));
                continue;
            }

            bool shining = item == lit && part == spot;
            if (item is BEditor.BEditorLanguageRow or BEditor.BEditorEsteemRow or BEditor.BEditorMarkerRow)
            {
                spoken = item == BEditor.BEditorLanguageRow ? line : spoken;
                marked = item == BEditor.BEditorMarkerRow ? line : marked;
                string? tone = !chronicle.CDeskChronicleRunning ? "muted"
                    : shining ? focused ? "lit" : "lit-other"
                    : null;
                (int, int, int) place = (line, column, width);
                texts.AddRange(item switch
                {
                    BEditor.BEditorLanguageRow => _bEditorSightSpeaker.BSpeakerDraw(glyphs, measure, place, tone),
                    BEditor.BEditorEsteemRow => _bEditorSightRegard.BRegardDraw(glyphs, measure, place, tone),
                    _ => _bEditorSightMarker.BMarkerDraw(glyphs, measure, place, (tone, focused)),
                });
                continue;
            }

            string shown = measure.BCanvasWidthClamp(text, width);
            if (!chronicle.CDeskChronicleRunning)
            {
                texts.Add(new BCanvasText(line, column, measure.BCanvasWidthResolve(shown), shown, "muted"));
                continue;
            }

            int cells = shining ? width : measure.BCanvasWidthResolve(shown);
            shown = shining ? shown + new string(' ', Math.Max(width - measure.BCanvasWidthResolve(shown), 0)) : shown;
            texts.Add(new BCanvasText(line, column, cells, shown, shining ? focused ? "lit" : "lit-other" : role));
            if (shining && focused && item == BEditor.BEditorHeadwordRow)
            {
                int end = Math.Min(measure.BCanvasWidthResolve(draft.CEntryDraftHeadword), width - 1);
                texts.Add(new BCanvasText(line, column + end, 0, string.Empty, "caret"));
            }
        }

        List<BCanvasText> dropped = [];
        if (_bEditorSightSpeaker.BSpeakerDrop is BDrop drop && spoken is int anchor)
        {
            dropped.AddRange(drop.BDropDraw(glyphs, measure, display, (anchor, column), ([], focused)));
        }
        else
        {
            if (marked is int under)
            {
                dropped.AddRange(
                    _bEditorSightMarker.BMarkerDropDraw(glyphs, measure, display, (under, column), focused));
            }

            dropped.AddRange(_bEditorSightPlacard.BPlacardDropDraw(glyphs, measure, display, (anchored, focused)));
            dropped.AddRange(_bEditorSightCard.BCardBond.BBondDropDraw(glyphs, measure, display, (anchored, focused)));
        }

        int below = lines.Count - top - page;
        if (lines.Count > page)
        {
            int mark = row + (page <= 1 ? 0 : (int)Math.Round((double)top * (page - 1) / (lines.Count - page)));
            string thumb = BLecternHeader.BLecternGlyphRead(glyphs, "Scroll", 1, "#");
            texts.Add(new BCanvasText(
                mark, column + width, measure.BCanvasWidthResolve(thumb), thumb, focused ? "title" : "side"));
        }

        if (height <= 1)
        {
            return [.. texts, .. dropped];
        }

        (bool undo, bool redo) = chronicle.CDeskChronicleState;
        List<(string, string)> status = [];
        if (_bEditorSightGate.CDeskDraft.CDeskDraftAltered)
        {
            status.Add((catalog.BLocalizationTextRead("Editor.Unsaved"), "text"));
        }

        if (chronicle.CDeskChronicleHalted)
        {
            status.Clear();
        }
        else if (owned)
        {
            status.Add((catalog.BLocalizationTextRead("Chronicle.Undo"), undo ? "text" : "muted"));
            status.Add((catalog.BLocalizationTextRead("Chronicle.Redo"), redo ? "text" : "muted"));
        }

        int foot = row + height - 1;
        int left = Math.Max(column + width - status.Sum(piece => measure.BCanvasWidthResolve(piece.Item1) + 2), column);
        int start = column;
        if (below > 0)
        {
            string down = BLecternHeader.BLecternGlyphRead(glyphs, "Order", 1, "v");
            string count = string.Format(
                CultureInfo.CurrentCulture, catalog.BLocalizationTextRead("Editor.More"), below);
            string whole = $"{down} {count}";
            int cells = Math.Min(measure.BCanvasWidthResolve(whole), Math.Max(left - column - 1, 0));
            string more = measure.BCanvasWidthClamp(whole, cells);
            texts.Add(new BCanvasText(foot, column, cells, more, "muted"));
            start = Math.Min(column + cells + 2, left);
        }

        bool prompted = lit == BEditor.BEditorHeadwordRow && field is null;
        string hint = measure.BCanvasWidthClamp(
            chronicle.CDeskChronicleHalted ? catalog.BLocalizationTextRead("Input.HoldFailed")
            : prompted ? catalog.BLocalizationTextRead("Input.Headword")
            : _bEditorSightInlet.ElementAtOrDefault(spot - lead - 1) is BInlet inlet && spot > lead
                && walk.ElementAtOrDefault(lit - BEditor.BEditorHeadSpan) is { CStepCard: CCardDraft held }
                && inlet.BInletBareRead(held)
                ? catalog.BLocalizationTextRead(inlet.BInletHint)
            : string.Empty,
            Math.Max(left - start - 1, 0));
        texts.Add(new BCanvasText(
            foot, start, measure.BCanvasWidthResolve(hint), hint, chronicle.CDeskChronicleHalted ? "text" : "muted"));
        foreach ((string text, string role) in status)
        {
            string shown = measure.BCanvasWidthClamp(text, Math.Max(column + width - left, 0));
            texts.Add(new BCanvasText(foot, left, measure.BCanvasWidthResolve(shown), shown, role));
            left += measure.BCanvasWidthResolve(text) + 2;
        }

        texts.AddRange(dropped);
        return texts;
    }

    internal static List<List<(int, int, string, string, bool)>> BEditorSightDivide(
        IReadOnlyList<(int, (string, string, bool))> tokens, int room, int gap)
    {
        ArgumentNullException.ThrowIfNull(tokens);

        List<List<(int, int, string, string, bool)>> lines = [[]];
        int edge = 0;
        foreach ((int cells, (string text, string role, bool entry)) in tokens)
        {
            if (edge > 0 && edge + cells > room)
            {
                lines.Add([]);
                edge = 0;
            }

            lines[^1].Add((edge, cells, text, role, entry));
            edge += cells + gap;
        }

        return lines;
    }

    internal static IEnumerable<BCanvasText> BEditorSightDraw(
        BCanvasWidth measure,
        (int, int, int) place,
        string? tone,
        IReadOnlyList<(string, string)> pieces)
    {
        ArgumentNullException.ThrowIfNull(measure);
        ArgumentNullException.ThrowIfNull(pieces);

        (int line, int column, int width) = place;
        if (tone is not null)
        {
            string whole = measure.BCanvasWidthClamp(string.Concat(pieces.Select(static piece => piece.Item1)), width);
            yield return new BCanvasText(line, column, width, whole, tone);
            yield break;
        }

        int edge = column;
        foreach ((string piece, string role) in pieces)
        {
            int room = Math.Min(measure.BCanvasWidthResolve(piece), Math.Max(column + width - edge, 0));
            string fit = measure.BCanvasWidthClamp(piece, room);
            yield return new BCanvasText(line, edge, room, fit, role);
            edge += room;
        }
    }
}

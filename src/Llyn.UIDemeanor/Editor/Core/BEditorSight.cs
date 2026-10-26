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

    private int _bEditorSightTop;

    internal BEditorSight(BLocalizationCatalog catalog, CDesk desk, string title)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(title);

        _bEditorSightCatalog = catalog;
        _bEditorSightGate = desk;
        _bEditorSightTitle = title;
    }

    internal int BEditorSightPage { get; private set; } = 1;

    internal IReadOnlyList<BCanvasText> BEditorSightDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        BBlueprintPart display,
        bool focused,
        BCanvasWidth measure,
        (CEntryDraft?, BField?, int, int) state,
        (IReadOnlyList<IReadOnlyList<CStep>>, IReadOnlyList<IReadOnlyList<int>>, bool) form)
    {
        ArgumentNullException.ThrowIfNull(glyphs);
        ArgumentNullException.ThrowIfNull(display);
        ArgumentNullException.ThrowIfNull(measure);

        (CEntryDraft? draft, BField? field, int lit, int stack) = state;
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
        string ellipsis = BLecternHeader.BLecternGlyphRead(glyphs, "Ellipsis", 0, "~");
        string[] keys = ["Display.Meaning", "Display.CollocationSingle", "Display.Etymology", "Display.Note"];
        int?[] counts = [.. chosen.Take(2).Select(static listed => (int?)listed.Count), null, null];
        IReadOnlyList<CStep> walk = steps[stack];
        List<(string, string, int?)> lines = [];
        for (int index = 0; index < walk.Count; index++)
        {
            if (index == BEditor.BEditorHeadwordRow)
            {
                lines.Add(draft.CEntryDraftHeadword.Length > 0
                    ? (draft.CEntryDraftHeadword, "text", index)
                    : (catalog.BLocalizationTextRead("Input.Headword"), "muted", index));
                lines.Add((string.Concat(Enumerable.Repeat(rule, width)), "muted", null));
            }
            else if (walk[index] is { CStepCard: CCardDraft card })
            {
                CStateWording wording = card.CCardDraftTitle;
                string caption = wording.CStateWordingMuted ? catalog.BLocalizationTextRead(keys[stack])
                    : wording.CStateWordingKey is string key ? catalog.BLocalizationTextRead(key)
                    : wording.CStateWordingText;
                string hinge = walk[index].CStepFold is not null
                    ? " " + BLecternHeader.BLecternGlyphRead(
                        glyphs, "Fold", card.CCardDraftFolded ? 1 : 0, card.CCardDraftFolded ? ">" : "v")
                    : string.Empty;
                string rank = card.CCardDraftPosition.ToString(CultureInfo.CurrentCulture);
                string opening = measure.BCanvasWidthShorten(
                    $"{rule}{rule} {rank} {rule} {caption} ",
                    Math.Max(width - measure.BCanvasWidthResolve(hinge) - 1, 0),
                    ellipsis);
                int fill = Math.Max(
                    width - measure.BCanvasWidthResolve(opening) - measure.BCanvasWidthResolve(hinge), 0);
                lines.Add((
                    opening + string.Concat(Enumerable.Repeat(rule, fill)) + hinge,
                    wording.CStateWordingMuted ? "muted" : "heading",
                    index));
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

                lines.Add((string.Concat(tabs[first..]), "text", index));
            }
        }

        int shone = Math.Max(lines.FindIndex(line => line.Item3 == lit), 0);
        _bEditorSightTop = Math.Clamp(_bEditorSightTop, Math.Max(shone - page + 1, 0), shone);
        int top = _bEditorSightTop;
        for (int at = top; at < Math.Min(lines.Count, top + page); at++)
        {
            (string text, string role, int? item) = lines[at];
            int line = row + at - top;
            if (item == BEditor.BEditorHeadwordRow && field is not null)
            {
                texts.AddRange(field.BSParleyBodyDraw(glyphs, line, column, width, 1, focused, "field", measure));
                continue;
            }

            string shown = measure.BCanvasWidthClamp(text, width);
            if (!chronicle.CDeskChronicleRunning)
            {
                texts.Add(new BCanvasText(line, column, measure.BCanvasWidthResolve(shown), shown, "muted"));
                continue;
            }

            bool shining = item == lit;
            int cells = shining ? width : measure.BCanvasWidthResolve(shown);
            shown = shining ? shown + new string(' ', Math.Max(width - measure.BCanvasWidthResolve(shown), 0)) : shown;
            texts.Add(new BCanvasText(line, column, cells, shown, shining ? focused ? "lit" : "lit-other" : role));
            if (shining && focused && item == BEditor.BEditorHeadwordRow)
            {
                int end = Math.Min(measure.BCanvasWidthResolve(draft.CEntryDraftHeadword), width - 1);
                texts.Add(new BCanvasText(line, column + end, 0, string.Empty, "caret"));
            }
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
            return texts;
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

        return texts;
    }
}

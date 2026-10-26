using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BInlet
{
    private const int BInletPage = 6;

    private const int BInletIndent = 4;

    private const string BInletGap = "  ";

    private readonly BLocalizationCatalog _bInletCatalog;

    private readonly string _bInletRole;

    private readonly CCaretTrail _bInletTrail;

    private readonly Func<CCardDraft, IReadOnlyList<(long, string)>> _bInletChip;

    private readonly (
        Func<bool, long, string, (string, IReadOnlyList<BInletRow>)>,
        Func<bool, long, string, (string, IReadOnlyList<BInletRow>)>,
        Func<bool, long, string, (string, IReadOnlyList<BInletRow>)>) _bInletGate;

    private readonly CCaret _bInletCaret;

    private readonly Func<(bool, long)?> _bInletSpot;

    private readonly Action _bInletChanged;

    private (bool, long) _bInletCard;

    private string _bInletRest = string.Empty;

    private readonly BKeymapRow _bInletOpenRow;

    private IReadOnlyList<BInletRow> _bInletRow = [];

    private BDrop? _bInletDrop;

    internal BInlet(
        BLocalizationCatalog catalog,
        (string, string) look,
        (CCaretTrail, Func<CCardDraft, IReadOnlyList<(long, string)>>) chips,
        (
            Func<bool, long, string, (string, IReadOnlyList<BInletRow>)>,
            Func<bool, long, string, (string, IReadOnlyList<BInletRow>)>,
            Func<bool, long, string, (string, IReadOnlyList<BInletRow>)>) gates,
        (CCaret, Func<(bool, long)?>, Func<Func<bool>, Func<BKeymapLive>>, Action) host)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(look.Item1);
        ArgumentNullException.ThrowIfNull(look.Item2);
        ArgumentNullException.ThrowIfNull(chips.Item1);
        ArgumentNullException.ThrowIfNull(chips.Item2);
        ArgumentNullException.ThrowIfNull(gates.Item1);
        ArgumentNullException.ThrowIfNull(gates.Item2);
        ArgumentNullException.ThrowIfNull(gates.Item3);
        ArgumentNullException.ThrowIfNull(host.Item1);
        ArgumentNullException.ThrowIfNull(host.Item2);
        ArgumentNullException.ThrowIfNull(host.Item3);
        ArgumentNullException.ThrowIfNull(host.Item4);

        _bInletCatalog = catalog;
        (BInletHint, _bInletRole) = look;
        (_bInletTrail, _bInletChip) = chips;
        _bInletGate = gates;
        Func<Func<bool>, Func<BKeymapLive>> when;
        (_bInletCaret, _bInletSpot, when, _bInletChanged) = host;

        const BKeymapFocus side = BKeymapFocus.BKeymapFocusEditor;
        Action changed = _bInletChanged;
        Func<Action, Action<ConsoleKeyInfo>> pressed = action => _ =>
        {
            action();
            changed();
        };
        Func<BKeymapLive> menu = when(() => BInletField is not null && _bInletDrop is not null);
        Func<BKeymapLive> chosen = when(() => BInletField is not null && _bInletDrop is not null
            && _bInletCaret.CCaretLanternLit);
        Func<int, Action<ConsoleKeyInfo>> step = way => pressed(() => BInletMenuMove(way));
        Func<BKeymapLive> typing = when(() => BInletField is not null);
        Func<BKeymapLive> idle = when(() => BInletField is null && _bInletSpot() is not null);
        _bInletOpenRow = new(ConsoleKey.Enter, 0, side, "Editor.Open", pressed(BInletOpen), idle);
        BInletKeymap =
        [
            new(ConsoleKey.UpArrow, 0, side, "Window.Help.Item", step(-1), menu),
            new(ConsoleKey.DownArrow, 0, side, "Window.Help.Item", step(1), menu),
            new(ConsoleKey.Enter, 0, side, "Editor.Pick", pressed(BInletMenuSelect), chosen),
            new(ConsoleKey.Enter, 0, side, "Card.ChipAdd", pressed(BInletTextSettle), typing),
            new(ConsoleKey.Escape, 0, side, "Editor.Shut", pressed(BInletMenuClose), menu),
            _bInletOpenRow,
        ];
    }

    internal IReadOnlyList<BKeymapRow> BInletKeymap { get; }

    internal string BInletHint { get; }

    internal BField? BInletField { get; private set; }

    internal bool BInletShut => BInletField is not null && _bInletDrop is null;

    internal void BInletOpen()
    {
        (bool, long) card = _bInletSpot() ?? throw new InvalidOperationException("No chip field is lit.");
        string rest = card == _bInletCard ? _bInletRest : string.Empty;
        _bInletCard = card;
        _bInletRest = string.Empty;
        BInletField = BInletFieldBuild(rest);
    }

    internal void BInletClose()
    {
        if (BInletField is not BField field)
        {
            return;
        }

        _ = field.BFieldPendingApply(null);
        string text = (BInletField ?? field).BFieldText;
        (bool collocated, long card) = _bInletCard;
        BInletReset();
        _bInletRest = _bInletGate.Item3(collocated, card, text).Item1;
    }

    internal void BInletReset()
    {
        BInletField?.BFieldClear();
        BInletField = null;
        _bInletRest = string.Empty;
        BInletMenuClose();
    }

    internal void BInletRefine((bool, long)? lit)
    {
        if (lit != _bInletCard)
        {
            BInletReset();
        }
        else if (BInletField is BField field && _bInletSpot() is null)
        {
            string text = field.BFieldText;
            BInletReset();
            _bInletRest = text;
        }
    }

    internal static Func<bool, long, string, (string, IReadOnlyList<BInletRow>)> BInletOfferRead(
        Func<bool, long, string, CProffer> gate, Action<bool, long, long> insert)
    {
        ArgumentNullException.ThrowIfNull(gate);
        ArgumentNullException.ThrowIfNull(insert);

        return (collocated, card, text) =>
        {
            CProffer offer = gate(collocated, card, text);
            IReadOnlyList<BInletRow> rows = offer.CProfferShown
                ? [.. offer.CProfferRows.Select(row => new BInletRow(
                    row.CProfferRowLead, row.CProfferRowMark, row.CProfferRowTail, row.CProfferRowCount,
                    (owner, id) => insert(owner, id, row.CProfferRowId)))]
                : [];
            return (offer.CProfferText, rows);
        };
    }

    internal bool BInletBareRead(CCardDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        return _bInletChip(draft).Count == 0;
    }

    internal bool BInletObserve(ConsoleKeyInfo key)
    {
        if (!char.IsControl(key.KeyChar))
        {
            _bInletOpenRow.BKeymapRowAction(key);
        }

        return BInletField?.BSParleyBodyObserve(key) ?? false;
    }

    internal IReadOnlyList<(Func<int, IEnumerable<BCanvasText>>, int?)> BInletDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        BCanvasWidth measure,
        (int, int) place,
        (bool, CStep) card,
        (string?, bool) look)
    {
        ArgumentNullException.ThrowIfNull(glyphs);
        ArgumentNullException.ThrowIfNull(measure);
        ArgumentNullException.ThrowIfNull(card.Item2);

        (int column, int width) = place;
        (bool collocated, CStep step) = card;
        (string? tone, bool focused) = look;
        CCardDraft draft = step.CStepCard ?? throw new ArgumentException("The step carries no card.", nameof(card));
        BField? field = BInletField is not null && _bInletCard == (collocated, draft.CCardDraftId) ? BInletField : null;
        IReadOnlyList<(long, string)> chips = _bInletChip(draft);
        bool bare = BInletBareRead(draft);
        int room = Math.Max(width - BInletIndent, 1);
        Func<string, string> fit = text => measure.BCanvasWidthClamp(
            text, Math.Min(measure.BCanvasWidthResolve(text), room));
        string hint = fit(_bInletCatalog.BLocalizationTextRead(BInletHint));
        List<(string, string, bool)> tokens = [.. chips.Select(chip => (fit(chip.Item2), _bInletRole, false))];
        if (field is not null)
        {
            long? anchor = step.CStepCaret?.GetValueOrDefault(_bInletTrail);
            int found = chips.Select(static chip => (long?)chip.Item1).ToList().IndexOf(anchor);
            tokens.Insert(found < 0 ? chips.Count : found, (field.BFieldText, "field", true));
        }
        else if (_bInletRest.Length > 0 && _bInletCard == (collocated, draft.CCardDraftId))
        {
            tokens.Add((fit(_bInletRest), "text", false));
        }
        else if (bare)
        {
            tokens.Add((hint, "muted", false));
        }

        int gap = measure.BCanvasWidthResolve(BInletGap);
        List<List<(int, int, string, string, bool)>> lines = [[]];
        int edge = 0;
        foreach ((string text, string role, bool entry) in tokens)
        {
            int cells = !entry ? measure.BCanvasWidthResolve(text)
                : Math.Min(Math.Max(measure.BCanvasWidthResolve(text) + 1,
                    bare && text.Length == 0 ?measure.BCanvasWidthResolve(hint) : 1), room);
            if (edge > 0 && edge + cells > room)
            {
                lines.Add([]);
                edge = 0;
            }

            lines[^1].Add((edge, cells, text, role, entry));
            edge += cells + gap;
        }

        int start = column + BInletIndent;
        return [.. lines.Select(pieces => ((Func<int, IEnumerable<BCanvasText>>)(line =>
        {
            if (field is null && tone is not null)
            {
                string whole = string.Concat(pieces.Select((piece, at) => string.Concat(
                    new string(' ', piece.Item1 - (at == 0 ? 0 : pieces[at - 1].Item1 + pieces[at - 1].Item2)),
                    piece.Item3)));
                string lead = new(' ', BInletIndent);
                return [new BCanvasText(line, column, width, measure.BCanvasWidthClamp(lead + whole, width), tone)];
            }

            List<BCanvasText> texts = [];
            foreach ((int at, int cells, string text, string role, bool entry) in pieces)
            {
                if (!entry)
                {
                    texts.Add(new BCanvasText(line, start + at, cells, text, role));
                    continue;
                }

                texts.AddRange(field!.BSParleyBodyDraw(glyphs, line, start + at, cells, 1, focused, "field", measure));
                if (text.Length == 0 && bare)
                {
                    string faint = measure.BCanvasWidthClamp(hint, cells);
                    texts.Add(new BCanvasText(line, start + at, cells, faint, "muted"));
                }
            }

            return texts;
        }), pieces.FindIndex(static piece => piece.Item5) is int found and >= 0
            ? start + pieces[found].Item1
            : (int?)null))];
    }

    internal IEnumerable<BCanvasText> BInletDropDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        BCanvasWidth measure,
        BBlueprintPart bounds,
        ((int, int)?, bool) look)
    {
        ArgumentNullException.ThrowIfNull(glyphs);
        ArgumentNullException.ThrowIfNull(measure);
        ArgumentNullException.ThrowIfNull(bounds);

        ((int, int)? anchor, bool focused) = look;
        IReadOnlyList<BInletRow> rows = _bInletRow;
        if (BInletField is null || _bInletDrop is not BDrop drop || rows.Count == 0
            || anchor is not (int line, int start))
        {
            return [];
        }

        string[] counts = [.. rows.Select(static row => row.BInletRowCount)];
        int shown = _bInletCaret.CCaretLanternIndex;
        bool? lit = _bInletCaret.CCaretLanternLit ? focused : null;
        List<BCanvasText> texts = [.. drop.BDropDraw(glyphs, measure, bounds, (line, start), (counts, lit))];
        BCanvasText frame = texts[0];
        int count = counts.Select(text => measure.BCanvasWidthResolve(text)).DefaultIfEmpty(0).Max();
        foreach (int index in drop.BDropLabelShown)
        {
            BInletRow row = rows[index];
            int lead = measure.BCanvasWidthResolve(row.BInletRowLead);
            int room = frame.BCanvasTextSpan - 4 - (measure.BCanvasWidthResolve(counts[index]) > 0 ? count + 1 : 0);
            int cells = Math.Min(measure.BCanvasWidthResolve(row.BInletRowMark), room - lead);
            if (index == shown || cells <= 0)
            {
                continue;
            }

            texts.Add(new BCanvasText(
                frame.BCanvasTextRow + 1 + index - drop.BDropTop,
                frame.BCanvasTextColumn + 2 + lead,
                cells,
                measure.BCanvasWidthClamp(row.BInletRowMark, cells),
                "menu-hotkey"));
        }

        return texts;
    }

    private void BInletTrailDelete(int step)
    {
        (bool collocated, long card) = _bInletCard;
        _bInletTrail.CCaretTrailDelete(collocated, card, step);
    }

    private void BInletTrailMove(int step)
    {
        (bool collocated, long card) = _bInletCard;
        _ = _bInletTrail.CCaretTrailMove(collocated, card, step);
    }

    private BField BInletFieldBuild(string text)
    {
        return new BField(BInletTextSet, text, remove: BInletTrailDelete, step: BInletTrailMove);
    }

    private void BInletTextSet(string text)
    {
        (bool collocated, long card) = _bInletCard;
        BInletOfferShow(_bInletGate.Item1(collocated, card, text));
        _bInletChanged();
    }

    private void BInletTextSettle()
    {
        BField field = BInletField ?? throw new InvalidOperationException("The chip field is not open.");
        _ = field.BFieldPendingApply(null);
        string text = (BInletField ?? field).BFieldText;
        (bool collocated, long card) = _bInletCard;
        BInletOfferShow(_bInletGate.Item2(collocated, card, text));
    }

    private void BInletOfferShow((string, IReadOnlyList<BInletRow>) offer)
    {
        BInletField = BInletField is not BField field
            || string.Equals(field.BFieldText, offer.Item1, StringComparison.Ordinal)
                ? BInletField
                : BInletFieldBuild(offer.Item1);
        _bInletRow = offer.Item2;
        _bInletDrop = _bInletRow.Count > 0
            ? new BDrop(
                [.. _bInletRow.Select(static row => row.BInletRowLead + row.BInletRowMark + row.BInletRowTail)],
                _bInletCaret.CCaretLanternIndex,
                true,
                BInletPage)
            : null;
    }

    private void BInletMenuMove(int step)
    {
        BDrop drop = _bInletDrop ?? throw new InvalidOperationException("The stored list is shut.");
        _ = _bInletCaret.CCaretLanternMove(step);
        drop.BDropLitMove(_bInletCaret.CCaretLanternIndex - drop.BDropLit);
    }

    private void BInletMenuSelect()
    {
        BField field = BInletField ?? throw new InvalidOperationException("The chip field is not open.");
        (bool collocated, long card) = _bInletCard;
        BInletRow pick = _bInletRow[_bInletCaret.CCaretLanternIndex];
        field.BFieldClear();
        BInletMenuClose();
        pick.BInletRowChoice(collocated, card);
    }

    private void BInletMenuClose()
    {
        _bInletDrop = null;
    }
}

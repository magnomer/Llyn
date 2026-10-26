using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BBond
{
    private const int BBondPage = 6;

    private const int BBondIndent = 3;

    private const string BBondGap = "  ";

    private readonly BLocalizationCatalog _bBondCatalog;

    private readonly (Func<IReadOnlyList<CMeaning>>, Action<long, long>, Action<long, long>) _bBondGate;

    private readonly Func<CCardDraft?> _bBondSpot;

    private int? _bBondChip;

    private BDrop? _bBondDrop;

    private IReadOnlyList<CMeaning>? _bBondSense;

    internal BBond(
        BLocalizationCatalog catalog,
        (Func<IReadOnlyList<CMeaning>>, Action<long, long>, Action<long, long>) gates,
        Func<CCardDraft?> spot,
        Func<Func<bool>, Func<BKeymapLive>> when,
        Action changed)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(gates.Item1);
        ArgumentNullException.ThrowIfNull(gates.Item2);
        ArgumentNullException.ThrowIfNull(gates.Item3);
        ArgumentNullException.ThrowIfNull(spot);
        ArgumentNullException.ThrowIfNull(when);
        ArgumentNullException.ThrowIfNull(changed);

        _bBondCatalog = catalog;
        _bBondGate = gates;
        _bBondSpot = spot;

        const BKeymapFocus side = BKeymapFocus.BKeymapFocusEditor;
        Func<Action, Action<ConsoleKeyInfo>> pressed = action => _ =>
        {
            action();
            changed();
        };
        Func<BKeymapLive> menu = when(() => _bBondDrop is not null && _bBondSpot() is not null);
        Func<BKeymapLive> chosen = when(() => _bBondDrop is { BDropLabel.Count: > 0 } && _bBondSpot() is not null);
        Func<BKeymapLive> linking = when(() => _bBondDrop is null
            && _bBondSpot() is CCardDraft card && (_bBondChip ?? int.MaxValue) >= card.CCardDraftSense.Count);
        Func<BKeymapLive> back = when(() => _bBondDrop is null
            && _bBondSpot() is CCardDraft card && Math.Min(_bBondChip ?? int.MaxValue, card.CCardDraftSense.Count) > 0);
        Func<BKeymapLive> forth = when(() => _bBondDrop is null
            && _bBondSpot() is CCardDraft card && _bBondChip < card.CCardDraftSense.Count);
        Func<int, Action<ConsoleKeyInfo>> step = way => pressed(() => _bBondDrop?.BDropLitMove(way));
        Func<int, Action<ConsoleKeyInfo>> light = way => pressed(() =>
        {
            CCardDraft card = _bBondSpot() ?? throw new InvalidOperationException("No linked-meanings row is lit.");
            int chip = Math.Min(_bBondChip ?? int.MaxValue, card.CCardDraftSense.Count) + way;
            _bBondChip = chip < card.CCardDraftSense.Count ? chip : null;
        });
        BBondKeymap =
        [
            new(ConsoleKey.UpArrow, 0, side, "Window.Help.Item", step(-1), menu),
            new(ConsoleKey.DownArrow, 0, side, "Window.Help.Item", step(1), menu),
            new(ConsoleKey.Enter, 0, side, "Editor.Pick", pressed(BBondSelect), chosen),
            new(ConsoleKey.Escape, 0, side, "Editor.Shut", pressed(BBondClose), menu),
            new(ConsoleKey.Enter, 0, side, "Card.SenseLink", pressed(BBondOpen), linking),
            new(ConsoleKey.LeftArrow, 0, side, "Card.SenseLight", light(-1), back),
            new(ConsoleKey.RightArrow, 0, side, "Card.SenseLight", light(1), forth),
            new(ConsoleKey.Delete, 0, side, "Card.SenseDetach", pressed(BBondDetach), forth),
        ];
    }

    internal IReadOnlyList<BKeymapRow> BBondKeymap { get; }

    internal bool BBondObserve(ConsoleKeyInfo key)
    {
        if (_bBondDrop is not BDrop drop || _bBondSpot() is null || char.IsControl(key.KeyChar))
        {
            return false;
        }

        _ = drop.BDropSeek(key.KeyChar);
        return true;
    }

    internal void BBondClose()
    {
        _bBondDrop = null;
        _bBondSense = null;
    }

    internal void BBondReset()
    {
        BBondClose();
        _bBondChip = null;
    }

    internal IReadOnlyList<(Func<int, IEnumerable<BCanvasText>>, int?)> BBondDraw(
        BCanvasWidth measure,
        (int, int) place,
        CCardDraft card,
        string? tone)
    {
        ArgumentNullException.ThrowIfNull(measure);
        ArgumentNullException.ThrowIfNull(card);

        (int column, int width) = place;
        IReadOnlyList<CMeaning> chips = card.CCardDraftSense;
        string? shine = tone is "lit" or "lit-other" ? tone : null;
        int shone = Math.Min(_bBondChip ?? chips.Count, chips.Count);
        string? calm = tone == "muted" ? tone : null;
        string lead = new(' ', BBondIndent);
        string caption = _bBondCatalog.BLocalizationTextRead("Card.SenseCaption");
        int offset = measure.BCanvasWidthResolve(lead + caption + BBondGap);
        int room = Math.Max(width - offset, 1);
        Func<string, string> fit = text =>
            measure.BCanvasWidthClamp(text, Math.Min(measure.BCanvasWidthResolve(text), room));
        List<(string, string, bool)> tokens = [.. chips.Select((chip, at) => (
            fit($"[{$"{chip.CMeaningPath} {chip.CMeaningName}".Trim()}]"),
            at == shone ? shine ?? "link" : "link",
            false))];
        tokens.Add((fit(_bBondCatalog.BLocalizationTextRead("Card.SenseAdd")),
            shone == chips.Count ? shine ?? "text" : "text", true));
        List<List<(int, int, string, string, bool)>> lines = BEditorSight.BEditorSightDivide(
            [.. tokens.Select(token => (measure.BCanvasWidthResolve(token.Item1), token))],
            room,
            measure.BCanvasWidthResolve(BBondGap));
        return [.. lines.Select((pieces, row) =>
        {
            List<(string, string)> parts = row == 0
                ? [(lead, "text"), (caption, "muted"), (BBondGap, "text")]
                : [(new string(' ', offset), "text")];
            int edge = 0;
            foreach ((int at, int cells, string text, string role, _) in pieces)
            {
                if (at > edge)
                {
                    parts.Add((new string(' ', at - edge), "text"));
                }

                parts.Add((text, role));
                edge = at + cells;
            }

            int found = pieces.FindIndex(static piece => piece.Item5);
            int? entry = _bBondDrop is not null && shine is not null && found >= 0
                ? Math.Min(column + offset + pieces[found].Item1, column + Math.Max(width - 1, 0))
                : null;
            return ((Func<int, IEnumerable<BCanvasText>>)(line =>
                BEditorSight.BEditorSightDraw(measure, (line, column, width), calm, parts)), entry);
        })];
    }

    internal IEnumerable<BCanvasText> BBondDropDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        BCanvasWidth measure,
        BBlueprintPart bounds,
        ((int, int)?, bool) look)
    {
        ArgumentNullException.ThrowIfNull(glyphs);
        ArgumentNullException.ThrowIfNull(measure);
        ArgumentNullException.ThrowIfNull(bounds);

        ((int, int)? anchor, bool focused) = look;
        return _bBondDrop is BDrop drop && _bBondSpot() is not null && anchor is (int line, int start)
            ? drop.BDropDraw(glyphs, measure, bounds, (line, start), ([], focused))
            : [];
    }

    private void BBondOpen()
    {
        _bBondChip = null;
        _bBondSense = _bBondGate.Item1();
        _bBondDrop = new BDrop(
            [.. _bBondSense.Select(static sense =>
                $"{new string(' ', 2 * Math.Max(sense.CMeaningDepth, 0))}{sense.CMeaningPath} {sense.CMeaningName}")],
            0,
            true,
            BBondPage);
    }

    private void BBondSelect()
    {
        CCardDraft card = _bBondSpot() ?? throw new InvalidOperationException("No linked-meanings row is lit.");
        BDrop drop = _bBondDrop ?? throw new InvalidOperationException("The meaning list is shut.");
        CMeaning pick = (_bBondSense ?? [])[drop.BDropLit];
        BBondClose();
        _bBondGate.Item2(card.CCardDraftId, pick.CMeaningId);
    }

    private void BBondDetach()
    {
        CCardDraft card = _bBondSpot() ?? throw new InvalidOperationException("No linked-meanings row is lit.");
        CMeaning chip = card.CCardDraftSense[_bBondChip.GetValueOrDefault()];
        _bBondGate.Item3(card.CCardDraftId, chip.CMeaningId);
    }
}

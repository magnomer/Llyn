using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BMarker
{
    private const int BMarkerPage = 6;

    private const int BMarkerSpeech = 2;

    private const string BMarkerGap = "   ";

    private readonly BLocalizationCatalog _bMarkerCatalog;

    private readonly CCardSpeech _bMarkerGate;

    private readonly CDesk _bMarkerDesk;

    private CMarker? _bMarkerHeld;

    private CCategory _bMarkerCategory = new([], false, false, false);

    private bool _bMarkerChosen;

    private readonly Func<BKeymapLive> _bMarkerTyping;

    internal BMarker(
        BLocalizationCatalog catalog,
        (CCardSpeech, CEntry, CDesk) gates,
        Func<Func<bool>, Func<BKeymapLive>> when,
        Action changed)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(gates.Item1);
        ArgumentNullException.ThrowIfNull(gates.Item2);
        ArgumentNullException.ThrowIfNull(gates.Item3);
        ArgumentNullException.ThrowIfNull(when);
        ArgumentNullException.ThrowIfNull(changed);

        CEntry entry;
        (_bMarkerGate, entry, _bMarkerDesk) = gates;
        _bMarkerCatalog = catalog;
        BMarkerUnit = new BUnit(catalog);
        BMarkerFreedom = new BFreedom(catalog);
        entry.CEntryDraftChanged += _ => BMarkerRefine();

        const BKeymapFocus side = BKeymapFocus.BKeymapFocusEditor;
        Func<Action, Action<ConsoleKeyInfo>> pressed = action => _ =>
        {
            action();
            changed();
        };
        Func<BKeymapLive> unit = when(() => BMarkerUnit.BUnitDrop is not null);
        Func<BKeymapLive> freedom = when(() => BMarkerFreedom.BFreedomDrop is not null);
        Func<BKeymapLive> menu = when(() => BMarkerField is not null && BMarkerMenu is not null);
        Func<BKeymapLive> chosen = when(() => BMarkerField is not null && _bMarkerChosen
            && BMarkerMenu is { BDropLabel.Count: > 0 });
        Func<BKeymapLive> typing = when(() => BMarkerField is not null);
        Func<BKeymapLive> shut = when(() => BMarkerField is not null && BMarkerMenu is null);
        Func<BKeymapLive> erase = when(() => BMarkerField is { BFieldText.Length: 0 }
            && _bMarkerHeld is { CMarkerSpeeches.Count: > 0 });
        Func<BKeymapLive> idle = when(() => !BMarkerOpened);
        Func<int, Func<BKeymapLive>> spot = at => when(() => !BMarkerOpened && BMarkerSpot == at);
        _bMarkerTyping = spot(BMarkerSpeech);
        BMarkerKeymap =
        [
            new(ConsoleKey.UpArrow, 0, side, "Window.Help.Item",
                pressed(() => BMarkerUnit.BUnitDrop?.BDropLitMove(-1)), unit),
            new(ConsoleKey.DownArrow, 0, side, "Window.Help.Item",
                pressed(() => BMarkerUnit.BUnitDrop?.BDropLitMove(1)), unit),
            new(ConsoleKey.Enter, 0, side, "Editor.Pick",
                pressed(() => BMarkerUnit.BUnitSelect(entry.CEntryUnitSet)), unit),
            new(ConsoleKey.Escape, 0, side, "Editor.Shut", pressed(BMarkerUnit.BUnitClose), unit),
            new(ConsoleKey.UpArrow, 0, side, "Window.Help.Item",
                pressed(() => BMarkerFreedom.BFreedomDrop?.BDropLitMove(-1)), freedom),
            new(ConsoleKey.DownArrow, 0, side, "Window.Help.Item",
                pressed(() => BMarkerFreedom.BFreedomDrop?.BDropLitMove(1)), freedom),
            new(ConsoleKey.Enter, 0, side, "Editor.Pick",
                pressed(() => BMarkerFreedom.BFreedomSelect(entry.CEntryFreedomSet)), freedom),
            new(ConsoleKey.Escape, 0, side, "Editor.Shut", pressed(BMarkerFreedom.BFreedomClose), freedom),
            new(ConsoleKey.UpArrow, 0, side, "Window.Help.Item", pressed(() => BMarkerMenuMove(-1)), menu),
            new(ConsoleKey.DownArrow, 0, side, "Window.Help.Item", pressed(() => BMarkerMenuMove(1)), menu),
            new(ConsoleKey.UpArrow, 0, side, "Speech.Offer", pressed(BMarkerMenuOpen), shut),
            new(ConsoleKey.DownArrow, 0, side, "Speech.Offer", pressed(BMarkerMenuOpen), shut),
            new(ConsoleKey.Enter, 0, side, "Editor.Pick", pressed(() => BMarkerMenuSelect(BMarkerSpeechAdd)), chosen),
            new(ConsoleKey.Enter, 0, side, "Speech.Add", pressed(() => BMarkerFieldSelect(BMarkerSpeechAdd)), typing),
            new(ConsoleKey.Escape, 0, side, "Editor.Shut", pressed(BMarkerMenuClose), menu),
            new(ConsoleKey.Escape, 0, side, "Editor.Close", pressed(BMarkerFieldClose), shut),
            new(ConsoleKey.Backspace, 0, side, "Speech.Remove", pressed(BMarkerSpeechRemove), erase),
            new(ConsoleKey.Enter, 0, side, "Editor.Unit", pressed(BMarkerUnit.BUnitOpen), spot(0)),
            new(ConsoleKey.Enter, 0, side, "Editor.Freedom", pressed(BMarkerFreedom.BFreedomOpen), spot(1)),
            new(ConsoleKey.Enter, 0, side, "Editor.Open", pressed(BMarkerFieldOpen), spot(BMarkerSpeech)),
            new(ConsoleKey.LeftArrow, 0, side, "Editor.Part", pressed(() => BMarkerSpotMove(-1)), idle),
            new(ConsoleKey.RightArrow, 0, side, "Editor.Part", pressed(() => BMarkerSpotMove(1)), idle),
        ];
    }

    internal IReadOnlyList<BKeymapRow> BMarkerKeymap { get; }

    internal BUnit BMarkerUnit { get; }

    internal BFreedom BMarkerFreedom { get; }

    internal BField? BMarkerField { get; private set; }

    internal BDrop? BMarkerMenu { get; private set; }

    internal int BMarkerSpot { get; private set; }

    internal bool BMarkerOpened =>
        BMarkerField is not null || BMarkerUnit.BUnitDrop is not null || BMarkerFreedom.BFreedomDrop is not null;

    internal void BMarkerClose()
    {
        BMarkerUnit.BUnitClose();
        BMarkerFreedom.BFreedomClose();
        if (BMarkerField is not null)
        {
            BMarkerFieldClose();
        }
    }

    internal bool BMarkerObserve(ConsoleKeyInfo key)
    {
        if (!char.IsControl(key.KeyChar) && _bMarkerTyping() is BKeymapLive.BKeymapLiveShown)
        {
            BMarkerFieldOpen();
        }

        return BMarkerField?.BSParleyBodyObserve(key)
            ?? (BMarkerUnit.BUnitObserve(key) || BMarkerFreedom.BFreedomObserve(key));
    }

    internal IEnumerable<BCanvasText> BMarkerDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        BCanvasWidth measure,
        (int, int, int) place,
        (string?, bool) look)
    {
        ArgumentNullException.ThrowIfNull(glyphs);
        ArgumentNullException.ThrowIfNull(measure);

        (int line, int column, int width) = place;
        (string? tone, bool focused) = look;
        string arrow = BLecternHeader.BLecternGlyphRead(glyphs, "Order", 1, "v");
        Func<int, string> role = at => tone is "lit" or "lit-other" && BMarkerSpot == at ? tone : "text";
        IReadOnlyList<string> chips = [.. (_bMarkerHeld?.CMarkerSpeeches ?? []).Select(static name => $"({name}) ")];
        List<(string, string)> pieces =
        [
            ($"{BMarkerUnit.BUnitName} {arrow}", role(0)),
            (BMarkerGap, "text"),
            ($"{BMarkerFreedom.BFreedomName} {arrow}", role(1)),
            (BMarkerGap, "text"),
            .. chips.Select(chip => (chip, role(BMarkerSpeech))),
        ];
        string? calm = tone == "muted" ? tone : null;
        if (BMarkerField is not BField field)
        {
            string typed = _bMarkerHeld?.CMarkerTyped ?? string.Empty;
            pieces.Add(chips.Count + typed.Length > 0
                ? (typed, role(BMarkerSpeech))
                : (_bMarkerCatalog.BLocalizationTextRead("Speech.Title"),
                    role(BMarkerSpeech) == "text" ? "muted" : role(BMarkerSpeech)));
            return BEditorSight.BEditorSightDraw(measure, place, calm, pieces);
        }

        int edge = Math.Min(column + pieces.Sum(piece => measure.BCanvasWidthResolve(piece.Item1)), column + width);
        return
        [
            .. BEditorSight.BEditorSightDraw(measure, place, calm, pieces),
            .. field.BSParleyBodyDraw(glyphs, line, edge, column + width - edge, 1, focused, "field", measure),
        ];
    }

    internal IEnumerable<BCanvasText> BMarkerDropDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        BCanvasWidth measure,
        BBlueprintPart bounds,
        (int, int) field,
        bool focused)
    {
        ArgumentNullException.ThrowIfNull(glyphs);
        ArgumentNullException.ThrowIfNull(measure);

        (int anchor, int column) = field;
        string arrow = BLecternHeader.BLecternGlyphRead(glyphs, "Order", 1, "v");
        int gap = measure.BCanvasWidthResolve(BMarkerGap);
        int freedom = column + measure.BCanvasWidthResolve($"{BMarkerUnit.BUnitName} {arrow}") + gap;
        int speech = freedom + measure.BCanvasWidthResolve($"{BMarkerFreedom.BFreedomName} {arrow}") + gap
            + (_bMarkerHeld?.CMarkerSpeeches ?? []).Sum(name => measure.BCanvasWidthResolve($"({name}) "));
        IReadOnlyList<CCategoryRow> rows = _bMarkerCategory.CCategoryRows;
        string check = BLecternHeader.BLecternGlyphRead(glyphs, "Check", 0, "[x]");
        BDrop? menu = BMarkerMenu is null ? null
            : rows.Count > 0 ? BMarkerMenu
            : _bMarkerCategory.CCategoryHint is string hint
                ? new BDrop([_bMarkerCatalog.BLocalizationTextRead(hint)], -1, false, 1)
            : null;
        return
        [
            .. BMarkerUnit.BUnitDraw(glyphs, measure, bounds, (anchor, column), focused),
            .. BMarkerFreedom.BFreedomDraw(glyphs, measure, bounds, (anchor, freedom), focused),
            .. menu?.BDropDraw(
                glyphs,
                measure,
                bounds,
                (anchor, speech),
                ([.. rows.Select(row => row.CCategoryRowTaken ? check : string.Empty)],
                    _bMarkerChosen ? focused : null))
                ?? [],
        ];
    }

    private void BMarkerRefine()
    {
        CMarker held = _bMarkerGate.CCardSpeechRead();
        _bMarkerHeld = held;
        BMarkerUnit.BUnitRefine(held.CMarkerUnits);
        BMarkerFreedom.BFreedomRefine(held.CMarkerFreedoms);
        _bMarkerCategory = BMarkerMenu is null ? held.CMarkerCategory : _bMarkerCategory;
    }

    private void BMarkerSpotMove(int step)
    {
        BMarkerSpot = Math.Clamp(BMarkerSpot + step, 0, BMarkerSpeech);
    }

    private void BMarkerFieldOpen()
    {
        BMarkerField = new BField(
            BMarkerTextSet, _bMarkerHeld?.CMarkerTyped ?? string.Empty, _bMarkerCatalog, "Speech.Title");
    }

    private void BMarkerTextSet(string text)
    {
        _bMarkerCategory = _bMarkerGate.CCardSpeechSet(text);
        BMarkerMenuOpen();
        BMarkerMenu = _bMarkerCategory.CCategoryShown ? BMarkerMenu : null;
    }

    private void BMarkerMenuOpen()
    {
        BMarkerMenu = new BDrop(
            [.. _bMarkerCategory.CCategoryRows.Select(static row => row.CCategoryRowName)], -1, true, BMarkerPage);
        _bMarkerChosen = false;
    }

    private void BMarkerMenuMove(int step)
    {
        BMarkerMenu?.BDropLitMove(_bMarkerChosen ? step : -1);
        _bMarkerChosen = true;
    }

    private void BMarkerMenuSelect(Action<string> take)
    {
        BDrop menu = BMarkerMenu ?? throw new InvalidOperationException("The part-of-speech menu is not open.");
        take(menu.BDropLabel[menu.BDropLit]);
    }

    private void BMarkerMenuClose()
    {
        BMarkerMenu = null;
        _bMarkerChosen = false;
    }

    private void BMarkerFieldSelect(Action<string> take)
    {
        BField field = BMarkerField ?? throw new InvalidOperationException("The part-of-speech field is not open.");
        take(field.BFieldText);
    }

    private void BMarkerFieldClose()
    {
        BMarkerField = null;
        BMarkerMenuClose();
        _bMarkerDesk.CDeskDraft.CDeskDraftPersist();
    }

    private void BMarkerSpeechAdd(string name)
    {
        _bMarkerGate.CCardSpeechAdd(name);
        BMarkerField?.BFieldClear();
        BMarkerMenuClose();
    }

    private void BMarkerSpeechRemove()
    {
        _bMarkerGate.CCardSpeechRemove(_bMarkerHeld!.CMarkerSpeeches[^1]);
    }
}

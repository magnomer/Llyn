using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BEditor
{
    internal const int BEditorHeadwordRow = 0;

    private const int BEditorStackRow = 1;

    private readonly BLocalizationCatalog _bEditorCatalog;

    private readonly CEditor _bEditorGate;

    private readonly BEditorSight _bEditorSight;

    private readonly Action _bEditorChanged;

    private CEntryDraft? _bEditorDraft;

    private BField? _bEditorField;

    private int _bEditorLit;

    private int _bEditorStack;

    internal BEditor(
        BLocalizationCatalog catalog,
        CEditor editor,
        string title,
        Func<bool> shown,
        Func<bool> focused,
        Action changed,
        Func<CDisplay, BLectern>? lectern = null,
        Func<bool>? reading = null,
        Action<Action>? settle = null)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(editor);
        ArgumentNullException.ThrowIfNull(title);
        ArgumentNullException.ThrowIfNull(shown);
        ArgumentNullException.ThrowIfNull(focused);
        ArgumentNullException.ThrowIfNull(changed);

        _bEditorCatalog = catalog;
        _bEditorGate = editor;
        _bEditorSight = new BEditorSight(catalog, editor.CEditorDesk, title);
        _bEditorChanged = changed;
        CDisplay display = editor.CEditorDisplay;
        BEditorLectern = lectern?.Invoke(display);
        editor.CEditorEntry.CEntryDraftChanged += draft =>
        {
            _bEditorDraft = draft;
            BEditorLitMove(0);
        };
        BEditorChronicle = new BEditorChronicle(
            editor.CEditorDesk, editor.CEditorEntry, () => _bEditorField, BEditorFieldOpen);
        CDeskChronicle steps = editor.CEditorDesk.CDeskChronicle;
        editor.CEditorWalkChanged += changed;
        editor.CEditorDesk.CDeskStarted += BEditorReset;
        editor.CEditorDesk.CDeskStateChanged += () =>
        {
            _bEditorField = steps.CDeskChronicleRunning ? _bEditorField : null;
            changed();
        };

        const BKeymapFocus side = BKeymapFocus.BKeymapFocusEditor;
        Func<Func<bool>, Func<BKeymapLive>> when = check => () =>
            !focused() || _bEditorDraft is null || !check() ? BKeymapLive.BKeymapLiveHidden
            : !steps.CDeskChronicleRunning ? BKeymapLive.BKeymapLiveDim
            : BKeymapLive.BKeymapLiveShown;
        Func<BKeymapLive> always = when(static () => true);
        Func<bool, Func<BKeymapLive>> chronicle = forward => () =>
            !focused() || _bEditorDraft is null ? BKeymapLive.BKeymapLiveHidden
            : !steps.CDeskChronicleRunning
                || !(forward ? steps.CDeskChronicleState.CDeskForward : steps.CDeskChronicleState.CDeskBackward)
                ? BKeymapLive.BKeymapLiveDim
            : BKeymapLive.BKeymapLiveShown;
        Func<BKeymapLive> lit = when(() => _bEditorField is null);
        Func<BKeymapLive> opened = when(() => _bEditorField is not null);
        Func<CStep?> here = () => editor.CEditorWalk[_bEditorStack].ElementAtOrDefault(_bEditorLit);
        Func<BKeymapLive> head = when(() => _bEditorField is null && _bEditorLit == BEditorHeadwordRow);
        Func<BKeymapLive> tabs = when(() => _bEditorField is null && _bEditorLit == BEditorStackRow);
        Func<BKeymapLive> fold = when(() => _bEditorField is null
            && here() is { CStepFold: not null });
        Func<int, int?> find = way =>
        {
            IEnumerable<int?> cards = editor.CEditorWalkCard[_bEditorStack].Select(static at => (int?)at);
            return way > 0
                ? cards.FirstOrDefault(at => at > _bEditorLit)
                : cards.LastOrDefault(at => at < _bEditorLit);
        };
        Func<Action, Action<ConsoleKeyInfo>> pressed = action => _ =>
        {
            action();
            changed();
        };
        Func<Func<int>, Action<ConsoleKeyInfo>> move = step => pressed(() => BEditorLitMove(step()));
        Func<int, Action<ConsoleKeyInfo>> seek = way => pressed(() =>
            BEditorLitMove((find(way) ?? _bEditorLit) - _bEditorLit));
        Func<int, Action<ConsoleKeyInfo>> choose = way => pressed(() => BEditorStackSelect(_bEditorStack + way));
        Action<ConsoleKeyInfo> pick = key =>
        {
            BEditorStackSelect(key.Key - ConsoleKey.D1);
            changed();
        };
        Func<int, Func<BKeymapLive>> card =way => when(() => _bEditorField is null && find(way) is not null);
        Func<bool, Func<Func<bool>, Func<BKeymapLive>>> gated = visible => enabled => () =>
            !shown() || !visible ? BKeymapLive.BKeymapLiveHidden
            : enabled() ? BKeymapLive.BKeymapLiveShown
            : BKeymapLive.BKeymapLiveDim;
        CDeskDraft draft = editor.CEditorDesk.CDeskDraft;
        const BKeymapFocus window = BKeymapFocus.BKeymapFocusWindow;
        BEditorKeymap = new BKeymap(
        [
            new(ConsoleKey.F2, 0, window, "Scribe.Save", pressed(BEditorEntrySave),
                gated(true)(() => draft.CDeskDraftStorable)),
            new(ConsoleKey.F7, 0, window, "Scribe.New", pressed(editor.CEditorEntryUndo),
                () => gated(editor.CEditorOwned)(() => draft.CDeskDraftAltered)()),
            .. (BKeymapRow[])(BEditorLectern is BLectern face && reading is not null && settle is not null
                ? [new(ConsoleKey.F6, 0, window, "Favorite.Mark", key => settle(() => face.BLecternFavoriteToggle(key)),
                    gated(true)(() => reading() && !display.CDisplayBlank))]
                : []),
            new(ConsoleKey.Z, ConsoleModifiers.Control, side, "Chronicle.Undo",
                pressed(BEditorChronicle.BEditorChronicleUndo), chronicle(false)),
            new(ConsoleKey.Y, ConsoleModifiers.Control, side, "Chronicle.Redo",
                pressed(BEditorChronicle.BEditorChronicleRedo), chronicle(true)),
            new(ConsoleKey.Z, ConsoleModifiers.Control | ConsoleModifiers.Shift, side, "Chronicle.Redo",
                pressed(BEditorChronicle.BEditorChronicleRedo), chronicle(true)),
            new(ConsoleKey.UpArrow, 0, side, "Window.Help.Item", move(static () => -1), lit),
            new(ConsoleKey.DownArrow, 0, side, "Window.Help.Item", move(static () => 1), lit),
            new(ConsoleKey.UpArrow, 0, side, "Editor.Close", BEditorFieldClose, opened),
            new(ConsoleKey.DownArrow, 0, side, "Editor.Close", BEditorFieldClose, opened),
            new(ConsoleKey.PageUp, 0, side, "Window.Help.Scroll", move(() => -_bEditorSight.BEditorSightPage), lit),
            new(ConsoleKey.PageDown, 0, side, "Window.Help.Scroll", move(() => _bEditorSight.BEditorSightPage), lit),
            new(ConsoleKey.Home, ConsoleModifiers.Control, side, "Window.Help.Scroll", move(() => -_bEditorLit), lit),
            new(ConsoleKey.End, ConsoleModifiers.Control, side, "Window.Help.Scroll",
                move(() => editor.CEditorWalk[_bEditorStack].Count), lit),
            new(ConsoleKey.PageUp, ConsoleModifiers.Control, side, "Editor.Card", seek(-1), card(-1)),
            new(ConsoleKey.PageDown, ConsoleModifiers.Control, side, "Editor.Card", seek(1), card(1)),
            new(ConsoleKey.Enter, 0, side, "Editor.Open", pressed(BEditorFieldOpen), head),
            new(ConsoleKey.Enter, 0, side, "Card.Fold", pressed(BEditorFoldToggle), fold),
            new(ConsoleKey.Enter, 0, side, "Editor.Close", BEditorFieldClose, opened),
            new(ConsoleKey.Escape, 0, side, "Editor.Close", BEditorFieldClose, opened),
            new(ConsoleKey.LeftArrow, 0, side, "Editor.Stack", choose(-1), tabs),
            new(ConsoleKey.RightArrow, 0, side, "Editor.Stack", choose(1), tabs),
            new(ConsoleKey.D1, ConsoleModifiers.Alt, side, "Editor.Stack", pick, always),
            new(ConsoleKey.D2, ConsoleModifiers.Alt, side, "Editor.Stack", pick, always),
            new(ConsoleKey.D3, ConsoleModifiers.Alt, side, "Editor.Stack", pick, always),
            new(ConsoleKey.D4, ConsoleModifiers.Alt, side, "Editor.Stack", pick, always),
        ]);
    }

    internal BKeymap BEditorKeymap { get; }

    internal BLectern? BEditorLectern { get; }

    internal BEditorChronicle BEditorChronicle { get; }

    internal bool BEditorObserve(ConsoleKeyInfo key)
    {
        bool typed = !char.IsControl(key.KeyChar);
        if (_bEditorField is null && _bEditorLit == BEditorHeadwordRow && _bEditorDraft is not null && typed
            && _bEditorGate.CEditorDesk.CDeskChronicle.CDeskChronicleRunning)
        {
            BEditorFieldOpen();
        }

        bool taken = _bEditorField?.BSParleyBodyObserve(key) ?? false;
        _bEditorChanged();
        return taken;
    }

    internal IReadOnlyList<BCanvasText> BEditorDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        BBlueprintPart display,
        bool focused,
        BCanvasWidth measure)
    {
        return _bEditorSight.BEditorSightDraw(
            glyphs,
            display,
            focused,
            measure,
            (_bEditorDraft, _bEditorField, _bEditorLit, _bEditorStack),
            (_bEditorGate.CEditorWalk, _bEditorGate.CEditorWalkCard, _bEditorGate.CEditorOwned));
    }

    private void BEditorLitMove(int step)
    {
        int count = _bEditorGate.CEditorWalk.ElementAtOrDefault(_bEditorStack)?.Count ?? 0;
        _bEditorLit = Math.Clamp(_bEditorLit + step, 0, Math.Max(count - 1, 0));
    }

    private void BEditorStackSelect(int stack)
    {
        _bEditorStack = Math.Clamp(stack, 0, 3);
        BEditorLitMove(0);
    }

    private void BEditorFieldOpen()
    {
        _bEditorField = new BField(
            _bEditorGate.CEditorEntry.CEntryHeadwordSet,
            _bEditorDraft?.CEntryDraftHeadword ?? string.Empty,
            _bEditorCatalog,
            "Input.Headword");
    }

    private void BEditorFieldClose(ConsoleKeyInfo key)
    {
        _ = _bEditorField?.BFieldPendingApply(null);
        _bEditorField = null;
        _bEditorGate.CEditorDesk.CDeskDraft.CDeskDraftPersist();
        BEditorLitMove(key.Key switch
        {
            ConsoleKey.UpArrow => -1,
            ConsoleKey.DownArrow => 1,
            _ => 0,
        });
        _bEditorChanged();
    }

    private void BEditorEntrySave()
    {
        _ = _bEditorField?.BFieldPendingApply(null);
        _bEditorField = null;
        _ = _bEditorGate.CEditorEntrySave();
    }

    private void BEditorFoldToggle()
    {
        CStep step = _bEditorGate.CEditorWalk[_bEditorStack][_bEditorLit];
        _ = step.CStepFold!(!step.CStepCard!.CCardDraftFolded);
    }

    private void BEditorReset()
    {
        _bEditorField = null;
        BEditorLitMove(-_bEditorLit);
    }
}

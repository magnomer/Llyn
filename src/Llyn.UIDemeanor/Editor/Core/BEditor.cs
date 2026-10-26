using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BEditor
{
    internal const int BEditorHeadwordRow = 0;

    internal const int BEditorLanguageRow = 1;

    internal const int BEditorEsteemRow = 2;

    internal const int BEditorMarkerRow = 3;

    internal const int BEditorHeadSpan = 3;

    private const int BEditorStackRow = 4;

    private readonly BLocalizationCatalog _bEditorCatalog;

    private readonly BSpeaker _bEditorSpeaker;

    private readonly BRegard _bEditorRegard;

    private readonly BMarker _bEditorMarker;

    private readonly BPlacard _bEditorPlacard;

    private readonly BCard _bEditorCard;

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
        Action<Action>? settle = null,
        Func<Task<IReadOnlyList<string>>>? languages = null)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(editor);
        ArgumentNullException.ThrowIfNull(title);
        ArgumentNullException.ThrowIfNull(shown);
        ArgumentNullException.ThrowIfNull(focused);
        ArgumentNullException.ThrowIfNull(changed);

        _bEditorCatalog = catalog;
        _bEditorGate = editor;
        _bEditorSpeaker = new BSpeaker(
            editor.CEditorEntry, languages ?? (static () => Task.FromResult<IReadOnlyList<string>>([])), changed);
        _bEditorRegard = new BRegard(catalog, editor.CEditorDesk, editor.CEditorEsteem, changed);
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
        Func<CStep?> here = () =>
            editor.CEditorWalk[_bEditorStack].ElementAtOrDefault(Math.Max(_bEditorLit - BEditorHeadSpan, 0));
        Func<BKeymapLive> head = when(() => _bEditorField is null && _bEditorLit == BEditorHeadwordRow);
        Func<BKeymapLive> tabs = when(() => _bEditorField is null && _bEditorLit == BEditorStackRow);
        Func<BKeymapLive> dropped = when(() => _bEditorSpeaker.BSpeakerDrop is not null);
        Func<BKeymapLive> chosen = when(() => _bEditorSpeaker.BSpeakerDrop is { BDropLabel.Count: > 0 });
        Func<BKeymapLive> speaker = when(() => _bEditorField is null && _bEditorSpeaker.BSpeakerDrop is null
            && _bEditorLit == BEditorLanguageRow);
        Func<BKeymapLive> rated = when(() => _bEditorField is null && _bEditorLit == BEditorEsteemRow);
        Func<BKeymapLive> esteem = () =>
        {
            BKeymapLive live = rated();
            return live is BKeymapLive.BKeymapLiveShown && !editor.CEditorDesk.CDeskStored
                ? BKeymapLive.BKeymapLiveDim
                : live;
        };
        _bEditorMarker = new BMarker(
            catalog,
            (editor.CEditorSpeech, editor.CEditorEntry, editor.CEditorDesk),
            check => when(() => _bEditorField is null && _bEditorLit == BEditorMarkerRow && check()),
            changed);
        _bEditorPlacard = new BPlacard(
            catalog,
            (editor.CEditorCaret, editor.CEditorDesk.CDeskDraft.CDeskDraftPersist),
            () => _bEditorField is null && _bEditorLit > BEditorHeadSpan && here() is CStep step
                ? (_bEditorStack == 1, step)
                : null,
            when,
            changed);
        Action<int> light = step =>
        {
            BEditorStackSelect(_bEditorStack);
            BEditorLitMove(step);
        };
        CCardList list = editor.CEditorList;
        _bEditorCard = new BCard(
            catalog,
            (list.CCardMeaningAdd, list.CCardCollocationAdd, list.CCardRemove, list.CCardMove,
                editor.CEditorField),
            (list.CCardPositionOpen, list.CCardPositionSettle, list.CCardPositionClose),
            (stack => (editor.CEditorWalk.ElementAtOrDefault(stack) ?? [],
                    editor.CEditorWalkCard.ElementAtOrDefault(stack) ?? []),
                () => _bEditorField is null ? (_bEditorLit, _bEditorStack, _bEditorPlacard.BPlacardPart) : null),
            when,
            (light, changed, editor.CEditorDesk.CDeskDraft.CDeskDraftPersist));
        editor.CEditorDesk.CDeskStateChanged += () =>
        {
            if (!steps.CDeskChronicleRunning)
            {
                _bEditorField = null;
                _bEditorCard.BCardReset();
                foreach (BInlet inlet in _bEditorPlacard.BPlacardInlet)
                {
                    inlet.BInletReset();
                }
            }

            changed();
        };
        _bEditorSight = new BEditorSight(
            catalog, editor.CEditorDesk, title, (_bEditorSpeaker, _bEditorRegard, _bEditorMarker),
            (_bEditorPlacard.BPlacardInlet, _bEditorCard));
        Func<ConsoleKey, BKeymapRow> digit = key => new(key, 0, side, "Grasp.Mark", BEditorEsteemObserve, () =>
            key - ConsoleKey.D0 <= editor.CEditorEsteem.CEsteemGraspHit.Count
                ? esteem()
                : BKeymapLive.BKeymapLiveHidden);
        Func<Action, Action<ConsoleKeyInfo>> pressed = action => _ =>
        {
            action();
            changed();
        };
        Func<Func<int>, Action<ConsoleKeyInfo>> move = step => pressed(() => light(step()));
        Func<int, Action<ConsoleKeyInfo>> row = way => pressed(() =>
        {
            _bEditorMarker.BMarkerClose();
            _bEditorPlacard.BPlacardClose();
            BEditorRowMove(way);
        });
        Func<int, Action<ConsoleKeyInfo>> part = way => pressed(() =>
        {
            _bEditorMarker.BMarkerClose();
            _bEditorPlacard.BPlacardClose();
            _bEditorPlacard.BPlacardPartMove(way);
            BEditorLitMove(0);
        });
        Func<int, Func<BKeymapLive>> inner = way => when(() => _bEditorField is null
            && _bEditorPlacard.BPlacardPartCheck(way));
        Func<int, Action<ConsoleKeyInfo>> choose = way => pressed(() => BEditorStackSelect(_bEditorStack + way));
        Action<ConsoleKeyInfo> pick = key =>
        {
            BEditorStackSelect(key.Key - ConsoleKey.D1);
            changed();
        };
        Func<bool, Func<Func<bool>, Func<BKeymapLive>>> gated = visible => enabled => () =>
            !shown() || !visible ? BKeymapLive.BKeymapLiveHidden
            : enabled() ? BKeymapLive.BKeymapLiveShown
            : BKeymapLive.BKeymapLiveDim;
        CDeskDraft draft = editor.CEditorDesk.CDeskDraft;
        const BKeymapFocus window = BKeymapFocus.BKeymapFocusWindow;
        Func<BKeymapLive> favored = () =>
            !shown() || (reading?.Invoke() ?? false) || _bEditorDraft is null ? BKeymapLive.BKeymapLiveHidden
            : !editor.CEditorDesk.CDeskStored || !steps.CDeskChronicleRunning ? BKeymapLive.BKeymapLiveDim
            : BKeymapLive.BKeymapLiveShown;
        BEditorKeymap = new BKeymap(
        [
            new(ConsoleKey.F2, 0, window, "Scribe.Save", pressed(BEditorEntrySave),
                gated(true)(() => draft.CDeskDraftStorable)),
            new(ConsoleKey.F7, 0, window, "Scribe.New", pressed(editor.CEditorEntryUndo),
                () => gated(editor.CEditorOwned)(() => draft.CDeskDraftAltered)()),
            new(ConsoleKey.F6, 0, window, "Favorite.Mark", BEditorEsteemObserve, favored),
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
            .. _bEditorMarker.BMarkerKeymap,
            .. _bEditorPlacard.BPlacardKeymap,
            .. _bEditorCard.BCardKeymap,
            new(ConsoleKey.UpArrow, 0, side, "Window.Help.Item",
                pressed(() => _bEditorSpeaker.BSpeakerDrop?.BDropLitMove(-1)), dropped),
            new(ConsoleKey.DownArrow, 0, side, "Window.Help.Item",
                pressed(() => _bEditorSpeaker.BSpeakerDrop?.BDropLitMove(1)), dropped),
            new(ConsoleKey.Enter, 0, side, "Editor.Pick", pressed(BEditorLanguageSelect), chosen),
            new(ConsoleKey.Escape, 0, side, "Editor.Shut", pressed(_bEditorSpeaker.BSpeakerClose), dropped),
            new(ConsoleKey.Enter, 0, side, "Editor.Language",
                pressed(() => _ = _bEditorSpeaker.BSpeakerOpen()), speaker),
            new(ConsoleKey.LeftArrow, 0, side, "Grasp.Mark", BEditorEsteemObserve, esteem),
            new(ConsoleKey.RightArrow, 0, side, "Grasp.Mark", BEditorEsteemObserve, esteem),
            digit(ConsoleKey.D1),
            digit(ConsoleKey.D2),
            digit(ConsoleKey.D3),
            digit(ConsoleKey.D4),
            digit(ConsoleKey.D5),
            digit(ConsoleKey.D6),
            digit(ConsoleKey.D7),
            digit(ConsoleKey.D8),
            digit(ConsoleKey.D9),
            new(ConsoleKey.UpArrow, 0, side, "Window.Help.Item", part(-1), inner(-1)),
            new(ConsoleKey.DownArrow, 0, side, "Window.Help.Item", part(1), inner(1)),
            new(ConsoleKey.UpArrow, 0, side, "Window.Help.Item", row(-1), lit),
            new(ConsoleKey.DownArrow, 0, side, "Window.Help.Item", row(1), lit),
            new(ConsoleKey.UpArrow, 0, side, "Editor.Close", BEditorFieldClose, opened),
            new(ConsoleKey.DownArrow, 0, side, "Editor.Close", BEditorFieldClose, opened),
            new(ConsoleKey.PageUp, 0, side, "Window.Help.Scroll", move(() => -_bEditorSight.BEditorSightPage), lit),
            new(ConsoleKey.PageDown, 0, side, "Window.Help.Scroll", move(() => _bEditorSight.BEditorSightPage), lit),
            new(ConsoleKey.Home, ConsoleModifiers.Control, side, "Window.Help.Scroll", move(() => -_bEditorLit), lit),
            new(ConsoleKey.End, ConsoleModifiers.Control, side, "Window.Help.Scroll",
                move(() => editor.CEditorWalk[_bEditorStack].Count + BEditorHeadSpan), lit),
            new(ConsoleKey.Enter, 0, side, "Editor.Open", pressed(BEditorFieldOpen), head),
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
        bool typed = !char.IsControl(key.KeyChar) && _bEditorField is null && _bEditorDraft is not null
            && _bEditorGate.CEditorDesk.CDeskChronicle.CDeskChronicleRunning;
        if (typed && _bEditorLit == BEditorHeadwordRow)
        {
            BEditorFieldOpen();
        }

        bool taken = _bEditorField?.BSParleyBodyObserve(key)
            ?? (_bEditorMarker.BMarkerObserve(key)
                || _bEditorCard.BCardObserve(key)
                || _bEditorPlacard.BPlacardObserve(key)
                || _bEditorSpeaker.BSpeakerObserve(key));
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
            (_bEditorDraft, _bEditorField, _bEditorLit, _bEditorStack, _bEditorPlacard.BPlacardPart),
            (_bEditorGate.CEditorWalk, _bEditorGate.CEditorWalkCard, _bEditorGate.CEditorOwned));
    }

    private void BEditorLitMove(int step)
    {
        int count = _bEditorGate.CEditorWalk.ElementAtOrDefault(_bEditorStack)?.Count ?? 0;
        int last = count + BEditorHeadSpan - (_bEditorStack < BEditorSight.BEditorSightAdder.Count ? 0 : 1);
        _bEditorLit = Math.Clamp(_bEditorLit + step, 0, Math.Max(last, 0));
        if (_bEditorLit != BEditorLanguageRow)
        {
            _bEditorSpeaker.BSpeakerClose();
        }

        _bEditorPlacard.BPlacardRefine();
    }

    private void BEditorRowMove(int way)
    {
        int lit = _bEditorLit;
        BEditorLitMove(way);
        if (_bEditorLit != lit)
        {
            _bEditorPlacard.BPlacardPartSet(way < 0);
        }
    }

    private void BEditorStackSelect(int stack)
    {
        _bEditorMarker.BMarkerClose();
        _bEditorPlacard.BPlacardClose();
        _bEditorCard.BCardClose();
        _bEditorStack = Math.Clamp(stack, 0, 3);
        _bEditorPlacard.BPlacardPartSet(false);
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
        _bEditorPlacard.BPlacardClose();
        _bEditorCard.BCardClose();
        foreach (BInlet inlet in _bEditorPlacard.BPlacardInlet)
        {
            inlet.BInletReset();
        }

        _ = _bEditorGate.CEditorEntrySave();
    }

    private void BEditorEsteemObserve(ConsoleKeyInfo key)
    {
        _bEditorRegard.BRegardObserve(key);
        _bEditorChanged();
    }

    private void BEditorLanguageSelect()
    {
        _bEditorSpeaker.BSpeakerSelect(_bEditorGate.CEditorEntry.CEntryLanguageSet);
    }

    private void BEditorReset()
    {
        _bEditorField = null;
        _bEditorSpeaker.BSpeakerClose();
        _bEditorMarker.BMarkerClose();
        _bEditorPlacard.BPlacardReset();
        _bEditorCard.BCardReset();
        BEditorLitMove(-_bEditorLit);
    }
}

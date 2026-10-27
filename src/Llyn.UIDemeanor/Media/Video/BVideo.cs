using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BVideo
{
    private const int BVideoPage = 4;

    private static readonly string[] BVideoMenuLabel = ["Card.Browse", "Card.VideoRemove"];

    private static readonly string[] BVideoHint = ["Card.LocationHint", "Card.TimestampHint"];

    private readonly BLocalizationCatalog _bVideoCatalog;

    private readonly CVideo _bVideoGate;

    private readonly Action<long, string>[] _bVideoSetter;

    private readonly Func<(bool, long?, CVideoDraft)?> _bVideoSpot;

    private readonly Action _bVideoKeeper;

    private readonly BKeymapRow _bVideoOpenRow;

    private long? _bVideoFieldId;

    private BDrop? _bVideoMenu;

    private int _bVideoSlot;

    private int _bVideoFieldSlot;

    internal BVideo(
        BLocalizationCatalog catalog,
        CVideo gate,
        Func<(bool, long?, CVideoDraft)?> spot,
        Func<Func<bool>, Func<BKeymapLive>> when,
        (Action, Action) host)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(gate);
        ArgumentNullException.ThrowIfNull(spot);
        ArgumentNullException.ThrowIfNull(when);
        ArgumentNullException.ThrowIfNull(host.Item1);
        ArgumentNullException.ThrowIfNull(host.Item2);

        _bVideoCatalog = catalog;
        _bVideoGate = gate;
        _bVideoSetter = [_bVideoGate.CVideoLocationSet, _bVideoGate.CVideoSpanSet];
        _bVideoSpot = spot;
        Action changed;
        (changed, _bVideoKeeper) = host;

        const BKeymapFocus side = BKeymapFocus.BKeymapFocusEditor;
        Func<Action, Action<ConsoleKeyInfo>> pressed = action => _ =>
        {
            action();
            changed();
        };
        Func<bool> calm = () => _bVideoSpot() is not null && BVideoField is null && _bVideoMenu is null;
        Func<BKeymapLive> idle = when(calm);
        Func<BKeymapLive> back = when(() => calm() && _bVideoSlot > 0);
        Func<BKeymapLive> forth = when(() => calm() && _bVideoSlot == 0);
        Func<BKeymapLive> typing = when(() => _bVideoSpot() is not null && BVideoField is not null);
        Func<BKeymapLive> menu = when(() => _bVideoSpot() is not null && _bVideoMenu is not null);
        Action<ConsoleKeyInfo> close = pressed(BVideoSettle);
        _bVideoOpenRow = new(ConsoleKey.Enter, 0, side, "Editor.Open", _ => BVideoFieldOpen(), idle);
        BVideoKeymap =
        [
            new(ConsoleKey.UpArrow, 0, side, "Window.Help.Item", pressed(() => _bVideoMenu?.BDropLitMove(-1)), menu),
            new(ConsoleKey.DownArrow, 0, side, "Window.Help.Item", pressed(() => _bVideoMenu?.BDropLitMove(1)), menu),
            new(ConsoleKey.Enter, 0, side, "Editor.Pick", pressed(BVideoMenuSelect), menu),
            new(ConsoleKey.Escape, 0, side, "Editor.Shut", pressed(() => _bVideoMenu = null), menu),
            new(ConsoleKey.Enter, 0, side, "Card.MenuOpen", pressed(BVideoMenuOpen), idle),
            new(ConsoleKey.Insert, 0, side, "Card.VideoAdd", pressed(BVideoAdd), idle),
            new(ConsoleKey.Delete, 0, side, "Card.VideoRemove", pressed(BVideoRemove), idle),
            new(ConsoleKey.O, ConsoleModifiers.Control, side, "Card.Browse", pressed(BVideoFileOpen), idle),
            new(ConsoleKey.LeftArrow, 0, side, "Card.VideoSlot", pressed(() => _bVideoSlot = 0), back),
            new(ConsoleKey.RightArrow, 0, side, "Card.VideoSlot", pressed(() => _bVideoSlot = 1), forth),
            new(ConsoleKey.Enter, 0, side, "Editor.Close", close, typing),
            new(ConsoleKey.Escape, 0, side, "Editor.Close", close, typing),
            new(ConsoleKey.UpArrow, 0, side, "Editor.Close", close, typing),
            new(ConsoleKey.DownArrow, 0, side, "Editor.Close", close, typing),
        ];
    }

    internal IReadOnlyList<BKeymapRow> BVideoKeymap { get; }

    internal BField? BVideoField { get; private set; }

    internal bool BVideoObserve(ConsoleKeyInfo key)
    {
        if (_bVideoSpot() is null)
        {
            return false;
        }

        if (_bVideoMenu is BDrop menu)
        {
            if (char.IsControl(key.KeyChar))
            {
                return false;
            }

            _ = menu.BDropSeek(key.KeyChar);
            return true;
        }

        if (!char.IsControl(key.KeyChar))
        {
            _bVideoOpenRow.BKeymapRowAction(key);
        }

        return BVideoField?.BSParleyBodyObserve(key) ?? false;
    }

    internal void BVideoClose()
    {
        _ = BVideoField?.BFieldPendingApply(null);
        BVideoField = null;
        _bVideoMenu = null;
        _bVideoFieldId = null;
    }

    internal void BVideoReset()
    {
        BVideoField = null;
        _bVideoMenu = null;
        _bVideoFieldId = null;
        _bVideoSlot = 0;
    }

    internal (Func<int, IEnumerable<BCanvasText>>, int?) BVideoDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        BCanvasWidth measure,
        (int, int) place,
        CVideoDraft video,
        (string?, bool) look)
    {
        ArgumentNullException.ThrowIfNull(glyphs);
        ArgumentNullException.ThrowIfNull(measure);
        ArgumentNullException.ThrowIfNull(video);

        (int column, int width) = place;
        (string? tone, bool focused) = look;
        string? shine = tone is "lit" or "lit-other" ? tone : null;
        string? calm = tone == "muted" ? tone : null;
        List<(string, string)> lead = BImage.BImageCaptionRead(_bVideoCatalog, measure, true);
        int offset = lead.Sum(piece => measure.BCanvasWidthResolve(piece.Item1));
        int gap = measure.BCanvasWidthResolve(BImage.BImageGap);
        string hint = _bVideoCatalog.BLocalizationTextRead(BVideoHint[1]);
        int spanCells = Math.Max(measure.BCanvasWidthResolve(video.CVideoDraftSpan.CStateValueText),
            measure.BCanvasWidthResolve(hint));
        int locationCells = Math.Max(width - offset - gap - spanCells, 1);
        int right = column + width;
        (int, int)[] cells =
        [
            (column + offset, Math.Min(locationCells, Math.Max(right - column - offset, 0))),
            (column + offset + locationCells + gap,
                Math.Min(spanCells, Math.Max(right - column - offset - locationCells - gap, 0))),
        ];
        BField? field = _bVideoFieldId == video.CVideoDraftId ? BVideoField : null;
        int open = _bVideoFieldSlot;
        int lit = _bVideoSlot;
        string ellipsis = BLecternHeader.BLecternGlyphRead(glyphs, "Ellipsis", 0, "~");
        (string, string)[] slots =
        [
            BVideoValueRead(video.CVideoDraftLocation, BVideoHint[0], video.CVideoDraftScreen is null,
                (measure, cells[0].Item2, ellipsis)),
            BVideoValueRead(video.CVideoDraftSpan, BVideoHint[1], false, (measure, cells[1].Item2, ellipsis)),
        ];
        int? entry = shine is not null && _bVideoMenu is not null ? Math.Min(column + offset, right - 1) : null;
        return (line =>
        {
            List<BCanvasText> texts =
                [.. BEditorSight.BEditorSightDraw(measure, (line, column, Math.Min(offset, width)), calm, lead)];
            for (int slot = 0; slot < BVideoHint.Length; slot++)
            {
                (int start, int room) = cells[slot];
                if (room <= 0)
                {
                    continue;
                }

                (string text, string role) = slots[slot];
                texts.AddRange(field is not null && open == slot
                    ? field.BSParleyBodyDraw(glyphs, line, start, room, 1, focused, "field", measure)
                    : BEditorSight.BEditorSightDraw(measure, (line, start, room), slot == lit ? shine ?? calm : calm,
                        [(measure.BCanvasWidthShorten(text, room, ellipsis), role)]));
            }

            return texts;
        }, entry);
    }

    internal IEnumerable<BCanvasText> BVideoDropDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        BCanvasWidth measure,
        BBlueprintPart bounds,
        ((int, int)?, bool) look)
    {
        ArgumentNullException.ThrowIfNull(glyphs);
        ArgumentNullException.ThrowIfNull(measure);
        ArgumentNullException.ThrowIfNull(bounds);

        ((int, int)? anchor, bool focused) = look;
        return _bVideoMenu is BDrop menu && _bVideoSpot() is not null && anchor is (int line, int start)
            ? menu.BDropDraw(glyphs, measure, bounds, (line, start), ([], focused))
            : [];
    }

    private (string, string) BVideoValueRead(
        CStateValue value, string hint, bool gone, (BCanvasWidth, int, string) fit)
    {
        string text = value.CStateValueText;
        if (text.Length == 0 || !gone)
        {
            return text.Length == 0 ? (_bVideoCatalog.BLocalizationTextRead(hint), "muted") : (text, "text");
        }

        (BCanvasWidth measure, int cells, string ellipsis) = fit;
        string mark = " " + _bVideoCatalog.BLocalizationTextRead("Display.Missing");
        int room = Math.Max(cells - measure.BCanvasWidthResolve(mark), 0);
        int size = measure.BCanvasWidthResolve(text);
        string name = text[(text.LastIndexOfAny(['/', '\\']) + 1)..];
        return (size <= room ? text + mark
            : room >= measure.BCanvasWidthResolve(ellipsis + name)
                ? measure.BCanvasWidthShorten(text, room, ellipsis) + mark
            : size <= cells ? text
            : measure.BCanvasWidthShorten(text, cells, ellipsis), "muted");
    }

    private void BVideoFieldOpen()
    {
        if (_bVideoSpot() is not { } spot)
        {
            return;
        }

        int slot = _bVideoSlot;
        _bVideoFieldId = spot.Item3.CVideoDraftId;
        _bVideoFieldSlot = slot;
        BVideoField = new BField(
            text => _bVideoSetter[slot](spot.Item3.CVideoDraftId, text),
            new[] { spot.Item3.CVideoDraftLocation, spot.Item3.CVideoDraftSpan }[slot].CStateValueText,
            _bVideoCatalog,
            BVideoHint[slot]);
    }

    private void BVideoSettle()
    {
        if (BVideoField is null)
        {
            _bVideoMenu = null;
            return;
        }

        BVideoClose();
        _bVideoKeeper();
    }

    private void BVideoMenuOpen()
    {
        _bVideoMenu = new BDrop(
            [.. BVideoMenuLabel.Select(_bVideoCatalog.BLocalizationTextRead)], 0, true, BVideoPage);
    }

    private void BVideoMenuSelect()
    {
        int pick = _bVideoMenu?.BDropLit ?? 0;
        _bVideoMenu = null;
        (pick == 0 ? (Action)BVideoFileOpen : BVideoRemove)();
    }

    private void BVideoAdd()
    {
        if (_bVideoSpot() is { } spot)
        {
            _bVideoGate.CVideoAdd(spot.Item1, spot.Item2);
        }
    }

    private void BVideoRemove()
    {
        if (_bVideoSpot() is { } spot)
        {
            _bVideoGate.CVideoRemove(spot.Item1, spot.Item2, spot.Item3.CVideoDraftId);
        }
    }

    private void BVideoFileOpen()
    {
        BVideoSettle();
        if (_bVideoSpot() is { } spot)
        {
            _bVideoGate.CVideoFileOpen(spot.Item3.CVideoDraftId);
        }
    }
}

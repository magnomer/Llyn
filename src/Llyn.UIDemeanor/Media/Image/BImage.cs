using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BImage
{
    internal const int BImageIndent = 3;

    internal const string BImageGap = "  ";

    private const int BImagePage = 4;

    private static readonly string[] BImageCaption = ["Card.ImageCaption", "Card.VideoCaption"];

    private static readonly string[] BImageMenuLabel = ["Card.Browse", "Card.ImageRemove"];

    private readonly BLocalizationCatalog _bImageCatalog;

    private readonly CImage _bImageGate;

    private readonly Func<(bool, long?, CImageDraft)?> _bImageSpot;

    private readonly Action _bImageKeeper;

    private readonly BKeymapRow _bImageOpenRow;

    private long? _bImageFieldId;

    private BDrop? _bImageMenu;

    internal BImage(
        BLocalizationCatalog catalog,
        CImage gate,
        Func<(bool, long?, CImageDraft)?> spot,
        Func<Func<bool>, Func<BKeymapLive>> when,
        (Action, Action) host)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(gate);
        ArgumentNullException.ThrowIfNull(spot);
        ArgumentNullException.ThrowIfNull(when);
        ArgumentNullException.ThrowIfNull(host.Item1);
        ArgumentNullException.ThrowIfNull(host.Item2);

        _bImageCatalog = catalog;
        _bImageGate = gate;
        _bImageSpot = spot;
        Action changed;
        (changed, _bImageKeeper) = host;

        const BKeymapFocus side = BKeymapFocus.BKeymapFocusEditor;
        Func<Action, Action<ConsoleKeyInfo>> pressed = action => _ =>
        {
            action();
            changed();
        };
        Func<bool> calm = () => _bImageSpot() is not null && BImageField is null && _bImageMenu is null;
        Func<BKeymapLive> idle = when(calm);
        Func<BKeymapLive> typing = when(() => _bImageSpot() is not null && BImageField is not null);
        Func<BKeymapLive> menu = when(() => _bImageSpot() is not null && _bImageMenu is not null);
        Action<ConsoleKeyInfo> close = pressed(BImageSettle);
        _bImageOpenRow = new(ConsoleKey.Enter, 0, side, "Editor.Open", _ => BImageFieldOpen(), idle);
        BImageKeymap =
        [
            new(ConsoleKey.UpArrow, 0, side, "Window.Help.Item", pressed(() => _bImageMenu?.BDropLitMove(-1)), menu),
            new(ConsoleKey.DownArrow, 0, side, "Window.Help.Item", pressed(() => _bImageMenu?.BDropLitMove(1)), menu),
            new(ConsoleKey.Enter, 0, side, "Editor.Pick", pressed(BImageMenuSelect), menu),
            new(ConsoleKey.Escape, 0, side, "Editor.Shut", pressed(() => _bImageMenu = null), menu),
            new(ConsoleKey.Enter, 0, side, "Card.MenuOpen", pressed(BImageMenuOpen), idle),
            new(ConsoleKey.Insert, 0, side, "Card.ImageAdd", pressed(BImageAdd), idle),
            new(ConsoleKey.Delete, 0, side, "Card.ImageRemove", pressed(BImageRemove), idle),
            new(ConsoleKey.O, ConsoleModifiers.Control, side, "Card.Browse", pressed(BImageFileOpen), idle),
            new(ConsoleKey.Enter, 0, side, "Editor.Close", close, typing),
            new(ConsoleKey.Escape, 0, side, "Editor.Close", close, typing),
            new(ConsoleKey.UpArrow, 0, side, "Editor.Close", close, typing),
            new(ConsoleKey.DownArrow, 0, side, "Editor.Close", close, typing),
        ];
    }

    internal IReadOnlyList<BKeymapRow> BImageKeymap { get; }

    internal BField? BImageField { get; private set; }

    internal bool BImageObserve(ConsoleKeyInfo key)
    {
        if (_bImageSpot() is null)
        {
            return false;
        }

        if (_bImageMenu is BDrop menu)
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
            _bImageOpenRow.BKeymapRowAction(key);
        }

        return BImageField?.BSParleyBodyObserve(key) ?? false;
    }

    internal void BImageClose()
    {
        _ = BImageField?.BFieldPendingApply(null);
        BImageReset();
    }

    internal void BImageReset()
    {
        BImageField = null;
        _bImageMenu = null;
        _bImageFieldId = null;
    }

    internal (Func<int, IEnumerable<BCanvasText>>, int?) BImageDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        BCanvasWidth measure,
        (int, int) place,
        CImageDraft image,
        (string?, bool) look)
    {
        ArgumentNullException.ThrowIfNull(glyphs);
        ArgumentNullException.ThrowIfNull(measure);
        ArgumentNullException.ThrowIfNull(image);

        (int column, int width) = place;
        (string? tone, bool focused) = look;
        List<(string, string)> lead = BImageCaptionRead(_bImageCatalog, measure, false);
        int offset = lead.Sum(piece => measure.BCanvasWidthResolve(piece.Item1));
        bool lit = tone is "lit" or "lit-other";
        int? entry = lit && _bImageMenu is not null ? column + Math.Min(offset, Math.Max(width - 1, 0)) : null;
        if (BImageField is BField field && _bImageFieldId == image.CImageDraftId)
        {
            return (line => [
                .. BEditorSight.BEditorSightDraw(measure, (line, column, Math.Min(offset, width)), tone, lead),
                .. field.BSParleyBodyDraw(
                    glyphs, line, column + offset, Math.Max(width - offset, 1), 1, focused, "field", measure),
            ], entry);
        }

        string ellipsis = BLecternHeader.BLecternGlyphRead(glyphs, "Ellipsis", 0, "~");
        List<(string, string)> pieces =
        [
            .. lead,
            BImageLocationRead(
                _bImageCatalog,
                image.CImageDraftWording,
                image.CImageDraftAddress is null,
                (measure, Math.Max(width - offset, 0), ellipsis)),
        ];
        return (line => BEditorSight.BEditorSightDraw(measure, (line, column, width), tone, pieces), entry);
    }

    internal IEnumerable<BCanvasText> BImageDropDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        BCanvasWidth measure,
        BBlueprintPart bounds,
        ((int, int)?, bool) look)
    {
        ArgumentNullException.ThrowIfNull(glyphs);
        ArgumentNullException.ThrowIfNull(measure);
        ArgumentNullException.ThrowIfNull(bounds);

        ((int, int)? anchor, bool focused) = look;
        return _bImageMenu is BDrop menu && _bImageSpot() is not null && anchor is (int line, int start)
            ? menu.BDropDraw(glyphs, measure, bounds, (line, start), ([], focused))
            : [];
    }

    internal static List<(string, string)> BImageCaptionRead(
        BLocalizationCatalog catalog, BCanvasWidth measure, bool filmed)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(measure);

        string caption = catalog.BLocalizationTextRead(BImageCaption[filmed ? 1 : 0]);
        int cells = BImageCaption.Max(key => measure.BCanvasWidthResolve(catalog.BLocalizationTextRead(key)));
        string pad = new(' ', Math.Max(cells - measure.BCanvasWidthResolve(caption), 0));
        return [(new string(' ', BImageIndent), "text"), (caption + pad, "muted"), (BImageGap, "text")];
    }

    private static (string, string) BImageLocationRead(
        BLocalizationCatalog catalog, CStateWording wording, bool gone, (BCanvasWidth, int, string) fit)
    {
        if (wording.CStateWordingMuted)
        {
            return (catalog.BLocalizationTextRead(wording.CStateWordingHint ?? "Card.LocationHint"), "muted");
        }

        string text = wording.CStateWordingKey is string key
            ? catalog.BLocalizationTextRead(key)
            : wording.CStateWordingText;
        if (!gone)
        {
            return (text, wording.CStateWordingKey is null ? "text" : "muted");
        }

        (BCanvasWidth measure, int cells, string ellipsis) = fit;
        string mark = " " + catalog.BLocalizationTextRead("Display.Missing");
        int room = Math.Max(cells - measure.BCanvasWidthResolve(mark), 0);
        int size = measure.BCanvasWidthResolve(text);
        string name = text[(text.LastIndexOfAny(['/', '\\']) + 1)..];
        return (size <= room ? text + mark
            : room >= measure.BCanvasWidthResolve(ellipsis + name)
                ? measure.BCanvasWidthShorten(text, room, ellipsis) + mark
            : size <= cells ? text
            : measure.BCanvasWidthShorten(text, cells, ellipsis), "muted");
    }

    private void BImageFieldOpen()
    {
        if (_bImageSpot() is not { } spot)
        {
            return;
        }

        _bImageFieldId = spot.Item3.CImageDraftId;
        BImageField = new BField(
            text => _bImageGate.CImageLocationSet(spot.Item3.CImageDraftId, text),
            spot.Item3.CImageDraftWording.CStateWordingText,
            _bImageCatalog,
            "Card.LocationHint");
    }

    private void BImageSettle()
    {
        if (BImageField is null)
        {
            _bImageMenu = null;
            return;
        }

        BImageClose();
        _bImageKeeper();
    }

    private void BImageMenuOpen()
    {
        _bImageMenu = new BDrop(
            [.. BImageMenuLabel.Select(_bImageCatalog.BLocalizationTextRead)], 0, true, BImagePage);
    }

    private void BImageMenuSelect()
    {
        int pick = _bImageMenu?.BDropLit ?? 0;
        _bImageMenu = null;
        (pick == 0 ? (Action)BImageFileOpen : BImageRemove)();
    }

    private void BImageAdd()
    {
        if (_bImageSpot() is { } spot)
        {
            _bImageGate.CImageAdd(spot.Item1, spot.Item2);
        }
    }

    private void BImageRemove()
    {
        if (_bImageSpot() is { } spot)
        {
            _bImageGate.CImageRemove(spot.Item1, spot.Item2, spot.Item3.CImageDraftId);
        }
    }

    private void BImageFileOpen()
    {
        BImageSettle();
        if (_bImageSpot() is { } spot)
        {
            _bImageGate.CImageFileOpen(spot.Item3.CImageDraftId);
        }
    }
}

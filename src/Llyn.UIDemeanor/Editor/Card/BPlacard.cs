using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BPlacard
{
    private static readonly string[] BPlacardAdder = ["Card.ImageAdd", "Card.VideoAdd"];

    private readonly BLocalizationCatalog _bPlacardCatalog;

    private readonly Func<(bool, CStep)?> _bPlacardLit;

    private readonly Action _bPlacardKeeper;

    private readonly BImage _bPlacardImage;

    private readonly BVideo _bPlacardVideo;

    private readonly (CImage, CVideo) _bPlacardGate;

    private int _bPlacardPart;

    private int _bPlacardPartLast;

    private int _bPlacardAdderLit;

    internal BPlacard(
        BLocalizationCatalog catalog,
        (CCaret, Action, CImage, CVideo) gates,
        Func<(bool, CStep)?> lit,
        Func<Func<bool>, Func<BKeymapLive>> when,
        Action changed)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(gates.Item1);
        ArgumentNullException.ThrowIfNull(gates.Item2);
        ArgumentNullException.ThrowIfNull(gates.Item3);
        ArgumentNullException.ThrowIfNull(gates.Item4);
        ArgumentNullException.ThrowIfNull(lit);
        ArgumentNullException.ThrowIfNull(when);
        ArgumentNullException.ThrowIfNull(changed);

        _bPlacardCatalog = catalog;
        _bPlacardLit = lit;
        _bPlacardKeeper = gates.Item2;
        Func<int, Func<(bool, long)?>> spot = part => () =>
            _bPlacardLit() is (bool collocated, { CStepCard: CCardDraft card })
                && BPlacardPart == part + BCard.BCardLeadRead(collocated)
                ? (collocated, card.CCardDraftId)
                : null;
        BContext context = new(catalog, gates.Item1, (spot(1), spot(2)), when, changed);
        BLabel label = new(catalog, gates.Item1, spot(3), when, changed);
        BLink link = new(catalog, gates.Item1, spot(4), when, changed);
        BPlacardInlet = [context.BContextSituation, context.BContextRegister, label.BLabelTag, link.BLinkTranslation];
        _bPlacardImage = new BImage(catalog, gates.Item3, () =>
            BPlacardMediaRead() is (bool collocated, CCardDraft card, int at)
                && card.CCardDraftImage.ElementAtOrDefault(at) is CImageDraft image
                ? (collocated, card.CCardDraftId, image)
                : null,
            when, (changed, gates.Item2));
        _bPlacardVideo = new BVideo(catalog, gates.Item4, () =>
            BPlacardMediaRead() is (bool collocated, CCardDraft card, int at)
                && card.CCardDraftVideo.ElementAtOrDefault(at - card.CCardDraftImage.Count) is CVideoDraft video
                ? (collocated, card.CCardDraftId, video)
                : null,
            when, (changed, gates.Item2));
        Func<bool> adding = () => BPlacardMediaRead() is (_, CCardDraft card, int at)
            && at == card.CCardDraftImage.Count + card.CCardDraftVideo.Count;
        Func<int, Func<BKeymapLive>> adder = kind => when(() => adding() && _bPlacardAdderLit == kind);
        Func<int, Action<ConsoleKeyInfo>> light = kind => _ =>
        {
            _bPlacardAdderLit = kind;
            changed();
        };
        _bPlacardGate = (gates.Item3, gates.Item4);
        Func<Action, Action<ConsoleKeyInfo>> pressed = action => _ =>
        {
            action();
            changed();
        };
        Action<ConsoleKeyInfo> picture = pressed(BPlacardImageAdd);
        Action<ConsoleKeyInfo> film = pressed(BPlacardVideoAdd);
        Func<BKeymapLive> shut = when(BPlacardShutCheck);
        BPlacardKeymap =
        [
            .. context.BContextKeymap,
            .. label.BLabelKeymap,
            .. link.BLinkKeymap,
            .. _bPlacardImage.BImageKeymap,
            .. _bPlacardVideo.BVideoKeymap,
            new(ConsoleKey.Enter, 0, BKeymapFocus.BKeymapFocusEditor, BPlacardAdder[0], picture, adder(0)),
            new(ConsoleKey.Enter, 0, BKeymapFocus.BKeymapFocusEditor, BPlacardAdder[1], film, adder(1)),
            new(ConsoleKey.RightArrow, 0, BKeymapFocus.BKeymapFocusEditor, "Card.AdderLight", light(1), adder(0)),
            new(ConsoleKey.LeftArrow, 0, BKeymapFocus.BKeymapFocusEditor, "Card.AdderLight", light(0), adder(1)),
            new(ConsoleKey.Escape, 0, BKeymapFocus.BKeymapFocusEditor, "Editor.Close", _ =>
            {
                BPlacardClose();
                changed();
            }, shut),
        ];
    }

    internal IReadOnlyList<BInlet> BPlacardInlet { get; }

    internal IReadOnlyList<BKeymapRow> BPlacardKeymap { get; }

    internal int BPlacardPart => _bPlacardPart < 0
        ? Math.Max(_bPlacardPartLast + 1 + _bPlacardPart, 0)
        : Math.Min(_bPlacardPart, _bPlacardPartLast);

    internal bool BPlacardPartCheck(int way)
    {
        BPlacardPartRead();
        return way > 0 ? BPlacardPart < _bPlacardPartLast : BPlacardPart > 0;
    }

    internal void BPlacardPartMove(int way)
    {
        _bPlacardPart += way;
    }

    internal void BPlacardPartSet(bool last)
    {
        _bPlacardPart = last ? -1 : 0;
    }

    internal bool BPlacardObserve(ConsoleKeyInfo key)
    {
        bool taken = false;
        foreach (BInlet inlet in BPlacardInlet)
        {
            taken |= inlet.BInletObserve(key);
        }

        taken |= _bPlacardImage.BImageObserve(key);
        taken |= _bPlacardVideo.BVideoObserve(key);
        return taken;
    }

    internal void BPlacardRefine()
    {
        BPlacardPartRead();
        (bool, long)? card = _bPlacardLit() is (bool collocated, { CStepCard: CCardDraft draft })
            ? (collocated, draft.CCardDraftId)
            : null;
        foreach (BInlet inlet in BPlacardInlet)
        {
            inlet.BInletRefine(card);
        }
    }

    internal void BPlacardClose()
    {
        bool closed = BPlacardInlet.Any(static inlet => inlet.BInletField is not null)
            || _bPlacardImage.BImageField is not null || _bPlacardVideo.BVideoField is not null;
        foreach (BInlet inlet in BPlacardInlet)
        {
            inlet.BInletClose();
        }

        _bPlacardImage.BImageClose();
        _bPlacardVideo.BVideoClose();
        if (closed)
        {
            _bPlacardKeeper();
        }
    }

    internal IReadOnlyList<(int, Func<int, IEnumerable<BCanvasText>>, int?)> BPlacardMediaDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        BCanvasWidth measure,
        (int, int) place,
        (bool, CCardDraft) card,
        (Func<int, string?>, bool) look)
    {
        ArgumentNullException.ThrowIfNull(glyphs);
        ArgumentNullException.ThrowIfNull(measure);
        ArgumentNullException.ThrowIfNull(card.Item2);
        ArgumentNullException.ThrowIfNull(look.Item1);

        (int column, int width) = place;
        (bool collocated, CCardDraft draft) = card;
        (Func<int, string?> tone, bool focused) = look;
        int part = BCard.BCardLeadRead(collocated) + BPlacardInlet.Count + 1;
        List<(int, Func<int, IEnumerable<BCanvasText>>, int?)> lines = [];
        foreach (CImageDraft image in draft.CCardDraftImage)
        {
            (Func<int, IEnumerable<BCanvasText>> paint, int? entry) =
                _bPlacardImage.BImageDraw(glyphs, measure, place, image, (tone(part), focused));
            lines.Add((part++, paint, entry));
        }

        foreach (CVideoDraft video in draft.CCardDraftVideo)
        {
            (Func<int, IEnumerable<BCanvasText>> paint, int? entry) =
                _bPlacardVideo.BVideoDraw(glyphs, measure, place, video, (tone(part), focused));
            lines.Add((part++, paint, entry));
        }

        string? shade = tone(part);
        string? shine = shade is "lit" or "lit-other" ? shade : null;
        string? calm = shade == "muted" ? shade : null;
        List<(string, string)> pieces = [(new string(' ', BImage.BImageIndent), "text")];
        for (int kind = 0; kind < BPlacardAdder.Length; kind++)
        {
            pieces.Add(($"+ {_bPlacardCatalog.BLocalizationTextRead(BPlacardAdder[kind])}",
                kind == _bPlacardAdderLit ? shine ?? "text" : "text"));
            pieces.Add((kind + 1 < BPlacardAdder.Length ? BImage.BImageGap + BImage.BImageGap : string.Empty, "text"));
        }

        lines.Add((part, line => BEditorSight.BEditorSightDraw(measure, (line, column, width), calm, pieces), null));
        return lines;
    }

    internal IEnumerable<BCanvasText> BPlacardDropDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        BCanvasWidth measure,
        BBlueprintPart bounds,
        ((int, int)?, bool) look)
    {
        ArgumentNullException.ThrowIfNull(glyphs);
        ArgumentNullException.ThrowIfNull(measure);
        ArgumentNullException.ThrowIfNull(bounds);

        List<BCanvasText> texts = [];
        foreach (BInlet inlet in BPlacardInlet)
        {
            texts.AddRange(inlet.BInletDropDraw(glyphs, measure, bounds, look));
        }

        texts.AddRange(_bPlacardImage.BImageDropDraw(glyphs, measure, bounds, look));
        texts.AddRange(_bPlacardVideo.BVideoDropDraw(glyphs, measure, bounds, look));
        return texts;
    }

    internal void BPlacardFieldReset()
    {
        foreach (BInlet inlet in BPlacardInlet)
        {
            inlet.BInletReset();
        }

        _bPlacardImage.BImageReset();
        _bPlacardVideo.BVideoReset();
    }

    internal void BPlacardReset()
    {
        BPlacardFieldReset();
        _bPlacardPart = 0;
        _bPlacardAdderLit = 0;
    }

    private bool BPlacardShutCheck()
    {
        return BPlacardInlet.Any(static inlet => inlet.BInletShut);
    }

    private void BPlacardPartRead()
    {
        _bPlacardPartLast = _bPlacardLit() is (bool collocated, { CStepCard: { CCardDraftFolded: false } card })
            ? BCard.BCardLeadRead(collocated) + BPlacardInlet.Count + card.CCardDraftImage.Count
                + card.CCardDraftVideo.Count + 1
            : 0;
    }

    private (bool, CCardDraft, int)? BPlacardMediaRead()
    {
        return _bPlacardLit() is (bool collocated, { CStepCard: { CCardDraftFolded: false } card })
            && BPlacardPart - BCard.BCardLeadRead(collocated) - BPlacardInlet.Count - 1 is int at and >= 0
            ? (collocated, card, at)
            : null;
    }

    private void BPlacardImageAdd()
    {
        if (BPlacardMediaRead() is { } media)
        {
            _bPlacardGate.Item1.CImageAdd(media.Item1, media.Item2.CCardDraftId);
        }
    }

    private void BPlacardVideoAdd()
    {
        if (BPlacardMediaRead() is { } media)
        {
            _bPlacardGate.Item2.CVideoAdd(media.Item1, media.Item2.CCardDraftId);
        }
    }
}

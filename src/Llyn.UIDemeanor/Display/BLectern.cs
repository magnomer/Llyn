using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BLectern
{
    private readonly BLocalizationCatalog _bLecternCatalog;

    private readonly CDisplay _bLecternDisplay;

    private readonly BLecternHeader _bLecternHeader;

    private readonly BLecternCard _bLecternCard;

    private readonly BLecternIncoming _bLecternIncoming;

    private readonly BLecternMention _bLecternMention;

    private readonly BLecternEtymology _bLecternEtymology;

    private readonly BLecternNote _bLecternNote;

    private readonly Action _bLecternChanged;

    private readonly List<int> _bLecternMeaning = [];

    private readonly Dictionary<(CCompassPart, int?), int> _bLecternPart = [];

    private IReadOnlyList<BLecternLine> _bLecternFrame = [];

    private int _bLecternPage = 1;

    private int _bLecternTop;

    private string? _bLecternLit;

    private (int, int)? _bLecternAnchor;

    private (int, int) _bLecternEdge;

    internal BLectern(
        BLocalizationCatalog catalog,
        CDisplay display,
        CAtelier atelier,
        Action changed,
        Func<bool> focused,
        Func<BSParley, Task?, int?> docket,
        BBlueprintPart parley,
        Func<BCanvasWidth> measure)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(display);
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(changed);
        ArgumentNullException.ThrowIfNull(focused);

        CNavigation navigation = atelier.CAtelierNavigation;
        _bLecternCatalog = catalog;
        _bLecternDisplay = display;
        _bLecternChanged = changed;
        _bLecternHeader = new BLecternHeader(catalog, display, changed);
        _bLecternMention = new BLecternMention(
            catalog, display, navigation, () => _bLecternLit, () => _bLecternAnchor, focused, changed, docket, parley,
            measure);
        _bLecternCard = new BLecternCard(catalog, display, _bLecternMention);
        _bLecternIncoming = new BLecternIncoming(catalog, display.CDisplayCard, navigation);
        _bLecternEtymology = new BLecternEtymology(catalog, display, changed);
        _bLecternNote = new BLecternNote(catalog, atelier);

        const BKeymapFocus lectern = BKeymapFocus.BKeymapFocusLectern;
        Func<string?, Func<BKeymapLive>> lit = item => () =>
            focused() && (item is null || _bLecternLit == item)
                ? BKeymapLive.BKeymapLiveShown
                : BKeymapLive.BKeymapLiveHidden;
        Func<BKeymapLive> heart = lit(BLecternHeader.BLecternItemFavorite);
        Func<BKeymapLive> stars = lit(BLecternHeader.BLecternItemGrasp);
        Func<BKeymapLive> read = lit(null);
        Func<Func<bool>, Func<BKeymapLive>> held = check => () =>
            focused() && check() ? BKeymapLive.BKeymapLiveShown : BKeymapLive.BKeymapLiveHidden;
        Func<Func<string?, bool>, Func<BKeymapLive>> owned = check => held(() => check(_bLecternLit));

        Func<Func<string, string?>, Action<ConsoleKeyInfo>> follow = open => _ =>
        {
            _bLecternLit = open(_bLecternLit ?? string.Empty) ?? _bLecternLit;
            _bLecternChanged();
        };
        Func<BKeymapLive> fold = owned(_bLecternCard.BLecternFoldCheck);
        Func<Func<string, string?>, Func<string?, bool>, BKeymapRow> press = (open, check) =>
            new(ConsoleKey.Enter, 0, lectern, "Window.Help.Press", follow(open), owned(check));
        Action<ConsoleKeyInfo> contents = _ =>
        {
            BSCompass.BSCompassShow(
                catalog, parley, docket, display.CDisplayCompass, _bLecternPart, _bLecternTop, BLecternLineShow,
                _bLecternEdge, measure());
            changed();
        };
        BLecternKeymap = new BKeymap(
        [
            new(ConsoleKey.Enter, 0, lectern, "Favorite.Mark", BLecternFavoriteToggle, heart),
            new(ConsoleKey.Spacebar, 0, lectern, "Card.Fold", follow(_bLecternCard.BLecternFoldToggle), fold),
            new(ConsoleKey.Enter, 0, lectern, "Card.Fold", follow(_bLecternCard.BLecternFoldToggle), fold),
            press(_bLecternCard.BLecternSeeOpen, _bLecternCard.BLecternSeeCheck),
            press(_bLecternCard.BLecternChipOpen, _bLecternCard.BLecternChipCheck),
            press(_bLecternHeader.BLecternGlyphOpen, _bLecternHeader.BLecternGlyphCheck),
            press(_bLecternIncoming.BLecternIncomingOpen, _bLecternIncoming.BLecternIncomingCheck),
            press(_bLecternEtymology.BLecternEtymologyOpen, _bLecternEtymology.BLecternEtymologyCheck),
            press(_bLecternNote.BLecternNoteOpen, _bLecternNote.BLecternNoteCheck),
            .. _bLecternMention.BLecternMentionKeymap.BKeymapRead(lectern),
            .. Enumerable.Range(0, 9).Select(index => new BKeymapRow(ConsoleKey.D1 + index, 0, lectern,
                "Display.Meaning", BLecternMeaningObserve, held(() => index < _bLecternMeaning.Count))),
            new(ConsoleKey.F3, ConsoleModifiers.Alt, lectern, "Contents.Title", contents, read),
            new(ConsoleKey.LeftArrow, 0, lectern, "Grasp.Mark", BLecternGraspMove, stars),
            new(ConsoleKey.RightArrow, 0, lectern, "Grasp.Mark", BLecternGraspMove, stars),
            new(ConsoleKey.UpArrow, 0, lectern, "Window.Help.Item", BLecternObserve, read),
            new(ConsoleKey.DownArrow, 0, lectern, "Window.Help.Item", BLecternObserve, read),
            new(ConsoleKey.PageUp, 0, lectern, "Window.Help.Scroll", BLecternObserve, read),
            new(ConsoleKey.PageDown, 0, lectern, "Window.Help.Scroll", BLecternObserve, read),
            new(ConsoleKey.Home, 0, lectern, "Window.Help.Scroll", BLecternObserve, read),
            new(ConsoleKey.End, 0, lectern, "Window.Help.Scroll", BLecternObserve, read),
        ]);

        display.CDisplayCompass.CCompassSought += (part, place) =>
        {
            _bLecternCard.BLecternCardSought = (part, place);
            changed();
        };
        display.CDisplayOpened += _bLecternIncoming.BLecternIncomingRefine;
        display.CDisplayOpened += _bLecternHeader.BLecternGlyphRefine;
        display.CDisplayOpened += BLecternCardRefine;
        display.CDisplayClosed += _bLecternIncoming.BLecternIncomingRefine;
        display.CDisplayClosed += _bLecternHeader.BLecternGlyphRefine;
        display.CDisplayClosed += BLecternCardRefine;
        display.CDisplayFoldChanged += _ => BLecternCardRefine();

        Func<Action, Action<CBulletin>> resonate = run => BObserver.BObserverCreate<CBulletin>(_ => run());
        display.CDisplayEntryChanged += resonate(display.CDisplayEntryResonate);
        display.CDisplayWorkspaceChanged += resonate(display.CDisplayWorkspaceResonate);
    }

    internal BKeymap BLecternKeymap { get; }

    internal void BLecternReset()
    {
        _bLecternTop = 0;
        _bLecternLit = null;
        _bLecternMention.BLecternMentionReset();
    }

    internal void BLecternFavoriteToggle(ConsoleKeyInfo key)
    {
        _bLecternHeader.BLecternFavoriteToggle();
        _bLecternChanged();
    }

    internal IReadOnlyList<BCanvasText> BLecternDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        int row,
        int column,
        int width,
        int height,
        bool focused,
        BCanvasWidth measure)
    {
        ArgumentNullException.ThrowIfNull(glyphs);
        ArgumentNullException.ThrowIfNull(measure);

        List<BCanvasText> texts = [];
        if (width <= 0 || height <= 0)
        {
            return texts;
        }

        if (_bLecternDisplay.CDisplayBlank)
        {
            BLecternReset();
            string prompt = _bLecternCatalog.BLocalizationTextRead("Display.Unselected");
            if (measure.BCanvasWidthResolve(prompt) > width)
            {
                prompt = measure.BCanvasWidthClamp(prompt, width);
            }

            int cells = measure.BCanvasWidthResolve(prompt);
            texts.Add(new BCanvasText(row + (height / 2), column + ((width - cells) / 2), cells, prompt, "muted"));
            return texts;
        }

        _bLecternPage = Math.Max(1, height - 1);
        _bLecternFrame = BLecternPageBuild(glyphs, width, measure);
        _bLecternEdge = (row, column + width);
        BLecternScroll(_bLecternTop);
        if (_bLecternCard.BLecternCardSought is (CCompassPart, int) place
            && _bLecternPart.TryGetValue(place, out int found))
        {
            string? kept = _bLecternLit;
            BLecternLineShow(found);
            _bLecternLit = BLecternItemsRead().Any(item => item.Item1 == kept
                && BLecternShownCheck(item.Item2)) ? kept : _bLecternLit;
        }

        _bLecternCard.BLecternCardSought = null;
        _bLecternLit = BLecternItemsRead().Any(item => item.Item1 == _bLecternLit) ? _bLecternLit : null;
        _bLecternAnchor = null;
        int last = Math.Min(_bLecternFrame.Count, _bLecternTop + _bLecternPage);
        for (int at = _bLecternTop; at < last; at++)
        {
            BLecternLine line = _bLecternFrame[at];
            int left = line.BLecternLineIndent;
            foreach ((string text, string role, string? item) in line.BLecternLinePieces)
            {
                int room = width - left;
                if (room <= 0)
                {
                    break;
                }

                string shown = measure.BCanvasWidthResolve(text) > room ? measure.BCanvasWidthClamp(text, room) : text;
                int cells = measure.BCanvasWidthResolve(shown);
                string drawn = item is not null && item == _bLecternLit ? focused ? "lit" : "lit-other" : role;
                _bLecternAnchor ??= drawn == role ? null : (row + at - _bLecternTop, column + left);
                texts.Add(new BCanvasText(row + at - _bLecternTop, column + left, cells, shown, drawn));
                left += cells;
            }
        }

        if (height > 1)
        {
            string ellipsis = glyphs.GetValueOrDefault("Ellipsis") is [string trail, ..] ? trail : "~";
            string shown = measure.BCanvasWidthShorten(
                _bLecternMention.BLecternMentionRead(glyphs)
                    ?? _bLecternNote.BLecternNoteRead(_bLecternLit)
                    ?? _bLecternHeader.BLecternFootRead(_bLecternLit),
                width,
                ellipsis);
            texts.Add(new BCanvasText(row + height - 1, column, measure.BCanvasWidthResolve(shown), shown, "muted"));
        }

        return texts;
    }

    private void BLecternObserve(ConsoleKeyInfo key)
    {
        int page = _bLecternPage;
        switch (key.Key)
        {
            case ConsoleKey.UpArrow or ConsoleKey.DownArrow:
                BLecternLitMove(key.Key == ConsoleKey.UpArrow ? -1 : 1);
                break;
            default:
                BLecternScroll(key.Key switch
                {
                    ConsoleKey.PageUp => _bLecternTop - page,
                    ConsoleKey.PageDown => _bLecternTop + page,
                    ConsoleKey.Home => 0,
                    _ => _bLecternFrame.Count,
                });
                _bLecternLit = BLecternItemsRead()
                    .Where(found => BLecternShownCheck(found.Item2))
                    .Select(static found => found.Item1)
                    .FirstOrDefault();
                break;
        }

        _bLecternChanged();
    }

    private void BLecternMeaningObserve(ConsoleKeyInfo key)
    {
        BLecternLineShow(_bLecternMeaning[key.Key - ConsoleKey.D1]);
        _bLecternChanged();
    }

    private void BLecternLineShow(int line)
    {
        BLecternScroll(line);
        _bLecternLit = BLecternItemsRead()
            .Where(found => found.Item2 >= line)
            .Select(static found => found.Item1)
            .FirstOrDefault();
    }

    private void BLecternCardRefine()
    {
        _bLecternCard.BLecternCardRefine();
        _bLecternChanged();
    }

    private void BLecternGraspMove(ConsoleKeyInfo key)
    {
        _bLecternHeader.BLecternGraspMove(key.Key == ConsoleKey.LeftArrow ? -1 : 1);
        _bLecternChanged();
    }

    internal IReadOnlyList<BLecternLine> BLecternPageBuild(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs, int width, BCanvasWidth measure)
    {
        Dictionary<(CCompassPart, int?), int> places = [];
        List<int> meanings = [];
        List<IReadOnlyDictionary<(CCompassPart, int?), int>> cards = [];
        List<BLecternLine> page =
        [
            .. _bLecternHeader.BLecternHeaderBuild(glyphs, width, measure),
            .. _bLecternHeader.BLecternGlyphBuild(),
        ];
        Func<CCompassPart, Action> card = part => () =>
            cards.Add(_bLecternCard.BLecternCardDraw(part, glyphs, width, measure, page, meanings));
        Dictionary<CCompassPart, Action> parts = new()
        {
            [CCompassPart.CCompassPartMeaning] = card(CCompassPart.CCompassPartMeaning),
            [CCompassPart.CCompassPartCollocation] = card(CCompassPart.CCompassPartCollocation),
            [CCompassPart.CCompassPartIncoming] = () => page.AddRange(_bLecternIncoming.BLecternIncomingDraw(glyphs)),
            [CCompassPart.CCompassPartEtymology] = () =>
                page.AddRange(_bLecternEtymology.BLecternEtymologyDraw(glyphs, width, measure)),
            [CCompassPart.CCompassPartNote] = () => page.AddRange(
                _bLecternNote.BLecternNoteDraw(_bLecternDisplay.CDisplayShown, glyphs, width, measure)),
            [CCompassPart.CCompassPartStamp] = () => page.AddRange(_bLecternHeader.BLecternStampBuild()),
        };
        _bLecternCard.BLecternCardClear();
        foreach (CCompassPart part in _bLecternDisplay.CDisplayCompass.CCompassShown)
        {
            if (parts.GetValueOrDefault(part) is Action draw)
            {
                places[(part, null)] = page.Count;
                draw();
            }
        }

        List<int> starts = [];
        List<BLecternLine> frame = [];
        foreach (BLecternLine line in page)
        {
            starts.Add(frame.Count);
            frame.AddRange(line.BLecternLineDivide(width, measure));
        }

        foreach (((CCompassPart, int?) place, int at) in cards.SelectMany(static map => map))
        {
            places[place] = at;
        }

        _bLecternPart.Clear();
        foreach (((CCompassPart, int?) place, int at) in places)
        {
            _bLecternPart.Add(place, at < starts.Count ? starts[at] : frame.Count);
        }

        _bLecternMeaning.Clear();
        _bLecternMeaning.AddRange(meanings.Select(at => starts[at]));
        return frame;
    }

    private List<(string, int)> BLecternItemsRead()
    {
        List<(string, int)> items = [];
        for (int at = 0; at < _bLecternFrame.Count; at++)
        {
            foreach ((_, _, string? item) in _bLecternFrame[at].BLecternLinePieces)
            {
                if (item is not null && items.All(found => found.Item1 != item))
                {
                    items.Add((item, at));
                }
            }
        }

        return items;
    }

    private bool BLecternShownCheck(int line)
    {
        return line >= _bLecternTop && line < _bLecternTop + _bLecternPage;
    }

    private void BLecternLitMove(int step)
    {
        List<(string, int)> items = BLecternItemsRead();
        int index = items.FindIndex(found => found.Item1 == _bLecternLit);
        int next = index >= 0 && BLecternShownCheck(items[index].Item2)
            ? index + step
            : step > 0
                ? items.FindIndex(found => found.Item2 >= _bLecternTop)
                : items.FindLastIndex(found => found.Item2 < _bLecternTop + _bLecternPage);
        if (next >= 0 && next < items.Count && BLecternShownCheck(items[next].Item2))
        {
            _bLecternLit = items[next].Item1;
            return;
        }

        BLecternScroll(_bLecternTop + step);
    }

    private void BLecternScroll(int top)
    {
        _bLecternTop = Math.Clamp(top, 0, Math.Max(0, _bLecternFrame.Count - _bLecternPage));
    }
}

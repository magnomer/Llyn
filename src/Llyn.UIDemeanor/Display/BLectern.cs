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

    private readonly Action _bLecternChanged;

    private readonly List<int> _bLecternMeaning = [];

    private IReadOnlyList<BLecternLine> _bLecternFrame = [];

    private int _bLecternPage = 1;

    private int _bLecternTop;

    private string? _bLecternLit;

    private (int, int)? _bLecternAnchor;

    internal BLectern(
        BLocalizationCatalog catalog,
        CDisplay display,
        CNavigation navigation,
        Action changed,
        Func<bool> focused,
        Func<BSParley, Task?, int?> docket,
        BBlueprintPart parley,
        Func<BCanvasWidth> measure)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(display);
        ArgumentNullException.ThrowIfNull(navigation);
        ArgumentNullException.ThrowIfNull(changed);
        ArgumentNullException.ThrowIfNull(focused);

        _bLecternCatalog = catalog;
        _bLecternDisplay = display;
        _bLecternChanged = changed;
        _bLecternHeader = new BLecternHeader(catalog, display);
        _bLecternMention = new BLecternMention(
            catalog, display, navigation, () => _bLecternLit, () => _bLecternAnchor, focused, changed, docket, parley,
            measure);
        _bLecternCard = new BLecternCard(catalog, display, _bLecternMention);
        _bLecternIncoming = new BLecternIncoming(catalog, display.CDisplayCard, navigation);
        _bLecternEtymology = new BLecternEtymology(catalog, display, changed);

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
        const string press = "Window.Help.Press";
        BLecternKeymap = new BKeymap(
        [
            new(ConsoleKey.Enter, 0, lectern, "Favorite.Mark", BLecternFavoriteToggle, heart),
            new(ConsoleKey.Spacebar, 0, lectern, "Card.Fold", follow(_bLecternCard.BLecternFoldToggle), fold),
            new(ConsoleKey.Enter, 0, lectern, "Card.Fold", follow(_bLecternCard.BLecternFoldToggle), fold),
            new(ConsoleKey.Enter, 0, lectern, press, follow(_bLecternCard.BLecternSeeOpen),
                owned(_bLecternCard.BLecternSeeCheck)),
            new(ConsoleKey.Enter, 0, lectern, press, follow(_bLecternCard.BLecternChipOpen),
                owned(_bLecternCard.BLecternChipCheck)),
            new(ConsoleKey.Enter, 0, lectern, press, follow(_bLecternHeader.BLecternGlyphOpen),
                owned(_bLecternHeader.BLecternGlyphCheck)),
            new(ConsoleKey.Enter, 0, lectern, press, follow(_bLecternIncoming.BLecternIncomingOpen),
                owned(_bLecternIncoming.BLecternIncomingCheck)),
            new(ConsoleKey.Enter, 0, lectern, press, follow(_bLecternEtymology.BLecternEtymologyOpen),
                owned(_bLecternEtymology.BLecternEtymologyCheck)),
            .. _bLecternMention.BLecternMentionKeymap.BKeymapRead(lectern),
            .. Enumerable.Range(0, 9).Select(index => new BKeymapRow(
                ConsoleKey.D1 + index,
                0,
                lectern,
                "Display.Meaning",
                BLecternMeaningObserve,
                held(() => index < _bLecternMeaning.Count))),
            new(ConsoleKey.LeftArrow, 0, lectern, "Grasp.Mark", BLecternGraspMove, stars),
            new(ConsoleKey.RightArrow, 0, lectern, "Grasp.Mark", BLecternGraspMove, stars),
            new(ConsoleKey.UpArrow, 0, lectern, "Window.Help.Item", BLecternObserve, read),
            new(ConsoleKey.DownArrow, 0, lectern, "Window.Help.Item", BLecternObserve, read),
            new(ConsoleKey.PageUp, 0, lectern, "Window.Help.Scroll", BLecternObserve, read),
            new(ConsoleKey.PageDown, 0, lectern, "Window.Help.Scroll", BLecternObserve, read),
            new(ConsoleKey.Home, 0, lectern, "Window.Help.Scroll", BLecternObserve, read),
            new(ConsoleKey.End, 0, lectern, "Window.Help.Scroll", BLecternObserve, read),
        ]);

        display.CDisplayOpened += BLecternFavoriteRefine;
        display.CDisplayOpened += BLecternGraspRefine;
        display.CDisplayOpened += BLecternFrequencyRefine;
        display.CDisplayClosed += BLecternFavoriteRefine;
        display.CDisplayClosed += BLecternGraspRefine;
        display.CDisplayClosed += BLecternFrequencyRefine;
        display.CDisplayFrequencyChanged += BLecternFrequencyRefine;
        display.CDisplayGrasp.CDisplayFavoriteChanged += BLecternFavoriteRefine;
        display.CDisplayGrasp.CDisplayGraspChanged += BLecternGraspRefine;

        display.CDisplayOpened += BLecternCardRefine;
        display.CDisplayOpened += BLecternIncomingRefine;
        display.CDisplayOpened += BLecternGlyphRefine;
        display.CDisplayClosed += BLecternCardRefine;
        display.CDisplayClosed += BLecternIncomingRefine;
        display.CDisplayClosed += BLecternGlyphRefine;
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
        BLecternScroll(_bLecternTop);
        if (_bLecternLit is string kept && BLecternItemsRead().All(found => found.Item1 != kept))
        {
            _bLecternLit = null;
        }

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
                _bLecternMention.BLecternMentionRead(glyphs) ?? _bLecternHeader.BLecternFootRead(_bLecternLit),
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
        int line = _bLecternMeaning[key.Key - ConsoleKey.D1];
        BLecternScroll(line);
        _bLecternLit = _bLecternFrame[line].BLecternLinePieces
            .Select(static piece => piece.BLecternLineItem)
            .FirstOrDefault(static item => item is not null);
        _bLecternChanged();
    }

    private void BLecternCardRefine()
    {
        _bLecternCard.BLecternCardRefine();
        _bLecternChanged();
    }

    private void BLecternIncomingRefine()
    {
        _bLecternIncoming.BLecternIncomingRefine();
        _bLecternChanged();
    }

    private void BLecternGlyphRefine()
    {
        _bLecternHeader.BLecternGlyphRefine();
        _bLecternChanged();
    }

    private void BLecternFavoriteRefine(CBulletin bulletin)
    {
        BLecternFavoriteRefine();
    }

    private void BLecternGraspRefine(CBulletin bulletin)
    {
        BLecternGraspRefine();
    }

    private void BLecternFrequencyRefine(CBulletin bulletin)
    {
        BLecternFrequencyRefine();
    }

    private void BLecternFavoriteRefine()
    {
        _bLecternHeader.BLecternFavoriteRefine();
        _bLecternChanged();
    }

    private void BLecternGraspRefine()
    {
        _bLecternHeader.BLecternGraspRefine();
        _bLecternChanged();
    }

    private void BLecternFrequencyRefine()
    {
        _bLecternHeader.BLecternFrequencyRefine();
        _bLecternChanged();
    }

    private void BLecternGraspMove(ConsoleKeyInfo key)
    {
        _bLecternHeader.BLecternGraspMove(key.Key == ConsoleKey.LeftArrow ? -1 : 1);
        _bLecternChanged();
    }

    private IReadOnlyList<BLecternLine> BLecternPageBuild(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs, int width, BCanvasWidth measure)
    {
        List<int> meanings = [];
        List<BLecternLine> page =
        [
            .. _bLecternHeader.BLecternHeaderBuild(glyphs, width, measure),
            .. _bLecternHeader.BLecternGlyphBuild(),
        ];
        _bLecternCard.BLecternCardDraw(glyphs, width, measure, page, meanings);
        page.AddRange(_bLecternIncoming.BLecternIncomingDraw(glyphs));
        page.AddRange(_bLecternEtymology.BLecternEtymologyDraw(glyphs, width, measure));
        page.AddRange(_bLecternHeader.BLecternStampBuild());
        List<int> starts = [];
        List<BLecternLine> frame = [];
        foreach (BLecternLine line in page)
        {
            starts.Add(frame.Count);
            frame.AddRange(line.BLecternLineDivide(width, measure));
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

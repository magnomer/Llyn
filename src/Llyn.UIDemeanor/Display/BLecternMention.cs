using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BLecternMention
{
    private readonly BLocalizationCatalog _bLecternMentionCatalog;

    private readonly CDisplayRoute _bLecternMentionRoute;

    private readonly CNavigation _bLecternMentionNavigation;

    private readonly Func<string?> _bLecternMentionLit;

    private readonly Func<(int, int)?> _bLecternMentionAnchor;

    private readonly Action _bLecternMentionChanged;

    private readonly Func<BSParley, Task?, int?> _bLecternMentionDocket;

    private readonly BBlueprintPart _bLecternMentionParley;

    private readonly Func<BCanvasWidth> _bLecternMentionMeasure;

    private readonly Dictionary<string, BExampleItem> _bLecternMentionLine = new(StringComparer.Ordinal);

    private string? _bLecternMentionItem;

    internal BLecternMention(
        BLocalizationCatalog catalog,
        CDisplay display,
        CNavigation navigation,
        Func<string?> lit,
        Func<(int, int)?> anchor,
        Func<bool> focused,
        Action changed,
        Func<BSParley, Task?, int?> docket,
        BBlueprintPart parley,
        Func<BCanvasWidth> measure)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(display);
        ArgumentNullException.ThrowIfNull(navigation);
        ArgumentNullException.ThrowIfNull(lit);
        ArgumentNullException.ThrowIfNull(anchor);
        ArgumentNullException.ThrowIfNull(focused);
        ArgumentNullException.ThrowIfNull(changed);
        ArgumentNullException.ThrowIfNull(docket);
        ArgumentNullException.ThrowIfNull(parley);
        ArgumentNullException.ThrowIfNull(measure);

        _bLecternMentionCatalog = catalog;
        _bLecternMentionRoute = display.CDisplayRoute;
        _bLecternMentionNavigation = navigation;
        _bLecternMentionLit = lit;
        _bLecternMentionAnchor = anchor;
        _bLecternMentionChanged = changed;
        _bLecternMentionDocket = docket;
        _bLecternMentionParley = parley;
        _bLecternMentionMeasure = measure;

        const BKeymapFocus lectern = BKeymapFocus.BKeymapFocusLectern;
        Func<BKeymapLive> idle = () =>
            focused() && lit() is string item && _bLecternMentionLine.ContainsKey(item) && !BLecternMentionCheck()
                ? BKeymapLive.BKeymapLiveShown
                : BKeymapLive.BKeymapLiveHidden;
        Func<BKeymapLive> active = () =>
            focused() && BLecternMentionCheck() ? BKeymapLive.BKeymapLiveShown : BKeymapLive.BKeymapLiveHidden;
        const ConsoleModifiers control = ConsoleModifiers.Control;
        BLecternMentionKeymap = new BKeymap(
        [
            new(ConsoleKey.Enter, 0, lectern, "Display.WordStart", BLecternMentionStart, idle),
            new(ConsoleKey.Enter, 0, lectern, "Display.WordFind", BLecternMentionFind, active),
            new(ConsoleKey.LeftArrow, 0, lectern, "Display.WordMove", BLecternMentionMove, active),
            new(ConsoleKey.RightArrow, 0, lectern, "Display.WordMove", BLecternMentionMove, active),
            new(ConsoleKey.LeftArrow, control, lectern, "Display.WordSeek", BLecternMentionSeek, active),
            new(ConsoleKey.RightArrow, control, lectern, "Display.WordSeek", BLecternMentionSeek, active),
            new(ConsoleKey.Escape, 0, lectern, "Display.WordStop", BLecternMentionStop, active),
        ]);

        display.CDisplayOpened += BLecternMentionReset;
        display.CDisplayClosed += BLecternMentionReset;
    }

    internal BKeymap BLecternMentionKeymap { get; }

    internal void BLecternMentionReset()
    {
        _bLecternMentionItem = null;
    }

    internal void BLecternMentionClear()
    {
        _bLecternMentionLine.Clear();
        _bLecternMentionItem = _bLecternMentionItem == _bLecternMentionLit() ? _bLecternMentionItem : null;
    }

    internal IReadOnlyList<(string, string, string?)> BLecternMentionDraw(CLeafLine sentence)
    {
        ArgumentNullException.ThrowIfNull(sentence);

        string item = "Example" + _bLecternMentionLine.Count.ToString(CultureInfo.InvariantCulture);
        _bLecternMentionLine.Add(item, new BExampleItem(sentence.CLeafLineSentence));
        bool cursor = _bLecternMentionItem == item;
        int offset = cursor ? _bLecternMentionRoute.CDisplayWordUnit : -1;
        int end = cursor ? offset + _bLecternMentionRoute.CDisplayWordLength : -1;
        List<(string, string, string?)> pieces = [];
        int at = 0;
        foreach (CMentionPiece piece in sentence.CLeafLinePiece)
        {
            string role = piece.CMentionPieceLinked switch { true => "link", false => "muted", null => "text" };
            string text = piece.CMentionPieceText;
            int start = 0;
            for (int index = 1; index <= text.Length; index++)
            {
                int place = at + index;
                if (index == text.Length || (text[index] != ' ' && text[index - 1] == ' ')
                    || place == offset || place == end)
                {
                    pieces.Add((text[start..index], role, !cursor || at + start == offset ? item : null));
                    start = index;
                }
            }

            at += text.Length;
        }

        return pieces;
    }

    internal string? BLecternMentionRead(IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs)
    {
        ArgumentNullException.ThrowIfNull(glyphs);

        return BLecternMentionCheck()
            ? string.Format(
                CultureInfo.CurrentCulture,
                _bLecternMentionCatalog.BLocalizationTextRead("Display.WordCursor"),
                BLecternHeader.BLecternGlyphRead(glyphs, "Arrow", 1, "<-"),
                BLecternHeader.BLecternGlyphRead(glyphs, "Arrow", 0, "->"))
            : null;
    }

    private bool BLecternMentionCheck()
    {
        return _bLecternMentionItem is string item && item == _bLecternMentionLit()
            && _bLecternMentionLine.ContainsKey(item);
    }

    private void BLecternMentionStart(ConsoleKeyInfo key)
    {
        string item = _bLecternMentionLit() ?? string.Empty;
        BExampleItem line = _bLecternMentionLine[item];
        _bLecternMentionRoute.CDisplayWordStart(line.BExampleItemSentence);
        _bLecternMentionItem = item;
        _bLecternMentionChanged();
    }

    private void BLecternMentionStop(ConsoleKeyInfo key)
    {
        BLecternMentionReset();
        _bLecternMentionChanged();
    }

    private void BLecternMentionMove(ConsoleKeyInfo key)
    {
        _bLecternMentionRoute.CDisplayWordMove(key.Key == ConsoleKey.LeftArrow ? -1 : 1);
        _bLecternMentionChanged();
    }

    private void BLecternMentionSeek(ConsoleKeyInfo key)
    {
        _bLecternMentionRoute.CDisplayWordSeek(key.Key == ConsoleKey.LeftArrow ? -1 : 1);
        _bLecternMentionChanged();
    }

    private void BLecternMentionFind(ConsoleKeyInfo key)
    {
        (int row, int column) = _bLecternMentionAnchor() ?? (0, 0);
        CMentionOffer? offer = _bLecternMentionRoute.CDisplayWordFind();
        BCanvasWidth measure = _bLecternMentionMeasure();
        int lead = measure.BCanvasWidthResolve(_bLecternMentionRoute.CDisplayWordLead);
        _bLecternMentionChanged();
        BSMentionMenu.BSMentionMenuShow(
            _bLecternMentionCatalog,
            _bLecternMentionParley,
            _bLecternMentionDocket,
            _bLecternMentionNavigation,
            offer,
            (row + 1, Math.Max(column - lead, 0)),
            measure);
        _bLecternMentionChanged();
    }

    private sealed record BExampleItem(long BExampleItemSentence);
}

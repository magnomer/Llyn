using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BSMentionMenu : BSParleyBody
{
    private const int BSMentionMenuStep = 2;

    private readonly IReadOnlyList<(CTranslationTarget, int)> _bsMentionMenuRows;

    private readonly TaskCompletionSource _bsMentionMenuChoice = new();

    private readonly int _bsMentionMenuColumn;

    private int _bsMentionMenuLit;

    internal BSMentionMenu(
        IReadOnlyList<CTranslationTarget> entries, string title, CNavigation navigation, BCanvasWidth measure)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(title);
        ArgumentNullException.ThrowIfNull(navigation);
        ArgumentNullException.ThrowIfNull(measure);

        _bsMentionMenuRows = [.. entries.Select(static entry => (entry, 0))];
        _bsMentionMenuColumn = _bsMentionMenuRows
            .Select(row => (row.Item2 * BSMentionMenuStep)
                + measure.BCanvasWidthResolve(row.Item1.CTranslationTargetHeadword))
            .DefaultIfEmpty(0)
            .Max();
        int widest = _bsMentionMenuRows
            .Select(row => _bsMentionMenuColumn + 2 + measure.BCanvasWidthResolve(row.Item1.CTranslationTargetLanguage))
            .DefaultIfEmpty(0)
            .Max();
        BSParleyBodyWidth = Math.Max(widest, measure.BCanvasWidthResolve(title)) + 6;
        int count = _bsMentionMenuRows.Count;
        if (count == 0)
        {
            _bsMentionMenuChoice.TrySetResult();
        }

        const BKeymapFocus focus = BKeymapFocus.BKeymapFocusParley;
        Func<BKeymapLive> live = () => count > 0 ? BKeymapLive.BKeymapLiveShown : BKeymapLive.BKeymapLiveHidden;
        Action<int> move = step => _bsMentionMenuLit = (_bsMentionMenuLit + step + count) % Math.Max(count, 1);
        Action<ConsoleKeyInfo> open = key =>
        {
            CTranslationTarget entry = _bsMentionMenuRows[_bsMentionMenuLit].Item1;
            _bsMentionMenuChoice.TrySetResult();
            _ = navigation.CNavigationEntryOpen(entry.CTranslationTargetId);
        };
        BSParleyBodyKeymap = new BKeymap(
        [
            new(ConsoleKey.UpArrow, 0, focus, "Window.Help.Item", _ => move(-1), live),
            new(ConsoleKey.DownArrow, 0, focus, "Window.Help.Item", _ => move(1), live),
            new(ConsoleKey.Enter, 0, focus, "Mention.Open", open, live),
        ]);
    }

    public int BSParleyBodyWidth { get; }

    public int BSParleyBodyHeight => _bsMentionMenuRows.Count;

    public string BSParleyBodyPosition => string.Empty;

    public BKeymap BSParleyBodyKeymap { get; }

    internal Task BSMentionMenuChoice => _bsMentionMenuChoice.Task;

    internal static void BSMentionMenuShow(
        BLocalizationCatalog catalog,
        BBlueprintPart blueprint,
        Func<BSParley, Task?, int?> docket,
        CNavigation navigation,
        CMentionOffer? offer,
        (int, int) anchor,
        BCanvasWidth measure)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(blueprint);
        ArgumentNullException.ThrowIfNull(docket);
        ArgumentNullException.ThrowIfNull(navigation);
        ArgumentNullException.ThrowIfNull(measure);

        if (offer is null || offer.CMentionOfferEntry.Count == 0)
        {
            return;
        }

        string title = catalog.BLocalizationTextRead(offer.CMentionOfferKey);
        BSMentionMenu body = new(offer.CMentionOfferEntry, title, navigation, measure);
        BSParley parley = new(blueprint, title, "dialog", [], [], 0, body) { BSParleyAnchor = anchor };
        navigation.CNavigationArrived += body.BSMentionMenuClose;
        try
        {
            _ = docket(parley, body.BSMentionMenuChoice);
        }
        finally
        {
            navigation.CNavigationArrived -= body.BSMentionMenuClose;
        }
    }

    public bool BSParleyBodyObserve(ConsoleKeyInfo key)
    {
        if (BSParleyBodyKeymap.BKeymapFind(BKeymapFocus.BKeymapFocusParley, key) is not BKeymapRow row)
        {
            return false;
        }

        row.BKeymapRowAction(key);
        return true;
    }

    public IEnumerable<BCanvasText> BSParleyBodyDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        int row,
        int column,
        int width,
        int height,
        bool focused,
        string role,
        BCanvasWidth measure)
    {
        ArgumentNullException.ThrowIfNull(glyphs);
        ArgumentNullException.ThrowIfNull(measure);

        int top = Math.Max(_bsMentionMenuLit - height + 1, 0);
        for (int line = 0; line < height && top + line < _bsMentionMenuRows.Count; line++)
        {
            (CTranslationTarget entry, int depth) = _bsMentionMenuRows[top + line];
            string name = new string(' ', depth * BSMentionMenuStep) + entry.CTranslationTargetHeadword;
            int pad = Math.Max(_bsMentionMenuColumn - measure.BCanvasWidthResolve(name), 0);
            string text = $"{name}{new string(' ', pad)}  {entry.CTranslationTargetLanguage}".TrimEnd();
            string shown = measure.BCanvasWidthResolve(text) > width ? measure.BCanvasWidthClamp(text, width) : text;
            bool lit = focused && top + line == _bsMentionMenuLit;
            yield return new BCanvasText(row + line, column, width, shown, lit ? "lit" : role);
        }
    }

    private void BSMentionMenuClose()
    {
        _bsMentionMenuChoice.TrySetResult();
    }
}

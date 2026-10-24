using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BSChoiceFilter : BSParleyBody
{
    private readonly IReadOnlyList<string> _bsChoiceFilterLanguages;
    private readonly bool[] _bsChoiceFilterApplied;
    private int _bsChoiceFilterLit;

    internal BSChoiceFilter(
        IReadOnlyList<string> languages,
        CCatalogFilter filter,
        Action<IReadOnlyList<string>> hide,
        BCanvasWidth measure)
    {
        ArgumentNullException.ThrowIfNull(languages);
        ArgumentNullException.ThrowIfNull(filter);
        ArgumentNullException.ThrowIfNull(hide);
        ArgumentNullException.ThrowIfNull(measure);

        _bsChoiceFilterLanguages = languages;
        bool[] applied = [.. languages.Select(language => filter.CCatalogFilterMatch(language))];
        _bsChoiceFilterApplied = applied;
        int count = languages.Count;
        BSParleyBodyWidth = languages.Select(language => measure.BCanvasWidthResolve(language))
            .DefaultIfEmpty(0)
            .Max() + 10;
        const BKeymapFocus focus = BKeymapFocus.BKeymapFocusParley;
        Func<BKeymapLive> live = () => count > 0 ? BKeymapLive.BKeymapLiveShown : BKeymapLive.BKeymapLiveHidden;
        Action<int> move = step =>
            _bsChoiceFilterLit = Math.Clamp(_bsChoiceFilterLit + step, 0, Math.Max(count - 1, 0));
        Action<ConsoleKeyInfo> tick = _ =>
        {
            applied[_bsChoiceFilterLit] = !applied[_bsChoiceFilterLit];
            hide([.. languages.Where((_, index) => !applied[index])]);
        };
        BSParleyBodyKeymap = new BKeymap(
        [
            new(ConsoleKey.UpArrow, 0, focus, "Window.Help.Item", _ => move(-1), live),
            new(ConsoleKey.DownArrow, 0, focus, "Window.Help.Item", _ => move(1), live),
            new(ConsoleKey.Spacebar, 0, focus, "Window.Help.Tick", tick, live),
        ]);
    }

    public int BSParleyBodyWidth { get; }

    public int BSParleyBodyHeight => _bsChoiceFilterLanguages.Count;

    public string BSParleyBodyPosition => string.Empty;

    public BKeymap BSParleyBodyKeymap { get; }

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

        string ticked = glyphs.GetValueOrDefault("Check") is [string check, ..] && check.Length > 1 ? check : "[x]";
        string blank =
            $"{ticked[0]}{new string(' ', Math.Max(measure.BCanvasWidthResolve(ticked) - 2, 0))}{ticked[^1]}";
        int top = Math.Max(_bsChoiceFilterLit - height + 1, 0);
        for (int line = 0; line < height && top + line < _bsChoiceFilterLanguages.Count; line++)
        {
            string language = _bsChoiceFilterLanguages[top + line];
            bool lit = focused && top + line == _bsChoiceFilterLit;
            string text = $"{(_bsChoiceFilterApplied[top + line] ? ticked : blank)} {language}";
            yield return new BCanvasText(
                row + line, column, width, measure.BCanvasWidthClamp(text, width), lit ? "lit" : role);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Llyn.UIDemeanor;

internal sealed class BSHelp : BSParleyBody
{
    internal const int BSHelpWidth = 78;
    internal const int BSHelpHeight = 14;
    private const int BSHelpSpan = BSHelpWidth - 6;

    private static readonly IReadOnlyDictionary<ConsoleKey, string> BSHelpKey = new Dictionary<ConsoleKey, string>
    {
        [ConsoleKey.Escape] = "Window.Help.Key.Escape",
        [ConsoleKey.Enter] = "Window.Help.Key.Enter",
        [ConsoleKey.Tab] = "Window.Help.Key.Tab",
        [ConsoleKey.Backspace] = "Window.Help.Key.Backspace",
        [ConsoleKey.UpArrow] = "Window.Help.Key.Up",
        [ConsoleKey.DownArrow] = "Window.Help.Key.Down",
        [ConsoleKey.LeftArrow] = "Window.Help.Key.Left",
        [ConsoleKey.RightArrow] = "Window.Help.Key.Right",
        [ConsoleKey.PageUp] = "Window.Help.Key.PageUp",
        [ConsoleKey.PageDown] = "Window.Help.Key.PageDown",
        [ConsoleKey.Home] = "Window.Help.Key.Home",
        [ConsoleKey.End] = "Window.Help.Key.End",
    };

    private readonly BLocalizationCatalog _bsHelpCatalog;
    private readonly Func<BKeymapFocus, IReadOnlyList<BKeymapRow>> _bsHelpKeys;
    private readonly IReadOnlyList<(
        string BSHelpTitle, string[] BSHelpProse, BKeymapFocus BSHelpFocus, Func<BKeymapRow, bool>? BSHelpFilter)>
        _bsHelpPages =
    [
        ("Window.Help.Page.Everywhere", ["Window.Help.Text.Everywhere"], BKeymapFocus.BKeymapFocusWindow, null),
        (
            "Window.Help.Page.Window",
            ["Window.Help.Text.Legend", "Window.Help.Text.Small", "Window.Help.Text.Inside"],
            BKeymapFocus.BKeymapFocusHelp,
            null),
        ("Window.Help.Page.Box", ["Window.Help.Text.Box"], BKeymapFocus.BKeymapFocusParley, null),
        (
            "Window.Help.Page.Tabs",
            ["Window.Help.Text.Tabs"],
            BKeymapFocus.BKeymapFocusRoof,
            static row => row.BKeymapRowModifiers == ConsoleModifiers.Alt),
    ];

    private readonly BPager _bsHelpPager;
    private readonly BCanvasWidth _bsHelpMeasure;

    internal BSHelp(
        BLocalizationCatalog catalog,
        Func<BKeymapFocus, IReadOnlyList<BKeymapRow>> keys,
        BKeymapFocus focus,
        BCanvasWidth measure)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(keys);
        ArgumentNullException.ThrowIfNull(measure);

        _bsHelpCatalog = catalog;
        _bsHelpKeys = keys;
        _bsHelpMeasure = measure;
        _bsHelpPager = new BPager(
            () =>
            [
                .. Enumerable.Range(0, _bsHelpPages.Count).Select(page => (IReadOnlyList<int?>)
                    [.. BSHelpPageBuild(page, _bsHelpMeasure).Select(static line => line.BSHelpTarget)]),
            ],
            focus switch
            {
                BKeymapFocus.BKeymapFocusParley => 2,
                BKeymapFocus.BKeymapFocusHelp => 0,
                _ => 1,
            },
            BSHelpHeight);
    }

    public int BSParleyBodyWidth => BSHelpWidth;

    public int BSParleyBodyHeight => BSHelpHeight;

    public string BSParleyBodyPosition
    {
        get
        {
            int links = BSHelpPageBuild(_bsHelpPager.BPagerPage, _bsHelpMeasure)
                .Count(static line => line.BSHelpTarget is not null);
            return links == 0 ? string.Empty : string.Format(
                CultureInfo.CurrentCulture,
                _bsHelpCatalog.BLocalizationTextRead("Window.Help.Position"),
                _bsHelpPager.BPagerLit + 1,
                links);
        }
    }

    public bool BSParleyBodyObserve(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.F1 && key.Modifiers == 0)
        {
            _bsHelpPager.BPagerShow(0);
            return true;
        }

        return _bsHelpPager.BPagerObserve(key);
    }

    public IEnumerable<BCanvasText> BSParleyBodyDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        int row,
        int column,
        int width,
        bool focused,
        string role,
        BCanvasWidth measure)
    {
        string pointer = glyphs.GetValueOrDefault("History") is [_, string forward] ? forward : ">";
        IReadOnlyList<(string BSHelpText, bool BSHelpDim, int? BSHelpTarget)> lines =
            BSHelpPageBuild(_bsHelpPager.BPagerPage, measure);
        int[] links = [.. Enumerable.Range(0, lines.Count).Where(line => lines[line].BSHelpTarget is not null)];
        int lit = links.Length > 0 ? links[_bsHelpPager.BPagerLit] : -1;
        int top = _bsHelpPager.BPagerTop;
        for (int line = top; line < Math.Min(lines.Count, top + BSHelpHeight); line++)
        {
            (string text, bool dim, int? target) = lines[line];
            string mark = line == lit && focused ? pointer : " ";
            string shown = target is null ? text : $"  {mark} {text}";
            yield return new BCanvasText(row + line - top, column, width, shown, dim ? "disabled" : role);
        }
    }

    private IReadOnlyList<(string BSHelpText, bool BSHelpDim, int? BSHelpTarget)> BSHelpPageBuild(
        int page, BCanvasWidth measure)
    {
        (string title, string[] prose, BKeymapFocus focus, Func<BKeymapRow, bool>? filter) = _bsHelpPages[page];
        IEnumerable<BKeymapRow> own = focus == BKeymapFocus.BKeymapFocusHelp
            ? _bsHelpPager.BPagerKeymap.BKeymapRead(focus)
            : [];
        BKeymapRow[] rows =
        [
            .. own.Concat(_bsHelpKeys(focus))
                .Where(row => filter?.Invoke(row) ?? true)
                .DistinctBy(static row => (row.BKeymapRowKey, row.BKeymapRowModifiers)),
        ];
        (string, int)[] links =
        [
            .. Enumerable.Range(0, _bsHelpPages.Count)
                .Where(other => other != page)
                .Select(other => (_bsHelpPages[other].BSHelpTitle, other)),
        ];
        Func<string, int, string> pad = (text, cells) =>
            text + new string(' ', Math.Max(cells - measure.BCanvasWidthResolve(text), 0));
        List<(string BSHelpText, bool BSHelpDim, int? BSHelpTarget)> lines =
            [(_bsHelpCatalog.BLocalizationTextRead(title), false, null)];
        foreach (string key in prose)
        {
            string text = string.Format(
                CultureInfo.CurrentCulture,
                _bsHelpCatalog.BLocalizationTextRead(key),
                BWindow.BWindowWidthMinimum,
                BWindow.BWindowHeightMinimum);
            lines.Add((string.Empty, false, null));
            lines.AddRange(measure.BCanvasWidthDivide(text, BSHelpSpan).Select(static cut => (cut, false, (int?)null)));
        }

        List<(string BSHelpLabel, List<string> BSHelpPressed, bool BSHelpDim)> groups = [];
        foreach (BKeymapRow row in rows)
        {
            BKeymapLive live = row.BKeymapRowLive();
            if (live == BKeymapLive.BKeymapLiveHidden)
            {
                continue;
            }

            string label = _bsHelpCatalog.BLocalizationTextRead(row.BKeymapRowLabel);
            string pressed = BSHelpKeyFormat(row);
            if (groups.FindIndex(group => group.BSHelpLabel == label) is int found and >= 0)
            {
                groups[found].BSHelpPressed.Add(pressed);
            }
            else
            {
                groups.Add((label, [pressed], live == BKeymapLive.BKeymapLiveDim));
            }
        }

        string[] joined = [.. groups.Select(static group => string.Join(", ", group.BSHelpPressed.Distinct()))];
        int wide = Math.Min(
            joined.Select(text => measure.BCanvasWidthResolve(text)).DefaultIfEmpty(0).Max(), BSHelpSpan / 2);
        if (groups.Count > 0)
        {
            lines.Add((string.Empty, false, null));
        }

        for (int index = 0; index < groups.Count; index++)
        {
            IReadOnlyList<string> pressed = measure.BCanvasWidthDivide(joined[index], wide);
            string label = groups[index].BSHelpLabel;
            IReadOnlyList<string> meaning = measure.BCanvasWidthDivide(label, BSHelpSpan - wide - 5);
            for (int cut = 0; cut < Math.Max(pressed.Count, meaning.Count); cut++)
            {
                string left = pad(cut < pressed.Count ? pressed[cut] : string.Empty, wide);
                string right = cut < meaning.Count ? meaning[cut] : string.Empty;
                lines.Add(($"  {left}   {right}".TrimEnd(), groups[index].BSHelpDim, null));
            }
        }

        lines.Add((string.Empty, false, null));
        lines.Add((_bsHelpCatalog.BLocalizationTextRead("Window.Help.Also"), false, null));
        foreach ((string name, int target) in links)
        {
            lines.Add((_bsHelpCatalog.BLocalizationTextRead(name), false, target));
        }

        return lines;
    }

    private string BSHelpKeyFormat(BKeymapRow row)
    {
        List<string> parts = [];
        ConsoleModifiers modifiers = row.BKeymapRowModifiers;
        if (modifiers.HasFlag(ConsoleModifiers.Control))
        {
            parts.Add(_bsHelpCatalog.BLocalizationTextRead("Window.Help.Key.Control"));
        }

        if (modifiers.HasFlag(ConsoleModifiers.Alt))
        {
            parts.Add(_bsHelpCatalog.BLocalizationTextRead("Window.Help.Key.Alt"));
        }

        if (modifiers.HasFlag(ConsoleModifiers.Shift))
        {
            parts.Add(_bsHelpCatalog.BLocalizationTextRead("Window.Help.Key.Shift"));
        }

        parts.Add(BSHelpKey.TryGetValue(row.BKeymapRowKey, out string? name)
            ? _bsHelpCatalog.BLocalizationTextRead(name)
            : row.BKeymapRowKey is >= ConsoleKey.D0 and <= ConsoleKey.D9 ? ((char)row.BKeymapRowKey).ToString()
            : row.BKeymapRowKey.ToString());
        return string.Join('+', parts);
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BMural
{
    private const int BMuralLegendWidth = 8;

    private readonly BLocalizationCatalog _bMuralCatalog;
    private readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyList<string>>> _bMuralGlyphs;
    private readonly IReadOnlyDictionary<string, IReadOnlyList<string>> _bMuralMarks;
    private readonly BNavigation _bMuralTabs;
    private readonly BBlueprintPart _bMuralBlueprint;
    private readonly BBlueprintPart _bMuralEstablishment;
    private readonly BBlueprintPart _bMuralLegend;
    private readonly BBlueprintPart _bMuralVoyage;
    private readonly BNotice _bMuralNotice;

    internal BMural(
        BBlueprint blueprint,
        BBlueprintPart window,
        BLocalizationCatalog catalog,
        IReadOnlyDictionary<string, IReadOnlyList<string>> marks,
        BNavigation tabs)
    {
        ArgumentNullException.ThrowIfNull(blueprint);
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(marks);
        ArgumentNullException.ThrowIfNull(tabs);

        _bMuralCatalog = catalog;
        _bMuralGlyphs = blueprint.BBlueprintGlyphs;
        _bMuralMarks = marks;
        _bMuralTabs = tabs;
        _bMuralBlueprint = window;
        _bMuralEstablishment = BContract.BContractFind(window, "AEstablishment");
        _bMuralLegend = BContract.BContractFind(window, "ALegend");
        _bMuralVoyage = BContract.BContractFind(window, "AVoyage");
        _bMuralNotice = new BNotice(blueprint);
    }

    internal IEnumerable<BCanvasText> BMuralDraw(
        int width,
        int height,
        BKeymapFocus focus,
        BRoof roof,
        CNavigationState? navigation,
        IReadOnlyList<BSParley> parleys,
        Func<BKeymapFocus, ConsoleKeyInfo, BKeymapRow?> find)
    {
        ArgumentNullException.ThrowIfNull(roof);
        ArgumentNullException.ThrowIfNull(parleys);
        ArgumentNullException.ThrowIfNull(find);

        return focus == BKeymapFocus.BKeymapFocusNotice
            ? BMuralNoticeBuild(width, height, find)
            : BMuralFrameBuild(width, height, focus, roof, navigation, parleys, find);
    }

    private IEnumerable<BCanvasText> BMuralFrameBuild(
        int width,
        int height,
        BKeymapFocus focus,
        BRoof roof,
        CNavigationState? navigation,
        IReadOnlyList<BSParley> parleys,
        Func<BKeymapFocus, ConsoleKeyInfo, BKeymapRow?> find)
    {
        BBlueprint.BBlueprintPlace(_bMuralBlueprint, 0, 0, width, height);
        foreach (BCanvasText scaffold in BBlueprint.BBlueprintDraw(_bMuralBlueprint, _bMuralGlyphs))
        {
            yield return scaffold;
        }

        BBlueprintPart establishment = _bMuralEstablishment;
        yield return new BCanvasText(
            establishment.BBlueprintPartRow,
            establishment.BBlueprintPartColumn,
            establishment.BBlueprintPartWidth,
            _bMuralCatalog.BLocalizationTextRead("Window.Opening"),
            establishment.BBlueprintPartTheme);
        BBlueprintPart legend = _bMuralLegend;
        foreach (BCanvasText key in BMuralLegendBuild(
            legend.BBlueprintPartRow, legend.BBlueprintPartColumn, focus, find))
        {
            yield return key;
        }

        foreach (BCanvasText text in roof.BRoofDraw(_bMuralMarks))
        {
            yield return text;
        }

        if (navigation is CNavigationState state)
        {
            BBlueprintPart top = BContract.BContractFind(_bMuralBlueprint, "ARoof");
            foreach (BCanvasText text in _bMuralTabs.BNavigationDraw(
                _bMuralCatalog, state, _bMuralMarks, top, _bMuralVoyage))
            {
                yield return text;
            }
        }

        foreach (BSParley parley in parleys)
        {
            foreach (BCanvasText text in parley.BSParleyDraw(_bMuralGlyphs, width, height))
            {
                yield return text;
            }
        }
    }

    private IEnumerable<BCanvasText> BMuralLegendBuild(
        int row, int column, BKeymapFocus focus, Func<BKeymapFocus, ConsoleKeyInfo, BKeymapRow?> find)
    {
        string theme = _bMuralLegend.BBlueprintPartTheme;
        int packed = column;
        for (ConsoleKey key = ConsoleKey.F1; key <= ConsoleKey.F10; key++)
        {
            BKeymapRow? found = find(focus, new ConsoleKeyInfo('\0', key, false, false, false));
            BKeymapLive live = found?.BKeymapRowLive() ?? BKeymapLive.BKeymapLiveHidden;
            if (found is null || live == BKeymapLive.BKeymapLiveHidden)
            {
                continue;
            }

            int slot = key - ConsoleKey.F1;
            int left = focus == BKeymapFocus.BKeymapFocusNotice ? packed : column + (slot * BMuralLegendWidth);
            packed += BMuralLegendWidth;
            string number = (slot + 1).ToString(CultureInfo.InvariantCulture);
            string label = _bMuralCatalog.BLocalizationTextRead(found.BKeymapRowLabel);
            string role = live == BKeymapLive.BKeymapLiveDim ? "disabled" : $"{theme}-label";
            yield return new BCanvasText(row, left, number.Length, number, $"{theme}-number");
            yield return new BCanvasText(row, left + number.Length, BMuralLegendWidth - number.Length, label, role);
        }
    }

    private IEnumerable<BCanvasText> BMuralNoticeBuild(
        int width, int height, Func<BKeymapFocus, ConsoleKeyInfo, BKeymapRow?> find)
    {
        string notice = string.Format(
            CultureInfo.CurrentCulture,
            _bMuralCatalog.BLocalizationTextRead("Window.Small"),
            BWindow.BWindowWidthMinimum,
            BWindow.BWindowHeightMinimum,
            width,
            height);
        return _bMuralNotice.BNoticeBuild(
            notice,
            width,
            height,
            _bMuralGlyphs,
            BMuralLegendBuild(height - 1, 0, BKeymapFocus.BKeymapFocusNotice, find));
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BMural
{
    private const int BMuralLegendWidth = 8;

    private readonly BLocalizationCatalog _bMuralCatalog;
    private readonly BNavigation _bMuralTabs;
    private readonly BBlueprintPart _bMuralBlueprint;
    private readonly BEstablishment _bMuralEstablishment;
    private readonly BBlueprintPart _bMuralLegend;
    private readonly BBlueprintPart _bMuralVoyage;
    private readonly BNotice _bMuralNotice;

    internal BMural(
        BBlueprint blueprint,
        BBlueprintPart window,
        BLocalizationCatalog catalog,
        BNavigation tabs,
        BEstablishment establishment)
    {
        ArgumentNullException.ThrowIfNull(blueprint);
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(tabs);
        ArgumentNullException.ThrowIfNull(establishment);

        _bMuralCatalog = catalog;
        _bMuralTabs = tabs;
        _bMuralBlueprint = window;
        _bMuralEstablishment = establishment;
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
        CCourierState? courier,
        IReadOnlyList<BSParley> parleys,
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyList<string>>> glyphs,
        IReadOnlyDictionary<string, IReadOnlyList<string>> marks,
        Func<BKeymapFocus, ConsoleKeyInfo, BKeymapRow?> find,
        BCanvasWidth measure)
    {
        ArgumentNullException.ThrowIfNull(roof);
        ArgumentNullException.ThrowIfNull(parleys);
        ArgumentNullException.ThrowIfNull(glyphs);
        ArgumentNullException.ThrowIfNull(marks);
        ArgumentNullException.ThrowIfNull(find);
        ArgumentNullException.ThrowIfNull(measure);

        return focus == BKeymapFocus.BKeymapFocusNotice
            ? BMuralNoticeBuild(width, height, glyphs, find, measure)
            : BMuralFrameBuild(
                width, height, focus, roof, navigation, courier, parleys, glyphs, marks, find, measure);
    }

    private IEnumerable<BCanvasText> BMuralFrameBuild(
        int width,
        int height,
        BKeymapFocus focus,
        BRoof roof,
        CNavigationState? navigation,
        CCourierState? courier,
        IReadOnlyList<BSParley> parleys,
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyList<string>>> glyphs,
        IReadOnlyDictionary<string, IReadOnlyList<string>> marks,
        Func<BKeymapFocus, ConsoleKeyInfo, BKeymapRow?> find,
        BCanvasWidth measure)
    {
        BBlueprint.BBlueprintPlace(_bMuralBlueprint, 0, 0, width, height);
        foreach (BCanvasText scaffold in BBlueprint.BBlueprintDraw(_bMuralBlueprint, glyphs))
        {
            yield return scaffold;
        }

        foreach (BCanvasText status in _bMuralEstablishment.BEstablishmentDraw(courier, marks, measure))
        {
            yield return status;
        }

        BBlueprintPart legend = _bMuralLegend;
        foreach (BCanvasText key in BMuralLegendBuild(
            legend.BBlueprintPartRow, legend.BBlueprintPartColumn, focus, find))
        {
            yield return key;
        }

        foreach (BCanvasText text in roof.BRoofDraw(marks, measure))
        {
            yield return text;
        }

        if (navigation is CNavigationState state)
        {
            BBlueprintPart top = BContract.BContractFind(_bMuralBlueprint, "ARoof");
            foreach (BCanvasText text in _bMuralTabs.BNavigationDraw(
                _bMuralCatalog, state, marks, top, _bMuralVoyage, measure))
            {
                yield return text;
            }
        }

        foreach (BSParley parley in parleys)
        {
            foreach (BCanvasText text in parley.BSParleyDraw(glyphs, width, height, measure))
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
        int width,
        int height,
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyList<string>>> glyphs,
        Func<BKeymapFocus, ConsoleKeyInfo, BKeymapRow?> find,
        BCanvasWidth measure)
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
            glyphs,
            BMuralLegendBuild(height - 1, 0, BKeymapFocus.BKeymapFocusNotice, find),
            measure);
    }
}

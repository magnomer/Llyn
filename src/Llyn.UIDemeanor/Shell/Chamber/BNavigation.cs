using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BNavigation
{
    private readonly CAtelier? _bNavigationAtelier;
    private readonly IReadOnlyList<IReadOnlyList<KeyValuePair<string, string>>> _bNavigationTabs;

    internal BNavigation(BBlueprint blueprint, CAtelier? atelier)
    {
        ArgumentNullException.ThrowIfNull(blueprint);

        Dictionary<string, KeyValuePair<string, string>> tabs = new(StringComparer.Ordinal)
        {
            ["ANavigationInput"] = new("Input", "Navigation.Input"),
            ["ANavigationLibrary"] = new("Library", "Navigation.Library"),
            ["ANavigationPhonology"] = new("Phonology", "Navigation.Phonology"),
            ["ANavigationXiesheng"] = new("Xiesheng", "Navigation.Xiesheng"),
            ["ANavigationYunjing"] = new("Yunjing", "Navigation.Yunjing"),
            ["ANavigationTaxonomy"] = new("Taxonomy", "Navigation.Taxonomy"),
            ["ANavigationTenor"] = new("Tenor", "Navigation.Tenor"),
            ["ANavigationRepertoire"] = new("Repertoire", "Navigation.Repertoire"),
            ["ANavigationCorpus"] = new("Corpus", "Navigation.Corpus"),
            ["ANavigationSource"] = new("Reference", "Navigation.Source"),
            ["ANavigationGuild"] = new("Guild", "Navigation.Guild"),
            ["ANavigationStudy"] = new("Study", "Navigation.Study"),
            ["ANavigationDeck"] = new("Deck", "Navigation.Deck"),
            ["ANavigationScoreboard"] = new("Scoreboard", "Navigation.Scoreboard"),
            ["ANavigationFavorite"] = new("Favorite", "Navigation.Favorite"),
            ["ANavigationDuplex"] = new("Duplex", "Navigation.DualPanel"),
            ["ANavigationSettings"] = new("Settings", "Navigation.Settings"),
        };
        BBlueprintPart rail = BContract.BContractFind(
            BContract.BContractBlueprintFind(blueprint, "ASNavigation"), "ANavigation");
        _bNavigationAtelier = atelier;
        _bNavigationTabs = [.. rail.BBlueprintPartChildren.Select(group => (IReadOnlyList<KeyValuePair<string, string>>)
            [.. group.BBlueprintPartChildren.Select(part => tabs[part.BBlueprintPartName ?? string.Empty])])];
    }

    internal IReadOnlyList<IReadOnlyList<KeyValuePair<string, string>>> BNavigationTabs => _bNavigationTabs;

    internal BRoofMenu BNavigationBuild(
        BLocalizationCatalog catalog,
        CNavigationState? state,
        IReadOnlyDictionary<string, IReadOnlyList<string>> marks,
        Action choose,
        BCanvasWidth measure)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(marks);
        ArgumentNullException.ThrowIfNull(choose);
        ArgumentNullException.ThrowIfNull(measure);

        string bullet = marks.GetValueOrDefault("Radio") is [string radio] ? radio : "*";
        Func<string, ConsoleKey?> letter = key => catalog.BLocalizationTextRead(key) is [char single]
            && char.IsAsciiLetter(single)
                ? ConsoleKey.A + (char.ToUpperInvariant(single) - 'A')
                : null;
        List<BRoofItem?> items = [];
        if (_bNavigationAtelier is CAtelier atelier && state is not null)
        {
            CNavigation navigation = atelier.CAtelierNavigation;
            string blank = new(' ', measure.BCanvasWidthResolve(bullet));
            foreach (IReadOnlyList<KeyValuePair<string, string>> group in _bNavigationTabs)
            {
                List<BRoofItem> built = [];
                foreach ((string tab, string label) in group)
                {
                    string mark = tab == state.CNavigationStateTab ? bullet : blank;
                    built.Add(new(
                        $"{mark} {catalog.BLocalizationTextRead(label)}",
                        null,
                        null,
                        () => _ = navigation.CNavigationTabSelect(tab),
                        static () => true));
                }

                BRoofItem[] shown = [.. built.Where((_, index) =>
                    !state.CNavigationStateHidden.Contains(group[index].Key, StringComparer.Ordinal))];
                if (shown.Length > 0 && items.Count > 0)
                {
                    items.Add(null);
                }

                items.AddRange(shown);
            }

            CVoyageState voyage = state.CNavigationStateVoyage;
            items.Add(null);
            items.Add(new(
                catalog.BLocalizationTextRead("Voyage.Retreat"),
                null,
                null,
                BNavigationRetreat,
                () => voyage.CVoyageStatePast));
            items.Add(new(
                catalog.BLocalizationTextRead("Voyage.Advance"),
                null,
                null,
                BNavigationAdvance,
                () => voyage.CVoyageStateFuture));
            items.Add(new(
                catalog.BLocalizationTextRead("Roof.Panel.Choose"),
                null,
                letter("Roof.Panel.Choose.Hotkey"),
                choose,
                static () => true));
        }

        return new BRoofMenu(catalog.BLocalizationTextRead("Roof.Panel"), letter("Roof.Panel.Hotkey"), items);
    }

    internal void BNavigationRetreat()
    {
        _ = _bNavigationAtelier?.CAtelierNavigation.CNavigationStationUndo();
    }

    internal void BNavigationAdvance()
    {
        _ = _bNavigationAtelier?.CAtelierNavigation.CNavigationStationRedo();
    }

    internal IEnumerable<BCanvasText> BNavigationDraw(
        BLocalizationCatalog catalog,
        CNavigationState state,
        IReadOnlyDictionary<string, IReadOnlyList<string>> marks,
        BBlueprintPart roof,
        BBlueprintPart foot,
        BCanvasWidth measure)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(marks);
        ArgumentNullException.ThrowIfNull(roof);
        ArgumentNullException.ThrowIfNull(foot);
        ArgumentNullException.ThrowIfNull(measure);

        string? label = _bNavigationTabs.SelectMany(static group => group)
            .FirstOrDefault(tab => tab.Key == state.CNavigationStateTab).Value;
        string name = label is null ? string.Empty : $" {catalog.BLocalizationTextRead(label)} ";
        int cells = Math.Min(measure.BCanvasWidthResolve(name), roof.BBlueprintPartWidth);
        int end = roof.BBlueprintPartColumn + roof.BBlueprintPartWidth - 1;
        List<BCanvasText> texts = [new(roof.BBlueprintPartRow, end - cells, cells, name, roof.BBlueprintPartTheme)];
        (string retreat, string advance) = marks.GetValueOrDefault("History") is [string earlier, string later]
            ? (earlier, later)
            : ("<", ">");
        CVoyageState voyage = state.CNavigationStateVoyage;
        string back = $"{retreat}{catalog.BLocalizationTextRead("Voyage.Retreat")}";
        string forward = $"{catalog.BLocalizationTextRead("Voyage.Advance")}{advance}";
        int span = Math.Min(measure.BCanvasWidthResolve(forward), foot.BBlueprintPartWidth);
        string theme = foot.BBlueprintPartTheme;
        texts.Add(new(
            foot.BBlueprintPartRow,
            foot.BBlueprintPartColumn,
            foot.BBlueprintPartWidth - span,
            back,
            voyage.CVoyageStatePast ? theme : "disabled"));
        texts.Add(new(
            foot.BBlueprintPartRow,
            foot.BBlueprintPartColumn + foot.BBlueprintPartWidth - span,
            span,
            forward,
            voyage.CVoyageStateFuture ? theme : "disabled"));
        return texts;
    }
}

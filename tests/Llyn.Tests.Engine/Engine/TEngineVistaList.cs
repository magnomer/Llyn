using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineVistaList
{
    [Fact]
    public void FavoriteFind_VistaTwins_NumbersRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry older = TEngineVista.TVistaEntryCreate(engine, "water", "English");
        LEntry newer = TEngineVista.TVistaEntryCreate(engine, "water", "English");
        engine.TEngineFavoriteSave(older.LEntryId);
        engine.TEngineFavoriteSave(newer.LEntryId);

        LVista vista = engine.TEngineVistaStart("favorite", LCatalogOrder.LCatalogOrderHeadword);
        IReadOnlyList<LVistaRow> rows = engine.TEngineFavoriteFind(vista);

        Assert.Equal("water (1)", rows.Single(row => row.LVistaRowId == older.LEntryId).LVistaRowName);
        Assert.Equal("water (2)", rows.Single(row => row.LVistaRowId == newer.LEntryId).LVistaRowName);
    }

    [Fact]
    public void PronunciationFind_VistaTwins_NumbersRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry older = TEngineVista.TVistaEntryCreate(engine, "water", "English");
        LEntry newer = TEngineVista.TVistaEntryCreate(engine, "water", "English");

        LVista vista = engine.TEngineVistaStart("phonology", LCatalogOrder.LCatalogOrderHeadword);
        IReadOnlyList<LCatalogPronunciation> rows = engine.TEnginePronunciationFind(vista);

        Assert.Equal(
            "water (1)",
            rows.Single(row => row.LCatalogPronunciationEntry.LEntryId == older.LEntryId).LCatalogPronunciationName);
        Assert.Equal(
            "water (2)",
            rows.Single(row => row.LCatalogPronunciationEntry.LEntryId == newer.LEntryId).LCatalogPronunciationName);
    }

    [Fact]
    public void PronunciationFind_NoEpithet_CarriesAnEmptyOne()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEngineVista.TVistaEntryCreate(engine, "water", "English");

        LVista vista = engine.TEngineVistaStart("phonology", LCatalogOrder.LCatalogOrderHeadword);

        Assert.Equal(string.Empty, Assert.Single(engine.TEnginePronunciationFind(vista)).LCatalogPronunciationEpithet);
    }

    [Fact]
    public void PronunciationFind_VistaOrder_SortsRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEngineVista.TVistaEntryCreate(engine, "stone", "English");
        TEngineVista.TVistaEntryCreate(engine, "water", "English");

        LVista vista = engine.TEngineVistaStart("phonology", LCatalogOrder.LCatalogOrderReverse);

        Assert.Equal(
            ["water", "stone"],
            engine.TEnginePronunciationFind(vista).Select(row => row.LCatalogPronunciationEntry.LEntryHeadword));
    }

    [Fact]
    public void DiweiFind_VistaUsageOrder_CountsFirst()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate("""{ "language": "Fixture" }""");
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        string language = pack.TLanguageFixtureName;
        TVistaDiweiPlace(engine, workspace, language, "爛", "來");
        TVistaDiweiPlace(engine, workspace, language, "孤", "見");
        TVistaDiweiPlace(engine, workspace, language, "古", "見");

        LVista named = engine.TEngineVistaStart("yunjing", LCatalogOrder.LCatalogOrderName);
        LVista used = engine.TEngineVistaStart("yunmu", LCatalogOrder.LCatalogOrderUsage);
        long? chosen = engine.TEngineDiweiFind(language, LDiwei.LDiweiInitial, "來")?.LDiweiId;

        Assert.Equal(
            ["來", "見"],
            engine.TEngineDiweiFind(named, chosen, false).Select(row => row.LDiweiKey));
        Assert.Equal(
            ["見", "來"],
            engine.TEngineDiweiFind(used, chosen, false).Select(row => row.LDiweiKey));
    }

    [Fact]
    public void TagFind_VistaQuery_MatchesName()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineTagCreate("noun");
        engine.TEngineTagCreate("verb");

        LVista vista = engine.TEngineVistaStart("taxonomy", LCatalogOrder.LCatalogOrderName);
        vista.TVistaQuerySet("no");

        Assert.Equal(["noun"], engine.TEngineTagFind(vista).Select(row => row.LCatalogTagStored.LTagText));
    }

    private static void TVistaDiweiPlace(
        LEngine engine, TWorkspace workspace, string language, string character, string initial)
    {
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            character,
            language,
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a state", [], [], [], [], [], 1)],
            [],
            reflexes: [TInterface.TReflexDraftCreate("Korean", "", "a")]));

        LFanqieArchive fanqie = TInterface.TFanqieArchiveCreate(workspace.TWorkspaceDatabase);
        fanqie.TFanqieSave(language, character, [TInterface.TFanqieRowCreate(character, 0, initial, "寒", "一", "平")]);
        TInterface.TDiweiArchiveCreate(workspace.TWorkspaceDatabase).TDiweiApply(language, character, null);

        IReadOnlyList<long> anchors =
            fanqie.TFanqieRead(language, character).Select(row => row.LFanqieRowId).ToList();
        engine.TEntryAnchorApply(entry.LEntryId, anchors);
    }
}

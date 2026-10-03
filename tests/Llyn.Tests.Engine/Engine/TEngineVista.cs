using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineVista
{
    [Fact]
    public void VistaOrderSet_WorkspaceReopened_KeepsOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            engine.TPostureStart().TPostureVistaStart("library", LCatalogOrder.LCatalogOrderHeadword)
                .TVistaOrderSet(LCatalogOrder.LCatalogOrderReverse);
        }

        using LEngine reopened = workspace.TWorkspaceEngineStart();
        LVista vista = reopened.TPostureStart().TPostureVistaStart("library", LCatalogOrder.LCatalogOrderHeadword);

        Assert.Equal(LCatalogOrder.LCatalogOrderReverse, vista.LVistaOrder);
    }

    [Fact]
    public void VistaFilterSet_WorkspaceReopened_KeepsFilter()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            engine.TPostureStart().TPostureVistaStart("library", LCatalogOrder.LCatalogOrderHeadword)
                .TVistaFilterSet(TInterface.TCatalogFilterCreate("Korean", "French"));
        }

        using LEngine reopened = workspace.TWorkspaceEngineStart();
        LVista vista = reopened.TPostureStart().TPostureVistaStart("library", LCatalogOrder.LCatalogOrderHeadword);

        Assert.Equal(["Korean", "French"], vista.LVistaFilter.LCatalogFilterHidden);
    }

    [Fact]
    public void VistaStart_NoStoredOrder_UsesFallback()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LVista vista = engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderRecent);

        Assert.Equal(LCatalogOrder.LCatalogOrderRecent, vista.LVistaOrder);
        Assert.False(vista.LVistaFilter.LCatalogFilterActive);
        Assert.Equal(string.Empty, vista.LVistaQuery);
        Assert.Null(vista.LVistaChosen);
    }

    [Fact]
    public void EntryFind_VistaFilter_HidesLanguage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TVistaEntryCreate(engine, "water", "English");
        TVistaEntryCreate(engine, "eau", "French");

        LVista vista = engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword);
        vista.TVistaFilterSet(TInterface.TCatalogFilterCreate("French"));

        Assert.Equal(["water"], engine.TEngineEntryFind(vista).Select(row => row.LVistaRowHeadword));
    }

    [Fact]
    public void EntryFind_NoEpithetHeld_CarriesAnEmptyEpithet()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TVistaEntryCreate(engine, "water", "English");

        LVista vista = engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword);

        Assert.Equal(string.Empty, Assert.Single(engine.TEngineEntryFind(vista)).LVistaRowEpithet);
    }

    [Fact]
    public void EntryFind_VistaQuery_MatchesHeadword()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TVistaEntryCreate(engine, "water", "English");
        TVistaEntryCreate(engine, "stone", "English");

        LVista vista = engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword);
        vista.TVistaQuerySet("wat");

        Assert.Equal(["water"], engine.TEngineEntryFind(vista).Select(row => row.LVistaRowHeadword));
    }

    [Fact]
    public void EntryFind_VistaBlank_AnswersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TVistaEntryCreate(engine, "water", "English");

        LVista blank = engine.TEngineVistaStart("left", LCatalogOrder.LCatalogOrderHeadword, true);
        LVista listing = engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword);

        Assert.Empty(engine.TEngineEntryFind(blank));
        Assert.Single(engine.TEngineEntryFind(listing));

        blank.TVistaQuerySet("wat");

        Assert.Single(engine.TEngineEntryFind(blank));
    }

    [Fact]
    public void VistaOrderSet_LeftTab_KeepsRightApart()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LPosture posture = engine.TPostureStart();
        posture.TPostureVistaStart("left", LCatalogOrder.LCatalogOrderHeadword, true)
            .TVistaOrderSet(LCatalogOrder.LCatalogOrderRecent);

        LVista left = posture.TPostureVistaStart("left", LCatalogOrder.LCatalogOrderHeadword, true);
        LVista right = posture.TPostureVistaStart("right", LCatalogOrder.LCatalogOrderHeadword, true);

        Assert.Equal(LCatalogOrder.LCatalogOrderRecent, left.LVistaOrder);
        Assert.Equal(LCatalogOrder.LCatalogOrderHeadword, right.LVistaOrder);
    }

    [Fact]
    public void EntryFind_VistaTwins_NumbersByEntryId()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry older = TVistaEntryCreate(engine, "water", "English");
        LEntry newer = TVistaEntryCreate(engine, "water", "English");
        TVistaEntryCreate(engine, "stone", "English");

        LVista vista = engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderRecent);
        IReadOnlyList<LVistaRow> rows = engine.TEngineEntryFind(vista);

        Assert.Equal("water (2)", rows.Single(row => row.LVistaRowId == newer.LEntryId).LVistaRowName);
        Assert.Equal("water (1)", rows.Single(row => row.LVistaRowId == older.LEntryId).LVistaRowName);
        Assert.Equal("stone", rows.Single(row => row.LVistaRowHeadword == "stone").LVistaRowName);
    }

    [Fact]
    public void FavoriteFind_VistaTwins_NumbersRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry older = TVistaEntryCreate(engine, "water", "English");
        LEntry newer = TVistaEntryCreate(engine, "water", "English");
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
        LEntry older = TVistaEntryCreate(engine, "water", "English");
        LEntry newer = TVistaEntryCreate(engine, "water", "English");

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
        TVistaEntryCreate(engine, "water", "English");

        LVista vista = engine.TEngineVistaStart("phonology", LCatalogOrder.LCatalogOrderHeadword);

        Assert.Equal(string.Empty, Assert.Single(engine.TEnginePronunciationFind(vista)).LCatalogPronunciationEpithet);
    }

    [Fact]
    public void EntryFind_VistaChosen_MarksRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TVistaEntryCreate(engine, "water", "English");
        LEntry chosen = TVistaEntryCreate(engine, "stone", "English");

        LVista vista = engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword);
        vista.TVistaSelect(chosen.LEntryId);

        Assert.Equal(
            [chosen.LEntryId],
            engine.TEngineEntryFind(vista).Where(row => row.LVistaRowChosen).Select(row => row.LVistaRowId));
    }

    [Fact]
    public void PronunciationFind_VistaOrder_SortsRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TVistaEntryCreate(engine, "stone", "English");
        TVistaEntryCreate(engine, "water", "English");

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

    [Fact]
    public void VistaOrderSet_ChangedOrder_RaisesVistaBulletin()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<long> raised = [];
        engine.TEngineObserverAttach(bulletin =>
        {
            if (bulletin.LBulletinSubject == LSubject.LSubjectVista)
            {
                raised.Add(bulletin.LBulletinId);
            }
        });
        LVista vista = engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword);

        vista.TVistaOrderSet(LCatalogOrder.LCatalogOrderRecent);

        Assert.Equal([vista.LVistaId], raised);
    }

    [Fact]
    public void VistaOrderSet_SameOrder_RaisesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<long> raised = [];
        engine.TEngineObserverAttach(bulletin =>
        {
            if (bulletin.LBulletinSubject == LSubject.LSubjectVista)
            {
                raised.Add(bulletin.LBulletinId);
            }
        });
        LVista vista = engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword);

        vista.TVistaOrderSet(LCatalogOrder.LCatalogOrderHeadword);
        vista.TVistaFilterSet(vista.LVistaFilter);
        vista.TVistaQuerySet(string.Empty);
        vista.TVistaSelect(null);

        Assert.Empty(raised);
    }

    private static LEntry TVistaEntryCreate(LEngine engine, string headword, string language)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword,
            language,
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(
                string.Empty,
                string.Empty,
                "a meaning",
                [],
                [],
                [],
                [],
                [],
                1)],
            []));
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

    [Fact]
    public void VistaOrderRead_NoVista_ReadsHeadwordOrder()
    {
        Assert.Equal(LCatalogOrder.LCatalogOrderHeadword, TInterface.TVistaOrderRead(null));
    }

    [Fact]
    public void VistaStoredCheck_OrphanRowOrNone_IsNotStored()
    {
        Assert.False(TInterface.TVistaStoredCheck(null));
        Assert.False(TInterface.TVistaStoredCheck(0));
        Assert.True(TInterface.TVistaStoredCheck(7));
    }

    [Fact]
    public void VistaFilterRead_NoVista_ReadsEmptyFilter()
    {
        Assert.Same(LCatalogFilter.LCatalogFilterEmpty, TInterface.TVistaFilterRead(null));
    }

    [Fact]
    public void VistaFilterSet_NothingHidden_KeepsSharedEmptyFilter()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LVista vista = engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword);

        vista.TVistaFilterSet(["Latin", "Greek"]);
        IReadOnlyList<string> hidden = vista.LVistaFilter.LCatalogFilterHidden;
        vista.TVistaFilterSet([]);

        Assert.Equal(["Latin", "Greek"], hidden);
        Assert.Same(LCatalogFilter.LCatalogFilterEmpty, vista.LVistaFilter);
    }

    [Fact]
    public void VistaUsageRead_CreditedAuthor_CountsItsWorks()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LReference book = engine.TEngineCitationCreate("Book");
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        engine.TRequestCreditApply(book.LReferenceId, ada.LAuthorId, 0);
        LVista vista = engine.TEngineVistaStart("guild", LCatalogOrder.LCatalogOrderName);

        int none = vista.TVistaUsageRead();
        vista.TVistaSelect(ada.LAuthorId);

        Assert.Equal(0, none);
        Assert.Equal(1, vista.TVistaUsageRead());
    }

    [Fact]
    public void VistaUsageRead_ChosenEntry_CountsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", "w", string.Empty, [TInterface.TCardCreate("a liquid", 1)], []));
        LVista vista = engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword);

        vista.TVistaSelect(entry.LEntryId);

        Assert.Equal(0, vista.TVistaUsageRead());
    }
}

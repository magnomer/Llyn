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

    internal static LEntry TVistaEntryCreate(LEngine engine, string headword, string language)
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
}

using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TAtlas
{
    [Fact]
    public void AtlasRowsRead_NoVista_ReadsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TAtlasSituationSave(engine, "at home");
        CAtlas atlas = TAtlasPrepare(engine, atelier);

        Assert.Empty(atlas.CAtlasRowsRead("?", "-"));
        Assert.False(atlas.CAtlasNarrowed);
        Assert.Null(atlas.CAtlasChosen);
    }

    [Fact]
    public void AtlasRowsRead_BlankTitle_NamesTheRowWithTheUntitledWord()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LSituation blank = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, LStateValue.LStateValueUnspecified, null, null));
        LSituation home = TAtlasSituationSave(engine, "at home");
        CAtlas atlas = TAtlasPrepare(engine, atelier);
        atlas.CAtlasVistaRestore(engine.TEngineVistaStart("repertoire", LCatalogOrder.LCatalogOrderName));

        IReadOnlyList<CCatalogSituation> rows = atlas.CAtlasRowsRead("?", "-");

        Assert.Equal("-", rows.Single(row => row.CCatalogSituationId == blank.LSituationId).CCatalogSituationTitle);
        Assert.Equal(
            "at home", rows.Single(row => row.CCatalogSituationId == home.LSituationId).CCatalogSituationTitle);
    }

    [Fact]
    public void AtlasQuerySet_MatchingTitle_NarrowsTheRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TAtlasSituationSave(engine, "at home");
        TAtlasSituationSave(engine, "in court");
        CAtlas atlas = TAtlasPrepare(engine, atelier);
        atlas.CAtlasVistaRestore(engine.TEngineVistaStart("repertoire", LCatalogOrder.LCatalogOrderName));

        atlas.CAtlasQuerySet("court");

        Assert.True(atlas.CAtlasNarrowed);
        Assert.Equal(["in court"], atlas.CAtlasRowsRead("?", "-").Select(row => row.CCatalogSituationTitle));
    }

    [Fact]
    public void AtlasOrderSet_NoOrderRow_KeepsTheOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CAtlas atlas = TAtlasPrepare(engine, atelier);
        atlas.CAtlasVistaRestore(engine.TEngineVistaStart("repertoire", LCatalogOrder.LCatalogOrderName));

        atlas.CAtlasOrderSet(CCatalogOrder.CCatalogOrderKind);
        atlas.CAtlasOrderSet(null);

        Assert.Equal(CCatalogOrder.CCatalogOrderKind, atlas.CAtlasPanel.CPanelOrder);
    }

    [Fact]
    public void AtlasFilterSet_HiddenLanguage_MarksTheListFiltered()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CAtlas atlas = TAtlasPrepare(engine, atelier);
        atlas.CAtlasVistaRestore(engine.TEngineVistaStart("repertoire", LCatalogOrder.LCatalogOrderName));

        atlas.CAtlasFilterSet(new CCatalogFilter(["English"]));

        Assert.True(atlas.CAtlasFiltered);
        Assert.Equal(["English"], atlas.CAtlasPanel.CPanelFilter.CCatalogFilterHidden);
    }

    [Fact]
    public void AtlasUsageRead_CitedSituation_CountsEveryCard()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LSituation home = TAtlasSituationSave(engine, "at home");
        engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "hearth",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(
                string.Empty,
                string.Empty,
                "a meaning",
                [],
                [TInterface.TSituationDraftCreate(home.LSituationTitle, home.LSituationId)],
                [],
                [],
                [],
                1)],
            []));
        CAtlas atlas = TAtlasPrepare(engine, atelier);

        Assert.Equal(1, atlas.CAtlasUsageRead()[home.LSituationId]);
    }

    [Fact]
    public void AtlasLanguageRead_Workspace_OffersTheCatalogLanguages()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CAtlas atlas = TAtlasPrepare(engine, atelier);

        Assert.Equal(atelier.CAtelierCatalog.CCatalogLanguageRead(), atlas.CAtlasLanguageRead());
    }

    [Fact]
    public void AtlasSituationRead_NoSituation_ReturnsNone()
    {
        Assert.Null(CAtlas.CAtlasSituationRead(null));
    }

    [Fact]
    public void AtlasSituationRead_StoredSituation_CarriesEveryField()
    {
        LSituation situation = TInterface.TSituationCreate(7, "Hearth", "By the fire", LStateValue.LStateValueUnknown);

        CSituationDraft? held = CAtlas.CAtlasSituationRead(situation);

        Assert.NotNull(held);
        Assert.Equal(7, held!.CSituationDraftId);
        Assert.Equal("Hearth", held.CSituationDraftTitle.CStateValueText);
        Assert.Equal("By the fire", held.CSituationDraftDescription.CStateValueText);
        Assert.True(held.CSituationDraftKind.CStateValueUncertain);
        Assert.Empty(held.CSituationDraftImage);
        Assert.Empty(held.CSituationDraftVideo);
    }

    private static CAtlas TAtlasPrepare(LEngine engine, CAtelier atelier)
    {
        CEnvoy envoy = TInterfaceConduct.TEnvoyCreate(false, []);
        CDesk desk = TInterfaceConduct.TDeskCreate(
            engine, "Situation", envoy, "Repertoire", CSubject.CSubjectSituation);
        return CAtlas.CAtlasCreate(atelier, desk, static () => true, envoy, static _ => true);
    }

    private static LSituation TAtlasSituationSave(LEngine engine, string title)
    {
        return engine.TEngineSituationCreate(TInterface.TSituationCreate(0, title, null, null));
    }
}

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

        Assert.Empty(atlas.TAtlasRowsRead()!);
        Assert.Null(atlas.CAtlasPanel.CPanelAperture.CApertureChosen);
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
        atlas.TAtlasVistaRestore(engine.TEngineVistaStart("repertoire", LCatalogOrder.LCatalogOrderName));

        IReadOnlyList<CCatalogSituation> rows = atlas.TAtlasRowsRead()!;

        Assert.Equal(
            TInterface.TLocalizationTextRead("Situation.Untitled"),
            rows.Single(row => row.CCatalogSituationId == blank.LSituationId).CCatalogSituationTitle);
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
        atlas.TAtlasVistaRestore(engine.TEngineVistaStart("repertoire", LCatalogOrder.LCatalogOrderName));

        atlas.CAtlasPanel.CPanelAperture.CApertureQuerySet("court");

        Assert.Equal(["in court"], atlas.TAtlasRowsRead()!.Select(row => row.CCatalogSituationTitle));
    }

    [Fact]
    public void AtlasOrderSet_NoOrderRow_KeepsTheOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CAtlas atlas = TAtlasPrepare(engine, atelier);
        atlas.TAtlasVistaRestore(engine.TEngineVistaStart("repertoire", LCatalogOrder.LCatalogOrderName));

        atlas.CAtlasPanel.CPanelAperture.CApertureOrderSet(CCatalogOrder.CCatalogOrderKind);
        atlas.CAtlasPanel.CPanelAperture.CApertureOrderSet(null);

        Assert.Equal(CCatalogOrder.CCatalogOrderKind, atlas.CAtlasPanel.CPanelAperture.CApertureOrder);
    }

    [Fact]
    public void AtlasFilterSet_HiddenLanguage_MarksTheListFiltered()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CAtlas atlas = TAtlasPrepare(engine, atelier);
        atlas.TAtlasVistaRestore(engine.TEngineVistaStart("repertoire", LCatalogOrder.LCatalogOrderName));

        atlas.CAtlasPanel.CPanelAperture.CApertureFilterSet(new CCatalogFilter(["English"]));

        Assert.True(atlas.CAtlasPanel.CPanelAperture.CApertureFiltered);
        Assert.Equal(["English"], atlas.CAtlasPanel.CPanelAperture.CApertureFilter.CCatalogFilterHidden);
    }

    [Fact]
    public void AtlasApertureTallyRead_CitedSituationChosen_WordsItsTally()
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
        List<string> tallies = [atlas.CAtlasPanel.CPanelAperture.CApertureTallyRead()];
        LVista vista = engine.TEngineVistaStart("repertoire", LCatalogOrder.LCatalogOrderName);
        atlas.TAtlasVistaRestore(vista);
        tallies.Add(atlas.CAtlasPanel.CPanelAperture.CApertureTallyRead());

        vista.TVistaSelect(home.LSituationId);
        tallies.Add(atlas.CAtlasPanel.CPanelAperture.CApertureTallyRead());

        Assert.Equal(
            [
                string.Empty,
                TInterface.TLocalizationTextRead("Situation.UsageNone"),
                TInterface.TLocalizationTextRead("Situation.UsageOne"),
            ],
            tallies);
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
    public void AtlasDraftRead_NoSituation_ReturnsNone()
    {
        Assert.Null(TInterfaceConductPanel.TAtlasDraftRead(null));
    }

    [Fact]
    public void AtlasDraftRead_StoredSituation_CarriesEveryField()
    {
        LSituation situation = TInterface.TSituationCreate(7, "Hearth", "By the fire", LStateValue.LStateValueUnknown);

        CSituationDraft? held = TInterfaceConductPanel.TAtlasDraftRead(situation);

        Assert.NotNull(held);
        Assert.Equal(7, held!.CSituationDraftId);
        Assert.Equal("Hearth", held.CSituationDraftTitle.CStateValueText);
        Assert.Equal("By the fire", held.CSituationDraftDescription.CStateValueText);
        Assert.True(held.CSituationDraftKind.CStateValueUncertain);
        Assert.Empty(held.CSituationDraftImage);
        Assert.Empty(held.CSituationDraftVideo);
    }

    [Fact]
    public void AtlasSituationRead_NoSituation_ReturnsNone()
    {
        Assert.Null(TInterfaceConductPanel.TAtlasSituationRead(null));
    }

    [Fact]
    public void AtlasSituationRead_WrittenFields_ShowsTheirTextWithNoKey()
    {
        LSituation situation = TInterface.TSituationCreate(
            7, TInterfaceState.TStateValueCreate("Hearth"), "By the fire", TInterfaceState.TStateValueCreate("home"));

        CSituation? shown = TInterfaceConductPanel.TAtlasSituationRead(situation);

        Assert.NotNull(shown);
        Assert.Equal(new CStateWording("Hearth", null, false, null), shown!.CSituationTitle);
        Assert.Equal(new CStateWording("home", null, false, null), shown.CSituationKind);
        Assert.Equal(new CStateWording("By the fire", null, false, null), shown.CSituationDescription);
    }

    [Fact]
    public void AtlasSituationRead_MarkdownDescription_ArrivesAsParsedBlocks()
    {
        LSituation situation = TInterface.TSituationCreate(7, "Hearth", "By the **fire**\n\n- warm", null);

        CSituation shown = TInterfaceConductPanel.TAtlasSituationRead(situation)!;

        Assert.Equal(2, shown.CSituationMarkdown.Count);
        Assert.True(shown.CSituationMarkdown[0].CMarkdownBlockSpan[1].CMarkdownSpanBold);
        Assert.Equal("fire", shown.CSituationMarkdown[0].CMarkdownBlockSpan[1].CMarkdownSpanText);
        Assert.True(shown.CSituationMarkdown[1].CMarkdownBlockListed);
    }

    [Fact]
    public void AtlasSituationRead_UnknownFields_WordsEachWithTheUnknownMark()
    {
        LSituation situation = TInterface.TSituationCreate(
            7, LStateValue.LStateValueUnknown, LStateValue.LStateValueUnknown, LStateValue.LStateValueUnknown);

        CSituation shown = TInterfaceConductPanel.TAtlasSituationRead(situation)!;

        Assert.Equal(
            new CStateWording(string.Empty, "Display.Unknown", false, "Display.Unknown"), shown.CSituationTitle);
        Assert.Equal(
            new CStateWording(string.Empty, "Display.Unknown", false, "Display.Unknown"), shown.CSituationKind);
        Assert.Equal(
            new CStateWording(string.Empty, "Display.Unknown", false, "Display.Unknown"), shown.CSituationDescription);
    }

    [Fact]
    public void AtlasSituationRead_UnwrittenFields_MutesTheTitleAndWordsNoKindOrDescription()
    {
        LSituation situation = TInterface.TSituationCreate(7, null, null, null);

        CSituation shown = TInterfaceConductPanel.TAtlasSituationRead(situation)!;

        Assert.Equal(new CStateWording(string.Empty, "Situation.Untitled", true, null), shown.CSituationTitle);
        Assert.Equal(new CStateWording(string.Empty, null, true, null), shown.CSituationKind);
        Assert.Equal(new CStateWording(string.Empty, null, true, null), shown.CSituationDescription);
    }

    [Fact]
    public void AtlasSituationRead_BlankMediaRows_CarriesOnlyTheFilledRowsInOrder()
    {
        LSituation situation = TInterface.TSituationCreate(7, "Hearth", null, null) with
        {
            LSituationImage =
            [
                TInterface.TImageDraftCreate("fire.png", 1),
                TInterface.TImageDraftCreate(string.Empty, 2),
                TInterface.TImageDraftCreate(string.Empty, 3) with
                {
                    LImageDraftLocation = LStateValue.LStateValueUnknown,
                },
                TInterface.TImageDraftCreate(string.Empty, 4) with
                {
                    LImageDraftLocation = TInterface.TStateUnreadableCreate("lost.png"),
                },
            ],
            LSituationVideo =
            [
                TInterface.TVideoDraftCreate(string.Empty, string.Empty, 5),
                TInterface.TVideoDraftCreate("fire.mp4", "00:10-00:40", 6),
            ],
        };

        CSituation shown = TInterfaceConductPanel.TAtlasSituationRead(situation)!;

        Assert.Equal([1L, 3L, 4L], shown.CSituationImage.Select(row => row.CImageDraftId));
        Assert.True(shown.CSituationImage[1].CImageDraftLocation.CStateValueUncertain);
        Assert.Equal("lost.png", shown.CSituationImage[2].CImageDraftLocation.CStateValueText);
        Assert.Equal([6L], shown.CSituationVideo.Select(row => row.CVideoDraftId));
        Assert.Equal("00:10-00:40", shown.CSituationVideo[0].CVideoDraftSpan.CStateValueText);
    }

    [Fact]
    public void AtlasRowsRead_EngineFails_ShowsTheLoadFailureAndAnswersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> asked = [];
        CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, asked);

        Assert.Null(TInterfaceConductPanel.TAtlasFailRead(engine, envoy));
        Assert.Equal(["Situation.LoadFailed"], asked);
    }

    [Fact]
    public void AtlasApertureTallyRead_EngineRefuses_ShowsTheLoadFailureAndAnswersEmpty()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, asked);

        CAtlas atlas = new(
            atelier.CAtelierEntryBundle.CEntryBundleSituation,
            atelier.CAtelierPortraitPort,
            atelier.CAtelierSettingsPort,
            atelier.CAtelierEntryBundle.CEntryBundleVista,
            TInterfaceConductDesk.TDeskCreate(engine, "Situation", envoy, "Repertoire", CSubject.CSubjectSituation),
            static () => true,
            envoy,
            static _ => true);
        atlas.TAtlasVistaRestore(engine.TEngineVistaStart("occurrence", LCatalogOrder.LCatalogOrderHeadword));

        Assert.Equal(string.Empty, atlas.CAtlasPanel.CPanelAperture.CApertureTallyRead());
        Assert.Equal(["Situation.LoadFailed"], asked);
    }

    [Fact]
    public void AtlasOrderRead_Menu_OffersNameKindAndUsage()
    {
        Assert.Equal(
            [CCatalogOrder.CCatalogOrderName, CCatalogOrder.CCatalogOrderKind, CCatalogOrder.CCatalogOrderUsage],
            CAtlas.CAtlasOrderRead());
    }

    private static CAtlas TAtlasPrepare(LEngine engine, CAtelier atelier)
    {
        CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, []);
        CDesk desk = TInterfaceConductDesk.TDeskCreate(
            engine, "Situation", envoy, "Repertoire", CSubject.CSubjectSituation);
        return new CAtlas(
            atelier.CAtelierEntryBundle.CEntryBundleSituation,
            atelier.CAtelierPortraitPort,
            atelier.CAtelierSettingsPort,
            atelier.CAtelierEntryBundle.CEntryBundleVista,
            desk,
            static () => true,
            envoy,
            static _ => true);
    }

    private static LSituation TAtlasSituationSave(LEngine engine, string title)
    {
        return engine.TEngineSituationCreate(TInterface.TSituationCreate(0, title, null, null));
    }
}

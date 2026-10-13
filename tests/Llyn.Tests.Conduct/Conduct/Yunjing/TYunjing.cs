using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TYunjing
{
    internal const string TYunjingPack = """{ "language": "Fixture" }""";

    internal const string TYunjingBook = "Classical Chinese";

    [Fact]
    public async Task YunjingEntryListLoad_FixtureCell_AnswersTheCellEntriesAfterTheFill()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TYunjingPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string language = pack.TLanguageFixtureName;
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, language, "爛", "來");
        CYunjing yunjing = TYunjingPrepare(atelier);
        yunjing.TYunjingDiweiOpen(language, LDiwei.LDiweiInitial, "來");

        CEnsignSheet<IReadOnlyList<CVistaRow>> sheet =
            await yunjing.CYunjingXiaoyun.CEntryListLoad(static (_, _) => static () => { });

        Assert.Equal(["爛"], sheet.CEnsignSheetRows.Select(static row => row.CVistaRowHeadword));
        Assert.Equal(atelier.CAtelierCatalog.CCatalogLanguageRead(), sheet.CEnsignSheetLanguages);
    }

    [Fact]
    public void YunjingShengmuRead_NothingPlaced_ListsNothingUnderTheBareKeys()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CYunjing yunjing = TYunjingPrepare(atelier);

        Assert.True(yunjing.LYunjingAllowed);
        Assert.Empty(yunjing.CYunjingShengmuRead());
        Assert.True(yunjing.CYunjingShengmu.CApertureEmpty);
        Assert.Equal("Yunjing.ShengmuEmpty", yunjing.CYunjingShengmu.CApertureKey);
        Assert.Empty(yunjing.CYunjingYunmuRead());
        Assert.True(yunjing.CYunjingYunmu.CApertureEmpty);
        Assert.Equal("Yunjing.YunmuEmpty", yunjing.CYunjingYunmu.CApertureKey);
        Assert.Empty(yunjing.CYunjingXiaoyun.CEntryListRead());
        Assert.True(yunjing.CYunjingXiaoyun.CEntryListPanel.CPanelAperture.CApertureEmpty);
        Assert.Equal("Yunjing.XiaoyunEmpty", yunjing.CYunjingXiaoyunKey);
        Assert.False(yunjing.CYunjingDiweiShown);
        Assert.True(yunjing.CYunjingDisplayShown);
        Assert.True(yunjing.CYunjingDiweiRead().CDiweiPageEmpty);
    }

    [Fact]
    public void YunjingShengmuRead_NoCellChosen_ListsTheFirstBookLanguage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, TYunjingBook, "爛", "來");
        CYunjing yunjing = TYunjingPrepare(atelier);

        Assert.Equal(["來"], yunjing.CYunjingShengmuRead().Select(static row => row.CDiweiKey));
        Assert.Equal(["寒 I"], yunjing.CYunjingYunmuRead().Select(static row => row.CDiweiKey));
        Assert.DoesNotContain(yunjing.CYunjingShengmuRead(), static row => row.CDiweiChosen);
        Assert.False(yunjing.CYunjingShengmu.CApertureEmpty);
        Assert.Empty(yunjing.CYunjingXiaoyun.CEntryListRead());
    }

    [Fact]
    public void YunjingDiweiOpen_FixtureCell_ListsThatLanguageWithTheCellChosen()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TYunjingPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string language = pack.TLanguageFixtureName;
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, language, "爛", "來");
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, language, "孤", "見");
        CYunjing yunjing = TYunjingPrepare(atelier);
        int changed = 0;
        yunjing.CYunjingChanged += () => changed++;

        yunjing.TYunjingDiweiOpen(language, LDiwei.LDiweiInitial, "來");

        Assert.Equal(1, changed);
        Assert.True(yunjing.CYunjingDiweiShown);
        Assert.False(yunjing.CYunjingDisplayShown);
        Assert.Equal(["來", "見"], yunjing.CYunjingShengmuRead().Select(static row => row.CDiweiKey));
        Assert.Equal([true, false], yunjing.CYunjingShengmuRead().Select(static row => row.CDiweiChosen));
        Assert.Equal(["寒 I"], yunjing.CYunjingYunmuRead().Select(static row => row.CDiweiKey));
        Assert.Equal(["爛"], yunjing.CYunjingXiaoyun.CEntryListRead().Select(static row => row.CVistaRowHeadword));
        Assert.False(yunjing.CYunjingXiaoyun.CEntryListPanel.CPanelAperture.CApertureEmpty);
        Assert.Equal("Yunjing.XiaoyunVacant", yunjing.CYunjingXiaoyunKey);
        Assert.Equal("Yunjing.Shengmu", yunjing.CYunjingDiweiKey);
    }

    [Fact]
    public void YunjingDiweiOpen_UnknownKey_ChoosesNothing()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TYunjingPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string language = pack.TLanguageFixtureName;
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, language, "爛", "來");
        CYunjing yunjing = TYunjingPrepare(atelier);
        int changed = 0;
        yunjing.CYunjingChanged += () => changed++;

        yunjing.TYunjingDiweiOpen(language, LDiwei.LDiweiInitial, "見");

        Assert.Equal(0, changed);
        Assert.False(yunjing.CYunjingDiweiShown);
    }

    [Fact]
    public void YunjingDiweiSelect_ChosenRimeAgain_HidesThePage()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TYunjingPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string language = pack.TLanguageFixtureName;
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, language, "爛", "來");
        CYunjing yunjing = TYunjingPrepare(atelier);
        yunjing.TYunjingDiweiOpen(language, LDiwei.LDiweiRime, "寒 I");
        long rime = Assert.Single(yunjing.CYunjingYunmuRead()).CDiweiId;

        Assert.True(yunjing.CYunjingDiweiShown);
        Assert.Equal("Yunjing.Yunmu", yunjing.CYunjingDiweiKey);

        yunjing.CYunjingDiweiSelect(rime, true);

        Assert.False(yunjing.CYunjingDiweiShown);
        Assert.True(yunjing.CYunjingDisplayShown);
        Assert.Empty(yunjing.CYunjingXiaoyun.CEntryListRead());
        Assert.True(yunjing.CYunjingXiaoyun.CEntryListPanel.CPanelAperture.CApertureEmpty);
        Assert.Equal("Yunjing.XiaoyunEmpty", yunjing.CYunjingXiaoyunKey);

        yunjing.CYunjingDiweiSelect(null, true);
        yunjing.CYunjingDiweiSelect(rime, null);

        Assert.False(yunjing.CYunjingDiweiShown);
    }

    [Fact]
    public void YunjingDiweiSelect_UnsavedDraftKept_ChangesNothing()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TYunjingPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string language = pack.TLanguageFixtureName;
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, language, "爛", "來");
        List<string> asked = [];
        CYunjing yunjing = TYunjingPrepare(atelier, TEnvoyFake.TEnvoyCreate(null, asked));
        yunjing.TYunjingDiweiOpen(language, LDiwei.LDiweiRime, "寒 I");
        long rime = Assert.Single(yunjing.CYunjingYunmuRead()).CDiweiId;
        yunjing.CYunjingXiaoyun.CEntryListPanel.CPanelFreshOpen();
        yunjing.CYunjingXiaoyun.CEntryListEditor.CEditorEntry.CEntryHeadwordSet("water");
        int changed = 0;
        yunjing.CYunjingChanged += () => changed++;

        yunjing.CYunjingDiweiSelect(rime, true);

        Assert.Equal(["Leave"], asked);
        Assert.True(Assert.Single(yunjing.CYunjingYunmuRead()).CDiweiChosen);
        Assert.True(yunjing.CYunjingXiaoyun.CEntryListPanel.CPanelEditing);
        Assert.Equal("water", yunjing.CYunjingXiaoyun.CEntryListEditor.TEditorDraftRead()?.CEntryDraftHeadword);
        Assert.Equal(0, changed);
    }

    [Fact]
    public void YunjingDiweiSelect_UnsavedDraftDiscarded_TogglesAndCloses()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TYunjingPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string language = pack.TLanguageFixtureName;
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, language, "爛", "來");
        List<string> asked = [];
        CYunjing yunjing = TYunjingPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        yunjing.TYunjingDiweiOpen(language, LDiwei.LDiweiRime, "寒 I");
        long rime = Assert.Single(yunjing.CYunjingYunmuRead()).CDiweiId;
        yunjing.CYunjingXiaoyun.CEntryListPanel.CPanelFreshOpen();
        yunjing.CYunjingXiaoyun.CEntryListEditor.CEditorEntry.CEntryHeadwordSet("water");
        int changed = 0;
        yunjing.CYunjingChanged += () => changed++;

        yunjing.CYunjingDiweiSelect(rime, true);

        Assert.Equal(["Leave"], asked);
        Assert.False(yunjing.CYunjingDiweiShown);
        Assert.DoesNotContain(yunjing.CYunjingYunmuRead(), static row => row.CDiweiChosen);
        Assert.False(yunjing.CYunjingXiaoyun.CEntryListPanel.CPanelEditing);
        Assert.Equal(1, changed);
    }

    [Fact]
    public void YunjingApertureQuerySet_UnmatchedQuery_UnchoosesAndReadsTheUnmatchedKeys()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TYunjingPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string language = pack.TLanguageFixtureName;
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, language, "爛", "來");
        CYunjing yunjing = TYunjingPrepare(atelier);
        yunjing.TYunjingDiweiOpen(language, LDiwei.LDiweiInitial, "來");

        yunjing.CYunjingXiaoyun.CEntryListPanel.CPanelAperture.CApertureQuerySet("zzz");

        Assert.Empty(yunjing.CYunjingXiaoyun.CEntryListRead());
        Assert.Equal("Yunjing.XiaoyunUnmatched", yunjing.CYunjingXiaoyunKey);

        yunjing.CYunjingYunmu.CApertureQuerySet("zzz");
        yunjing.CYunjingShengmu.CApertureQuerySet("zzz");

        Assert.Empty(yunjing.CYunjingYunmuRead());
        Assert.Equal("Yunjing.YunmuUnmatched", yunjing.CYunjingYunmu.CApertureKey);
        Assert.Empty(yunjing.CYunjingShengmuRead());
        Assert.True(yunjing.CYunjingShengmu.CApertureEmpty);
        Assert.Equal("Yunjing.ShengmuUnmatched", yunjing.CYunjingShengmu.CApertureKey);
        Assert.False(yunjing.CYunjingDiweiShown);
    }

    [Fact]
    public void YunjingDiweiOpen_QueriedColumns_EmptiesBothQueriesBeforeTheOpening()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TYunjingPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string language = pack.TLanguageFixtureName;
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, language, "爛", "來");
        CYunjing yunjing = TYunjingPrepare(atelier);
        yunjing.CYunjingShengmu.CApertureQuerySet("zzz");
        yunjing.CYunjingYunmu.CApertureQuerySet("zzz");
        List<string> seen = [];
        yunjing.CYunjingDiweiOpened += () =>
            seen.Add(yunjing.CYunjingShengmu.CApertureKey + " " + yunjing.CYunjingYunmu.CApertureKey);

        yunjing.TYunjingDiweiOpen(language, LDiwei.LDiweiInitial, "來");

        Assert.Equal(["Yunjing.ShengmuEmpty Yunjing.YunmuEmpty"], seen);
        Assert.True(yunjing.CYunjingDiweiShown);
    }

    [Fact]
    public void YunjingApertureOrderSet_NullAfterAnOrder_KeepsTheChosenOne()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CYunjing yunjing = TYunjingPrepare(atelier);

        Assert.Equal(CCatalogOrder.CCatalogOrderName, yunjing.CYunjingShengmu.CApertureOrder);
        Assert.Equal(CCatalogOrder.CCatalogOrderName, yunjing.CYunjingYunmu.CApertureOrder);

        yunjing.CYunjingShengmu.CApertureOrderSet(CCatalogOrder.CCatalogOrderReverse);
        yunjing.CYunjingShengmu.CApertureOrderSet(null);
        yunjing.CYunjingYunmu.CApertureOrderSet(CCatalogOrder.CCatalogOrderUsage);
        yunjing.CYunjingYunmu.CApertureOrderSet(null);

        Assert.Equal(CCatalogOrder.CCatalogOrderReverse, yunjing.CYunjingShengmu.CApertureOrder);
        Assert.Equal(CCatalogOrder.CCatalogOrderUsage, yunjing.CYunjingYunmu.CApertureOrder);
    }

    [Fact]
    public void YunjingPanelRowOpen_ScribeOn_OpensTheEntryInTheEditorUntilClosed()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TYunjingPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string language = pack.TLanguageFixtureName;
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, language, "爛", "來");
        CYunjing yunjing = TYunjingPrepare(atelier);
        yunjing.TYunjingDiweiOpen(language, LDiwei.LDiweiInitial, "來");
        long entry = Assert.Single(yunjing.CYunjingXiaoyun.CEntryListRead()).CVistaRowId;

        yunjing.CYunjingXiaoyun.CEntryListPanel.CPanelRowOpen(entry);
        yunjing.CYunjingXiaoyun.CEntryListPanel.CPanelScribeToggle(true);

        Assert.True(yunjing.CYunjingXiaoyun.CEntryListEditing);
        Assert.Equal(entry, yunjing.CYunjingXiaoyun.CEntryListEditor.CEditorDesk.CDeskStoredRead());

        yunjing.CYunjingXiaoyun.CEntryListPanel.CPanelEntryClose();

        Assert.False(yunjing.CYunjingXiaoyun.CEntryListEditing);
        Assert.Null(yunjing.CYunjingXiaoyun.CEntryListEditor.CEditorDesk.CDeskStoredRead());
    }

    [Fact]
    public void YunjingOrderRead_Menu_OffersTheThreeOrderings()
    {
        Assert.Equal(
            [
                CCatalogOrder.CCatalogOrderName,
                CCatalogOrder.CCatalogOrderReverse,
                CCatalogOrder.CCatalogOrderUsage,
            ],
            CYunjing.CYunjingOrderRead());
    }

    internal static CYunjing TYunjingPrepare(CAtelier atelier)
    {
        return TYunjingPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
    }

    internal static CYunjing TYunjingPrepare(CAtelier atelier, CEnvoy envoy)
    {
        CYunjing yunjing = CYunjing.CYunjingCreate(atelier, static () => true, envoy, static run => run());
        return yunjing;
    }
}

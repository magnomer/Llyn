using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TYunjingLanguage
{
    [Fact]
    public void DiweiShow_FixtureCell_ListsThatLanguage()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate("""{ "language": "Fixture" }""");
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        string language = pack.TLanguageFixtureName;
        TYunjingDiweiPlace(engine, workspace, language, "爛", "來");
        TYunjingDiweiPlace(engine, workspace, language, "孤", "見");
        LYunjing panel = TYunjingPrepare(engine);
        int changed = 0;
        panel.LYunjingChanged += () => changed++;

        panel.TYunjingDiweiShow(language, LDiwei.LDiweiInitial, "來");

        Assert.True(panel.LYunjingAllowed);
        Assert.True(panel.LYunjingDiweiShown);
        Assert.False(panel.LYunjingDisplayShown);
        Assert.Equal(1, changed);
        Assert.Equal(["來", "見"], panel.TYunjingShengmuRead().Select(row => row.LDiweiKey));
        Assert.Equal([true, false], panel.TYunjingShengmuRead().Select(row => row.LDiweiChosen));
        Assert.Equal(["寒 I"], panel.TYunjingYunmuRead().Select(row => row.LDiweiKey));
        Assert.Equal(["爛"], panel.TYunjingXiaoyunRead().Select(row => row.LVistaRowHeadword));
        Assert.False(panel.LYunjingXiaoyunEmpty);
        Assert.Equal("Yunjing.Shengmu", panel.LYunjingDiweiKey);
    }

    [Fact]
    public void DiweiSelect_ChosenCellAgain_HidesPage()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate("""{ "language": "Fixture" }""");
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        string language = pack.TLanguageFixtureName;
        TYunjingDiweiPlace(engine, workspace, language, "爛", "來");
        LYunjing panel = TYunjingPrepare(engine);
        panel.TYunjingDiweiShow(language, LDiwei.LDiweiRime, "寒 I");

        Assert.True(panel.LYunjingDiweiShown);
        Assert.Equal("Yunjing.Yunmu", panel.LYunjingDiweiKey);

        panel.TYunjingDiweiSelect(panel.TYunjingYunmuRead()[0].LDiweiId, true);

        Assert.False(panel.LYunjingDiweiShown);
        Assert.True(panel.LYunjingDisplayShown);
        Assert.Empty(panel.TYunjingXiaoyunRead());
        Assert.True(panel.LYunjingXiaoyunEmpty);
        Assert.Equal("Yunjing.XiaoyunEmpty", panel.LYunjingXiaoyunKey);
    }

    internal static void TYunjingDiweiPlace(
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
        engine.TEngineReflexSet(
            entry.LEntryId,
            engine.TEngineReflexRead(entry.LEntryId)
                .Select(reflex => reflex with { LReflexAnchors = anchors })
                .ToList());
    }

    internal static LYunjing TYunjingPrepare(LEngine engine)
    {
        LYunjing panel = TInterfaceDeportment.TYunjingCreate(engine, () => false, () => true);
        panel.TYunjingVistaRestore(
            engine.TEngineVistaStart("yunjing", LCatalogOrder.LCatalogOrderName),
            engine.TEngineVistaStart("yunmu", LCatalogOrder.LCatalogOrderName),
            engine.TEngineVistaStart("xiaoyun", LCatalogOrder.LCatalogOrderHeadword));
        return panel;
    }
}

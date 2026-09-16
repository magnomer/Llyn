using System.Linq;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TDiweiArchive
{
    private const string TDiweiArchivePack = """{ "language": "Fixture" }""";

    private static readonly LHypothesis TDiweiArchiveTables = TInterface.THypothesisCreate(
        new Dictionary<string, string> { ["來"] = "l", ["見"] = "k" },
        new Dictionary<string, string> { ["寒 一"] = "an", ["模 一"] = "o" },
        new Dictionary<string, IReadOnlyList<LHypothesisTone>>
        {
            ["去"] =
            [
                TInterface.THypothesisToneCreate("^[ptcskʔh]", [], "5"),
                TInterface.THypothesisToneCreate("", [], "6"),
            ],
            ["平"] = [TInterface.THypothesisToneCreate("", [], "1")],
        });

    [Fact]
    public void DiweiApply_TwoPlacements_LinksEveryPartOfEach()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TDiweiArchivePack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        string language = pack.TLanguageFixtureName;
        LEntry lan = engine.TEngineEntrySave(TDiweiDraftCreate("爛", language, "란"));
        engine.TEngineEntrySave(TDiweiDraftCreate("爛漫", language, "란만"));
        LEntry gu = engine.TEngineEntrySave(TDiweiDraftCreate("孤", language, "고"));
        LFanqieArchive fanqie = TInterface.TFanqieArchiveCreate(workspace.TWorkspaceDatabase);
        fanqie.TFanqieSave(
            language, "爛", [TDiweiRowCreate("爛", "來", "寒", "去"), TDiweiRowCreate("爛", "來", "寒A", "平", 1)]);
        fanqie.TFanqieSave(language, "孤", [TDiweiRowCreate("孤", "見", "模", "平")]);
        LDiweiArchive archive = TInterface.TDiweiArchiveCreate(workspace.TWorkspaceDatabase);

        archive.TDiweiApply(language, "爛", TDiweiArchiveTables);
        archive.TDiweiApply(language, "孤", TDiweiArchiveTables);
        TDiweiAnchorApply(engine, lan.LEntryId, fanqie.TFanqieRead(language, "爛"));
        TDiweiAnchorApply(engine, gu.LEntryId, fanqie.TFanqieRead(language, "孤"));

        Assert.Equal(["來", "見"], TDiweiKeyScan(archive, language, LDiwei.LDiweiInitial));
        Assert.Equal(["寒 I", "模 I"], TDiweiKeyScan(archive, language, LDiwei.LDiweiRime));
        Assert.Equal(["6", "1"], TDiweiKeyScan(archive, language, LDiwei.LDiweiTone));
        LDiwei lai = Assert.IsType<LDiwei>(archive.TDiweiFind(language, LDiwei.LDiweiInitial, "來"));
        LDiwei han = Assert.IsType<LDiwei>(archive.TDiweiFind(language, LDiwei.LDiweiRime, "寒 I"));
        LDiwei sixth = Assert.IsType<LDiwei>(archive.TDiweiFind(language, LDiwei.LDiweiTone, "6"));
        LDiwei first = Assert.IsType<LDiwei>(archive.TDiweiFind(language, LDiwei.LDiweiTone, "1"));
        Assert.Equal((1, 1, 1, 2), (lai.LDiweiCount, han.LDiweiCount, sixth.LDiweiCount, first.LDiweiCount));
        LDiwei jian = Assert.IsType<LDiwei>(archive.TDiweiFind(language, LDiwei.LDiweiInitial, "見"));
        Assert.Equal([lan.LEntryId], archive.TDiweiEntryScan(language, [lai.LDiweiId, sixth.LDiweiId]));
        Assert.Equal([lan.LEntryId, gu.LEntryId], archive.TDiweiEntryScan(language, [first.LDiweiId]));
        Assert.Empty(archive.TDiweiEntryScan(language, [han.LDiweiId, jian.LDiweiId]));
        Assert.Equal(9, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM fanqie_diwei;"));
    }

    [Fact]
    public void DiweiApply_Hypothesis_StoresReadingAndClassOnRows()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TDiweiArchivePack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        string language = pack.TLanguageFixtureName;
        LFanqieArchive fanqie = TInterface.TFanqieArchiveCreate(workspace.TWorkspaceDatabase);
        fanqie.TFanqieSave(
            language, "爛", [TDiweiRowCreate("爛", "來", "寒", "去"), TDiweiRowCreate("爛", "日", "寒", "平", 1)]);
        LDiweiArchive archive = TInterface.TDiweiArchiveCreate(workspace.TWorkspaceDatabase);

        archive.TDiweiApply(language, "爛", TDiweiArchiveTables);

        Assert.Equal(
            [("lan", "6"), ("", "")],
            fanqie.TFanqieRead(language, "爛").Select(row => (row.LFanqieRowReading, row.LFanqieRowClass)));

        archive.TDiweiRebuild(language, null);

        Assert.Equal(
            ["", ""], fanqie.TFanqieRead(language, "爛").Select(row => row.LFanqieRowReading));
    }

    [Fact]
    public void DiweiRebuild_NewHypothesis_ReplacesToneClassesAndDropsOrphans()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TDiweiArchivePack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        string language = pack.TLanguageFixtureName;
        LFanqieArchive fanqie = TInterface.TFanqieArchiveCreate(workspace.TWorkspaceDatabase);
        fanqie.TFanqieSave(language, "爛", [TDiweiRowCreate("爛", "來", "寒", "去")]);
        LDiweiArchive archive = TInterface.TDiweiArchiveCreate(workspace.TWorkspaceDatabase);
        archive.TDiweiApply(language, "爛", TDiweiArchiveTables);

        archive.TDiweiRebuild(language, null);

        Assert.Empty(archive.TDiweiRead(language, LDiwei.LDiweiTone));
        Assert.Single(archive.TDiweiRead(language, LDiwei.LDiweiInitial));
        Assert.Equal(2, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM diwei;"));
    }

    [Fact]
    public void FanqieSave_Refetch_DropsStaleLinksByCascade()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TDiweiArchivePack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        string language = pack.TLanguageFixtureName;
        LFanqieArchive fanqie = TInterface.TFanqieArchiveCreate(workspace.TWorkspaceDatabase);
        LDiweiArchive archive = TInterface.TDiweiArchiveCreate(workspace.TWorkspaceDatabase);
        fanqie.TFanqieSave(language, "爛", [TDiweiRowCreate("爛", "來", "寒", "去")]);
        archive.TDiweiApply(language, "爛", TDiweiArchiveTables);

        fanqie.TFanqieSave(language, "爛", [TDiweiRowCreate("爛", "見", "模", "平")]);
        archive.TDiweiApply(language, "爛", TDiweiArchiveTables);

        Assert.Equal(3, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM fanqie_diwei;"));
        Assert.Null(archive.TDiweiFind(language, LDiwei.LDiweiInitial, "來"));
        Assert.NotNull(archive.TDiweiFind(language, LDiwei.LDiweiInitial, "見"));
    }

    [Theory]
    [InlineData("模", "一", false, "模 I")]
    [InlineData("寒A", "一", true, "寒 I W")]
    [InlineData("侵", "三", false, "侵 III")]
    [InlineData("侵", "", true, "侵 W")]
    [InlineData("侵", "五", false, "侵 五")]
    [InlineData("", "一", true, "")]
    public void RimeFormat_DivisionAndRounding_KeysSeparateRows(
        string rime, string division, bool rounded, string expected)
    {
        Assert.Equal(expected, TInterface.TDiweiRimeFormat(rime, division, rounded));
    }

    private static LFanqieRow TDiweiRowCreate(
        string character, string initial, string rime, string tone, int position = 0)
    {
        return TInterface.TFanqieRowCreate(character, position, initial, rime, "一", tone);
    }

    private static IReadOnlyList<string> TDiweiKeyScan(LDiweiArchive archive, string language, string kind)
    {
        return archive.TDiweiRead(language, kind).Select(row => row.LDiweiKey).ToList();
    }

    private static void TDiweiAnchorApply(LEngine engine, long entryId, IReadOnlyList<LFanqieRow> rows)
    {
        IReadOnlyList<long> anchors = rows.Select(row => row.LFanqieRowId).ToList();
        engine.TEngineReflexSet(
            entryId,
            engine.TEngineReflexRead(entryId).Select(reflex => reflex with { LReflexAnchors = anchors }).ToList());
    }

    private static LEntryDraft TDiweiDraftCreate(string headword, string language, string reading = "")
    {
        return TInterface.TEntryDraftCreate(
            headword,
            language,
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a state", [], [], [], [], [], 1)],
            [],
            reflexes: reading.Length == 0 ? [] : [TInterface.TReflexDraftCreate("Korean", "", reading)]);
    }
}

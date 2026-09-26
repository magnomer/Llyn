using System.Linq;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TStemArchive
{
    private const string TStemArchivePack = """{ "language": "Fixture" }""";

    [Fact]
    public void StemApply_TwoSeries_LinksBothAndCountsEntries()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TStemArchivePack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        string language = pack.TLanguageFixtureName;
        LEntry jiang = engine.TEngineEntrySave(TStemDraftCreate("江", language));
        LEntry gong = engine.TEngineEntrySave(TStemDraftCreate("工", language));
        LShengfuArchive shengfu = TInterface.TShengfuArchiveCreate(workspace.TWorkspaceDatabase);
        shengfu.TShengfuSave(language, TInterface.TShengfuCreate("江", "工"));
        shengfu.TShengfuSave(language, TInterface.TShengfuCreate("工", "工·巨"));
        LStemArchive archive = TInterface.TStemArchiveCreate(workspace.TWorkspaceDatabase);

        archive.TStemApply(language, "江", ["工"]);
        archive.TStemApply(language, "工", ["工", "巨"]);

        Assert.Equal(["工", "巨"], archive.TStemRead(language).Select(row => row.LStemKey));
        LStem stem = Assert.IsType<LStem>(archive.TStemFind(language, "工"));
        LStem other = Assert.IsType<LStem>(archive.TStemFind(language, "巨"));
        Assert.Equal((2, 1), (stem.LStemCount, other.LStemCount));
        Assert.Equal(["江", "工"], archive.TStemCharacterRead(stem.LStemId));
        Assert.Equal([jiang.LEntryId, gong.LEntryId], archive.TStemEntryScan(language, [stem.LStemId]));
        Assert.Equal([gong.LEntryId], archive.TStemEntryScan(language, [stem.LStemId, other.LStemId]));
        Assert.Equal(3, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM shengfu_stem;"));
    }

    [Fact]
    public void StemApply_SeriesGone_DropsTheRowNothingLinksTo()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TStemArchivePack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        string language = pack.TLanguageFixtureName;
        LShengfuArchive shengfu = TInterface.TShengfuArchiveCreate(workspace.TWorkspaceDatabase);
        shengfu.TShengfuSave(language, TInterface.TShengfuCreate("江", "工"));
        LStemArchive archive = TInterface.TStemArchiveCreate(workspace.TWorkspaceDatabase);
        archive.TStemApply(language, "江", ["工"]);

        archive.TStemApply(language, "江", ["攻"]);

        Assert.Null(archive.TStemFind(language, "工"));
        Assert.NotNull(archive.TStemFind(language, "攻"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM shengfu_stem;"));
    }

    [Fact]
    public void StemApply_CharacterWithoutStoredSeries_LinksNothing()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TStemArchivePack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LStemArchive archive = TInterface.TStemArchiveCreate(workspace.TWorkspaceDatabase);

        archive.TStemApply(pack.TLanguageFixtureName, "江", ["工"]);

        Assert.Empty(archive.TStemRead(pack.TLanguageFixtureName));
    }

    [Fact]
    public void StemRebuild_StoredRows_BuildsEverySeriesAgain()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TStemArchivePack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        string language = pack.TLanguageFixtureName;
        LShengfuArchive shengfu = TInterface.TShengfuArchiveCreate(workspace.TWorkspaceDatabase);
        shengfu.TShengfuSave(language, TInterface.TShengfuCreate("江", "工"));
        shengfu.TShengfuSave(language, TInterface.TShengfuCreate("龍", "龍·尨"));
        LStemArchive archive = TInterface.TStemArchiveCreate(workspace.TWorkspaceDatabase);

        archive.TStemRebuild(language, "·");

        Assert.Equal(["工", "龍", "尨"], archive.TStemRead(language).Select(row => row.LStemKey));
        Assert.Equal(3, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM shengfu_stem;"));
    }

    [Fact]
    public void ShengfuSave_Refetch_DropsStaleLinksByCascade()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TStemArchivePack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        string language = pack.TLanguageFixtureName;
        LShengfuArchive shengfu = TInterface.TShengfuArchiveCreate(workspace.TWorkspaceDatabase);
        shengfu.TShengfuSave(language, TInterface.TShengfuCreate("江", "工"));
        LStemArchive archive = TInterface.TStemArchiveCreate(workspace.TWorkspaceDatabase);
        archive.TStemApply(language, "江", ["工"]);

        workspace.TWorkspaceScriptRun("DELETE FROM shengfu WHERE character = '江';");

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM shengfu_stem;"));
    }

    [Theory]
    [InlineData("工", "·", new[] { "工" })]
    [InlineData("龍·尨", "·", new[] { "龍", "尨" })]
    [InlineData(" 工 · 工 ", "·", new[] { "工" })]
    [InlineData("龍·尨", "", new[] { "龍·尨" })]
    [InlineData("", "·", new string[0])]
    public void StemKeyScan_StoredText_CutsOneKeyPerSeries(string text, string separator, string[] expected)
    {
        Assert.Equal(expected, TInterface.TStemKeyScan(text, separator));
    }

    private static LEntryDraft TStemDraftCreate(string headword, string language)
    {
        return TInterface.TEntryDraftCreate(
            headword,
            language,
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a state", [], [], [], [], [], 1)],
            []);
    }
}

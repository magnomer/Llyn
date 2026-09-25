using System.Globalization;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TDisplay
{
    [Fact]
    public void DisplayStampFormat_UnreadableText_ReturnsEmpty()
    {
        Assert.Empty(TInterfaceConduct.TDisplayStampFormat("not a moment"));
        Assert.Empty(TInterfaceConduct.TDisplayStampFormat(null));
    }

    [Fact]
    public void DisplayFoldScan_NoRule_ReturnsEmpty()
    {
        Assert.Empty(TInterfaceConduct.TDisplayFoldScan([]));
    }

    [Fact]
    public void DisplayFoldSet_CurrentValue_KeepsFold()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDisplaySound sound = TInterfaceConduct.TDisplaySoundCreate(engine);

        sound.TDisplayFoldSet(true);
        sound.TDisplayFoldSet(true);

        Assert.True(sound.LDisplayFoldOpened);
    }

    [Fact]
    public void DisplayBandResolve_UnbandedRows_ReturnsZero()
    {
        LFrequency[] rows =
        [
            TInterface.TFrequencyCreate("one", "12", null),
            TInterface.TFrequencyCreate("two", "3", null),
        ];

        Assert.Equal(0, TInterfaceConduct.TDisplayBandResolve(rows));
    }

    [Fact]
    public void DisplayBandResolve_BandedRow_ReturnsItsRank()
    {
        LFrequency[] rows =
        [
            TInterface.TFrequencyCreate("one", "12", null),
            TInterface.TFrequencyCreate("two", "3", "Everyday"),
        ];

        Assert.Equal(3, TInterfaceConduct.TDisplayBandResolve(rows));
    }

    [Fact]
    public void DisplayBandResolve_UnknownBandFirst_ReturnsNextRank()
    {
        LFrequency[] rows =
        [
            TInterface.TFrequencyCreate("one", "12", "Nowhere"),
            TInterface.TFrequencyCreate("two", "3", "Core"),
        ];

        Assert.Equal(4, TInterfaceConduct.TDisplayBandResolve(rows));
    }

    [Fact]
    public void DisplayBandRead_TopRank_ReturnsCore()
    {
        Assert.Equal("Core", TInterfaceConduct.TDisplayBandRead(4));
    }

    [Fact]
    public void DisplayFoldScan_FoldedRule_ReturnsItsLanguage()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(
            """
            { "reflex": [
                { "language": "Jin", "url": "https://example.test/{word}", "match": "(?<text>x)", "folded": true },
                { "language": "Wu", "url": "https://example.test/{word}", "match": "(?<text>x)" } ] }
            """);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        Assert.Equal(["Jin"], TInterfaceConduct.TDisplayFoldScan(engine.TEngineReflexRead(pack.TLanguageFixtureName)));
    }

    [Fact]
    public void DisplaySoundClear_ShownDraft_DropsDraftAndEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDisplaySound sound = TInterfaceConduct.TDisplaySoundCreate(engine);
        sound.TDisplaySoundShow(7, TDisplayDraftCreate([]));

        sound.TDisplaySoundClear();

        Assert.Null(sound.LDisplayShown);
        Assert.Null(sound.LDisplayEntry);
    }

    [Fact]
    public void DisplayReflexRead_LoadedReflex_PrefersIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDisplaySound sound = TInterfaceConduct.TDisplaySoundCreate(engine);
        LEntryDraft draft = TDisplayDraftCreate([TInterface.TReflexDraftCreate("Korean", "", "a")]);
        long id = TExemplar.TExemplarSave(engine, draft)[0];
        sound.TDisplaySoundShow(id, TDisplayDraftCreate([]));

        sound.TDisplayReflexLoad();

        Assert.Equal("a", Assert.Single(sound.TDisplayReflexRead()).LReflexDraftText);
    }

    [Fact]
    public void DisplaySourceFormat_OnceInterval_FormatsInterval()
    {
        LFrequency[] rows =
        [
            TInterface.TFrequencyIntervalCreate("Corpus", "0.5", 12000),
            TInterface.TFrequencyCreate("List", "7", null),
        ];

        string text = TInterfaceConduct.TDisplaySourceFormat(rows, "once in {0} words");

        Assert.Equal(
            "Corpus: once in " + 12000.ToString("N0", CultureInfo.CurrentCulture) + " words\nList: 7",
            text);
    }

    private static LEntryDraft TDisplayDraftCreate(IReadOnlyList<LReflexDraft> reflexes) =>
        TInterface.TEntryDraftCreate(
            "kindle",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a state", [], [], [], [], [], 1)],
            [],
            reflexes: reflexes);
}

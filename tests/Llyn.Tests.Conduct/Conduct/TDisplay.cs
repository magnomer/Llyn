using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TDisplay
{
    [Fact]
    public void DisplaySoundClear_ShownDraft_DropsDraftAndEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDisplaySound sound = TInterfaceConductSound.TDisplaySoundCreate(engine);
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
        LDisplaySound sound = TInterfaceConductSound.TDisplaySoundCreate(engine);
        LEntryDraft draft = TDisplayDraftCreate([TInterface.TReflexDraftCreate("Korean", "", "a")]);
        long id = TExemplar.TExemplarSave(engine, draft)[0];
        sound.TDisplaySoundShow(id, TDisplayDraftCreate([]));

        sound.TDisplayReflexLoad();

        Assert.Equal("a", Assert.Single(sound.TDisplayReflexRead()).LReflexDraftText);
    }

    [Fact]
    public void DisplayFrequencyRead_NoEntry_ReturnsNone()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDisplay display = new TEditorFixture(TInterfaceEditor.TEditorCreate(engine)).TEditorFixtureDisplay;

        Assert.Null(display.TDisplayFrequencyRead(null, "once in {0} words"));
    }

    [Fact]
    public void DisplayFrequencyRead_EntryWithoutFrequency_ReturnsNone()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long id = TExemplar.TExemplarSave(engine, TDisplayDraftCreate([]))[0];
        CDisplay display = new TEditorFixture(TInterfaceEditor.TEditorCreate(engine)).TEditorFixtureDisplay;

        Assert.Null(display.TDisplayFrequencyRead(id, "once in {0} words"));
    }

    [Fact]
    public void DisplayCompassRead_ShownCards_NamesEachPartAndCardAndNumbersTwins()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDisplay display = new TEditorFixture(TInterfaceEditor.TEditorCreate(engine)).TEditorFixtureDisplay;
        LEntryDraft draft = TInterface.TEntryDraftCreate(
            "kindle",
            "English",
            string.Empty,
            string.Empty,
            [
                TInterface.TCardDraftCreate("to light", string.Empty, "a state", [], [], [], [], [], 1),
                TInterface.TCardDraftCreate(string.Empty, string.Empty, "a flame", [], [], [], [], [], 2),
                TInterface.TCardDraftCreate(
                    LStateValue.LStateValueUnknown, string.Empty, "a spark", [], [], [], [], [], 3),
            ],
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a fire", [], [], [], [], [], 1)]);
        display.LDisplayRule.LDisplaySound.TDisplaySoundShow(7, draft);

        IReadOnlyList<CCompassRow> rows = display.CDisplayCompass.CCompassRead(
            [CCompassPart.CCompassPartSpeech, CCompassPart.CCompassPartMeaning, CCompassPart.CCompassPartCollocation],
            key => key == "Display.CollocationSingle" ? "Display.MeaningSingle" : key);

        Assert.Equal(
            [
                "Speech.Title",
                "Display.MeaningPlural",
                "to light",
                "Display.MeaningSingle (1)",
                "Display.Unknown",
                "Display.Collocation",
                "Display.MeaningSingle (2)",
            ],
            rows.Select(static row => row.CCompassRowName));
        Assert.Equal(["", "", "1", "2", "3", "", "1"], rows.Select(static row => row.CCompassRowNumber));
        Assert.Equal([0, 0, 1, 1, 1, 0, 1], rows.Select(static row => row.CCompassRowDepth));
        Assert.Equal([null, null, 0, 1, 2, null, 0], rows.Select(static row => row.CCompassRowCard));
        Assert.Equal(CCompassPart.CCompassPartCollocation, rows[^1].CCompassRowPart);
    }

    [Fact]
    public void DisplayCompassRead_NothingShown_NamesOnlyTheParts()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDisplay display = new TEditorFixture(TInterfaceEditor.TEditorCreate(engine)).TEditorFixtureDisplay;

        IReadOnlyList<CCompassRow> rows = display.CDisplayCompass.CCompassRead(
            [CCompassPart.CCompassPartMeaning, CCompassPart.CCompassPartIncoming, CCompassPart.CCompassPartNote],
            static key => "<" + key + ">");

        Assert.Equal(
            ["<Display.MeaningPlural>", "<Display.Translated>", "<Display.Note>"],
            rows.Select(static row => row.CCompassRowName));
        Assert.All(rows, static row => Assert.Null(row.CCompassRowCard));
    }

    [Theory]
    [InlineData(int.MinValue, 0, CFrequencyTier.CFrequencyTierUnknown, false)]
    [InlineData(-1, 0, CFrequencyTier.CFrequencyTierUnknown, false)]
    [InlineData(0, 0, CFrequencyTier.CFrequencyTierUnknown, false)]
    [InlineData(1, 1, CFrequencyTier.CFrequencyTierRare, true)]
    [InlineData(2, 2, CFrequencyTier.CFrequencyTierAdvanced, true)]
    [InlineData(3, 3, CFrequencyTier.CFrequencyTierEveryday, true)]
    [InlineData(4, 4, CFrequencyTier.CFrequencyTierCore, true)]
    public void DisplayFrequencyRead_HostileBand_KeepsAFullStarRow(
        int band, int stars, CFrequencyTier rank, bool ranked)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDisplay display = TInterfaceConductSound.TDisplayChosenCreate(
            engine,
            TInterfaceConduct.TEntryBundleCreate(
                engine, TFrequencyPortCreate(TInterface.TFrequencyGaugeCreate(band, string.Empty))),
            1);
        int row = TInterface.TFrequencyScaleRead().Count;

        CFrequency? frequency = display.CDisplayFrequencyRead(static key => key);

        Assert.Equal(new CFrequency(stars, string.Empty, rank, row - stars, ranked), frequency);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(int.MaxValue)]
    public void DisplayFrequencyRead_BandPastScale_ReadsUnknownTier(int band)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDisplay display = TInterfaceConductSound.TDisplayChosenCreate(
            engine,
            TInterfaceConduct.TEntryBundleCreate(
                engine, TFrequencyPortCreate(TInterface.TFrequencyGaugeCreate(band, "src"))),
            1);
        CFrequency? frequency = null;

        Exception? thrown = Record.Exception(() => { frequency = display.CDisplayFrequencyRead(static key => key); });

        Assert.Null(thrown);
        CFrequency read = Assert.IsType<CFrequency>(frequency);
        Assert.Equal(CFrequencyTier.CFrequencyTierUnknown, read.CFrequencyRank);
        Assert.Equal("Frequency.Unknown", read.CFrequencyKey);
        Assert.Equal(0, read.CFrequencySpare);
        Assert.InRange(read.CFrequencyBand, 0, int.MaxValue);
        Assert.Equal("src", read.CFrequencySource);
    }

    [Fact]
    public void DisplayFrequencyRead_NoGauge_ReadsNone()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDisplay display = TInterfaceConductSound.TDisplayChosenCreate(
            engine, TInterfaceConduct.TEntryBundleCreate(engine, TFrequencyPortCreate(null)), 1);

        Assert.Null(display.CDisplayFrequencyRead(static key => key));
    }

    private static LPronunciationPort TFrequencyPortCreate(LFrequencyGauge? gauge) =>
        TEngineFake.TEngineCreate<LPronunciationPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineFrequencyResolve"] = _ => gauge,
        });

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

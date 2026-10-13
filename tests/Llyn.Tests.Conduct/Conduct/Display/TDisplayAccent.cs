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

public sealed class TDisplayAccent
{
    [Fact]
    public void DisplayAccentRead_ShownEntry_AnswersTheBlockReadyToShow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = TDisplayEntrySave(engine, "English", "ˈwɔːtə");
        CWing wing = TDisplayWingPrepare(atelier, []);
        wing.CWingEntryOpen(water.LEntryId);

        CLecternAccent accent = wing.CWingDisplay.CDisplayAccent.CDisplayAccentRead();

        Assert.Equal(new CRespellingMark(false, "[", "]"), accent.CLecternAccentMark);
        Assert.Empty(accent.CLecternAccentContour);
        Assert.Equal("ˈwɔːtə", accent.CLecternAccentText);
        Assert.True(accent.CLecternAccentSpoken);
        Assert.Equal(CVariety.CVarietyRead("English", "British"), accent.CLecternAccentPrimary);
        CAccent row = Assert.Single(accent.CLecternAccentRows);
        Assert.Equal("ˈwɑːtɚ", row.CAccentText);
        Assert.Equal(CVariety.CVarietyRead("English", "American"), row.CAccentVariety);
        Assert.True(accent.CLecternAccentFlagged);
    }

    [Fact]
    public void DisplayAccentRead_RespellingShown_PrintsEachReadingRespelled()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        engine.TEngineRespellingSave(true);
        LEntry weight = TDisplayEntrySave(engine, "English", "ˈweɪtə");
        CWing wing = TDisplayWingPrepare(atelier, []);
        wing.CWingEntryOpen(weight.LEntryId);
        IReadOnlyList<LPronunciationDraft> stored =
            wing.CWingDisplay.LDisplayRule.LDisplaySound.LDisplayShown!.LEntryDraftPronunciations;

        CLecternAccent accent = wing.CWingDisplay.CDisplayAccent.CDisplayAccentRead();

        Assert.Equal(new CRespellingMark(true, "[", "]"), accent.CLecternAccentMark);
        Assert.NotEqual("ˈweɪtə", accent.CLecternAccentText);
        Assert.Equal(stored[0].LPronunciationDraftRespelling, accent.CLecternAccentText);
    }

    [Fact]
    public void DisplayAccentRead_NothingShown_AnswersTheMuteBlock()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CWing wing = TDisplayWingPrepare(atelier, []);

        CLecternAccent accent = wing.CWingDisplay.CDisplayAccent.CDisplayAccentRead();

        Assert.Empty(accent.CLecternAccentText);
        Assert.False(accent.CLecternAccentSpoken);
        Assert.Empty(accent.CLecternAccentRows);
        Assert.False(accent.CLecternAccentFlagged);
    }

    [Fact]
    public async Task DisplayAccentLoad_FlaggedPack_StoresTheVarietyFlagsAndAnswersTheBlock()
    {
        using TLanguageFixture pack = TDisplayPackCreate();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry hill = TDisplayEntrySave(engine, pack.TLanguageFixtureName, "a˥");
        CWing wing = TDisplayWingPrepare(atelier, []);
        wing.CWingEntryOpen(hill.LEntryId);
        List<string> stored = [];

        CLecternAccent? accent = await wing.CWingDisplay.CDisplayAccent.TDisplayAccentLoad((rows, _) =>
        {
            stored.AddRange(rows.Select(static row => row.CEnsignRowKey));
            return static () => { };
        });

        Assert.Equal([pack.TLanguageFixtureName + "/British"], stored);
        Assert.NotNull(accent);
        Assert.True(accent.CLecternAccentFlagged);
        CContour syllable = Assert.Single(accent.CLecternAccentContour);
        Assert.Equal("a˥", syllable.CContourText);
        Assert.Equal([5], syllable.CContourLevels);
        Assert.Equal(
            CVariety.CVarietyRead(pack.TLanguageFixtureName, "British"), accent.CLecternAccentPrimary);
    }

    [Fact]
    public async Task DisplayAccentLoad_AnotherEntryShownMeanwhile_AnswersNothing()
    {
        using TLanguageFixture pack = TDisplayPackCreate();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry hill = TDisplayEntrySave(engine, pack.TLanguageFixtureName, "a˥");
        LEntry dale = TDisplayEntrySave(engine, pack.TLanguageFixtureName, "b˩");
        CWing wing = TDisplayWingPrepare(atelier, []);
        wing.CWingEntryOpen(hill.LEntryId);

        CLecternAccent? accent = await wing.CWingDisplay.CDisplayAccent.TDisplayAccentLoad((_, _) =>
        {
            wing.CWingEntryOpen(dale.LEntryId);
            return static () => { };
        });

        Assert.Null(accent);
    }

    [Fact]
    public async Task DisplayAccentLoad_StoreFails_ShowsTheLoadFailureOnce()
    {
        using TLanguageFixture pack = TDisplayPackCreate();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry hill = TDisplayEntrySave(engine, pack.TLanguageFixtureName, "a˥");
        List<string> asked = [];
        CWing wing = TDisplayWingPrepare(atelier, asked);
        wing.CWingEntryOpen(hill.LEntryId);

        CLecternAccent? accent = await wing.CWingDisplay.CDisplayAccent.TDisplayAccentLoad(
            (_, _) => throw new InvalidOperationException("The flags cannot be stored."));

        Assert.Null(accent);
        Assert.Equal(["Sound.LoadFailed"], asked);
    }

    [Fact]
    public async Task DisplayAccentLoad_NothingShown_AnswersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CWing wing = TDisplayWingPrepare(atelier, []);

        CLecternAccent? accent = await wing.CWingDisplay.CDisplayAccent.TDisplayAccentLoad(
            (_, _) => throw new InvalidOperationException("Nothing is shown, so nothing loads."));

        Assert.Null(accent);
    }

    internal static TLanguageFixture TDisplayPackCreate()
    {
        TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate("{}");
        string flag = Path.Combine(AppContext.BaseDirectory, "languages", pack.TLanguageFixtureName, "hill.svg");
        pack.TLanguageFixtureSave("hill.svg", "<svg xmlns=\"http://www.w3.org/2000/svg\"/>");
        pack.TLanguageFixtureSave(
            "source.json",
            "{ \"tonal\": true, \"varieties\": { \"shown\": \"flag\", \"list\": [ { \"name\": \"British\", \"flag\": \""
            + flag.Replace('\\', '/')
            + "\" } ] } }");
        return pack;
    }

    private static CWing TDisplayWingPrepare(CAtelier atelier, List<string> asked)
    {
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, asked), true);
        return wing;
    }

    private static LEntry TDisplayEntrySave(LEngine engine, string language, string reading) =>
        engine.TEngineEntrySave(
            TInterface.TEntryDraftCreate(
                reading, language, string.Empty, string.Empty, [TInterface.TCardCreate("a thing", 1)], []) with
            {
                LEntryDraftPronunciations =
                [
                    TInterface.TPronunciationDraftCreate(reading, "British"),
                    TInterface.TPronunciationDraftCreate(reading.Replace('ɔ', 'ɑ').Replace("ə", "ɚ"), "American"),
                ],
            });
}

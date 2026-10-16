using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TScoreboard
{
    [Fact]
    public void ScoreboardRead_NoDeck_ReturnsAllRowOnly()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CScoreboard scoreboard = new(atelier, TEnvoyFake.TEnvoyCreate(false, []));

        CScoreboardRoll roll = scoreboard.CScoreboardRead();

        Assert.Equal(new CScoreboardRow(null, null, string.Empty, true), Assert.Single(roll.CScoreboardRollDecks));
        Assert.Equal("Scoreboard.Empty", scoreboard.CScoreboardEmptyKey);
    }

    [Fact]
    public void ScoreboardRead_Decks_ReturnsAllRowFirst()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long folder = port.TEngineFolderCreate("Words");
        long verbs = port.TEngineDeckCreate("Verbs", null, LGrain.LGrainMeaning);
        long nouns = port.TEngineDeckCreate("Nouns", folder, LGrain.LGrainMeaning);
        CScoreboard scoreboard = new(atelier, TEnvoyFake.TEnvoyCreate(false, []));

        CScoreboardRoll roll = scoreboard.CScoreboardRead();

        Assert.Equal(folder, Assert.Single(roll.CScoreboardRollFolders).CDeckFolderId);
        Assert.Equal(new CScoreboardRow(null, null, string.Empty, true), roll.CScoreboardRollDecks[0]);
        Assert.Equal(new CScoreboardRow(verbs, null, "Verbs", false), Assert.Single(
            roll.CScoreboardRollDecks, row => row.CScoreboardRowId == verbs));
        Assert.Equal(new CScoreboardRow(nouns, folder, "Nouns", false), Assert.Single(
            roll.CScoreboardRollDecks, row => row.CScoreboardRowId == nouns));
    }

    [Fact]
    public void ScoreboardRead_Throws_ShowsLoadFailed()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierFaultCreate(engine, "LDeckPort.LEngineDeckRead", false);
        List<string> asked = [];
        CScoreboard scoreboard = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));

        CScoreboardRoll roll = scoreboard.CScoreboardRead();

        Assert.Empty(roll.CScoreboardRollDecks);
        Assert.Equal(["Deck.LoadFailed"], asked);
    }

    [Fact]
    public void ScoreboardSelect_Deck_MarksChosenRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        port.TEngineDeckCreate("Verbs", null, LGrain.LGrainMeaning);
        long second = port.TEngineDeckCreate("Nouns", null, LGrain.LGrainMeaning);
        CScoreboard scoreboard = new(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        int changed = 0;
        scoreboard.CScoreboardChanged += () => changed++;

        scoreboard.CScoreboardSelect(second);

        CScoreboardRow chosen = Assert.Single(
            scoreboard.CScoreboardRead().CScoreboardRollDecks, static row => row.CScoreboardRowChosen);
        Assert.Equal(second, chosen.CScoreboardRowId);
        Assert.Equal(1, changed);
    }

    [Fact]
    public void ScoreboardSelect_All_MarksAllRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long deck = TInterface.TDeckOutletCreate(engine).TEngineDeckCreate("Verbs", null, LGrain.LGrainMeaning);
        CScoreboard scoreboard = new(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        scoreboard.CScoreboardSelect(deck);

        scoreboard.CScoreboardSelect(null);

        CScoreboardRow chosen = Assert.Single(
            scoreboard.CScoreboardRead().CScoreboardRollDecks, static row => row.CScoreboardRowChosen);
        Assert.Null(chosen.CScoreboardRowId);
    }

    [Fact]
    public void AtelierOpen_ScoreboardDeckChosen_ResetsToAll()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long deck = TInterface.TDeckOutletCreate(engine).TEngineDeckCreate("Verbs", null, LGrain.LGrainMeaning);
        CScoreboard scoreboard = new(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        scoreboard.CScoreboardSelect(deck);
        int changed = 0;
        scoreboard.CScoreboardChanged += () => changed++;

        atelier.TAtelierStubOpen();

        CScoreboardRow chosen = Assert.Single(
            scoreboard.CScoreboardRead().CScoreboardRollDecks, static row => row.CScoreboardRowChosen);
        Assert.Null(chosen.CScoreboardRowId);
        Assert.True(changed > 0);
    }

    [Fact]
    public void ScoreboardCensusRead_AllDecksWithoutCards_ReturnsScoreboardEmpty()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TInterface.TDeckOutletCreate(engine).TEngineDeckCreate("Verbs", null, LGrain.LGrainMeaning);
        CScoreboard scoreboard = new(atelier, TEnvoyFake.TEnvoyCreate(false, []));

        CScoreboardCensus census = Assert.IsType<CScoreboardCensus>(scoreboard.CScoreboardCensusRead());

        Assert.Equal("Scoreboard.Empty", census.CScoreboardCensusEmpty);
    }

    [Fact]
    public void ScoreboardCensusRead_DeckWithoutCards_ReturnsVacantKey()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long deck = TInterface.TDeckOutletCreate(engine).TEngineDeckCreate("Verbs", null, LGrain.LGrainMeaning);
        CScoreboard scoreboard = new(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        scoreboard.CScoreboardSelect(deck);

        CScoreboardCensus census = Assert.IsType<CScoreboardCensus>(scoreboard.CScoreboardCensusRead());

        Assert.Equal("0", census.CScoreboardCensusTotal);
        Assert.Equal("Scoreboard.Vacant", census.CScoreboardCensusEmpty);
    }

    [Fact]
    public void ScoreboardCensusRead_Cards_ReturnsWordedCounts()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long deck = TScoreboardDeckCreate(engine, "one", "two");
        TScoreboardDeckCreate(engine, "three");
        CScoreboard scoreboard = new(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        scoreboard.CScoreboardSelect(deck);

        CScoreboardCensus census = Assert.IsType<CScoreboardCensus>(scoreboard.CScoreboardCensusRead());

        Assert.Equal(
            ["2", "2", "0", "0", "0"],
            [
                census.CScoreboardCensusTotal,
                census.CScoreboardCensusActive,
                census.CScoreboardCensusExcluded,
                census.CScoreboardCensusOrphaned,
                census.CScoreboardCensusFlagged,
            ]);
        Assert.Equal(12, census.CScoreboardCensusWeeks.Count);
        CScoreboardWeek current = census.CScoreboardCensusWeeks[^1];
        Assert.Equal((2, "2"), (current.CScoreboardWeekAdded, current.CScoreboardWeekTally));
        Assert.All(census.CScoreboardCensusWeeks, static week => Assert.NotEmpty(week.CScoreboardWeekMonday));
        Assert.Null(census.CScoreboardCensusEmpty);
    }

    [Fact]
    public void ScoreboardCensusRead_KoreanMachineCulture_WordsWeeksInInterfaceLanguage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TScoreboardDeckCreate(engine, "one");
        CScoreboard scoreboard = new(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        CultureInfo machine = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ko-KR");
        CScoreboardCensus census;
        try
        {
            census = Assert.IsType<CScoreboardCensus>(scoreboard.CScoreboardCensusRead());
        }
        finally
        {
            CultureInfo.CurrentCulture = machine;
        }

        string[] months = CultureInfo.GetCultureInfo("en").DateTimeFormat.MonthNames;
        Assert.All(
            census.CScoreboardCensusWeeks,
            week => Assert.Contains(
                months,
                month => month.Length != 0 && week.CScoreboardWeekMonday.StartsWith(month, StringComparison.Ordinal)));
    }

    [Fact]
    public void ScoreboardCensusRead_AllDecks_SumsEveryDeck()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TScoreboardDeckCreate(engine, "one", "two");
        TScoreboardDeckCreate(engine, "three");
        CScoreboard scoreboard = new(atelier, TEnvoyFake.TEnvoyCreate(false, []));

        CScoreboardCensus census = Assert.IsType<CScoreboardCensus>(scoreboard.CScoreboardCensusRead());

        Assert.Equal("3", census.CScoreboardCensusTotal);
        Assert.Equal(3, census.CScoreboardCensusWeeks.Sum(static week => week.CScoreboardWeekAdded));
    }

    [Fact]
    public void ScoreboardCensusRead_NeverSynced_ReturnsUnsynced()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TScoreboardDeckCreate(engine, "one");
        CScoreboard scoreboard = new(atelier, TEnvoyFake.TEnvoyCreate(false, []));

        CScoreboardCensus census = Assert.IsType<CScoreboardCensus>(scoreboard.CScoreboardCensusRead());

        Assert.Equal(engine.TEngineTextRead("Scoreboard.Unsynced"), census.CScoreboardCensusSynced);
    }

    [Fact]
    public void ScoreboardCensusRead_Throws_ShowsLoadFailed()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierFaultCreate(engine, "LDeckPort.LEngineCensusRead", false);
        List<string> asked = [];
        CScoreboard scoreboard = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));

        Assert.Null(scoreboard.CScoreboardCensusRead());
        Assert.Equal(["Deck.LoadFailed"], asked);
    }

    [Fact]
    public void ScoreboardCensusRead_AccessionShownAdd_ReadsTotal()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TAccession.TAccessionEntrySave(workspace, "가다", "오다");
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CScoreboard scoreboard = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        CScoreboardRoll? roll = null;
        CScoreboardCensus? census = null;
        scoreboard.CScoreboardChanged += () => roll = scoreboard.CScoreboardRead();
        scoreboard.CScoreboardChanged += () => census = scoreboard.CScoreboardCensusRead();
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCoinageCreate("Korean verbs", null, asked));

        deck.CDeckCreate(null);
        deck.CDeckAccession.CAccessionRead();
        deck.CDeckAccession.CAccessionShownAdd();

        Assert.Equal(
            ["", "Korean verbs"],
            Assert.IsType<CScoreboardRoll>(roll).CScoreboardRollDecks.Select(static row => row.CScoreboardRowName));
        Assert.Equal("2", Assert.IsType<CScoreboardCensus>(census).CScoreboardCensusTotal);
        Assert.Null(census.CScoreboardCensusEmpty);
    }

    [Fact]
    public void ScoreboardCensusRead_EachDeckWrite_RereadsOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CScoreboard scoreboard = new(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        int changed = 0;
        scoreboard.CScoreboardChanged += () => changed++;

        IReadOnlyList<int> raised = TStudy.TStudyWriteRead(atelier, engine, () => changed);

        Assert.Equal(Enumerable.Repeat(1, 12), raised);
    }

    [Fact]
    public void ScoreboardCensusRead_HeldEntryDelete_RereadsOrphaned()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        List<long> entries = TAccession.TAccessionEntrySave(workspace, "bank", "river");
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long stored = TInterface.TDeckOutletCreate(engine).TEngineDeckCreate("Verbs", null, LGrain.LGrainEntry);
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        deck.CDeckSelect(stored);
        deck.CDeckAccession.CAccessionAdd(entries[0]);
        CScoreboard scoreboard = new(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        List<string?> orphaned = [];
        scoreboard.CScoreboardChanged +=
            () => orphaned.Add(scoreboard.CScoreboardCensusRead()?.CScoreboardCensusOrphaned);

        _ = engine.TEngineEntryDelete(entries[1]);
        _ = engine.TEngineEntryDelete(entries[0]);

        Assert.Equal(["1"], orphaned);
    }

    private static long TScoreboardDeckCreate(LEngine engine, params string[] fronts)
    {
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long deck = port.TEngineDeckCreate("Scoreboard", null, LGrain.LGrainMeaning);
        foreach (string front in fronts)
        {
            port.TEngineCueCreate(
                deck,
                [
                    TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 1, front),
                    TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 2, front + " back"),
                ]);
        }

        return deck;
    }
}

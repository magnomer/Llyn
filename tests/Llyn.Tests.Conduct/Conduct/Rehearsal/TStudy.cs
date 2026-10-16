using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TStudy
{
    [Fact]
    public void StudyRead_NoDeck_ReturnsStudyEmpty()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CStudy study = new(atelier, TEnvoyFake.TEnvoyCreate(false, []), new Random(7).Next);

        CStudyRoll roll = study.CStudyRead();

        Assert.Empty(roll.CStudyRollDecks);
        Assert.Equal("Study.Empty", roll.CStudyRollEmpty);
        Assert.Equal("Study.Empty", study.CStudyEmptyKey);
    }

    [Fact]
    public void StudyRead_NoChosenDeck_ReturnsUnchosenKey()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TInterface.TDeckOutletCreate(engine).TEngineDeckCreate("Verbs", null, LGrain.LGrainMeaning);
        CStudy study = new(atelier, TEnvoyFake.TEnvoyCreate(false, []), new Random(7).Next);

        Assert.Equal("Study.Unchosen", study.CStudyRead().CStudyRollEmpty);
    }

    [Fact]
    public void StudyRead_DeckWithoutCards_ReturnsInactiveKey()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long deck = TInterface.TDeckOutletCreate(engine).TEngineDeckCreate("Verbs", null, LGrain.LGrainMeaning);
        CStudy study = new(atelier, TEnvoyFake.TEnvoyCreate(false, []), new Random(7).Next);
        study.CStudySelect(deck);

        Assert.Equal("Study.Inactive", study.CStudyRead().CStudyRollEmpty);
    }

    [Fact]
    public void StudyRead_ActiveCards_ReturnsCounts()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long folder = port.TEngineFolderCreate("Words");
        long deck = TStudyDeckCreate(engine, "one", "two");
        long empty = port.TEngineDeckCreate("Empty", folder, LGrain.LGrainMeaning);
        CStudy study = new(atelier, TEnvoyFake.TEnvoyCreate(false, []), new Random(7).Next);
        study.CStudySelect(deck);

        CStudyRoll roll = study.CStudyRead();

        Assert.Equal(folder, Assert.Single(roll.CStudyRollFolders).CDeckFolderId);
        Assert.Equal(2, Assert.Single(roll.CStudyRollDecks, row => row.CStudyRowId == deck).CStudyRowActive);
        CStudyRow idle = Assert.Single(roll.CStudyRollDecks, row => row.CStudyRowId == empty);
        Assert.Equal(0, idle.CStudyRowActive);
        Assert.Equal(folder, idle.CStudyRowFolder);
        Assert.Null(roll.CStudyRollEmpty);
    }

    [Fact]
    public void StudySelect_Deck_MarksChosenRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        port.TEngineDeckCreate("Verbs", null, LGrain.LGrainMeaning);
        long second = port.TEngineDeckCreate("Nouns", null, LGrain.LGrainMeaning);
        CStudy study = new(atelier, TEnvoyFake.TEnvoyCreate(false, []), new Random(7).Next);
        int changed = 0;
        study.CStudyChanged += () => changed++;

        study.CStudySelect(second);

        CStudyRow chosen = Assert.Single(study.CStudyRead().CStudyRollDecks, static row => row.CStudyRowChosen);
        Assert.Equal(second, chosen.CStudyRowId);
        Assert.Equal("Nouns", chosen.CStudyRowName);
        Assert.Equal(1, changed);
    }

    [Fact]
    public void StudyStart_ChosenDeck_ReadsFirstCard()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long deck = TStudyDeckCreate(engine, "one", "two", "three");
        CStudy study = new(atelier, TEnvoyFake.TEnvoyCreate(false, []), new Random(7).Next);
        study.CStudySelect(deck);

        study.CStudyStart();

        CSitting sitting = Assert.IsType<CSitting>(study.CStudySitting);
        Assert.Equal(("1 / 3", "one", "one back", false, false, false, (string?)null),
            TSittingRead(sitting.CSittingRead()));
    }

    [Fact]
    public void StudyStart_NoActiveCard_StartsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long deck = TInterface.TDeckOutletCreate(engine).TEngineDeckCreate("Verbs", null, LGrain.LGrainMeaning);
        CStudy study = new(atelier, TEnvoyFake.TEnvoyCreate(false, []), new Random(7).Next);
        study.CStudyStart();
        study.CStudySelect(deck);

        study.CStudyStart();

        Assert.Null(study.CStudySitting);
    }

    [Fact]
    public void StudyFinish_RunningSitting_ClearsSitting()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long deck = TStudyDeckCreate(engine, "one");
        CStudy study = new(atelier, TEnvoyFake.TEnvoyCreate(false, []), new Random(7).Next);
        study.CStudySelect(deck);
        study.CStudyStart();
        int changed = 0;
        study.CStudyChanged += () => changed++;

        study.CStudyFinish();
        study.CStudyFinish();

        Assert.Null(study.CStudySitting);
        Assert.Equal(1, changed);
    }

    [Fact]
    public void SittingSideToggle_FrontShown_TurnsCard()
    {
        CSitting sitting = TSittingStart("one", "two");

        Assert.True(sitting.CSittingSideToggle().CSittingStateTurned);
        Assert.False(sitting.CSittingSideToggle().CSittingStateTurned);
    }

    [Fact]
    public void SittingSideToggle_EmptyBack_ReturnsBlankKey()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long deck = port.TEngineDeckCreate("Study", null, LGrain.LGrainMeaning);
        port.TEngineCueCreate(deck, [TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 1, "one")]);
        CStudy study = new(atelier, TEnvoyFake.TEnvoyCreate(false, []), new Random(7).Next);
        study.CStudySelect(deck);
        study.CStudyStart();
        CSitting sitting = Assert.IsType<CSitting>(study.CStudySitting);

        Assert.Null(sitting.CSittingRead().CSittingStateBlank);
        Assert.Equal("Study.BackEmpty", sitting.CSittingSideToggle().CSittingStateBlank);
    }

    [Fact]
    public void SittingAdvance_TurnedCard_ShowsNextUnturned()
    {
        CSitting sitting = TSittingStart("one", "two");
        sitting.CSittingSideToggle();

        CSittingState state = sitting.CSittingAdvance();

        Assert.Equal(("2 / 2", "two", "two back", false, false, false, (string?)null),
            TSittingRead(state));
    }

    [Fact]
    public void SittingAdvance_LastCard_ReachesEnd()
    {
        CSitting sitting = TSittingStart("one", "two");
        sitting.CSittingAdvance();

        sitting.CSittingAdvance();
        CSittingState state = sitting.CSittingAdvance();

        Assert.Equal(("2 / 2", string.Empty, string.Empty, false, true, false, (string?)null),
            TSittingRead(state));
        Assert.Equal("two", sitting.CSittingRetreat().CSittingStateFront.CSittingSideText);
    }

    [Fact]
    public void SittingRetreat_FirstCard_KeepsPosition()
    {
        CSitting sitting = TSittingStart("one", "two");
        sitting.CSittingAdvance();
        sitting.CSittingSideToggle();

        Assert.Equal(("1 / 2", "one", "one back", false, false, false, (string?)null),
            TSittingRead(sitting.CSittingRetreat()));
        Assert.Equal("1 / 2", sitting.CSittingRetreat().CSittingStatePosition);
    }

    [Fact]
    public void SittingShuffleToggle_FiveCards_KeepsPermutation()
    {
        string[] fronts = ["one", "two", "three", "four", "five"];
        CSitting sitting = TSittingStart(fronts);
        sitting.CSittingAdvance();
        sitting.CSittingSideToggle();

        CSittingState state = sitting.CSittingShuffleToggle();

        Assert.True(state.CSittingStateShuffled);
        Assert.Equal("1 / 5", state.CSittingStatePosition);
        Assert.False(state.CSittingStateTurned);
        Assert.Equal(fronts.Order(), TSittingFrontRead(sitting, state).Order());
    }

    [Fact]
    public void SittingShuffleToggle_Twice_RestoresDeckOrder()
    {
        string[] fronts = ["one", "two", "three", "four", "five"];
        CSitting sitting = TSittingStart(fronts);
        sitting.CSittingShuffleToggle();

        CSittingState state = sitting.CSittingShuffleToggle();

        Assert.False(state.CSittingStateShuffled);
        Assert.Equal(fronts, TSittingFrontRead(sitting, state));
    }

    private static List<string> TSittingFrontRead(CSitting sitting, CSittingState state)
    {
        List<string> seen = [];
        while (!state.CSittingStateEnd)
        {
            seen.Add(state.CSittingStateFront.CSittingSideText);
            Assert.Equal(state.CSittingStateFront.CSittingSideText + " back", state.CSittingStateBack.CSittingSideText);
            state = sitting.CSittingAdvance();
        }

        return seen;
    }

    private static (string, string, string, bool, bool, bool, string?) TSittingRead(CSittingState state)
    {
        return (state.CSittingStatePosition, state.CSittingStateFront.CSittingSideText,
            state.CSittingStateBack.CSittingSideText, state.CSittingStateTurned, state.CSittingStateEnd,
            state.CSittingStateShuffled, state.CSittingStateBlank);
    }

    private static CSitting TSittingStart(params string[] fronts)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CStudy study = new(atelier, TEnvoyFake.TEnvoyCreate(false, []), new Random(7).Next);
        study.CStudySelect(TStudyDeckCreate(engine, fronts));
        study.CStudyStart();
        return Assert.IsType<CSitting>(study.CStudySitting);
    }

    [Fact]
    public void StudyRead_AccessionShownAdd_ReadsActiveDeck()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TAccession.TAccessionEntrySave(workspace, "가다", "오다");
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CStudy study = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked), new Random(7).Next);
        CStudyRoll? shown = null;
        study.CStudyChanged += () => shown = study.CStudyRead();
        CDeck deck = new(atelier, TEnvoyFake.TEnvoyCoinageCreate("Korean verbs", null, asked));

        deck.CDeckCreate(null);
        deck.CDeckAccession.CAccessionRead();
        deck.CDeckAccession.CAccessionShownAdd();

        CStudyRow row = Assert.Single(Assert.IsType<CStudyRoll>(shown).CStudyRollDecks);
        Assert.Equal("Korean verbs", row.CStudyRowName);
        Assert.Equal(2, row.CStudyRowActive);
    }

    [Fact]
    public void StudyRead_EachDeckWrite_RereadsOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CStudy study = new(atelier, TEnvoyFake.TEnvoyCreate(false, asked), new Random(7).Next);
        int changed = 0;
        study.CStudyChanged += () => changed++;

        IReadOnlyList<int> raised = TStudyWriteRead(atelier, engine, () => changed);

        Assert.Equal(Enumerable.Repeat(1, 12), raised);
    }

    internal static IReadOnlyList<int> TStudyWriteRead(CAtelier atelier, LEngine engine, Func<int> changed)
    {
        List<string> asked = [];
        CDeck coiner = new(atelier, TEnvoyFake.TEnvoyCoinageCreate("Verbs", null, asked));
        CDeck remover = new(atelier, TEnvoyFake.TEnvoyCreate(true, asked));
        List<int> raised = [];
        long deck = 0;
        long folder = 0;
        long cue = 0;
        foreach (Action write in (Action[])
                 [
                     () => coiner.CDeckCreate(null),
                     () => coiner.CDeckFolderCreate(),
                     () => coiner.CDeckNameSet(deck, "Nouns"),
                     () => coiner.CDeckFolderUpdate(folder, "Words"),
                     () => coiner.CDeckMove(Assert.Single(coiner.CDeckDestinationRead(deck))),
                     () => coiner.CDeckCue.CCueCreate(),
                     () => coiner.CDeckCue.CCueMove(cue, 0),
                     () => coiner.CDeckCue.CCueSet(cue, true, false),
                     () => coiner.CDeckCue.CCueSet(cue, true, true),
                     () => remover.CDeckCue.CCueRemove([cue]),
                     () => remover.CDeckDelete(deck),
                     () => remover.CDeckFolderDelete(folder),
                 ])
        {
            int before = changed();
            write();
            raised.Add(changed() - before);
            CDeckRoll roll = coiner.CDeckRead();
            deck = roll.CDeckRollDecks.Select(static row => row.CDeckRowId).FirstOrDefault();
            folder = roll.CDeckRollFolders.Select(static row => row.CDeckFolderId).FirstOrDefault();
            cue = deck == 0 ? 0 : TInterface.TDeckOutletCreate(engine).TEngineCueRead(deck)
                .Select(static row => row.LCueId).FirstOrDefault();
        }

        return raised;
    }

    private static long TStudyDeckCreate(LEngine engine, params string[] fronts)
    {
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long deck = port.TEngineDeckCreate("Study", null, LGrain.LGrainMeaning);
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

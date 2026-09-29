using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCorpus
{
    [Fact]
    public void CorpusExampleCreate_NoRowChosen_StartsABlankTranscript()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CCorpus corpus = TCorpusPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        List<CExample?> held = [];
        corpus.CCorpusTranscriptChanged += held.Add;

        corpus.CCorpusExampleCreate();

        Assert.True(corpus.CCorpusDesk.CDeskHeld);
        Assert.True(corpus.CCorpusTranscriptShown);
        Assert.True(corpus.CCorpusScribeChecked);
        Assert.NotNull(held[^1]);
        Assert.NotNull(corpus.TCorpusTranscriptRead());
    }

    [Fact]
    public void CorpusExampleCreate_ExampleChosen_OpensAQuotingEntryInTheEditor()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample cat = TCorpusExampleSave(engine, "a cat sat");
        CCorpus corpus = TCorpusPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        corpus.TCorpusExampleOpen(cat.LExampleId);
        List<CEntryDraft> shown = [];
        corpus.CCorpusEditor.CEditorDraftChanged += shown.Add;

        corpus.CCorpusExampleCreate();

        Assert.True(corpus.CCorpusEditorShown);
        Assert.False(corpus.CCorpusExcerptShown);
        Assert.False(corpus.CCorpusDesk.CDeskHeld);
        Assert.Equal(cat.LExampleId, corpus.CCorpusAnthology.CAnthologyChosen);
        Assert.Equal(
            "a cat sat",
            shown[0].CEntryDraftMeanings[0].CCardDraftSentence[0].CSentenceDraftExample?.CExampleDraftText
                .CStateValueText);
    }

    [Fact]
    public void CorpusExampleOpen_StoredExample_ShowsItOnTheExcerpt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample cat = TCorpusExampleSave(engine, "a cat sat");
        CCorpus corpus = TCorpusPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        List<CExample> shown = [];
        corpus.CCorpusExampleChanged += shown.Add;

        corpus.TCorpusExampleOpen(cat.LExampleId);

        Assert.True(corpus.CCorpusExcerptShown);
        Assert.True(corpus.CCorpusExcerptHeld);
        Assert.True(corpus.CCorpusPressAllowed);
        Assert.False(corpus.CCorpusPortraitAllowed);
        Assert.Equal("a cat sat", Assert.Single(shown).CExampleText.CStateValueText);
    }

    [Fact]
    public void CorpusExampleOpen_HiddenByTheQuery_DropsTheQueryAndShowsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample cat = TCorpusExampleSave(engine, "a cat sat");
        TCorpusExampleSave(engine, "a dog ran");
        CCorpus corpus = TCorpusPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        corpus.CCorpusAnthology.CAnthologyQuerySet("dog");
        corpus.CCorpusAnthology.CAnthologyPanel.CPanelRowsChanged += () => corpus.CCorpusRowsRead("?", "-");
        int cleared = 0;
        corpus.CCorpusQueryCleared += () => cleared++;

        corpus.TCorpusExampleOpen(cat.LExampleId);

        Assert.Equal(1, cleared);
        Assert.Equal(2, corpus.CCorpusRowsRead("?", "-").Count);
        Assert.Equal(cat.LExampleId, corpus.CCorpusAnthology.CAnthologyChosen);
        Assert.True(corpus.CCorpusExcerptHeld);
    }

    [Fact]
    public void CorpusExampleSelect_NothingUnsaved_RecordsAndShowsTheExample()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample cat = TCorpusExampleSave(engine, "a cat sat");
        List<string> asked = [];
        CCorpus corpus = TCorpusPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, asked));
        int recorded = 0;
        atelier.CAtelierNavigation.CNavigationChanged += _ => recorded++;

        corpus.CCorpusExampleSelect(cat.LExampleId);
        corpus.CCorpusExampleSelect(null);

        Assert.Equal(1, recorded);
        Assert.Empty(asked);
        Assert.Equal(cat.LExampleId, corpus.CCorpusAnthology.CAnthologyChosen);
    }

    [Fact]
    public void CorpusExampleSelect_UnsavedTranscriptKept_RecordsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample cat = TCorpusExampleSave(engine, "a cat sat");
        List<string> asked = [];
        CCorpus corpus = TCorpusPrepare(atelier, TInterfaceConduct.TEnvoyCreate(null, asked));
        corpus.CCorpusExampleCreate();
        corpus.CCorpusDesk.TDeskDefer(
            TInterface.TExampleTextCreate(corpus.CCorpusDesk.CDeskId, TInterface.TStateValueCreate("a dog")));
        int recorded = 0;
        atelier.CAtelierNavigation.CNavigationChanged += _ => recorded++;

        corpus.CCorpusExampleSelect(cat.LExampleId);

        Assert.Equal(0, recorded);
        Assert.Equal(["Leave"], asked);
        Assert.True(corpus.CCorpusTranscriptShown);
        Assert.Null(corpus.CCorpusAnthology.CAnthologyChosen);
    }

    [Fact]
    public void CorpusLeaveConfirm_UnsavedTranscriptDiscarded_LeavesWithoutStoring()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CCorpus corpus = TCorpusPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, asked));

        Assert.True(corpus.CCorpusLeaveConfirm());
        Assert.Empty(asked);

        corpus.CCorpusExampleCreate();
        corpus.CCorpusDesk.TDeskDefer(
            TInterface.TExampleTextCreate(corpus.CCorpusDesk.CDeskId, TInterface.TStateValueCreate("a dog")));

        Assert.True(corpus.CCorpusLeaveConfirm());
        Assert.Equal(["Leave"], asked);
        Assert.Empty(corpus.CCorpusAnthology.TAnthologyRowsRead("?", "-"));
    }

    [Fact]
    public void CorpusScribeToggle_ClosingTheTranscript_CancelsTheDesk()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample cat = TCorpusExampleSave(engine, "a cat sat");
        CCorpus corpus = TCorpusPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        corpus.TCorpusExampleOpen(cat.LExampleId);

        corpus.CCorpusScribeToggle(true);

        Assert.True(corpus.CCorpusTranscriptShown);
        Assert.True(corpus.CCorpusDesk.CDeskHeld);

        corpus.CCorpusScribeToggle(false);

        Assert.True(corpus.CCorpusExcerptShown);
        Assert.False(corpus.CCorpusDesk.CDeskHeld);
        Assert.Equal(cat.LExampleId, corpus.CCorpusAnthology.CAnthologyChosen);
    }

    [Fact]
    public void CorpusScribeToggle_ClosingTheQuotationEditor_FallsBackToTheChosenExample()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample cat = TCorpusExampleSave(engine, "a cat sat");
        CCorpus corpus = TCorpusPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        corpus.TCorpusExampleOpen(cat.LExampleId);
        corpus.CCorpusExampleCreate();

        corpus.CCorpusScribeToggle(false);

        Assert.False(corpus.CCorpusEditorShown);
        Assert.True(corpus.CCorpusExcerptShown);
        Assert.Equal(cat.LExampleId, corpus.CCorpusAnthology.CAnthologyChosen);
    }

    [Fact]
    public void CorpusQuotationSelect_QuotingEntry_ShowsItOnTheDisplay()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample cat = TCorpusExampleSave(engine, "a cat sat");
        LEntry water = TCorpusEntrySave(engine);
        engine.TRequestQuoteApply(water.LEntryId, cat.LExampleId);
        CCorpus corpus = TCorpusPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        corpus.TCorpusExampleOpen(cat.LExampleId);

        corpus.CCorpusQuotationSelect(water.LEntryId);

        Assert.False(corpus.CCorpusExcerptShown);
        Assert.True(corpus.CCorpusDisplayShown);
        Assert.True(corpus.CCorpusPortraitAllowed);
        Assert.False(corpus.CCorpusBinEnabled);
        Assert.Equal(cat.LExampleId, corpus.CCorpusAnthology.CAnthologyChosen);
    }

    [Fact]
    public void CorpusExampleClose_ChosenExample_ClearsBothLists()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample cat = TCorpusExampleSave(engine, "a cat sat");
        CCorpus corpus = TCorpusPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        corpus.TCorpusExampleOpen(cat.LExampleId);

        corpus.CCorpusExampleClose();

        Assert.True(corpus.CCorpusExcerptBlank);
        Assert.False(corpus.CCorpusDisplayShown);
        Assert.Null(corpus.CCorpusAnthology.CAnthologyChosen);
    }

    [Fact]
    public void CorpusEntryResonate_NoChosenQuotation_KeepsTheTranscript()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CCorpus corpus = TCorpusPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        corpus.CCorpusExampleCreate();

        corpus.CCorpusEntryResonate();

        Assert.True(corpus.CCorpusTranscriptShown);
        Assert.True(corpus.CCorpusDesk.CDeskHeld);
    }

    [Fact]
    public void CorpusEntryResonate_QuotationOnDisplay_KeepsTheQuotationSide()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample cat = TCorpusExampleSave(engine, "a cat sat");
        LEntry water = TCorpusEntrySave(engine);
        engine.TRequestQuoteApply(water.LEntryId, cat.LExampleId);
        CCorpus corpus = TCorpusPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        corpus.TCorpusExampleOpen(cat.LExampleId);
        corpus.CCorpusQuotationSelect(water.LEntryId);

        corpus.CCorpusEntryResonate();

        Assert.True(corpus.CCorpusDisplayShown);
        Assert.Equal(cat.LExampleId, corpus.CCorpusAnthology.CAnthologyChosen);
    }

    [Fact]
    public void CorpusLeaveConfirm_UnsavedTranscriptKept_StaysOnTheTranscript()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CCorpus corpus = TCorpusPrepare(atelier, TInterfaceConduct.TEnvoyCreate(null, asked));
        corpus.CCorpusExampleCreate();
        corpus.CCorpusDesk.TDeskDefer(
            TInterface.TExampleTextCreate(corpus.CCorpusDesk.CDeskId, TInterface.TStateValueCreate("a dog")));

        Assert.False(corpus.CCorpusLeaveConfirm());
        Assert.Equal(["Leave"], asked);
        Assert.True(corpus.CCorpusTranscriptShown);
        Assert.True(corpus.CCorpusDesk.CDeskHeld);
    }

    [Fact]
    public void CorpusLeaveConfirm_UnsavedTranscriptStored_StoresTheExample()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CCorpus corpus = TCorpusPrepare(atelier, TInterfaceConduct.TEnvoyCreate(true, []));
        corpus.CCorpusExampleCreate();
        corpus.CCorpusDesk.TDeskDefer(
            TInterface.TExampleTextCreate(corpus.CCorpusDesk.CDeskId, TInterface.TStateValueCreate("a dog")));

        Assert.True(corpus.CCorpusLeaveConfirm());
        Assert.Single(corpus.CCorpusAnthology.TAnthologyRowsRead("?", "-"));
    }

    [Fact]
    public void CorpusSessionSave_FreshTranscript_ShowsTheStoredExampleOnTheExcerpt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CCorpus corpus = TCorpusPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        corpus.CCorpusExampleCreate();
        corpus.CCorpusDesk.TDeskDefer(
            TInterface.TExampleTextCreate(corpus.CCorpusDesk.CDeskId, TInterface.TStateValueCreate("a dog")));

        Assert.True(corpus.CCorpusSession.CSessionSave());

        Assert.True(corpus.CCorpusExcerptShown);
        Assert.True(corpus.CCorpusExcerptHeld);
        Assert.False(corpus.CCorpusScribeChecked);
    }

    [Fact]
    public void CorpusRowsRead_ChosenExampleNarrowedAway_ClearsBothLists()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample cat = TCorpusExampleSave(engine, "a cat sat");
        TCorpusExampleSave(engine, "a dog ran");
        CCorpus corpus = TCorpusPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        corpus.TCorpusExampleOpen(cat.LExampleId);

        Assert.Equal(2, corpus.CCorpusRowsRead("?", "-").Count);
        Assert.True(corpus.CCorpusExcerptHeld);

        corpus.CCorpusAnthology.CAnthologyQuerySet("dog");

        Assert.Single(corpus.CCorpusRowsRead("?", "-"));
        Assert.True(corpus.CCorpusExcerptBlank);
        Assert.Null(corpus.CCorpusAnthology.CAnthologyChosen);
    }

    [Fact]
    public void CorpusExampleDelete_ChosenExampleConfirmed_DeletesItAndClearsTheExcerpt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample cat = TCorpusExampleSave(engine, "a cat sat");
        List<string> asked = [];
        CCorpus corpus = TCorpusPrepare(atelier, TInterfaceConduct.TEnvoyCreate(true, asked));
        corpus.TCorpusExampleOpen(cat.LExampleId);

        corpus.CCorpusExampleDelete();

        Assert.Equal(["Example.DeleteConfirm"], asked);
        Assert.Empty(corpus.CCorpusAnthology.TAnthologyRowsRead("?", "-"));
        Assert.True(corpus.CCorpusExcerptBlank);
    }

    [Fact]
    public void CorpusExampleDelete_QuotationOnDisplay_DeletesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample cat = TCorpusExampleSave(engine, "a cat sat");
        LEntry water = TCorpusEntrySave(engine);
        engine.TRequestQuoteApply(water.LEntryId, cat.LExampleId);
        List<string> asked = [];
        CCorpus corpus = TCorpusPrepare(atelier, TInterfaceConduct.TEnvoyCreate(true, asked));
        corpus.TCorpusExampleOpen(cat.LExampleId);
        corpus.CCorpusQuotationSelect(water.LEntryId);

        corpus.CCorpusExampleDelete();

        Assert.Empty(asked);
        Assert.Single(corpus.CCorpusAnthology.TAnthologyRowsRead("?", "-"));
        Assert.True(corpus.CCorpusDisplayShown);
    }

    [Fact]
    public void AtelierClose_QuotingEntryOnTheEditor_CancelsTheEditorDeskAndStopsPlayback()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        int stopped = 0;
        LMediaPort media = TEngineFake.TEngineCreate<LMediaPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineRecordingStop"] = _ =>
            {
                stopped++;
                return null;
            },
        });
        using CAtelier atelier = TInterfaceConduct.TAtelierMediaCreate(engine, media);
        LExample cat = TCorpusExampleSave(engine, "a cat sat");
        CCorpus corpus = TCorpusPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        corpus.TCorpusExampleOpen(cat.LExampleId);
        corpus.CCorpusExampleCreate();
        Assert.True(corpus.CCorpusEditor.CEditorDesk.CDeskHeld);

        atelier.CAtelierClose();

        Assert.False(corpus.CCorpusEditor.CEditorDesk.CDeskHeld);
        Assert.Equal(1, stopped);
    }

    [Fact]
    public void CorpusCreate_TranscriptEdited_HandsTheHeldExampleThroughTheMarshal()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        engine.TEngineDelaySet(0);
        LExample cat = TCorpusExampleSave(engine, "a cat sat");
        int marshalled = 0;
        CCorpus corpus = CCorpus.CCorpusCreate(
            atelier,
            static () => true,
            TInterfaceConduct.TEnvoyCreate(false, []),
            run =>
            {
                marshalled++;
                run();
            });
        corpus.CCorpusVistaRestore();
        corpus.TCorpusExampleOpen(cat.LExampleId);
        corpus.CCorpusScribeToggle(true);
        List<CExample?> drafted = [];
        corpus.CCorpusDraftChanged += drafted.Add;

        corpus.CCorpusMentionAdd("a cat sat", 2, 3, 0);
        corpus.CCorpusDesk.CDeskPersist();

        Assert.True(marshalled > 0);
        Assert.Equal("a cat sat", drafted[^1]!.CExampleText.CStateValueText);
    }

    internal static CCorpus TCorpusPrepare(CAtelier atelier, CEnvoy envoy)
    {
        CCorpus corpus = CCorpus.CCorpusCreate(atelier, static () => true, envoy, static run => run());
        corpus.CCorpusVistaRestore();
        return corpus;
    }

    internal static LExample TCorpusExampleSave(LEngine engine, string text)
    {
        return engine.TEngineExampleCreate(TInterface.TExampleCreate(
            0, "English", TInterface.TStateValueCreate(text), null, TInterface.TStateAnchorRead(null)));
    }

    internal static LEntry TCorpusEntrySave(LEngine engine)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", "wata", string.Empty, [TInterface.TCardCreate("a liquid", 1)], []));
    }
}

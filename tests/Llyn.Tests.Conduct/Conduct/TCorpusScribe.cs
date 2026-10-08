using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCorpusScribe
{
    [Fact]
    public void CorpusExampleCreate_NoRowChosen_StartsABlankTranscript()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CCorpus corpus = TCorpus.TCorpusPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        List<CExample> held = [];
        corpus.CCorpusTranscriptChanged += held.Add;

        corpus.CCorpusDiptych.CDiptychEntryCreate();

        Assert.True(corpus.CCorpusDesk.CDeskHeld);
        Assert.True(corpus.CCorpusDiptych.CDiptychParentEditing);
        Assert.True(corpus.CCorpusDiptych.CDiptychScribeChecked);
        Assert.NotNull(held[^1]);
        Assert.NotNull(corpus.TCorpusTranscriptRead());
    }

    [Fact]
    public void CorpusExampleSelect_UnsavedTranscriptKept_RecordsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample cat = TCorpus.TCorpusExampleSave(engine, "a cat sat");
        List<string> asked = [];
        CCorpus corpus = TCorpus.TCorpusPrepare(atelier, TEnvoyFake.TEnvoyCreate(null, asked));
        corpus.CCorpusDiptych.CDiptychEntryCreate();
        corpus.CCorpusDesk.TDeskDefer(
            TInterface.TExampleTextCreate(corpus.CCorpusDesk.CDeskId, TInterfaceState.TStateValueCreate("a dog")));
        int recorded = 0;
        atelier.CAtelierNavigation.CNavigationChanged += _ => recorded++;

        corpus.CCorpusDiptych.CDiptychParentSelect(cat.LExampleId);

        Assert.Equal(0, recorded);
        Assert.Equal(["Leave"], asked);
        Assert.True(corpus.CCorpusDiptych.CDiptychParentEditing);
        Assert.Null(corpus.CCorpusAnthology.CAnthologyPanel.CPanelAperture.CApertureChosen);
    }

    [Fact]
    public void CorpusLeaveConfirm_UnsavedTranscriptDiscarded_LeavesWithoutStoring()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CCorpus corpus = TCorpus.TCorpusPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, asked));

        Assert.True(corpus.TCorpusLeaveConfirm());
        Assert.Empty(asked);

        corpus.CCorpusDiptych.CDiptychEntryCreate();
        corpus.CCorpusDesk.TDeskDefer(
            TInterface.TExampleTextCreate(corpus.CCorpusDesk.CDeskId, TInterfaceState.TStateValueCreate("a dog")));

        Assert.True(corpus.TCorpusLeaveConfirm());
        Assert.Equal(["Leave"], asked);
        Assert.Empty(corpus.CCorpusAnthology.TAnthologyRowsRead());
    }

    [Fact]
    public void CorpusLeaveConfirm_UnsavedTranscriptKept_StaysOnTheTranscript()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CCorpus corpus = TCorpus.TCorpusPrepare(atelier, TEnvoyFake.TEnvoyCreate(null, asked));
        corpus.CCorpusDiptych.CDiptychEntryCreate();
        corpus.CCorpusDesk.TDeskDefer(
            TInterface.TExampleTextCreate(corpus.CCorpusDesk.CDeskId, TInterfaceState.TStateValueCreate("a dog")));

        Assert.False(corpus.TCorpusLeaveConfirm());
        Assert.Equal(["Leave"], asked);
        Assert.True(corpus.CCorpusDiptych.CDiptychParentEditing);
        Assert.True(corpus.CCorpusDesk.CDeskHeld);
    }

    [Fact]
    public void CorpusLeaveConfirm_UnsavedTranscriptStored_StoresTheExample()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CCorpus corpus = TCorpus.TCorpusPrepare(atelier, TEnvoyFake.TEnvoyCreate(true, []));
        corpus.CCorpusDiptych.CDiptychEntryCreate();
        corpus.CCorpusDesk.TDeskDefer(
            TInterface.TExampleTextCreate(corpus.CCorpusDesk.CDeskId, TInterfaceState.TStateValueCreate("a dog")));

        Assert.True(corpus.TCorpusLeaveConfirm());
        Assert.Single(corpus.CCorpusAnthology.TAnthologyRowsRead());
    }

    [Fact]
    public void CorpusSessionSave_FreshTranscript_ShowsTheStoredExampleOnTheExcerpt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CCorpus corpus = TCorpus.TCorpusPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        corpus.CCorpusDiptych.CDiptychEntryCreate();
        corpus.CCorpusDesk.TDeskDefer(
            TInterface.TExampleTextCreate(corpus.CCorpusDesk.CDeskId, TInterfaceState.TStateValueCreate("a dog")));

        Assert.True(corpus.CCorpusSession.CSessionSave());

        Assert.True(corpus.CCorpusDiptych.CDiptychParentShown);
        Assert.True(corpus.CCorpusExcerptHeld);
        Assert.False(corpus.CCorpusDiptych.CDiptychScribeChecked);
    }

    [Fact]
    public void CorpusScribeToggle_ClosingTheTranscript_CancelsTheDesk()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample cat = TCorpus.TCorpusExampleSave(engine, "a cat sat");
        CCorpus corpus = TCorpus.TCorpusPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        corpus.TCorpusExampleOpen(cat.LExampleId);

        corpus.CCorpusDiptych.CDiptychScribeToggle(true);

        Assert.True(corpus.CCorpusDiptych.CDiptychParentEditing);
        Assert.True(corpus.CCorpusDesk.CDeskHeld);

        corpus.CCorpusDiptych.CDiptychScribeToggle(false);

        Assert.True(corpus.CCorpusDiptych.CDiptychParentShown);
        Assert.False(corpus.CCorpusDesk.CDeskHeld);
        Assert.Equal(cat.LExampleId, corpus.CCorpusAnthology.CAnthologyPanel.CPanelAperture.CApertureChosen);
    }

    [Fact]
    public void CorpusScribeToggle_ClosingTheQuotationEditor_FallsBackToTheChosenExample()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample cat = TCorpus.TCorpusExampleSave(engine, "a cat sat");
        CCorpus corpus = TCorpus.TCorpusPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        corpus.TCorpusExampleOpen(cat.LExampleId);
        corpus.CCorpusDiptych.CDiptychEntryCreate();

        corpus.CCorpusDiptych.CDiptychScribeToggle(false);

        Assert.False(corpus.CCorpusDiptych.CDiptychChildEditing);
        Assert.True(corpus.CCorpusDiptych.CDiptychParentShown);
        Assert.Equal(cat.LExampleId, corpus.CCorpusAnthology.CAnthologyPanel.CPanelAperture.CApertureChosen);
    }

    [Fact]
    public void CorpusEntryResonate_NoChosenQuotation_KeepsTheTranscript()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CCorpus corpus = TCorpus.TCorpusPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        corpus.CCorpusDiptych.CDiptychEntryCreate();

        corpus.TCorpusEntryResonate();

        Assert.True(corpus.CCorpusDiptych.CDiptychParentEditing);
        Assert.True(corpus.CCorpusDesk.CDeskHeld);
    }
}

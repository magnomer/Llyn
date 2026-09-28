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
        Assert.NotNull(corpus.CCorpusTranscriptRead());
    }

    [Fact]
    public void CorpusExampleCreate_ExampleChosen_OpensAQuotingEntryInTheEditor()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample cat = TCorpusExampleSave(engine, "a cat sat");
        CCorpus corpus = TCorpusPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        corpus.CCorpusExampleOpen(cat.LExampleId);
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

        corpus.CCorpusExampleOpen(cat.LExampleId);

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

        corpus.CCorpusExampleOpen(cat.LExampleId);

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

        corpus.CCorpusExampleSelect(cat.LExampleId, () => recorded++);
        corpus.CCorpusExampleSelect(null, () => recorded++);

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

        corpus.CCorpusExampleSelect(cat.LExampleId, () => recorded++);

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
        corpus.CCorpusExampleOpen(cat.LExampleId);

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
        corpus.CCorpusExampleOpen(cat.LExampleId);
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
        corpus.CCorpusExampleOpen(cat.LExampleId);

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
        corpus.CCorpusExampleOpen(cat.LExampleId);

        corpus.CCorpusExampleClose();

        Assert.True(corpus.CCorpusExcerptBlank);
        Assert.False(corpus.CCorpusDisplayShown);
        Assert.Null(corpus.CCorpusAnthology.CAnthologyChosen);
    }

    [Fact]
    public void CorpusEntryUpdate_NoChosenQuotation_KeepsTheTranscript()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CCorpus corpus = TCorpusPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        corpus.CCorpusExampleCreate();

        corpus.CCorpusEntryUpdate();

        Assert.True(corpus.CCorpusTranscriptShown);
        Assert.True(corpus.CCorpusDesk.CDeskHeld);
    }

    [Fact]
    public void CorpusEntryUpdate_QuotationOnDisplay_KeepsTheQuotationSide()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample cat = TCorpusExampleSave(engine, "a cat sat");
        LEntry water = TCorpusEntrySave(engine);
        engine.TRequestQuoteApply(water.LEntryId, cat.LExampleId);
        CCorpus corpus = TCorpusPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        corpus.CCorpusExampleOpen(cat.LExampleId);
        corpus.CCorpusQuotationSelect(water.LEntryId);

        corpus.CCorpusEntryUpdate();

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
        corpus.CCorpusExampleOpen(cat.LExampleId);

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
        corpus.CCorpusExampleOpen(cat.LExampleId);

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
        corpus.CCorpusExampleOpen(cat.LExampleId);
        corpus.CCorpusQuotationSelect(water.LEntryId);

        corpus.CCorpusExampleDelete();

        Assert.Empty(asked);
        Assert.Single(corpus.CCorpusAnthology.TAnthologyRowsRead("?", "-"));
        Assert.True(corpus.CCorpusDisplayShown);
    }

    [Fact]
    public async Task CorpusPortraitExport_QuotationOnDisplay_WritesTheEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample cat = TCorpusExampleSave(engine, "a cat sat");
        LEntry water = TCorpusEntrySave(engine);
        engine.TRequestQuoteApply(water.LEntryId, cat.LExampleId);
        CCorpus corpus = TCorpusPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        corpus.CCorpusExampleOpen(cat.LExampleId);
        string path = Path.Combine(workspace.TWorkspaceFolder, "water.md");

        await corpus.CCorpusPortraitExport(path, CPortraitMedium.CPortraitMediumMarkdown, TCorpusLabelCreate());

        Assert.False(File.Exists(path));

        corpus.CCorpusQuotationSelect(water.LEntryId);
        await corpus.CCorpusPortraitExport(path, CPortraitMedium.CPortraitMediumMarkdown, TCorpusLabelCreate());

        Assert.Contains("water", File.ReadAllText(path), StringComparison.Ordinal);
    }

    [Fact]
    public void CorpusPortraitPrint_NothingChosen_PrintsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CCorpus corpus = TCorpusPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));

        Task printed = corpus.CCorpusPortraitPrint(TCorpusLabelCreate(), null!, null!);

        Assert.Same(Task.CompletedTask, printed);
        Assert.False(corpus.CCorpusPressAllowed);
    }

    internal static CCorpus TCorpusPrepare(CAtelier atelier, CEnvoy envoy)
    {
        CCorpus corpus = CCorpus.CCorpusCreate(atelier, static () => true, envoy);
        corpus.CCorpusVistaRestore();
        return corpus;
    }

    internal static LExample TCorpusExampleSave(LEngine engine, string text)
    {
        return engine.TEngineExampleCreate(TInterface.TExampleCreate(
            0, "English", TInterface.TStateValueCreate(text), null, TInterface.TStateAnchorRead(null)));
    }

    internal static CPortraitLabel TCorpusLabelCreate()
    {
        string[] words = [.. Enumerable.Range(0, 22).Select(static index => $"word{index}")];
        return (CPortraitLabel)Activator.CreateInstance(typeof(CPortraitLabel), words)!;
    }

    internal static LEntry TCorpusEntrySave(LEngine engine)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", "wata", string.Empty, [TInterface.TCardCreate("a liquid", 1)], []));
    }
}

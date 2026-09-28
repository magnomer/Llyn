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
        CCorpus corpus = TCorpusPrepare(engine, atelier, TInterfaceConduct.TEnvoyCreate(false, []));
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
        CCorpus corpus = TCorpusPrepare(engine, atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        corpus.CCorpusExampleOpen(cat.LExampleId);

        corpus.CCorpusExampleCreate();

        Assert.True(corpus.CCorpusEditorShown);
        Assert.True(corpus.CCorpusQuotationSide);
        Assert.False(corpus.CCorpusDesk.CDeskHeld);
        Assert.Equal(cat.LExampleId, corpus.CCorpusAnthology.CAnthologyChosen);
    }

    [Fact]
    public void CorpusExampleOpen_StoredExample_ShowsItOnTheExcerpt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample cat = TCorpusExampleSave(engine, "a cat sat");
        CCorpus corpus = TCorpusPrepare(engine, atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        List<CExample> shown = [];
        corpus.CCorpusExampleChanged += shown.Add;

        corpus.CCorpusExampleOpen(cat.LExampleId);

        Assert.True(corpus.CCorpusExcerptShown);
        Assert.True(corpus.CCorpusExcerptHeld);
        Assert.True(corpus.CCorpusRowShown);
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
        CCorpus corpus = TCorpusPrepare(engine, atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        corpus.CCorpusAnthology.CAnthologyQuerySet("dog");
        corpus.CCorpusAnthology.CAnthologyPanel.CPanelRowsChanged += () => TCorpusRowsCheck(corpus);
        int cleared = 0;
        corpus.CCorpusQueryCleared += () => cleared++;

        corpus.CCorpusExampleOpen(cat.LExampleId);

        Assert.Equal(1, cleared);
        Assert.False(corpus.CCorpusAnthology.CAnthologyNarrowed);
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
        CCorpus corpus = TCorpusPrepare(engine, atelier, TInterfaceConduct.TEnvoyCreate(false, asked));
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
        CCorpus corpus = TCorpusPrepare(engine, atelier, TInterfaceConduct.TEnvoyCreate(null, asked));
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
        CCorpus corpus = TCorpusPrepare(engine, atelier, TInterfaceConduct.TEnvoyCreate(false, asked));

        Assert.True(corpus.CCorpusLeaveConfirm());
        Assert.Empty(asked);

        corpus.CCorpusExampleCreate();
        corpus.CCorpusDesk.TDeskDefer(
            TInterface.TExampleTextCreate(corpus.CCorpusDesk.CDeskId, TInterface.TStateValueCreate("a dog")));

        Assert.True(corpus.CCorpusLeaveConfirm());
        Assert.Equal(["Leave"], asked);
        Assert.Empty(corpus.CCorpusAnthology.CAnthologyRowsRead("?", "-"));
    }

    [Fact]
    public void CorpusScribeToggle_ClosingTheTranscript_CancelsTheDesk()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample cat = TCorpusExampleSave(engine, "a cat sat");
        CCorpus corpus = TCorpusPrepare(engine, atelier, TInterfaceConduct.TEnvoyCreate(false, []));
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
        CCorpus corpus = TCorpusPrepare(engine, atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        corpus.CCorpusExampleOpen(cat.LExampleId);
        corpus.CCorpusExampleCreate();

        corpus.CCorpusScribeToggle(false);

        Assert.False(corpus.CCorpusQuotationSide);
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
        CCorpus corpus = TCorpusPrepare(engine, atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        corpus.CCorpusExampleOpen(cat.LExampleId);

        corpus.CCorpusQuotationSelect(water.LEntryId);

        Assert.True(corpus.CCorpusQuotationSide);
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
        CCorpus corpus = TCorpusPrepare(engine, atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        corpus.CCorpusExampleOpen(cat.LExampleId);

        corpus.CCorpusExampleClose();

        Assert.True(corpus.CCorpusExcerptBlank);
        Assert.False(corpus.CCorpusQuotationSide);
        Assert.Null(corpus.CCorpusAnthology.CAnthologyChosen);
    }

    [Fact]
    public void CorpusEntryUpdate_NoChosenQuotation_KeepsTheTranscript()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CCorpus corpus = TCorpusPrepare(engine, atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        corpus.CCorpusExampleCreate();

        corpus.CCorpusEntryUpdate();

        Assert.True(corpus.CCorpusTranscriptShown);
        Assert.True(corpus.CCorpusDesk.CDeskHeld);
    }

    private static CCorpus TCorpusPrepare(LEngine engine, CAtelier atelier, CEnvoy envoy)
    {
        CEditor editor = CEditor.CEditorCreate(atelier, envoy);
        CCorpus corpus = CCorpus.CCorpusCreate(atelier, editor, static () => true, envoy);
        LVista example = engine.TEngineVistaStart("corpus", LCatalogOrder.LCatalogOrderText);
        LVista quotation = engine.TEngineVistaStart("quotation", LCatalogOrder.LCatalogOrderHeadword);
        corpus.CCorpusAnthology.CAnthologyVistaRestore(example);
        corpus.CCorpusQuotation.TPanelVistaRestore(quotation);
        editor.TEditorVistaRestore(quotation);
        return corpus;
    }

    private static void TCorpusRowsCheck(CCorpus corpus)
    {
        if (!corpus.CCorpusRowShown)
        {
            return;
        }

        if (corpus.CCorpusAnthology.CAnthologyRowsRead("?", "-").Any(row => row.CCatalogExampleChosen))
        {
            return;
        }

        corpus.CCorpusExampleClose();
    }

    private static LExample TCorpusExampleSave(LEngine engine, string text)
    {
        return engine.TEngineExampleCreate(TInterface.TExampleCreate(
            0, "English", TInterface.TStateValueCreate(text), null, TInterface.TStateAnchorRead(null)));
    }

    private static LEntry TCorpusEntrySave(LEngine engine)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", "wata", string.Empty, [TInterface.TCardCreate("a liquid", 1)], []));
    }
}

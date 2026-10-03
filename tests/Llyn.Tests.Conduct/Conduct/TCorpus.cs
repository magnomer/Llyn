using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCorpus
{
    [Fact]
    public void CorpusExampleCreate_ExampleChosen_OpensAQuotingEntryInTheEditor()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample cat = TCorpusExampleSave(engine, "a cat sat");
        CCorpus corpus = TCorpusPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
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
        CCorpus corpus = TCorpusPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
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
        CCorpus corpus = TCorpusPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        corpus.CCorpusAnthology.CAnthologyQuerySet("dog");
        corpus.CCorpusAnthology.CAnthologyPanel.CPanelRowsChanged += () => corpus.CCorpusRowsRead();
        int cleared = 0;
        corpus.CCorpusQueryCleared += () => cleared++;

        corpus.TCorpusExampleOpen(cat.LExampleId);

        Assert.Equal(1, cleared);
        Assert.Equal(2, corpus.CCorpusRowsRead().Count);
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
        CCorpus corpus = TCorpusPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        int recorded = 0;
        atelier.CAtelierNavigation.CNavigationChanged += _ => recorded++;

        corpus.CCorpusExampleSelect(cat.LExampleId);
        corpus.CCorpusExampleSelect(null);

        Assert.Equal(1, recorded);
        Assert.Empty(asked);
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
        CCorpus corpus = TCorpusPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        corpus.TCorpusExampleOpen(cat.LExampleId);

        corpus.CCorpusQuotationSelect(water.LEntryId);

        Assert.False(corpus.CCorpusExcerptShown);
        Assert.True(corpus.CCorpusDisplayShown);
        Assert.True(corpus.CCorpusPortraitAllowed);
        Assert.False(corpus.CCorpusBinEnabled);
        Assert.Equal(cat.LExampleId, corpus.CCorpusAnthology.CAnthologyChosen);
    }

    [Fact]
    public void CorpusWorkspaceResonate_ChosenExample_ClearsBothListsAndTellsTheDriver()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample cat = TCorpusExampleSave(engine, "a cat sat");
        CCorpus corpus = TCorpusPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        corpus.TCorpusExampleOpen(cat.LExampleId);
        int told = 0;
        corpus.CCorpusWorkspaceChanged += () => told++;

        engine.TEngineBulletinRaise(LSubject.LSubjectWorkspace, 0);

        Assert.True(corpus.CCorpusExcerptBlank);
        Assert.False(corpus.CCorpusDisplayShown);
        Assert.Null(corpus.CCorpusAnthology.CAnthologyChosen);
        Assert.Equal(1, told);
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
        CCorpus corpus = TCorpusPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        corpus.TCorpusExampleOpen(cat.LExampleId);
        corpus.CCorpusQuotationSelect(water.LEntryId);

        corpus.TCorpusEntryResonate();

        Assert.True(corpus.CCorpusDisplayShown);
        Assert.Equal(cat.LExampleId, corpus.CCorpusAnthology.CAnthologyChosen);
    }

    [Fact]
    public void CorpusRowsRead_ChosenExampleNarrowedAway_ClearsBothLists()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample cat = TCorpusExampleSave(engine, "a cat sat");
        TCorpusExampleSave(engine, "a dog ran");
        CCorpus corpus = TCorpusPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        corpus.TCorpusExampleOpen(cat.LExampleId);

        Assert.Equal(2, corpus.CCorpusRowsRead().Count);
        Assert.True(corpus.CCorpusExcerptHeld);

        corpus.CCorpusAnthology.CAnthologyQuerySet("dog");

        Assert.Single(corpus.CCorpusRowsRead());
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
        CCorpus corpus = TCorpusPrepare(atelier, TEnvoyFake.TEnvoyCreate(true, asked));
        corpus.TCorpusExampleOpen(cat.LExampleId);

        corpus.CCorpusExampleDelete();

        Assert.Equal(["Example.DeleteConfirm"], asked);
        Assert.Empty(corpus.CCorpusAnthology.TAnthologyRowsRead());
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
        CCorpus corpus = TCorpusPrepare(atelier, TEnvoyFake.TEnvoyCreate(true, asked));
        corpus.TCorpusExampleOpen(cat.LExampleId);
        corpus.CCorpusQuotationSelect(water.LEntryId);

        corpus.CCorpusExampleDelete();

        Assert.Empty(asked);
        Assert.Single(corpus.CCorpusAnthology.TAnthologyRowsRead());
        Assert.True(corpus.CCorpusDisplayShown);
    }

    internal static CCorpus TCorpusPrepare(CAtelier atelier, CEnvoy envoy)
    {
        CCorpus corpus = CCorpus.CCorpusCreate(atelier, static () => true, envoy, static run => run());
        return corpus;
    }

    internal static LExample TCorpusExampleSave(LEngine engine, string text)
    {
        return engine.TEngineExampleCreate(TInterfaceExample.TExampleCreate(
            0, "English", TInterfaceState.TStateValueCreate(text), null, TInterfaceState.TStateAnchorRead(null)));
    }

    internal static LEntry TCorpusEntrySave(LEngine engine)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", "wata", string.Empty, [TInterface.TCardCreate("a liquid", 1)], []));
    }
}

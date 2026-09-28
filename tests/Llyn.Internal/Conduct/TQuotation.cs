using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TQuotation
{
    [Fact]
    public void QuotationRowsRead_ChosenExample_ListsTheQuotingEntries()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample cat = TCorpus.TCorpusExampleSave(engine, "a cat sat");
        LExample dog = TCorpus.TCorpusExampleSave(engine, "a dog ran");
        LEntry water = TCorpus.TCorpusEntrySave(engine);
        engine.TRequestQuoteApply(water.LEntryId, cat.LExampleId);
        CCorpus corpus = TCorpus.TCorpusPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        corpus.CCorpusExampleOpen(dog.LExampleId);

        Assert.Empty(corpus.CCorpusQuotation.CQuotationRowsRead());

        corpus.CCorpusExampleOpen(cat.LExampleId);

        Assert.Equal(water.LEntryId, Assert.Single(corpus.CCorpusQuotation.CQuotationRowsRead()).CVistaRowId);
    }

    [Fact]
    public void QuotationQuerySet_UnmatchedText_EmptiesTheRowsAndWordsTheEmptyList()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample cat = TCorpus.TCorpusExampleSave(engine, "a cat sat");
        LEntry water = TCorpus.TCorpusEntrySave(engine);
        engine.TRequestQuoteApply(water.LEntryId, cat.LExampleId);
        CCorpus corpus = TCorpus.TCorpusPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        corpus.CCorpusExampleOpen(cat.LExampleId);

        Assert.Equal("Example.Vacant", corpus.CCorpusQuotation.CQuotationEmptyKey);

        corpus.CCorpusQuotation.CQuotationQuerySet("zzz");

        Assert.Empty(corpus.CCorpusQuotation.CQuotationRowsRead());
        Assert.Equal("Example.Unmatched", corpus.CCorpusQuotation.CQuotationEmptyKey);

        corpus.CCorpusQuotation.CQuotationQuerySet("  ");

        Assert.Equal("Example.Vacant", corpus.CCorpusQuotation.CQuotationEmptyKey);
    }

    [Fact]
    public void QuotationFileRead_QuotingEntryOnDisplay_OffersItsHeadword()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample cat = TCorpus.TCorpusExampleSave(engine, "a cat sat");
        LEntry water = TCorpus.TCorpusEntrySave(engine);
        engine.TRequestQuoteApply(water.LEntryId, cat.LExampleId);
        CCorpus corpus = TCorpus.TCorpusPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        corpus.CCorpusExampleOpen(cat.LExampleId);

        Assert.Equal("entry", corpus.CCorpusQuotation.CQuotationFileRead());

        corpus.CCorpusQuotationSelect(water.LEntryId);

        Assert.Equal("water", corpus.CCorpusQuotation.CQuotationFileRead());
    }
}

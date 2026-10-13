using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TQuotation
{
    [Fact]
    public async Task QuotationRowsLoad_ChosenExample_AnswersTheQuotingEntriesAfterTheFill()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample cat = TCorpus.TCorpusExampleSave(engine, "a cat sat");
        LEntry water = TCorpus.TCorpusEntrySave(engine);
        engine.TRequestQuoteApply(water.LEntryId, cat.LExampleId);
        CCorpus corpus = TCorpus.TCorpusPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        corpus.TCorpusExampleOpen(cat.LExampleId);

        CEnsignSheet<IReadOnlyList<CVistaRow>> sheet =
            await corpus.CCorpusQuotation.CQuotationRowsLoad(static (_, _) => static () => { });

        Assert.Equal([water.LEntryId], sheet.CEnsignSheetRows.Select(static row => row.CVistaRowId));
        Assert.Equal(atelier.CAtelierCatalog.CCatalogLanguageRead(), sheet.CEnsignSheetLanguages);
    }

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
        CCorpus corpus = TCorpus.TCorpusPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        corpus.TCorpusExampleOpen(dog.LExampleId);

        Assert.Empty(corpus.CCorpusQuotation.CQuotationRowsRead());

        corpus.TCorpusExampleOpen(cat.LExampleId);

        Assert.Equal(water.LEntryId, Assert.Single(corpus.CCorpusQuotation.CQuotationRowsRead()).CVistaRowId);
    }

    [Fact]
    public void QuotationRowsRead_EngineFails_ShowsTheLoadFailureAndAnswersNoRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> asked = [];

        Assert.Empty(TInterfaceMention.TQuotationFailRead(engine, TEnvoyFake.TEnvoyCreate(false, asked)));
        Assert.Equal(["Example.LoadFailed"], asked);
    }

    [Fact]
    public void QuotationRowsRead_NoExampleChosen_ListsEveryEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = TCorpus.TCorpusEntrySave(engine);
        CCorpus corpus = TCorpus.TCorpusPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));

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
        CCorpus corpus = TCorpus.TCorpusPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        corpus.TCorpusExampleOpen(cat.LExampleId);

        Assert.Equal("Example.Vacant", corpus.CCorpusQuotation.CQuotationPanel.CPanelAperture.CApertureKey);

        corpus.CCorpusQuotation.CQuotationPanel.CPanelAperture.CApertureQuerySet("zzz");

        Assert.Empty(corpus.CCorpusQuotation.CQuotationRowsRead());
        Assert.Equal("Example.Unmatched", corpus.CCorpusQuotation.CQuotationPanel.CPanelAperture.CApertureKey);

        corpus.CCorpusQuotation.CQuotationPanel.CPanelAperture.CApertureQuerySet("  ");

        Assert.Equal("Example.Vacant", corpus.CCorpusQuotation.CQuotationPanel.CPanelAperture.CApertureKey);
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
        CCorpus corpus = TCorpus.TCorpusPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        corpus.TCorpusExampleOpen(cat.LExampleId);

        Assert.Equal("entry", corpus.CCorpusQuotation.TQuotationFileRead());

        corpus.CCorpusDiptych.CDiptychChildSelect(water.LEntryId);

        Assert.Equal("water", corpus.CCorpusQuotation.TQuotationFileRead());
    }
}

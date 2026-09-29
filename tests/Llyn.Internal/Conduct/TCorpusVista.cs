using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCorpusVista
{
    [Fact]
    public void CorpusVistaRestore_QueriesHeld_CarriesThemIntoTheFreshVistas()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TCorpus.TCorpusExampleSave(engine, "a cat sat");
        TCorpus.TCorpusExampleSave(engine, "a dog ran");
        CCorpus corpus = TCorpus.TCorpusPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        corpus.CCorpusAnthology.CAnthologyQuerySet("dog");
        corpus.CCorpusQuotation.CQuotationQuerySet("zzz");

        corpus.CCorpusVistaRestore();

        Assert.Equal(["a dog ran"], corpus.CCorpusRowsRead().Select(row => row.CCatalogExampleText));
        Assert.Equal("Example.Unmatched", corpus.CCorpusQuotation.CQuotationEmptyKey);
    }

    [Fact]
    public void CorpusVistaRestore_ExampleNoticeAfterASecondRestore_RaisesEachListOnceThroughTheMarshal()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample cat = TCorpus.TCorpusExampleSave(engine, "a cat sat");
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
        corpus.CCorpusVistaRestore();
        int anthology = 0;
        int quotation = 0;
        corpus.CCorpusAnthology.CAnthologyPanel.CPanelRowsChanged += () => anthology++;
        corpus.CCorpusQuotation.CQuotationPanel.CPanelRowsChanged += () => quotation++;

        engine.TEngineBulletinRaise(LSubject.LSubjectExample, cat.LExampleId);

        Assert.Equal(1, anthology);
        Assert.Equal(1, quotation);
        Assert.Equal(1, marshalled);
    }

    [Fact]
    public void CorpusVistaRestore_EntryNotice_RaisesTheExampleRowsForTheirTallies()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = TCorpus.TCorpusEntrySave(engine);
        CCorpus corpus = TCorpus.TCorpusPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        int anthology = 0;
        corpus.CCorpusAnthology.CAnthologyPanel.CPanelRowsChanged += () => anthology++;

        engine.TEngineBulletinRaise(LSubject.LSubjectEntry, water.LEntryId);

        Assert.Equal(1, anthology);
    }

    [Fact]
    public void AnthologyPanelTallyRead_QuotingEntriesAdded_WordsEachForm()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample cat = TCorpus.TCorpusExampleSave(engine, "a cat sat");
        engine.TRequestQuoteApply(TCorpus.TCorpusEntrySave(engine).LEntryId, cat.LExampleId);
        CCorpus corpus = TCorpus.TCorpusPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        List<string> tallies = [corpus.CCorpusAnthology.CAnthologyPanel.CPanelTallyRead()];

        corpus.TCorpusExampleOpen(cat.LExampleId);
        tallies.Add(corpus.CCorpusAnthology.CAnthologyPanel.CPanelTallyRead());
        engine.TRequestQuoteApply(TCorpus.TCorpusEntrySave(engine).LEntryId, cat.LExampleId);
        tallies.Add(corpus.CCorpusAnthology.CAnthologyPanel.CPanelTallyRead());

        Assert.Equal(
            [
                TInterface.TLocalizationTextRead("Example.UsageNone"),
                TInterface.TLocalizationTextRead("Example.UsageOne"),
                "2 " + TInterface.TLocalizationTextRead("Example.UsageMany"),
            ],
            tallies);
    }
}

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
    public async Task CorpusRowsLoad_StoredExample_AnswersTheRowsAndTheLanguagesAfterTheFill()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TCorpus.TCorpusExampleSave(engine, "a cat sat");
        CCorpus corpus = TCorpus.TCorpusPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));

        CEnsignSheet<IReadOnlyList<CCatalogExample>> sheet =
            await corpus.CCorpusRowsLoad(static (_, _) => static () => { });

        Assert.Equal(["a cat sat"], sheet.CEnsignSheetRows.Select(static row => row.CCatalogExampleText));
        Assert.Equal(atelier.CAtelierCatalog.CCatalogLanguageRead(), sheet.CEnsignSheetLanguages);
    }

    [Fact]
    public void CorpusVistaRestore_QueriesHeld_CarriesThemIntoTheFreshVistas()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TCorpus.TCorpusExampleSave(engine, "a cat sat");
        TCorpus.TCorpusExampleSave(engine, "a dog ran");
        CCorpus corpus = TCorpus.TCorpusPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        corpus.CCorpusAnthology.CAnthologyPanel.CPanelAperture.CApertureQuerySet("dog");
        corpus.CCorpusQuotation.CQuotationPanel.CPanelAperture.CApertureQuerySet("zzz");

        corpus.TCorpusVistaRestore();

        Assert.Equal(["a dog ran"], corpus.CCorpusRowsRead().Select(row => row.CCatalogExampleText));
        Assert.Equal("Example.Unmatched", corpus.CCorpusQuotation.CQuotationPanel.CPanelAperture.CApertureKey);
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
            TEnvoyFake.TEnvoyCreate(false, []),
            run =>
            {
                marshalled++;
                run();
            });
        corpus.TCorpusVistaRestore();
        int anthology = 0;
        int quotation = 0;
        corpus.CCorpusAnthology.CAnthologyPanel.CPanelAperture.CApertureRowsChanged += () => anthology++;
        corpus.CCorpusQuotation.CQuotationPanel.CPanelAperture.CApertureRowsChanged += () => quotation++;

        engine.TEngineBulletinRaise(LSubject.LSubjectExample, cat.LExampleId);

        Assert.Equal(1, anthology);
        Assert.Equal(1, quotation);
        Assert.Equal(1, marshalled);
    }

    [Fact]
    public void CorpusCreate_ExampleNoticeWithoutARestore_RaisesEachListOnceThroughTheMarshal()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample cat = TCorpus.TCorpusExampleSave(engine, "a cat sat");
        int marshalled = 0;
        CCorpus corpus = CCorpus.CCorpusCreate(
            atelier,
            static () => true,
            TEnvoyFake.TEnvoyCreate(false, []),
            run =>
            {
                marshalled++;
                run();
            });
        int anthology = 0;
        int quotation = 0;
        corpus.CCorpusAnthology.CAnthologyPanel.CPanelAperture.CApertureRowsChanged += () => anthology++;
        corpus.CCorpusQuotation.CQuotationPanel.CPanelAperture.CApertureRowsChanged += () => quotation++;

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
        CCorpus corpus = TCorpus.TCorpusPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        int anthology = 0;
        corpus.CCorpusAnthology.CAnthologyPanel.CPanelAperture.CApertureRowsChanged += () => anthology++;

        engine.TEngineBulletinRaise(LSubject.LSubjectEntry, water.LEntryId);

        Assert.Equal(1, anthology);
    }

    [Fact]
    public void AnthologyApertureTallyRead_QuotingEntriesAdded_WordsEachForm()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample cat = TCorpus.TCorpusExampleSave(engine, "a cat sat");
        engine.TRequestQuoteApply(TCorpus.TCorpusEntrySave(engine).LEntryId, cat.LExampleId);
        CCorpus corpus = TCorpus.TCorpusPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        List<string> tallies = [corpus.CCorpusAnthology.CAnthologyPanel.CPanelAperture.CApertureTallyRead()];

        corpus.TCorpusExampleOpen(cat.LExampleId);
        tallies.Add(corpus.CCorpusAnthology.CAnthologyPanel.CPanelAperture.CApertureTallyRead());
        engine.TRequestQuoteApply(TCorpus.TCorpusEntrySave(engine).LEntryId, cat.LExampleId);
        tallies.Add(corpus.CCorpusAnthology.CAnthologyPanel.CPanelAperture.CApertureTallyRead());

        Assert.Equal(
            [
                TInterface.TLocalizationTextRead("Example.UsageNone"),
                TInterface.TLocalizationTextRead("Example.UsageOne"),
                "2 " + TInterface.TLocalizationTextRead("Example.UsageMany"),
            ],
            tallies);
    }
}

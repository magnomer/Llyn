using System;
using System.IO;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCorpusPortrait
{
    [Fact]
    public async Task QuotationPortraitExport_EntryOnDisplay_WritesTheEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LExample cat = TCorpus.TCorpusExampleSave(engine, "a cat sat");
        LEntry water = TCorpus.TCorpusEntrySave(engine);
        engine.TRequestQuoteApply(water.LEntryId, cat.LExampleId);
        string path = Path.Combine(workspace.TWorkspaceFolder, "water.md");
        CCorpus corpus = TCorpus.TCorpusPrepare(
            atelier, TEnvoyFake.TEnvoyFileCreate(path, CPortraitMedium.CPortraitMediumMarkdown, []));
        corpus.TCorpusExampleOpen(cat.LExampleId);

        await corpus.CCorpusQuotation.CQuotationPortraitExport();

        Assert.False(File.Exists(path));

        corpus.CCorpusQuotationSelect(water.LEntryId);
        await corpus.CCorpusQuotation.CQuotationPortraitExport();

        Assert.Contains("water", File.ReadAllText(path), StringComparison.Ordinal);
    }

    [Fact]
    public void CorpusPortraitPrint_NothingChosen_PrintsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CCorpus corpus = TCorpus.TCorpusPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));

        Task printed = corpus.CCorpusPortraitPrint();

        Assert.Same(Task.CompletedTask, printed);
        Assert.False(corpus.CCorpusPressAllowed);
    }
}

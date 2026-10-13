using System;
using System.IO;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TRepertoirePortrait
{
    [Fact]
    public async Task RepertoirePortraitExport_OccurrenceOnDisplay_WritesTheEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LSituation home = TRepertoire.TRepertoireSituationSave(engine, "at home");
        LEntry hearth = TRepertoire.TRepertoireEntrySave(engine, "hearth", home);
        string path = Path.Combine(workspace.TWorkspaceFolder, "hearth.md");
        CRepertoire repertoire = TRepertoire.TRepertoirePrepare(
            atelier, TEnvoyFake.TEnvoyFileCreate(path, CPortraitMedium.CPortraitMediumMarkdown, []));
        repertoire.TRepertoireSituationOpen(home.LSituationId);

        await repertoire.CRepertoirePortraitExport();

        Assert.False(File.Exists(path));

        repertoire.CRepertoireDiptych.CDiptychChildSelect(hearth.LEntryId);
        await repertoire.CRepertoirePortraitExport();

        Assert.Equal("hearth", repertoire.CRepertoireOccurrence.TOccurrenceFileRead());
        Assert.Contains("hearth", File.ReadAllText(path), StringComparison.Ordinal);
    }

    [Fact]
    public void RepertoirePortraitPrint_NothingChosen_PrintsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CRepertoire repertoire = TRepertoire.TRepertoirePrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));

        Task printed = repertoire.CRepertoirePortraitPrint();

        Assert.Same(Task.CompletedTask, printed);
        Assert.False(repertoire.CRepertoirePressAllowed);
    }
}

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

public sealed class TYunjingPortrait
{
    [Fact]
    public async Task YunjingPortraitExport_ChosenEntry_WritesItAndNothingBefore()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TYunjing.TYunjingPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string language = pack.TLanguageFixtureName;
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, language, "爛", "來");
        string path = Path.Combine(workspace.TWorkspaceFolder, "rotten.md");
        CYunjing yunjing = TYunjing.TYunjingPrepare(
            atelier, TEnvoyFake.TEnvoyFileCreate(path, CPortraitMedium.CPortraitMediumMarkdown, []));

        await yunjing.CYunjingPortraitExport();

        Assert.False(File.Exists(path));

        yunjing.TYunjingDiweiOpen(language, LDiwei.LDiweiInitial, "來");
        yunjing.CYunjingPanel.CPanelRowSelect(Assert.Single(yunjing.CYunjingXiaoyunRead()).CVistaRowId);
        await yunjing.CYunjingPortraitExport();

        Assert.Contains("爛", File.ReadAllText(path), StringComparison.Ordinal);
        Assert.Contains("爛", yunjing.TYunjingFileRead(), StringComparison.Ordinal);
        Assert.False(yunjing.CYunjingDiweiShown);
        Assert.True(yunjing.CYunjingDisplayShown);
    }

    [Fact]
    public void YunjingPortraitPrint_NoEntryChosen_PrintsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CYunjing yunjing = TYunjing.TYunjingPrepare(atelier);

        Assert.False(yunjing.CYunjingPanel.CPanelPressAllowed);
        Assert.Same(Task.CompletedTask, yunjing.CYunjingPortraitPrint());
    }
}

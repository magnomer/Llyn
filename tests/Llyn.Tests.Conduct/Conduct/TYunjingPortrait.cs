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
    public async Task YunjingEntryListExport_ChosenEntry_WritesItAndNothingBefore()
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

        CEntryList xiaoyun = yunjing.CYunjingXiaoyun;
        await xiaoyun.CEntryListExport();

        Assert.False(File.Exists(path));

        yunjing.TYunjingDiweiOpen(language, LDiwei.LDiweiInitial, "來");
        xiaoyun.CEntryListPanel.CPanelRowSelect(Assert.Single(xiaoyun.CEntryListRead()).CVistaRowId);
        await xiaoyun.CEntryListExport();

        Assert.Contains("爛", File.ReadAllText(path), StringComparison.Ordinal);
        Assert.Contains("爛", yunjing.TYunjingFileRead(), StringComparison.Ordinal);
        Assert.False(yunjing.CYunjingDiweiShown);
        Assert.True(yunjing.CYunjingDisplayShown);
    }

    [Fact]
    public void YunjingEntryListPrint_NoEntryChosen_PrintsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CEntryList xiaoyun = TYunjing.TYunjingPrepare(atelier).CYunjingXiaoyun;

        Assert.False(xiaoyun.CEntryListPanel.CPanelPressAllowed);
        Assert.Same(Task.CompletedTask, xiaoyun.CEntryListPrint());
    }
}

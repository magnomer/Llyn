using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TXieshengVista
{
    [Fact]
    public async Task XieshengVistaRestore_WorkspaceNotice_UnchoosesTheSeriesAndTellsTheDriver()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TXiesheng.TXieshengPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TXiesheng.TXieshengEngineStart(workspace);
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string language = pack.TLanguageFixtureName;
        await TXiesheng.TXieshengStemSave(engine, language);
        CXiesheng xiesheng = TXiesheng.TXieshengPrepare(atelier);
        xiesheng.TXieshengStemOpen(language, "龍");
        Assert.True(xiesheng.CXieshengStemShown);
        int changed = 0;
        int told = 0;
        xiesheng.CXieshengChanged += () => changed++;
        xiesheng.CXieshengWorkspaceChanged += () => told++;

        engine.TEngineBulletinRaise(LSubject.LSubjectWorkspace, 0);

        Assert.Equal(1, changed);
        Assert.Equal(1, told);
        Assert.False(xiesheng.CXieshengStemShown);
        Assert.DoesNotContain(xiesheng.CXieshengGroveRead(), static row => row.CStemChosen);
        Assert.Empty(xiesheng.CXieshengKindred.CEntryListRead());
    }

    [Fact]
    public void XieshengVistaRestore_FanqieNotice_RaisesTheGroveAndTheEntryList()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CXiesheng xiesheng = TXiesheng.TXieshengPrepare(atelier);
        int changed = 0;
        int rows = 0;
        xiesheng.CXieshengChanged += () => changed++;
        xiesheng.CXieshengKindred.CEntryListPanel.CPanelAperture.CApertureRowsChanged += () => rows++;

        engine.TEngineBulletinRaise(LSubject.LSubjectFanqie, 0);

        Assert.Equal((1, 1), (changed, rows));
    }

    [Fact]
    public void XieshengVistaRestore_SettingsNotice_RaisesTheGroveAndTheEntryList()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CXiesheng xiesheng = TXiesheng.TXieshengPrepare(atelier);
        int changed = 0;
        int rows = 0;
        xiesheng.CXieshengChanged += () => changed++;
        xiesheng.CXieshengKindred.CEntryListPanel.CPanelAperture.CApertureRowsChanged += () => rows++;

        engine.TEngineBulletinRaise(LSubject.LSubjectSettings, 0);

        Assert.Equal((1, 1), (changed, rows));
    }

    [Fact]
    public void XieshengVistaRestore_EntryNotice_RaisesTheGroveAndRelistsTheEntries()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CXiesheng xiesheng = TXiesheng.TXieshengPrepare(atelier);
        int changed = 0;
        int rows = 0;
        int panel = 0;
        xiesheng.CXieshengChanged += () => changed++;
        xiesheng.CXieshengKindred.CEntryListPanel.CPanelAperture.CApertureRowsChanged += () => rows++;
        xiesheng.CXieshengKindred.CEntryListPanel.CPanelChanged += () => panel++;

        engine.TEngineBulletinRaise(LSubject.LSubjectEntry, 42);

        Assert.Equal((1, 1, 1), (changed, rows, panel));
    }

    [Fact]
    public async Task XieshengVistaRestore_QueriesHeld_CarriesEachIntoItsFreshVista()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TXiesheng.TXieshengPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TXiesheng.TXieshengEngineStart(workspace);
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        await TXiesheng.TXieshengStemSave(engine, pack.TLanguageFixtureName);
        CXiesheng xiesheng = TXiesheng.TXieshengPrepare(atelier);
        xiesheng.CXieshengGrove.CApertureQuerySet("zzz");
        xiesheng.CXieshengKindred.CEntryListPanel.CPanelAperture.CApertureQuerySet("zzz");

        xiesheng.TXieshengVistaRestore();

        Assert.Empty(xiesheng.CXieshengGroveRead());
        Assert.Equal("Xiesheng.GroveUnmatched", xiesheng.CXieshengGrove.CApertureKey);
        xiesheng.TXieshengStemOpen(pack.TLanguageFixtureName, "龍");
        Assert.Empty(xiesheng.CXieshengKindred.CEntryListRead());
        Assert.Equal("Xiesheng.KindredUnmatched", xiesheng.CXieshengKindredKey);
    }

    [Fact]
    public void XieshengVistaRestore_FanqieNoticeAfterASecondRestore_RaisesTheGroveOnceThroughTheMarshal()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        int marshalled = 0;
        CXiesheng xiesheng = CXiesheng.CXieshengCreate(
            atelier,
            static () => true,
            TEnvoyFake.TEnvoyCreate(false, []),
            run =>
            {
                marshalled++;
                run();
            });
        xiesheng.TXieshengVistaRestore();
        int changed = 0;
        xiesheng.CXieshengChanged += () => changed++;

        engine.TEngineBulletinRaise(LSubject.LSubjectFanqie, 0);

        Assert.Equal(1, changed);
        Assert.Equal(1, marshalled);
    }

    [Fact]
    public void XieshengCreate_FanqieNoticeWithoutARestore_RaisesTheGroveOnceThroughTheMarshal()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        int marshalled = 0;
        CXiesheng xiesheng = CXiesheng.CXieshengCreate(
            atelier,
            static () => true,
            TEnvoyFake.TEnvoyCreate(false, []),
            run =>
            {
                marshalled++;
                run();
            });
        int changed = 0;
        xiesheng.CXieshengChanged += () => changed++;

        engine.TEngineBulletinRaise(LSubject.LSubjectFanqie, 0);

        Assert.Equal(1, changed);
        Assert.Equal(1, marshalled);
    }

    [Fact]
    public void XieshengOrderRead_Menu_OffersTheThreeOrderings()
    {
        Assert.Equal(
            [
                CCatalogOrder.CCatalogOrderName,
                CCatalogOrder.CCatalogOrderReverse,
                CCatalogOrder.CCatalogOrderUsage,
            ],
            CXiesheng.CXieshengOrderRead());
    }
}

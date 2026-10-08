using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEntryList
{
    [Fact]
    public async Task EntryListLoad_FetchedSeries_AnswersItsEntryAfterTheFill()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TXiesheng.TXieshengPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TXiesheng.TXieshengEngineStart(workspace);
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string language = pack.TLanguageFixtureName;
        LEntry entry = await TXiesheng.TXieshengStemSave(engine, language);
        CXiesheng xiesheng = TXiesheng.TXieshengPrepare(atelier);
        xiesheng.TXieshengStemOpen(language, "龍");

        CEnsignSheet<IReadOnlyList<CVistaRow>> sheet =
            await xiesheng.CXieshengKindred.CEntryListLoad(static (_, _) => static () => { });

        Assert.Equal(entry.LEntryId, Assert.Single(sheet.CEnsignSheetRows).CVistaRowId);
        Assert.Equal(atelier.CAtelierCatalog.CCatalogLanguageRead(), sheet.CEnsignSheetLanguages);
    }

    [Fact]
    public async Task EntryListExport_ChosenEntry_WritesItAndNothingBefore()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TXiesheng.TXieshengPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TXiesheng.TXieshengEngineStart(workspace);
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string language = pack.TLanguageFixtureName;
        LEntry entry = await TXiesheng.TXieshengStemSave(engine, language);
        string path = Path.Combine(workspace.TWorkspaceFolder, "dragon.md");
        CXiesheng xiesheng = TXiesheng.TXieshengPrepare(
            atelier, TEnvoyFake.TEnvoyFileCreate(path, CPortraitMedium.CPortraitMediumMarkdown, []));

        await xiesheng.CXieshengKindred.CEntryListExport();

        Assert.False(File.Exists(path));

        xiesheng.TXieshengStemOpen(language, "龍");
        xiesheng.CXieshengKindred.CEntryListPanel.CPanelRowSelect(entry.LEntryId);
        await xiesheng.CXieshengKindred.CEntryListExport();

        Assert.Contains("龍", File.ReadAllText(path), StringComparison.Ordinal);
        Assert.Contains("龍", xiesheng.TXieshengFileRead(), StringComparison.Ordinal);
        Assert.False(xiesheng.CXieshengStemShown);
        Assert.True(xiesheng.CXieshengDisplayShown);
    }

    [Fact]
    public async Task EntryListPanelRowOpen_ScribeOn_OpensTheEntryInTheEditorUntilClosed()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TXiesheng.TXieshengPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TXiesheng.TXieshengEngineStart(workspace);
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry entry = await TXiesheng.TXieshengStemSave(engine, pack.TLanguageFixtureName);
        CEntryList kindred = TXiesheng.TXieshengPrepare(atelier).CXieshengKindred;

        kindred.CEntryListPanel.CPanelRowOpen(entry.LEntryId);
        kindred.CEntryListPanel.CPanelScribeToggle(true);

        Assert.True(kindred.CEntryListEditing);
        Assert.Equal(entry.LEntryId, kindred.CEntryListEditor.CEditorDesk.CDeskStoredRead());

        kindred.CEntryListPanel.CPanelEntryClose();

        Assert.False(kindred.CEntryListEditing);
        Assert.Null(kindred.CEntryListEditor.CEditorDesk.CDeskStoredRead());
    }

    [Fact]
    public async Task EntryListClose_KindredExitGate_ClosesTheEditorAndStopsTheRecording()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TXiesheng.TXieshengPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TXiesheng.TXieshengEngineStart(workspace);
        int stopped = 0;
        LMediaPort media = TEngineFake.TEngineCreate<LMediaPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineRecordingStop"] = _ =>
            {
                stopped++;
                return null;
            },
        });
        using CAtelier atelier = TInterfaceConduct.TAtelierMediaCreate(engine, media);
        LEntry entry = await TXiesheng.TXieshengStemSave(engine, pack.TLanguageFixtureName);
        CEntryList kindred = TXiesheng.TXieshengPrepare(atelier).CXieshengKindred;
        kindred.CEntryListPanel.CPanelRowOpen(entry.LEntryId);
        kindred.CEntryListPanel.CPanelScribeToggle(true);
        Assert.True(kindred.CEntryListEditor.CEditorDesk.CDeskHeld);

        atelier.CAtelierClose();

        Assert.False(kindred.CEntryListEditor.CEditorDesk.CDeskHeld);
        Assert.Equal(1, stopped);
    }

    [Fact]
    public void EntryListClose_XiaoyunExitGate_ClosesTheEditorAndStopsTheRecording()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TYunjing.TYunjingPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        int stopped = 0;
        LMediaPort media = TEngineFake.TEngineCreate<LMediaPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineRecordingStop"] = _ =>
            {
                stopped++;
                return null;
            },
        });
        using CAtelier atelier = TInterfaceConduct.TAtelierMediaCreate(engine, media);
        string language = pack.TLanguageFixtureName;
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, language, "爛", "來");
        CYunjing yunjing = TYunjing.TYunjingPrepare(atelier);
        yunjing.TYunjingDiweiOpen(language, LDiwei.LDiweiInitial, "來");
        CEntryList xiaoyun = yunjing.CYunjingXiaoyun;
        xiaoyun.CEntryListPanel.CPanelRowOpen(Assert.Single(xiaoyun.CEntryListRead()).CVistaRowId);
        xiaoyun.CEntryListPanel.CPanelScribeToggle(true);
        Assert.True(xiaoyun.CEntryListEditor.CEditorDesk.CDeskHeld);

        atelier.CAtelierClose();

        Assert.False(xiaoyun.CEntryListEditor.CEditorDesk.CDeskHeld);
        Assert.Equal(1, stopped);
    }
}

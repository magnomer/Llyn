using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TYunjingVista
{
    [Fact]
    public void YunjingVistaRestore_WorkspaceNotice_UnchoosesBothColumnsAndTellsTheDriver()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TYunjing.TYunjingPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string language = pack.TLanguageFixtureName;
        TEngineXiaoyun.TXiaoyunDiweiPlace(engine, workspace, language, "爛", "來");
        CYunjing yunjing = TYunjing.TYunjingPrepare(atelier);
        yunjing.TYunjingDiweiOpen(language, LDiwei.LDiweiInitial, "來");
        yunjing.CYunjingDiweiSelect(Assert.Single(yunjing.CYunjingYunmuRead()).CDiweiId, true);
        int changed = 0;
        int told = 0;
        yunjing.CYunjingChanged += () => changed++;
        yunjing.CYunjingWorkspaceChanged += () => told++;

        engine.TEngineBulletinRaise(LSubject.LSubjectWorkspace, 0);

        Assert.Equal(1, changed);
        Assert.Equal(1, told);
        Assert.False(yunjing.CYunjingDiweiShown);
        Assert.DoesNotContain(yunjing.CYunjingShengmuRead(), static row => row.CDiweiChosen);
        Assert.DoesNotContain(yunjing.CYunjingYunmuRead(), static row => row.CDiweiChosen);
        Assert.Empty(yunjing.CYunjingXiaoyunRead());
    }

    [Fact]
    public void YunjingVistaRestore_EachColumnChanged_RaisesOnceAndChangesOnlyThatColumn()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TEngineXiaoyun.TXiaoyunDiweiPlace(engine, workspace, TYunjing.TYunjingBook, "爛", "來");
        TEngineXiaoyun.TXiaoyunDiweiPlace(engine, workspace, TYunjing.TYunjingBook, "孤", "見");
        CYunjing yunjing = TYunjing.TYunjingPrepare(atelier);
        IReadOnlyList<CDiwei> yunmu = yunjing.CYunjingYunmuRead();
        CCatalogOrder order = yunjing.CYunjingShengmuOrder;
        int changed = 0;
        yunjing.CYunjingChanged += () => changed++;

        yunjing.CYunjingShengmuFind("來");

        Assert.Equal(1, changed);
        Assert.Equal(["來"], yunjing.CYunjingShengmuRead().Select(static row => row.CDiweiKey));
        Assert.Equal(yunmu, yunjing.CYunjingYunmuRead());

        yunjing.CYunjingYunmuSet(CCatalogOrder.CCatalogOrderReverse);

        Assert.Equal(2, changed);
        Assert.Equal(CCatalogOrder.CCatalogOrderReverse, yunjing.CYunjingYunmuOrder);
        Assert.Equal(order, yunjing.CYunjingShengmuOrder);
        Assert.Equal(["來"], yunjing.CYunjingShengmuRead().Select(static row => row.CDiweiKey));
    }

    [Fact]
    public void YunjingVistaRestore_FanqieNotice_RaisesTheColumnsAndTheEntryList()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CYunjing yunjing = TYunjing.TYunjingPrepare(atelier);
        int changed = 0;
        int rows = 0;
        yunjing.CYunjingChanged += () => changed++;
        yunjing.CYunjingPanel.CPanelRowsChanged += () => rows++;

        engine.TEngineBulletinRaise(LSubject.LSubjectFanqie, 0);

        Assert.Equal((1, 1), (changed, rows));
    }

    [Fact]
    public void YunjingVistaRestore_SettingsAndReflexNotices_RaiseTheColumnsAndTheEntryListEach()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CYunjing yunjing = TYunjing.TYunjingPrepare(atelier);
        int changed = 0;
        int rows = 0;
        yunjing.CYunjingChanged += () => changed++;
        yunjing.CYunjingPanel.CPanelRowsChanged += () => rows++;

        engine.TEngineBulletinRaise(LSubject.LSubjectSettings, 0);
        engine.TEngineBulletinRaise(LSubject.LSubjectReflex, 0);

        Assert.Equal((2, 2), (changed, rows));
    }

    [Fact]
    public void YunjingVistaRestore_EntryNotice_RaisesTheColumnsAndRelistsTheEntries()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CYunjing yunjing = TYunjing.TYunjingPrepare(atelier);
        int changed = 0;
        int rows = 0;
        int panel = 0;
        yunjing.CYunjingChanged += () => changed++;
        yunjing.CYunjingPanel.CPanelRowsChanged += () => rows++;
        yunjing.CYunjingPanel.CPanelChanged += () => panel++;

        engine.TEngineBulletinRaise(LSubject.LSubjectEntry, 42);

        Assert.Equal((1, 1, 1), (changed, rows, panel));
    }

    [Fact]
    public void YunjingVistaRestore_QueriesHeld_CarriesEachIntoItsFreshVista()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TEngineXiaoyun.TXiaoyunDiweiPlace(engine, workspace, TYunjing.TYunjingBook, "爛", "來");
        CYunjing yunjing = TYunjing.TYunjingPrepare(atelier);
        yunjing.CYunjingShengmuFind("zzz");
        yunjing.CYunjingYunmuFind("zzz");
        yunjing.CYunjingXiaoyunFind("zzz");

        yunjing.TYunjingVistaRestore();

        Assert.Empty(yunjing.CYunjingShengmuRead());
        Assert.Equal("Yunjing.ShengmuUnmatched", yunjing.CYunjingShengmuKey);
        Assert.Empty(yunjing.CYunjingYunmuRead());
        Assert.Equal("Yunjing.YunmuUnmatched", yunjing.CYunjingYunmuKey);
        Assert.Empty(yunjing.CYunjingXiaoyunRead());
        Assert.Equal("Yunjing.XiaoyunEmpty", yunjing.CYunjingXiaoyunKey);
    }

    [Fact]
    public void YunjingVistaRestore_FanqieNoticeAfterASecondRestore_RaisesTheColumnsOnceThroughTheMarshal()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        int marshalled = 0;
        CYunjing yunjing = CYunjing.CYunjingCreate(
            atelier,
            static () => true,
            TEnvoyFake.TEnvoyCreate(false, []),
            run =>
            {
                marshalled++;
                run();
            });
        yunjing.TYunjingVistaRestore();
        int changed = 0;
        yunjing.CYunjingChanged += () => changed++;

        engine.TEngineBulletinRaise(LSubject.LSubjectFanqie, 0);

        Assert.Equal(1, changed);
        Assert.Equal(1, marshalled);
    }

    [Fact]
    public void YunjingCreate_FanqieNoticeWithoutARestore_RaisesTheColumnsOnceThroughTheMarshal()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        int marshalled = 0;
        CYunjing yunjing = CYunjing.CYunjingCreate(
            atelier,
            static () => true,
            TEnvoyFake.TEnvoyCreate(false, []),
            run =>
            {
                marshalled++;
                run();
            });
        int changed = 0;
        yunjing.CYunjingChanged += () => changed++;

        engine.TEngineBulletinRaise(LSubject.LSubjectFanqie, 0);

        Assert.Equal(1, changed);
        Assert.Equal(1, marshalled);
    }

    [Fact]
    public void YunjingClose_ExitGate_ClosesTheEditorAndStopsTheRecording()
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
        TEngineXiaoyun.TXiaoyunDiweiPlace(engine, workspace, language, "爛", "來");
        CYunjing yunjing = TYunjing.TYunjingPrepare(atelier);
        yunjing.TYunjingDiweiOpen(language, LDiwei.LDiweiInitial, "來");
        yunjing.CYunjingPanel.CPanelRowOpen(Assert.Single(yunjing.CYunjingXiaoyunRead()).CVistaRowId);
        yunjing.CYunjingPanel.CPanelScribeToggle(true);
        Assert.True(yunjing.CYunjingEditor.CEditorDesk.CDeskHeld);

        atelier.CAtelierClose();

        Assert.False(yunjing.CYunjingEditor.CEditorDesk.CDeskHeld);
        Assert.Equal(1, stopped);
    }
}

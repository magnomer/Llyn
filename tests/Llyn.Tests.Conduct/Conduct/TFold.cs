using System;
using System.Collections.Generic;
using System.IO;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TFold
{
    [Fact]
    public void FoldToggle_OpenedBoxes_ReadsTheNewStateAndRaisesTheChange()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CFold fold = TFoldEditorPrepare(engine, null).CEditorFold;
        int changed = 0;
        fold.CFoldChanged += () => changed++;

        fold.CFoldFanqieToggle(true);
        fold.CFoldScriptToggle(true);

        Assert.True(fold.CFoldFanqieOpened);
        Assert.True(fold.CFoldScriptOpened);
        Assert.Equal(2, changed);

        fold.CFoldFanqieToggle(false);

        Assert.False(fold.CFoldFanqieOpened);
        Assert.True(fold.CFoldScriptOpened);
    }

    [Fact]
    public void FoldToggle_FreshConduct_KeepsTheSavedState()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TFoldEditorPrepare(engine, null).CEditorFold.CFoldFanqieToggle(true);
        TFoldEditorPrepare(engine, null).CEditorFold.CFoldScriptToggle(true);

        CFold fresh = TFoldEditorPrepare(engine, TFoldEntrySave(engine)).CEditorFold;

        Assert.True(fresh.CFoldFanqieOpened);
        Assert.True(fresh.CFoldScriptOpened);
        Assert.True(TInterface.TSettingsLoad(workspace.TWorkspaceFolder).LSettingsFanqieOpened);
        Assert.True(TInterface.TSettingsLoad(workspace.TWorkspaceFolder).LSettingsScriptOpened);
    }

    [Fact]
    public void FoldToggle_SecondEditor_RaisesEachSavedChangeThere()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CFold toggled = TFoldEditorPrepare(engine, null).CEditorFold;
        CEditor second = TFoldEditorPrepare(engine, null);
        second.CEditorObserverAttach(static run => run());
        int changed = 0;
        second.CEditorFold.CFoldChanged += () => changed++;

        toggled.CFoldFanqieToggle(true);
        toggled.CFoldScriptToggle(true);
        toggled.CFoldScriptToggle(true);

        Assert.Equal(2, changed);
        Assert.True(second.CEditorFold.CFoldFanqieOpened);
        Assert.True(second.CEditorFold.CFoldScriptOpened);
    }

    [Fact]
    public void FoldToggle_SecondEditorClosed_RaisesNothingThere()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CFold toggled = TFoldEditorPrepare(engine, null).CEditorFold;
        CEditor second = TFoldEditorPrepare(engine, null);
        second.CEditorObserverAttach(static run => run());
        int changed = 0;
        second.CEditorFold.CFoldChanged += () => changed++;

        second.CEditorClose();
        toggled.CFoldFanqieToggle(true);

        Assert.Equal(0, changed);
    }

    [Fact]
    public void FoldToggle_RefusedSave_ShowsTheSaveFailureAndKeepsTheState()
    {
        List<string> notices = [];
        LSettings opened = new("English", LSettingsFanqieOpened: true, LSettingsScriptOpened: true);
        Dictionary<string, Func<object?[]?, object?>> answers = new()
        {
            ["LEngineSettingsRead"] = _ => opened,
            ["LEngineFanqieSave"] = _ => throw new InvalidOperationException("The settings file is unreadable."),
            ["LEngineScriptSave"] = _ => throw new InvalidOperationException("The settings file is unreadable."),
            ["LEngineFailureRead"] = args => ((string)args![1]!, (string?)null, (string?)null),
        };
        LSettingsPort refusing = TEngineFake.TEngineCreate<LSettingsPort>(answers);
        CFold fold = TInterfaceConductSound.TFoldCreate(refusing, TEnvoyFake.TEnvoyCreate(false, notices));
        int changed = 0;
        fold.CFoldChanged += () => changed++;

        fold.CFoldFanqieToggle(false);
        fold.CFoldScriptToggle(false);

        Assert.Equal(["Settings.SaveFailed", "Settings.SaveFailed"], notices);
        Assert.Equal(2, changed);
        Assert.True(fold.CFoldFanqieOpened);
        Assert.True(fold.CFoldScriptOpened);
    }

    [Fact]
    public void FoldToggle_FaultingStore_ShowsTheSaveFailureKeepsTheStateAndRecordsTheFault()
    {
        List<string> notices = [];
        List<Exception> recorded = [];
        Dictionary<string, Func<object?[]?, object?>> stored = new()
        {
            ["LSettingsRead"] = _ => TInterface.TSettingsCreate("en"),
            ["LSettingsSave"] = _ => throw TInterface.TVaultFaultCreate("The disk is full."),
        };
        Dictionary<string, Func<object?[]?, object?>> audited = new()
        {
            ["LAuditRecord"] = args =>
            {
                recorded.Add((Exception)args![0]!);
                return null;
            },
        };
        LSettingsVault faulting = TEngineFake.TEngineCreate<LSettingsVault>(stored);
        LAuditVault audit = TEngineFake.TEngineCreate<LAuditVault>(audited);
        using LEngine engine = TRigFake.TRigFakeStart(
            TRigFake.TRigFakeBuild() with { LRigSettings = faulting, LRigAudit = audit });
        LSettings before = engine.TEngineSettingsRead();
        CFold fold = TInterfaceEditor.TEditorCreate(engine, TEnvoyFake.TEnvoyCreate(false, notices)).CEditorFold;

        fold.CFoldFanqieToggle(!before.LSettingsFanqieOpened);

        Assert.Equal(["Settings.SaveFailed"], notices);
        Assert.Equal(before.LSettingsFanqieOpened, fold.CFoldFanqieOpened);
        Assert.Equal(before, engine.TEngineSettingsRead());
        Assert.IsType<LVaultFault>(Assert.Single(recorded));
    }

    private static long TFoldEntrySave(LEngine engine)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", "ˈwɔːtə", string.Empty, [TInterface.TCardCreate("a liquid", 1)], [])).LEntryId;
    }

    private static CEditor TFoldEditorPrepare(LEngine engine, long? entry)
    {
        CEditor editor = TInterfaceEditor.TEditorCreate(engine);
        editor.TEditorVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        editor.CEditorEntryOpen(entry);
        return editor;
    }
}

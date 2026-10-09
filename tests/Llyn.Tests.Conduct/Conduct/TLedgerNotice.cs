using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TLedgerNotice
{
    [Fact]
    public void LedgerFolderOpen_Pressed_OpensTheWorkspaceFolderAndAsksNothing()
    {
        int opened = 0;
        List<string> asked = [];
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(
            engine,
            TInterfaceConduct.TLedgerPortCreate(new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineFolderOpen"] = _ =>
                {
                    opened++;
                    return null;
                },
            }));

        atelier.CAtelierLedger.CLedgerFolderOpen(TEnvoyFake.TEnvoyCreate(false, asked));

        Assert.Equal(1, opened);
        Assert.Empty(asked);
    }

    [Fact]
    public void LedgerFolderOpen_ShellFails_ShowsTheFolderFailure()
    {
        List<string> asked = [];
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(
            engine,
            TInterfaceConduct.TLedgerPortCreate(new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineFolderOpen"] = _ => throw new IOException("moved away"),
            }));

        atelier.CAtelierLedger.CLedgerFolderOpen(TEnvoyFake.TEnvoyCreate(false, asked));

        Assert.Equal(["Settings.FolderFailed"], asked);
    }

    [Fact]
    public void LedgerFolderOpen_ShellFailsTwice_ShowsTwoNotices()
    {
        List<string> asked = [];
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(
            engine,
            TInterfaceConduct.TLedgerPortCreate(new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineFolderOpen"] = _ => throw new IOException("moved away"),
            }));
        CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, asked);

        atelier.CAtelierLedger.CLedgerFolderOpen(envoy);
        atelier.CAtelierLedger.CLedgerFolderOpen(envoy);

        Assert.Equal(["Settings.FolderFailed", "Settings.FolderFailed"], asked);
    }

    [Fact]
    public void LedgerFailureShow_FailsTwice_ShowsTwoNotices()
    {
        List<string> asked = [];
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TInterfaceConduct.TLedgerPortCreate([]));
        CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, asked);

        atelier.TLedgerFailureShow(envoy, "Export.Failed", new IOException("locked"));
        atelier.TLedgerFailureShow(envoy, "Export.Failed", new IOException("locked"));

        Assert.Equal(["Export.Failed", "Export.Failed"], asked);
    }

    [Fact]
    public void LedgerRepaintShow_FailsTwice_ShowsOneNotice()
    {
        List<string> asked = [];
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TInterfaceConduct.TLedgerPortCreate([]));
        CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, asked);

        atelier.TLedgerRepaintShow(envoy, "Display.OrderFailed", new IOException("locked"));
        atelier.TLedgerRepaintShow(envoy, "Display.OrderFailed", new IOException("locked"));

        Assert.Equal(["Display.OrderFailed"], asked);
    }

    [Fact]
    public void LedgerRepaintShow_SameKeyThenOtherKey_ShowsEachKeyOnce()
    {
        List<string> asked = [];
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TInterfaceConduct.TLedgerPortCreate([]));
        CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, asked);

        atelier.TLedgerRepaintShow(envoy, "Display.OrderFailed", new IOException("locked"));
        atelier.TLedgerRepaintShow(envoy, "Display.OrderFailed", new IOException("locked"));
        atelier.TLedgerRepaintShow(envoy, "Display.CitationFailed", new IOException("locked"));
        atelier.TLedgerRepaintShow(envoy, "Display.OrderFailed", new IOException("locked"));

        Assert.Equal(["Display.OrderFailed", "Display.CitationFailed"], asked);
    }

    [Fact]
    public void LedgerRepaintShow_OtherAtelier_ShowsTheKeyAgain()
    {
        List<string> asked = [];
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TInterfaceConduct.TLedgerPortCreate([]));
        using CAtelier other = TInterfaceConduct.TAtelierCreate(engine, TInterfaceConduct.TLedgerPortCreate([]));
        CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, asked);

        atelier.TLedgerRepaintShow(envoy, "Display.OrderFailed", new IOException("locked"));
        other.TLedgerRepaintShow(envoy, "Display.OrderFailed", new IOException("locked"));

        Assert.Equal(["Display.OrderFailed", "Display.OrderFailed"], asked);
    }

    [Fact]
    public void LedgerRepaintShow_SettingsSavedBetween_ShowsTheKeyAgain()
    {
        List<string> asked = [];
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<CLedgerState> shown = TInterfaceConduct.TLedgerShowRead(atelier);
        CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, asked);

        atelier.TLedgerRepaintShow(envoy, "Display.OrderFailed", new IOException("locked"));
        atelier.TLedgerRepaintShow(envoy, "Display.OrderFailed", new IOException("locked"));
        atelier.CAtelierLedger.CLedgerEpithetSave(
            !shown[^1].CLedgerStateSettings.CSettingsEpithet, TEnvoyFake.TEnvoyCreate(false, []));
        atelier.TLedgerRepaintShow(envoy, "Display.OrderFailed", new IOException("locked"));

        Assert.Equal(["Display.OrderFailed", "Display.OrderFailed"], asked);
    }

    [Fact]
    public void LedgerSave_RefusedSaves_ShowsEachSaveFailureAndRepaints()
    {
        List<string> asked = [];
        Func<object?[]?, object?> refuse = _ => throw new IOException("The disk is full.");
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(
            engine,
            TInterfaceConduct.TLedgerPortCreate(new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineLocalizationSave"] = refuse,
                ["LEngineEpithetSave"] = refuse,
                ["LEngineFrequencySave"] = refuse,
                ["LEngineMorphologySave"] = refuse,
                ["LEngineRespellingSave"] = refuse,
            }));
        List<CLedgerState> shown = TInterfaceConduct.TLedgerShowRead(atelier);
        CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, asked);
        CLedger ledger = atelier.CAtelierLedger;

        ledger.CLedgerLocalizationSave("de", envoy);
        ledger.CLedgerEpithetSave(true, envoy);
        ledger.CLedgerFrequencySave(false, envoy);
        ledger.CLedgerMorphologySave(false, envoy);
        ledger.CLedgerRespellingSave(false, envoy);

        Assert.Equal(Enumerable.Repeat("Settings.SaveFailed", 5), asked);
        Assert.Equal(6, shown.Count);
        Assert.Equal(shown[0].CLedgerStateSettings, shown[^1].CLedgerStateSettings);
    }

    [Fact]
    public void LedgerSave_FaultingStore_ShowsTheSaveFailureAndRecordsTheFaultOnce()
    {
        List<string> asked = [];
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
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild() with
        {
            LRigSettings = TEngineFake.TEngineCreate<LSettingsVault>(stored),
            LRigAudit = TEngineFake.TEngineCreate<LAuditVault>(audited),
        });
        LSettingsPort settings = TInterfaceConduct.TSettingsOutletCreate(engine);
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, settings);
        bool epithet = engine.TEngineSettingsRead().LSettingsEpithet;

        atelier.CAtelierLedger.CLedgerEpithetSave(!epithet, TEnvoyFake.TEnvoyCreate(false, asked));

        Assert.Equal(["Settings.SaveFailed"], asked);
        Assert.Equal(epithet, engine.TEngineSettingsRead().LSettingsEpithet);
        Assert.IsType<LVaultFault>(Assert.Single(recorded));
    }

    [Fact]
    public void LedgerNoticeRead_WrappedRefusal_AnswersItsReasonAlone()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        Exception wrapped = new InvalidOperationException("outer", TInterface.TRefusalCreate(LRefusal.LRefusalStale));

        CLedgerNotice notice = atelier.TLedgerNoticeRead(wrapped);
        Assert.Equal(new CLedgerNotice(LRefusal.LRefusalStale, null, null), notice);
    }

    [Fact]
    public void LedgerNoticeRead_Fault_AnswersTheUnexpectedKeyAndItsAuditFile()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        Exception bare = new InvalidOperationException("bare");

        CLedgerNotice notice = atelier.TLedgerNoticeRead(bare);
        Assert.Equal("Notice.Unexpected", notice.CLedgerNoticeKey);
        Assert.Equal("Notice.Recorded", notice.CLedgerNoticeLabel);
        Assert.True(File.Exists(notice.CLedgerNoticePath));
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TAtelier
{
    [Fact]
    public void AtelierAboutRead_AnySession_ReadsVersionKey()
    {
        Assert.Equal("Headquarter.Version", CAtelier.CAtelierAboutRead());
    }

    [Fact]
    public void AtelierRefusalRead_BusyOrNot_ReadsItsOwnKey()
    {
        Assert.Equal("Workspace.Busy", CAtelier.CAtelierRefusalRead(true));
        Assert.Equal("Workspace.OpenFailed", CAtelier.CAtelierRefusalRead(false));
    }

    [Fact]
    public void AtelierRescueRead_SetAsideOrNot_ReadsTheKeyOnlyWhenDone()
    {
        Assert.Equal("Workspace.DatabaseReset", CAtelier.CAtelierRescueRead(true));
        Assert.Null(CAtelier.CAtelierRescueRead(false));
    }

    [Fact]
    public void AtelierVistaStart_EverySubjectAndOrder_ReachesTheEngineByName()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);

        foreach (CCatalogOrder order in Enum.GetValues<CCatalogOrder>())
        {
            LVista vista = atelier.CAtelierVistaStart("order" + order, CSubject.CSubjectReference, order);

            Assert.Equal(order.ToString()[1..], vista.LVistaOrder.ToString()[1..]);
        }

        foreach (CSubject subject in Enum.GetValues<CSubject>())
        {
            LVista vista = atelier.CAtelierVistaStart("subject" + subject, subject, CCatalogOrder.CCatalogOrderName);

            Assert.Equal(subject.ToString()[1..], vista.LVistaSubject?.ToString()[1..]);
        }

        Assert.Null(atelier.CAtelierVistaStart("bare", null, CCatalogOrder.CCatalogOrderName).LVistaSubject);
    }

    [Fact]
    public void AtelierRead_FreshEngine_ReadsDefaultVolumeAndSplit()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());

        Assert.Equal(1, atelier.CAtelierVolumeRead());
        Assert.False(TInterfaceConduct.TAtelierSplitRead(atelier));
    }

    [Fact]
    public void AtelierVolumeSet_Unsettled_PlaysWithoutWriting()
    {
        List<double> played = [];
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TAtelierMediaCreate(played));

        atelier.CAtelierVolumeSet(0.4, false);

        using CAtelier reopened = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());
        Assert.Equal(0.4, atelier.CAtelierVolumeRead());
        Assert.Equal(1, reopened.CAtelierVolumeRead());
        Assert.Equal([0.4], played);
    }

    [Fact]
    public void AtelierVolumeSet_Settled_WritesTheLevel()
    {
        List<double> played = [];
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TAtelierMediaCreate(played));

        atelier.CAtelierVolumeSet(0.3, false);
        atelier.CAtelierVolumeSet(0.6, true);

        using CAtelier reopened = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());
        Assert.Equal(0.6, reopened.CAtelierVolumeRead());
        Assert.Equal([0.3, 0.6], played);
    }

    [Fact]
    public void AtelierVolumeSet_FaultingPostureStore_ShowsTheLayoutFailureOnceAndRecordsEachFault()
    {
        List<double> played = [];
        List<string> notices = [];
        List<Exception> thrown = [];
        List<Exception> recorded = [];
        LPostureVault faulting = TEngineFake.TEngineCreate<LPostureVault>(
            new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LPostureRead"] = _ => TInterface.TPostureStateCreate(),
                ["LPostureSave"] = _ =>
                {
                    LVaultFault fault = TInterface.TVaultFaultCreate("The disk is full.");
                    thrown.Add(fault);
                    throw fault;
                },
            });
        LAuditVault audit = TEngineFake.TEngineCreate<LAuditVault>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LAuditRecord"] = args =>
            {
                recorded.Add((Exception)args![0]!);
                return null;
            },
        });
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(
            workspace.TWorkspaceRigCreate() with { LRigPosture = faulting, LRigAudit = audit },
            _ => throw new NotSupportedException("The test moves no workspace."),
            _ => { });
        using CAtelier atelier = TInterfaceConduct.TAtelierMediaCreate(engine, TAtelierMediaCreate(played));
        atelier.TAtelierOpen(TEnvoyFake.TEnvoyCreate(false, notices));

        atelier.CAtelierVolumeSet(0.5, true);
        atelier.CAtelierVolumeSet(0.25, true);

        Assert.Equal(["Layout.SaveFailed"], notices);
        Assert.Equal(2, thrown.Count);
        Assert.Equal(thrown, recorded);
        Assert.Equal([0.5, 0.25], played);
        Assert.Equal(0.25, atelier.CAtelierVolumeRead());
    }

    [Fact]
    public void AtelierOpen_LayoutFailureBeforeOpen_ShowsTheNoticeOnTheOpen()
    {
        List<string> notices = [];
        LPostureVault faulting = TEngineFake.TEngineCreate<LPostureVault>(
            new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LPostureRead"] = _ => TInterface.TPostureStateCreate(),
                ["LPostureSave"] = _ => throw TInterface.TVaultFaultCreate("The disk is full."),
            });
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(
            workspace.TWorkspaceRigCreate() with { LRigPosture = faulting },
            _ => throw new NotSupportedException("The test moves no workspace."),
            _ => { });
        using CAtelier atelier = TInterfaceConduct.TAtelierMediaCreate(engine, TAtelierMediaCreate([]));
        atelier.CAtelierWorkspace.TWorkspaceObserverAttach(static run => run());
        atelier.CAtelierVolumeSet(0.5, true);
        Assert.Empty(notices);

        atelier.TAtelierOpen(TEnvoyFake.TEnvoyCreate(false, notices));
        atelier.TAtelierOpen(TEnvoyFake.TEnvoyCreate(false, notices));

        Assert.Equal(["Layout.SaveFailed"], notices);
    }

    [Theory]
    [InlineData(-0.5, 0)]
    [InlineData(1.5, 1)]
    [InlineData(double.NaN, 1)]
    [InlineData(double.PositiveInfinity, 1)]
    [InlineData(double.NegativeInfinity, 0)]
    public void AtelierVolumeRead_KeptLevelOutOfRange_ReadsAFiniteLevelFromSilenceToFull(double level, double read)
    {
        LPostureVault kept = TEngineFake.TEngineCreate<LPostureVault>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LPostureRead"] = _ => TInterface.TPostureStateCreate(volume: level),
            ["LPostureSave"] = _ => null,
        });
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild() with { LRigPosture = kept });
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());

        Assert.Equal(read, atelier.CAtelierVolumeRead());
    }

    [Theory]
    [InlineData(-0.5, 0)]
    [InlineData(1.5, 1)]
    [InlineData(double.NaN, 1)]
    [InlineData(double.PositiveInfinity, 1)]
    [InlineData(double.NegativeInfinity, 0)]
    public void AtelierVolumeSet_LevelOutOfRange_PlaysAndKeepsAFiniteLevelFromSilenceToFull(double level, double kept)
    {
        List<double> played = [];
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TAtelierMediaCreate(played));
        atelier.CAtelierVolumeSet(0.4, false);

        atelier.CAtelierVolumeSet(level, true);

        using CAtelier reopened = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());
        Assert.Equal([0.4, kept], played);
        Assert.Equal(kept, atelier.CAtelierVolumeRead());
        Assert.Equal(kept, reopened.CAtelierVolumeRead());
    }

    [Fact]
    public void WorkspaceEstablishmentChanged_Opened_ShowsTheStatusAtOnceAndStopsOnDetach()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<CEstablishment> shown = [];

        atelier.CAtelierWorkspace.CWorkspaceEstablishmentChanged += shown.Add;
        atelier.TAtelierStubOpen();
        atelier.CAtelierWorkspace.CWorkspaceEstablishmentChanged -= shown.Add;
        atelier.CAtelierLedger.CLedgerEpithetSave(
            !engine.TEngineSettingsRead().LSettingsEpithet, TEnvoyFake.TEnvoyCreate(false, []));

        Assert.Equal([atelier.TAtelierEstablishmentRead()], shown);
    }

    [Theory]
    [InlineData(0, 0L, 3000L, "Establishment.Entry", "Establishment.Kilobyte", 3d)]
    [InlineData(2, 1L, 1024L * 1024, "Establishment.EntryOne", "Establishment.Megabyte", 1d)]
    [InlineData(0, 7L, 3L * 1024 * 1024 / 2, "Establishment.Entry", "Establishment.Megabyte", 1.5d)]
    public void AtelierEstablishmentRead_EngineVerdicts_ChoosesWordingKeysAndAmount(
        int unsaved, long entry, long size, string entryKey, string sizeKey, double amount)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(
            engine,
            TEngineFake.TEngineCreate<LSettingsPort>(new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineEstablishmentRead"] = _ =>
                    TInterfaceEngineWorkspace.TEstablishmentCreate(unsaved, entry, size),
            }));

        CEstablishment establishment = atelier.TAtelierEstablishmentRead();

        Assert.Equal(unsaved, establishment.CEstablishmentUnsaved);
        Assert.Equal(entry, establishment.CEstablishmentEntry);
        Assert.Equal(unsaved > 0, establishment.CEstablishmentPending);
        Assert.Equal(entryKey, establishment.CEstablishmentEntryKey);
        Assert.Equal(sizeKey, establishment.CEstablishmentSizeKey);
        Assert.Equal(
            amount.ToString(size >= 1024L * 1024 ? "0.0" : "0", CultureInfo.CurrentCulture),
            establishment.CEstablishmentAmount);
    }

    [Fact]
    public void AtelierOpen_Opened_RaisesTheViewsBeforeTheirState()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> heard = [];
        atelier.CAtelierWorkspace.CWorkspaceOpened += () => heard.Add("Opened");
        atelier.CAtelierWorkspace.TWorkspaceStateAdd(
            state => heard.Add(state == new CWorkspaceState(null, null) ? "Blank" : "Kept"));

        atelier.TAtelierStubOpen();

        Assert.Equal(["Opened", "Blank"], heard);
    }

    [Fact]
    public void AtelierOpen_BlankLeftoverDraft_SweepsItBeforeTheViewsRestore()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        long blank;
        using (LEngine earlier = workspace.TWorkspaceEngineStart())
        {
            blank = earlier.TEngineDraftStart("Input", null).LDraftId;
        }

        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        bool? swept = null;
        atelier.CAtelierWorkspace.CWorkspaceOpened += () => swept = engine.TEngineDraftRead(blank) is null;

        atelier.TAtelierStubOpen();

        Assert.True(swept);
    }

    [Fact]
    public void AtelierOpen_Reopened_HearsEachLedgerChangeOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<CLedgerState> shown = [];
        atelier.CAtelierLedger.CLedgerChanged += shown.Add;
        atelier.TAtelierStubOpen();
        atelier.TAtelierStubOpen();
        int opened = shown.Count;

        atelier.CAtelierLedger.CLedgerEpithetSave(
            !engine.TEngineSettingsRead().LSettingsEpithet, TEnvoyFake.TEnvoyCreate(false, []));

        Assert.Equal(opened + 1, shown.Count);
    }

    [Fact]
    public void AtelierWorkspaceChange_InputHeld_OpenLeavesTheInputOnABlankEntry()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(first.TWorkspaceRigCreate(), _ => second.TWorkspaceRigCreate(), _ => { });
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CEnvoy envoy = TEngineFake.TEngineCreate<CEnvoy>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["CEnvoyLeaveConfirm"] = _ =>
            {
                asked.Add("Leave");
                return false;
            },
        });
        TEditorFixture editor = new(atelier.CAtelierInputCreate(envoy));
        editor.TEditorFixtureOpen(null);
        editor.TEditorFixtureEntry.CEntryHeadwordSet("water");

        atelier.CAtelierWorkspace.CWorkspaceChange(second.TWorkspaceFolder, envoy);

        Assert.Equal(["Leave"], asked);
        Assert.True(editor.TEditorFixtureDesk.CDeskHeld);
        Assert.False(editor.TEditorFixtureDesk.TDeskChangeCheck());
        Assert.Equal(string.Empty, editor.TEditorDraftRead()?.CEntryDraftHeadword);
    }

    [Fact]
    public void AtelierQuitConfirm_InputUnsaved_AsksOnceAndDiscardsEveryArea()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        List<bool> closed = [];
        CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, asked);
        TEditorFixture editor = new(atelier.CAtelierInputCreate(envoy));
        editor.TEditorFixtureOpen(null);
        editor.TEditorFixtureEntry.CEntryHeadwordSet("water");
        atelier.CAtelierWorkspace.TWorkspaceDraftAdd(static () => false, store => { closed.Add(store); return true; });

        bool quit = atelier.CAtelierQuitConfirm(envoy);

        Assert.True(quit);
        Assert.Equal(["Leave"], asked);
        Assert.Equal([false], closed);
        Assert.False(editor.TEditorFixtureDesk.CDeskHeld);
    }

    private static LMediaPort TAtelierMediaCreate(List<double> played)
    {
        return TEngineFake.TEngineCreate<LMediaPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineVolumeSet"] = args =>
            {
                played.Add((double)args![0]!);
                return null;
            },
        });
    }
}

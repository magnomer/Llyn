using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed partial class TFault
{
    private static readonly IReadOnlyList<TFaultWrite> TFaultWriteRows =
    [
        new(
            "CAtelier.CAtelierVolumeSet",
            "LMediaPort.LEngineVolumeSet",
            "Sound.VolumeFailed",
            static stage =>
            {
                CAtelier atelier = TFaultAtelierStart(stage);
                atelier.TAtelierOpen(TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard));
                return (() => atelier.CAtelierVolumeSet(0.25, true), null);
            }),
        new(
            "CAtelier.CAtelierOpen",
            "LSettingsPort.LEngineWorkspaceStart",
            "Workspace.OpenFailed",
            static stage =>
            {
                CAtelier atelier = TFaultAtelierStart(stage);
                CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard);
                return (() => atelier.CAtelierOpen(envoy, static run => run()), null);
            }),
        new(
            "CAtelier.CAtelierClose",
            "LDraftPort.LEngineLeftoverSweep",
            null,
            static stage =>
            {
                CAtelier atelier = TInterfaceConduct.TAtelierFaultCreate(
                    TFaultEngineStart(stage), stage.TFaultStageMember, stage.TFaultStageThrown);
                atelier.TAtelierOpen(TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard));
                return (atelier.CAtelierClose, null);
            }),
        new(
            "CWorkspace.CWorkspaceChange(String, CEnvoy)",
            "LSettingsPort.LEngineWorkspaceChange",
            "Workspace.OpenFailed",
            static stage =>
            {
                CAtelier atelier = TFaultAtelierStart(stage);
                string chosen = stage.TFaultStageAdd(TWorkspace.TWorkspacePrepare()).TWorkspaceFolder;
                CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard);
                return (() => atelier.CAtelierWorkspace.CWorkspaceChange(chosen, envoy), atelier.CAtelierPathRead);
            }),
        new(
            "CWorkspace.CWorkspaceChange(CEnvoy)",
            "LSettingsPort.LEngineWorkspaceChange",
            "Workspace.OpenFailed",
            static stage =>
            {
                CAtelier atelier = TFaultAtelierStart(stage);
                string chosen = stage.TFaultStageAdd(TWorkspace.TWorkspacePrepare()).TWorkspaceFolder;
                CEnvoy envoy = TFaultFolderCreate(stage, chosen);
                return (() => atelier.CAtelierWorkspace.CWorkspaceChange(envoy), atelier.CAtelierPathRead);
            }),
        new(
            "CLedger.CLedgerLocalizationSave",
            "LSettingsVault.LSettingsSave",
            "Settings.SaveFailed",
            static stage =>
            {
                LEngine engine = TFaultVaultStart(stage);
                CAtelier atelier = TFaultAtelierCreate(stage, engine);
                CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard);
                string language = engine.TEngineSettingsRead().LSettingsLocalization == "de" ? "ko" : "de";
                return (
                    () => atelier.CAtelierLedger.CLedgerLocalizationSave(language, envoy),
                    () => engine.TEngineSettingsRead().LSettingsLocalization);
            }),
        new(
            "CLedger.CLedgerEpithetSave",
            "LSettingsVault.LSettingsSave",
            "Settings.SaveFailed",
            static stage =>
            {
                LEngine engine = TFaultVaultStart(stage);
                CAtelier atelier = TFaultAtelierCreate(stage, engine);
                CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard);
                bool chosen = !engine.TEngineSettingsRead().LSettingsEpithet;
                return (
                    () => atelier.CAtelierLedger.CLedgerEpithetSave(chosen, envoy),
                    () => engine.TEngineSettingsRead().LSettingsEpithet);
            }),
        new(
            "CLedger.CLedgerFrequencySave",
            "LSettingsVault.LSettingsSave",
            "Settings.SaveFailed",
            static stage =>
            {
                LEngine engine = TFaultVaultStart(stage);
                CAtelier atelier = TFaultAtelierCreate(stage, engine);
                CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard);
                bool chosen = !engine.TEngineSettingsRead().LSettingsFrequency;
                return (
                    () => atelier.CAtelierLedger.CLedgerFrequencySave(chosen, envoy),
                    () => engine.TEngineSettingsRead().LSettingsFrequency);
            }),
        new(
            "CLedger.CLedgerMorphologySave",
            "LSettingsVault.LSettingsSave",
            "Settings.SaveFailed",
            static stage =>
            {
                LEngine engine = TFaultVaultStart(stage);
                CAtelier atelier = TFaultAtelierCreate(stage, engine);
                CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard);
                bool chosen = !engine.TEngineSettingsRead().LSettingsMorphology;
                return (
                    () => atelier.CAtelierLedger.CLedgerMorphologySave(chosen, envoy),
                    () => engine.TEngineSettingsRead().LSettingsMorphology);
            }),
        new(
            "CLedger.CLedgerRespellingSave",
            "LSettingsVault.LSettingsSave",
            "Settings.SaveFailed",
            static stage =>
            {
                LEngine engine = TFaultVaultStart(stage);
                CAtelier atelier = TFaultAtelierCreate(stage, engine);
                CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard);
                bool chosen = !engine.TEngineSettingsRead().LSettingsRespelled;
                return (
                    () => atelier.CAtelierLedger.CLedgerRespellingSave(chosen, envoy),
                    () => engine.TEngineSettingsRead().LSettingsRespelled);
            }),
        new(
            "CLedger.CLedgerFolderOpen",
            "LSettingsPort.LEngineFolderOpen",
            "Settings.FolderFailed",
            static stage =>
            {
                CAtelier atelier = TFaultAtelierStart(stage);
                CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard);
                return (() => atelier.CAtelierLedger.CLedgerFolderOpen(envoy), null);
            }),
        new(
            "CNavigation.CNavigationTabSelect",
            "LPostureVault.LPostureSave",
            "Layout.SaveFailed",
            static stage =>
            {
                CAtelier atelier = TFaultAtelierCreate(stage, TFaultVaultStart(stage));
                atelier.TAtelierOpen(TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard));
                return (() => atelier.CAtelierNavigation.CNavigationTabSelect("Library"), null);
            }),
        new(
            "CYunjing.CYunjingTallyToggle",
            "LSettingsVault.LSettingsSave",
            "Settings.SaveFailed",
            static stage =>
            {
                LEngine engine = TFaultVaultStart(stage);
                CYunjing yunjing = CYunjing.CYunjingCreate(
                    TFaultAtelierCreate(stage, engine),
                    static () => true,
                    TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard),
                    static run => run());
                bool chosen = !engine.TEngineSettingsRead().LSettingsTally;
                return (
                    () => yunjing.CYunjingTallyToggle(chosen),
                    () => engine.TEngineSettingsRead().LSettingsTally);
            }),
        .. TFaultMarkRows,
    ];

    public static TheoryData<string> TFaultWriteGates
    {
        get
        {
            TheoryData<string> gates = new();
            foreach (TFaultWrite row in TFaultWriteRows)
            {
                gates.Add(row.TFaultWriteGate);
            }

            return gates;
        }
    }

    [Theory]
    [MemberData(nameof(TFaultWriteGates))]
    public void EnvoyFailureShow_FailedWrite_ReceivesKeyAndKeepsState(string gate) => TFaultWriteRun(gate);

    private static void TFaultWriteRun(string gate)
    {
        TFaultWrite row = TFaultWriteRows.Single(
            found => string.Equals(found.TFaultWriteGate, gate, StringComparison.Ordinal));
        using TFaultStage stage = new(row.TFaultWriteMember, true);
        (Action call, Func<object?>? kept) = row.TFaultWriteArrange(stage);
        object? before = kept?.Invoke();
        stage.TFaultStageHeard.Clear();

        Exception? escaped = Record.Exception(call);

        Assert.Null(escaped);
        if (row.TFaultWriteKey is string key)
        {
            Assert.Equal([key], stage.TFaultStageHeard);
        }
        else
        {
            Assert.Empty(stage.TFaultStageHeard);
        }

        if (kept is not null)
        {
            Assert.Equal(before, kept());
        }
    }

    private static LEngine TFaultVaultStart(TFaultStage stage)
    {
        TWorkspace workspace = stage.TFaultStageAdd(TWorkspace.TWorkspacePrepare());
        using (LEngine primer = workspace.TWorkspaceEngineStart())
        {
            TInterfaceConduct.TAtelierCreate(primer).Dispose();
        }

        LRig rig = workspace.TWorkspaceRigCreate();
        LSettingsVault settings = rig.LRigSettings;
        LPostureVault posture = rig.LRigPosture;
        LRig faulted = rig with
        {
            LRigSettings = TEngineFake.TEngineCreate<LSettingsVault>(new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LSettingsExist"] = _ => settings.TSettingsExist(),
                ["LSettingsRead"] = _ => settings.TSettingsRead(),
                ["LSettingsSave"] = args =>
                {
                    TFaultVaultCheck(stage, "LSettingsVault.LSettingsSave");
                    settings.TSettingsSave((LSettings)args![0]!);
                    return null;
                },
            }),
            LRigPosture = TEngineFake.TEngineCreate<LPostureVault>(new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LPostureRead"] = args => posture.TPostureRead((string)args![0]!),
                ["LPostureSave"] = args =>
                {
                    TFaultVaultCheck(stage, "LPostureVault.LPostureSave");
                    posture.TPostureSave((string)args![0]!, (LPostureState)args[1]!);
                    return null;
                },
            }),
        };
        return stage.TFaultStageAdd(TInterface.TEngineCreate(faulted));
    }

    private static void TFaultVaultCheck(TFaultStage stage, string member)
    {
        if (string.Equals(stage.TFaultStageMember, member, StringComparison.Ordinal))
        {
            throw TInterface.TVaultFaultCreate($"The fault vault faults {member}.");
        }
    }

    private static CEnvoy TFaultFolderCreate(TFaultStage stage, string chosen) =>
        TEngineFake.TEngineCreate<CEnvoy>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["CEnvoyWorkspaceRead"] = _ => chosen,
            ["CEnvoyFailureShow"] = args =>
            {
                stage.TFaultStageHeard.Add((string)args![0]!);
                return null;
            },
        });

    private sealed record TFaultWrite(
        string TFaultWriteGate,
        string TFaultWriteMember,
        string? TFaultWriteKey,
        Func<TFaultStage, (Action, Func<object?>?)> TFaultWriteArrange);
}

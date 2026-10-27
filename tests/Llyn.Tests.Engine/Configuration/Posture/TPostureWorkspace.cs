using System.Collections.Generic;
using System.IO;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TPostureWorkspace
{
    [Fact]
    public void PostureStart_PostureBesideLegacy_PrefersPosture()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        File.WriteAllText(
            Path.Combine(workspace.TWorkspaceFolder, "settings.json"),
            "{ \"localization\": \"en\", \"mode\": \"Library\" }");
        File.WriteAllText(Path.Combine(workspace.TWorkspaceFolder, "posture.json"), "{ \"mode\": \"Corpus\" }");
        using LEngine engine = workspace.TWorkspaceEngineStart();

        Assert.Equal("Corpus", engine.TPostureStart().TPostureRead().LPostureStateMode);
    }

    [Fact]
    public void WorkspaceOpen_TargetEmpty_InheritsCurrentPosture()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspaceCreate();
        using LEngine engine = first.TWorkspaceEngineStart();
        LPosture posture = engine.TPostureStart();
        posture.TPostureModeSave("Tenor");

        engine.TEngineWorkspaceOpen(second.TWorkspaceFolder);

        Assert.Equal("Tenor", posture.TPostureRead().LPostureStateMode);
        Assert.True(File.Exists(Path.Combine(second.TWorkspaceFolder, "posture.json")));
    }

    [Fact]
    public void WorkspaceOpen_TargetHasPosture_KeepsTargetPosture()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspaceCreate();
        File.WriteAllText(Path.Combine(second.TWorkspaceFolder, "posture.json"), "{ \"mode\": \"Corpus\" }");
        using LEngine engine = first.TWorkspaceEngineStart();
        LPosture posture = engine.TPostureStart();
        posture.TPostureModeSave("Tenor");

        engine.TEngineWorkspaceOpen(second.TWorkspaceFolder);

        Assert.Equal("Corpus", posture.TPostureRead().LPostureStateMode);
    }

    [Fact]
    public void WorkspaceOpen_LegacySettingsOnly_MigratesPosture()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspaceCreate();
        File.WriteAllText(
            Path.Combine(second.TWorkspaceFolder, "settings.json"),
            "{ \"localization\": \"en\", \"window\": { \"left\": 1, \"top\": 2, \"width\": 700, \"height\": 500 }, "
            + "\"mode\": \"Corpus\", \"volume\": 0.5 }");
        using LEngine engine = first.TWorkspaceEngineStart();
        LPosture posture = engine.TPostureStart();
        posture.TPostureModeSave("Tenor");

        engine.TEngineWorkspaceOpen(second.TWorkspaceFolder);

        LPostureState state = posture.TPostureRead();
        Assert.Equal("Corpus", state.LPostureStateMode);
        Assert.Equal(0.5, state.LPostureStateVolume);
        Assert.True(File.Exists(Path.Combine(second.TWorkspaceFolder, "posture.json")));
    }

    [Fact]
    public void WorkspaceOpen_PostureUnreadable_KeepsHeldPosture()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspaceCreate();
        string path = Path.Combine(second.TWorkspaceFolder, "posture.json");
        File.WriteAllText(path, "{ \"mode\": \"Corpus\" }");
        using LEngine engine = first.TWorkspaceEngineStart();
        LPosture posture = engine.TPostureStart();
        posture.TPostureModeSave("Tenor");

        using (new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None))
        {
            engine.TEngineWorkspaceOpen(second.TWorkspaceFolder);
        }

        Assert.Equal("Tenor", posture.TPostureRead().LPostureStateMode);
        Assert.Equal("{ \"mode\": \"Corpus\" }", File.ReadAllText(path));
    }

    [Fact]
    public void PostureStart_ReadFault_RaisesReadFailedWithFault()
    {
        List<Exception> thrown = [];
        List<Exception> failed = [];
        LPostureVault faulting = TEngineFake.TEngineCreate<LPostureVault>(
            new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LPostureRead"] = _ =>
                {
                    LVaultFault fault = new(new IOException("The disk is gone."));
                    thrown.Add(fault);
                    throw fault;
                },
                ["LPostureSave"] = _ => null,
            });
        List<Exception> recorded = [];
        LAuditVault audit = TEngineFake.TEngineCreate<LAuditVault>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LAuditRecord"] = args =>
            {
                recorded.Add((Exception)args![0]!);
                return null;
            },
        });
        using LEngine engine = TRigFake.TRigFakeStart(
            TRigFake.TRigFakeBuild() with { LRigPosture = faulting, LRigAudit = audit });
        LPosture posture = engine.TPostureStart();
        posture.LPostureReadFailed += failed.Add;

        posture.TPostureModeSave("Corpus");

        Assert.Equal(Assert.Single(thrown), Assert.Single(failed));
        Assert.Equal(thrown, recorded);
    }

    [Fact]
    public void PostureSave_FaultAfterRigApply_RaisesFailureAgain()
    {
        List<Exception> thrown = [];
        List<Exception> failed = [];
        LPostureVault faulting = TEngineFake.TEngineCreate<LPostureVault>(
            new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LPostureRead"] = _ => TInterface.TPostureStateCreate(),
                ["LPostureSave"] = _ =>
                {
                    LVaultFault fault = new(new IOException("The disk is full."));
                    thrown.Add(fault);
                    throw fault;
                },
            });
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild() with { LRigPosture = faulting });
        LPosture posture = engine.TPostureStart();
        posture.LPostureSaveFailed += failed.Add;
        posture.TPostureModeSave("Corpus");

        engine.TEngineRigApply(TRigFake.TRigFakeBuild(new TVaultFake(), "fake-second") with { LRigPosture = faulting });
        posture.TPostureModeSave("Library");

        Assert.Equal(2, thrown.Count);
        Assert.Equal(thrown, failed);
    }
}

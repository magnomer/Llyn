using System.Diagnostics;

using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TClaim
{
    [Fact]
    public void ClaimArchiveCheck_ProcessRunning_HoldsDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using Process running = Process.GetCurrentProcess();

        TInterface.TClaimArchiveSave(
            workspace.TWorkspaceFolder,
            TInterface.TClaimCreate("kindle", running.Id, new DateTimeOffset(running.StartTime)));

        Assert.True(TInterface.TClaimArchiveCheck(workspace.TWorkspaceFolder, "kindle"));
        Assert.NotNull(TInterface.TClaimArchiveRead(workspace.TWorkspaceFolder, "kindle"));
        Assert.Single(TInterface.TClaimArchiveScan(workspace.TWorkspaceFolder));
    }

    [Fact]
    public void ClaimArchiveCheck_ProcessGone_SweepsClaim()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        Process held = TClaimProcessStart();
        LClaim claim = TInterface.TClaimCreate(
            "kindle", held.Id, new DateTimeOffset(held.StartTime));
        TClaimProcessStop(held);

        TInterface.TClaimArchiveSave(workspace.TWorkspaceFolder, claim);

        Assert.False(TInterface.TClaimArchiveCheck(workspace.TWorkspaceFolder, "kindle"));
        Assert.Null(TInterface.TClaimArchiveRead(workspace.TWorkspaceFolder, "kindle"));
        Assert.Empty(TInterface.TClaimArchiveScan(workspace.TWorkspaceFolder));
    }

    [Fact]
    public void ClaimArchiveCheck_ProcessIdTakenOver_SweepsClaim()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using Process running = Process.GetCurrentProcess();

        TInterface.TClaimArchiveSave(
            workspace.TWorkspaceFolder,
            TInterface.TClaimCreate(
                "kindle", running.Id, new DateTimeOffset(running.StartTime).AddMinutes(-5)));

        Assert.False(TInterface.TClaimArchiveCheck(workspace.TWorkspaceFolder, "kindle"));
        Assert.Null(TInterface.TClaimArchiveRead(workspace.TWorkspaceFolder, "kindle"));
    }

    [Fact]
    public void ClaimArchiveCheck_DraftNothingClaims_AnswersFalse()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        Assert.False(TInterface.TClaimArchiveCheck(workspace.TWorkspaceFolder, "kindle"));
    }

    [Fact]
    public void LeftoverRead_ClaimHeldByAnotherProcess_PassesOverIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        Process held = TClaimProcessStart();

        string draftId = TClaimDraftCreate(workspace);

        TInterface.TClaimArchiveSave(
            workspace.TWorkspaceFolder,
            TInterface.TClaimCreate(draftId, held.Id, new DateTimeOffset(held.StartTime)));

        using (LEngine launched = workspace.TWorkspaceEngineStart())
        {
            Assert.Empty(launched.TEngineLeftoverRead());
        }

        TClaimProcessStop(held);

        using LEngine swept = workspace.TWorkspaceEngineStart();
        IReadOnlyList<LDraft> leftovers = swept.TEngineLeftoverRead();

        Assert.Single(leftovers);
        Assert.Equal(draftId, leftovers[0].LDraftId);
        Assert.Null(TInterface.TClaimArchiveRead(workspace.TWorkspaceFolder, draftId));
    }

    [Fact]
    public void DraftCommit_StoredDraft_DropsItsClaim()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("Input", null);

        Assert.NotNull(TInterface.TClaimArchiveRead(workspace.TWorkspaceFolder, started.LDraftId));

        engine.TEngineDraftSave(started with
        {
            LDraftContent = started.LDraftContent with { LEntryDraftHeadword = "kindle" },
        });
        engine.TEngineDraftCommit(started.LDraftId);

        Assert.Null(TInterface.TClaimArchiveRead(workspace.TWorkspaceFolder, started.LDraftId));
    }

    [Fact]
    public void DraftCancel_DiscardedDraft_DropsItsClaim()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("Input", null);
        engine.TEngineDraftCancel(started.LDraftId);

        Assert.Null(TInterface.TClaimArchiveRead(workspace.TWorkspaceFolder, started.LDraftId));
    }

    private static string TClaimDraftCreate(TWorkspace workspace)
    {
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);

        engine.TEngineDraftSave(started with
        {
            LDraftContent = started.LDraftContent with { LEntryDraftHeadword = "kindle" },
        });

        return started.LDraftId;
    }

    private static Process TClaimProcessStart()
    {
        ProcessStartInfo start = new("cmd.exe", "/c pause")
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        return Process.Start(start)!;
    }

    private static void TClaimProcessStop(Process held)
    {
        held.Kill(true);
        held.WaitForExit();
        held.Dispose();
    }
}

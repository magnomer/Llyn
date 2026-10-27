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
            workspace.TWorkspaceFolder, -1, running.Id, new DateTimeOffset(running.StartTime));

        Assert.True(TInterface.TClaimArchiveCheck(workspace.TWorkspaceFolder, -1));
        Assert.NotNull(TInterface.TClaimArchiveRead(workspace.TWorkspaceFolder, -1));
        Assert.Single(TInterface.TClaimArchiveScan(workspace.TWorkspaceFolder));
    }

    [Fact]
    public void ClaimArchiveCheck_ProcessGone_SweepsClaim()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        Process held = TClaimProcessStart();
        int process = held.Id;
        DateTimeOffset moment = new(held.StartTime);
        TClaimProcessStop(held);

        TInterface.TClaimArchiveSave(workspace.TWorkspaceFolder, -1, process, moment);

        Assert.False(TInterface.TClaimArchiveCheck(workspace.TWorkspaceFolder, -1));
        Assert.Null(TInterface.TClaimArchiveRead(workspace.TWorkspaceFolder, -1));
        Assert.Empty(TInterface.TClaimArchiveScan(workspace.TWorkspaceFolder));
    }

    [Fact]
    public void ClaimArchiveCheck_ProcessIdTakenOver_SweepsClaim()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using Process running = Process.GetCurrentProcess();

        TInterface.TClaimArchiveSave(
            workspace.TWorkspaceFolder, -1, running.Id, new DateTimeOffset(running.StartTime).AddMinutes(-5));

        Assert.False(TInterface.TClaimArchiveCheck(workspace.TWorkspaceFolder, -1));
        Assert.Null(TInterface.TClaimArchiveRead(workspace.TWorkspaceFolder, -1));
    }

    [Fact]
    public void ClaimArchiveCheck_DraftNothingClaims_AnswersFalse()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        Assert.False(TInterface.TClaimArchiveCheck(workspace.TWorkspaceFolder, -1));
    }

    [Fact]
    public void LeftoverRead_ClaimHeldByAnotherProcess_SkipsDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        Process held = TClaimProcessStart();

        long draftId = TClaimDraftCreate(workspace);

        TInterface.TClaimArchiveSave(
            workspace.TWorkspaceFolder, draftId, held.Id, new DateTimeOffset(held.StartTime));

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
    public void DraftCommit_StoredDraft_DropsClaim()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("Input", null);

        Assert.NotNull(TInterface.TClaimArchiveRead(workspace.TWorkspaceFolder, started.LDraftId));

        engine.TRequestContentApply(started.LDraftId, started.LDraftContent with { LEntryDraftHeadword = "kindle" });
        engine.TEngineDraftCommit(started.LDraftId);

        Assert.Null(TInterface.TClaimArchiveRead(workspace.TWorkspaceFolder, started.LDraftId));
    }

    [Fact]
    public void DraftCancel_DiscardedDraft_DropsClaim()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("Input", null);
        engine.TEngineDraftCancel(started.LDraftId);

        Assert.Null(TInterface.TClaimArchiveRead(workspace.TWorkspaceFolder, started.LDraftId));
    }

    [Fact]
    public void DossierCancel_TentativeOwner_DropsTargetAndRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        string root = workspace.TWorkspaceFolder;
        LDraft owner = TInterface.TDraftArchiveStart(root, TInterface.TDraftNestedCreate("Library", "ember"));
        LDraft target = TInterface.TDraftArchiveStart(root, TInterface.TDraftNestedCreate("Input", "kindle"));
        TInterface.TCourtArchiveSave(root, TInterface.TDraftLinkCreate(owner.LDraftId, target.LDraftId, "kindle"));

        LDraftRelease release = TInterface.TDraftArchiveCancel(
            root, owner.LDraftId, new HashSet<long> { owner.LDraftId, target.LDraftId });

        Assert.Empty(TInterface.TCourtArchiveScan(root));
        Assert.Null(TInterface.TDraftArchiveRead(root, owner.LDraftId));
        Assert.Null(TInterface.TDraftArchiveRead(root, target.LDraftId));
        Assert.Null(TInterface.TClaimArchiveRead(root, target.LDraftId));
        Assert.Equal([target.LDraftId, owner.LDraftId], release.LDraftReleaseDrafts);
        Assert.Empty(release.LDraftReleaseOwners);
    }

    [Fact]
    public void DossierCancel_TargetLinkedElsewhere_KeepsTarget()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        string root = workspace.TWorkspaceFolder;
        LDraft owner = TInterface.TDraftArchiveStart(root, TInterface.TDraftNestedCreate("Library", "ember"));
        LDraft other = TInterface.TDraftArchiveStart(root, TInterface.TDraftNestedCreate("Library", "hearth"));
        LDraft target = TInterface.TDraftArchiveStart(root, TInterface.TDraftNestedCreate("Input", "kindle"));
        TInterface.TCourtArchiveSave(root, TInterface.TDraftLinkCreate(owner.LDraftId, target.LDraftId, "kindle"));
        TInterface.TCourtArchiveSave(root, TInterface.TDraftLinkCreate(other.LDraftId, target.LDraftId, "kindle"));

        LDraftRelease release = TInterface.TDraftArchiveCancel(
            root, owner.LDraftId, new HashSet<long> { owner.LDraftId, other.LDraftId, target.LDraftId });

        LCourt left = Assert.Single(TInterface.TCourtArchiveScan(root));
        Assert.Equal(other.LDraftId, left.LCourtOwnerId);
        Assert.NotNull(TInterface.TDraftArchiveRead(root, target.LDraftId));
        Assert.True(TInterface.TClaimArchiveCheck(root, target.LDraftId));
        Assert.Equal([owner.LDraftId], release.LDraftReleaseDrafts);
    }

    private static long TClaimDraftCreate(TWorkspace workspace)
    {
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);

        engine.TRequestContentApply(started.LDraftId, started.LDraftContent with { LEntryDraftHeadword = "kindle" });

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

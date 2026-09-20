using Llyn.Application;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TClaimClerk
{
    [Fact]
    public void ClaimClerkStart_ThisProcess_HoldsTheDraft()
    {
        LRig rig = TInterface.TRigClaimCreate();
        LClaimClerk clerk = TInterface.TClaimClerkCreate(rig);

        LDraft draft = clerk.TClaimClerkStart("editor", 0);

        Assert.Contains(draft.LDraftId, clerk.TClaimHeldRead());
        Assert.True(clerk.TClaimClerkCheck(draft.LDraftId));
        Assert.False(clerk.TClaimForeignCheck(draft.LDraftId));
        Assert.Equal(draft.LDraftId, clerk.TClaimDraftRead(draft.LDraftId)?.LDraftId);
    }

    [Fact]
    public void ClaimForeignCheck_AnotherProcessHoldsTheDraft_SeesTheClaim()
    {
        LRig rig = TInterface.TRigClaimCreate();
        LClaimClerk first = TInterface.TClaimClerkCreate(rig);
        LDraft draft = first.TClaimClerkStart("editor", 0);

        LClaimClerk second = TInterface.TClaimClerkCreate(rig.TRigProcessSet(2));

        Assert.True(second.TClaimForeignCheck(draft.LDraftId));
        Assert.False(second.TClaimClerkCheck(draft.LDraftId));
        Assert.Empty(second.TClaimHeldRead());
        Assert.Equal(draft.LDraftId, second.TClaimDraftRead(draft.LDraftId)?.LDraftId);
    }

    [Fact]
    public void ClaimClerkFinish_HeldDraft_DropsFileClaimAndHold()
    {
        LRig rig = TInterface.TRigClaimCreate();
        LClaimClerk clerk = TInterface.TClaimClerkCreate(rig);
        LDraft draft = clerk.TClaimClerkStart("editor", 0);
        LClaimClerk other = TInterface.TClaimClerkCreate(rig.TRigProcessSet(2));

        clerk.TClaimClerkFinish(draft.LDraftId);

        Assert.DoesNotContain(draft.LDraftId, clerk.TClaimHeldRead());
        Assert.Null(clerk.TClaimDraftRead(draft.LDraftId));
        Assert.False(other.TClaimForeignCheck(draft.LDraftId));
    }

    [Fact]
    public void ClaimClerkSweep_StaleDraft_CancelsItWithItsClaim()
    {
        LRig rig = TInterface.TRigClaimCreate();
        LClaimClerk clerk = TInterface.TClaimClerkCreate(rig);
        LDraft kept = clerk.TClaimClerkStart("editor", 0);
        LDraft stale = clerk.TClaimClerkStart("editor", 0);
        rig.TDraftStaleSet(stale.LDraftId);
        LClaimClerk other = TInterface.TClaimClerkCreate(rig.TRigProcessSet(2));

        clerk.TClaimClerkSweep();

        Assert.Equal([kept.LDraftId], clerk.TClaimDraftScan().Select(draft => draft.LDraftId));
        Assert.False(other.TClaimForeignCheck(stale.LDraftId));
        Assert.True(other.TClaimForeignCheck(kept.LDraftId));
    }

    [Fact]
    public void ClaimClerkCancel_OwnerOfATentativeTarget_DropsTargetAndRow()
    {
        LRig rig = TInterface.TRigClaimCreate();
        LClaimClerk clerk = TInterface.TClaimClerkCreate(rig);
        LCourtClerk courts = TInterface.TCourtClerkCreate(rig);
        LDraft owner = clerk.TClaimClerkStart("editor", 0);
        LDraft target = clerk.TClaimClerkStart("editor", 0);
        courts.TCourtClerkSave(owner.LDraftId, target.LDraftId, "kindle");

        clerk.TClaimClerkCancel(owner.LDraftId);

        Assert.Empty(courts.TCourtClerkScan());
        Assert.Null(clerk.TClaimDraftRead(owner.LDraftId));
        Assert.Null(clerk.TClaimDraftRead(target.LDraftId));
        Assert.Empty(clerk.TClaimHeldRead());
    }

    [Fact]
    public void ClaimClerkCancel_TargetAnotherDraftStillLinks_KeepsTarget()
    {
        LRig rig = TInterface.TRigClaimCreate();
        LClaimClerk clerk = TInterface.TClaimClerkCreate(rig);
        LCourtClerk courts = TInterface.TCourtClerkCreate(rig);
        LDraft owner = clerk.TClaimClerkStart("editor", 0);
        LDraft other = clerk.TClaimClerkStart("editor", 0);
        LDraft target = clerk.TClaimClerkStart("editor", 0);
        courts.TCourtClerkSave(owner.LDraftId, target.LDraftId, "kindle");
        courts.TCourtClerkSave(other.LDraftId, target.LDraftId, "kindle");

        clerk.TClaimClerkCancel(owner.LDraftId);

        LCourt left = Assert.Single(courts.TCourtClerkScan());
        Assert.Equal(other.LDraftId, left.LCourtOwnerId);
        Assert.NotNull(clerk.TClaimDraftRead(target.LDraftId));
        Assert.True(clerk.TClaimClerkCheck(target.LDraftId));
    }
}

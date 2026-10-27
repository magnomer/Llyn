using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TTenureQueue
{
    private readonly List<LBulletin> _tTenureQueueNotices = [];

    [Fact]
    public void TenureQueueApply_RefusedRequest_RaisesRefusalBulletin()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = TTenureQueueStart(engine);

        LBulletin refused = Assert.Single(_tTenureQueueNotices, static notice => notice.LBulletinRefusal is not null);

        Assert.Equal(LSubject.LSubjectDraft, refused.LBulletinSubject);
        Assert.Equal(tenure.LTenureId, refused.LBulletinId);
        Assert.Equal(LRefusal.LRefusalCard, Assert.IsType<LRefusal>(refused.LBulletinRefusal).LRefusalReason);
        tenure.TTenureCancel();
    }

    [Fact]
    public void TenureQueueApply_RefusedRequest_AppliesNextRequest()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = TTenureQueueStart(engine);

        Assert.Equal("ember", tenure.TTenureRead()?.LDraftContent.LEntryDraftHeadword);
        Assert.False(tenure.TTenureStateRead().LTenureStateHalted);
        tenure.TTenureCancel();
    }

    private LTenure TTenureQueueStart(LEngine engine)
    {
        engine.TEngineDelaySet(TEngineTenure.TTenureHold);
        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectEntry, null);
        tenure.TTenureDraftAttach(LSubject.LSubjectDraft, _tTenureQueueNotices.Add);
        tenure.TTenureRequestDefer(TInterface.TRequestTitleCreate(
            tenure.LTenureId, TInterface.TIdentityCreate(), TInterfaceState.TStateValueCreate("set alight")));
        tenure.TTenureRequestDefer(TInterface.TRequestHeadwordCreate(tenure.LTenureId, "ember"));
        tenure.TTenurePersist();
        return tenure;
    }
}

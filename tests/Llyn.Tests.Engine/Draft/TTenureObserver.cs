using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TTenureObserver
{
    private readonly List<LBulletin> _tTenureNotices = [];

    [Fact]
    public void Prepare_RequestApplied_ReturnsNewestDraftAndDropsItsNotice()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectExample, null);
        tenure.TTenureDraftAttach(LSubject.LSubjectDraft, _tTenureNotices.Add);
        LDraft? draft = tenure.TTenurePrepare(() =>
        {
            tenure.TTenureRequestApply(
                TInterface.TExampleTextCreate(tenure.LTenureId, TInterfaceState.TStateValueCreate("new text")));
            Assert.Empty(_tTenureNotices);
        });
        Assert.Equal("new text", draft?.LDraftExample?.LExampleText.LStateValueText);
        Assert.Empty(_tTenureNotices);
        engine.TEngineBulletinRaise(LSubject.LSubjectDraft, tenure.LTenureId);
        Assert.Single(_tTenureNotices);
    }

    [Fact]
    public void Attach_SubjectAndDraftFilters_DeliverOnlyMatchingNotices()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectEntry, null);
        tenure.TTenureObserverAttach(LSubject.LSubjectSettings, _tTenureNotices.Add);
        tenure.TTenureDraftAttach(LSubject.LSubjectDraft, _tTenureNotices.Add);

        engine.TEngineBulletinRaise(LSubject.LSubjectDraft, tenure.LTenureId + 1);
        engine.TEngineBulletinRaise(LSubject.LSubjectEntry, tenure.LTenureId);
        engine.TEngineBulletinRaise(LSubject.LSubjectSettings, 0);
        engine.TEngineBulletinRaise(LSubject.LSubjectDraft, tenure.LTenureId);

        Assert.Equal(
            [LSubject.LSubjectSettings, LSubject.LSubjectDraft],
            _tTenureNotices.Select(row => row.LBulletinSubject));
        tenure.TTenureCancel();
        _tTenureNotices.Clear();
        engine.TEngineBulletinRaise(LSubject.LSubjectSettings, 0);
        Assert.Empty(_tTenureNotices);
    }

    [Fact]
    public void EntryAttach_StoredEntryIdentity_IsDistinctFromDraftIdentity()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate("water", "English", "", "", [], []));
        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectEntry, entry.LEntryId);
        tenure.TTenureEntryAttach(LSubject.LSubjectFrequency, _tTenureNotices.Add);

        engine.TEngineBulletinRaise(LSubject.LSubjectFrequency, tenure.LTenureId);
        engine.TEngineBulletinRaise(LSubject.LSubjectFrequency, entry.LEntryId);

        Assert.Equal(entry.LEntryId, Assert.Single(_tTenureNotices).LBulletinId);
    }

    [Fact]
    public void StoredRead_StoredDraft_AnswersIdWithoutPersisting()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(TEngineTenure.TTenureHold);
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate("water", "English", "", "", [], []));
        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectEntry, entry.LEntryId);
        tenure.TTenureRequestDefer(TInterface.TRequestHeadwordCreate(tenure.LTenureId, "ember"));

        Assert.Equal(entry.LEntryId, tenure.TTenureStoredRead());
        Assert.Equal("water", tenure.TTenureRead()?.LDraftContent.LEntryDraftHeadword);
        tenure.TTenureCancel();
    }

    [Fact]
    public void StoredRead_RefusedDraft_AnswersNull()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspacePrepare();
        using LEngine engine = first.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate("water", "English", "", "", [], []));
        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectEntry, entry.LEntryId);

        engine.TEngineWorkspaceOpen(second.TWorkspaceFolder);

        Assert.Throws<LRefusal>(() => tenure.TTenureRead());
        Assert.Null(tenure.TTenureStoredRead());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Prepare_NestedNotices_DropsEveryOneUntilOutermostScopeExits(bool fails)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectExample, null);
        tenure.TTenureDraftAttach(LSubject.LSubjectDraft, _tTenureNotices.Add);

        void TTenurePrepare()
        {
            tenure.TTenurePrepare(() =>
            {
                engine.TEngineBulletinRaise(LSubject.LSubjectDraft, tenure.LTenureId);
                tenure.TTenurePrepare(() => engine.TEngineBulletinRaise(LSubject.LSubjectDraft, tenure.LTenureId));
                Assert.Empty(_tTenureNotices);
                if (fails)
                {
                    throw new InvalidOperationException("test");
                }
            });
        }

        if (fails)
        {
            Assert.Throws<InvalidOperationException>(TTenurePrepare);
        }
        else
        {
            TTenurePrepare();
        }

        Assert.Empty(_tTenureNotices);
        engine.TEngineBulletinRaise(LSubject.LSubjectDraft, tenure.LTenureId);
        Assert.Equal(tenure.LTenureId, Assert.Single(_tTenureNotices).LBulletinId);
    }
}

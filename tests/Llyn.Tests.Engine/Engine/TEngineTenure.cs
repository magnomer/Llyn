using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineTenure
{
    internal const int TTenureHold = 600000;

    [Fact]
    public void TenureDefer_SameKey_AppliesLast()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(TTenureHold);

        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectExample, null);
        tenure.TTenureRequestDefer(
            TInterface.TExampleTextCreate(tenure.LTenureId, TInterfaceState.TStateValueCreate("one")));
        tenure.TTenureRequestDefer(
            TInterface.TExampleTextCreate(tenure.LTenureId, TInterfaceState.TStateValueCreate("two")));

        Assert.Equal(string.Empty, tenure.TTenureRead()?.LDraftExample?.LExampleText.TStateValueShow());

        tenure.TTenurePersist();

        Assert.Equal("two", tenure.TTenureRead()?.LDraftExample?.LExampleText.TStateValueShow());
        Assert.True(tenure.TTenureStateRead().LTenureStateChanged);
        tenure.TTenureCancel();
    }

    [Fact]
    public void TenureFinish_Unchanged_Cancels()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);

        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectExample, null);
        long held = tenure.LTenureId;

        Assert.Null(tenure.TTenureFinish(true));
        Assert.Null(engine.TEngineDraftRead(held));
        Assert.Null(tenure.TTenureRead());
        Assert.Empty(engine.TEngineExampleFind(string.Empty, LCatalogOrder.LCatalogOrderEarliest));
    }

    [Fact]
    public void TenureDefer_ApplyThrows_MarksHalted()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);

        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectExample, null);
        engine.TEngineDraftCancel(tenure.LTenureId);

        tenure.TTenureRequestDefer(
            TInterface.TExampleTextCreate(tenure.LTenureId, TInterfaceState.TStateValueCreate("lost")));

        Assert.True(tenure.TTenureStateRead().LTenureStateHalted);
        Assert.Throws<LRefusal>(() => tenure.TTenureFinish(true));
        Assert.Null(tenure.TTenureUndo());
    }

    [Fact]
    public void TenureDefer_Refused_KeepsRunning()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);
        List<long> drafts = [];
        engine.TEngineObserverAttach(bulletin =>
        {
            if (bulletin.LBulletinSubject == LSubject.LSubjectDraft)
            {
                drafts.Add(bulletin.LBulletinId);
            }
        });

        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectEntry, null);
        tenure.TTenureRequestDefer(
            TInterface.TRequestAdditionCreate(tenure.LTenureId, LCardKind.LCardKindCollocation, 5, 0));
        tenure.TTenureRequestDefer(TInterface.TRequestHeadwordCreate(tenure.LTenureId, "ember"));

        Assert.False(tenure.TTenureStateRead().LTenureStateHalted);
        Assert.Equal("ember", tenure.TTenureRead()?.LDraftContent.LEntryDraftHeadword);
        Assert.Contains(tenure.LTenureId, drafts);
        tenure.TTenureCancel();
    }

    [Fact]
    public void TenureFinish_Discard_LeavesWaiting()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(TTenureHold);
        List<long> drafts = [];
        engine.TEngineObserverAttach(bulletin =>
        {
            if (bulletin.LBulletinSubject == LSubject.LSubjectDraft)
            {
                drafts.Add(bulletin.LBulletinId);
            }
        });

        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectExample, null);
        tenure.TTenureRequestDefer(
            TInterface.TExampleTextCreate(tenure.LTenureId, TInterfaceState.TStateValueCreate("gone")));

        Assert.Null(tenure.TTenureFinish(false));
        Assert.Null(tenure.TTenureRead());
        Assert.DoesNotContain(tenure.LTenureId, drafts);
    }

    [Fact]
    public void TenureUndo_AfterDefer_PersistsFirst()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(TTenureHold);
        DateTimeOffset moment = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        workspace.TWorkspaceClockSet(() => moment);

        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectExample, null);
        tenure.TTenureRequestApply(
            TInterface.TExampleTextCreate(tenure.LTenureId, TInterfaceState.TStateValueCreate("one")));
        moment += TimeSpan.FromMilliseconds(2000);
        tenure.TTenureRequestDefer(
            TInterface.TExampleTextCreate(tenure.LTenureId, TInterfaceState.TStateValueCreate("two")));

        LDraft? restored = tenure.TTenureUndo();

        Assert.Equal("one", restored?.LDraftExample?.LExampleText.TStateValueShow());
        Assert.True(tenure.TTenureStateRead().LTenureStateForward);
        Assert.Equal("two", tenure.TTenureRedo()?.LDraftExample?.LExampleText.TStateValueShow());
        tenure.TTenureCancel();
    }

    [Fact]
    public void TenureChangeCheck_FreshEntry_AnswersUnchanged()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectEntry, null);

        Assert.False(tenure.TTenureChangeCheck());
        Assert.False(tenure.TTenureStorableRead());
        tenure.TTenureCancel();
    }

    [Fact]
    public void TenureDefer_GlossTwice_OneChronicleStep()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(TTenureHold);
        DateTimeOffset moment = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        workspace.TWorkspaceClockSet(() => moment);

        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectEntry, null);
        long draft = tenure.LTenureId;
        tenure.TTenureRequestApply(TInterface.TRequestHeadwordCreate(draft, "kindle"));
        tenure.TTenureRequestApply(TInterface.TRequestAdditionCreate(draft, LCardKind.LCardKindMeaning, 0, 0));
        long card = tenure.TTenureRead()!.LDraftContent.LEntryDraftMeanings[0].LCardDraftId;
        tenure.TTenureRequestApply(TInterface.TSentenceAdditionCreate(draft, card, 0));
        long sentence = TInterface.TRequestCardFind(tenure.TTenureRead()!.LDraftContent, card)
            .LCardDraftSentence[0].LSentenceDraftId;
        tenure.TTenureRequestApply(
            TInterface.TSentenceTextCreate(draft, card, sentence, TInterfaceState.TStateValueCreate("she knelt")));
        tenure.TTenureRequestApply(TInterface.TGlossAdditionCreate(draft, card, sentence, "French", 0));
        long gloss = TInterface.TRequestCardFind(tenure.TTenureRead()!.LDraftContent, card)
            .LCardDraftSentence[0].LSentenceDraftExample!.LExampleDraftGloss[0].LGlossDraftId;
        moment += TimeSpan.FromMilliseconds(60000);

        tenure.TTenureRequestDefer(
            TInterface.TGlossTextCreate(draft, card, sentence, gloss, TInterfaceState.TStateValueCreate("elle")));
        tenure.TTenureRequestDefer(TInterface.TGlossTextCreate(
            draft, card, sentence, gloss, TInterfaceState.TStateValueCreate("elle s'agenouilla")));
        tenure.TTenurePersist();

        LDraft? restored = tenure.TTenureUndo();

        Assert.True(TInterface.TRequestCardFind(restored!.LDraftContent, card)
            .LCardDraftSentence[0].LSentenceDraftExample!.LExampleDraftGloss[0].LGlossDraftText.LStateValueEmpty);
        Assert.True(tenure.TTenureStateRead().LTenureStateForward);
        tenure.TTenureCancel();
    }
}

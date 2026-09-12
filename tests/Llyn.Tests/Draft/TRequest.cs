using System.Collections.Generic;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TRequest
{
    [Fact]
    public void RequestApply_HeadwordText_PersistsInDraftFile()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);

        LDraft answered = engine.TEngineRequestApply(TInterface.TRequestHeadwordCreate(started.LDraftId, "kindle"));
        LDraft? held = engine.TEngineDraftRead(started.LDraftId);

        Assert.Equal("kindle", answered.LDraftContent.LEntryDraftHeadword);
        Assert.NotNull(held);
        Assert.Equal("kindle", held.LDraftContent.LEntryDraftHeadword);
    }

    [Fact]
    public void RequestApply_CardAddition_MintsNegativeIdAndRaisesBulletin()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);
        TRequestObserver observer = new();
        engine.TEngineObserverAttach(observer);

        LDraft answered = engine.TEngineRequestApply(
            TInterface.TRequestAdditionCreate(started.LDraftId, LCardKind.LCardKindMeaning, 0, 0));

        LCardDraft card = Assert.Single(answered.LDraftContent.LEntryDraftMeanings);
        Assert.True(card.LCardDraftId < 0);
        Assert.Equal(1, card.LCardDraftPosition);
        LBulletin bulletin = Assert.Single(observer.TRequestObserverBulletins);
        Assert.Equal(LSubject.LSubjectDraft, bulletin.LBulletinSubject);
        Assert.Equal(started.LDraftId, bulletin.LBulletinId);
    }

    [Fact]
    public void RequestApply_CardShift_ReordersAndRenumbersCards()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);
        long first = TRequestCardAdd(engine, started.LDraftId, 0);
        long second = TRequestCardAdd(engine, started.LDraftId, 1);
        long third = TRequestCardAdd(engine, started.LDraftId, 2);

        LDraft answered = engine.TEngineRequestApply(
            TInterface.TRequestShiftCreate(started.LDraftId, third, 0, 0));

        IReadOnlyList<LCardDraft> cards = answered.LDraftContent.LEntryDraftMeanings;
        Assert.Equal([third, first, second], cards.Select(card => card.LCardDraftId));
        Assert.Equal([1, 2, 3], cards.Select(card => card.LCardDraftPosition));
    }

    [Fact]
    public void RequestApply_CardRemoval_DropsCardAndRenumbersRest()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);
        long first = TRequestCardAdd(engine, started.LDraftId, 0);
        long second = TRequestCardAdd(engine, started.LDraftId, 1);

        LDraft answered = engine.TEngineRequestApply(TInterface.TRequestRemovalCreate(started.LDraftId, first));

        LCardDraft kept = Assert.Single(answered.LDraftContent.LEntryDraftMeanings);
        Assert.Equal(second, kept.LCardDraftId);
        Assert.Equal(1, kept.LCardDraftPosition);
    }

    [Fact]
    public void RequestApply_CardTitle_ChangesOnlyThatCard()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);
        long first = TRequestCardAdd(engine, started.LDraftId, 0);
        long second = TRequestCardAdd(engine, started.LDraftId, 1);

        LDraft answered = engine.TEngineRequestApply(
            TInterface.TRequestTitleCreate(started.LDraftId, second, TInterface.TStateValueCreate("set alight")));

        IReadOnlyList<LCardDraft> cards = answered.LDraftContent.LEntryDraftMeanings;
        Assert.Equal(first, cards[0].LCardDraftId);
        Assert.True(cards[0].LCardDraftTitle.LStateValueEmpty);
        Assert.Equal("set alight", cards[1].LCardDraftTitle.TStateValueShow());
    }

    [Fact]
    public void RequestApply_UnknownCard_Refuses()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);

        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineRequestApply(
            TInterface.TRequestTitleCreate(started.LDraftId, TInterface.TIdentityCreate(), "set alight")));

        Assert.Equal(LRefusal.LRefusalCard, refusal.LRefusalReason);
    }

    [Fact]
    public void RequestApply_ZeroCardId_Refuses()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);

        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineRequestApply(
            TInterface.TRequestRemovalCreate(started.LDraftId, 0)));

        Assert.Equal(LRefusal.LRefusalCard, refusal.LRefusalReason);
    }

    [Fact]
    public void RequestApply_CollocationParent_Refuses()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);
        LDraft answered = engine.TEngineRequestApply(
            TInterface.TRequestAdditionCreate(started.LDraftId, LCardKind.LCardKindCollocation, 0, 0));
        long parent = answered.LDraftContent.LEntryDraftCollocations[0].LCardDraftId;

        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineRequestApply(
            TInterface.TRequestAdditionCreate(started.LDraftId, LCardKind.LCardKindCollocation, parent, 0)));

        Assert.Equal(LRefusal.LRefusalCollocation, refusal.LRefusalReason);
    }

    [Fact]
    public void RequestApply_DraftNotHeld_Refuses()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineRequestApply(
            TInterface.TRequestHeadwordCreate(TInterface.TIdentityCreate(), "kindle")));

        Assert.Equal(LRefusal.LRefusalDraft, refusal.LRefusalReason);
    }

    private static long TRequestCardAdd(LEngine engine, long draftId, int position)
    {
        LDraft answered = engine.TEngineRequestApply(
            TInterface.TRequestAdditionCreate(draftId, LCardKind.LCardKindMeaning, 0, position));
        return answered.LDraftContent.LEntryDraftMeanings[position].LCardDraftId;
    }

    private sealed class TRequestObserver : LObserver
    {
        internal List<LBulletin> TRequestObserverBulletins { get; } = [];

        public void LObserverBulletinHandle(LBulletin bulletin)
        {
            TRequestObserverBulletins.Add(bulletin);
        }
    }
}
